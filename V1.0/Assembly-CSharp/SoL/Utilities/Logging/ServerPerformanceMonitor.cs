using System;
using SoL.Managers;
using SoL.Networking.Managers;

namespace SoL.Utilities.Logging
{
	// Token: 0x02000319 RID: 793
	public class ServerPerformanceMonitor : BasePerformanceMonitor
	{
		// Token: 0x1700054A RID: 1354
		// (get) Token: 0x06001614 RID: 5652 RVA: 0x00051717 File Offset: 0x0004F917
		private bool HostIsValid
		{
			get
			{
				return LocalZoneManager.ZoneRecord != null && NetworkManager.MyHost != null && NetworkManager.MyHost.IsSet;
			}
		}

		// Token: 0x1700054B RID: 1355
		// (get) Token: 0x06001615 RID: 5653 RVA: 0x0005179F File Offset: 0x0004F99F
		protected override float LogCadence
		{
			get
			{
				return 60f;
			}
		}

		// Token: 0x06001616 RID: 5654 RVA: 0x000FE6C4 File Offset: 0x000FC8C4
		protected override bool CanUpdate()
		{
			int previousConnectedCount = this.m_previousConnectedCount;
			int playerConnectedCount = BaseNetworkEntityManager.PlayerConnectedCount;
			this.m_previousConnectedCount = previousConnectedCount;
			if (this.m_sentZeroCount || previousConnectedCount <= 0 || playerConnectedCount > 0)
			{
				if (playerConnectedCount > 0)
				{
					this.m_sentZeroCount = false;
				}
				return playerConnectedCount > 0 && this.HostIsValid;
			}
			if (this.HostIsValid)
			{
				this.m_sentZeroCount = true;
				return true;
			}
			return false;
		}

		// Token: 0x06001617 RID: 5655 RVA: 0x000FE720 File Offset: 0x000FC920
		protected override void LogToIndex(SolPerformanceTimerReport fpsReport, BasePerformanceMonitor.NetworkPerformanceReport networkReport)
		{
			this.m_objectArray[0] = LocalZoneManager.ZoneRecord.DisplayName;
			this.m_objectArray[1] = fpsReport.Count;
			this.m_objectArray[2] = fpsReport.Elapsed.TotalSeconds;
			this.m_objectArray[3] = fpsReport.Min;
			this.m_objectArray[4] = fpsReport.Max;
			this.m_objectArray[5] = fpsReport.Avg;
			this.m_objectArray[6] = fpsReport.Std;
			this.m_objectArray[7] = BaseNetworkEntityManager.PlayerConnectedCount;
			this.m_objectArray[8] = networkReport.TimeDelta.TotalSeconds;
			this.m_objectArray[9] = networkReport.PacketsSent;
			this.m_objectArray[10] = networkReport.PacketsReceived;
			this.m_objectArray[11] = networkReport.BytesSent;
			this.m_objectArray[12] = networkReport.BytesReceived;
			SolPerformanceTimerReport solPerformanceTimerReport = ServerGameManager.ServerNetworkEntityManager.PerformanceTimer.GenerateReport();
			this.m_objectArray[13] = solPerformanceTimerReport.Count;
			this.m_objectArray[14] = solPerformanceTimerReport.Elapsed.TotalSeconds;
			this.m_objectArray[15] = solPerformanceTimerReport.Min;
			this.m_objectArray[16] = solPerformanceTimerReport.Max;
			this.m_objectArray[17] = solPerformanceTimerReport.Avg;
			this.m_objectArray[18] = solPerformanceTimerReport.Std;
			SolDebug.LogToIndex(LogLevel.Information, LogIndex.ServerPerformance, "{@Zone} || {@FPS_Count} {@FPS_Elapsed} {@FPS_Min} {@FPS_Max} {@FPS_Avg} {@FPS_Std} || {@NET_Connected} {@NET_Elapsed} {@NET_PacketsSent} {@NET_PacketsReceived} {@NET_BytesSent} {@NET_BytesReceived} || {@NEU_Count} {@NEU_Elapsed} {@NEU_Min} {@NEU_Max} {@NEU_Avg} {@NEU_Std}", this.m_objectArray);
		}

		// Token: 0x04001E09 RID: 7689
		private const string kTemplate = "{@Zone} || {@FPS_Count} {@FPS_Elapsed} {@FPS_Min} {@FPS_Max} {@FPS_Avg} {@FPS_Std} || {@NET_Connected} {@NET_Elapsed} {@NET_PacketsSent} {@NET_PacketsReceived} {@NET_BytesSent} {@NET_BytesReceived} || {@NEU_Count} {@NEU_Elapsed} {@NEU_Min} {@NEU_Max} {@NEU_Avg} {@NEU_Std}";

		// Token: 0x04001E0A RID: 7690
		private readonly object[] m_objectArray = new object[20];

		// Token: 0x04001E0B RID: 7691
		private int m_previousConnectedCount;

		// Token: 0x04001E0C RID: 7692
		private bool m_sentZeroCount;
	}
}
