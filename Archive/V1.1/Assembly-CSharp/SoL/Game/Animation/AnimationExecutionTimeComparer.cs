using System;
using System.Collections.Generic;

namespace SoL.Game.Animation
{
	// Token: 0x02000D5C RID: 3420
	public struct AnimationExecutionTimeComparer : IEqualityComparer<AnimationExecutionTime>
	{
		// Token: 0x060066E8 RID: 26344 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(AnimationExecutionTime x, AnimationExecutionTime y)
		{
			return x == y;
		}

		// Token: 0x060066E9 RID: 26345 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(AnimationExecutionTime obj)
		{
			return (int)obj;
		}
	}
}
