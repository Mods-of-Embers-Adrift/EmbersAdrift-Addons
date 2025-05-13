using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C5C RID: 3164
	public static class EffectCategoryExtensions
	{
		// Token: 0x06006133 RID: 24883 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this EffectCategoryFlags a, EffectCategoryFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x06006134 RID: 24884 RVA: 0x000502DF File Offset: 0x0004E4DF
		public static bool HasAnyFlags(this EffectCategoryFlags a, EffectCategoryFlags b)
		{
			return a != EffectCategoryFlags.None && b != EffectCategoryFlags.None && (a & b) > EffectCategoryFlags.None;
		}
	}
}
