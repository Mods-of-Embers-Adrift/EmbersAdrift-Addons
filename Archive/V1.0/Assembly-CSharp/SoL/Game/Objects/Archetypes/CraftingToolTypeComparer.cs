using System;
using System.Collections.Generic;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A6D RID: 2669
	public struct CraftingToolTypeComparer : IEqualityComparer<CraftingToolType>
	{
		// Token: 0x0600529F RID: 21151 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(CraftingToolType x, CraftingToolType y)
		{
			return x == y;
		}

		// Token: 0x060052A0 RID: 21152 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(CraftingToolType obj)
		{
			return (int)obj;
		}
	}
}
