using System;

namespace TriangleNet.Logging
{
	// Token: 0x02000137 RID: 311
	public interface ILogItem
	{
		// Token: 0x17000334 RID: 820
		// (get) Token: 0x06000AA8 RID: 2728
		DateTime Time { get; }

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x06000AA9 RID: 2729
		LogLevel Level { get; }

		// Token: 0x17000336 RID: 822
		// (get) Token: 0x06000AAA RID: 2730
		string Message { get; }

		// Token: 0x17000337 RID: 823
		// (get) Token: 0x06000AAB RID: 2731
		string Info { get; }
	}
}
