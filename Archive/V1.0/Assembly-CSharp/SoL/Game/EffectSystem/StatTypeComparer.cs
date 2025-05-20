using System;
using System.Collections.Generic;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C4B RID: 3147
	public struct StatTypeComparer : IEqualityComparer<StatType>
	{
		// Token: 0x060060ED RID: 24813 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(StatType x, StatType y)
		{
			return x == y;
		}

		// Token: 0x060060EE RID: 24814 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(StatType obj)
		{
			return (int)obj;
		}
	}
}
