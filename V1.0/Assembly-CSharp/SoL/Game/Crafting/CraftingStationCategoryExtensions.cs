using System;

namespace SoL.Game.Crafting
{
	// Token: 0x02000CD7 RID: 3287
	public static class CraftingStationCategoryExtensions
	{
		// Token: 0x0600637E RID: 25470 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this CraftingStationCategory a, CraftingStationCategory b)
		{
			return (a & b) == b;
		}
	}
}
