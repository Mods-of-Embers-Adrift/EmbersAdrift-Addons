using System;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Utilities
{
	// Token: 0x020002FC RID: 764
	public class FogQualityModifier : VolumeComponentModifier<Fog>
	{
		// Token: 0x0600158F RID: 5519 RVA: 0x000FCBE8 File Offset: 0x000FADE8
		private void Start()
		{
			base.Component.denoisingMode.value = FogDenoisingMode.Gaussian;
			base.Component.denoisingMode.overrideState = true;
			base.Component.quality.overrideState = true;
			this.FogQualityChanged();
			Options.VideoOptions.VolumetricQuality.Changed += this.FogQualityChanged;
		}

		// Token: 0x06001590 RID: 5520 RVA: 0x000512F7 File Offset: 0x0004F4F7
		private void OnDestroy()
		{
			Options.VideoOptions.VolumetricQuality.Changed -= this.FogQualityChanged;
		}

		// Token: 0x06001591 RID: 5521 RVA: 0x0005130F File Offset: 0x0004F50F
		private void FogQualityChanged()
		{
			base.Component.quality.value = 3;
			base.Component.volumetricFogBudget = this.GetBudgetForQuality();
		}

		// Token: 0x06001592 RID: 5522 RVA: 0x000FCC44 File Offset: 0x000FAE44
		private float GetBudgetForQuality()
		{
			switch (Options.VideoOptions.VolumetricQuality.Value)
			{
			case 0:
				return 0.14f;
			case 1:
				return 0.3f;
			case 2:
				return 0.6f;
			default:
				return 0.3f;
			}
		}
	}
}
