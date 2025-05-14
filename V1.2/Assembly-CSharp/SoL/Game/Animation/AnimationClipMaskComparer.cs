using System;
using System.Collections.Generic;

namespace SoL.Game.Animation
{
	// Token: 0x02000D6E RID: 3438
	public struct AnimationClipMaskComparer : IEqualityComparer<AnimationClipMask>
	{
		// Token: 0x06006787 RID: 26503 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(AnimationClipMask x, AnimationClipMask y)
		{
			return x == y;
		}

		// Token: 0x06006788 RID: 26504 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(AnimationClipMask obj)
		{
			return (int)obj;
		}
	}
}
