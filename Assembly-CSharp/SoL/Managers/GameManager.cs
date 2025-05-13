using System;
using ENet;
using SoL.Game;
using SoL.Game.Settings;
using SoL.Networking.Managers;
using SoL.Networking.SolServer;
using SoL.Utilities;
using SoL.Utilities.Logging;
using UnityEngine;
using UnityEngine.Scripting;

namespace SoL.Managers
{
	// Token: 0x02000528 RID: 1320
	public abstract class GameManager : MonoBehaviour
	{
		// Token: 0x17000830 RID: 2096
		// (get) Token: 0x06002796 RID: 10134 RVA: 0x0005BC7A File Offset: 0x00059E7A
		public static bool IsOnline
		{
			get
			{
				return !GameManager.IsOffline;
			}
		}

		// Token: 0x17000831 RID: 2097
		// (get) Token: 0x06002797 RID: 10135 RVA: 0x0005BC84 File Offset: 0x00059E84
		// (set) Token: 0x06002798 RID: 10136 RVA: 0x0005BC8B File Offset: 0x00059E8B
		public static bool IsServer { get; private set; }

		// Token: 0x17000832 RID: 2098
		// (get) Token: 0x06002799 RID: 10137 RVA: 0x0005BC93 File Offset: 0x00059E93
		// (set) Token: 0x0600279A RID: 10138 RVA: 0x0005BC9A File Offset: 0x00059E9A
		public static SceneCompositionManager SceneCompositionManager { get; protected set; }

		// Token: 0x17000833 RID: 2099
		// (get) Token: 0x0600279B RID: 10139 RVA: 0x0005BCA2 File Offset: 0x00059EA2
		// (set) Token: 0x0600279C RID: 10140 RVA: 0x0005BCA9 File Offset: 0x00059EA9
		public static NetworkManager NetworkManager { get; protected set; }

		// Token: 0x17000834 RID: 2100
		// (get) Token: 0x0600279D RID: 10141 RVA: 0x0005BCB1 File Offset: 0x00059EB1
		// (set) Token: 0x0600279E RID: 10142 RVA: 0x0005BCB8 File Offset: 0x00059EB8
		public static SolServerConnectionManager SolServerConnectionManager { get; protected set; }

		// Token: 0x17000835 RID: 2101
		// (get) Token: 0x0600279F RID: 10143 RVA: 0x0005BCC0 File Offset: 0x00059EC0
		// (set) Token: 0x060027A0 RID: 10144 RVA: 0x0005BCC7 File Offset: 0x00059EC7
		public static QuestManager QuestManager { get; protected set; }

		// Token: 0x17000836 RID: 2102
		// (get) Token: 0x060027A1 RID: 10145
		protected abstract bool m_isServer { get; }

		// Token: 0x17000837 RID: 2103
		// (get) Token: 0x060027A2 RID: 10146 RVA: 0x0005BCCF File Offset: 0x00059ECF
		// (set) Token: 0x060027A3 RID: 10147 RVA: 0x0005BCD6 File Offset: 0x00059ED6
		public static bool AllowAlchemy { get; private set; } = false;

		// Token: 0x060027A4 RID: 10148 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool LoadGameServerConfig()
		{
			return false;
		}

		// Token: 0x060027A5 RID: 10149 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void SendMessageToStatusChannel(string msg)
		{
		}

		// Token: 0x060027A6 RID: 10150 RVA: 0x00138774 File Offset: 0x00136974
		protected virtual void Awake()
		{
			if (GameManager.Instance != null)
			{
				GameManager.Instance.Reset();
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			GameManager.Instance = this;
			Application.logMessageReceived += this.ApplicationOnLogMessageReceived;
			GameManager.IsOffline = false;
			CustomSerialization.Initialize();
			this.m_initialized = Library.Initialize();
			GameManager.IsServer = this.m_isServer;
			SolDebug.Initialize();
			this.InitializeGameManager();
		}

		// Token: 0x060027A7 RID: 10151 RVA: 0x001387E8 File Offset: 0x001369E8
		private void Start()
		{
			GameManager.AllowAlchemy = (GlobalSettings.Values != null && GlobalSettings.Values.Ashen != null && GlobalSettings.Values.Ashen.AllowAlchemy);
			Debug.Log("Allow AA: " + GameManager.AllowAlchemy.ToString());
			if (GlobalSettings.Values != null && GlobalSettings.Values.Configs != null && GlobalSettings.Values.Configs.Data != null)
			{
				bool usingGraphicsJobs = GlobalSettings.Values.Configs.Data.UsingGraphicsJobs;
				bool usingIncrementalGc = GlobalSettings.Values.Configs.Data.UsingIncrementalGc;
				Debug.Log(string.Concat(new string[]
				{
					"Configs.Data:\n  UsingGraphicsJobs: ",
					usingGraphicsJobs.ToString(),
					"\n  UsingIncrementalGC: ",
					usingIncrementalGc.ToString(),
					" (isIncremental: ",
					GarbageCollector.isIncremental.ToString(),
					")"
				}));
			}
		}

		// Token: 0x060027A8 RID: 10152 RVA: 0x0005BCDE File Offset: 0x00059EDE
		protected virtual void OnDestroy()
		{
			if (GameManager.Instance != null && GameManager.Instance == this)
			{
				Application.logMessageReceived -= this.ApplicationOnLogMessageReceived;
			}
			if (this.m_initialized)
			{
				Library.Deinitialize();
			}
		}

		// Token: 0x060027A9 RID: 10153 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void InitializeGameManager()
		{
		}

		// Token: 0x060027AA RID: 10154 RVA: 0x0005BD18 File Offset: 0x00059F18
		public virtual void Reset()
		{
			if (GameManager.SolServerConnectionManager != null)
			{
				GameManager.SolServerConnectionManager.Disconnect();
			}
		}

		// Token: 0x060027AB RID: 10155 RVA: 0x001388F8 File Offset: 0x00136AF8
		private void ApplicationOnLogMessageReceived(string condition, string stacktrace, LogType type)
		{
			if (type == LogType.Error || type == LogType.Exception)
			{
				for (int i = 0; i < GameManager.m_stackTraceBlacklist.Length; i++)
				{
					if (stacktrace.Contains(GameManager.m_stackTraceBlacklist[i]))
					{
						return;
					}
				}
				for (int j = 0; j < GameManager.m_conditionBlacklist.Length; j++)
				{
					if (condition.Contains(GameManager.m_conditionBlacklist[j]))
					{
						return;
					}
				}
				if (!string.Equals(this.m_previousStacktrace, stacktrace))
				{
					string text = GameManager.IsServer ? "SERVER" : "NONE";
					string text2 = GameManager.IsServer ? text : LocalPlayer.GetDebugPositionString();
					string text3 = (SessionData.User != null) ? SessionData.User.Id : text;
					string text4 = (SessionData.SelectedCharacter != null) ? SessionData.SelectedCharacter.Id : text;
					string text5 = (SessionData.SelectedCharacter != null) ? SessionData.SelectedCharacter.Name : text;
					SolDebug.LogToIndex(LogLevel.Error, LogIndex.Error, "{@Server} {@Condition} {@StackTrace} {@LogType} {@UserId} {@CharacterId} {@PlayerName} {@DebugPosition}", new object[]
					{
						GameManager.IsServer,
						condition,
						stacktrace,
						type,
						text3,
						text4,
						text5,
						text2
					});
				}
				this.m_previousStacktrace = stacktrace;
			}
		}

		// Token: 0x060027AC RID: 10156 RVA: 0x0005BD31 File Offset: 0x00059F31
		protected void AddMainThreadDispatcher()
		{
			base.gameObject.AddComponent<MainThreadDispatcher>();
		}

		// Token: 0x04002947 RID: 10567
		public static GameManager Instance = null;

		// Token: 0x04002948 RID: 10568
		public static bool IsOffline = true;

		// Token: 0x0400294E RID: 10574
		private bool m_initialized;

		// Token: 0x04002950 RID: 10576
		private string m_previousStacktrace;

		// Token: 0x04002951 RID: 10577
		private static readonly string[] m_stackTraceBlacklist = new string[]
		{
			"UMA.CharacterSystem.DynamicCharacterAvatar.LoadWhenReady",
			"UnityEngine.Rendering.HighDefinition.HDRenderPipeline.PushLightDataGlobalParams"
		};

		// Token: 0x04002952 RID: 10578
		private static readonly string[] m_conditionBlacklist = new string[]
		{
			"No more space in the 2D Cookie Texture Atlas"
		};
	}
}
