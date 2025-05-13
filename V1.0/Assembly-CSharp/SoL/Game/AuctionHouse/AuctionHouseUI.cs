using System;
using System.Collections.Generic;
using Cysharp.Text;
using NetStack.Serialization;
using SoL.Game.Objects.Containers;
using SoL.Game.UI;
using SoL.Game.UI.Merchants;
using SoL.Networking.Database;
using SoL.Utilities;
using TMPro;
using UnityEngine;

namespace SoL.Game.AuctionHouse
{
	// Token: 0x02000D3F RID: 3391
	public class AuctionHouseUI : BaseMerchantUI<InteractiveAuctionHouse>
	{
		// Token: 0x06006620 RID: 26144 RVA: 0x00210180 File Offset: 0x0020E380
		internal static void UpdateBidLabel(TextMeshProUGUI label, int? bidCount)
		{
			if (!label)
			{
				return;
			}
			if (bidCount == null)
			{
				label.ZStringSetText("Bid:");
				return;
			}
			if (bidCount.Value <= 0)
			{
				label.ZStringSetText("Starting Bid:");
				return;
			}
			string arg = (bidCount.Value > 1) ? "s" : "";
			label.SetTextFormat("{0} Bid{1}:", bidCount.Value, arg);
		}

		// Token: 0x17001870 RID: 6256
		// (get) Token: 0x06006621 RID: 26145 RVA: 0x00084CBC File Offset: 0x00082EBC
		public UniversalContainerUI NewAuction
		{
			get
			{
				return this.m_newAuction;
			}
		}

		// Token: 0x17001871 RID: 6257
		// (get) Token: 0x06006622 RID: 26146 RVA: 0x00084CC4 File Offset: 0x00082EC4
		protected override ContainerType m_containerType
		{
			get
			{
				return ContainerType.AuctionOutgoing;
			}
		}

		// Token: 0x06006623 RID: 26147 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool ButtonClickedInternal()
		{
			return false;
		}

		// Token: 0x06006624 RID: 26148 RVA: 0x002101EC File Offset: 0x0020E3EC
		protected override void Awake()
		{
			base.Awake();
			if (base.UIWindow)
			{
				base.UIWindow.ShowCalled += this.UIWindowOnShowCalled;
				base.UIWindow.HideCalled += this.UIWindowOnHideCalled;
			}
		}

		// Token: 0x06006625 RID: 26149 RVA: 0x00084CC8 File Offset: 0x00082EC8
		protected override void Start()
		{
			base.Start();
			if (LocalPlayer.IsInitialized)
			{
				this.Init();
				return;
			}
			LocalPlayer.LocalPlayerInitialized += this.LocalPlayerOnLocalPlayerInitialized;
		}

		// Token: 0x06006626 RID: 26150 RVA: 0x0021023C File Offset: 0x0020E43C
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (base.UIWindow)
			{
				base.UIWindow.ShowCalled -= this.UIWindowOnShowCalled;
				base.UIWindow.HideCalled -= this.UIWindowOnHideCalled;
			}
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null)
			{
				LocalPlayer.GameEntity.CollectionController.Inventory.CurrencyChanged -= this.OnCurrencyChanged;
				LocalPlayer.GameEntity.CollectionController.PersonalBank.CurrencyChanged -= this.OnCurrencyChanged;
			}
		}

		// Token: 0x06006627 RID: 26151 RVA: 0x00084CEF File Offset: 0x00082EEF
		private void UIWindowOnShowCalled()
		{
			base.RefreshAvailableCurrency();
		}

		// Token: 0x06006628 RID: 26152 RVA: 0x00084CF7 File Offset: 0x00082EF7
		private void UIWindowOnHideCalled()
		{
			this.UpdateAuctionList(null);
			if (this.m_auctionList)
			{
				this.m_auctionList.WindowClosed();
			}
		}

		// Token: 0x06006629 RID: 26153 RVA: 0x00084D18 File Offset: 0x00082F18
		private void LocalPlayerOnLocalPlayerInitialized()
		{
			LocalPlayer.LocalPlayerInitialized -= this.LocalPlayerOnLocalPlayerInitialized;
			this.Init();
		}

		// Token: 0x0600662A RID: 26154 RVA: 0x002102E4 File Offset: 0x0020E4E4
		private void Init()
		{
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null)
			{
				LocalPlayer.GameEntity.CollectionController.Inventory.CurrencyChanged += this.OnCurrencyChanged;
				LocalPlayer.GameEntity.CollectionController.PersonalBank.CurrencyChanged += this.OnCurrencyChanged;
			}
			base.RefreshAvailableCurrency();
		}

		// Token: 0x0600662B RID: 26155 RVA: 0x00084CEF File Offset: 0x00082EEF
		private void OnCurrencyChanged(ulong obj)
		{
			base.RefreshAvailableCurrency();
		}

		// Token: 0x0600662C RID: 26156 RVA: 0x00084D31 File Offset: 0x00082F31
		public void UpdateAuctionList(List<AuctionRecord> auctions)
		{
			if (this.m_auctionList)
			{
				this.m_auctionList.UpdateList(auctions);
			}
			StaticListPool<AuctionRecord>.ReturnToPool(auctions);
		}

		// Token: 0x0600662D RID: 26157 RVA: 0x00084D52 File Offset: 0x00082F52
		public void ProcessAuctionHouseUpdate(BitBuffer buffer)
		{
			if (this.m_auctionList)
			{
				this.m_auctionList.ProcessAuctionHouseUpdate(buffer);
			}
		}

		// Token: 0x0600662E RID: 26158 RVA: 0x00084D6D File Offset: 0x00082F6D
		internal static void ToggleCanvasGroup(CanvasGroup group, bool isEnabled)
		{
			if (group)
			{
				group.blocksRaycasts = isEnabled;
				group.interactable = isEnabled;
				group.alpha = (isEnabled ? 1f : 0.5f);
			}
		}

		// Token: 0x040058D0 RID: 22736
		internal const ulong kMaxAuction = 9999999UL;

		// Token: 0x040058D1 RID: 22737
		[SerializeField]
		private UniversalContainerUI m_newAuction;

		// Token: 0x040058D2 RID: 22738
		[SerializeField]
		private AuctionHouseForSaleList m_auctionList;
	}
}
