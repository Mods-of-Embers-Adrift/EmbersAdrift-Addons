using System;

namespace SoL.Game
{
	// Token: 0x0200057E RID: 1406
	[Flags]
	public enum GameEntityTypeFlags
	{
		// Token: 0x04002BBD RID: 11197
		None = 0,
		// Token: 0x04002BBE RID: 11198
		Player = 1,
		// Token: 0x04002BBF RID: 11199
		Npc = 2,
		// Token: 0x04002BC0 RID: 11200
		Interactive = 4
	}
}
