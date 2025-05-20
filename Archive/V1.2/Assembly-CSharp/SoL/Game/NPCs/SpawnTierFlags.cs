using System;

namespace SoL.Game.NPCs
{
	// Token: 0x02000829 RID: 2089
	[Flags]
	public enum SpawnTierFlags
	{
		// Token: 0x04003B75 RID: 15221
		None = 0,
		// Token: 0x04003B76 RID: 15222
		Weak = 1,
		// Token: 0x04003B77 RID: 15223
		Normal = 2,
		// Token: 0x04003B78 RID: 15224
		Strong = 4,
		// Token: 0x04003B79 RID: 15225
		Champion = 8,
		// Token: 0x04003B7A RID: 15226
		Elite = 16,
		// Token: 0x04003B7B RID: 15227
		Boss = 32,
		// Token: 0x04003B7C RID: 15228
		Epic = 64,
		// Token: 0x04003B7D RID: 15229
		All = -1
	}
}
