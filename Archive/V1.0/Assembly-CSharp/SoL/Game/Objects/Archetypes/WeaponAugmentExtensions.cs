using System;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AB4 RID: 2740
	public static class WeaponAugmentExtensions
	{
		// Token: 0x0600549D RID: 21661 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this WeaponAugmentFlags a, WeaponAugmentFlags b)
		{
			return (a & b) == b;
		}
	}
}
