using System;
using System.Collections.Generic;

namespace SoL
{
	// Token: 0x0200022F RID: 559
	public struct UniqueIdComparer : IEqualityComparer<UniqueId>
	{
		// Token: 0x060012C4 RID: 4804 RVA: 0x0004F592 File Offset: 0x0004D792
		public bool Equals(UniqueId x, UniqueId y)
		{
			return x.Equals(y);
		}

		// Token: 0x060012C5 RID: 4805 RVA: 0x0004F59C File Offset: 0x0004D79C
		public int GetHashCode(UniqueId obj)
		{
			return obj.GetHashCode();
		}
	}
}
