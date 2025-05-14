using System;

namespace SoL.Game
{
	// Token: 0x020005D4 RID: 1492
	[Flags]
	public enum StanceFlags
	{
		// Token: 0x04002E7B RID: 11899
		None = 0,
		// Token: 0x04002E7C RID: 11900
		Idle = 1,
		// Token: 0x04002E7D RID: 11901
		Crouch = 2,
		// Token: 0x04002E7E RID: 11902
		Combat = 4,
		// Token: 0x04002E7F RID: 11903
		Torch = 8,
		// Token: 0x04002E80 RID: 11904
		Sit = 16,
		// Token: 0x04002E81 RID: 11905
		Swim = 32,
		// Token: 0x04002E82 RID: 11906
		Unconscious = 64,
		// Token: 0x04002E83 RID: 11907
		Looting = 128
	}
}
