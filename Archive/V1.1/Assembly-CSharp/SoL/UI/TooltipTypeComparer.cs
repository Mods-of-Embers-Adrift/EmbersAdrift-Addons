using System;
using System.Collections.Generic;

namespace SoL.UI
{
	// Token: 0x02000384 RID: 900
	public struct TooltipTypeComparer : IEqualityComparer<TooltipType>
	{
		// Token: 0x060018BB RID: 6331 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(TooltipType x, TooltipType y)
		{
			return x == y;
		}

		// Token: 0x060018BC RID: 6332 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(TooltipType obj)
		{
			return (int)obj;
		}
	}
}
