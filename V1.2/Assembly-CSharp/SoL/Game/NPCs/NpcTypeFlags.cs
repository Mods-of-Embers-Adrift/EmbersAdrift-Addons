using System;

namespace SoL.Game.NPCs
{
	// Token: 0x02000805 RID: 2053
	[Flags]
	public enum NpcTypeFlags
	{
		// Token: 0x040039DF RID: 14815
		None = 0,
		// Token: 0x040039E0 RID: 14816
		Guard = 1,
		// Token: 0x040039E1 RID: 14817
		Bandit = 2,
		// Token: 0x040039E2 RID: 14818
		PassiveAnimal = 4,
		// Token: 0x040039E3 RID: 14819
		AggressiveAnimal = 8
	}
}
