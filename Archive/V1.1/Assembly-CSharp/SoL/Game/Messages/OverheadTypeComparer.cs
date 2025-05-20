using System;
using System.Collections.Generic;

namespace SoL.Game.Messages
{
	// Token: 0x020009E9 RID: 2537
	public struct OverheadTypeComparer : IEqualityComparer<OverheadType>
	{
		// Token: 0x06004D2D RID: 19757 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(OverheadType x, OverheadType y)
		{
			return x == y;
		}

		// Token: 0x06004D2E RID: 19758 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(OverheadType obj)
		{
			return (int)obj;
		}
	}
}
