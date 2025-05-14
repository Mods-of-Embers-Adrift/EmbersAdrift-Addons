using System;
using System.Collections.Generic;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C0E RID: 3086
	public struct DiminishingReturnTypeComparer : IEqualityComparer<DiminishingReturnType>
	{
		// Token: 0x06005EF9 RID: 24313 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(DiminishingReturnType x, DiminishingReturnType y)
		{
			return x == y;
		}

		// Token: 0x06005EFA RID: 24314 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(DiminishingReturnType obj)
		{
			return (int)obj;
		}
	}
}
