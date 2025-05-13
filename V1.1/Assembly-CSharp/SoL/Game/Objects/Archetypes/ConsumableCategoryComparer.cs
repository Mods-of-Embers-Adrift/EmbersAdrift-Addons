using System;
using System.Collections.Generic;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A5B RID: 2651
	public struct ConsumableCategoryComparer : IEqualityComparer<ConsumableCategory>
	{
		// Token: 0x0600521E RID: 21022 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(ConsumableCategory x, ConsumableCategory y)
		{
			return x == y;
		}

		// Token: 0x0600521F RID: 21023 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(ConsumableCategory obj)
		{
			return (int)obj;
		}
	}
}
