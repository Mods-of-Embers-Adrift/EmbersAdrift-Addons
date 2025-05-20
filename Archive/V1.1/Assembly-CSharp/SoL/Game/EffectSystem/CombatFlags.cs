using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C64 RID: 3172
	[Flags]
	public enum CombatFlags
	{
		// Token: 0x0400546F RID: 21615
		None = 0,
		// Token: 0x04005470 RID: 21616
		Advantage = 1,
		// Token: 0x04005471 RID: 21617
		Disadvantage = 2,
		// Token: 0x04005472 RID: 21618
		IgnoreActiveDefenses = 4
	}
}
