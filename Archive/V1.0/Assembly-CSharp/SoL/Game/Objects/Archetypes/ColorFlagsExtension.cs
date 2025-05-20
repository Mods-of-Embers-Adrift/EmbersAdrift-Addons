using System;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A86 RID: 2694
	public static class ColorFlagsExtension
	{
		// Token: 0x06005380 RID: 21376 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this ItemCategory.ColorFlags a, ItemCategory.ColorFlags b)
		{
			return (a & b) == b;
		}
	}
}
