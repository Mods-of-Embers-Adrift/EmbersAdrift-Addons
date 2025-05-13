using System;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D65 RID: 3429
	internal struct ClipSpeedData
	{
		// Token: 0x06006774 RID: 26484 RVA: 0x0008587E File Offset: 0x00083A7E
		public ClipSpeedData(AnimationClip clip, Vector2 threshold, float playbackSpeed)
		{
			this.Speed = 0f;
			this.Clip = clip;
			this.Threshold = threshold;
			this.PlaybackSpeed = playbackSpeed;
		}

		// Token: 0x040059D2 RID: 22994
		public float Speed;

		// Token: 0x040059D3 RID: 22995
		public AnimationClip Clip;

		// Token: 0x040059D4 RID: 22996
		public Vector2 Threshold;

		// Token: 0x040059D5 RID: 22997
		public float PlaybackSpeed;
	}
}
