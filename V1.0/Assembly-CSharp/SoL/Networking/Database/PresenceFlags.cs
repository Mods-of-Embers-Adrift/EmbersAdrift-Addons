using System;

namespace SoL.Networking.Database
{
	// Token: 0x02000464 RID: 1124
	[Flags]
	public enum PresenceFlags : byte
	{
		// Token: 0x04002507 RID: 9479
		Invalid = 0,
		// Token: 0x04002508 RID: 9480
		Online = 1,
		// Token: 0x04002509 RID: 9481
		AwayUserSet = 2,
		// Token: 0x0400250A RID: 9482
		AwayAutomatic = 4,
		// Token: 0x0400250B RID: 9483
		DoNotDisturb = 8,
		// Token: 0x0400250C RID: 9484
		Anonymous = 16,
		// Token: 0x0400250D RID: 9485
		Invisible = 32,
		// Token: 0x0400250E RID: 9486
		GM = 64
	}
}
