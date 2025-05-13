using System;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI
{
	// Token: 0x020008BC RID: 2236
	[Serializable]
	public class FloatSlider : SliderBase
	{
		// Token: 0x06004188 RID: 16776 RVA: 0x0018FA5C File Offset: 0x0018DC5C
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
			if (!this.InitInternal())
			{
				return;
			}
			this.RefreshLabelText();
			this.m_obj.Slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderChanged));
			this.m_obj.ResetButton.onClick.AddListener(new UnityAction(this.ResetButtonClicked));
			this.m_initialized = true;
		}

		// Token: 0x06004189 RID: 16777 RVA: 0x0018FB38 File Offset: 0x0018DD38
		protected override bool InitInternal()
		{
			if (!this.m_obj || !this.m_obj.Slider)
			{
				return false;
			}
			if (this.m_wholeNumbers)
			{
				this.m_obj.Slider.minValue = (float)Mathf.FloorToInt(this.m_minValue);
				this.m_obj.Slider.maxValue = (float)Mathf.FloorToInt(this.m_maxValue);
				this.m_obj.Slider.wholeNumbers = this.m_wholeNumbers;
				this.m_obj.Slider.value = (float)Mathf.FloorToInt(this.m_option.Value);
			}
			else
			{
				this.m_obj.Slider.minValue = this.m_minValue;
				this.m_obj.Slider.maxValue = this.m_maxValue;
				this.m_obj.Slider.wholeNumbers = this.m_wholeNumbers;
				this.m_obj.Slider.value = this.m_option.Value;
			}
			return true;
		}

		// Token: 0x0600418A RID: 16778 RVA: 0x0006C400 File Offset: 0x0006A600
		protected virtual void OnSliderChanged(float value)
		{
			this.m_option.Value = value;
			this.RefreshLabelText();
		}

		// Token: 0x0600418B RID: 16779 RVA: 0x0018FC40 File Offset: 0x0018DE40
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

		// Token: 0x0600418C RID: 16780 RVA: 0x0006C414 File Offset: 0x0006A614
		private void ResetButtonClicked()
		{
			this.m_obj.Slider.value = this.m_option.DefaultValue;
		}

		// Token: 0x0600418D RID: 16781 RVA: 0x0006C431 File Offset: 0x0006A631
		public void SetSliderValue(float value)
		{
			if (this.m_obj && this.m_obj.Slider)
			{
				this.m_obj.Slider.value = value;
			}
		}

		// Token: 0x04003ECD RID: 16077
		[SerializeField]
		protected bool m_wholeNumbers;

		// Token: 0x04003ECE RID: 16078
		[SerializeField]
		protected bool m_displayRawValue;

		// Token: 0x04003ECF RID: 16079
		[SerializeField]
		private string m_displayFormat = "F2";

		// Token: 0x04003ED0 RID: 16080
		protected Options.Option_Float m_option;
	}
}
