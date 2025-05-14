using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Objects.Containers;
using SoL.Game.Transactions;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI
{
	// Token: 0x0200086E RID: 2158
	public class CurrencyDisplayPanelUI : MonoBehaviour, IContextMenu, IInteractiveBase
	{
		// Token: 0x140000BA RID: 186
		// (add) Token: 0x06003E9B RID: 16027 RVA: 0x00185860 File Offset: 0x00183A60
		// (remove) Token: 0x06003E9C RID: 16028 RVA: 0x00185898 File Offset: 0x00183A98
		public event Action<ulong> ValueChanged;

		// Token: 0x140000BB RID: 187
		// (add) Token: 0x06003E9D RID: 16029 RVA: 0x001858D0 File Offset: 0x00183AD0
		// (remove) Token: 0x06003E9E RID: 16030 RVA: 0x00185908 File Offset: 0x00183B08
		public event Action EditChanged;

		// Token: 0x17000E76 RID: 3702
		// (get) Token: 0x06003E9F RID: 16031 RVA: 0x0006A617 File Offset: 0x00068817
		public ulong TotalCurrency
		{
			get
			{
				return this.m_currency.TotalCurrency;
			}
		}

		// Token: 0x17000E77 RID: 3703
		// (get) Token: 0x06003EA0 RID: 16032 RVA: 0x0006A624 File Offset: 0x00068824
		public ulong MinimumCurrency
		{
			get
			{
				return this.m_minimumCurrency;
			}
		}

		// Token: 0x17000E78 RID: 3704
		// (get) Token: 0x06003EA1 RID: 16033 RVA: 0x0006A62C File Offset: 0x0006882C
		// (set) Token: 0x06003EA2 RID: 16034 RVA: 0x0006A634 File Offset: 0x00068834
		public ContainerInstance Container { get; set; }

		// Token: 0x17000E79 RID: 3705
		// (get) Token: 0x06003EA3 RID: 16035 RVA: 0x0006A63D File Offset: 0x0006883D
		// (set) Token: 0x06003EA4 RID: 16036 RVA: 0x0006A645 File Offset: 0x00068845
		public bool IsEditing
		{
			get
			{
				return this.m_isEditing;
			}
			set
			{
				if (this.m_isEditing == value)
				{
					return;
				}
				this.m_isEditing = value;
				Action editChanged = this.EditChanged;
				if (editChanged == null)
				{
					return;
				}
				editChanged();
			}
		}

		// Token: 0x17000E7A RID: 3706
		// (get) Token: 0x06003EA5 RID: 16037 RVA: 0x0006A668 File Offset: 0x00068868
		private bool PlayerCanDepositWithdraw
		{
			get
			{
				return this.m_containerAllowsDepositWithdraw && LocalPlayer.GameEntity && !LocalPlayer.GameEntity.IsMissingBag;
			}
		}

		// Token: 0x17000E7B RID: 3707
		// (get) Token: 0x06003EA6 RID: 16038 RVA: 0x0006A68D File Offset: 0x0006888D
		// (set) Token: 0x06003EA7 RID: 16039 RVA: 0x0006A695 File Offset: 0x00068895
		public string EditCurrencyTitle { get; set; } = "Set Amount";

		// Token: 0x06003EA8 RID: 16040 RVA: 0x00185940 File Offset: 0x00183B40
		private void Start()
		{
			if (this.m_rectTransform)
			{
				this.m_defaultWidth = this.m_rectTransform.sizeDelta.x;
			}
			if (this.m_editButton)
			{
				this.m_editButton.onClick.AddListener(new UnityAction(this.EditCurrencyClicked));
			}
		}

		// Token: 0x06003EA9 RID: 16041 RVA: 0x0006A69E File Offset: 0x0006889E
		private void OnDestroy()
		{
			if (this.m_editButton)
			{
				this.m_editButton.onClick.RemoveListener(new UnityAction(this.EditCurrencyClicked));
			}
		}

		// Token: 0x06003EAA RID: 16042 RVA: 0x0018599C File Offset: 0x00183B9C
		public void SetLimits(ulong minimumCurrency, ulong maximumCurrency)
		{
			this.m_minimumCurrency = minimumCurrency;
			this.m_maximumCurrency = maximumCurrency;
			if (this.TotalCurrency < this.m_minimumCurrency)
			{
				this.UpdateCoin(this.m_minimumCurrency);
				return;
			}
			if (this.TotalCurrency > this.m_maximumCurrency)
			{
				this.UpdateCoin(this.m_maximumCurrency);
			}
		}

		// Token: 0x06003EAB RID: 16043 RVA: 0x0006A6C9 File Offset: 0x000688C9
		public void SetLimitsUpdateCoin(ulong value, ulong minimumCurrency, ulong maximumCurrency)
		{
			this.m_minimumCurrency = minimumCurrency;
			this.m_maximumCurrency = maximumCurrency;
			if (value < this.m_minimumCurrency)
			{
				value = this.m_minimumCurrency;
			}
			else if (value > this.m_maximumCurrency)
			{
				value = this.m_maximumCurrency;
			}
			this.UpdateCoin(value);
		}

		// Token: 0x06003EAC RID: 16044 RVA: 0x001859EC File Offset: 0x00183BEC
		private void EditCurrencyClicked()
		{
			if (this.m_containerAllowsDepositWithdraw)
			{
				if (ClientGameManager.UIManager && ClientGameManager.UIManager.ContextMenu)
				{
					this.FillContextMenu();
					ClientGameManager.UIManager.ContextMenu.Init("Deposit/Withdraw");
					return;
				}
			}
			else
			{
				SelectCurrencyOptions opts = new SelectCurrencyOptions
				{
					Title = this.EditCurrencyTitle,
					AllowableCurrency = this.m_maximumCurrency,
					InitialCurrency = this.TotalCurrency,
					MinimumCurrency = this.m_minimumCurrency,
					Callback = new Action<bool, object>(this.EditCurrencyCallback),
					ParentWindow = this.m_parentWindow,
					PosOverride = this.GetPosOverride()
				};
				ClientGameManager.UIManager.CurrencyPickerDialog.Init(opts);
				this.IsEditing = true;
			}
		}

		// Token: 0x06003EAD RID: 16045 RVA: 0x00185AC4 File Offset: 0x00183CC4
		private void EditCurrencyCallback(bool answer, object obj)
		{
			if (answer)
			{
				ulong value = (ulong)obj;
				this.UpdateCoin(value);
			}
			this.IsEditing = false;
		}

		// Token: 0x06003EAE RID: 16046 RVA: 0x00185AEC File Offset: 0x00183CEC
		public void UpdateCoin(ulong value)
		{
			this.m_currency = new CurrencyConverter(value);
			this.m_gold.UpdateValue(this.m_currency.Gold, false);
			this.m_silver.UpdateValue((ulong)this.m_currency.Silver, this.m_currency.Gold > 0UL);
			this.m_copper.UpdateValue((ulong)this.m_currency.Copper, false);
			this.m_event.TogglePanel(false);
			Action<ulong> valueChanged = this.ValueChanged;
			if (valueChanged == null)
			{
				return;
			}
			valueChanged(value);
		}

		// Token: 0x06003EAF RID: 16047 RVA: 0x00185B78 File Offset: 0x00183D78
		public void UpdateEventCurrency(ulong value)
		{
			this.m_currency = new CurrencyConverter(value);
			this.m_event.UpdateValue(value, false);
			this.m_gold.TogglePanel(false);
			this.m_silver.TogglePanel(false);
			this.m_copper.TogglePanel(false);
			Action<ulong> valueChanged = this.ValueChanged;
			if (valueChanged == null)
			{
				return;
			}
			valueChanged(value);
		}

		// Token: 0x06003EB0 RID: 16048 RVA: 0x00185BD4 File Offset: 0x00183DD4
		public void InitCoinDisplay(ContainerType containerType)
		{
			this.m_containerAllowsDepositWithdraw = containerType.AllowCurrencyDepositWithrdaw();
			this.RefreshVisuals();
			if (containerType <= ContainerType.PersonalBank)
			{
				if (containerType != ContainerType.Inventory && containerType != ContainerType.PersonalBank)
				{
					return;
				}
			}
			else if (containerType - ContainerType.TradeOutgoing > 1 && containerType - ContainerType.PostOutgoing > 1 && containerType != ContainerType.Bank)
			{
				return;
			}
			this.m_gold.HideWhenZero = false;
			this.m_silver.HideWhenZero = false;
			this.m_copper.HideWhenZero = false;
		}

		// Token: 0x06003EB1 RID: 16049 RVA: 0x0006A704 File Offset: 0x00068904
		public void InitCoinDisplay(bool allowDepositWithdraw, bool hideGoldWhenZero, bool hideSilverWhenZero, bool hideCopperWhenZero)
		{
			this.m_containerAllowsDepositWithdraw = allowDepositWithdraw;
			this.m_gold.HideWhenZero = hideGoldWhenZero;
			this.m_silver.HideWhenZero = hideSilverWhenZero;
			this.m_copper.HideWhenZero = hideCopperWhenZero;
			this.RefreshVisuals();
		}

		// Token: 0x06003EB2 RID: 16050 RVA: 0x00185C38 File Offset: 0x00183E38
		private void RefreshVisuals()
		{
			if (this.m_rectTransform && this.m_ellipsisRectTransform)
			{
				Vector2 sizeDelta = this.m_rectTransform.sizeDelta;
				sizeDelta.x = ((this.m_containerAllowsDepositWithdraw && this.m_ellipsisRectTransform) ? (this.m_defaultWidth + this.m_ellipsisRectTransform.sizeDelta.x) : this.m_defaultWidth);
				this.m_rectTransform.sizeDelta = sizeDelta;
				this.m_ellipsisRectTransform.gameObject.SetActive(this.m_containerAllowsDepositWithdraw);
			}
		}

		// Token: 0x06003EB3 RID: 16051 RVA: 0x00185CC8 File Offset: 0x00183EC8
		private Vector3? GetPosOverride()
		{
			Vector3? result = null;
			if (this.m_editButton)
			{
				RectTransform rectTransform = this.m_editButton.transform as RectTransform;
				if (rectTransform != null)
				{
					result = new Vector3?(rectTransform.GetWorldCorner(RectTransformCorner.LowerLeft));
				}
			}
			return result;
		}

		// Token: 0x17000E7C RID: 3708
		// (get) Token: 0x06003EB4 RID: 16052 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06003EB5 RID: 16053 RVA: 0x0006A738 File Offset: 0x00068938
		public string FillActionsGetTitle()
		{
			if (!this.m_containerAllowsDepositWithdraw)
			{
				return null;
			}
			this.FillContextMenu();
			return "Deposit/Withdraw";
		}

		// Token: 0x06003EB6 RID: 16054 RVA: 0x00185D10 File Offset: 0x00183F10
		private void FillContextMenu()
		{
			Vector3? posOverride = this.GetPosOverride();
			ContextMenuUI.ClearContextActions();
			ContextMenuUI.AddContextAction("Deposit", this.PlayerCanDepositWithdraw && LocalPlayer.GameEntity.CollectionController.Inventory.Currency > 0UL, delegate()
			{
				SelectCurrencyOptions opts = new SelectCurrencyOptions
				{
					Title = "Deposit",
					AllowableCurrency = LocalPlayer.GameEntity.CollectionController.Inventory.Currency,
					Callback = new Action<bool, object>(this.DepositCallback),
					ParentWindow = this.m_parentWindow,
					PosOverride = posOverride
				};
				ClientGameManager.UIManager.CurrencyPickerDialog.Init(opts);
				this.IsEditing = true;
			}, null, null);
			ContextMenuUI.AddContextAction("Withdraw", this.PlayerCanDepositWithdraw && this.m_currency.TotalCurrency > 0UL, delegate()
			{
				SelectCurrencyOptions opts = new SelectCurrencyOptions
				{
					Title = "Withdraw",
					AllowableCurrency = this.m_currency.TotalCurrency,
					Callback = new Action<bool, object>(this.WithdrawCallback),
					ParentWindow = this.m_parentWindow,
					PosOverride = posOverride
				};
				ClientGameManager.UIManager.CurrencyPickerDialog.Init(opts);
				this.IsEditing = true;
			}, null, null);
		}

		// Token: 0x06003EB7 RID: 16055 RVA: 0x00185DA8 File Offset: 0x00183FA8
		private void DepositCallback(bool answer, object obj)
		{
			if (answer)
			{
				ulong num = (ulong)obj;
				if (num > 0UL && LocalPlayer.GameEntity.CollectionController.Inventory.Currency >= num)
				{
					CurrencyTransaction toRemove = new CurrencyTransaction
					{
						Add = false,
						Amount = num,
						TargetContainer = ContainerType.Inventory.ToString()
					};
					CurrencyTransaction toAdd = new CurrencyTransaction
					{
						Add = true,
						Amount = num,
						TargetContainer = this.Container.Id
					};
					LocalPlayer.NetworkEntity.PlayerRpcHandler.CurrencyTransferRequest(toRemove, toAdd);
				}
			}
			this.IsEditing = false;
		}

		// Token: 0x06003EB8 RID: 16056 RVA: 0x00185E58 File Offset: 0x00184058
		private void WithdrawCallback(bool answer, object obj)
		{
			if (answer)
			{
				ulong num = (ulong)obj;
				if (num > 0UL && this.Container.Currency >= num)
				{
					CurrencyTransaction toRemove = new CurrencyTransaction
					{
						Add = false,
						Amount = num,
						TargetContainer = this.Container.Id
					};
					CurrencyTransaction toAdd = new CurrencyTransaction
					{
						Add = true,
						Amount = num,
						TargetContainer = ContainerType.Inventory.ToString()
					};
					LocalPlayer.NetworkEntity.PlayerRpcHandler.CurrencyTransferRequest(toRemove, toAdd);
				}
			}
			this.IsEditing = false;
		}

		// Token: 0x06003EBA RID: 16058 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003CA8 RID: 15528
		[SerializeField]
		private CurrencyDisplayPanelUI.CoinData m_gold;

		// Token: 0x04003CA9 RID: 15529
		[SerializeField]
		private CurrencyDisplayPanelUI.CoinData m_silver;

		// Token: 0x04003CAA RID: 15530
		[SerializeField]
		private CurrencyDisplayPanelUI.CoinData m_copper;

		// Token: 0x04003CAB RID: 15531
		[SerializeField]
		private CurrencyDisplayPanelUI.CoinData m_event;

		// Token: 0x04003CAC RID: 15532
		[SerializeField]
		private UIWindow m_parentWindow;

		// Token: 0x04003CAD RID: 15533
		[SerializeField]
		private RectTransform m_ellipsisRectTransform;

		// Token: 0x04003CAE RID: 15534
		[SerializeField]
		private RectTransform m_rectTransform;

		// Token: 0x04003CAF RID: 15535
		[SerializeField]
		private SolButton m_editButton;

		// Token: 0x04003CB0 RID: 15536
		private ulong m_minimumCurrency;

		// Token: 0x04003CB1 RID: 15537
		private ulong m_maximumCurrency = 9999999UL;

		// Token: 0x04003CB3 RID: 15539
		private CurrencyConverter m_currency;

		// Token: 0x04003CB4 RID: 15540
		private bool m_containerAllowsDepositWithdraw;

		// Token: 0x04003CB5 RID: 15541
		private float m_defaultWidth = 142f;

		// Token: 0x04003CB6 RID: 15542
		private bool m_isEditing;

		// Token: 0x04003CB8 RID: 15544
		private const string kContextMenuTitle = "Deposit/Withdraw";

		// Token: 0x0200086F RID: 2159
		[Serializable]
		private class CoinData
		{
			// Token: 0x17000E7D RID: 3709
			// (set) Token: 0x06003EBB RID: 16059 RVA: 0x0006A779 File Offset: 0x00068979
			public bool HideWhenZero
			{
				set
				{
					this.m_hideWhenZero = value;
				}
			}

			// Token: 0x06003EBC RID: 16060 RVA: 0x00185EFC File Offset: 0x001840FC
			public void UpdateValue(ulong value, bool requireShow = false)
			{
				this.m_label.SetTextFormat("{0:N0}", value);
				bool isEnabled = !this.m_hideWhenZero || requireShow || value > 0UL;
				this.TogglePanel(isEnabled);
			}

			// Token: 0x06003EBD RID: 16061 RVA: 0x0006A782 File Offset: 0x00068982
			public void TogglePanel(bool isEnabled)
			{
				if (this.m_panel.gameObject.activeSelf != isEnabled)
				{
					this.m_panel.gameObject.SetActive(isEnabled);
				}
			}

			// Token: 0x04003CB9 RID: 15545
			[SerializeField]
			private RectTransform m_panel;

			// Token: 0x04003CBA RID: 15546
			[SerializeField]
			private TextMeshProUGUI m_label;

			// Token: 0x04003CBB RID: 15547
			[SerializeField]
			private bool m_hideWhenZero;
		}
	}
}
