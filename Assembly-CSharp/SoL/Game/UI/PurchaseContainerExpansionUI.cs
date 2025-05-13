using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Messages;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI
{
	// Token: 0x020008CC RID: 2252
	public class PurchaseContainerExpansionUI : MonoBehaviour
	{
		// Token: 0x060041D3 RID: 16851 RVA: 0x0006C795 File Offset: 0x0006A995
		private void Awake()
		{
			if (this.m_purchaseButton)
			{
				this.m_purchaseButton.onClick.AddListener(new UnityAction(this.PurchaseClicked));
			}
			UIManager.AvailableCurrencyChanged += this.UIManagerOnAvailableCurrencyChanged;
		}

		// Token: 0x060041D4 RID: 16852 RVA: 0x0006C7D1 File Offset: 0x0006A9D1
		private void OnDestroy()
		{
			if (this.m_purchaseButton)
			{
				this.m_purchaseButton.onClick.RemoveListener(new UnityAction(this.PurchaseClicked));
			}
			UIManager.AvailableCurrencyChanged -= this.UIManagerOnAvailableCurrencyChanged;
		}

		// Token: 0x060041D5 RID: 16853 RVA: 0x0006C80D File Offset: 0x0006AA0D
		public void Init(UniversalContainerUI controller, BankProfile bankProfile)
		{
			this.m_controller = controller;
			this.m_bankProfile = bankProfile;
			this.RefreshUI();
		}

		// Token: 0x060041D6 RID: 16854 RVA: 0x001908A0 File Offset: 0x0018EAA0
		private void PurchaseClicked()
		{
			if (LocalPlayer.GameEntity == null || LocalPlayer.NetworkEntity == null || LocalPlayer.NetworkEntity.PlayerRpcHandler == null)
			{
				return;
			}
			if (this.GetTotalCurrency() < this.m_costOfNextPurchase)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Not enough money!");
				return;
			}
			if (ClientGameManager.UIManager == null || ClientGameManager.UIManager.ConfirmationDialog == null)
			{
				return;
			}
			CurrencyConverter currencyConverter = new CurrencyConverter(this.m_costOfNextPurchase);
			DialogOptions opts = new DialogOptions
			{
				Title = "Purchase Expansion",
				Text = ZString.Format<string>("Are you sure you want to spend <b>{0}</b> for an additional row of slots?", currencyConverter.ToString()),
				ConfirmationText = "Yes",
				CancelText = "No",
				Callback = new Action<bool, object>(this.PurchaseConfirmation)
			};
			ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
		}

		// Token: 0x060041D7 RID: 16855 RVA: 0x00190998 File Offset: 0x0018EB98
		private void PurchaseConfirmation(bool answer, object arg2)
		{
			if (!answer || LocalPlayer.GameEntity == null || LocalPlayer.NetworkEntity == null || LocalPlayer.NetworkEntity.PlayerRpcHandler == null)
			{
				return;
			}
			if (this.GetTotalCurrency() < this.m_costOfNextPurchase)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Not enough money!");
				return;
			}
			this.m_purchaseButton.interactable = false;
			LocalPlayer.NetworkEntity.PlayerRpcHandler.PurchaseContainerExpansionRequest(this.m_controller.Instance.Id);
		}

		// Token: 0x060041D8 RID: 16856 RVA: 0x00190A20 File Offset: 0x0018EC20
		private ulong GetTotalCurrency()
		{
			if (this.m_controller == null || this.m_controller.Instance == null)
			{
				return 0UL;
			}
			ulong num = this.m_controller.Instance.Currency;
			if (LocalPlayer.GameEntity != null && !LocalPlayer.GameEntity.IsMissingBag)
			{
				num += LocalPlayer.GameEntity.CollectionController.Inventory.Currency;
			}
			return num;
		}

		// Token: 0x060041D9 RID: 16857 RVA: 0x00190A90 File Offset: 0x0018EC90
		public void RefreshUI()
		{
			if (this.m_bankProfile == null || this.m_controller == null || this.m_controller.Instance == null)
			{
				base.gameObject.SetActive(false);
				return;
			}
			bool active = this.m_bankProfile.CanPurchaseMore(this.m_controller.Instance.ExpansionsPurchased, out this.m_costOfNextPurchase);
			this.m_currency.UpdateCoin(this.m_costOfNextPurchase);
			this.m_purchaseButton.interactable = (this.GetTotalCurrency() >= this.m_costOfNextPurchase);
			base.gameObject.SetActive(active);
		}

		// Token: 0x060041DA RID: 16858 RVA: 0x0006C823 File Offset: 0x0006AA23
		private void UIManagerOnAvailableCurrencyChanged()
		{
			if (this.m_controller && this.m_controller.IsShown)
			{
				this.RefreshUI();
			}
		}

		// Token: 0x04003F0D RID: 16141
		[SerializeField]
		private CurrencyDisplayPanelUI m_currency;

		// Token: 0x04003F0E RID: 16142
		[SerializeField]
		private SolButton m_purchaseButton;

		// Token: 0x04003F0F RID: 16143
		private UniversalContainerUI m_controller;

		// Token: 0x04003F10 RID: 16144
		private BankProfile m_bankProfile;

		// Token: 0x04003F11 RID: 16145
		private ulong m_costOfNextPurchase;
	}
}
