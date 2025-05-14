using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Game.UI;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.AuctionHouse
{
	// Token: 0x02000D3A RID: 3386
	public class AuctionHouseForSaleListItem : MonoBehaviour, ITooltip, IInteractiveBase, ICursor, IContextMenu, IPointerClickHandler, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
	{
		// Token: 0x060065BE RID: 26046 RVA: 0x0020E1F4 File Offset: 0x0020C3F4
		private void Awake()
		{
			if (this.m_selfIndicators != null)
			{
				Color blueColor = UIManager.BlueColor;
				blueColor.a = 0.5f;
				for (int i = 0; i < this.m_selfIndicators.Length; i++)
				{
					if (this.m_selfIndicators[i])
					{
						this.m_selfIndicators[i].color = blueColor;
					}
				}
			}
			if (this.m_pricePerItemPanel)
			{
				this.m_pricePerItemPanel.SetActive(false);
			}
		}

		// Token: 0x060065BF RID: 26047 RVA: 0x0020E264 File Offset: 0x0020C464
		public void InitItem(AuctionHouseForSaleList controller, AuctionRecord auction)
		{
			this.m_controller = controller;
			this.m_auction = auction;
			BaseArchetype archetype = null;
			Color? color = null;
			if (this.m_auction != null && this.m_auction.Instance != null && this.m_auction.Instance.Archetype)
			{
				archetype = this.m_auction.Instance.Archetype;
				color = new Color?(this.m_auction.Instance.Archetype.IconTint);
			}
			this.m_archetypeIcon.SetIcon(archetype, color);
			this.m_isThisCharacter = (this.m_auction != null && LocalPlayer.IsActiveCharacter(this.m_auction.SellerCharacterId));
			this.m_isMyAuction = (this.m_auction != null && SessionData.IsMyCharacter(this.m_auction.SellerCharacterId));
			if (this.m_selfIndicators != null)
			{
				for (int i = 0; i < this.m_selfIndicators.Length; i++)
				{
					if (this.m_selfIndicators[i])
					{
						this.m_selfIndicators[i].enabled = this.m_isMyAuction;
					}
				}
			}
			this.RefreshAvailability();
		}

		// Token: 0x060065C0 RID: 26048 RVA: 0x0020E374 File Offset: 0x0020C574
		public void RefreshAvailability()
		{
			this.RefreshStackCount();
			this.RefreshCurrencyPanels();
			this.RefreshLabels();
			this.RefreshIsWinning();
			this.RefreshSelected();
			if (this.m_deathIcon)
			{
				this.m_deathIcon.gameObject.SetActive(false);
			}
			this.m_disabledPanel.enabled = false;
		}

		// Token: 0x060065C1 RID: 26049 RVA: 0x0020E3CC File Offset: 0x0020C5CC
		internal void RefreshCurrencyPanels()
		{
			this.UpdateCurrencyPanel(this.m_currentBid, this.m_auction.CurrentBid);
			this.UpdateCurrencyPanel(this.m_buyItNow, this.m_auction.BuyNowPrice);
			if (this.m_pricePerItemPanel)
			{
				this.m_pricePerItemPanel.SetActive(this.m_stackCount != null && this.m_controller && this.m_controller.HoldingShift);
			}
		}

		// Token: 0x060065C2 RID: 26050 RVA: 0x0020E448 File Offset: 0x0020C648
		private void UpdateCurrencyPanel(CurrencyDisplayPanelUI currencyDisplay, ulong? amount)
		{
			if (currencyDisplay)
			{
				bool flag = amount != null;
				if (flag)
				{
					ulong value = (ulong)((this.m_controller && this.m_controller.HoldingShift && this.m_stackCount != null) ? ((long)Mathf.CeilToInt(amount.Value / (float)this.m_stackCount.Value)) : ((long)amount.Value));
					currencyDisplay.UpdateCoin(value);
				}
				if (currencyDisplay.gameObject.activeSelf != flag)
				{
					currencyDisplay.gameObject.SetActive(flag);
				}
			}
		}

		// Token: 0x060065C3 RID: 26051 RVA: 0x00045BCA File Offset: 0x00043DCA
		private bool IsHoldingShiftValid()
		{
			return false;
		}

		// Token: 0x060065C4 RID: 26052 RVA: 0x0020E4D8 File Offset: 0x0020C6D8
		private void RefreshStackCount()
		{
			if (this.m_auction != null && this.m_auction.Instance != null && this.m_auction.Instance.ItemData != null && this.m_auction.Instance.ItemData.Count != null)
			{
				this.m_stackCount = new int?(this.m_auction.Instance.ItemData.Count.Value);
				return;
			}
			this.m_stackCount = null;
		}

		// Token: 0x060065C5 RID: 26053 RVA: 0x0020E560 File Offset: 0x0020C760
		private void RefreshLabels()
		{
			if (this.m_auction == null || this.m_auction.Instance == null || !this.m_auction.Instance.Archetype)
			{
				this.m_itemLabel.ZStringSetText("");
				this.m_timeLeftLabel.ZStringSetText("");
				this.m_sellerLabel.ZStringSetText("");
				this.m_countLabel.ZStringSetText("");
				AuctionHouseUI.UpdateBidLabel(this.m_bidLabel, null);
				return;
			}
			string text = this.m_auction.Instance.Archetype.GetModifiedDisplayName(this.m_auction.Instance);
			Color color;
			if (this.m_auction.Instance.Archetype.TryGetItemCategoryColor(ItemCategory.ColorFlags.DisplayName, out color))
			{
				text = text.Color(color);
			}
			this.m_itemLabel.ZStringSetText(text);
			if (this.m_auction.Instance.Archetype.ArchetypeHasCount())
			{
				int arg = (this.m_stackCount != null) ? this.m_stackCount.Value : 1;
				this.m_countLabel.SetTextFormat("{0}", arg);
			}
			else
			{
				this.m_countLabel.ZStringSetText("");
			}
			AuctionHouseUI.UpdateBidLabel(this.m_bidLabel, new int?(this.m_auction.BidCount));
			string text2 = this.m_auction.SellerName;
			if (SessionData.IsMyCharacter(this.m_auction.SellerCharacterId))
			{
				text2 = text2.Color(UIManager.BlueColor);
			}
			text2 = SoL.Utilities.Extensions.TextMeshProExtensions.CreatePlayerLink(text2);
			this.m_sellerLabel.ZStringSetText(text2);
			this.RefreshTimeLeftLabel();
		}

		// Token: 0x060065C6 RID: 26054 RVA: 0x0020E6F4 File Offset: 0x0020C8F4
		private void RefreshIsWinning()
		{
			this.m_isWinningAuction = (this.m_auction != null && this.m_auction.CurrentBid != null && !string.IsNullOrEmpty(this.m_auction.BuyerCharacterId) && SessionData.IsMyCharacter(this.m_auction.BuyerCharacterId));
			if (this.m_isWinningOverlay)
			{
				this.m_isWinningOverlay.SetActive(this.m_isWinningAuction);
			}
		}

		// Token: 0x060065C7 RID: 26055 RVA: 0x0020E764 File Offset: 0x0020C964
		public void RefreshTimeLeftLabel()
		{
			DateTime serverCorrectedDateTimeUtc = GameTimeReplicator.GetServerCorrectedDateTimeUtc();
			TimeSpan timeLeft = this.m_auction.Expiration - serverCorrectedDateTimeUtc;
			this.m_timeLeftLabel.ZStringSetText(AuctionHouseForSaleListItem.GetTimeLeftText(timeLeft));
		}

		// Token: 0x060065C8 RID: 26056 RVA: 0x0020E79C File Offset: 0x0020C99C
		public void RefreshSelected()
		{
			this.m_isSelected = (this.m_controller && this.m_controller.SelectedAuction != null && this.m_auction != null && this.m_auction.Id == this.m_controller.SelectedAuction.Id);
			this.RefreshHover();
		}

		// Token: 0x060065C9 RID: 26057 RVA: 0x0020E7FC File Offset: 0x0020C9FC
		private void RefreshHover()
		{
			bool enabled = false;
			Color blueColor = UIManager.BlueColor;
			if (this.m_isSelected && this.m_mouseHover)
			{
				enabled = true;
			}
			else if (this.m_isSelected)
			{
				enabled = true;
			}
			else if (this.m_mouseHover)
			{
				enabled = true;
				blueColor.a = 0.2f;
			}
			this.m_hover.color = blueColor;
			this.m_hover.enabled = enabled;
		}

		// Token: 0x060065CA RID: 26058 RVA: 0x0020E860 File Offset: 0x0020CA60
		public static string GetTimeLeftText(TimeSpan timeLeft)
		{
			if (timeLeft.TotalDays > 1.0)
			{
				int arg = Mathf.CeilToInt((float)timeLeft.TotalDays);
				return ZString.Format<int>("{0} Days", arg);
			}
			if (timeLeft.TotalHours >= 1.0)
			{
				double totalHours = timeLeft.TotalHours;
				for (int i = 0; i < AuctionHouseForSaleListItem.kHourThresholds.Length - 1; i++)
				{
					if (totalHours <= (double)AuctionHouseForSaleListItem.kHourThresholds[i] && totalHours > (double)AuctionHouseForSaleListItem.kHourThresholds[i + 1])
					{
						return ZString.Format<int>("{0} Hours", AuctionHouseForSaleListItem.kHourThresholds[i]);
					}
				}
				return "1 Hour";
			}
			if (timeLeft.TotalMinutes > 1.0)
			{
				double totalMinutes = timeLeft.TotalMinutes;
				if (totalMinutes > (double)AuctionHouseForSaleListItem.kMinuteThreshold[0])
				{
					return "1 Hour";
				}
				for (int j = 0; j < AuctionHouseForSaleListItem.kMinuteThreshold.Length - 1; j++)
				{
					if (totalMinutes <= (double)AuctionHouseForSaleListItem.kMinuteThreshold[j] && totalMinutes > (double)AuctionHouseForSaleListItem.kMinuteThreshold[j + 1])
					{
						return ZString.Format<int>("{0} Minutes", AuctionHouseForSaleListItem.kMinuteThreshold[j]);
					}
				}
			}
			return ZString.Format<int>("{0} Minutes", AuctionHouseForSaleListItem.kMinuteThreshold[AuctionHouseForSaleListItem.kMinuteThreshold.Length - 1]);
		}

		// Token: 0x060065CB RID: 26059 RVA: 0x00084914 File Offset: 0x00082B14
		private bool AutoCancelCancel()
		{
			return !this.CanCancel() || !this.m_controller || !this.m_controller.Window || !this.m_controller.Window.Visible;
		}

		// Token: 0x060065CC RID: 26060 RVA: 0x00084952 File Offset: 0x00082B52
		private bool CanCancel()
		{
			return this.m_auction != null && this.m_auction.Instance != null && this.m_isMyAuction && LocalPlayer.NetworkEntity && LocalPlayer.NetworkEntity.PlayerRpcHandler;
		}

		// Token: 0x060065CD RID: 26061 RVA: 0x0020E980 File Offset: 0x0020CB80
		private void CancelAuction()
		{
			if (this.CanCancel())
			{
				DialogOptions opts = new DialogOptions
				{
					Title = "Cancel Auction",
					Text = "Are you sure you want to cancel this auction? Your initial deposit will be lost.",
					ConfirmationText = "Yes",
					CancelText = "NO",
					Callback = new Action<bool, object>(this.CancelCallback),
					Instance = this.m_auction.Instance,
					AutoCancel = new Func<bool>(this.AutoCancelCancel),
					Currency = new ulong?(ServerAuctionHouseManager.GetDepositCost(this.m_auction)),
					CurrencyLabel = "Deposit:"
				};
				ClientGameManager.UIManager.ItemConfirmationDialog.Init(opts);
			}
		}

		// Token: 0x060065CE RID: 26062 RVA: 0x0008498E File Offset: 0x00082B8E
		private void CancelCallback(bool arg1, object arg2)
		{
			if (arg1 && this.CanCancel())
			{
				LocalPlayer.GameEntity.NetworkEntity.PlayerRpcHandler.Client_AuctionHouse_CancelAuction(this.m_auction.Id);
			}
		}

		// Token: 0x060065CF RID: 26063 RVA: 0x000849BA File Offset: 0x00082BBA
		private bool AutoCancelRefund()
		{
			return !this.CanRefund() || !this.m_controller || !this.m_controller.Window || !this.m_controller.Window.Visible;
		}

		// Token: 0x060065D0 RID: 26064 RVA: 0x0020EA40 File Offset: 0x0020CC40
		private bool CanRefund()
		{
			return this.m_auction != null && this.m_auction.Instance != null && LocalPlayer.GameEntity && LocalPlayer.GameEntity.GM && LocalPlayer.NetworkEntity && LocalPlayer.NetworkEntity.PlayerRpcHandler;
		}

		// Token: 0x060065D1 RID: 26065 RVA: 0x0004475B File Offset: 0x0004295B
		private void RefundAuction()
		{
		}

		// Token: 0x060065D2 RID: 26066 RVA: 0x0004475B File Offset: 0x0004295B
		private void RefundCallback(bool arg1, object arg2)
		{
		}

		// Token: 0x17001861 RID: 6241
		// (get) Token: 0x060065D3 RID: 26067 RVA: 0x00070E66 File Offset: 0x0006F066
		internal CursorType CursorType
		{
			get
			{
				return CursorType.GloveCursor;
			}
		}

		// Token: 0x17001862 RID: 6242
		// (get) Token: 0x060065D4 RID: 26068 RVA: 0x000849F8 File Offset: 0x00082BF8
		CursorType ICursor.Type
		{
			get
			{
				return this.CursorType;
			}
		}

		// Token: 0x17001863 RID: 6243
		// (get) Token: 0x060065D5 RID: 26069 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060065D6 RID: 26070 RVA: 0x0020EA98 File Offset: 0x0020CC98
		string IContextMenu.FillActionsGetTitle()
		{
			if (this.m_auction == null)
			{
				return null;
			}
			ContextMenuUI.ClearContextActions();
			string text = string.Empty;
			if (this.m_isMyAuction)
			{
				text = (this.m_isThisCharacter ? "Your Auction" : ZString.Format<string>("{0}'s Auction (Your Alt)", this.m_auction.SellerName));
				ContextMenuUI.AddContextAction(ZString.Format<string>("Cancel {0}", text), true, new Action(this.CancelAuction), null, null);
			}
			return text;
		}

		// Token: 0x060065D7 RID: 26071 RVA: 0x0020EB08 File Offset: 0x0020CD08
		internal ITooltipParameter GetInstanceTooltipParameter()
		{
			if (this.m_auction == null || this.m_auction.Instance == null)
			{
				return null;
			}
			return new ArchetypeTooltipParameter
			{
				Instance = this.m_auction.Instance
			};
		}

		// Token: 0x060065D8 RID: 26072 RVA: 0x0020EB4C File Offset: 0x0020CD4C
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_auction == null || !this.m_isMyAuction)
			{
				return null;
			}
			double num = (this.m_auction.Expiration - GameTimeReplicator.GetServerCorrectedDateTimeUtc()).TotalSeconds;
			if (num < 0.0)
			{
				num = 0.0;
			}
			string arg = this.m_isThisCharacter ? "Your Auction" : ZString.Format<string>("{0}'s Auction (Your Alt)", this.m_auction.SellerName);
			string arg2 = "(Right click to cancel)";
			return new ObjectTextTooltipParameter(this, ZString.Format<string, string, string>("{0}\nEnds in: ~{1}\n<size=80%>{2}</size>", arg, num.GetFormattedTime(true), arg2), false);
		}

		// Token: 0x17001864 RID: 6244
		// (get) Token: 0x060065D9 RID: 26073 RVA: 0x00084A00 File Offset: 0x00082C00
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17001865 RID: 6245
		// (get) Token: 0x060065DA RID: 26074 RVA: 0x00084A0E File Offset: 0x00082C0E
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x060065DB RID: 26075 RVA: 0x0020EBEC File Offset: 0x0020CDEC
		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left && this.m_controller && this.m_auction != null)
			{
				this.m_controller.SelectItem(this.m_auction);
				if (ClientGameManager.UIManager)
				{
					ClientGameManager.UIManager.PlayDefaultClick();
				}
			}
		}

		// Token: 0x060065DC RID: 26076 RVA: 0x00084A16 File Offset: 0x00082C16
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			this.m_chatWasFocusedOnPointerDown = UIManager.IsChatActive;
		}

		// Token: 0x060065DD RID: 26077 RVA: 0x0020EC40 File Offset: 0x0020CE40
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			if (this.m_mouseHover && this.m_chatWasFocusedOnPointerDown && this.m_auction != null && this.m_auction.Instance != null && eventData.button == PointerEventData.InputButton.Left && ClientGameManager.InputManager != null && ClientGameManager.InputManager.HoldingShift && ClientGameManager.UIManager && !ClientGameManager.UIManager.IsDragging)
			{
				UIManager.ActiveChatInput.AddInstanceLink(this.m_auction.Instance);
			}
			this.m_chatWasFocusedOnPointerDown = false;
		}

		// Token: 0x060065DE RID: 26078 RVA: 0x00084A23 File Offset: 0x00082C23
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			this.m_mouseHover = true;
			this.RefreshHover();
		}

		// Token: 0x060065DF RID: 26079 RVA: 0x00084A32 File Offset: 0x00082C32
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			this.m_mouseHover = false;
			this.RefreshHover();
		}

		// Token: 0x060065E2 RID: 26082 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04005881 RID: 22657
		public const float kMyAuctionIndicatorAlpha = 0.5f;

		// Token: 0x04005882 RID: 22658
		private const string kYourAuction = "Your Auction";

		// Token: 0x04005883 RID: 22659
		private const string kYourAltsAuction = "{0}'s Auction (Your Alt)";

		// Token: 0x04005884 RID: 22660
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04005885 RID: 22661
		[SerializeField]
		private ArchetypeIconUI m_archetypeIcon;

		// Token: 0x04005886 RID: 22662
		[SerializeField]
		private TextMeshProUGUI m_itemLabel;

		// Token: 0x04005887 RID: 22663
		[SerializeField]
		private TextMeshProUGUI m_timeLeftLabel;

		// Token: 0x04005888 RID: 22664
		[SerializeField]
		private TextMeshProUGUI m_sellerLabel;

		// Token: 0x04005889 RID: 22665
		[SerializeField]
		private TextMeshProUGUI m_countLabel;

		// Token: 0x0400588A RID: 22666
		[SerializeField]
		private TextMeshProUGUI m_bidLabel;

		// Token: 0x0400588B RID: 22667
		[SerializeField]
		private CurrencyDisplayPanelUI m_currentBid;

		// Token: 0x0400588C RID: 22668
		[SerializeField]
		private CurrencyDisplayPanelUI m_buyItNow;

		// Token: 0x0400588D RID: 22669
		[SerializeField]
		private Image m_frame;

		// Token: 0x0400588E RID: 22670
		[SerializeField]
		private Image m_hover;

		// Token: 0x0400588F RID: 22671
		[SerializeField]
		private Image m_disabledPanel;

		// Token: 0x04005890 RID: 22672
		[SerializeField]
		private Image m_deathIcon;

		// Token: 0x04005891 RID: 22673
		[SerializeField]
		private Image m_bgHighlight;

		// Token: 0x04005892 RID: 22674
		[SerializeField]
		private Image[] m_selfIndicators;

		// Token: 0x04005893 RID: 22675
		[SerializeField]
		private GameObject m_isWinningOverlay;

		// Token: 0x04005894 RID: 22676
		[SerializeField]
		private GameObject m_pricePerItemPanel;

		// Token: 0x04005895 RID: 22677
		private AuctionHouseForSaleList m_controller;

		// Token: 0x04005896 RID: 22678
		private AuctionRecord m_auction;

		// Token: 0x04005897 RID: 22679
		private bool m_isWinningAuction;

		// Token: 0x04005898 RID: 22680
		private bool m_isThisCharacter;

		// Token: 0x04005899 RID: 22681
		private bool m_isMyAuction;

		// Token: 0x0400589A RID: 22682
		private bool m_mouseHover;

		// Token: 0x0400589B RID: 22683
		private bool m_isSelected;

		// Token: 0x0400589C RID: 22684
		private int? m_stackCount;

		// Token: 0x0400589D RID: 22685
		private bool m_chatWasFocusedOnPointerDown;

		// Token: 0x0400589E RID: 22686
		private static readonly int[] kHourThresholds = new int[]
		{
			24,
			12,
			6,
			2,
			1
		};

		// Token: 0x0400589F RID: 22687
		private static readonly int[] kMinuteThreshold = new int[]
		{
			30,
			20,
			10,
			5
		};
	}
}
