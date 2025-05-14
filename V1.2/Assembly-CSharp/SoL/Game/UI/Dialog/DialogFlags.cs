using System;

namespace SoL.Game.UI.Dialog
{
	// Token: 0x02000989 RID: 2441
	[Flags]
	public enum DialogFlags
	{
		// Token: 0x040043EB RID: 17387
		None = 0,
		// Token: 0x040043EC RID: 17388
		Emotive = 1,
		// Token: 0x040043ED RID: 17389
		Player = 2,
		// Token: 0x040043EE RID: 17390
		Warning = 4,
		// Token: 0x040043EF RID: 17391
		StageDirection = 8,
		// Token: 0x040043F0 RID: 17392
		ForceNewline = 16
	}
}
