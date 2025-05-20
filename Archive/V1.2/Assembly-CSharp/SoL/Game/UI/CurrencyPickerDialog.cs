using System;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x02000872 RID: 2162
	public class CurrencyPickerDialog : BaseDialog<SelectCurrencyOptions>
	{
		// Token: 0x17000E88 RID: 3720
		// (get) Token: 0x06003ECC RID: 16076 RVA: 0x0006A7F0 File Offset: 0x000689F0
		protected override object Result
		{
			get
			{
				return this.m_converter.TotalCurrency;
			}
		}

		// Token: 0x06003ECD RID: 16077 RVA: 0x0018605C File Offset: 0x0018425C
		protected override void Start()
		{
			base.Start();
			this.m_slider.onValueChanged.AddListener(new UnityAction<float>(this.SliderValueChanged));
			this.m_slider.wholeNumbers = true;
			this.m_copperInput.characterValidation = TMP_InputField.CharacterValidation.Integer;
			this.m_silverInput.characterValidation = TMP_InputField.CharacterValidation.Integer;
			this.m_goldInput.characterValidation = TMP_InputField.CharacterValidation.Integer;
			this.m_copperInput.onDeselect.AddListener(new UnityAction<string>(this.CopperDeselected));
			this.m_silverInput.onDeselect.AddListener(new UnityAction<string>(this.SilverDeselected));
			this.m_goldInput.onDeselect.AddListener(new UnityAction<string>(this.GoldDeselected));
		}

		// Token: 0x06003ECE RID: 16078 RVA: 0x00186110 File Offset: 0x00184310
		private void Update()
		{
			if (base.Visible && ClientGameManager.InputManager != null && EventSystem.current && EventSystem.current.currentSelectedGameObject)
			{
				GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
				bool holdingShift = ClientGameManager.InputManager.HoldingShift;
				if (ClientGameManager.InputManager.TabDown)
				{
					if (currentSelectedGameObject == this.m_goldInput.gameObject)
					{
						if (!holdingShift)
						{
							this.m_silverInput.Select();
							return;
						}
					}
					else
					{
						if (currentSelectedGameObject == this.m_silverInput.gameObject)
						{
							(holdingShift ? this.m_goldInput : this.m_copperInput).Select();
							return;
						}
						if (currentSelectedGameObject == this.m_copperInput.gameObject && holdingShift)
						{
							this.m_silverInput.Select();
							return;
						}
					}
				}
				else if (ClientGameManager.InputManager.EnterDown && (currentSelectedGameObject == this.m_goldInput.gameObject || currentSelectedGameObject == this.m_silverInput.gameObject || currentSelectedGameObject == this.m_copperInput.gameObject))
				{
					EventSystem.current.SetSelectedGameObject(null);
				}
			}
		}

		// Token: 0x06003ECF RID: 16079 RVA: 0x00186238 File Offset: 0x00184438
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.m_slider.onValueChanged.RemoveListener(new UnityAction<float>(this.SliderValueChanged));
			this.m_copperInput.onDeselect.RemoveListener(new UnityAction<string>(this.CopperDeselected));
			this.m_silverInput.onDeselect.RemoveListener(new UnityAction<string>(this.SilverDeselected));
			this.m_goldInput.onDeselect.RemoveListener(new UnityAction<string>(this.GoldDeselected));
		}

		// Token: 0x06003ED0 RID: 16080 RVA: 0x0006A802 File Offset: 0x00068A02
		protected override void Confirm()
		{
			this.ValidateAllInputFieldsAndRefresh();
			base.Confirm();
		}

		// Token: 0x06003ED1 RID: 16081 RVA: 0x0006A810 File Offset: 0x00068A10
		public override void Hide(bool skipTransition = false)
		{
			base.Hide(skipTransition);
			this.Cleanup();
		}

		// Token: 0x06003ED2 RID: 16082 RVA: 0x001862BC File Offset: 0x001844BC
		protected override void InitInternal()
		{
			if (base.RectTransform)
			{
				base.RectTransform.position = ((this.m_currentOptions.PosOverride != null) ? this.m_currentOptions.PosOverride.Value : Input.mousePosition);
				base.RectTransform.ClampToScreen();
				this.m_parentWindow = this.m_currentOptions.ParentWindow;
				if (this.m_parentWindow)
				{
					this.m_previousParent = base.RectTransform.parent;
					if (this.m_parentWindow.RectTransform)
					{
						base.RectTransform.SetParent(this.m_parentWindow.RectTransform);
					}
					this.m_parentWindow.HideCalled += this.ParentWindowOnHideCalled;
				}
			}
			this.m_minimumCurrency = new CurrencyConverter(this.m_currentOptions.MinimumCurrency);
			this.m_maximumCurrency = new CurrencyConverter(this.m_currentOptions.AllowableCurrency);
			this.m_converter = new CurrencyConverter(this.m_currentOptions.InitialCurrency);
			this.m_manuallySetting = true;
			this.m_slider.minValue = this.m_currentOptions.MinimumCurrency;
			this.m_slider.maxValue = this.m_currentOptions.AllowableCurrency;
			bool flag = this.m_currentOptions.HideSlider || this.m_currentOptions.AllowableCurrency >= 9999999UL;
			this.m_slider.gameObject.transform.parent.gameObject.SetActive(!flag);
			this.m_manuallySetting = false;
			this.RefreshInputsForValue(true);
		}

		// Token: 0x06003ED3 RID: 16083 RVA: 0x0006A81F File Offset: 0x00068A1F
		private void ParentWindowOnHideCalled()
		{
			this.Cleanup();
			base.Cancel();
		}

		// Token: 0x06003ED4 RID: 16084 RVA: 0x00186458 File Offset: 0x00184658
		private void Cleanup()
		{
			if (this.m_parentWindow)
			{
				this.m_parentWindow.HideCalled -= this.ParentWindowOnHideCalled;
				this.m_parentWindow = null;
			}
			if (this.m_previousParent && base.RectTransform)
			{
				base.RectTransform.SetParent(this.m_previousParent);
			}
			this.m_previousParent = null;
		}

		// Token: 0x06003ED5 RID: 16085 RVA: 0x0006A82D File Offset: 0x00068A2D
		private void SliderValueChanged(float value)
		{
			if (!this.m_manuallySetting)
			{
				this.m_converter = new CurrencyConverter((ulong)value);
				this.RefreshInputsForValue(true);
			}
		}

		// Token: 0x06003ED6 RID: 16086 RVA: 0x0006A84B File Offset: 0x00068A4B
		private void CopperDeselected(string arg0)
		{
			this.ValidateAllInputFieldsAndRefresh();
		}

		// Token: 0x06003ED7 RID: 16087 RVA: 0x0006A84B File Offset: 0x00068A4B
		private void SilverDeselected(string arg0)
		{
			this.ValidateAllInputFieldsAndRefresh();
		}

		// Token: 0x06003ED8 RID: 16088 RVA: 0x0006A84B File Offset: 0x00068A4B
		private void GoldDeselected(string arg0)
		{
			this.ValidateAllInputFieldsAndRefresh();
		}

		// Token: 0x06003ED9 RID: 16089 RVA: 0x001864C4 File Offset: 0x001846C4
		private ulong GetInputFieldValue(TMP_InputField inputField)
		{
			ulong result;
			if (!inputField || string.IsNullOrEmpty(inputField.text) || !ulong.TryParse(inputField.text, out result))
			{
				return 0UL;
			}
			return result;
		}

		// Token: 0x06003EDA RID: 16090 RVA: 0x001864FC File Offset: 0x001846FC
		private void ValidateInputFieldValues()
		{
			ulong num = this.GetInputFieldValue(this.m_copperInput);
			ulong num2 = this.GetInputFieldValue(this.m_silverInput) * 100UL;
			ulong num3 = this.GetInputFieldValue(this.m_goldInput) * 10000UL;
			ulong num4 = num + num2 + num3;
			if (num4 < this.m_minimumCurrency.TotalCurrency)
			{
				ulong num5 = this.m_minimumCurrency.TotalCurrency - num4;
				ulong num6 = ((num5 >= 10000UL) ? (num5 / 10000UL) : 0UL) * 10000UL;
				num5 -= num6;
				ulong num7 = ((num5 >= 100UL) ? (num5 / 100UL) : 0UL) * 100UL;
				num5 -= num7;
				ulong num8 = num5;
				num += num8;
				num2 += num7;
				num3 += num6;
			}
			else if (num4 > this.m_maximumCurrency.TotalCurrency)
			{
				ulong num9 = num4 - this.m_maximumCurrency.TotalCurrency;
				if (num3 >= num9)
				{
					num3 -= num9;
					num9 = 0UL;
				}
				else
				{
					num9 -= num3;
					num3 = 0UL;
				}
				if (num2 >= num9)
				{
					num2 -= num9;
					num9 = 0UL;
				}
				else
				{
					num9 -= num2;
					num2 = 0UL;
				}
				if (num >= num9)
				{
					num -= num9;
				}
				else
				{
					num9 -= num;
					num = 0UL;
				}
			}
			num4 = num + num2 + num3;
			this.m_converter = new CurrencyConverter(num4);
		}

		// Token: 0x06003EDB RID: 16091 RVA: 0x0006A853 File Offset: 0x00068A53
		private void ValidateAllInputFieldsAndRefresh()
		{
			this.ValidateInputFieldValues();
			this.RefreshInputsForValue(false);
		}

		// Token: 0x06003EDC RID: 16092 RVA: 0x00186634 File Offset: 0x00184834
		private void RefreshInputsForValue(bool forceConvertValueToText = false)
		{
			this.m_manuallySetting = true;
			this.m_slider.value = this.m_converter.TotalCurrency;
			if (forceConvertValueToText || !string.IsNullOrEmpty(this.m_copperInput.text))
			{
				this.m_copperInput.text = this.m_converter.Copper.ToString();
			}
			if (forceConvertValueToText || !string.IsNullOrEmpty(this.m_silverInput.text))
			{
				this.m_silverInput.text = this.m_converter.Silver.ToString();
			}
			if (forceConvertValueToText || !string.IsNullOrEmpty(this.m_goldInput.text))
			{
				this.m_goldInput.text = this.m_converter.Gold.ToString();
			}
			this.m_manuallySetting = false;
		}

		// Token: 0x04003CCE RID: 15566
		[SerializeField]
		private TMP_InputField m_copperInput;

		// Token: 0x04003CCF RID: 15567
		[SerializeField]
		private TMP_InputField m_silverInput;

		// Token: 0x04003CD0 RID: 15568
		[SerializeField]
		private TMP_InputField m_goldInput;

		// Token: 0x04003CD1 RID: 15569
		[SerializeField]
		private Slider m_slider;

		// Token: 0x04003CD2 RID: 15570
		private bool m_manuallySetting;

		// Token: 0x04003CD3 RID: 15571
		private CurrencyConverter m_converter;

		// Token: 0x04003CD4 RID: 15572
		private CurrencyConverter m_minimumCurrency;

		// Token: 0x04003CD5 RID: 15573
		private CurrencyConverter m_maximumCurrency;

		// Token: 0x04003CD6 RID: 15574
		private UIWindow m_parentWindow;

		// Token: 0x04003CD7 RID: 15575
		private Transform m_previousParent;

		// Token: 0x02000873 RID: 2163
		private enum CurrencyType
		{
			// Token: 0x04003CD9 RID: 15577
			Copper,
			// Token: 0x04003CDA RID: 15578
			Silver,
			// Token: 0x04003CDB RID: 15579
			Gold
		}
	}
}
