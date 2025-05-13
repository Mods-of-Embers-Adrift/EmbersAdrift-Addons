using System;
using System.Collections.Generic;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C6D RID: 3181
	public struct EffectSubChannelTypeComparer : IEqualityComparer<EffectSubChannelType>
	{
		// Token: 0x06006149 RID: 24905 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(EffectSubChannelType x, EffectSubChannelType y)
		{
			return x == y;
		}

		// Token: 0x0600614A RID: 24906 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(EffectSubChannelType obj)
		{
			return (int)obj;
		}
	}
}
