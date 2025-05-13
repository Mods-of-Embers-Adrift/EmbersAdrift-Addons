using System;

namespace SoL.Networking
{
	// Token: 0x020003D4 RID: 980
	public static class ReplicationFlagsExtension
	{
		// Token: 0x06001A56 RID: 6742 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this ReplicationFlags a, ReplicationFlags b)
		{
			return (a & b) == b;
		}
	}
}
