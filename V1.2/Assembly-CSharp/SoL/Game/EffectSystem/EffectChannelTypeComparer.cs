using System;
using System.Collections.Generic;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C5E RID: 3166
	public struct EffectChannelTypeComparer : IEqualityComparer<EffectChannelType>
	{
		// Token: 0x06006135 RID: 24885 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(EffectChannelType x, EffectChannelType y)
		{
			return x == y;
		}

		// Token: 0x06006136 RID: 24886 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(EffectChannelType obj)
		{
			return (int)obj;
		}
	}
}
