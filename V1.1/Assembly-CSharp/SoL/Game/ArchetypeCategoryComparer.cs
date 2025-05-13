using System;
using System.Collections.Generic;

namespace SoL.Game
{
	// Token: 0x02000556 RID: 1366
	public struct ArchetypeCategoryComparer : IEqualityComparer<ArchetypeCategory>
	{
		// Token: 0x0600297C RID: 10620 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(ArchetypeCategory x, ArchetypeCategory y)
		{
			return x == y;
		}

		// Token: 0x0600297D RID: 10621 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(ArchetypeCategory obj)
		{
			return (int)obj;
		}
	}
}
