using System;
using System.Collections.Generic;

namespace SoL.Networking
{
	// Token: 0x020003C0 RID: 960
	public struct NetworkChannelComparer : IEqualityComparer<NetworkChannel>
	{
		// Token: 0x060019EE RID: 6638 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(NetworkChannel x, NetworkChannel y)
		{
			return x == y;
		}

		// Token: 0x060019EF RID: 6639 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(NetworkChannel obj)
		{
			return (int)obj;
		}
	}
}
