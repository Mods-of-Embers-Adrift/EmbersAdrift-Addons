using System;

namespace SoL.Game.Spawning
{
	// Token: 0x0200066D RID: 1645
	[Flags]
	public enum CallForHelpFlags
	{
		// Token: 0x0400314B RID: 12619
		None = 0,
		// Token: 0x0400314C RID: 12620
		OnInitialThreat = 1,
		// Token: 0x0400314D RID: 12621
		Periodically = 2,
		// Token: 0x0400314E RID: 12622
		OnDeath = 4,
		// Token: 0x0400314F RID: 12623
		WhileFleeing = 8
	}
}
