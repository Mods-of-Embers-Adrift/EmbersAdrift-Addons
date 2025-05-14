using System;
using System.Collections.Generic;

namespace SoL.Game.UMA
{
	// Token: 0x02000622 RID: 1570
	public struct MaterialColorTypeComparer : IEqualityComparer<MaterialColorType>
	{
		// Token: 0x060031A6 RID: 12710 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(MaterialColorType x, MaterialColorType y)
		{
			return x == y;
		}

		// Token: 0x060031A7 RID: 12711 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(MaterialColorType obj)
		{
			return (int)obj;
		}
	}
}
