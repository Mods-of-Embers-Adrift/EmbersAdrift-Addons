using System;
using System.Collections.Generic;

namespace SoL.Game.Spawning
{
	// Token: 0x02000679 RID: 1657
	public struct DifficultyRatingComparer : IEqualityComparer<DifficultyRating>
	{
		// Token: 0x06003348 RID: 13128 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(DifficultyRating x, DifficultyRating y)
		{
			return x == y;
		}

		// Token: 0x06003349 RID: 13129 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(DifficultyRating obj)
		{
			return (int)obj;
		}
	}
}
