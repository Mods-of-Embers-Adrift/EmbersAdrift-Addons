using System;
using System.Collections.Generic;

namespace SoL.Game
{
	// Token: 0x020005D5 RID: 1493
	public struct StanceComparer : IEqualityComparer<Stance>
	{
		// Token: 0x06002F6B RID: 12139 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(Stance x, Stance y)
		{
			return x == y;
		}

		// Token: 0x06002F6C RID: 12140 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(Stance obj)
		{
			return (int)obj;
		}
	}
}
