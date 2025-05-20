using System;
using System.Collections.Generic;

namespace SoL.Game
{
	// Token: 0x02000597 RID: 1431
	public struct TierComparer : IEqualityComparer<LevelTiers>
	{
		// Token: 0x06002C9C RID: 11420 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(LevelTiers x, LevelTiers y)
		{
			return x == y;
		}

		// Token: 0x06002C9D RID: 11421 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(LevelTiers obj)
		{
			return (int)obj;
		}
	}
}
