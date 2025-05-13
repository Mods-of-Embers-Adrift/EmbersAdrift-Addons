using System;
using System.Collections.Generic;

namespace SoL.Utilities
{
	// Token: 0x02000265 RID: 613
	public struct CursorImageComparer : IEqualityComparer<CursorType>
	{
		// Token: 0x06001374 RID: 4980 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(CursorType x, CursorType y)
		{
			return x == y;
		}

		// Token: 0x06001375 RID: 4981 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(CursorType obj)
		{
			return (int)obj;
		}
	}
}
