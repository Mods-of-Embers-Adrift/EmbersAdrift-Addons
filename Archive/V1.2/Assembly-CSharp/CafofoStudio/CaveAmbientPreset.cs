using System;
using UnityEngine;

namespace CafofoStudio
{
	// Token: 0x0200004D RID: 77
	[CreateAssetMenu(fileName = "MyCaveAmbientPreset", menuName = "CafofoStudio/Create Custom Preset Asset/Cave", order = 1)]
	public class CaveAmbientPreset : AmbientPreset
	{
		// Token: 0x0400036C RID: 876
		[Range(0f, 1f)]
		public float atmosphere1Intensity;

		// Token: 0x0400036D RID: 877
		[Range(0f, 1f)]
		public float atmosphere1VolumeMultiplier = 1f;

		// Token: 0x0400036E RID: 878
		[Range(0f, 1f)]
		public float atmosphere2Intensity;

		// Token: 0x0400036F RID: 879
		[Range(0f, 1f)]
		public float atmosphere2VolumeMultiplier = 1f;

		// Token: 0x04000370 RID: 880
		[Range(0f, 1f)]
		public float atmosphere3Intensity;

		// Token: 0x04000371 RID: 881
		[Range(0f, 1f)]
		public float atmosphere3VolumeMultiplier = 1f;

		// Token: 0x04000372 RID: 882
		[Range(0f, 1f)]
		public float sedimentIntensity;

		// Token: 0x04000373 RID: 883
		[Range(0f, 1f)]
		public float sedimentVolumeMultiplier = 1f;

		// Token: 0x04000374 RID: 884
		[Range(0f, 1f)]
		public float waterDropsIntensity;

		// Token: 0x04000375 RID: 885
		[Range(0f, 1f)]
		public float waterDropsVolumeMultiplier = 1f;

		// Token: 0x04000376 RID: 886
		[Range(0f, 1f)]
		public float waterStreamIntensity;

		// Token: 0x04000377 RID: 887
		[Range(0f, 1f)]
		public float waterStreamVolumeMultiplier = 1f;

		// Token: 0x04000378 RID: 888
		[Range(0f, 1f)]
		public float sewerIntensity;

		// Token: 0x04000379 RID: 889
		[Range(0f, 1f)]
		public float sewerVolumeMultiplier = 1f;

		// Token: 0x0400037A RID: 890
		[Range(0f, 1f)]
		public float fireIntensity;

		// Token: 0x0400037B RID: 891
		[Range(0f, 1f)]
		public float fireVolumeMultiplier = 1f;

		// Token: 0x0400037C RID: 892
		[Range(0f, 1f)]
		public float crittersIntensity;

		// Token: 0x0400037D RID: 893
		[Range(0f, 1f)]
		public float crittersVolumeMultiplier = 1f;
	}
}
