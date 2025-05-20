using System;
using System.Collections.Generic;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C78 RID: 3192
	public struct HitTypeComparer : IEqualityComparer<HitType>
	{
		// Token: 0x0600616F RID: 24943 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(HitType x, HitType y)
		{
			return x == y;
		}

		// Token: 0x06006170 RID: 24944 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(HitType obj)
		{
			return (int)obj;
		}
	}
}
