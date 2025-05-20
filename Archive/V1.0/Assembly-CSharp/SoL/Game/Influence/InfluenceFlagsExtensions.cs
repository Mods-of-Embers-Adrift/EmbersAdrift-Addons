using System;

namespace SoL.Game.Influence
{
	// Token: 0x02000BC1 RID: 3009
	public static class InfluenceFlagsExtensions
	{
		// Token: 0x06005D1C RID: 23836 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this InfluenceFlags a, InfluenceFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x06005D1D RID: 23837 RVA: 0x000578B5 File Offset: 0x00055AB5
		public static InfluenceFlags SetBitFlag(this InfluenceFlags a, InfluenceFlags b)
		{
			return a | b;
		}

		// Token: 0x06005D1E RID: 23838 RVA: 0x000578BA File Offset: 0x00055ABA
		public static InfluenceFlags UnsetBitFlag(this InfluenceFlags a, InfluenceFlags b)
		{
			return a & ~b;
		}
	}
}
