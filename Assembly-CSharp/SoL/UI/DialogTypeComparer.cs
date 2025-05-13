using System;
using System.Collections.Generic;

namespace SoL.UI
{
	// Token: 0x02000352 RID: 850
	public struct DialogTypeComparer : IEqualityComparer<DialogType>
	{
		// Token: 0x06001738 RID: 5944 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(DialogType x, DialogType y)
		{
			return x == y;
		}

		// Token: 0x06001739 RID: 5945 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(DialogType obj)
		{
			return (int)obj;
		}
	}
}
