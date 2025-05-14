using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Objects.Containers;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI.Merchants
{
	// Token: 0x02000966 RID: 2406
	public class EssenceConverterUI : BaseMerchantUI<InteractiveEssenceConverter>
	{
		// Token: 0x17000FDA RID: 4058
		// (get) Token: 0x0600476D RID: 18285 RVA: 0x000701E6 File Offset: 0x0006E3E6
		protected override ContainerType m_containerType
		{
			get
			{
				return ContainerType.RuneCollector;
			}
		}

		// Token: 0x0600476E RID: 18286 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool ButtonClickedInternal()
		{
			return false;
		}

		// Token: 0x0600476F RID: 18287 RVA: 0x001A6D68 File Offset: 0x001A4F68
		protected override void Awake()
		{
			base.Awake();
			base.UIWindow.ShowCalled += this.WindowShown;
			base.UIWindow.HideCalled += this.WindowHidden;
			this.m_essenceSlider.wholeNumbers = true;
			this.m_essenceSlider.onValueChanged.AddListener(new UnityAction<float>(this.EssenceSliderChanged));
			this.m_acceptButton.onClick.AddListener(new UnityAction(this.AcceptClicked));
		}

		// Token: 0x06004770 RID: 18288 RVA: 0x001A6DF0 File Offset: 0x001A4FF0
		protected override void OnDestroy()
		{
			base.OnDestroy();
			base.UIWindow.ShowCalled -= this.WindowShown;
			base.UIWindow.HideCalled -= this.WindowHidden;
			this.m_essenceSlider.onValueChanged.RemoveListener(new UnityAction<float>(this.EssenceSliderChanged));
			this.m_acceptButton.onClick.RemoveListener(new UnityAction(this.AcceptClicked));
		}

		// Token: 0x06004771 RID: 18289 RVA: 0x001A6E6C File Offset: 0x001A506C
		private void WindowShown()
		{
			if (!this.m_subscribed)
			{
				LocalPlayer.GameEntity.CollectionController.Inventory.CurrencyChanged += this.CurrencyChanged;
				LocalPlayer.GameEntity.CollectionController.PersonalBank.CurrencyChanged += this.CurrencyChanged;
				LocalPlayer.GameEntity.CollectionController.EmberStoneChanged += this.EmberStoneChanged;
				this.m_subscribed = true;
			}
			this.m_preventSliderTrigger = true;
			this.m_essenceSlider.value = 0f;
			this.m_preventSliderTrigger = false;
			this.RefreshAll();
		}

		// Token: 0x06004772 RID: 18290 RVA: 0x001A6F08 File Offset: 0x001A5108
		private void WindowHidden()
		{
			if (this.m_subscribed)
			{
				LocalPlayer.GameEntity.CollectionController.Inventory.CurrencyChanged -= this.CurrencyChanged;
				LocalPlayer.GameEntity.CollectionController.PersonalBank.CurrencyChanged -= this.CurrencyChanged;
				LocalPlayer.GameEntity.CollectionController.EmberStoneChanged -= this.EmberStoneChanged;
				this.m_subscribed = false;
			}
		}

		// Token: 0x06004773 RID: 18291 RVA: 0x000701EA File Offset: 0x0006E3EA
		private void CurrencyChanged(ulong obj)
		{
			this.RefreshAll();
		}

		// Token: 0x06004774 RID: 18292 RVA: 0x000701EA File Offset: 0x0006E3EA
		private void EmberStoneChanged()
		{
			this.RefreshAll();
		}

		// Token: 0x06004775 RID: 18293 RVA: 0x000701F2 File Offset: 0x0006E3F2
		private void EssenceSliderChanged(float arg0)
		{
			if (!this.m_preventSliderTrigger)
			{
				this.RefreshAll();
			}
		}

		// Token: 0x06004776 RID: 18294 RVA: 0x001A6F80 File Offset: 0x001A5180
		private void AcceptClicked()
		{
			if (ClientGameManager.UIManager && ClientGameManager.UIManager.ConfirmationDialog)
			{
				int num = Mathf.FloorToInt(this.m_essenceSlider.value);
				int arg;
				ulong currency;
				GlobalSettings.Values.Ashen.GetTravelEssenceConversionValues(num, out arg, out currency);
				CurrencyConverter currencyConverter = new CurrencyConverter(currency);
				DialogOptions opts = new DialogOptions
				{
					Title = "Purchase Travel Essence",
					Text = ZString.Format<int, int, string>("Are you sure you want to convert {0} Ember Essence to {1} Travel Essence for {2}?", arg, num, currencyConverter.ToString()),
					ConfirmationText = "Yes",
					CancelText = "No",
					Callback = new Action<bool, object>(this.AcceptConfirmation),
					AutoCancel = new Func<bool>(this.AutoCancel)
				};
				this.m_essenceSlider.interactable = false;
				this.m_acceptButton.interactable = false;
				ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
			}
		}

		// Token: 0x06004777 RID: 18295 RVA: 0x001A707C File Offset: 0x001A527C
		private bool AutoCancel()
		{
			InteractiveEssenceConverter interactiveEssenceConverter;
			return !LocalPlayer.GameEntity || LocalPlayer.GameEntity.CollectionController == null || !LocalPlayer.GameEntity.CollectionController.InteractiveStation || !LocalPlayer.GameEntity.CollectionController.InteractiveStation.TryGetAsType(out interactiveEssenceConverter) || !interactiveEssenceConverter.IsWithinDistance(LocalPlayer.GameEntity);
		}

		// Token: 0x06004778 RID: 18296 RVA: 0x001A70E0 File Offset: 0x001A52E0
		private void AcceptConfirmation(bool answer, object obj)
		{
			if (answer)
			{
				LocalPlayer.GameEntity.NetworkEntity.PlayerRpcHandler.PurchaseTravelEssence(Mathf.FloorToInt(this.m_essenceSlider.value));
				this.m_essenceSlider.value = 0f;
				if (EssenceConvertEffect.Instance)
				{
					EssenceConvertEffect.Instance.Trigger();
				}
			}
			this.RefreshAll();
		}

		// Token: 0x06004779 RID: 18297 RVA: 0x001A7140 File Offset: 0x001A5340
		private void RefreshAll()
		{
			base.RefreshAvailableCurrency();
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.CurrentEmberStone != null && LocalPlayer.GameEntity.CollectionController.Record != null && LocalPlayer.GameEntity.CollectionController.Record.EmberStoneData != null)
			{
				this.m_preventSliderTrigger = true;
				int maxCapacity = LocalPlayer.GameEntity.CollectionController.CurrentEmberStone.MaxCapacity;
				int count = LocalPlayer.GameEntity.CollectionController.Record.EmberStoneData.Count;
				int travelCount = LocalPlayer.GameEntity.CollectionController.Record.EmberStoneData.TravelCount;
				int a = Mathf.Clamp(maxCapacity - travelCount, 0, maxCapacity);
				int maxTravelCanPurchase = GlobalSettings.Values.Ashen.GetMaxTravelCanPurchase(count);
				this.m_essenceSlider.minValue = 0f;
				this.m_essenceSlider.maxValue = (float)Mathf.Min(a, maxTravelCanPurchase);
				int num = Mathf.FloorToInt(this.m_essenceSlider.value);
				int num2;
				ulong num3;
				GlobalSettings.Values.Ashen.GetTravelEssenceConversionValues(num, out num2, out num3);
				this.m_cost.UpdateCoin(num3);
				this.m_leftLabel.SetTextFormat("{0}", count - num2);
				this.m_rightLabel.SetTextFormat("{0}/{1}", num + travelCount, maxCapacity);
				ulong localPlayerCurrency = this.GetLocalPlayerCurrency();
				this.m_acceptButton.interactable = (num3 > 0UL && localPlayerCurrency >= num3);
				this.m_essenceSlider.interactable = true;
				this.m_preventSliderTrigger = false;
				return;
			}
			this.m_cost.UpdateCoin(0UL);
			this.m_essenceSlider.interactable = false;
			this.m_acceptButton.interactable = false;
		}

		// Token: 0x0600477A RID: 18298 RVA: 0x001A6A44 File Offset: 0x001A4C44
		private ulong GetLocalPlayerCurrency()
		{
			if (!LocalPlayer.GameEntity)
			{
				return 0UL;
			}
			CurrencySources currencySources;
			return LocalPlayer.GameEntity.GetAvailableCurrencyForInteractiveStation(out currencySources);
		}

		// Token: 0x04004338 RID: 17208
		[SerializeField]
		private Slider m_essenceSlider;

		// Token: 0x04004339 RID: 17209
		[SerializeField]
		private SolButton m_acceptButton;

		// Token: 0x0400433A RID: 17210
		[SerializeField]
		private CurrencyDisplayPanelUI m_cost;

		// Token: 0x0400433B RID: 17211
		[SerializeField]
		private TextMeshProUGUI m_leftLabel;

		// Token: 0x0400433C RID: 17212
		[SerializeField]
		private TextMeshProUGUI m_rightLabel;

		// Token: 0x0400433D RID: 17213
		private bool m_subscribed;

		// Token: 0x0400433E RID: 17214
		private bool m_preventSliderTrigger;
	}
}
