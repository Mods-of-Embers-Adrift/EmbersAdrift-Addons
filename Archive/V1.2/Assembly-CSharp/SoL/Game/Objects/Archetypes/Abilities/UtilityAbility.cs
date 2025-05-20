using System;
using System.Collections.Generic;
using ENet;
using NetStack.Serialization;
using SoL.Game.Crafting;
using SoL.Game.EffectSystem;
using SoL.Game.Interactives;
using SoL.Game.Messages;
using SoL.Game.Player;
using SoL.Game.Spawning;
using SoL.Game.Transactions;
using SoL.Managers;
using SoL.Networking;
using SoL.Networking.Objects;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes.Abilities
{
	// Token: 0x02000AFC RID: 2812
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Abilities/Utility Ability")]
	public class UtilityAbility : AbilityArchetype
	{
		// Token: 0x17001469 RID: 5225
		// (get) Token: 0x060056FB RID: 22267 RVA: 0x00079EBE File Offset: 0x000780BE
		private bool m_showDuration
		{
			get
			{
				return this.m_function == UtilityFunction.StartFire || this.m_function == UtilityFunction.Survey;
			}
		}

		// Token: 0x1700146A RID: 5226
		// (get) Token: 0x060056FC RID: 22268 RVA: 0x00045BCA File Offset: 0x00043DCA
		public override bool ConsiderHaste
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700146B RID: 5227
		// (get) Token: 0x060056FD RID: 22269 RVA: 0x00045BCA File Offset: 0x00043DCA
		public override bool AllowAlchemy
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700146C RID: 5228
		// (get) Token: 0x060056FE RID: 22270 RVA: 0x00079ED4 File Offset: 0x000780D4
		protected override float m_creditFactor
		{
			get
			{
				if (this.m_function == UtilityFunction.Survey)
				{
					return 0f;
				}
				return base.m_creditFactor;
			}
		}

		// Token: 0x060056FF RID: 22271 RVA: 0x001E1A98 File Offset: 0x001DFC98
		protected override bool ExecutionCheck(ExecutionCache executionCache, float executionProgress)
		{
			if (!base.ExecutionCheck(executionCache, executionProgress))
			{
				return false;
			}
			executionCache.SetTargetNetworkEntity(executionCache.SourceEntity.NetworkEntity);
			switch (this.m_function)
			{
			case UtilityFunction.SenseHeading:
			case UtilityFunction.Survey:
				return true;
			case UtilityFunction.StartFire:
			{
				Collider[] colliders = Hits.Colliders5;
				int num = Physics.OverlapSphereNonAlloc(executionCache.SourceEntity.gameObject.transform.position, 5f, colliders, LayerMap.Detection.LayerMask, QueryTriggerInteraction.Ignore);
				int i = 0;
				while (i < num)
				{
					LightableCampfire component = colliders[i].gameObject.GetComponent<LightableCampfire>();
					if (component != null)
					{
						if (component.IsLit)
						{
							executionCache.Message = "Already lit!";
							return false;
						}
						executionCache.SetTargetNetworkEntity(component.GameEntity.NetworkEntity);
						return true;
					}
					else
					{
						i++;
					}
				}
				executionCache.Message = "Nothing to light!";
				break;
			}
			case UtilityFunction.Forage:
				if (executionCache.SourceEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.MissingBag))
				{
					executionCache.Message = "You cannot forage while missing your bag.";
					return false;
				}
				return true;
			}
			return false;
		}

		// Token: 0x06005700 RID: 22272 RVA: 0x001E1BA8 File Offset: 0x001DFDA8
		protected override void PostExecution(ExecutionCache executionCache)
		{
			base.PostExecution(executionCache);
			if (!GameManager.IsServer)
			{
				if (this.m_function == UtilityFunction.Survey)
				{
					float associatedLevel = executionCache.MasteryInstance.GetAssociatedLevel(executionCache.SourceEntity);
					float scaledValueForMasteryLevel = this.m_fractionRange.GetScaledValueForMasteryLevel(base.LevelRange, associatedLevel);
					this.m_durationRange.GetScaledValueForMasteryLevel(base.LevelRange, associatedLevel);
					Collider[] colliders = Hits.Colliders100;
					int num = Physics.OverlapSphereNonAlloc(LocalPlayer.GameEntity.gameObject.transform.position, scaledValueForMasteryLevel, colliders, LayerMap.Interaction.LayerMask, QueryTriggerInteraction.Ignore);
					int num2 = 0;
					for (int i = 0; i < num; i++)
					{
						InteractiveGatheringNode component = colliders[i].gameObject.GetComponent<InteractiveGatheringNode>();
						if (component == null)
						{
							GameEntityComponent component2 = colliders[i].gameObject.GetComponent<GameEntityComponent>();
							if (component2 != null)
							{
								component = component2.GameEntity.GetComponent<InteractiveGatheringNode>();
							}
						}
						component != null;
					}
					string text = Mathf.FloorToInt(scaledValueForMasteryLevel).ToString();
					string content = (num2 > 0) ? string.Format("You survey {0} nearby nodes within {1}m.", num2, text) : ("Your survey reveals no nearby nodes within " + text + "m.");
					MessageManager.ChatQueue.AddToQueue(MessageType.Skills, content);
				}
				return;
			}
			UtilityResult utilityResult = new UtilityResult
			{
				SendType = UtilityResultSendType.None,
				Function = this.m_function
			};
			float num3 = executionCache.MasteryInstance.GetAssociatedLevel(executionCache.SourceEntity) / 50f;
			switch (this.m_function)
			{
			case UtilityFunction.SenseHeading:
			{
				float associatedLevel2 = executionCache.MasteryInstance.GetAssociatedLevel(executionCache.SourceEntity);
				float num4 = 180f * (1f - num3);
				float y = executionCache.SourceEntity.gameObject.transform.eulerAngles.y;
				float num5 = UnityEngine.Random.Range(y - num4, y + num4);
				if (num5 > 360f)
				{
					num5 -= 360f;
				}
				else if (num5 < 0f)
				{
					num5 += 360f;
				}
				int num6 = (int)(num5 / 45f + 0.5f);
				if (num6 == UtilityAbility.SenseHeadingData.kDirections.Length)
				{
					num6 = 0;
				}
				int value;
				if (associatedLevel2 < 25f)
				{
					value = 0;
				}
				else if (associatedLevel2 < 50f)
				{
					value = 1;
				}
				else if (associatedLevel2 < 75f)
				{
					value = 2;
				}
				else if (associatedLevel2 < 100f)
				{
					value = 3;
				}
				else
				{
					value = 4;
				}
				utilityResult.SendType = UtilityResultSendType.Source;
				utilityResult.Index0 = new int?(num6);
				utilityResult.Index1 = new int?(value);
				break;
			}
			case UtilityFunction.StartFire:
			{
				LightableCampfire component3 = executionCache.TargetNetworkEntity.GetComponent<LightableCampfire>();
				if (component3 != null)
				{
					int associatedLevelInteger = executionCache.MasteryInstance.GetAssociatedLevelInteger(executionCache.SourceEntity);
					float f = component3.LightFire(executionCache.SourceEntity, associatedLevelInteger);
					utilityResult.SendType = UtilityResultSendType.Area;
					utilityResult.Index0 = new int?(associatedLevelInteger);
					utilityResult.Index1 = new int?(Mathf.FloorToInt(f));
				}
				break;
			}
			case UtilityFunction.Forage:
			{
				float num7 = SolMath.Gaussian() + 1f * num3;
				float x = executionCache.SourceEntity.NetworkEntity.NetworkId.Value * executionCache.SourceEntity.gameObject.transform.position.x + Time.time;
				float y2 = executionCache.SourceEntity.NetworkEntity.NetworkId.Value * executionCache.SourceEntity.gameObject.transform.position.z + Time.time;
				float num8 = Mathf.PerlinNoise(x, y2);
				float num9 = (1f - num8) * Mathf.Abs(num7);
				HitType hitTypeForRoll = HitTypeExtensions.GetHitTypeForRoll(num7 - num9);
				utilityResult.SendType = UtilityResultSendType.Source;
				utilityResult.Index0 = new int?((hitTypeForRoll == HitType.Miss) ? 0 : 1);
				if (hitTypeForRoll - HitType.Heavy <= 1)
				{
					ItemArchetypeProbabilityCollection forageItems = this.m_forageItems;
					ItemArchetypeProbabilityEntry itemArchetypeProbabilityEntry = (forageItems != null) ? forageItems.GetEntry(null, false) : null;
					if (itemArchetypeProbabilityEntry != null && itemArchetypeProbabilityEntry.Obj != null)
					{
						ItemArchetype obj = itemArchetypeProbabilityEntry.Obj;
						ArchetypeInstance archetypeInstance = executionCache.SourceEntity.CollectionController.AddItemToPlayer(obj, ItemAddContext.Forage, 1, -1, ItemFlags.None, false);
						if (archetypeInstance == null)
						{
							utilityResult.Index0 = new int?(0);
							utilityResult.Text = "No room";
						}
						else
						{
							utilityResult.Text = obj.GetModifiedDisplayName(archetypeInstance);
						}
					}
				}
				else
				{
					utilityResult.Index0 = new int?(0);
				}
				break;
			}
			}
			if (utilityResult.SendType != UtilityResultSendType.None)
			{
				BitBuffer fromPool = BitBufferExtensions.GetFromPool();
				fromPool.AddHeader(executionCache.SourceEntity.NetworkEntity, OpCodes.ChatMessage, true);
				utilityResult.PackData(fromPool);
				Packet packetFromBuffer_ReturnBufferToPool = fromPool.GetPacketFromBuffer_ReturnBufferToPool(PacketFlags.Reliable);
				NetworkCommand networkCommand = NetworkCommandPool.GetNetworkCommand();
				networkCommand.Channel = NetworkChannel.UtilityResult;
				networkCommand.Packet = packetFromBuffer_ReturnBufferToPool;
				switch (utilityResult.SendType)
				{
				case UtilityResultSendType.Area:
				{
					UtilityAbility.m_nearbyEntities.Clear();
					ServerGameManager.SpatialManager.QueryRadius(executionCache.SourceEntity.gameObject.transform.position, 30f, UtilityAbility.m_nearbyQuery);
					bool flag = false;
					for (int j = 0; j < UtilityAbility.m_nearbyQuery.Count; j++)
					{
						flag = (flag || UtilityAbility.m_nearbyQuery[j] == executionCache.SourceEntity.NetworkEntity);
						if (UtilityAbility.m_nearbyQuery[j] != null && UtilityAbility.m_nearbyQuery[j].GameEntity.Type == GameEntityType.Player)
						{
							UtilityAbility.m_nearbyEntities.Add(UtilityAbility.m_nearbyQuery[j]);
						}
					}
					Peer[] array = PeerArrayPool.GetArray(flag ? UtilityAbility.m_nearbyEntities.Count : (UtilityAbility.m_nearbyEntities.Count + 1));
					for (int k = 0; k < UtilityAbility.m_nearbyEntities.Count; k++)
					{
						array[k] = UtilityAbility.m_nearbyEntities[k].NetworkId.Peer;
					}
					if (!flag)
					{
						array[UtilityAbility.m_nearbyEntities.Count] = executionCache.SourceEntity.NetworkEntity.NetworkId.Peer;
					}
					UtilityAbility.m_nearbyEntities.Clear();
					networkCommand.Type = CommandType.BroadcastGroup;
					networkCommand.TargetGroup = array;
					goto IL_752;
				}
				case UtilityResultSendType.Group:
					if (executionCache.SourceEntity.CharacterData.GroupMembersNearby > 0)
					{
						Peer[] array2 = PeerArrayPool.GetArray(executionCache.SourceEntity.CharacterData.GroupMembersNearby + 1);
						for (int l = 0; l < executionCache.SourceEntity.CharacterData.NearbyGroupMembers.Count; l++)
						{
							array2[l] = executionCache.SourceEntity.CharacterData.NearbyGroupMembers[l].NetworkEntity.NetworkId.Peer;
						}
						array2[executionCache.SourceEntity.CharacterData.NearbyGroupMembers.Count] = executionCache.SourceEntity.NetworkEntity.NetworkId.Peer;
						networkCommand.Type = CommandType.BroadcastGroup;
						networkCommand.TargetGroup = array2;
						goto IL_752;
					}
					networkCommand.Type = CommandType.Send;
					networkCommand.Target = executionCache.SourceEntity.NetworkEntity.NetworkId.Peer;
					goto IL_752;
				}
				networkCommand.Type = CommandType.Send;
				networkCommand.Target = executionCache.SourceEntity.NetworkEntity.NetworkId.Peer;
				IL_752:
				GameManager.NetworkManager.AddCommandToQueue(networkCommand);
			}
		}

		// Token: 0x06005701 RID: 22273 RVA: 0x001E2314 File Offset: 0x001E0514
		public static void ProcessUtilityAbilityResult(NetworkEntity sourceEntity, UtilityResult result)
		{
			MessageType messageType = MessageType.None;
			string content = string.Empty;
			switch (result.Function)
			{
			case UtilityFunction.SenseHeading:
				messageType = MessageType.Skills;
				content = UtilityAbility.SenseHeadingData.kConfidence[result.Index1.Value] + " " + UtilityAbility.SenseHeadingData.kDirections[result.Index0.Value];
				break;
			case UtilityFunction.StartFire:
			{
				string text = (sourceEntity == LocalPlayer.NetworkEntity) ? "You have" : (sourceEntity.GameEntity.CharacterData.Name.Value + " has");
				int num = (result.Index0 != null) ? result.Index0.Value : -1;
				int value = (result.Index1 != null) ? result.Index1.Value : -1;
				messageType = MessageType.Skills;
				content = string.Concat(new string[]
				{
					text,
					" started a +",
					num.ToString(),
					" fire for ",
					value.GetFormattedTime(true)
				});
				break;
			}
			case UtilityFunction.Forage:
			{
				int value2 = result.Index0.Value;
				if (value2 != 0)
				{
					if (value2 == 1)
					{
						messageType = MessageType.Skills;
						content = "Your foraging attempt turns up " + result.Text + ".";
					}
				}
				else
				{
					string str = string.IsNullOrEmpty(result.Text) ? "" : ("(" + result.Text + ")");
					messageType = MessageType.Skills;
					content = "Your foraging attempt fails! " + str;
				}
				break;
			}
			}
			if (messageType != MessageType.None)
			{
				MessageManager.ChatQueue.AddToQueue(messageType, content);
			}
		}

		// Token: 0x06005702 RID: 22274 RVA: 0x001E24A8 File Offset: 0x001E06A8
		protected override void FillTooltipBlocksInternal(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity, int masteryLevel, int abilityLevel)
		{
			base.FillTooltipBlocksInternal(tooltip, instance, entity, masteryLevel, abilityLevel);
			TooltipTextBlock effectsBlock = tooltip.EffectsBlock;
			if (this.m_function == UtilityFunction.Survey)
			{
				float scaledValueForMasteryLevel = this.m_fractionRange.GetScaledValueForMasteryLevel(base.LevelRange, (float)masteryLevel);
				float scaledValueForMasteryLevel2 = this.m_durationRange.GetScaledValueForMasteryLevel(base.LevelRange, (float)masteryLevel);
				effectsBlock.AppendLine("Range", scaledValueForMasteryLevel.FormattedArmorClass() + " m");
				effectsBlock.AppendLine("Duration", scaledValueForMasteryLevel2.GetFormattedTime(true) ?? "");
			}
		}

		// Token: 0x04004CB5 RID: 19637
		[SerializeField]
		private UtilityFunction m_function;

		// Token: 0x04004CB6 RID: 19638
		[SerializeField]
		private MinMaxFloatRange m_durationRange = new MinMaxFloatRange(0f, 0f);

		// Token: 0x04004CB7 RID: 19639
		[SerializeField]
		private MinMaxFloatRange m_fractionRange = new MinMaxFloatRange(0f, 0f);

		// Token: 0x04004CB8 RID: 19640
		[SerializeField]
		private ItemArchetypeProbabilityCollection m_forageItems;

		// Token: 0x04004CB9 RID: 19641
		private static readonly List<NetworkEntity> m_nearbyQuery = new List<NetworkEntity>();

		// Token: 0x04004CBA RID: 19642
		private static readonly List<NetworkEntity> m_nearbyEntities = new List<NetworkEntity>();

		// Token: 0x02000AFD RID: 2813
		private static class SenseHeadingData
		{
			// Token: 0x04004CBB RID: 19643
			public static string[] kDirections = new string[]
			{
				"North",
				"North East",
				"East",
				"South East",
				"South",
				"South West",
				"West",
				"North West"
			};

			// Token: 0x04004CBC RID: 19644
			public static string[] kConfidence = new string[]
			{
				"I could be facing",
				"I am likely facing",
				"I should be facing",
				"I am most likely facing",
				"I am facing"
			};
		}
	}
}
