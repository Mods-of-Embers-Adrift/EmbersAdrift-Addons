using System;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x020008C3 RID: 2243
	[Serializable]
	public abstract class SliderBase
	{
		// Token: 0x17000EFB RID: 3835
		// (get) Token: 0x060041AE RID: 16814 RVA: 0x0006C5F8 File Offset: 0x0006A7F8
		public OptionsSlider Slider
		{
			get
			{
				return this.m_obj;
			}
		}

		// Token: 0x17000EFC RID: 3836
		// (get) Token: 0x060041AF RID: 16815 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool m_showSliderConfig
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060041B0 RID: 16816
		protected abstract bool InitInternal();

		// Token: 0x060041B1 RID: 16817 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void OnDestroyInternal()
		{
		}

		// Token: 0x060041B2 RID: 16818 RVA: 0x0006C600 File Offset: 0x0006A800
		public void OnDestroy()
		{
			if (this.m_initialized)
			{
				this.m_obj.Slider.onValueChanged.RemoveAllListeners();
				this.m_obj.ResetButton.onClick.RemoveAllListeners();
				this.OnDestroyInternal();
			}
		}

		// Token: 0x04003EDA RID: 16090
		[SerializeField]
		protected OptionsSlider m_obj;

		// Token: 0x04003EDB RID: 16091
		protected const string kSliderConfigGroup = "Slider Config";

		// Token: 0x04003EDC RID: 16092
		[SerializeField]
		protected float m_minValue;

		// Token: 0x04003EDD RID: 16093
		[SerializeField]
		protected float m_maxValue = 1f;

		// Token: 0x04003EDE RID: 16094
		[SerializeField]
		protected float m_minPercentage;

		// Token: 0x04003EDF RID: 16095
		[SerializeField]
		protected float m_maxPercentage = 1f;

		// Token: 0x04003EE0 RID: 16096
		[SerializeField]
		protected string m_labelSuffix = "%";

		// Token: 0x04003EE1 RID: 16097
		protected bool m_initialized;
	}
}
