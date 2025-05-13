using System;
using ENet;
using SoL.Game.Grouping;
using SoL.Game.Interactives;
using SoL.Game.Pooling;
using SoL.Game.SkyDome;
using SoL.Managers;
using SoL.Networking.Objects;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A62 RID: 2658
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Consumables/Stackable Utility")]
	public class ConsumableItemUtility : ConsumableItemStackable, ILocationEvent
	{
		// Token: 0x06005265 RID: 21093 RVA: 0x00076FEE File Offset: 0x000751EE
		private bool HasBitFlag(ConsumableItemUtility.LevelUpType a, ConsumableItemUtility.LevelUpType b)
		{
			return (a & b) == b;
		}

		// Token: 0x06005266 RID: 21094 RVA: 0x001D365C File Offset: 0x001D185C
		protected override bool ExecutionCheckInternal(ExecutionCache executionCache)
		{
			if (!base.ExecutionCheckInternal(executionCache))
			{
				return false;
			}
			ConsumableItemUtility.UtilityType type = this.m_type;
			switch (type)
			{
			case ConsumableItemUtility.UtilityType.GroundTorch:
			{
				GroundTorch nearbyGroundTorch = InteractiveExtensions.GetNearbyGroundTorch(executionCache.SourceEntity.gameObject.transform.position, 20f);
				if (nearbyGroundTorch != null && !this.CanReplaceTorch(nearbyGroundTorch, executionCache.SourceEntity))
				{
					executionCache.Message = "Another ground torch is nearby!";
					return false;
				}
				return true;
			}
			case ConsumableItemUtility.UtilityType.FireStarter:
			{
				Collider[] colliders = Hits.Colliders50;
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
				return false;
			}
			case ConsumableItemUtility.UtilityType.Fireworks:
				if (SkyDomeManager.SkyDomeController != null && SkyDomeManager.SkyDomeController.IsIndoors)
				{
					executionCache.Message = "Cannot use indoors!";
					return false;
				}
				return true;
			default:
				if (type != ConsumableItemUtility.UtilityType.LevelUp)
				{
					executionCache.Message = "Unknown Error";
					return false;
				}
				return true;
			}
		}

		// Token: 0x06005267 RID: 21095 RVA: 0x001D37B0 File Offset: 0x001D19B0
		protected override void PostExecution(ExecutionCache executionCache)
		{
			base.PostExecution(executionCache);
			if (GameManager.IsServer)
			{
				ConsumableItemUtility.UtilityType type = this.m_type;
				switch (type)
				{
				case ConsumableItemUtility.UtilityType.GroundTorch:
				{
					GroundTorch groundTorch = InteractiveExtensions.GetNearbyGroundTorch(executionCache.SourceEntity.gameObject.transform.position, 20f);
					if (groundTorch != null && !this.CanReplaceTorch(groundTorch, executionCache.SourceEntity))
					{
						executionCache.Message = "Another ground torch is nearby!";
						return;
					}
					NetworkEntity networkEntity = null;
					if (groundTorch == null)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_groundTorchPrefab);
						networkEntity = gameObject.GetComponent<NetworkEntity>();
						networkEntity.ServerInit(default(Peer), true, false);
						groundTorch = gameObject.GetComponent<GroundTorch>();
					}
					if (executionCache.SourceEntity.GroundTorch != null && executionCache.SourceEntity.GroundTorch != groundTorch)
					{
						UnityEngine.Object.Destroy(executionCache.SourceEntity.GroundTorch.gameObject);
					}
					groundTorch.Initialize(executionCache.SourceEntity, this.m_groundTorchDuration);
					if (networkEntity != null)
					{
						ServerGameManager.ServerNetworkEntityManager.SpawnNetworkEntityForRemoteClients(networkEntity);
						return;
					}
					break;
				}
				case ConsumableItemUtility.UtilityType.FireStarter:
				{
					LightableCampfire component = executionCache.TargetNetworkEntity.GetComponent<LightableCampfire>();
					if (component)
					{
						component.LightFire(executionCache.SourceEntity, 1);
						return;
					}
					break;
				}
				case ConsumableItemUtility.UtilityType.Fireworks:
				{
					LocationEvent locationEvent = new LocationEvent
					{
						ArchetypeId = base.Id,
						Location = executionCache.SourceEntity.gameObject.transform.position
					};
					ServerGameManager.SpatialManager.LocationEventAllPlayers(executionCache.SourceEntity, locationEvent);
					return;
				}
				default:
					if (type != ConsumableItemUtility.UtilityType.LevelUp)
					{
						return;
					}
					if (executionCache.SourceEntity && executionCache.SourceEntity.CollectionController != null && executionCache.SourceEntity.CollectionController.Masteries != null)
					{
						foreach (ArchetypeInstance archetypeInstance in executionCache.SourceEntity.CollectionController.Masteries.Instances)
						{
							if (!(archetypeInstance.Mastery == null))
							{
								switch (archetypeInstance.Mastery.Type)
								{
								case MasteryType.Combat:
									if (!this.HasBitFlag(this.m_levelUpType, ConsumableItemUtility.LevelUpType.Adventuring))
									{
										continue;
									}
									break;
								case MasteryType.Trade:
									if (!this.HasBitFlag(this.m_levelUpType, ConsumableItemUtility.LevelUpType.Trade))
									{
										continue;
									}
									break;
								case MasteryType.Harvesting:
									if (!this.HasBitFlag(this.m_levelUpType, ConsumableItemUtility.LevelUpType.Gathering))
									{
										continue;
									}
									break;
								}
								bool flag = false;
								float baseLevel = archetypeInstance.MasteryData.BaseLevel;
								LevelProgressionUpdate levelProgressionUpdate = new LevelProgressionUpdate
								{
									ArchetypeId = archetypeInstance.ArchetypeId,
									BaseLevel = (float)this.m_targetLevel
								};
								if (DeploymentBranchFlagsExtensions.IsQA())
								{
									flag = true;
									archetypeInstance.MasteryData.BaseLevel = (float)this.m_targetLevel;
									if (archetypeInstance.MasteryData.Specialization != null)
									{
										levelProgressionUpdate.SpecializationLevel = new float?((float)this.m_targetLevel);
										archetypeInstance.MasteryData.SpecializationLevel = (float)this.m_targetLevel;
									}
								}
								else
								{
									if (baseLevel < (float)this.m_targetLevel)
									{
										archetypeInstance.MasteryData.BaseLevel = (float)this.m_targetLevel;
										flag = true;
									}
									if (archetypeInstance.MasteryData.Specialization != null && archetypeInstance.MasteryData.SpecializationLevel < (float)this.m_targetLevel)
									{
										levelProgressionUpdate.SpecializationLevel = new float?((float)this.m_targetLevel);
										archetypeInstance.MasteryData.SpecializationLevel = (float)this.m_targetLevel;
										flag = true;
									}
								}
								if (flag)
								{
									executionCache.SourceEntity.Vitals.MasteryLevelChanged(archetypeInstance, baseLevel, (float)this.m_targetLevel);
									executionCache.SourceEntity.CharacterData.UpdateHighestMasteryLevel(archetypeInstance);
									executionCache.SourceEntity.NetworkEntity.PlayerRpcHandler.Server_LevelProgressionUpdate(levelProgressionUpdate);
								}
							}
						}
					}
					break;
				}
			}
		}

		// Token: 0x06005268 RID: 21096 RVA: 0x001D3BA4 File Offset: 0x001D1DA4
		private bool CanReplaceTorch(GroundTorch torch, GameEntity sourceEntity)
		{
			bool result = false;
			GroupMember groupMember;
			if (GameManager.IsServer)
			{
				if (torch.CurrentSource == null)
				{
					result = true;
				}
				else if (torch.CurrentSource == sourceEntity)
				{
					result = true;
				}
				else if (!torch.CurrentSource.CharacterData.GroupId.IsEmpty && torch.CurrentSource.CharacterData.GroupId == sourceEntity.CharacterData.GroupId)
				{
					result = true;
				}
			}
			else if (torch.TorchData.SourceName == sourceEntity.CharacterData.Name.Value)
			{
				result = true;
			}
			else if (ClientGameManager.GroupManager.IsGrouped && ClientGameManager.GroupManager.TryGetGroupMember(torch.TorchData.SourceName, out groupMember))
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06005269 RID: 21097 RVA: 0x00076FF6 File Offset: 0x000751F6
		public void ExecuteLocationEvent(Vector3 location)
		{
			if (this.m_type == ConsumableItemUtility.UtilityType.Fireworks && this.m_fireworksVFX)
			{
				this.m_fireworksVFX.GetPooledInstance<PooledVFX>().InitializeSimple(null, location, Quaternion.identity, new float?((float)20));
			}
		}

		// Token: 0x0600526A RID: 21098 RVA: 0x001D3C74 File Offset: 0x001D1E74
		public override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			base.FillTooltipBlocks(tooltip, instance, entity);
			TooltipTextBlock dataBlock = tooltip.DataBlock;
			ConsumableItemUtility.UtilityType type = this.m_type;
			if (type == ConsumableItemUtility.UtilityType.GroundTorch)
			{
				dataBlock.AppendLine("Duration:", this.m_groundTorchDuration.GetFormattedTime(false));
				return;
			}
			if (type != ConsumableItemUtility.UtilityType.LevelUp)
			{
				return;
			}
			dataBlock.AppendLine("Bumps your role, specialization, and professions to level " + this.m_targetLevel.ToString(), 0);
		}

		// Token: 0x040049AA RID: 18858
		public const float kGroundTorchRange = 20f;

		// Token: 0x040049AB RID: 18859
		public const float kFireStarterRange = 5f;

		// Token: 0x040049AC RID: 18860
		[SerializeField]
		private ConsumableItemUtility.UtilityType m_type;

		// Token: 0x040049AD RID: 18861
		[SerializeField]
		private GameObject m_groundTorchPrefab;

		// Token: 0x040049AE RID: 18862
		[SerializeField]
		private int m_groundTorchDuration = 60;

		// Token: 0x040049AF RID: 18863
		[SerializeField]
		private PooledVFX m_fireworksVFX;

		// Token: 0x040049B0 RID: 18864
		[SerializeField]
		private ConsumableItemUtility.LevelUpType m_levelUpType = ConsumableItemUtility.LevelUpType.All;

		// Token: 0x040049B1 RID: 18865
		[Range(1f, 50f)]
		[SerializeField]
		private int m_targetLevel = 6;

		// Token: 0x02000A63 RID: 2659
		private enum UtilityType
		{
			// Token: 0x040049B3 RID: 18867
			None,
			// Token: 0x040049B4 RID: 18868
			GroundTorch,
			// Token: 0x040049B5 RID: 18869
			FireStarter,
			// Token: 0x040049B6 RID: 18870
			Fireworks,
			// Token: 0x040049B7 RID: 18871
			LevelUp = 10
		}

		// Token: 0x02000A64 RID: 2660
		[Flags]
		private enum LevelUpType
		{
			// Token: 0x040049B9 RID: 18873
			None = 0,
			// Token: 0x040049BA RID: 18874
			Adventuring = 1,
			// Token: 0x040049BB RID: 18875
			Gathering = 2,
			// Token: 0x040049BC RID: 18876
			Trade = 4,
			// Token: 0x040049BD RID: 18877
			All = 7
		}
	}
}
