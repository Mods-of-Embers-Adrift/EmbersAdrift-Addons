using System;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Utilities
{
	// Token: 0x020002FB RID: 763
	public class AutoExposureController : VolumeComponentModifier<Exposure>
	{
		// Token: 0x0600158B RID: 5515 RVA: 0x00051290 File Offset: 0x0004F490
		private void Start()
		{
			this.ExposureAdjustmentOnChanged();
			Options.VideoOptions.ExposureAdjustment.Changed += this.ExposureAdjustmentOnChanged;
		}

		// Token: 0x0600158C RID: 5516 RVA: 0x000512AE File Offset: 0x0004F4AE
		private void OnDestroy()
		{
			Options.VideoOptions.ExposureAdjustment.Changed -= this.ExposureAdjustmentOnChanged;
		}

		// Token: 0x0600158D RID: 5517 RVA: 0x000512C6 File Offset: 0x0004F4C6
		private void ExposureAdjustmentOnChanged()
		{
			if (base.Component)
			{
				base.Component.limitMin.value = Options.VideoOptions.ExposureAdjustment.Value;
			}
		}
	}
}
