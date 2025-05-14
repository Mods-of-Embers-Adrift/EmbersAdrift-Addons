using System;
using SoL.Game.Interactives;
using SoL.Game.Objects.Containers;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI.Merchants
{
	// Token: 0x02000964 RID: 2404
	public abstract class BaseMerchantUI<T> : MonoBehaviour where T : BaseNetworkedInteractive
	{
		// Token: 0x17000FD4 RID: 4052
		// (get) Token: 0x0600473F RID: 18239 RVA: 0x00070080 File Offset: 0x0006E280
		public UniversalContainerUI OutgoingUI
		{
			get
			{
				return this.m_outgoingUI;
			}
		}

		// Token: 0x17000FD5 RID: 4053
		// (get) Token: 0x06004740 RID: 18240 RVA: 0x00070088 File Offset: 0x0006E288
		public UIWindow UIWindow
		{
			get
			{
				return this.m_uiWindow;
			}
		}

		// Token: 0x17000FD6 RID: 4054
		// (get) Token: 0x06004741 RID: 18241 RVA: 0x00070090 File Offset: 0x0006E290
		// (set) Token: 0x06004742 RID: 18242 RVA: 0x00070098 File Offset: 0x0006E298
		public T Interactive { get; set; }

		// Token: 0x17000FD7 RID: 4055
		// (get) Token: 0x06004743 RID: 18243
		protected abstract ContainerType m_containerType { get; }

		// Token: 0x06004744 RID: 18244
		protected abstract bool ButtonClickedInternal();

		// Token: 0x06004745 RID: 18245 RVA: 0x00045BCA File Offset: 0x00043DCA
		internal virtual bool UseEventCurrency()
		{
			return false;
		}

		// Token: 0x140000DC RID: 220
		// (add) Token: 0x06004746 RID: 18246 RVA: 0x001A6610 File Offset: 0x001A4810
		// (remove) Token: 0x06004747 RID: 18247 RVA: 0x001A6648 File Offset: 0x001A4848
		internal event Action AvailableCurrencyChanged;

		// Token: 0x17000FD8 RID: 4056
		// (get) Token: 0x06004748 RID: 18248 RVA: 0x000700A1 File Offset: 0x0006E2A1
		// (set) Token: 0x06004749 RID: 18249 RVA: 0x000700A9 File Offset: 0x0006E2A9
		internal ulong AvailableCurrency
		{
			get
			{
				return this.m_availableCurrencyValue;
			}
			private set
			{
				if (this.m_availableCurrencyValue == value)
				{
					return;
				}
				this.m_availableCurrencyValue = value;
				Action availableCurrencyChanged = this.AvailableCurrencyChanged;
				if (availableCurrencyChanged == null)
				{
					return;
				}
				availableCurrencyChanged();
			}
		}

		// Token: 0x0600474A RID: 18250 RVA: 0x001A6680 File Offset: 0x001A4880
		protected virtual void Awake()
		{
			if (this.m_button)
			{
				this.m_button.interactable = false;
				this.m_button.onClick.AddListener(new UnityAction(this.ButtonClicked));
			}
			BaseCollectionController.InteractiveStationChanged += this.CollectionControllerOnInteractiveStationChanged;
			if (this.m_availableCurrency)
			{
				this.m_availableCurrency.InitCoinDisplay(false, true, true, false);
			}
		}

		// Token: 0x0600474B RID: 18251 RVA: 0x000700CC File Offset: 0x0006E2CC
		protected virtual void Start()
		{
			if (LocalPlayer.GameEntity.CollectionController.TryGetInstance(this.m_containerType, out this.m_containerInstance))
			{
				this.m_containerInstance.ContentsChanged += this.ContainerInstanceOnContentsChanged;
			}
			this.ContainerInstanceOnContentsChanged();
		}

		// Token: 0x0600474C RID: 18252 RVA: 0x001A66F0 File Offset: 0x001A48F0
		protected virtual void OnDestroy()
		{
			if (this.m_button)
			{
				this.m_button.onClick.RemoveListener(new UnityAction(this.ButtonClicked));
			}
			if (this.m_containerInstance != null)
			{
				this.m_containerInstance.ContentsChanged -= this.ContainerInstanceOnContentsChanged;
			}
			BaseCollectionController.InteractiveStationChanged -= this.CollectionControllerOnInteractiveStationChanged;
		}

		// Token: 0x0600474D RID: 18253 RVA: 0x00070109 File Offset: 0x0006E309
		private void ButtonClicked()
		{
			if (this.m_containerInstance != null && this.ButtonClickedInternal())
			{
				this.m_containerInstance.LockFlags |= ContainerLockFlags.UI;
			}
		}

		// Token: 0x0600474E RID: 18254 RVA: 0x0007012E File Offset: 0x0006E32E
		public void InteractionComplete()
		{
			this.m_containerInstance.LockFlags &= ~ContainerLockFlags.UI;
			this.ContainerInstanceOnContentsChanged();
		}

		// Token: 0x0600474F RID: 18255 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void ContainerInstanceOnContentsChanged()
		{
		}

		// Token: 0x06004750 RID: 18256 RVA: 0x001A6758 File Offset: 0x001A4958
		private void CollectionControllerOnInteractiveStationChanged()
		{
			if (LocalPlayer.GameEntity.CollectionController.InteractiveStation == null)
			{
				this.Interactive = default(T);
				this.UIWindow.HideCallback = null;
				this.LeavingWindow();
				if (this.UIWindow.Visible)
				{
					this.UIWindow.Hide(false);
					return;
				}
			}
			else if (LocalPlayer.GameEntity.CollectionController.InteractiveStation.ContainerType == this.m_containerType)
			{
				this.Interactive = (LocalPlayer.GameEntity.CollectionController.InteractiveStation as T);
				this.UIWindow.HideCallback = LocalPlayer.GameEntity.CollectionController.InteractiveStation.HideCallback;
				this.UIWindow.Show(false);
				this.Refresh();
			}
		}

		// Token: 0x06004751 RID: 18257 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void Refresh()
		{
		}

		// Token: 0x06004752 RID: 18258 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void LeavingWindow()
		{
		}

		// Token: 0x06004753 RID: 18259 RVA: 0x001A6824 File Offset: 0x001A4A24
		protected void RefreshAvailableCurrency()
		{
			ulong availableCurrency = 0UL;
			if (this.m_availableCurrency && LocalPlayer.GameEntity)
			{
				CurrencySources currencySources;
				ulong availableCurrencyForInteractiveStation = LocalPlayer.GameEntity.GetAvailableCurrencyForInteractiveStation(out currencySources);
				availableCurrency = availableCurrencyForInteractiveStation;
				if (this.UseEventCurrency())
				{
					this.m_availableCurrency.UpdateEventCurrency(availableCurrencyForInteractiveStation);
					this.ToggleAvailableSourcePanels(false, false, false);
				}
				else
				{
					this.m_availableCurrency.UpdateCoin(availableCurrencyForInteractiveStation);
					bool inventoryAndBank = currencySources == (CurrencySources.Inventory | CurrencySources.PersonalBank);
					bool inventoryOnly = currencySources == CurrencySources.Inventory;
					bool bankOnly = currencySources == CurrencySources.PersonalBank;
					this.ToggleAvailableSourcePanels(inventoryAndBank, inventoryOnly, bankOnly);
				}
			}
			else
			{
				this.ToggleAvailableSourcePanels(false, false, false);
			}
			this.AvailableCurrency = availableCurrency;
		}

		// Token: 0x06004754 RID: 18260 RVA: 0x001A68B8 File Offset: 0x001A4AB8
		private void ToggleAvailableSourcePanels(bool inventoryAndBank, bool inventoryOnly, bool bankOnly)
		{
			if (this.m_inventoryAndBankPanel)
			{
				this.m_inventoryAndBankPanel.SetActive(inventoryAndBank);
			}
			if (this.m_inventoryOnlyPanel)
			{
				this.m_inventoryOnlyPanel.SetActive(inventoryOnly);
			}
			if (this.m_bankOnlyPanel)
			{
				this.m_bankOnlyPanel.SetActive(bankOnly);
			}
		}

		// Token: 0x04004327 RID: 17191
		[SerializeField]
		protected SolButton m_button;

		// Token: 0x04004328 RID: 17192
		[SerializeField]
		private UniversalContainerUI m_outgoingUI;

		// Token: 0x04004329 RID: 17193
		[SerializeField]
		private UIWindow m_uiWindow;

		// Token: 0x0400432A RID: 17194
		[SerializeField]
		private CurrencyDisplayPanelUI m_availableCurrency;

		// Token: 0x0400432B RID: 17195
		[SerializeField]
		private GameObject m_inventoryAndBankPanel;

		// Token: 0x0400432C RID: 17196
		[SerializeField]
		private GameObject m_inventoryOnlyPanel;

		// Token: 0x0400432D RID: 17197
		[SerializeField]
		private GameObject m_bankOnlyPanel;

		// Token: 0x0400432E RID: 17198
		protected ContainerInstance m_containerInstance;

		// Token: 0x04004331 RID: 17201
		private ulong m_availableCurrencyValue;
	}
}
