using System;

namespace SoL.Game
{
	// Token: 0x020005AF RID: 1455
	[Flags]
	public enum PlayerFlags
	{
		// Token: 0x04002D45 RID: 11589
		None = 0,
		// Token: 0x04002D46 RID: 11590
		InCombat = 1,
		// Token: 0x04002D47 RID: 11591
		InCampfire = 2,
		// Token: 0x04002D48 RID: 11592
		InGroup = 4,
		// Token: 0x04002D49 RID: 11593
		MissingBag = 8,
		// Token: 0x04002D4A RID: 11594
		InTrade = 16,
		// Token: 0x04002D4B RID: 11595
		RemoteContainer = 32,
		// Token: 0x04002D4C RID: 11596
		Invulnerable = 64,
		// Token: 0x04002D4D RID: 11597
		Invisible = 128,
		// Token: 0x04002D4E RID: 11598
		NoTarget = 256,
		// Token: 0x04002D4F RID: 11599
		OnRoad = 512,
		// Token: 0x04002D50 RID: 11600
		Pvp = 1024
	}
}
