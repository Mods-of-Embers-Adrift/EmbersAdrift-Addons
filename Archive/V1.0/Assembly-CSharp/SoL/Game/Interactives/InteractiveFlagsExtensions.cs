using System;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B8C RID: 2956
	public static class InteractiveFlagsExtensions
	{
		// Token: 0x06005B1D RID: 23325 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this InteractiveFlags a, InteractiveFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x06005B1E RID: 23326 RVA: 0x000578B5 File Offset: 0x00055AB5
		public static InteractiveFlags SetBitFlag(this InteractiveFlags a, InteractiveFlags b)
		{
			return a | b;
		}

		// Token: 0x06005B1F RID: 23327 RVA: 0x0007D3EE File Offset: 0x0007B5EE
		public static InteractiveFlags UnsetBitFlag(this InteractiveFlags a, InteractiveFlags b)
		{
			return a & ~b;
		}
	}
}
