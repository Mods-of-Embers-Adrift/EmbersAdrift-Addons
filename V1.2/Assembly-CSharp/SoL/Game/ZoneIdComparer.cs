using System;
using System.Collections.Generic;

namespace SoL.Game
{
	// Token: 0x02000608 RID: 1544
	public struct ZoneIdComparer : IEqualityComparer<ZoneId>
	{
		// Token: 0x06003132 RID: 12594 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(ZoneId x, ZoneId y)
		{
			return x == y;
		}

		// Token: 0x06003133 RID: 12595 RVA: 0x00061E21 File Offset: 0x00060021
		public int GetHashCode(ZoneId obj)
		{
			return obj.GetHashCode();
		}
	}
}
