using System;
using Cysharp.Text;
using SoL.Game.Messages;
using SoL.Game.Objects.Archetypes;
using SoL.Game.UI;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.AuctionHouse
{
	// Token: 0x02000D3D RID: 3389
	public class AuctionHouseSelectedAuctionUI : MonoBehaviour
	{
		// Token: 0x1700186C RID: 6252
		// (get) Token: 0x06006604 RID: 26116 RVA: 0x00084BE0 File Offset: 0x00082DE0
		private ulong AvailableCurrency
		{
			get
			{
				if (!this.m_auctionHouseUi)
				{
					return 0UL;
				}
				return this.m_auctionHouseUi.AvailableCurrency;
			}
		}

		// Token: 0x06006605 RID: 26117 RVA: 0x0020F534 File Offset: 0x0020D734
		private void Awake()
		{
			this.m_bidCurrency.ValueChanged += this.BidCurrencyOnValueChanged;
			this.m_bidCurrency.EditChanged += this.CurrencyEditChanged;
			this.m_bidButton.onClick.AddListener(new UnityAction(this.BidClicked));
			this.m_buyItNowButton.onClick.AddListener(new UnityAction(this.BuyItNowClicked));
			this.m_auctionHouseUi.AvailableCurrencyChanged += this.AuctionHouseUiOnAvailableCurrencyChanged;
			this.m_auctionList.SelectedAuctionChanged += this.AuctionListOnSelectedAuctionChanged;
			this.AuctionListOnSelectedAuctionChanged();
			if (this.m_myAuctionIndicator)
			{
				Color blueColor = UIManager.BlueColor;
				blueColor.a = 0.5f;
				this.m_myAuctionIndicator.color = blueColor;
				this.m_myAuctionIndicator.enabled = false;
			}
		}

		// Token: 0x06006606 RID: 26118 RVA: 0x0020F614 File Offset: 0x0020D814
		private void OnDestroy()
		{
			this.m_bidCurrency.ValueChanged -= this.BidCurrencyOnValueChanged;
			this.m_bidCurrency.EditChanged -= this.CurrencyEditChanged;
			this.m_bidButton.onClick.RemoveListener(new UnityAction(this.BidClicked));
			this.m_buyItNowButton.onClick.RemoveListener(new UnityAction(this.BuyItNowClicked));
			if (this.m_auctionHouseUi)
			{
				this.m_auctionHouseUi.AvailableCurrencyChanged -= this.AuctionHouseUiOnAvailableCurrencyChanged;
			}
			if (this.m_auctionList)
			{
				this.m_auctionList.SelectedAuctionChanged -= this.AuctionListOnSelectedAuctionChanged;
			}
		}

		// Token: 0x06006607 RID: 26119 RVA: 0x00084BFD File Offset: 0x00082DFD
		private void BidCurrencyOnValueChanged(ulong obj)
		{
			this.RefreshButtonAvailability();
		}

		// Token: 0x06006608 RID: 26120 RVA: 0x00084BFD File Offset: 0x00082DFD
		private void CurrencyEditChanged()
		{
			this.RefreshButtonAvailability();
		}

		// Token: 0x06006609 RID: 26121 RVA: 0x00084C05 File Offset: 0x00082E05
		private void AuctionHouseUiOnAvailableCurrencyChanged()
		{
			this.m_bidCurrency.SetLimits(this.m_bidCurrency.MinimumCurrency, this.AvailableCurrency);
			this.RefreshButtonAvailability();
		}

		// Token: 0x0600660A RID: 26122 RVA: 0x0020F6D0 File Offset: 0x0020D8D0
		private void RefreshButtonAvailability()
		{
			if (!this.m_auctionList || this.m_auctionList.SelectedAuction == null || this.m_auctionList.SelectedAuction.Instance == null || !this.m_auctionList.SelectedAuction.Instance.Archetype)
			{
				this.m_bidButton.interactable = false;
				this.m_buyItNowButton.interactable = false;
				if (this.m_myAuctionIndicator)
				{
					this.m_myAuctionIndicator.enabled = false;
				}
				if (this.m_isWinningOverlay)
				{
					this.m_isWinningOverlay.SetActive(false);
				}
				return;
			}
			AuctionRecord selectedAuction = this.m_auctionList.SelectedAuction;
			bool flag = SessionData.IsMyCharacter(selectedAuction.SellerCharacterId);
			string text;
			bool flag2 = this.CanBidTooltip(flag, selectedAuction.CurrentBid, selectedAuction.BuyNowPrice, out text);
			this.m_bidButton.interactable = flag2;
			this.m_bidTooltip.Text = text;
			this.m_bidTooltip.gameObject.SetActive(!flag2);
			AuctionHouseUI.ToggleCanvasGroup(this.m_bidCanvasGroup, flag2);
			string text2;
			bool flag3 = this.CanBuyItNowTooltip(flag, selectedAuction.BuyNowPrice, out text2);
			this.m_buyItNowButton.interactable = flag3;
			this.m_buyItNowTooltip.Text = text2;
			this.m_buyItNowTooltip.gameObject.SetActive(!flag3);
			AuctionHouseUI.ToggleCanvasGroup(this.m_buyItNowCanvasGroup, flag3);
			if (this.m_myAuctionIndicator)
			{
				this.m_myAuctionIndicator.enabled = flag;
			}
			if (this.m_isWinningOverlay)
			{
				bool active = selectedAuction.CurrentBid != null && !string.IsNullOrEmpty(selectedAuction.BuyerCharacterId) && SessionData.IsMyCharacter(selectedAuction.BuyerCharacterId);
				this.m_isWinningOverlay.SetActive(active);
			}
		}

		// Token: 0x0600660B RID: 26123 RVA: 0x0020F880 File Offset: 0x0020DA80
		private bool CanBidTooltip(bool sellerIsMyCharacter, ulong? currentBid, ulong? buyItNowPrice, out string tooltipText)
		{
			tooltipText = null;
			if (sellerIsMyCharacter)
			{
				tooltipText = "You cannot bid on your own auction.";
				return false;
			}
			if (currentBid == null)
			{
				tooltipText = "This auction does not allow bids.";
				return false;
			}
			if (currentBid.Value > this.AvailableCurrency)
			{
				tooltipText = "You cannot afford to bid on this.";
				return false;
			}
			if (currentBid.Value > this.m_bidCurrency.TotalCurrency)
			{
				tooltipText = "Bid not high enough.";
				return false;
			}
			if (buyItNowPrice != null && buyItNowPrice.Value <= this.m_bidCurrency.MinimumCurrency)
			{
				tooltipText = "Bid cannot meet or exceed Buy It Now price.";
				return false;
			}
			if (this.m_bidCurrency.IsEditing)
			{
				tooltipText = "Currently Editing Bid.";
				return false;
			}
			return true;
		}

		// Token: 0x0600660C RID: 26124 RVA: 0x0020F928 File Offset: 0x0020DB28
		private bool CanBuyItNowTooltip(bool sellerIsMyCharacter, ulong? buyItNowPrice, out string tooltipText)
		{
			tooltipText = null;
			if (sellerIsMyCharacter)
			{
				tooltipText = "You cannot buy your own auction.";
				return false;
			}
			if (buyItNowPrice == null)
			{
				tooltipText = "This auction only accepts bids";
				return false;
			}
			if (buyItNowPrice.Value > this.AvailableCurrency)
			{
				tooltipText = "You cannot afford to buy this.";
				return false;
			}
			if (this.m_bidCurrency.IsEditing)
			{
				tooltipText = "Currently Editing Bid.";
				return false;
			}
			return true;
		}

		// Token: 0x0600660D RID: 26125 RVA: 0x00084C29 File Offset: 0x00082E29
		private bool AutoCancelBuyItNow()
		{
			return !this.CanBuyItNow() || !this.m_auctionList || !this.m_auctionList.Window || !this.m_auctionList.Window.Visible;
		}

		// Token: 0x0600660E RID: 26126 RVA: 0x0020F988 File Offset: 0x0020DB88
		private bool CanBuyItNow()
		{
			return this.m_buyItNowButton.interactable && this.m_auctionList && this.m_auctionList.SelectedAuction != null && this.m_auctionList.SelectedAuction.Instance != null && this.m_auctionList.SelectedAuction.BuyNowPrice != null && this.m_auctionList.SelectedAuction.BuyNowPrice.Value <= this.AvailableCurrency && this.m_buyItNowCurrency.TotalCurrency == this.m_auctionList.SelectedAuction.BuyNowPrice.Value && !SessionData.IsMyCharacter(this.m_auctionList.SelectedAuction.SellerCharacterId) && LocalPlayer.NetworkEntity && LocalPlayer.NetworkEntity.PlayerRpcHandler;
		}

		// Token: 0x0600660F RID: 26127 RVA: 0x00084C67 File Offset: 0x00082E67
		private void BuyItNowConfirmation(bool arg1, object arg2)
		{
			if (arg1 && this.CanBuyItNow())
			{
				LocalPlayer.NetworkEntity.PlayerRpcHandler.Client_AuctionHouse_BuyItNow(this.m_auctionList.SelectedAuction.Id);
			}
		}

		// Token: 0x06006610 RID: 26128 RVA: 0x0020FA64 File Offset: 0x0020DC64
		private void BuyItNowClicked()
		{
			if (this.CanBuyItNow())
			{
				DialogOptions opts = new DialogOptions
				{
					Title = "Buy It Now",
					Text = "Are you sure you want to purchase this item?",
					ConfirmationText = "Yes",
					CancelText = "NO",
					Callback = new Action<bool, object>(this.BuyItNowConfirmation),
					Instance = this.m_auctionList.SelectedAuction.Instance,
					AutoCancel = new Func<bool>(this.AutoCancelBuyItNow),
					Currency = new ulong?(this.m_buyItNowCurrency.TotalCurrency),
					CurrencyLabel = "Buy It Now:"
				};
				ClientGameManager.UIManager.ItemConfirmationDialog.Init(opts);
			}
		}

		// Token: 0x06006611 RID: 26129 RVA: 0x0020FB28 File Offset: 0x0020DD28
		private bool ConfirmBidChanged()
		{
			return this.m_auctionList && this.m_auctionList.SelectedAuction != null && this.m_auctionList.SelectedAuction.CurrentBid != null && this.m_auctionList.SelectedAuction.CurrentBid.Value != this.m_confirmBid;
		}

		// Token: 0x06006612 RID: 26130 RVA: 0x0020FB88 File Offset: 0x0020DD88
		private bool AutoCancelBid()
		{
			return !this.CanBid() || this.ConfirmBidChanged() || !this.m_auctionList || !this.m_auctionList.Window || !this.m_auctionList.Window.Visible;
		}

		// Token: 0x06006613 RID: 26131 RVA: 0x0020FBDC File Offset: 0x0020DDDC
		private bool CanBid()
		{
			return this.m_bidButton.interactable && this.m_auctionList && this.m_auctionList.SelectedAuction != null && this.m_auctionList.SelectedAuction.Instance != null && this.m_auctionList.SelectedAuction.CurrentBid != null && this.m_bidCurrency.TotalCurrency <= this.AvailableCurrency && !SessionData.IsMyCharacter(this.m_auctionList.SelectedAuction.SellerCharacterId) && LocalPlayer.NetworkEntity && LocalPlayer.NetworkEntity.PlayerRpcHandler;
		}

		// Token: 0x06006614 RID: 26132 RVA: 0x00084C93 File Offset: 0x00082E93
		private void BidConfirmation(bool arg1, object arg2)
		{
			if (arg1 && this.CanBid())
			{
				this.ExecuteBid();
			}
		}

		// Token: 0x06006615 RID: 26133 RVA: 0x0020FC84 File Offset: 0x0020DE84
		private void BidClicked()
		{
			if (this.CanBid() && this.m_auctionList.SelectedAuction.CurrentBid != null)
			{
				if (Options.GameOptions.ShowBidConfirmation.Value)
				{
					this.m_confirmBid = this.m_auctionList.SelectedAuction.CurrentBid.Value;
					DialogOptions opts = new DialogOptions
					{
						Title = "Bid",
						Text = "Are you sure you want to bid this item?",
						ConfirmationText = "Yes",
						CancelText = "NO",
						Callback = new Action<bool, object>(this.BidConfirmation),
						Instance = this.m_auctionList.SelectedAuction.Instance,
						AutoCancel = new Func<bool>(this.AutoCancelBid),
						Currency = new ulong?(this.m_bidCurrency.TotalCurrency),
						CurrencyLabel = "Bid:"
					};
					ClientGameManager.UIManager.ItemConfirmationDialog.Init(opts);
					return;
				}
				this.ExecuteBid();
			}
		}

		// Token: 0x06006616 RID: 26134 RVA: 0x0020FD94 File Offset: 0x0020DF94
		private void ExecuteBid()
		{
			if (Time.time - this.m_timeOfLastBid < 10f)
			{
				string text = "Bidding too soon!";
				UIManager.TriggerCannotPerform(text);
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, text);
				return;
			}
			if (this.CanBid())
			{
				LocalPlayer.NetworkEntity.PlayerRpcHandler.Client_AuctionHouse_PlaceBid(this.m_auctionList.SelectedAuction.Id, this.m_bidCurrency.TotalCurrency);
				this.m_timeOfLastBid = Time.time;
			}
		}

		// Token: 0x06006617 RID: 26135 RVA: 0x0020FE0C File Offset: 0x0020E00C
		private void AuctionListOnSelectedAuctionChanged()
		{
			if (this.m_auctionList && this.m_auctionList.SelectedAuction != null && this.m_auctionList.SelectedAuction.Instance != null && this.m_auctionList.SelectedAuction.Instance.Archetype)
			{
				AuctionRecord selectedAuction = this.m_auctionList.SelectedAuction;
				this.m_displayIcon.SetIcon(selectedAuction.Instance.Archetype, new Color?(selectedAuction.Instance.Archetype.IconTint));
				this.m_displayIcon.gameObject.SetActive(true);
				string text = selectedAuction.Instance.Archetype.GetModifiedDisplayName(selectedAuction.Instance);
				Color color;
				if (selectedAuction.Instance.Archetype.TryGetItemCategoryColor(ItemCategory.ColorFlags.DisplayName, out color))
				{
					text = text.Color(color);
				}
				this.m_label.ZStringSetText(text);
				ulong num = (selectedAuction.BuyNowPrice != null) ? selectedAuction.BuyNowPrice.Value : 0UL;
				this.m_buyItNowCurrency.UpdateCoin(num);
				ulong minimumBid = ServerAuctionHouseManager.GetMinimumBid(selectedAuction.CurrentBid, selectedAuction.BidCount);
				if (minimumBid > 0UL && num > 0UL && minimumBid >= num)
				{
					this.m_bidCurrency.SetLimitsUpdateCoin(num, num, num);
				}
				else
				{
					ulong maximumCurrency = (num > 0UL) ? (num - 1UL) : this.AvailableCurrency;
					this.m_bidCurrency.SetLimitsUpdateCoin(minimumBid, minimumBid, maximumCurrency);
				}
				this.UpdateCurrentBidPanel(selectedAuction.CurrentBid);
				string arg = string.Empty;
				if (selectedAuction.Instance.Archetype.ArchetypeHasCount())
				{
					arg = ((selectedAuction.Instance.ItemData != null && selectedAuction.Instance.ItemData.Count != null) ? selectedAuction.Instance.ItemData.Count.Value : 1).ToString();
				}
				this.m_countLabel.ZStringSetText(arg);
				AuctionHouseUI.UpdateBidLabel(this.m_bidLabel, new int?(this.m_auctionList.SelectedAuction.BidCount));
				this.m_controls.interactable = true;
				this.m_controls.alpha = 1f;
			}
			else
			{
				this.m_displayIcon.SetIcon(null, null);
				this.m_displayIcon.gameObject.SetActive(false);
				this.m_label.ZStringSetText("");
				this.m_countLabel.ZStringSetText("");
				this.m_bidCurrency.UpdateCoin(0UL);
				this.m_buyItNowCurrency.UpdateCoin(0UL);
				this.UpdateCurrentBidPanel(null);
				AuctionHouseUI.UpdateBidLabel(this.m_bidLabel, null);
				this.m_controls.interactable = false;
				this.m_controls.alpha = 0.5f;
			}
			this.RefreshButtonAvailability();
		}

		// Token: 0x06006618 RID: 26136 RVA: 0x002100E0 File Offset: 0x0020E2E0
		private void UpdateCurrentBidPanel(ulong? value)
		{
			ulong num = (value != null) ? value.Value : 0UL;
			this.m_currentBidCurrency.UpdateCoin(num);
			AuctionHouseUI.ToggleCanvasGroup(this.m_currentBidCanvasGroup, num > 0UL);
		}

		// Token: 0x040058B8 RID: 22712
		private const float kBidRateLimit = 10f;

		// Token: 0x040058B9 RID: 22713
		[SerializeField]
		private AuctionHouseUI m_auctionHouseUi;

		// Token: 0x040058BA RID: 22714
		[SerializeField]
		private AuctionHouseForSaleList m_auctionList;

		// Token: 0x040058BB RID: 22715
		[SerializeField]
		private ArchetypeIconUI m_displayIcon;

		// Token: 0x040058BC RID: 22716
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x040058BD RID: 22717
		[SerializeField]
		private TextMeshProUGUI m_countLabel;

		// Token: 0x040058BE RID: 22718
		[SerializeField]
		private TextMeshProUGUI m_bidLabel;

		// Token: 0x040058BF RID: 22719
		[SerializeField]
		private CanvasGroup m_controls;

		// Token: 0x040058C0 RID: 22720
		[SerializeField]
		private SolButton m_bidButton;

		// Token: 0x040058C1 RID: 22721
		[SerializeField]
		private CurrencyDisplayPanelUI m_bidCurrency;

		// Token: 0x040058C2 RID: 22722
		[SerializeField]
		private CanvasGroup m_bidCanvasGroup;

		// Token: 0x040058C3 RID: 22723
		[SerializeField]
		private TextTooltipTrigger m_bidTooltip;

		// Token: 0x040058C4 RID: 22724
		[SerializeField]
		private SolButton m_buyItNowButton;

		// Token: 0x040058C5 RID: 22725
		[SerializeField]
		private CurrencyDisplayPanelUI m_buyItNowCurrency;

		// Token: 0x040058C6 RID: 22726
		[SerializeField]
		private CanvasGroup m_buyItNowCanvasGroup;

		// Token: 0x040058C7 RID: 22727
		[SerializeField]
		private TextTooltipTrigger m_buyItNowTooltip;

		// Token: 0x040058C8 RID: 22728
		[SerializeField]
		private CurrencyDisplayPanelUI m_currentBidCurrency;

		// Token: 0x040058C9 RID: 22729
		[SerializeField]
		private CanvasGroup m_currentBidCanvasGroup;

		// Token: 0x040058CA RID: 22730
		[SerializeField]
		private Image m_myAuctionIndicator;

		// Token: 0x040058CB RID: 22731
		[SerializeField]
		private GameObject m_isWinningOverlay;

		// Token: 0x040058CC RID: 22732
		private ulong m_confirmBid;

		// Token: 0x040058CD RID: 22733
		private float m_timeOfLastBid;
	}
}
