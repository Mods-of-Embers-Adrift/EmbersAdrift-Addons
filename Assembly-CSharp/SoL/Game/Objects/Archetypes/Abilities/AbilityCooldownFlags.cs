using System;

namespace SoL.Game.Objects.Archetypes.Abilities
{
	// Token: 0x02000AEA RID: 2794
	[Flags]
	public enum AbilityCooldownFlags
	{
		// Token: 0x04004C5F RID: 19551
		None = 0,
		// Token: 0x04004C60 RID: 19552
		Regular = 1,
		// Token: 0x04004C61 RID: 19553
		Execution = 2,
		// Token: 0x04004C62 RID: 19554
		Global = 4,
		// Token: 0x04004C63 RID: 19555
		Memorization = 8,
		// Token: 0x04004C64 RID: 19556
		WeaponRuneSwap = 16,
		// Token: 0x04004C65 RID: 19557
		Alchemy = 32
	}
}
