using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C4E RID: 3150
	[Flags]
	public enum BehaviorEffectTypeFlags
	{
		// Token: 0x040053E2 RID: 21474
		None = 0,
		// Token: 0x040053E3 RID: 21475
		Stunned = 1,
		// Token: 0x040053E4 RID: 21476
		Feared = 2,
		// Token: 0x040053E5 RID: 21477
		Charmed = 4,
		// Token: 0x040053E6 RID: 21478
		Dazed = 8,
		// Token: 0x040053E7 RID: 21479
		Enraged = 16,
		// Token: 0x040053E8 RID: 21480
		Confused = 32,
		// Token: 0x040053E9 RID: 21481
		Lull = 64
	}
}
