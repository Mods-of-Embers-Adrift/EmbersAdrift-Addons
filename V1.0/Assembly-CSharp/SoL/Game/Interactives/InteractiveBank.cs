using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Logging;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B7F RID: 2943
	public class InteractiveBank : InteractiveRemoteContainer, ITooltip, IInteractiveBase
	{
		// Token: 0x1700152E RID: 5422
		// (get) Token: 0x06005AA9 RID: 23209 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool AllowInteractionWhileMissingBag
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700152F RID: 5423
		// (get) Token: 0x06005AAA RID: 23210 RVA: 0x00065D5E File Offset: 0x00063F5E
		private IEnumerable GetProfiles
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<BankProfile>();
			}
		}

		// Token: 0x06005AAB RID: 23211 RVA: 0x001ED2DC File Offset: 0x001EB4DC
		private string GetContainerId()
		{
			if (this.m_bankProfile == null)
			{
				throw new ArgumentNullException("m_bankProfile");
			}
			if (!(this.m_bankProfile != null) || !this.m_bankProfile.IsPersonalBank)
			{
				return this.m_bankProfile.Id;
			}
			return ContainerType.PersonalBank.ToString();
		}

		// Token: 0x06005AAC RID: 23212 RVA: 0x0007CE39 File Offset: 0x0007B039
		protected override void Awake()
		{
			if (this.m_bankProfile == null)
			{
				base.gameObject.SetActive(false);
				throw new ArgumentNullException("m_bankProfile");
			}
			base.Awake();
		}

		// Token: 0x06005AAD RID: 23213 RVA: 0x001ED340 File Offset: 0x001EB540
		public override bool CanInteract(GameEntity entity)
		{
			ContainerInstance containerInstance;
			return this.m_bankProfile != null && AccessFlagsExtensions.HasAccessToInteractive(entity, this.m_accessFlagRequirement) && base.CanInteract(entity) && !entity.CollectionController.TryGetInstance(this.m_bankProfile.Id, out containerInstance);
		}

		// Token: 0x06005AAE RID: 23214 RVA: 0x001ED394 File Offset: 0x001EB594
		public override void BeginInteraction(GameEntity interactionSource)
		{
			base.BeginInteraction(interactionSource);
			if (this.m_bankProfile == null)
			{
				return;
			}
			if (GameManager.IsServer)
			{
				ContainerRecord containerRecord = null;
				string userId = null;
				if (this.m_bankProfile.IsPersonalBank)
				{
					interactionSource.ServerPlayerController.Record.Storage.TryGetValue(ContainerType.PersonalBank, out containerRecord);
				}
				else
				{
					userId = interactionSource.ServerPlayerController.Record.UserId;
					containerRecord = ContainerRecord.Load(ExternalGameDatabase.Database, userId, this.m_bankProfile.Id);
					if (containerRecord != null)
					{
						bool flag = false;
						CharacterRecord record = interactionSource.ServerPlayerController.Record;
						for (int i = 0; i < containerRecord.Instances.Count; i++)
						{
							ArchetypeInstance archetypeInstance = containerRecord.Instances[i];
							if (!ContainerType.Bank.IsValidInstanceForContainer(archetypeInstance))
							{
								if (record.InvalidItems == null)
								{
									record.InvalidItems = new List<ArchetypeInstance>(10);
								}
								record.InvalidItems.Add(archetypeInstance);
								flag = true;
								BaseCollectionController.InvalidInstanceArguments[0] = record.Name;
								BaseCollectionController.InvalidInstanceArguments[1] = record.Id;
								BaseCollectionController.InvalidInstanceArguments[2] = archetypeInstance.ArchetypeId.Value;
								BaseCollectionController.InvalidInstanceArguments[3] = ContainerType.Bank.ToString();
								SolDebug.LogToIndex(LogLevel.Error, LogIndex.Error, "{@CharacterName} ({@CharacterId}) had an {@EventType} ({@ItemId}) in {@ContainerType}!", BaseCollectionController.InvalidInstanceArguments);
								containerRecord.Instances.RemoveAt(i);
								i--;
							}
						}
						if (flag)
						{
							record.UpdateInvalidItems(ExternalGameDatabase.Database);
							containerRecord.UpdateRecord(ExternalGameDatabase.Database);
						}
					}
				}
				if (containerRecord == null)
				{
					if (this.m_bankProfile.IsPersonalBank)
					{
						containerRecord = new ContainerRecord
						{
							ContainerId = this.GetContainerId(),
							Type = ContainerType.PersonalBank,
							Instances = new List<ArchetypeInstance>()
						};
						interactionSource.ServerPlayerController.Record.Storage.Add(ContainerType.PersonalBank, containerRecord);
					}
					else
					{
						containerRecord = new ContainerRecord
						{
							Id = ObjectId.GenerateNewId().ToString(),
							ContainerId = this.GetContainerId(),
							Type = ContainerType.Bank,
							UserId = userId,
							Instances = new List<ArchetypeInstance>()
						};
					}
				}
				base.OpenRemoteContainer(interactionSource, containerRecord);
			}
		}

		// Token: 0x06005AAF RID: 23215 RVA: 0x0007CE66 File Offset: 0x0007B066
		public override void EndInteraction(GameEntity interactionSource, bool clientIsEnding)
		{
			base.EndInteraction(interactionSource, clientIsEnding);
			base.CloseRemoteContainer(interactionSource, this.GetContainerId(), clientIsEnding);
		}

		// Token: 0x17001530 RID: 5424
		// (get) Token: 0x06005AB0 RID: 23216 RVA: 0x0007CE7E File Offset: 0x0007B07E
		protected override CursorType ActiveCursorType
		{
			get
			{
				return CursorType.MerchantCursor;
			}
		}

		// Token: 0x17001531 RID: 5425
		// (get) Token: 0x06005AB1 RID: 23217 RVA: 0x0007CE82 File Offset: 0x0007B082
		protected override CursorType InactiveCursorType
		{
			get
			{
				return CursorType.MerchantCursorInactive;
			}
		}

		// Token: 0x06005AB2 RID: 23218 RVA: 0x001ED5B4 File Offset: 0x001EB7B4
		private ITooltipParameter GetTooltipParameter()
		{
			BaseTooltip.Sb.Clear();
			BaseTooltip.Sb.AppendLine(this.m_bankProfile.DisplayName);
			PlayerFlags value = LocalPlayer.GameEntity.CharacterData.CharacterFlags.Value;
			if (value.BlockRemoteContainerInteractions(base.PreventInteractionsWhileMissingBag, false))
			{
				if (base.PreventInteractionsWhileMissingBag && value.HasBitFlag(PlayerFlags.MissingBag))
				{
					BaseTooltip.Sb.AppendLine("You do not have your bag!");
				}
				if (value.HasBitFlag(PlayerFlags.InTrade))
				{
					BaseTooltip.Sb.AppendLine("You are currently part of a trade!");
				}
			}
			if (this.m_accessFlagRequirement.HasBitFlag(AccessFlags.FullClient) && SessionData.User != null && SessionData.User.IsTrial())
			{
				BaseTooltip.Sb.AppendLine("(Purchase Required)");
			}
			return new ObjectTextTooltipParameter(this, BaseTooltip.Sb.ToString(), false);
		}

		// Token: 0x17001532 RID: 5426
		// (get) Token: 0x06005AB3 RID: 23219 RVA: 0x0007CE86 File Offset: 0x0007B086
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17001533 RID: 5427
		// (get) Token: 0x06005AB4 RID: 23220 RVA: 0x0007CE94 File Offset: 0x0007B094
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06005AB6 RID: 23222 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004F8C RID: 20364
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04004F8D RID: 20365
		[SerializeField]
		private BankProfile m_bankProfile;

		// Token: 0x04004F8E RID: 20366
		[SerializeField]
		private AccessFlags m_accessFlagRequirement;
	}
}
