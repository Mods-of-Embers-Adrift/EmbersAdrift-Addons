using System;

namespace SoL.Utilities
{
	// Token: 0x020002E5 RID: 741
	public class SolFpsPerformanceTimer : SolPerformanceTimerBase
	{
		// Token: 0x17000522 RID: 1314
		// (get) Token: 0x0600153B RID: 5435 RVA: 0x00050E5C File Offset: 0x0004F05C
		// (set) Token: 0x0600153C RID: 5436 RVA: 0x00050E64 File Offset: 0x0004F064
		public bool Paused
		{
			get
			{
				return this.m_paused;
			}
			set
			{
				if (this.m_paused == value)
				{
					return;
				}
				this.m_paused = value;
				if (this.m_paused)
				{
					this.m_stopwatch.Stop();
					return;
				}
				this.Reset();
			}
		}

		// Token: 0x0600153D RID: 5437 RVA: 0x00050E91 File Offset: 0x0004F091
		public override void Reset()
		{
			base.Reset();
			this.m_framesRendered = 0U;
		}

		// Token: 0x0600153E RID: 5438 RVA: 0x000FC3FC File Offset: 0x000FA5FC
		public void Update()
		{
			this.m_framesRendered += 1U;
			if (this.m_stopwatch.ElapsedMilliseconds >= 1000L)
			{
				this.m_values.Add((long)((ulong)this.m_framesRendered));
				this.m_framesRendered = 0U;
				this.m_stopwatch.Restart();
			}
		}

		// Token: 0x0600153F RID: 5439 RVA: 0x000FC450 File Offset: 0x000FA650
		protected override SolPerformanceTimerReport GenerateReportInternal(long min, long max, float avg, float std, int count, TimeSpan elapsed)
		{
			return new SolPerformanceTimerReport
			{
				Min = (double)min,
				Max = (double)max,
				Avg = (double)avg,
				Std = (double)std,
				Count = (long)count,
				Elapsed = elapsed
			};
		}

		// Token: 0x04001D6F RID: 7535
		private const long kMeasureCadenceInS = 1L;

		// Token: 0x04001D70 RID: 7536
		private const long kMeasureCadenceInMs = 1000L;

		// Token: 0x04001D71 RID: 7537
		private uint m_framesRendered;

		// Token: 0x04001D72 RID: 7538
		private bool m_paused = true;
	}
}
