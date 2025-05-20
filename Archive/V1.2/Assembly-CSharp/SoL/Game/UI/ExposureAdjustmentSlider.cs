using System;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI
{
	// Token: 0x020008BB RID: 2235
	[Serializable]
	public class ExposureAdjustmentSlider : SliderBase
	{
		// Token: 0x06004180 RID: 16768 RVA: 0x0018F8A4 File Offset: 0x0018DAA4
		public void Init(Options.Option_Float opt)
		{
			if (!this.m_obj || !this.m_obj.Slider)
			{
				return;
			}
			this.m_option = opt;
			if (this.m_option.Value < this.m_minValue || this.m_option.Value > this.m_maxValue)
			{
				this.m_option.Value = Mathf.Clamp(this.m_option.Value, this.m_minValue, this.m_maxValue);
			}
			this.m_obj.Slider.minValue = this.m_minValue;
			this.m_obj.Slider.maxValue = this.m_maxValue;
			this.m_obj.Slider.wholeNumbers = false;
			this.m_obj.Slider.value = this.GetSliderValue(this.m_option.Value);
			this.m_obj.Slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderChanged));
			this.m_obj.ResetButton.onClick.AddListener(new UnityAction(this.ResetButtonClicked));
			this.RefreshLabelText();
			this.m_initialized = true;
		}

		// Token: 0x06004181 RID: 16769 RVA: 0x0006C378 File Offset: 0x0006A578
		private float GetSliderValue(float optionValue)
		{
			return optionValue.Remap(-1f, 1f, 1f, -1f);
		}

		// Token: 0x06004182 RID: 16770 RVA: 0x0006C394 File Offset: 0x0006A594
		private float GetOptionValue(float sliderValue)
		{
			return sliderValue.Remap(1f, -1f, -1f, 1f);
		}

		// Token: 0x06004183 RID: 16771 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool InitInternal()
		{
			return true;
		}

		// Token: 0x06004184 RID: 16772 RVA: 0x0006C3B0 File Offset: 0x0006A5B0
		private void ResetButtonClicked()
		{
			this.m_obj.Slider.value = this.GetSliderValue(this.m_option.DefaultValue);
		}

		// Token: 0x06004185 RID: 16773 RVA: 0x0006C3D3 File Offset: 0x0006A5D3
		private void OnSliderChanged(float value)
		{
			this.m_option.Value = this.GetOptionValue(value);
			this.RefreshLabelText();
		}

		// Token: 0x06004186 RID: 16774 RVA: 0x0018F9D4 File Offset: 0x0018DBD4
		private void RefreshLabelText()
		{
			if (this.m_obj && this.m_obj.Label)
			{
				float num = this.m_obj.Slider.value - this.m_obj.Slider.minValue;
				float num2 = this.m_obj.Slider.maxValue - this.m_obj.Slider.minValue;
				float t = num / num2;
				int num3 = Mathf.CeilToInt(Mathf.Lerp(this.m_minPercentage, this.m_maxPercentage, t) * 100f);
				string str = this.m_displayRawValue ? this.m_obj.Slider.value.ToString(this.m_displayFormat) : num3.ToString();
				this.m_obj.Value.SetText(str + this.m_labelSuffix);
			}
		}

		// Token: 0x04003ECA RID: 16074
		[SerializeField]
		protected bool m_displayRawValue;

		// Token: 0x04003ECB RID: 16075
		[SerializeField]
		private string m_displayFormat = "F2";

		// Token: 0x04003ECC RID: 16076
		private Options.Option_Float m_option;
	}
}
