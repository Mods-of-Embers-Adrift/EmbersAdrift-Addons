using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D69 RID: 3433
	[Serializable]
	internal class LocomotionSetArray
	{
		// Token: 0x06006778 RID: 26488 RVA: 0x00213054 File Offset: 0x00211254
		internal List<AnimationClip> GetAllClips()
		{
			List<AnimationClip> list = new List<AnimationClip>();
			list.AddRange(this.Forward);
			list.AddRange(this.Left);
			list.AddRange(this.Right);
			list.AddRange(this.Left45);
			list.AddRange(this.Right45);
			list.AddRange(this.Left135);
			list.AddRange(this.Right135);
			list.AddRange(this.Backward);
			return list;
		}

		// Token: 0x040059E7 RID: 23015
		public AnimationClip[] Forward;

		// Token: 0x040059E8 RID: 23016
		public AnimationClip[] Left;

		// Token: 0x040059E9 RID: 23017
		public AnimationClip[] Right;

		// Token: 0x040059EA RID: 23018
		public AnimationClip[] Left45;

		// Token: 0x040059EB RID: 23019
		public AnimationClip[] Right45;

		// Token: 0x040059EC RID: 23020
		public AnimationClip[] Left135;

		// Token: 0x040059ED RID: 23021
		public AnimationClip[] Right135;

		// Token: 0x040059EE RID: 23022
		public AnimationClip[] Backward;
	}
}
