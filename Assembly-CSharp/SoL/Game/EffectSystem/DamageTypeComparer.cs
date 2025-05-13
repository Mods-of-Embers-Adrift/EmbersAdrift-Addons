using System;
using System.Collections.Generic;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C53 RID: 3155
	public struct DamageTypeComparer : IEqualityComparer<DamageType>
	{
		// Token: 0x0600611A RID: 24858 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(DamageType x, DamageType y)
		{
			return x == y;
		}

		// Token: 0x0600611B RID: 24859 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(DamageType obj)
		{
			return (int)obj;
		}
	}
}
