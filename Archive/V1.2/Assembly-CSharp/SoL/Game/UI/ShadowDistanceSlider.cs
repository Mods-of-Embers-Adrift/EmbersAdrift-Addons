using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Game.UI
{
	// Token: 0x020008C2 RID: 2242
	[Serializable]
	public class ShadowDistanceSlider : FloatSlider
	{
		// Token: 0x060041A7 RID: 16807 RVA: 0x00190098 File Offset: 0x0018E298
		protected override bool InitInternal()
		{
			if (!this.m_obj || !this.m_obj.Slider)
			{
				return false;
			}
			this.m_obj.Slider.value = this.m_option.Value;
			this.OnSliderChanged(this.m_option.Value);
			return true;
		}

		// Token: 0x060041A8 RID: 16808 RVA: 0x0006C5AF File Offset: 0x0006A7AF
		protected override void OnSliderChanged(float value)
		{
			base.OnSliderChanged(value);
		}

		// Token: 0x060041A9 RID: 16809 RVA: 0x0006C5B8 File Offset: 0x0006A7B8
		protected override void OnDestroyInternal()
		{
			if (this.m_shadowSettings != null && this.m_originalDistance != null)
			{
				this.m_shadowSettings.maxShadowDistance.value = this.m_originalDistance.Value;
			}
		}

		// Token: 0x060041AA RID: 16810 RVA: 0x0004475B File Offset: 0x0004295B
		public void TriggerAdjustShadowsForRange()
		{
		}

		// Token: 0x060041AB RID: 16811 RVA: 0x001900F4 File Offset: 0x0018E2F4
		public void AdjustShadowsForRange(float value)
		{
			float num = 0f;
			float num2 = 10f;
			float num3 = 20f;
			float num4 = 40f;
			switch (QualitySettings.GetQualityLevel())
			{
			case 0:
				num = 0f;
				break;
			case 1:
				num = 15f;
				break;
			case 2:
				num = 70f;
				num2 = 10f;
				break;
			case 3:
				num = 120f;
				num2 = 20f;
				break;
			case 4:
				num = 300f;
				num2 = 15f;
				num3 = 30f;
				num4 = 100f;
				break;
			case 5:
				num = 500f;
				num2 = 20f;
				num3 = 50f;
				num4 = 150f;
				break;
			}
			num *= value;
			QualitySettings.shadowDistance = num;
			QualitySettings.shadowCascade2Split = Mathf.Clamp01(num2 / num);
			QualitySettings.shadowCascade4Split = new Vector3(Mathf.Clamp01(num2 / num), Mathf.Clamp01(num3 / num), Mathf.Clamp01(num4 / num));
			if (this.ShadowSettings != null)
			{
				float value2 = Mathf.Clamp(value * 300f, 0f, 300f);
				this.ShadowSettings.maxShadowDistance.value = value2;
			}
		}

		// Token: 0x17000EFA RID: 3834
		// (get) Token: 0x060041AC RID: 16812 RVA: 0x0006C5F0 File Offset: 0x0006A7F0
		private HDShadowSettings ShadowSettings
		{
			get
			{
				return this.m_shadowSettings;
			}
		}

		// Token: 0x04003ED8 RID: 16088
		private float? m_originalDistance;

		// Token: 0x04003ED9 RID: 16089
		private HDShadowSettings m_shadowSettings;
	}
}
