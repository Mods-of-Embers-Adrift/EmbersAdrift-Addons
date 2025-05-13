using System;
using SoL.Managers;
using SoL.Networking.Managers;
using UnityEngine;

namespace SoL.Utilities.Logging
{
	// Token: 0x02000311 RID: 785
	public abstract class BasePerformanceMonitor : MonoBehaviour
	{
		// Token: 0x17000544 RID: 1348
		// (get) Token: 0x060015F2 RID: 5618
		protected abstract float LogCadence { get; }

		// Token: 0x060015F3 RID: 5619
		protected abstract bool CanUpdate();

		// Token: 0x060015F4 RID: 5620
		protected abstract void LogToIndex(SolPerformanceTimerReport fpsReport, BasePerformanceMonitor.NetworkPerformanceReport networkReport);

		// Token: 0x060015F5 RID: 5621 RVA: 0x000FDF14 File Offset: 0x000FC114
		protected virtual void Awake()
		{
			this.m_fpsPerformanceTimer = new SolFpsPerformanceTimer();
			this.m_nextLog = DateTime.UtcNow.AddSeconds((double)this.LogCadence);
		}

		// Token: 0x060015F6 RID: 5622 RVA: 0x000FDF48 File Offset: 0x000FC148
		private void LateUpdate()
		{
			if (!this.CanUpdate())
			{
				this.m_fpsPerformanceTimer.Paused = true;
				return;
			}
			DateTime utcNow = DateTime.UtcNow;
			if (this.m_fpsPerformanceTimer.Paused)
			{
				this.m_fpsPerformanceTimer.Paused = false;
				this.m_nextLog = utcNow.AddSeconds((double)this.LogCadence);
			}
			else
			{
				this.m_fpsPerformanceTimer.Update();
			}
			if (utcNow >= this.m_nextLog)
			{
				this.m_nextLog = utcNow.AddSeconds((double)this.LogCadence);
				BasePerformanceMonitor.NetworkPerformanceData networkPerformanceData = new BasePerformanceMonitor.NetworkPerformanceData
				{
					Timestamp = utcNow,
					PacketsSent = (ulong)NetworkManager.MyHost.PacketsSent,
					PacketsReceived = (ulong)NetworkManager.MyHost.PacketsReceived,
					PacketsLost = (GameManager.IsServer ? 0UL : NetworkManager.MyPeer.PacketsLost),
					BytesSent = (ulong)NetworkManager.MyHost.BytesSent,
					BytesReceived = (ulong)NetworkManager.MyHost.BytesReceived
				};
				BasePerformanceMonitor.NetworkPerformanceReport networkReport = new BasePerformanceMonitor.NetworkPerformanceReport
				{
					TimeDelta = networkPerformanceData.Timestamp - this.m_lastNetworkPerformanceData.Timestamp,
					PacketsSent = networkPerformanceData.PacketsSent - this.m_lastNetworkPerformanceData.PacketsSent,
					PacketsReceived = networkPerformanceData.PacketsReceived - this.m_lastNetworkPerformanceData.PacketsReceived,
					PacketsLost = networkPerformanceData.PacketsLost - this.m_lastNetworkPerformanceData.PacketsLost,
					BytesSent = networkPerformanceData.BytesSent - this.m_lastNetworkPerformanceData.BytesSent,
					BytesReceived = networkPerformanceData.BytesReceived - this.m_lastNetworkPerformanceData.BytesReceived
				};
				this.m_lastNetworkPerformanceData = networkPerformanceData;
				this.LogToIndex(this.m_fpsPerformanceTimer.GenerateReport(), networkReport);
			}
		}

		// Token: 0x060015F7 RID: 5623 RVA: 0x000FE108 File Offset: 0x000FC308
		protected void ResetMeasurements()
		{
			this.m_nextLog = DateTime.UtcNow.AddSeconds((double)this.LogCadence);
			this.m_lastNetworkPerformanceData = default(BasePerformanceMonitor.NetworkPerformanceData);
			SolFpsPerformanceTimer fpsPerformanceTimer = this.m_fpsPerformanceTimer;
			if (fpsPerformanceTimer == null)
			{
				return;
			}
			fpsPerformanceTimer.Reset();
		}

		// Token: 0x04001DE8 RID: 7656
		private DateTime m_nextLog = DateTime.MinValue;

		// Token: 0x04001DE9 RID: 7657
		private BasePerformanceMonitor.NetworkPerformanceData m_lastNetworkPerformanceData;

		// Token: 0x04001DEA RID: 7658
		private SolFpsPerformanceTimer m_fpsPerformanceTimer;

		// Token: 0x02000312 RID: 786
		private struct NetworkPerformanceData
		{
			// Token: 0x04001DEB RID: 7659
			public DateTime Timestamp;

			// Token: 0x04001DEC RID: 7660
			public ulong PacketsSent;

			// Token: 0x04001DED RID: 7661
			public ulong PacketsReceived;

			// Token: 0x04001DEE RID: 7662
			public ulong PacketsLost;

			// Token: 0x04001DEF RID: 7663
			public ulong BytesSent;

			// Token: 0x04001DF0 RID: 7664
			public ulong BytesReceived;
		}

		// Token: 0x02000313 RID: 787
		protected struct NetworkPerformanceReport
		{
			// Token: 0x04001DF1 RID: 7665
			public TimeSpan TimeDelta;

			// Token: 0x04001DF2 RID: 7666
			public ulong PacketsSent;

			// Token: 0x04001DF3 RID: 7667
			public ulong PacketsReceived;

			// Token: 0x04001DF4 RID: 7668
			public ulong PacketsLost;

			// Token: 0x04001DF5 RID: 7669
			public ulong BytesSent;

			// Token: 0x04001DF6 RID: 7670
			public ulong BytesReceived;
		}
	}
}
