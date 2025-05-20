using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002E4 RID: 740
	public class SolPerformanceTimer : SolPerformanceTimerBase
	{
		// Token: 0x06001537 RID: 5431 RVA: 0x000FC2F4 File Offset: 0x000FA4F4
		public void StartMonitor()
		{
			if (this.m_previousFrameCount == -1)
			{
				this.m_previousFrameCount = Time.frameCount;
				this.m_stopwatch.Restart();
				return;
			}
			if (this.m_previousFrameCount != Time.frameCount)
			{
				this.m_values.Add(this.m_stopwatch.ElapsedTicks);
				this.m_stopwatch.Restart();
				this.m_previousFrameCount = Time.frameCount;
			}
		}

		// Token: 0x06001538 RID: 5432 RVA: 0x00050E40 File Offset: 0x0004F040
		public void EndMonitor()
		{
			this.m_stopwatch.Stop();
		}

		// Token: 0x06001539 RID: 5433 RVA: 0x000FC35C File Offset: 0x000FA55C
		protected override SolPerformanceTimerReport GenerateReportInternal(long min, long max, float avg, float std, int count, TimeSpan elapsed)
		{
			return new SolPerformanceTimerReport
			{
				Min = TimeSpan.FromTicks(min).TotalMilliseconds,
				Max = TimeSpan.FromTicks(max).TotalMilliseconds,
				Avg = TimeSpan.FromTicks((long)avg).TotalMilliseconds,
				Std = TimeSpan.FromTicks((long)std).TotalMilliseconds,
				Count = (long)count,
				Elapsed = elapsed
			};
		}

		// Token: 0x04001D6E RID: 7534
		private int m_previousFrameCount = -1;
	}
}
