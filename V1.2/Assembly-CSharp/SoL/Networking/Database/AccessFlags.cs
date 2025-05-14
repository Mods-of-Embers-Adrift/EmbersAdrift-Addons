using System;

namespace SoL.Networking.Database
{
	// Token: 0x02000415 RID: 1045
	[Flags]
	public enum AccessFlags
	{
		// Token: 0x0400237F RID: 9087
		None = 0,
		// Token: 0x04002380 RID: 9088
		Active = 1,
		// Token: 0x04002381 RID: 9089
		GM = 2,
		// Token: 0x04002382 RID: 9090
		DevPlus = 4,
		// Token: 0x04002383 RID: 9091
		Streamer = 8,
		// Token: 0x04002384 RID: 9092
		FullClient = 16,
		// Token: 0x04002385 RID: 9093
		Subscriber = 32
	}
}
