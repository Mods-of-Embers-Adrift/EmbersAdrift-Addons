using System;

namespace SoL.Game
{
	// Token: 0x020005EB RID: 1515
	[Flags]
	public enum HealthStateFlags
	{
		// Token: 0x04002ED3 RID: 11987
		None = 0,
		// Token: 0x04002ED4 RID: 11988
		Alive = 1,
		// Token: 0x04002ED5 RID: 11989
		Unconscious = 2,
		// Token: 0x04002ED6 RID: 11990
		WakingUp = 4,
		// Token: 0x04002ED7 RID: 11991
		Dead = 8
	}
}
