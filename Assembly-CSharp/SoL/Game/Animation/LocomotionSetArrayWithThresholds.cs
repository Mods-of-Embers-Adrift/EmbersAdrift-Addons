using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D6A RID: 3434
	[Serializable]
	internal class LocomotionSetArrayWithThresholds
	{
		// Token: 0x0600677A RID: 26490 RVA: 0x00213068 File Offset: 0x00211268
		internal List<AnimationClip> GetAllClips()
		{
			List<AnimationClip> list = new List<AnimationClip>();
			this.AddClipsToArray(list, this.Forward);
			this.AddClipsToArray(list, this.Left);
			this.AddClipsToArray(list, this.Right);
			this.AddClipsToArray(list, this.Left45);
			this.AddClipsToArray(list, this.Right45);
			this.AddClipsToArray(list, this.Left135);
			this.AddClipsToArray(list, this.Right135);
			this.AddClipsToArray(list, this.Backward);
			return list;
		}

		// Token: 0x0600677B RID: 26491 RVA: 0x002130E4 File Offset: 0x002112E4
		private void AddClipsToArray(List<AnimationClip> clipList, AnimationClipWithThreshold[] clipThreshold)
		{
			if (clipThreshold == null || clipThreshold.Length == 0)
			{
				return;
			}
			foreach (AnimationClipWithThreshold animationClipWithThreshold in clipThreshold)
			{
				if (animationClipWithThreshold.Clip)
				{
					clipList.Add(animationClipWithThreshold.Clip);
				}
			}
		}

		// Token: 0x040059EF RID: 23023
		public AnimationClipWithThreshold[] Forward;

		// Token: 0x040059F0 RID: 23024
		public AnimationClipWithThreshold[] Left;

		// Token: 0x040059F1 RID: 23025
		public AnimationClipWithThreshold[] Right;

		// Token: 0x040059F2 RID: 23026
		public AnimationClipWithThreshold[] Left45;

		// Token: 0x040059F3 RID: 23027
		public AnimationClipWithThreshold[] Right45;

		// Token: 0x040059F4 RID: 23028
		public AnimationClipWithThreshold[] Left135;

		// Token: 0x040059F5 RID: 23029
		public AnimationClipWithThreshold[] Right135;

		// Token: 0x040059F6 RID: 23030
		public AnimationClipWithThreshold[] Backward;
	}
}
