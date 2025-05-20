using System;
using System.Collections.Generic;

namespace SoL.Game.UI
{
	// Token: 0x02000859 RID: 2137
	internal struct BindingTypeComparer : IEqualityComparer<BindingType>
	{
		// Token: 0x06003DB3 RID: 15795 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(BindingType x, BindingType y)
		{
			return x == y;
		}

		// Token: 0x06003DB4 RID: 15796 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(BindingType obj)
		{
			return (int)obj;
		}
	}
}
