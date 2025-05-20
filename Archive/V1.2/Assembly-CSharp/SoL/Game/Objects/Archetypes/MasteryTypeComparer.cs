using System;
using System.Collections.Generic;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AD7 RID: 2775
	public struct MasteryTypeComparer : IEqualityComparer<MasteryType>
	{
		// Token: 0x0600559B RID: 21915 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(MasteryType x, MasteryType y)
		{
			return x == y;
		}

		// Token: 0x0600559C RID: 21916 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(MasteryType obj)
		{
			return (int)obj;
		}
	}
}
