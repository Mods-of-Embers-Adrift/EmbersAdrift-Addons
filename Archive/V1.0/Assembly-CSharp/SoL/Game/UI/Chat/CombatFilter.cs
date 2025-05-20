using System;

namespace SoL.Game.UI.Chat
{
	// Token: 0x020009B3 RID: 2483
	[Flags]
	public enum CombatFilter
	{
		// Token: 0x040045A7 RID: 17831
		All = 15,
		// Token: 0x040045A8 RID: 17832
		MyCombatOut = 1,
		// Token: 0x040045A9 RID: 17833
		MyCombatIn = 2,
		// Token: 0x040045AA RID: 17834
		OtherCombat = 4,
		// Token: 0x040045AB RID: 17835
		WarlordSongs = 8
	}
}
