using System;
using System.Collections.Generic;

namespace SoL.Game
{
	// Token: 0x02000609 RID: 1545
	public struct SubZoneIdComparer : IEqualityComparer<SubZoneId>
	{
		// Token: 0x06003134 RID: 12596 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(SubZoneId x, SubZoneId y)
		{
			return x == y;
		}

		// Token: 0x06003135 RID: 12597 RVA: 0x00061E30 File Offset: 0x00060030
		public int GetHashCode(SubZoneId obj)
		{
			return obj.GetHashCode();
		}
	}
}
