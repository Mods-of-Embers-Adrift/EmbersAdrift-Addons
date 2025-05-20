using System;
using System.Collections.Generic;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C67 RID: 3175
	public struct EffectKeyComparer : IEqualityComparer<EffectKey>
	{
		// Token: 0x06006145 RID: 24901 RVA: 0x00081923 File Offset: 0x0007FB23
		public bool Equals(EffectKey x, EffectKey y)
		{
			return x.Equals(y);
		}

		// Token: 0x06006146 RID: 24902 RVA: 0x0008192D File Offset: 0x0007FB2D
		public int GetHashCode(EffectKey obj)
		{
			return obj.GetHashCode();
		}
	}
}
