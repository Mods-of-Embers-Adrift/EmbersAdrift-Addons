using System;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D68 RID: 3432
	[Serializable]
	internal class AnimationClipWithThreshold
	{
		// Token: 0x040059E2 RID: 23010
		public AnimationClip Clip;

		// Token: 0x040059E3 RID: 23011
		public bool Exclude;

		// Token: 0x040059E4 RID: 23012
		public bool ReversePlayback;

		// Token: 0x040059E5 RID: 23013
		public bool OverrideThreshold;

		// Token: 0x040059E6 RID: 23014
		public Vector2 Threshold = Vector2.zero;
	}
}
