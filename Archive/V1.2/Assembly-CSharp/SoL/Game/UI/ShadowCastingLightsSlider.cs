using System;
using SoL.Game.Culling;

namespace SoL.Game.UI
{
	// Token: 0x020008C1 RID: 2241
	[Serializable]
	public class ShadowCastingLightsSlider : IntSlider
	{
		// Token: 0x060041A4 RID: 16804 RVA: 0x0006C577 File Offset: 0x0006A777
		protected override bool InitInternal()
		{
			if (base.InitInternal())
			{
				this.OnSliderChanged((float)this.m_option.Value);
				return true;
			}
			return false;
		}

		// Token: 0x060041A5 RID: 16805 RVA: 0x0006C596 File Offset: 0x0006A796
		protected override void OnSliderChanged(float value)
		{
			base.OnSliderChanged(value);
			CullingManager.SetMaxShadowCastingLights(this.m_option.Value);
		}
	}
}
