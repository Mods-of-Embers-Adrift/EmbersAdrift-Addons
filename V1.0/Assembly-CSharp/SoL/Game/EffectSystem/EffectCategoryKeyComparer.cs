using System;
using System.Collections.Generic;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C5B RID: 3163
	internal struct EffectCategoryKeyComparer : IEqualityComparer<EffectCategoryKey>
	{
		// Token: 0x06006131 RID: 24881 RVA: 0x000817F8 File Offset: 0x0007F9F8
		public bool Equals(EffectCategoryKey x, EffectCategoryKey y)
		{
			return x.Polarity == y.Polarity && x.VariantType == y.VariantType && x.CategoryFlags.HasAnyFlags(y.CategoryFlags);
		}

		// Token: 0x06006132 RID: 24882 RVA: 0x00081829 File Offset: 0x0007FA29
		public int GetHashCode(EffectCategoryKey obj)
		{
			return (int)(((obj.Polarity * (Polarity)397 ^ (Polarity)obj.CategoryFlags) & (Polarity)397) ^ (Polarity)obj.VariantType);
		}
	}
}
