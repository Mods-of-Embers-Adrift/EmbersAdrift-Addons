using System;
using SoL.Game;
using SoL.Managers;
using SoL.Networking.Managers;
using UnityEngine;

namespace SoL.Utilities.Logging
{
	// Token: 0x02000314 RID: 788
	public class ClientPerformanceMonitor : BasePerformanceMonitor
	{
		// Token: 0x17000545 RID: 1349
		// (get) Token: 0x060015F9 RID: 5625 RVA: 0x00051627 File Offset: 0x0004F827
		protected override float LogCadence
		{
			get
			{
				return 300f;
			}
		}

		// Token: 0x060015FA RID: 5626 RVA: 0x000FE14C File Offset: 0x000FC34C
		protected override bool CanUpdate()
		{
			return this.m_zoneLoaded && this.m_localPlayerInitialized && NetworkManager.MyPeer.IsSet && NetworkManager.MyHost != null && NetworkManager.MyHost.IsSet;
		}

		// Token: 0x060015FB RID: 5627 RVA: 0x000FE18C File Offset: 0x000FC38C
		protected override void LogToIndex(SolPerformanceTimerReport fpsReport, BasePerformanceMonitor.NetworkPerformanceReport networkReport)
		{
			this.m_objectArray[8] = QualitySettings.names[QualitySettings.GetQualityLevel()];
			this.m_objectArray[9] = Screen.width.ToString() + "x" + Screen.height.ToString();
			this.m_objectArray[10] = Screen.fullScreen;
			this.m_objectArray[11] = LocalZoneManager.ZoneRecord.DisplayName;
			this.m_objectArray[12] = fpsReport.Count;
			this.m_objectArray[13] = fpsReport.Elapsed.TotalSeconds;
			this.m_objectArray[14] = fpsReport.Min;
			this.m_objectArray[15] = fpsReport.Max;
			this.m_objectArray[16] = fpsReport.Avg;
			this.m_objectArray[17] = fpsReport.Std;
			this.m_objectArray[18] = networkReport.TimeDelta.TotalSeconds;
			this.m_objectArray[19] = networkReport.PacketsSent;
			this.m_objectArray[20] = networkReport.PacketsReceived;
			this.m_objectArray[21] = networkReport.PacketsLost;
			this.m_objectArray[22] = networkReport.BytesSent;
			this.m_objectArray[23] = networkReport.BytesReceived;
			this.m_objectArray[24] = NetworkManager.MyPeer.RoundTripTime;
			SolDebug.LogToIndex(LogLevel.Information, LogIndex.ClientPerformance, "{@SYS_OS} {@SYS_GraphicsAPI} {@SYS_Processor} {@SYS_ProcessorFrequency} {@SYS_ProcessorCount} {@SYS_SystemMemory} {@SYS_GraphicsCard} {@SYS_GraphicsMemory} {@SYS_Quality} {@SYS_Resolution} {@SYS_FullScreen} || {@Zone} || {@FPS_Count} {@FPS_Elapsed} {@FPS_Min} {@FPS_Max} {@FPS_Avg} {@FPS_Std} || {@NET_Elapsed} {@NET_PacketsSent} {@NET_PacketsReceived} {@NET_PacketsLost} {@NET_BytesSent} {@NET_BytesReceived} {@NET_RoundTripTime}", this.m_objectArray);
		}

		// Token: 0x060015FC RID: 5628 RVA: 0x0005162E File Offset: 0x0004F82E
		protected override void Awake()
		{
			base.Awake();
			SceneCompositionManager.ZoneLoadStarted += this.SceneCompositionManagerOnZoneLoadStarted;
			SceneCompositionManager.ZoneLoaded += this.SceneCompositionManagerOnZoneLoaded;
			LocalPlayer.LocalPlayerInitialized += this.LocalPlayerOnLocalPlayerInitialized;
		}

		// Token: 0x060015FD RID: 5629 RVA: 0x000FE328 File Offset: 0x000FC528
		private void Start()
		{
			this.m_objectArray[0] = SystemInfo.operatingSystem;
			this.m_objectArray[1] = SystemInfo.graphicsDeviceType.ToString();
			this.m_objectArray[2] = SystemInfo.processorType;
			this.m_objectArray[3] = SystemInfo.processorFrequency;
			this.m_objectArray[4] = SystemInfo.processorCount;
			this.m_objectArray[5] = SystemInfo.systemMemorySize;
			this.m_objectArray[6] = SystemInfo.graphicsDeviceName;
			this.m_objectArray[7] = SystemInfo.graphicsMemorySize;
		}

		// Token: 0x060015FE RID: 5630 RVA: 0x00051669 File Offset: 0x0004F869
		private void OnDestroy()
		{
			SceneCompositionManager.ZoneLoadStarted -= this.SceneCompositionManagerOnZoneLoadStarted;
			SceneCompositionManager.ZoneLoaded -= this.SceneCompositionManagerOnZoneLoaded;
			LocalPlayer.LocalPlayerInitialized -= this.LocalPlayerOnLocalPlayerInitialized;
		}

		// Token: 0x060015FF RID: 5631 RVA: 0x0005169E File Offset: 0x0004F89E
		private void SceneCompositionManagerOnZoneLoadStarted(ZoneId obj)
		{
			this.m_zoneLoaded = false;
			this.m_localPlayerInitialized = false;
		}

		// Token: 0x06001600 RID: 5632 RVA: 0x000516AE File Offset: 0x0004F8AE
		private void SceneCompositionManagerOnZoneLoaded(ZoneId obj)
		{
			this.m_zoneLoaded = true;
		}

		// Token: 0x06001601 RID: 5633 RVA: 0x000516B7 File Offset: 0x0004F8B7
		private void LocalPlayerOnLocalPlayerInitialized()
		{
			this.m_localPlayerInitialized = true;
			base.ResetMeasurements();
		}

		// Token: 0x04001DF7 RID: 7671
		private const string kTemplate = "{@SYS_OS} {@SYS_GraphicsAPI} {@SYS_Processor} {@SYS_ProcessorFrequency} {@SYS_ProcessorCount} {@SYS_SystemMemory} {@SYS_GraphicsCard} {@SYS_GraphicsMemory} {@SYS_Quality} {@SYS_Resolution} {@SYS_FullScreen} || {@Zone} || {@FPS_Count} {@FPS_Elapsed} {@FPS_Min} {@FPS_Max} {@FPS_Avg} {@FPS_Std} || {@NET_Elapsed} {@NET_PacketsSent} {@NET_PacketsReceived} {@NET_PacketsLost} {@NET_BytesSent} {@NET_BytesReceived} {@NET_RoundTripTime}";

		// Token: 0x04001DF8 RID: 7672
		private readonly object[] m_objectArray = new object[25];

		// Token: 0x04001DF9 RID: 7673
		private bool m_zoneLoaded;

		// Token: 0x04001DFA RID: 7674
		private bool m_localPlayerInitialized;
	}
}
