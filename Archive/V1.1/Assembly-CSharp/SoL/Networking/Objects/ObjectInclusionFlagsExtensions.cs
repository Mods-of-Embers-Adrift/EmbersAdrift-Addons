using System;

namespace SoL.Networking.Objects
{
	// Token: 0x020004BA RID: 1210
	public static class ObjectInclusionFlagsExtensions
	{
		// Token: 0x060021EC RID: 8684 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this NetworkInclusionFlags a, NetworkInclusionFlags b)
		{
			return (a & b) == b;
		}
	}
}
