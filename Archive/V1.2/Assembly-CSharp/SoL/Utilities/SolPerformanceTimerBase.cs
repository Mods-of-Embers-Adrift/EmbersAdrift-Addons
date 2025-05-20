using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002E3 RID: 739
	public abstract class SolPerformanceTimerBase
	{
		// Token: 0x06001533 RID: 5427
		protected abstract SolPerformanceTimerReport GenerateReportInternal(long min, long max, float avg, float std, int count, TimeSpan elapsed);

		// Token: 0x06001534 RID: 5428 RVA: 0x00050DE9 File Offset: 0x0004EFE9
		protected SolPerformanceTimerBase()
		{
			this.m_stopwatch = new Stopwatch();
			this.m_values = new List<long>(1000);
			this.Reset();
		}

		// Token: 0x06001535 RID: 5429 RVA: 0x00050E1D File Offset: 0x0004F01D
		public virtual void Reset()
		{
			this.m_stopwatch.Restart();
			this.m_values.Clear();
			this.m_lastReportTime = DateTime.UtcNow;
		}

		// Token: 0x06001536 RID: 5430 RVA: 0x000FC208 File Offset: 0x000FA408
		public SolPerformanceTimerReport GenerateReport()
		{
			int count = this.m_values.Count;
			long num = 0L;
			long num2 = 0L;
			long num3 = 0L;
			for (int i = 0; i < count; i++)
			{
				if (this.m_values[i] < num2)
				{
					num2 = this.m_values[i];
				}
				if (this.m_values[i] > num)
				{
					num = this.m_values[i];
				}
				num3 += this.m_values[i];
			}
			float num4 = (count > 0) ? ((float)num3 / (float)count) : 0f;
			float std = 0f;
			if (count > 1)
			{
				float num5 = 0f;
				for (int j = 0; j < count; j++)
				{
					float num6 = (float)this.m_values[j] - num4;
					num5 += num6 * num6;
				}
				std = Mathf.Sqrt(num5 / ((float)count - 1f));
			}
			TimeSpan elapsed = DateTime.UtcNow - this.m_lastReportTime;
			this.Reset();
			return this.GenerateReportInternal(num2, num, num4, std, count, elapsed);
		}

		// Token: 0x04001D6B RID: 7531
		protected readonly Stopwatch m_stopwatch;

		// Token: 0x04001D6C RID: 7532
		protected readonly List<long> m_values;

		// Token: 0x04001D6D RID: 7533
		private DateTime m_lastReportTime = DateTime.MinValue;
	}
}
