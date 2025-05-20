using System;

namespace SoL.Game.Quests.Objectives
{
	// Token: 0x0200079B RID: 1947
	[Flags]
	public enum CraftResult
	{
		// Token: 0x04003830 RID: 14384
		None = 0,
		// Token: 0x04003831 RID: 14385
		Success = 1,
		// Token: 0x04003832 RID: 14386
		FailureItem = 2,
		// Token: 0x04003833 RID: 14387
		FailureNoItem = 4
	}
}
