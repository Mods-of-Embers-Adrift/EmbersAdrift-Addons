using System;

namespace SoL.Game.Culling
{
	// Token: 0x02000CC6 RID: 3270
	public static class CullingExtensions
	{
		// Token: 0x06006315 RID: 25365 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this CullingFlags a, CullingFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x06006316 RID: 25366 RVA: 0x000578B5 File Offset: 0x00055AB5
		public static CullingFlags SetBitFlag(this CullingFlags a, CullingFlags b)
		{
			return a | b;
		}

		// Token: 0x06006317 RID: 25367 RVA: 0x000578BA File Offset: 0x00055ABA
		public static CullingFlags UnsetBitFlag(this CullingFlags a, CullingFlags b)
		{
			return a & ~b;
		}

		// Token: 0x06006318 RID: 25368 RVA: 0x00082CB7 File Offset: 0x00080EB7
		public static CullingFlags UnsetLimitBitFlags(this CullingFlags flags)
		{
			return flags & ~CullingExtensions.GetLimitFlags();
		}

		// Token: 0x06006319 RID: 25369 RVA: 0x00082CC1 File Offset: 0x00080EC1
		private static CullingFlags GetLimitFlags()
		{
			return CullingFlags.LightShadowLimit | CullingFlags.ObjectShadowLimit | CullingFlags.IKLimit | CullingFlags.UmaFeatureLimit | CullingFlags.Physics;
		}
	}
}
