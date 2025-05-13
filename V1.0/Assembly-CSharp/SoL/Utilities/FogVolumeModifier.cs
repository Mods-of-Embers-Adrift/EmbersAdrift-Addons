using System;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Utilities
{
	// Token: 0x020002FD RID: 765
	public class FogVolumeModifier : VolumeComponentModifier<Fog>
	{
		// Token: 0x06001594 RID: 5524 RVA: 0x0005133B File Offset: 0x0004F53B
		private void Start()
		{
			this.VolumetricsOnChanged();
			Options.VideoOptions.Volumetrics.Changed += this.VolumetricsOnChanged;
		}

		// Token: 0x06001595 RID: 5525 RVA: 0x00051359 File Offset: 0x0004F559
		private void OnDestroy()
		{
			Options.VideoOptions.Volumetrics.Changed -= this.VolumetricsOnChanged;
		}

		// Token: 0x06001596 RID: 5526 RVA: 0x00051371 File Offset: 0x0004F571
		private void VolumetricsOnChanged()
		{
			base.Component.enableVolumetricFog.value = Options.VideoOptions.Volumetrics.Value;
		}
	}
}
