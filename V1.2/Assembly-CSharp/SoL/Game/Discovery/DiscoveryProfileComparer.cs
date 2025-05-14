using System;
using System.Collections.Generic;

namespace SoL.Game.Discovery
{
	// Token: 0x02000CA5 RID: 3237
	public struct DiscoveryProfileComparer : IEqualityComparer<DiscoveryProfile>
	{
		// Token: 0x0600621E RID: 25118 RVA: 0x0020386C File Offset: 0x00201A6C
		public bool Equals(DiscoveryProfile x, DiscoveryProfile y)
		{
			return x == y || (x != null && y != null && !(x.GetType() != y.GetType()) && x.Id.Equals(y.Id));
		}

		// Token: 0x0600621F RID: 25119 RVA: 0x002038B4 File Offset: 0x00201AB4
		public int GetHashCode(DiscoveryProfile obj)
		{
			return obj.Id.GetHashCode();
		}
	}
}
