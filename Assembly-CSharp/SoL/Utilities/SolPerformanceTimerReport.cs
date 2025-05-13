using System;

namespace SoL.Utilities
{
	// Token: 0x020002E2 RID: 738
	public struct SolPerformanceTimerReport
	{
		// Token: 0x04001D65 RID: 7525
		public double Min;

		// Token: 0x04001D66 RID: 7526
		public double Max;

		// Token: 0x04001D67 RID: 7527
		public double Avg;

		// Token: 0x04001D68 RID: 7528
		public double Std;

		// Token: 0x04001D69 RID: 7529
		public long Count;

		// Token: 0x04001D6A RID: 7530
		public TimeSpan Elapsed;
	}
}
