using System;
using System.Collections.Generic;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C74 RID: 3188
	public struct StatusEffectTypeComparer : IEqualityComparer<StatusEffectType>
	{
		// Token: 0x06006157 RID: 24919 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(StatusEffectType x, StatusEffectType y)
		{
			return x == y;
		}

		// Token: 0x06006158 RID: 24920 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(StatusEffectType obj)
		{
			return (int)obj;
		}
	}
}
