using System;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI
{
	// Token: 0x020008BD RID: 2237
	[Serializable]
	public class IntSlider : SliderBase
	{
		// Token: 0x0600418F RID: 16783 RVA: 0x0018FD24 File Offset: 0x0018DF24
		public void Init(Options.Option_Int opt)
		{
			if (!this.m_obj || !this.m_obj.Slider)
			{
				return;
			}
			this.m_option = opt;
			int num = Mathf.FloorToInt(this.m_minValue);
			int max = Mathf.FloorToInt(this.m_maxValue);
			if (this.m_option.Value < num || (float)this.m_option.Value > this.m_maxValue)
			{
				this.m_option.Value = Mathf.Clamp(this.m_option.Value, num, max);
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

		// Token: 0x06004190 RID: 16784 RVA: 0x0018FE0C File Offset: 0x0018E00C
		protected override bool InitInternal()
		{
			if (!this.m_obj || !this.m_obj.Slider)
			{
				return false;
			}
			this.m_obj.Slider.minValue = (float)Mathf.FloorToInt(this.m_minValue);
			this.m_obj.Slider.maxValue = (float)Mathf.FloorToInt(this.m_maxValue);
			this.m_obj.Slider.wholeNumbers = true;
			this.m_obj.Slider.value = (float)this.m_option.Value;
			return true;
		}

		// Token: 0x06004191 RID: 16785 RVA: 0x0006C476 File Offset: 0x0006A676
		protected virtual void OnSliderChanged(float value)
		{
			this.m_option.Value = Mathf.FloorToInt(value);
			this.RefreshLabelText();
		}

		// Token: 0x06004192 RID: 16786 RVA: 0x0018FEA0 File Offset: 0x0018E0A0
		private void RefreshLabelText()
		{
			if (this.m_obj && this.m_obj.Label)
			{
				this.m_obj.Value.SetText(this.m_option.Value.ToString() + this.m_labelSuffix);
			}
		}

		// Token: 0x06004193 RID: 16787 RVA: 0x0006C48F File Offset: 0x0006A68F
		protected virtual void ResetButtonClicked()
		{
			this.m_obj.Slider.value = (float)this.m_option.DefaultValue;
		}

		// Token: 0x04003ED1 RID: 16081
		protected Options.Option_Int m_option;
	}
}
