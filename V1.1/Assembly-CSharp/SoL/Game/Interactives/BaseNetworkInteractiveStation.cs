using System;
using SoL.Game.Objects.Containers;
using SoL.Game.Transactions;
using SoL.Managers;
using SoL.Networking;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B79 RID: 2937
	public abstract class BaseNetworkInteractiveStation : BaseNetworkedInteractive, ITooltip, IInteractiveBase
	{
		// Token: 0x17001513 RID: 5395
		// (get) Token: 0x06005A74 RID: 23156 RVA: 0x0007CBE5 File Offset: 0x0007ADE5
		public Action HideCallback
		{
			get
			{
				return this.m_hideCallback;
			}
		}

		// Token: 0x17001514 RID: 5396
		// (get) Token: 0x06005A75 RID: 23157 RVA: 0x0007CBED File Offset: 0x0007ADED
		public ContainerType ContainerType
		{
			get
			{
				return this.m_containerType;
			}
		}

		// Token: 0x17001515 RID: 5397
		// (get) Token: 0x06005A76 RID: 23158
		protected abstract string m_tooltipText { get; }

		// Token: 0x17001516 RID: 5398
		// (get) Token: 0x06005A77 RID: 23159
		protected abstract ContainerType m_containerType { get; }

		// Token: 0x17001517 RID: 5399
		// (get) Token: 0x06005A78 RID: 23160 RVA: 0x0007CBF5 File Offset: 0x0007ADF5
		public virtual string CurrencyRemovalMessage { get; }

		// Token: 0x06005A79 RID: 23161 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool UseEventCurrency()
		{
			return false;
		}

		// Token: 0x06005A7A RID: 23162 RVA: 0x0007CBFD File Offset: 0x0007ADFD
		protected override void Awake()
		{
			if (!this.EnabledForBranch())
			{
				base.gameObject.SetActive(false);
				return;
			}
			base.Awake();
		}

		// Token: 0x06005A7B RID: 23163 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void EndInteractionInternal(GameEntity interactionSource, ContainerInstance inventory)
		{
		}

		// Token: 0x06005A7C RID: 23164 RVA: 0x001ECC5C File Offset: 0x001EAE5C
		protected bool EnabledForBranch()
		{
			if (this.m_branchFlags == DeploymentBranchFlags.None)
			{
				return true;
			}
			DeploymentBranchFlags branchFlags = DeploymentBranchFlagsExtensions.GetBranchFlags();
			return branchFlags == DeploymentBranchFlags.None || this.m_branchFlags.HasBitFlag(branchFlags);
		}

		// Token: 0x06005A7D RID: 23165 RVA: 0x001ECC8C File Offset: 0x001EAE8C
		public sealed override bool CanInteract(GameEntity entity)
		{
			return this.EnabledForBranch() && entity != null && AccessFlagsExtensions.HasAccessToInteractive(entity, this.m_accessFlagRequirement) && entity.CharacterData != null && !entity.CharacterData.CharacterFlags.Value.BlockRemoteContainerInteractions(base.PreventInteractionsWhileMissingBag, this.m_notifyOnCanInteractFail) && this.m_rewardRequirement.ClientMeetsRewardRequirement(entity) && base.CanInteract(entity);
		}

		// Token: 0x06005A7E RID: 23166 RVA: 0x001ECD00 File Offset: 0x001EAF00
		public override void BeginInteraction(GameEntity interactionSource)
		{
			bool flag;
			if (GameManager.IsServer)
			{
				interactionSource.NetworkEntity.PlayerRpcHandler.Server_RequestInteraction(base.GameEntity.NetworkEntity);
				interactionSource.CharacterData.CharacterFlags.Value |= PlayerFlags.RemoteContainer;
				interactionSource.CollectionController.InteractiveStation = this;
				flag = true;
			}
			else
			{
				if (this.m_hideCallback == null)
				{
					this.m_hideCallback = new Action(this.LocalClientHide);
				}
				interactionSource.CollectionController.InteractiveStation = this;
				flag = true;
			}
			if (flag)
			{
				base.BeginInteraction(interactionSource);
			}
		}

		// Token: 0x06005A7F RID: 23167 RVA: 0x001ECD8C File Offset: 0x001EAF8C
		public override void EndInteraction(GameEntity interactionSource, bool clientIsEnding)
		{
			if (GameManager.IsServer)
			{
				interactionSource.CharacterData.CharacterFlags.Value &= ~PlayerFlags.RemoteContainer;
				interactionSource.CollectionController.InteractiveStation = null;
				ContainerInstance containerInstance = null;
				ContainerInstance containerInstance2;
				if (this.m_containerType.TransferContentsToInventoryOnEndInteraction() && interactionSource.CollectionController.TryGetInstance(this.m_containerType, out containerInstance2) && containerInstance2.Count > 0 && interactionSource.CollectionController.TryGetInstance(ContainerType.Inventory, out containerInstance) && (this.m_containerType.RequiresSymbolicLinkForPlacement() || containerInstance2.Count + containerInstance.Count <= containerInstance.GetMaxCapacity()))
				{
					TakeAllResponse response = containerInstance2.MoveContentsToContainerInstance(containerInstance, false, this.m_containerType.RequiresSymbolicLinkForPlacement());
					response.Op = OpCodes.Ok;
					response.SourceContainerId = containerInstance2.Id;
					interactionSource.CollectionController.UpdateOutgoingCurrency(containerInstance2.ContainerType);
					if (interactionSource.NetworkEntity.PlayerRpcHandler)
					{
						interactionSource.NetworkEntity.PlayerRpcHandler.TakeAllRequestResponse(response);
					}
				}
				this.EndInteractionInternal(interactionSource, containerInstance);
				if (!clientIsEnding && interactionSource.NetworkEntity && interactionSource.NetworkEntity.PlayerRpcHandler)
				{
					interactionSource.NetworkEntity.PlayerRpcHandler.Server_CancelInteraction(base.GameEntity.NetworkEntity);
				}
			}
			else
			{
				interactionSource.CollectionController.InteractiveStation = null;
				if (clientIsEnding)
				{
					interactionSource.NetworkEntity.PlayerRpcHandler.Client_CancelInteraction(base.GameEntity.NetworkEntity);
				}
			}
			base.EndInteraction(interactionSource, clientIsEnding);
		}

		// Token: 0x06005A80 RID: 23168 RVA: 0x0007CC1A File Offset: 0x0007AE1A
		private void LocalClientHide()
		{
			this.EndInteraction(LocalPlayer.GameEntity, true);
		}

		// Token: 0x06005A81 RID: 23169 RVA: 0x001ECF08 File Offset: 0x001EB108
		public bool TryRemoveCurrency(GameEntity entity, ulong currencyToRemove)
		{
			if (currencyToRemove <= 0UL)
			{
				return false;
			}
			CurrencySources currencySources;
			ulong availableCurrency = this.GetAvailableCurrency(entity, out currencySources);
			if (availableCurrency <= 0UL || availableCurrency < currencyToRemove)
			{
				return false;
			}
			if (!this.UseEventCurrency())
			{
				ContainerInstance inventory = entity.CollectionController.Inventory;
				ContainerInstance personalBank = entity.CollectionController.PersonalBank;
				if (!entity.IsMissingBag)
				{
					if (inventory.Currency >= currencyToRemove)
					{
						inventory.RemoveCurrency(currencyToRemove);
						entity.NetworkEntity.PlayerRpcHandler.ProcessInteractiveStationCurrencyTransaction(new ulong?(inventory.Currency), null);
						return true;
					}
					ulong num = currencyToRemove - inventory.Currency;
					if (personalBank.Currency >= num)
					{
						if (inventory.Currency > 0UL)
						{
							inventory.RemoveCurrency(inventory.Currency);
						}
						personalBank.RemoveCurrency(num);
						entity.NetworkEntity.PlayerRpcHandler.ProcessInteractiveStationCurrencyTransaction(new ulong?(inventory.Currency), new ulong?(personalBank.Currency));
						return true;
					}
				}
				else if (!this.m_disablePersonalBank && personalBank.Currency >= currencyToRemove)
				{
					personalBank.RemoveCurrency(currencyToRemove);
					entity.NetworkEntity.PlayerRpcHandler.ProcessInteractiveStationCurrencyTransaction(null, new ulong?(personalBank.Currency));
					return true;
				}
				return false;
			}
			if (entity.CollectionController != null)
			{
				entity.CollectionController.ModifyEventCurrency(currencyToRemove, true);
				return true;
			}
			return false;
		}

		// Token: 0x06005A82 RID: 23170 RVA: 0x001ED04C File Offset: 0x001EB24C
		public ulong GetAvailableCurrency(GameEntity entity, out CurrencySources currencySource)
		{
			currencySource = CurrencySources.None;
			if (entity == null || entity.CollectionController == null)
			{
				return 0UL;
			}
			ulong num = 0UL;
			if (this.UseEventCurrency())
			{
				if (entity.User != null && entity.User.EventCurrency != null)
				{
					num = entity.User.EventCurrency.Value;
				}
			}
			else
			{
				if (!entity.IsMissingBag && entity.CollectionController.Inventory != null)
				{
					num += entity.CollectionController.Inventory.Currency;
					currencySource |= CurrencySources.Inventory;
				}
				if (!this.m_disablePersonalBank && entity.CollectionController.PersonalBank != null)
				{
					num += entity.CollectionController.PersonalBank.Currency;
					currencySource |= CurrencySources.PersonalBank;
				}
			}
			return num;
		}

		// Token: 0x06005A83 RID: 23171 RVA: 0x001ED108 File Offset: 0x001EB308
		private ITooltipParameter GetTooltipParameter()
		{
			BaseTooltip.Sb.Clear();
			BaseTooltip.Sb.AppendLine(this.m_tooltipText);
			if (LocalPlayer.GameEntity != null)
			{
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
			}
			return new ObjectTextTooltipParameter(this, BaseTooltip.Sb.ToString(), false);
		}

		// Token: 0x17001518 RID: 5400
		// (get) Token: 0x06005A84 RID: 23172 RVA: 0x0007CC28 File Offset: 0x0007AE28
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17001519 RID: 5401
		// (get) Token: 0x06005A85 RID: 23173 RVA: 0x0007CC36 File Offset: 0x0007AE36
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06005A87 RID: 23175 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004F6D RID: 20333
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04004F6E RID: 20334
		[SerializeField]
		protected bool m_disablePersonalBank;

		// Token: 0x04004F6F RID: 20335
		[SerializeField]
		private ClientRewardRequirement m_rewardRequirement;

		// Token: 0x04004F70 RID: 20336
		[SerializeField]
		private DeploymentBranchFlags m_branchFlags;

		// Token: 0x04004F71 RID: 20337
		[SerializeField]
		private AccessFlags m_accessFlagRequirement;

		// Token: 0x04004F72 RID: 20338
		private Action m_hideCallback;
	}
}
