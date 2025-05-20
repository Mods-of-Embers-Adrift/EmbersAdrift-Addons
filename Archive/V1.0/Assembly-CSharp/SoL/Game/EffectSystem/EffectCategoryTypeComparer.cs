using System;
using System.Collections.Generic;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C5A RID: 3162
	internal struct EffectCategoryTypeComparer : IEqualityComparer<EffectCategoryType>
	{
		// Token: 0x0600612F RID: 24879 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(EffectCategoryType x, EffectCategoryType y)
		{
			return x == y;
		}

		// Token: 0x06006130 RID: 24880 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(EffectCategoryType obj)
		{
			return (int)obj;
		}
	}
}
