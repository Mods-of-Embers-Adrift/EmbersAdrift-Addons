using System;
using UnityEngine;

namespace CafofoStudio
{
	// Token: 0x0200004F RID: 79
	[CreateAssetMenu(fileName = "VillageAmbiencePreset", menuName = "CafofoStudio/Create Custom Preset Asset/Village", order = 1)]
	public class VillageAmbiencePreset : AmbientPreset
	{
		// Token: 0x04000388 RID: 904
		[Range(0f, 1f)]
		public float birdsIntensity;

		// Token: 0x04000389 RID: 905
		[Range(0f, 1f)]
		public float birdsVolumeMultiplier = 1f;

		// Token: 0x0400038A RID: 906
		[Range(0f, 1f)]
		public float rainIntensity;

		// Token: 0x0400038B RID: 907
		[Range(0f, 1f)]
		public float rainVolumeMultiplier = 1f;

		// Token: 0x0400038C RID: 908
		[Range(0f, 1f)]
		public float waterStreamIntensity;

		// Token: 0x0400038D RID: 909
		[Range(0f, 1f)]
		public float waterStreamVolumeMultiplier = 1f;

		// Token: 0x0400038E RID: 910
		[Range(0f, 1f)]
		public float fireIntensity;

		// Token: 0x0400038F RID: 911
		[Range(0f, 1f)]
		public float fireVolumeMultiplier = 1f;

		// Token: 0x04000390 RID: 912
		[Range(0f, 1f)]
		public float crowdIntensity;

		// Token: 0x04000391 RID: 913
		[Range(0f, 1f)]
		public float crowdVolumeMultiplier = 1f;

		// Token: 0x04000392 RID: 914
		[Range(0f, 1f)]
		public float blacksmithIntensity;

		// Token: 0x04000393 RID: 915
		[Range(0f, 1f)]
		public float blacksmithVolumeMultiplier = 1f;

		// Token: 0x04000394 RID: 916
		[Range(0f, 1f)]
		public float lumbermillIntensity;

		// Token: 0x04000395 RID: 917
		[Range(0f, 1f)]
		public float lumbermillVolumeMultiplier = 1f;

		// Token: 0x04000396 RID: 918
		[Range(0f, 1f)]
		public float humanActivityIntensity;

		// Token: 0x04000397 RID: 919
		[Range(0f, 1f)]
		public float humanActivityVolumeMultiplier = 1f;

		// Token: 0x04000398 RID: 920
		[Range(0f, 1f)]
		public float farmAnimalsIntensity;

		// Token: 0x04000399 RID: 921
		[Range(0f, 1f)]
		public float farmAnimalsVolumeMultiplier = 1f;
	}
}
