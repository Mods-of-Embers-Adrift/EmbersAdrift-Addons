using System;
using UnityEngine.Rendering;

namespace SoL.Game.UI
{
	// Token: 0x020008C0 RID: 2240
	[Serializable]
	public class ResolutionScaleSlider : FloatSlider
	{
		// Token: 0x060041A1 RID: 16801 RVA: 0x0006C538 File Offset: 0x0006A738
		protected override bool InitInternal()
		{
			bool flag = base.InitInternal();
			if (flag)
			{
				DynamicResolutionHandler.SetDynamicResScaler(new PerformDynamicRes(this.GetDynamicResolutionScale), DynamicResScalePolicyType.ReturnsMinMaxLerpFactor);
			}
			return flag;
		}

		// Token: 0x060041A2 RID: 16802 RVA: 0x0006C555 File Offset: 0x0006A755
		private float GetDynamicResolutionScale()
		{
			if (!this.m_initialized)
			{
				return 1f;
			}
			return Options.VideoOptions.ResolutionScale.Value;
		}
	}
}
