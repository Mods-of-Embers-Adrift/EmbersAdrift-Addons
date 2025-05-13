using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Objects.Containers;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SoL.Game.UI.Merchants
{
	// Token: 0x02000973 RID: 2419
	public class MerchantUI : BaseMerchantUI<InteractiveMerchant>
	{
		// Token: 0x17000FF4 RID: 4084
		// (get) Token: 0x060047E8 RID: 18408 RVA: 0x000706BE File Offset: 0x0006E8BE
		protected override ContainerType m_containerType
		{
			get
			{
				return ContainerType.MerchantOutgoing;
			}
		}

		// Token: 0x060047E9 RID: 18409 RVA: 0x000706C2 File Offset: 0x0006E8C2
		internal override bool UseEventCurrency()
		{
			return this.CurrentMerchantType == MerchantType.Event;
		}

		// Token: 0x17000FF5 RID: 4085
		// (get) Token: 0x060047EA RID: 18410 RVA: 0x000706CD File Offset: 0x0006E8CD
		// (set) Token: 0x060047EB RID: 18411 RVA: 0x001A80A4 File Offset: 0x001A62A4
		private MerchantType CurrentMerchantType
		{
			get
			{
				return this.m_currentMerchantType;
			}
			set
			{
				if (this.m_currentMerchantType == value)
				{
					return;
				}
				this.m_currentMerchantType = value;
				switch (this.m_currentMerchantType)
				{
				case MerchantType.Standard:
					this.m_label.ZStringSetText("Merchant");
					this.m_standardPanel.SetActive(true);
					this.m_bagRecoveryPanel.SetActive(false);
					this.m_tabController.gameObject.SetActive(true);
					this.m_sellButton.gameObject.SetActive(true);
					this.m_eventPanel.SetActive(false);
					return;
				case MerchantType.BagRecovery:
					this.m_label.ZStringSetText("Bag Recovery Merchant");
					this.m_standardPanel.SetActive(false);
					this.m_bagRecoveryPanel.SetActive(true);
					this.m_tabController.gameObject.SetActive(true);
					this.m_sellButton.gameObject.SetActive(true);
					this.m_eventPanel.SetActive(false);
					return;
				case MerchantType.Event:
				{
					this.m_label.ZStringSetText("Event Merchant");
					this.m_standardPanel.SetActive(true);
					this.m_bagRecoveryPanel.SetActive(false);
					this.m_tabController.gameObject.SetActive(false);
					this.m_sellButton.gameObject.SetActive(false);
					for (int i = 0; i < this.m_eventIcons.Length; i++)
					{
						if (this.m_eventIcons[i])
						{
							this.m_eventIcons[i].color = GlobalSettings.Values.Subscribers.SubscriberColor;
						}
					}
					int eventCostDiscount = GlobalSettings.Values.Subscribers.EventCostDiscount;
					string arg = SessionData.IsSubscriber ? "ACTIVE" : "INACTIVE";
					Color color = SessionData.IsSubscriber ? UIManager.BlueColor : UIManager.RedColor;
					this.m_eventLabel.SetTextFormat("<size=80%>{0}% Subscriber Discount:</size> <b><color={1}>{2}</color></b>", eventCostDiscount, color.ToHex(), arg);
					this.m_eventPanel.SetActive(true);
					return;
				}
				default:
					return;
				}
			}
		}

		// Token: 0x17000FF6 RID: 4086
		// (get) Token: 0x060047EC RID: 18412 RVA: 0x001A8274 File Offset: 0x001A6474
		private MerchantBuybackList CurrentBuybackList
		{
			get
			{
				switch (this.CurrentMerchantType)
				{
				case MerchantType.Standard:
				case MerchantType.Event:
					return this.m_buybackList;
				case MerchantType.BagRecovery:
					return this.m_bagRecoveryBuybackList;
				default:
					throw new ArgumentException("CurrentMerchantType");
				}
			}
		}

		// Token: 0x060047ED RID: 18413 RVA: 0x001A82B4 File Offset: 0x001A64B4
		protected override void Awake()
		{
			base.Awake();
			if (this.m_sellButton)
			{
				this.m_sellButton.onClick.AddListener(new UnityAction(this.SellButtonClicked));
			}
			if (this.m_buybackRefreshButton)
			{
				this.m_buybackRefreshButton.onClick.AddListener(new UnityAction(this.RequestBuybackRefresh));
			}
			if (this.m_bagRecoveryRefreshButton)
			{
				this.m_bagRecoveryRefreshButton.onClick.AddListener(new UnityAction(this.RequestBuybackRefresh));
			}
			if (this.m_tabController)
			{
				this.m_tabController.TabIndexActivated += this.TabControllerOnTabIndexActivated;
			}
			if (this.m_filter)
			{
				this.m_filter.InputChanged += this.OnFilterInputChanged;
			}
			base.UIWindow.ShowCalled += this.WindowShown;
			base.UIWindow.HideCalled += this.WindowHidden;
		}

		// Token: 0x060047EE RID: 18414 RVA: 0x001A83B8 File Offset: 0x001A65B8
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (this.m_sellButton)
			{
				this.m_sellButton.onClick.RemoveListener(new UnityAction(this.SellButtonClicked));
			}
			if (this.m_buybackRefreshButton)
			{
				this.m_buybackRefreshButton.onClick.RemoveListener(new UnityAction(this.RequestBuybackRefresh));
			}
			if (this.m_bagRecoveryRefreshButton)
			{
				this.m_bagRecoveryRefreshButton.onClick.RemoveListener(new UnityAction(this.RequestBuybackRefresh));
			}
			if (this.m_tabController)
			{
				this.m_tabController.TabIndexActivated -= this.TabControllerOnTabIndexActivated;
			}
			if (this.m_filter)
			{
				this.m_filter.InputChanged -= this.OnFilterInputChanged;
			}
			base.UIWindow.ShowCalled -= this.WindowShown;
			base.UIWindow.HideCalled -= this.WindowHidden;
			this.ExitSellMode();
		}

		// Token: 0x060047EF RID: 18415 RVA: 0x000706D5 File Offset: 0x0006E8D5
		private void OnFilterInputChanged(string value)
		{
			this.m_saleList.UpdateItems(this.m_currentForSaleItemIds, value);
		}

		// Token: 0x060047F0 RID: 18416 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool ButtonClickedInternal()
		{
			return false;
		}

		// Token: 0x060047F1 RID: 18417 RVA: 0x000706E9 File Offset: 0x0006E8E9
		protected override void ContainerInstanceOnContentsChanged()
		{
			base.ContainerInstanceOnContentsChanged();
			this.m_button.interactable = (this.m_containerInstance != null && this.m_containerInstance.Currency > 0UL);
		}

		// Token: 0x060047F2 RID: 18418 RVA: 0x00070716 File Offset: 0x0006E916
		public void UpdateForSaleItems(MerchantType buybackSource, ForSaleItemIds forSaleItemIds)
		{
			this.m_currentForSaleItemIds = forSaleItemIds.ItemIds;
			this.CurrentMerchantType = buybackSource;
			this.m_saleList.UpdateItems(this.m_currentForSaleItemIds, this.m_filter.GetText());
		}

		// Token: 0x060047F3 RID: 18419 RVA: 0x00070747 File Offset: 0x0006E947
		public void UpdateBuybackItems(MerchantType buybackSource, BuybackItemData buybackItemData)
		{
			this.CurrentMerchantType = buybackSource;
			this.CurrentBuybackList.UpdateItems(buybackItemData);
		}

		// Token: 0x060047F4 RID: 18420 RVA: 0x0007075C File Offset: 0x0006E95C
		private void TabControllerOnTabIndexActivated(int obj)
		{
			if (obj == 1)
			{
				this.ExitSellMode();
				this.RequestBuybackRefresh();
			}
		}

		// Token: 0x060047F5 RID: 18421 RVA: 0x0007076E File Offset: 0x0006E96E
		private void RequestBuybackRefresh()
		{
			if (LocalPlayer.NetworkEntity != null && LocalPlayer.NetworkEntity.PlayerRpcHandler != null)
			{
				LocalPlayer.NetworkEntity.PlayerRpcHandler.MerchantBuybackUpdateRequest();
			}
		}

		// Token: 0x060047F6 RID: 18422 RVA: 0x0007079E File Offset: 0x0006E99E
		protected override void Refresh()
		{
			base.Refresh();
			base.RefreshAvailableCurrency();
			this.m_saleList.RefreshAvailability();
			this.CurrentBuybackList.RefreshAvailability();
		}

		// Token: 0x060047F7 RID: 18423 RVA: 0x000707C2 File Offset: 0x0006E9C2
		protected override void LeavingWindow()
		{
			base.LeavingWindow();
			this.ExitSellMode();
		}

		// Token: 0x060047F8 RID: 18424 RVA: 0x000707D0 File Offset: 0x0006E9D0
		private void SellButtonClicked()
		{
			CursorManager.ToggleGameMode(CursorGameMode.Sell);
		}

		// Token: 0x060047F9 RID: 18425 RVA: 0x000707D8 File Offset: 0x0006E9D8
		private void ExitSellMode()
		{
			CursorManager.ExitGameMode(CursorGameMode.Sell);
		}

		// Token: 0x060047FA RID: 18426 RVA: 0x001A84C4 File Offset: 0x001A66C4
		private void WindowShown()
		{
			if (!this.m_subscribed)
			{
				LocalPlayer.GameEntity.CollectionController.Inventory.CurrencyChanged += this.CurrencyChanged;
				LocalPlayer.GameEntity.CollectionController.Equipment.ContentsChanged += this.EquipmentOnContentsChanged;
				LocalPlayer.GameEntity.CollectionController.Pouch.ContentsChanged += this.PouchOnContentsChanged;
				LocalPlayer.GameEntity.CollectionController.PersonalBank.CurrencyChanged += this.CurrencyChanged;
				LocalPlayer.GameEntity.CharacterData.CharacterFlags.Changed += this.CharacterFlagsOnChanged;
				LocalPlayer.GameEntity.CharacterData.AdventuringLevelSync.Changed += this.AdventuringLevelSyncOnChanged;
				LocalPlayer.GameEntity.CharacterData.AnyMasteryLevelChanged += this.MasteryLevelChanged;
				UIManager.EventCurrencyChanged += this.EventCurrencyChanged;
				this.m_subscribed = true;
			}
			if (this.m_filter)
			{
				this.m_filter.SetText(string.Empty);
			}
			if (this.m_tabController)
			{
				this.m_tabController.SwitchToTab(0);
			}
		}

		// Token: 0x060047FB RID: 18427 RVA: 0x001A8608 File Offset: 0x001A6808
		private void WindowHidden()
		{
			if (this.m_subscribed)
			{
				LocalPlayer.GameEntity.CollectionController.Inventory.CurrencyChanged -= this.CurrencyChanged;
				LocalPlayer.GameEntity.CollectionController.Equipment.ContentsChanged -= this.EquipmentOnContentsChanged;
				LocalPlayer.GameEntity.CollectionController.Pouch.ContentsChanged -= this.PouchOnContentsChanged;
				LocalPlayer.GameEntity.CollectionController.PersonalBank.CurrencyChanged -= this.CurrencyChanged;
				LocalPlayer.GameEntity.CharacterData.CharacterFlags.Changed -= this.CharacterFlagsOnChanged;
				LocalPlayer.GameEntity.CharacterData.AdventuringLevelSync.Changed -= this.AdventuringLevelSyncOnChanged;
				LocalPlayer.GameEntity.CharacterData.AnyMasteryLevelChanged -= this.MasteryLevelChanged;
				UIManager.EventCurrencyChanged -= this.EventCurrencyChanged;
				this.m_subscribed = false;
			}
			if (this.m_filter)
			{
				this.m_filter.Deactivate();
			}
		}

		// Token: 0x060047FC RID: 18428 RVA: 0x000707E0 File Offset: 0x0006E9E0
		private void EventCurrencyChanged()
		{
			this.Refresh();
		}

		// Token: 0x060047FD RID: 18429 RVA: 0x000707E0 File Offset: 0x0006E9E0
		private void CurrencyChanged(ulong obj)
		{
			this.Refresh();
		}

		// Token: 0x060047FE RID: 18430 RVA: 0x000707E0 File Offset: 0x0006E9E0
		private void EquipmentOnContentsChanged()
		{
			this.Refresh();
		}

		// Token: 0x060047FF RID: 18431 RVA: 0x000707E0 File Offset: 0x0006E9E0
		private void PouchOnContentsChanged()
		{
			this.Refresh();
		}

		// Token: 0x06004800 RID: 18432 RVA: 0x000707E0 File Offset: 0x0006E9E0
		private void AdventuringLevelSyncOnChanged(byte obj)
		{
			this.Refresh();
		}

		// Token: 0x06004801 RID: 18433 RVA: 0x000707E0 File Offset: 0x0006E9E0
		private void CharacterFlagsOnChanged(PlayerFlags obj)
		{
			this.Refresh();
		}

		// Token: 0x06004802 RID: 18434 RVA: 0x000707E0 File Offset: 0x0006E9E0
		private void MasteryLevelChanged()
		{
			this.Refresh();
		}

		// Token: 0x0400436A RID: 17258
		private const string kMerchantGroup = "Standard";

		// Token: 0x0400436B RID: 17259
		private const string kBagMerchantGroup = "Bag Recovery";

		// Token: 0x0400436C RID: 17260
		private const string kEventGroup = "Event";

		// Token: 0x0400436D RID: 17261
		[FormerlySerializedAs("m_merchantPanel")]
		[SerializeField]
		private GameObject m_standardPanel;

		// Token: 0x0400436E RID: 17262
		[SerializeField]
		private SolButton m_sellButton;

		// Token: 0x0400436F RID: 17263
		[SerializeField]
		private TabController m_tabController;

		// Token: 0x04004370 RID: 17264
		[SerializeField]
		private MerchantForSaleList m_saleList;

		// Token: 0x04004371 RID: 17265
		[SerializeField]
		private MerchantBuybackList m_buybackList;

		// Token: 0x04004372 RID: 17266
		[SerializeField]
		private SolButton m_buybackRefreshButton;

		// Token: 0x04004373 RID: 17267
		[FormerlySerializedAs("m_bagBuybackPanel")]
		[SerializeField]
		private GameObject m_bagRecoveryPanel;

		// Token: 0x04004374 RID: 17268
		[FormerlySerializedAs("m_bagBuybackList")]
		[SerializeField]
		private MerchantBuybackList m_bagRecoveryBuybackList;

		// Token: 0x04004375 RID: 17269
		[FormerlySerializedAs("m_bagBuybackRefreshButton")]
		[SerializeField]
		private SolButton m_bagRecoveryRefreshButton;

		// Token: 0x04004376 RID: 17270
		[SerializeField]
		private GameObject m_eventPanel;

		// Token: 0x04004377 RID: 17271
		[SerializeField]
		private TextMeshProUGUI m_eventLabel;

		// Token: 0x04004378 RID: 17272
		[SerializeField]
		private Image[] m_eventIcons;

		// Token: 0x04004379 RID: 17273
		[SerializeField]
		private TextInputFilter m_filter;

		// Token: 0x0400437A RID: 17274
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x0400437B RID: 17275
		private bool m_subscribed;

		// Token: 0x0400437C RID: 17276
		private MerchantType m_currentMerchantType;

		// Token: 0x0400437D RID: 17277
		private UniqueId[] m_currentForSaleItemIds;
	}
}
