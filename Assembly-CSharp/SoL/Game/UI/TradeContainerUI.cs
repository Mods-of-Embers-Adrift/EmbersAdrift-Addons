using System;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Managers;
using SoL.Networking.Objects;
using SoL.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x020008E7 RID: 2279
	public class TradeContainerUI : DraggableUIWindow, ITooltip, IInteractiveBase
	{
		// Token: 0x17000F27 RID: 3879
		// (get) Token: 0x060042B6 RID: 17078 RVA: 0x0006D0CF File Offset: 0x0006B2CF
		public TradeContainerHalfUI Outgoing
		{
			get
			{
				return this.m_outgoingTrade;
			}
		}

		// Token: 0x17000F28 RID: 3880
		// (get) Token: 0x060042B7 RID: 17079 RVA: 0x0006D0D7 File Offset: 0x0006B2D7
		public TradeContainerHalfUI Incoming
		{
			get
			{
				return this.m_incomingTrade;
			}
		}

		// Token: 0x060042B8 RID: 17080 RVA: 0x001933C0 File Offset: 0x001915C0
		protected override void Awake()
		{
			base.Awake();
			this.m_accept.onClick.AddListener(new UnityAction(this.OnAcceptClicked));
			this.m_cancel.onClick.AddListener(new UnityAction(this.OnCancelClicked));
			this.m_outgoingTrade.ContentsChanged += this.OnContentsChanged;
			this.m_outgoingTrade.Initialized += this.OutgoingTradeOnInitialized;
			LocalPlayer.NetworkEntity.OnStartLocalClient += this.NetworkEntityOnOnStartLocalClient;
		}

		// Token: 0x060042B9 RID: 17081 RVA: 0x00193450 File Offset: 0x00191650
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.m_accept.onClick.RemoveListener(new UnityAction(this.OnAcceptClicked));
			this.m_cancel.onClick.RemoveListener(new UnityAction(this.OnCancelClicked));
			this.m_outgoingTrade.ContentsChanged -= this.OnContentsChanged;
			this.m_outgoingTrade.Initialized -= this.OutgoingTradeOnInitialized;
			LocalPlayer.NetworkEntity.OnStartLocalClient -= this.NetworkEntityOnOnStartLocalClient;
			if (this.m_localInventory != null)
			{
				this.m_localInventory.InstanceAdded -= this.LocalInventoryChanged;
				this.m_localInventory.InstanceRemoved -= this.LocalInventoryChanged;
			}
		}

		// Token: 0x060042BA RID: 17082 RVA: 0x0006D0DF File Offset: 0x0006B2DF
		private void NetworkEntityOnOnStartLocalClient()
		{
			this.Initialize();
		}

		// Token: 0x060042BB RID: 17083 RVA: 0x0006D0E7 File Offset: 0x0006B2E7
		private void OutgoingTradeOnInitialized()
		{
			this.ResetTradeAccepted();
		}

		// Token: 0x060042BC RID: 17084 RVA: 0x0006D0E7 File Offset: 0x0006B2E7
		private void OnContentsChanged()
		{
			this.ResetTradeAccepted();
		}

		// Token: 0x060042BD RID: 17085 RVA: 0x0006D0EF File Offset: 0x0006B2EF
		public override void CloseButtonPressed()
		{
			ClientGameManager.TradeManager.Client_CancelTradeClicked();
			base.CloseButtonPressed();
		}

		// Token: 0x060042BE RID: 17086 RVA: 0x0006D101 File Offset: 0x0006B301
		public override void Show(bool skipTransition = false)
		{
			base.Show(skipTransition);
			this.ResetTradeAccepted();
			this.m_incomingTrade.UpdateHeaderText();
		}

		// Token: 0x060042BF RID: 17087 RVA: 0x00193518 File Offset: 0x00191718
		private void Initialize()
		{
			this.ResetTradeAccepted();
			LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.Inventory, out this.m_localInventory);
			this.m_localInventory.InstanceAdded += this.LocalInventoryChanged;
			this.m_localInventory.InstanceRemoved += this.LocalInventoryChanged;
			this.LocalInventoryChanged(null);
		}

		// Token: 0x060042C0 RID: 17088 RVA: 0x0006D11B File Offset: 0x0006B31B
		private void OnAcceptClicked()
		{
			ClientGameManager.TradeManager.Client_AcceptTradeClicked();
			this.m_accept.interactable = false;
		}

		// Token: 0x060042C1 RID: 17089 RVA: 0x0006B391 File Offset: 0x00069591
		private void OnCancelClicked()
		{
			this.CloseButtonPressed();
		}

		// Token: 0x060042C2 RID: 17090 RVA: 0x0006D133 File Offset: 0x0006B333
		public void TradeAccepted(NetworkEntity entity)
		{
			if (entity == LocalPlayer.NetworkEntity)
			{
				this.m_outgoingTrade.ContainerInstance.LockFlags |= ContainerLockFlags.Trade;
				this.m_outgoingHighlight.enabled = true;
				return;
			}
			this.m_incomingHighlight.enabled = true;
		}

		// Token: 0x060042C3 RID: 17091 RVA: 0x00193578 File Offset: 0x00191778
		public void ResetTradeAccepted()
		{
			this.m_outgoingTrade.ContainerInstance.LockFlags &= ~ContainerLockFlags.Trade;
			this.m_outgoingHighlight.enabled = false;
			this.m_incomingHighlight.enabled = false;
			this.m_cancel.interactable = true;
			this.m_accept.interactable = this.HasRoom();
		}

		// Token: 0x060042C4 RID: 17092 RVA: 0x001935D4 File Offset: 0x001917D4
		private void LocalInventoryChanged(ArchetypeInstance obj)
		{
			if (!base.Visible)
			{
				return;
			}
			bool flag = this.HasRoom();
			this.m_accept.interactable = flag;
			if (!flag)
			{
				this.ResetTradeAccepted();
				ClientGameManager.TradeManager.Client_ResetTradeAgreement();
			}
		}

		// Token: 0x060042C5 RID: 17093 RVA: 0x00193610 File Offset: 0x00191810
		private bool HasRoom()
		{
			bool hasRoom = false;
			if (this.m_incomingTrade != null && this.m_incomingTrade.ContainerInstance != null && this.m_localInventory != null)
			{
				hasRoom = (this.m_incomingTrade.ContainerInstance.Count + this.m_localInventory.Count <= this.m_localInventory.GetMaxCapacity());
			}
			this.m_hasRoom = hasRoom;
			return this.m_hasRoom;
		}

		// Token: 0x060042C6 RID: 17094 RVA: 0x0006D173 File Offset: 0x0006B373
		private ITooltipParameter GetTooltipParameter()
		{
			if (!this.m_hasRoom)
			{
				return new ObjectTextTooltipParameter(this, "Not enough room in your inventory!", false);
			}
			return null;
		}

		// Token: 0x17000F29 RID: 3881
		// (get) Token: 0x060042C7 RID: 17095 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000F2A RID: 3882
		// (get) Token: 0x060042C8 RID: 17096 RVA: 0x0006D190 File Offset: 0x0006B390
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000F2B RID: 3883
		// (get) Token: 0x060042C9 RID: 17097 RVA: 0x0006D19E File Offset: 0x0006B39E
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x060042CB RID: 17099 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003F96 RID: 16278
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003F97 RID: 16279
		[SerializeField]
		private TradeContainerHalfUI m_outgoingTrade;

		// Token: 0x04003F98 RID: 16280
		[SerializeField]
		private TradeContainerHalfUI m_incomingTrade;

		// Token: 0x04003F99 RID: 16281
		private ContainerInstance m_localInventory;

		// Token: 0x04003F9A RID: 16282
		[SerializeField]
		private SolButton m_accept;

		// Token: 0x04003F9B RID: 16283
		[SerializeField]
		private SolButton m_cancel;

		// Token: 0x04003F9C RID: 16284
		[SerializeField]
		private Image m_outgoingHighlight;

		// Token: 0x04003F9D RID: 16285
		[SerializeField]
		private Image m_incomingHighlight;

		// Token: 0x04003F9E RID: 16286
		private bool m_hasRoom;
	}
}
