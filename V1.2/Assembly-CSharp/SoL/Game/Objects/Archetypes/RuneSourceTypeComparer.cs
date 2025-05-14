using System;
using System.Collections.Generic;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AA2 RID: 2722
	public struct RuneSourceTypeComparer : IEqualityComparer<RuneSourceType>
	{
		// Token: 0x0600542D RID: 21549 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(RuneSourceType x, RuneSourceType y)
		{
			return x == y;
		}

		// Token: 0x0600542E RID: 21550 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(RuneSourceType obj)
		{
			return (int)obj;
		}
	}
}
