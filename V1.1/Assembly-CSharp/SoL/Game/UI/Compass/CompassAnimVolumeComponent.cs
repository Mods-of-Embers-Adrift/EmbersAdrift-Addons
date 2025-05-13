using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Game.UI.Compass
{
	// Token: 0x0200099F RID: 2463
	[VolumeComponentMenuForRenderPipeline("Compass Anim", new Type[]
	{
		typeof(HDRenderPipeline)
	})]
	[Serializable]
	public class CompassAnimVolumeComponent : VolumeComponent
	{
		// Token: 0x040044B2 RID: 17586
		public FloatParameter NoiseSpeed = new FloatParameter(1f, false);

		// Token: 0x040044B3 RID: 17587
		public FloatParameter MaxOffset = new FloatParameter(30f, false);

		// Token: 0x040044B4 RID: 17588
		public FloatParameter SmoothTime = new FloatParameter(0.3f, false);
	}
}
