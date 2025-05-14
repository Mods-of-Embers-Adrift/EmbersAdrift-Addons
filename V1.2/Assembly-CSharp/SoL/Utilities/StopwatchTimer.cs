using System;
using System.Diagnostics;
using System.Globalization;
using Cysharp.Text;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002EC RID: 748
	public class StopwatchTimer
	{
		// Token: 0x06001553 RID: 5459 RVA: 0x00051007 File Offset: 0x0004F207
		public StopwatchTimer()
		{
			this.m_stopwatch = new Stopwatch();
		}

		// Token: 0x06001554 RID: 5460 RVA: 0x0005101A File Offset: 0x0004F21A
		public void Start()
		{
			this.m_stopwatch.Reset();
			this.m_stopwatch.Start();
		}

		// Token: 0x06001555 RID: 5461 RVA: 0x00051032 File Offset: 0x0004F232
		public void Stop()
		{
			this.m_stopwatch.Stop();
		}

		// Token: 0x06001556 RID: 5462 RVA: 0x000FC5B0 File Offset: 0x000FA7B0
		public void StopAndPrintElapsed(string msg)
		{
			this.Stop();
			UnityEngine.Debug.Log(ZString.Format<string, string, long>("[{0}] {1}: {2}ms", DateTime.Now.ToString(CultureInfo.InvariantCulture), msg, this.m_stopwatch.ElapsedMilliseconds));
		}

		// Token: 0x04001D7D RID: 7549
		private readonly Stopwatch m_stopwatch;
	}
}
