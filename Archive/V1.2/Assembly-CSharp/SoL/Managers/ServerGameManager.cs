using System;
using System.Collections;
using AwesomeTechnologies;
using Newtonsoft.Json;
using SoL.Game.AuctionHouse;
using SoL.Game.Dueling;
using SoL.Game.Grouping;
using SoL.Game.Settings;
using SoL.Game.Spawning;
using SoL.Game.Trading;
using SoL.Networking;
using SoL.Networking.Managers;
using SoL.Utilities.Logging;
using SoL.Utilities.RuntimeMigration;
using UnityEngine;

namespace SoL.Managers
{
	// Token: 0x02000540 RID: 1344
	public class ServerGameManager : GameManager
	{
		// Token: 0x17000860 RID: 2144
		// (get) Token: 0x060028BC RID: 10428 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool m_isServer
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000861 RID: 2145
		// (get) Token: 0x060028BD RID: 10429 RVA: 0x0005C426 File Offset: 0x0005A626
		// (set) Token: 0x060028BE RID: 10430 RVA: 0x0005C42D File Offset: 0x0005A62D
		public static ServerTradeManager TradeManager { get; private set; }

		// Token: 0x17000862 RID: 2146
		// (get) Token: 0x060028BF RID: 10431 RVA: 0x0005C435 File Offset: 0x0005A635
		// (set) Token: 0x060028C0 RID: 10432 RVA: 0x0005C43C File Offset: 0x0005A63C
		public static ServerDuelManager DuelManager { get; private set; }

		// Token: 0x17000863 RID: 2147
		// (get) Token: 0x060028C1 RID: 10433 RVA: 0x0005C444 File Offset: 0x0005A644
		// (set) Token: 0x060028C2 RID: 10434 RVA: 0x0005C44B File Offset: 0x0005A64B
		public static ServerNetworkEntityManager ServerNetworkEntityManager { get; set; }

		// Token: 0x17000864 RID: 2148
		// (get) Token: 0x060028C3 RID: 10435 RVA: 0x0005C453 File Offset: 0x0005A653
		// (set) Token: 0x060028C4 RID: 10436 RVA: 0x0005C45A File Offset: 0x0005A65A
		public static SpatialManager SpatialManager { get; set; }

		// Token: 0x17000865 RID: 2149
		// (get) Token: 0x060028C5 RID: 10437 RVA: 0x0005C462 File Offset: 0x0005A662
		// (set) Token: 0x060028C6 RID: 10438 RVA: 0x0005C469 File Offset: 0x0005A669
		public static ServerGroupManager GroupManager { get; set; }

		// Token: 0x17000866 RID: 2150
		// (get) Token: 0x060028C7 RID: 10439 RVA: 0x0005C471 File Offset: 0x0005A671
		// (set) Token: 0x060028C8 RID: 10440 RVA: 0x0005C478 File Offset: 0x0005A678
		public static NpcBehaviorManager NpcBehaviorManager { get; set; }

		// Token: 0x17000867 RID: 2151
		// (get) Token: 0x060028C9 RID: 10441 RVA: 0x0005C480 File Offset: 0x0005A680
		// (set) Token: 0x060028CA RID: 10442 RVA: 0x0005C487 File Offset: 0x0005A687
		public static NpcTargetManager NpcTargetManager { get; set; }

		// Token: 0x17000868 RID: 2152
		// (get) Token: 0x060028CB RID: 10443 RVA: 0x0005C48F File Offset: 0x0005A68F
		// (set) Token: 0x060028CC RID: 10444 RVA: 0x0005C496 File Offset: 0x0005A696
		public static LoginConfig LoginConfig { get; private set; }

		// Token: 0x17000869 RID: 2153
		// (get) Token: 0x060028CD RID: 10445 RVA: 0x0005C49E File Offset: 0x0005A69E
		// (set) Token: 0x060028CE RID: 10446 RVA: 0x0005C4A5 File Offset: 0x0005A6A5
		public static GameServerConfig GameServerConfig { get; private set; }

		// Token: 0x1700086A RID: 2154
		// (get) Token: 0x060028CF RID: 10447 RVA: 0x0005C4AD File Offset: 0x0005A6AD
		// (set) Token: 0x060028D0 RID: 10448 RVA: 0x0005C4B4 File Offset: 0x0005A6B4
		public static ProximityConfig PlayerProximityConfig { get; private set; }

		// Token: 0x1700086B RID: 2155
		// (get) Token: 0x060028D1 RID: 10449 RVA: 0x0005C4BC File Offset: 0x0005A6BC
		// (set) Token: 0x060028D2 RID: 10450 RVA: 0x0005C4C3 File Offset: 0x0005A6C3
		public static MailConfig MailConfig { get; private set; }

		// Token: 0x1700086C RID: 2156
		// (get) Token: 0x060028D3 RID: 10451 RVA: 0x0005C4CB File Offset: 0x0005A6CB
		// (set) Token: 0x060028D4 RID: 10452 RVA: 0x0005C4D2 File Offset: 0x0005A6D2
		public static LootRollManager LootRollManager { get; private set; }

		// Token: 0x1700086D RID: 2157
		// (get) Token: 0x060028D5 RID: 10453 RVA: 0x0005C4DA File Offset: 0x0005A6DA
		// (set) Token: 0x060028D6 RID: 10454 RVA: 0x0005C4E1 File Offset: 0x0005A6E1
		public static ServerAuctionHouseManager AuctionHouseManager { get; set; }

		// Token: 0x1700086E RID: 2158
		// (get) Token: 0x060028D7 RID: 10455 RVA: 0x0005C4E9 File Offset: 0x0005A6E9
		// (set) Token: 0x060028D8 RID: 10456 RVA: 0x0005C4F0 File Offset: 0x0005A6F0
		public static RemoteSpawnableNpcs RemoteSpawnableNpcs { get; private set; }

		// Token: 0x1700086F RID: 2159
		// (get) Token: 0x060028D9 RID: 10457 RVA: 0x0005C4F8 File Offset: 0x0005A6F8
		// (set) Token: 0x060028DA RID: 10458 RVA: 0x0005C4FF File Offset: 0x0005A6FF
		public static RemoteSpawnableNodes RemoteSpawnableNodes { get; private set; }

		// Token: 0x060028DB RID: 10459 RVA: 0x0005C507 File Offset: 0x0005A707
		protected override void OnDestroy()
		{
			base.OnDestroy();
			Application.targetFrameRate = -1;
		}

		// Token: 0x060028DC RID: 10460 RVA: 0x0013D71C File Offset: 0x0013B91C
		protected override void InitializeGameManager()
		{
			base.InitializeGameManager();
			if (this.RuntimeMigration())
			{
				Application.Quit();
				return;
			}
			StormhavenExtensions.IsServer = true;
			ServerGameManager.RemoteSpawnableNpcs = this.m_remoteSpawnableNpcs;
			ServerGameManager.RemoteSpawnableNodes = this.m_remoteSpawnableNodes;
			this.LoadGameServerConfig();
			ServerGameManager.LoginConfig = LoginConfig.GetConfigFromDB();
			ServerGameManager.PlayerProximityConfig = ProximityConfig.GetFromDB();
			ServerGameManager.MailConfig = MailConfig.GetConfigFromDB();
			GameManager.SceneCompositionManager = base.gameObject.AddComponent<ServerSceneCompositionManager>();
			GameManager.NetworkManager = base.gameObject.AddComponent<ServerNetworkManager>();
			GameManager.QuestManager = base.gameObject.AddComponent<ServerQuestManager>();
			ServerGameManager.TradeManager = base.gameObject.AddComponent<ServerTradeManager>();
			ServerGameManager.DuelManager = base.gameObject.AddComponent<ServerDuelManager>();
			ServerGameManager.GroupManager = base.gameObject.AddComponent<ServerGroupManager>();
			ServerGameManager.SpatialManager = base.gameObject.AddComponent<SpatialManager>();
			ServerGameManager.NpcBehaviorManager = base.gameObject.AddComponent<NpcBehaviorManager>();
			ServerGameManager.NpcTargetManager = base.gameObject.AddComponent<NpcTargetManager>();
			ServerGameManager.LootRollManager = base.gameObject.AddComponent<LootRollManager>();
			base.gameObject.AddComponent<ServerPerformanceMonitor>();
			base.gameObject.AddComponent<ServerMemoryMonitor>();
			Debug.Log("Enabling Framerate Limiter.  Min: " + ServerGameManager.GameServerConfig.MinFramerate.ToString() + ", Max: " + ServerGameManager.GameServerConfig.MaxFramerate.ToString());
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = ServerGameManager.GameServerConfig.MinFramerate;
			if (GlobalSettings.Values != null)
			{
				if (ServerGameManager.PlayerProximityConfig != null)
				{
					GlobalSettings.Values.Player.PlayersUseProximity = ServerGameManager.PlayerProximityConfig.PlayersUseProximity;
					GlobalSettings.Values.Player.NpcsUseProximity = ServerGameManager.PlayerProximityConfig.NpcsUseProximity;
					Debug.Log("Initializing PlayerProximity with: " + JsonConvert.SerializeObject(ServerGameManager.PlayerProximityConfig));
				}
				if (GlobalSettings.Values.Configs.Data != null)
				{
					Debug.Log("Starting Server: " + GlobalSettings.Values.Configs.Data.GetDataString());
					Debug.Log("Server API Version: " + GlobalSettings.Values.Configs.Data.ServerApiVersion);
				}
			}
			base.AddMainThreadDispatcher();
		}

		// Token: 0x060028DD RID: 10461 RVA: 0x0013D944 File Offset: 0x0013BB44
		public override bool LoadGameServerConfig()
		{
			GameServerConfig configFromDB = GameServerConfig.GetConfigFromDB();
			if (configFromDB == null)
			{
				Debug.LogError("Unable to load GameServerConfig!");
				return false;
			}
			ServerGameManager.GameServerConfig = configFromDB;
			Debug.Log(ServerGameManager.GameServerConfig.GetStartupString());
			if (ServerGameManager.GameServerConfig.MemoryLeakSettings != null)
			{
				ServerGameManager.GameServerConfig.MemoryLeakSettings.Init();
				Debug.Log(ServerGameManager.GameServerConfig.MemoryLeakSettings.GetStartupString());
			}
			if (GlobalSettings.Values != null)
			{
				if (ServerGameManager.GameServerConfig.AdventuringLevelCurve != null)
				{
					GlobalSettings.Values.Progression.AdventuringLevelCurve = ServerGameManager.GameServerConfig.AdventuringLevelCurve;
					Debug.Log("[LEVEL CURVE] ADVENTURING:\n" + GlobalSettings.Values.Progression.AdventuringLevelCurve.ToString());
				}
				if (ServerGameManager.GameServerConfig.GatheringLevelCurve != null)
				{
					GlobalSettings.Values.Progression.GatheringLevelCurve = ServerGameManager.GameServerConfig.GatheringLevelCurve;
					Debug.Log("[LEVEL CURVE] GATHERING:\n" + GlobalSettings.Values.Progression.GatheringLevelCurve.ToString());
				}
				if (ServerGameManager.GameServerConfig.CraftingLevelCurve != null)
				{
					GlobalSettings.Values.Progression.CraftingLevelCurve = ServerGameManager.GameServerConfig.CraftingLevelCurve;
					Debug.Log("[LEVEL CURVE] CRAFTING:\n" + GlobalSettings.Values.Progression.CraftingLevelCurve.ToString());
				}
			}
			return true;
		}

		// Token: 0x060028DE RID: 10462 RVA: 0x0013DA94 File Offset: 0x0013BC94
		private bool RuntimeMigration()
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				if (i < commandLineArgs.Length - 1 && commandLineArgs[i].Equals("-runtimemigration", StringComparison.InvariantCultureIgnoreCase) && commandLineArgs[i + 1].Equals("MigrateStorageColumnCount", StringComparison.InvariantCultureIgnoreCase))
				{
					Debug.Log("Executing SoL.Utilities.RuntimeMigration.StorageMigration.MigrateStorageColumnCount");
					StorageMigration.MigrateStorageColumnCount();
					return true;
				}
			}
			return false;
		}

		// Token: 0x060028DF RID: 10463 RVA: 0x0004475B File Offset: 0x0004295B
		public override void SendMessageToStatusChannel(string msg)
		{
		}

		// Token: 0x060028E0 RID: 10464 RVA: 0x0005C515 File Offset: 0x0005A715
		private IEnumerator SendMessageToStatusChannelInternal(string msg)
		{
			yield return null;
			yield break;
		}

		// Token: 0x040029F8 RID: 10744
		private const string kRemoteSpawnables = "Remote Spawnables";

		// Token: 0x040029FB RID: 10747
		[SerializeField]
		private RemoteSpawnableNpcs m_remoteSpawnableNpcs;

		// Token: 0x040029FC RID: 10748
		[SerializeField]
		private RemoteSpawnableNodes m_remoteSpawnableNodes;

		// Token: 0x02000541 RID: 1345
		[Serializable]
		private struct SlackMessage
		{
			// Token: 0x040029FD RID: 10749
			public string text;
		}
	}
}
