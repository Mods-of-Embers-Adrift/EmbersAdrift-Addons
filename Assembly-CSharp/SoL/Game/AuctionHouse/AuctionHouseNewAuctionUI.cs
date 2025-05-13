using System;
using Cysharp.Text;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.UI;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.AuctionHouse
{
	// Token: 0x02000D3C RID: 3388
	public class AuctionHouseNewAuctionUI : MonoBehaviour
	{
		// Token: 0x1700186A RID: 6250
		// (get) Token: 0x060065EA RID: 26090 RVA: 0x00084ABD File Offset: 0x00082CBD
		internal ulong CurrentDeposit
		{
			get
			{
				return this.m_currentCost;
			}
		}

		// Token: 0x1700186B RID: 6251
		// (get) Token: 0x060065EB RID: 26091 RVA: 0x00084AC5 File Offset: 0x00082CC5
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

		// Token: 0x060065EC RID: 26092 RVA: 0x0020EC64 File Offset: 0x0020CE64
		private void Awake()
		{
			this.m_itemLabel.ZStringSetText("");
			this.m_submit.onClick.AddListener(new UnityAction(this.OnSubmitClicked));
			this.SetControlCanvasGroups(false);
			this.m_auctionToggle.isOn = true;
			this.m_auctionToggle.onValueChanged.AddListener(new UnityAction<bool>(this.AuctionToggleChanged));
			AuctionHouseUI.ToggleCanvasGroup(this.m_auctionCanvasGroup, true);
			this.m_auctionCurrency.EditCurrencyTitle = "Starting Bid";
			this.m_auctionCurrency.ValueChanged += this.AuctionCurrencyOnValueChanged;
			this.m_auctionCurrency.EditChanged += this.CurrencyEditChanged;
			this.m_buyItNowToggle.isOn = true;
			this.m_buyItNowToggle.onValueChanged.AddListener(new UnityAction<bool>(this.BuyItNowToggleChanged));
			AuctionHouseUI.ToggleCanvasGroup(this.m_buyItNowCanvasGroup, true);
			this.m_buyItNowCurrency.EditCurrencyTitle = "Buy It Now Price";
			this.m_buyItNowCurrency.EditChanged += this.CurrencyEditChanged;
			string arg = DeploymentBranchFlagsExtensions.IsQA() ? "Hours" : "Days";
			this.m_durationUnitLabel.ZStringSetText(arg);
			this.m_decreaseDurationButton.onClick.AddListener(new UnityAction(this.DecreaseDurationClicked));
			this.m_increaseDurationButton.onClick.AddListener(new UnityAction(this.IncreaseDurationClicked));
			this.m_currentDuration = 1;
			this.RefreshDuration();
			this.m_auctionHouseUi.AvailableCurrencyChanged += this.AuctionHouseUiOnAvailableCurrencyChanged;
			if (this.m_subscriberImagesToColor != null)
			{
				for (int i = 0; i < this.m_subscriberImagesToColor.Length; i++)
				{
					if (this.m_subscriberImagesToColor[i])
					{
						this.m_subscriberImagesToColor[i].color = UIManager.SubscriberColor;
					}
				}
			}
		}

		// Token: 0x060065ED RID: 26093 RVA: 0x00084AE2 File Offset: 0x00082CE2
		private void Start()
		{
			if (LocalPlayer.IsInitialized)
			{
				this.Init();
				return;
			}
			LocalPlayer.LocalPlayerInitialized += this.LocalPlayerOnLocalPlayerInitialized;
		}

		// Token: 0x060065EE RID: 26094 RVA: 0x0020EE2C File Offset: 0x0020D02C
		private void OnDestroy()
		{
			this.m_submit.onClick.RemoveListener(new UnityAction(this.OnSubmitClicked));
			this.m_auctionToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.AuctionToggleChanged));
			this.m_auctionCurrency.ValueChanged -= this.AuctionCurrencyOnValueChanged;
			this.m_auctionCurrency.EditChanged -= this.CurrencyEditChanged;
			this.m_buyItNowToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.BuyItNowToggleChanged));
			this.m_buyItNowCurrency.EditChanged -= this.CurrencyEditChanged;
			if (this.m_auctionContainerInstance != null)
			{
				this.m_auctionContainerInstance.ContentsChanged -= this.AuctionContainerOnContentsChanged;
			}
			this.m_auctionContainerInstance = null;
			this.m_decreaseDurationButton.onClick.RemoveListener(new UnityAction(this.DecreaseDurationClicked));
			this.m_increaseDurationButton.onClick.RemoveListener(new UnityAction(this.IncreaseDurationClicked));
			if (this.m_auctionHouseUi)
			{
				this.m_auctionHouseUi.AvailableCurrencyChanged += this.AuctionHouseUiOnAvailableCurrencyChanged;
			}
		}

		// Token: 0x060065EF RID: 26095 RVA: 0x0020EF54 File Offset: 0x0020D154
		private void Init()
		{
			ContainerInstance auctionContainerInstance;
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.AuctionOutgoing, out auctionContainerInstance))
			{
				this.m_auctionContainerInstance = auctionContainerInstance;
				this.m_auctionContainerInstance.ContentsChanged += this.AuctionContainerOnContentsChanged;
			}
			this.AuctionContainerOnContentsChanged();
		}

		// Token: 0x060065F0 RID: 26096 RVA: 0x0020EFB4 File Offset: 0x0020D1B4
		private ulong GetBuyItNowMinimum(ulong cost)
		{
			ulong minimumBid = ServerAuctionHouseManager.GetMinimumBid(new ulong?(cost), 1);
			if (minimumBid <= 0UL)
			{
				return 1UL;
			}
			return minimumBid;
		}

		// Token: 0x060065F1 RID: 26097 RVA: 0x0020EFD8 File Offset: 0x0020D1D8
		private void RefreshSubmitButton()
		{
			this.m_submit.interactable = (this.m_instanceUpForAuction != null && (this.m_auctionToggle.isOn || this.m_buyItNowToggle.isOn) && this.AvailableCurrency >= this.m_currentCost && !this.m_auctionCurrency.IsEditing && !this.m_buyItNowCurrency.IsEditing);
		}

		// Token: 0x060065F2 RID: 26098 RVA: 0x0020F040 File Offset: 0x0020D240
		private void SetControlCanvasGroups(bool canInteract)
		{
			for (int i = 0; i < this.m_controlCanvasGroups.Length; i++)
			{
				if (this.m_controlCanvasGroups[i])
				{
					this.m_controlCanvasGroups[i].interactable = canInteract;
					this.m_controlCanvasGroups[i].alpha = (canInteract ? 1f : 0.5f);
				}
			}
		}

		// Token: 0x060065F3 RID: 26099 RVA: 0x00084B03 File Offset: 0x00082D03
		private void LocalPlayerOnLocalPlayerInitialized()
		{
			LocalPlayer.LocalPlayerInitialized -= this.LocalPlayerOnLocalPlayerInitialized;
			this.Init();
		}

		// Token: 0x060065F4 RID: 26100 RVA: 0x00084B1C File Offset: 0x00082D1C
		private void IncreaseDurationClicked()
		{
			this.m_currentDuration++;
			this.RefreshDuration();
		}

		// Token: 0x060065F5 RID: 26101 RVA: 0x00084B32 File Offset: 0x00082D32
		private void DecreaseDurationClicked()
		{
			this.m_currentDuration--;
			this.RefreshDuration();
		}

		// Token: 0x060065F6 RID: 26102 RVA: 0x0020F09C File Offset: 0x0020D29C
		private void RefreshDuration()
		{
			this.m_currentDuration = Mathf.Clamp(this.m_currentDuration, 1, 5);
			this.m_durationValueLabel.ZStringSetText(this.m_currentDuration);
			int currentDuration = this.m_currentDuration;
			if (currentDuration != 1)
			{
				if (currentDuration != 5)
				{
					this.m_decreaseDurationButton.interactable = true;
					this.m_increaseDurationButton.interactable = true;
				}
				else
				{
					this.m_decreaseDurationButton.interactable = true;
					this.m_increaseDurationButton.interactable = false;
				}
			}
			else
			{
				this.m_decreaseDurationButton.interactable = false;
				this.m_increaseDurationButton.interactable = true;
			}
			this.GetUpdateDeposit();
			this.RefreshSubmitButton();
		}

		// Token: 0x060065F7 RID: 26103 RVA: 0x00084B48 File Offset: 0x00082D48
		private void AuctionHouseUiOnAvailableCurrencyChanged()
		{
			this.RefreshSubmitButton();
		}

		// Token: 0x060065F8 RID: 26104 RVA: 0x00084B48 File Offset: 0x00082D48
		private void CurrencyEditChanged()
		{
			this.RefreshSubmitButton();
		}

		// Token: 0x060065F9 RID: 26105 RVA: 0x00084B50 File Offset: 0x00082D50
		private void AuctionCurrencyOnValueChanged(ulong obj)
		{
			this.m_buyItNowCurrency.SetLimits(this.GetBuyItNowMinimum(obj), 9999999UL);
		}

		// Token: 0x060065FA RID: 26106 RVA: 0x0020F138 File Offset: 0x0020D338
		private void AuctionToggleChanged(bool arg0)
		{
			AuctionHouseUI.ToggleCanvasGroup(this.m_auctionCanvasGroup, arg0);
			this.RefreshSubmitButton();
			ulong cost = this.m_auctionToggle.isOn ? this.m_auctionCurrency.TotalCurrency : 1UL;
			this.m_buyItNowCurrency.SetLimits(this.GetBuyItNowMinimum(cost), 9999999UL);
		}

		// Token: 0x060065FB RID: 26107 RVA: 0x00084B6A File Offset: 0x00082D6A
		private void BuyItNowToggleChanged(bool arg0)
		{
			AuctionHouseUI.ToggleCanvasGroup(this.m_buyItNowCanvasGroup, arg0);
			this.RefreshSubmitButton();
		}

		// Token: 0x060065FC RID: 26108 RVA: 0x0020F18C File Offset: 0x0020D38C
		private ulong GetUpdateDeposit()
		{
			if (this.m_instanceUpForAuction != null)
			{
				ulong depositCost = ServerAuctionHouseManager.GetDepositCost(this.m_instanceUpForAuction, this.m_currentDuration);
				this.m_currentCost = depositCost;
				this.m_costCurrency.UpdateCoin(this.m_currentCost);
				return depositCost;
			}
			return 0UL;
		}

		// Token: 0x060065FD RID: 26109 RVA: 0x0020F1D0 File Offset: 0x0020D3D0
		private void AuctionContainerOnContentsChanged()
		{
			ArchetypeInstance archetypeInstance;
			if (this.m_auctionContainerInstance != null && this.m_auctionContainerInstance.TryGetInstanceForIndex(0, out archetypeInstance) && archetypeInstance != null)
			{
				this.m_instanceUpForAuction = archetypeInstance;
				ulong updateDeposit = this.GetUpdateDeposit();
				ulong num = this.m_instanceUpForAuction.GetSellPrice();
				num = (ulong)Mathf.Max(num, updateDeposit);
				this.m_auctionCurrency.SetLimitsUpdateCoin(num, 1UL, 9999999UL);
				ulong cost = this.m_auctionToggle.isOn ? num : 1UL;
				ulong buyItNowMinimum = this.GetBuyItNowMinimum(cost);
				this.m_buyItNowCurrency.SetLimitsUpdateCoin(buyItNowMinimum * 5UL, buyItNowMinimum, 9999999UL);
				string text = archetypeInstance.Archetype.GetModifiedDisplayName(archetypeInstance);
				Color color;
				if (archetypeInstance.Archetype.TryGetItemCategoryColor(ItemCategory.ColorFlags.DisplayName, out color))
				{
					text = text.Color(color);
				}
				this.m_itemLabel.ZStringSetText(text);
				this.SetControlCanvasGroups(true);
			}
			else
			{
				this.m_currentCost = 0UL;
				this.m_instanceUpForAuction = null;
				this.m_costCurrency.UpdateCoin(0UL);
				this.m_auctionCurrency.SetLimitsUpdateCoin(0UL, 0UL, 9999999UL);
				this.m_buyItNowCurrency.SetLimitsUpdateCoin(0UL, 0UL, 9999999UL);
				this.m_itemLabel.ZStringSetText("");
				this.SetControlCanvasGroups(false);
			}
			this.RefreshSubmitButton();
		}

		// Token: 0x060065FE RID: 26110 RVA: 0x00084B7E File Offset: 0x00082D7E
		private void OnSubmitClicked()
		{
			this.ValidateAndPostAuction(true);
		}

		// Token: 0x060065FF RID: 26111 RVA: 0x00084B87 File Offset: 0x00082D87
		private bool AutoCancelPost()
		{
			return !this.CanPost() || !this.m_auctionHouseUi || !this.m_auctionHouseUi.UIWindow || !this.m_auctionHouseUi.UIWindow.Visible;
		}

		// Token: 0x06006600 RID: 26112 RVA: 0x0020F314 File Offset: 0x0020D514
		private bool CanPost()
		{
			return this.m_submit.interactable && this.AvailableCurrency >= this.m_currentCost && LocalPlayer.NetworkEntity && LocalPlayer.NetworkEntity.PlayerRpcHandler && this.m_auctionContainerUi && this.m_auctionContainerUi.ContainerInstance != null && this.m_auctionContainerUi.ContainerInstance.Count > 0;
		}

		// Token: 0x06006601 RID: 26113 RVA: 0x00084BC5 File Offset: 0x00082DC5
		private void PostNewAuctionConfirmation(bool arg1, object arg2)
		{
			if (arg1)
			{
				this.ValidateAndPostAuction(false);
			}
		}

		// Token: 0x06006602 RID: 26114 RVA: 0x0020F388 File Offset: 0x0020D588
		private void ValidateAndPostAuction(bool confirm)
		{
			ArchetypeInstance archetypeInstance;
			if (this.CanPost() && this.m_auctionContainerUi.ContainerInstance.TryGetInstanceForIndex(0, out archetypeInstance))
			{
				if (confirm)
				{
					DialogOptions opts = new DialogOptions
					{
						Title = "New Auction",
						Text = "Are you sure you want to post this item for auction? The deposit will only be refunded if the item sells.",
						ConfirmationText = "Yes",
						CancelText = "NO",
						Callback = new Action<bool, object>(this.PostNewAuctionConfirmation),
						Instance = archetypeInstance,
						AutoCancel = new Func<bool>(this.AutoCancelPost),
						Currency = new ulong?(this.m_currentCost),
						CurrencyLabel = "Deposit:"
					};
					ClientGameManager.UIManager.ItemConfirmationDialog.Init(opts);
					return;
				}
				ulong? startingBid = null;
				ulong? buyNowPrice = null;
				if (this.m_auctionToggle.isOn && this.m_buyItNowToggle.isOn)
				{
					startingBid = new ulong?(this.m_auctionCurrency.TotalCurrency);
					buyNowPrice = new ulong?(this.m_buyItNowCurrency.TotalCurrency);
				}
				else if (this.m_auctionToggle.isOn)
				{
					startingBid = new ulong?(this.m_auctionCurrency.TotalCurrency);
				}
				else
				{
					if (!this.m_buyItNowToggle.isOn)
					{
						return;
					}
					buyNowPrice = new ulong?(this.m_buyItNowCurrency.TotalCurrency);
				}
				AuctionRequest auctionRequest = new AuctionRequest
				{
					InstanceId = archetypeInstance.InstanceId,
					StartingBid = startingBid,
					BuyNowPrice = buyNowPrice,
					Duration = (byte)this.m_currentDuration
				};
				LocalPlayer.NetworkEntity.PlayerRpcHandler.Client_AuctionHouse_NewAuction(auctionRequest);
			}
		}

		// Token: 0x040058A2 RID: 22690
		private const ulong kBuyItNowInitialMarkup = 5UL;

		// Token: 0x040058A3 RID: 22691
		[SerializeField]
		private AuctionHouseUI m_auctionHouseUi;

		// Token: 0x040058A4 RID: 22692
		[SerializeField]
		private UniversalContainerUI m_auctionContainerUi;

		// Token: 0x040058A5 RID: 22693
		[SerializeField]
		private TextMeshProUGUI m_itemLabel;

		// Token: 0x040058A6 RID: 22694
		[SerializeField]
		private SolButton m_submit;

		// Token: 0x040058A7 RID: 22695
		[SerializeField]
		private CanvasGroup[] m_controlCanvasGroups;

		// Token: 0x040058A8 RID: 22696
		[SerializeField]
		private SolToggle m_auctionToggle;

		// Token: 0x040058A9 RID: 22697
		[SerializeField]
		private CanvasGroup m_auctionCanvasGroup;

		// Token: 0x040058AA RID: 22698
		[SerializeField]
		private CurrencyDisplayPanelUI m_auctionCurrency;

		// Token: 0x040058AB RID: 22699
		[SerializeField]
		private SolToggle m_buyItNowToggle;

		// Token: 0x040058AC RID: 22700
		[SerializeField]
		private CanvasGroup m_buyItNowCanvasGroup;

		// Token: 0x040058AD RID: 22701
		[SerializeField]
		private CurrencyDisplayPanelUI m_buyItNowCurrency;

		// Token: 0x040058AE RID: 22702
		[SerializeField]
		private CurrencyDisplayPanelUI m_costCurrency;

		// Token: 0x040058AF RID: 22703
		[SerializeField]
		private SolButton m_decreaseDurationButton;

		// Token: 0x040058B0 RID: 22704
		[SerializeField]
		private SolButton m_increaseDurationButton;

		// Token: 0x040058B1 RID: 22705
		[SerializeField]
		private TextMeshProUGUI m_durationUnitLabel;

		// Token: 0x040058B2 RID: 22706
		[SerializeField]
		private TextMeshProUGUI m_durationValueLabel;

		// Token: 0x040058B3 RID: 22707
		[SerializeField]
		private Image[] m_subscriberImagesToColor;

		// Token: 0x040058B4 RID: 22708
		private ContainerInstance m_auctionContainerInstance;

		// Token: 0x040058B5 RID: 22709
		private ArchetypeInstance m_instanceUpForAuction;

		// Token: 0x040058B6 RID: 22710
		private ulong m_currentCost;

		// Token: 0x040058B7 RID: 22711
		private int m_currentDuration = 1;
	}
}
