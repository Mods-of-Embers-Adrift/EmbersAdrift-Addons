using System;

namespace SoL.Networking.Replication
{
	// Token: 0x02000481 RID: 1153
	public static class ReplicatorTypesExtensions
	{
		// Token: 0x06002042 RID: 8258 RVA: 0x000578B5 File Offset: 0x00055AB5
		public static ReplicatorTypes SetFlag(this ReplicatorTypes a, ReplicatorTypes b)
		{
			return a | b;
		}

		// Token: 0x06002043 RID: 8259 RVA: 0x000578BA File Offset: 0x00055ABA
		public static ReplicatorTypes UnsetFlag(this ReplicatorTypes a, ReplicatorTypes b)
		{
			return a & ~b;
		}

		// Token: 0x06002044 RID: 8260 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this ReplicatorTypes a, ReplicatorTypes b)
		{
			return (a & b) == b;
		}

		// Token: 0x06002045 RID: 8261 RVA: 0x000578C0 File Offset: 0x00055AC0
		public static bool LocalClientIsAuthority(this ReplicatorTypes type)
		{
			return type - ReplicatorTypes.Transform <= 1 || type == ReplicatorTypes.Animancer;
		}
	}
}
