using System;

namespace SoL.Game.Objects.Containers
{
	// Token: 0x02000A0B RID: 2571
	public static class ContainerLockFlagsExtensions
	{
		// Token: 0x06004E84 RID: 20100 RVA: 0x00056E2C File Offset: 0x0005502C
		public static bool IsLocked(this ContainerLockFlags flags)
		{
			return flags > ContainerLockFlags.None;
		}

		// Token: 0x06004E85 RID: 20101 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this ContainerLockFlags a, ContainerLockFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x06004E86 RID: 20102 RVA: 0x000578B5 File Offset: 0x00055AB5
		public static ContainerLockFlags SetBitFlag(this ContainerLockFlags a, ContainerLockFlags b)
		{
			return a | b;
		}

		// Token: 0x06004E87 RID: 20103 RVA: 0x000578BA File Offset: 0x00055ABA
		public static ContainerLockFlags UnsetBitFlag(this ContainerLockFlags a, ContainerLockFlags b)
		{
			return a & ~b;
		}
	}
}
