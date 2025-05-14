using System;
using System.Collections.Generic;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C75 RID: 3189
	public struct StatusEffectSubTypeComparer : IEqualityComparer<StatusEffectSubType>
	{
		// Token: 0x06006159 RID: 24921 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(StatusEffectSubType x, StatusEffectSubType y)
		{
			return x == y;
		}

		// Token: 0x0600615A RID: 24922 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(StatusEffectSubType obj)
		{
			return (int)obj;
		}
	}
}
