using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Text;
using Ionic.Zlib;
using SoL.Game;
using SoL.Game.Audio.Music;
using SoL.Game.Culling;
using SoL.Game.Player;
using SoL.Game.Pooling;
using SoL.Game.Settings;
using SoL.Game.Trading;
using SoL.Networking.Managers;
using SoL.Networking.SolServer;
using SoL.Utilities.Extensions;
using SoL.Utilities.Logging;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace SoL.Managers
{
	// Token: 0x020004EC RID: 1260
	public class ClientGameManager : GameManager
	{
		// Token: 0x17000714 RID: 1812
		// (get) Token: 0x06002327 RID: 8999 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool m_isServer
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000715 RID: 1813
		// (get) Token: 0x06002328 RID: 9000 RVA: 0x000595C2 File Offset: 0x000577C2
		// (set) Token: 0x06002329 RID: 9001 RVA: 0x000595C9 File Offset: 0x000577C9
		public static UIManager UIManager { get; set; }

		// Token: 0x17000716 RID: 1814
		// (get) Token: 0x0600232A RID: 9002 RVA: 0x000595D1 File Offset: 0x000577D1
		// (set) Token: 0x0600232B RID: 9003 RVA: 0x000595D8 File Offset: 0x000577D8
		public static CombatTextManager CombatTextManager { get; set; }

		// Token: 0x17000717 RID: 1815
		// (get) Token: 0x0600232C RID: 9004 RVA: 0x000595E0 File Offset: 0x000577E0
		// (set) Token: 0x0600232D RID: 9005 RVA: 0x000595E7 File Offset: 0x000577E7
		public static DelayedEventManager DelayedEventManager { get; private set; }

		// Token: 0x17000718 RID: 1816
		// (get) Token: 0x0600232E RID: 9006 RVA: 0x000595EF File Offset: 0x000577EF
		// (set) Token: 0x0600232F RID: 9007 RVA: 0x000595F6 File Offset: 0x000577F6
		public static ClientGroupManager GroupManager { get; private set; }

		// Token: 0x17000719 RID: 1817
		// (get) Token: 0x06002330 RID: 9008 RVA: 0x000595FE File Offset: 0x000577FE
		// (set) Token: 0x06002331 RID: 9009 RVA: 0x00059605 File Offset: 0x00057805
		public static ClientTradeManager TradeManager { get; private set; }

		// Token: 0x1700071A RID: 1818
		// (get) Token: 0x06002332 RID: 9010 RVA: 0x0005960D File Offset: 0x0005780D
		// (set) Token: 0x06002333 RID: 9011 RVA: 0x00059614 File Offset: 0x00057814
		public static CullingManager CullingManager { get; private set; }

		// Token: 0x1700071B RID: 1819
		// (get) Token: 0x06002334 RID: 9012 RVA: 0x0005961C File Offset: 0x0005781C
		// (set) Token: 0x06002335 RID: 9013 RVA: 0x00059623 File Offset: 0x00057823
		public static LoginApiManager LoginApiManager { get; private set; }

		// Token: 0x1700071C RID: 1820
		// (get) Token: 0x06002336 RID: 9014 RVA: 0x0005962B File Offset: 0x0005782B
		// (set) Token: 0x06002337 RID: 9015 RVA: 0x00059632 File Offset: 0x00057832
		public static MusicManager MusicManager { get; set; }

		// Token: 0x1700071D RID: 1821
		// (get) Token: 0x06002338 RID: 9016 RVA: 0x0005963A File Offset: 0x0005783A
		// (set) Token: 0x06002339 RID: 9017 RVA: 0x00059641 File Offset: 0x00057841
		public static IInputManager InputManager { get; set; }

		// Token: 0x1700071E RID: 1822
		// (get) Token: 0x0600233A RID: 9018 RVA: 0x00059649 File Offset: 0x00057849
		// (set) Token: 0x0600233B RID: 9019 RVA: 0x00059650 File Offset: 0x00057850
		public static SocialManager SocialManager { get; private set; }

		// Token: 0x1700071F RID: 1823
		// (get) Token: 0x0600233C RID: 9020 RVA: 0x00059658 File Offset: 0x00057858
		// (set) Token: 0x0600233D RID: 9021 RVA: 0x0005965F File Offset: 0x0005785F
		public static OverheadNameplateManager OverheadNameplateManager { get; private set; }

		// Token: 0x17000720 RID: 1824
		// (get) Token: 0x0600233E RID: 9022 RVA: 0x00059667 File Offset: 0x00057867
		// (set) Token: 0x0600233F RID: 9023 RVA: 0x0005966E File Offset: 0x0005786E
		public static NotificationsManager NotificationsManager { get; private set; }

		// Token: 0x17000721 RID: 1825
		// (get) Token: 0x06002340 RID: 9024 RVA: 0x00059676 File Offset: 0x00057876
		// (set) Token: 0x06002341 RID: 9025 RVA: 0x0005967D File Offset: 0x0005787D
		public static CampManager CampManager { get; private set; }

		// Token: 0x17000722 RID: 1826
		// (get) Token: 0x06002342 RID: 9026 RVA: 0x00059685 File Offset: 0x00057885
		// (set) Token: 0x06002343 RID: 9027 RVA: 0x0005968C File Offset: 0x0005788C
		public static WaterTrailManager WaterTrailManager { get; private set; }

		// Token: 0x17000723 RID: 1827
		// (get) Token: 0x06002344 RID: 9028 RVA: 0x00059694 File Offset: 0x00057894
		// (set) Token: 0x06002345 RID: 9029 RVA: 0x0005969B File Offset: 0x0005789B
		public static SteamManager SteamManager { get; private set; }

		// Token: 0x17000724 RID: 1828
		// (get) Token: 0x06002347 RID: 9031 RVA: 0x000596AB File Offset: 0x000578AB
		// (set) Token: 0x06002346 RID: 9030 RVA: 0x000596A3 File Offset: 0x000578A3
		public static Camera MainCamera
		{
			get
			{
				if (!ClientGameManager.m_mainCamera)
				{
					ClientGameManager.m_mainCamera = Camera.main;
				}
				return ClientGameManager.m_mainCamera;
			}
			set
			{
				ClientGameManager.m_mainCamera = value;
			}
		}

		// Token: 0x06002348 RID: 9032 RVA: 0x00128D68 File Offset: 0x00126F68
		protected override void Awake()
		{
			base.Awake();
			this.CheckPreviousCrash();
			LocalPlayer.SetProcessIsRunningKey("Login");
			UnityEngine.AudioSettings.OnAudioConfigurationChanged += this.AudioSettingsOnOnAudioConfigurationChanged;
			ClientGameManager.SetJobCount();
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				utf16ValueStringBuilder.Append("SYSTEM INFO: ");
				utf16ValueStringBuilder.Append("Processor=" + SystemInfo.processorType + ", ");
				utf16ValueStringBuilder.Append("ProcessorCount=" + SystemInfo.processorCount.ToString() + ", ");
				utf16ValueStringBuilder.Append("Memory=" + SystemInfo.systemMemorySize.ToString() + ", ");
				utf16ValueStringBuilder.Append("Graphics=" + SystemInfo.graphicsDeviceName + ", ");
				utf16ValueStringBuilder.Append("GraphicsMemory=" + SystemInfo.graphicsMemorySize.ToString() + ", ");
				utf16ValueStringBuilder.Append("OperatingSystem=" + SystemInfo.operatingSystem);
				Debug.Log(utf16ValueStringBuilder.ToString());
			}
		}

		// Token: 0x06002349 RID: 9033 RVA: 0x000596C8 File Offset: 0x000578C8
		protected override void OnDestroy()
		{
			base.OnDestroy();
			UnityEngine.AudioSettings.OnAudioConfigurationChanged -= this.AudioSettingsOnOnAudioConfigurationChanged;
		}

		// Token: 0x0600234A RID: 9034 RVA: 0x000596E1 File Offset: 0x000578E1
		private void OnApplicationQuit()
		{
			LocalPlayer.DeleteProcessIsRunningKey();
		}

		// Token: 0x0600234B RID: 9035 RVA: 0x00128E9C File Offset: 0x0012709C
		protected override void InitializeGameManager()
		{
			base.InitializeGameManager();
			GameManager.SceneCompositionManager = base.gameObject.AddComponent<ClientSceneCompositionManager>();
			GameManager.NetworkManager = base.gameObject.AddComponent<ClientNetworkManager>();
			GameManager.SolServerConnectionManager = base.gameObject.AddComponent<SolServerConnectionManager>();
			GameManager.QuestManager = base.gameObject.AddComponent<ClientQuestManager>();
			ClientGameManager.InputManager = base.gameObject.AddComponent<InputManager>();
			ClientGameManager.CullingManager = base.gameObject.AddComponent<CullingManager>();
			ClientGameManager.TradeManager = new ClientTradeManager();
			CursorManager.Initialize();
			ClientGameManager.SocialManager = base.gameObject.AddComponent<SocialManager>();
			ClientGameManager.GroupManager = base.gameObject.AddComponent<ClientGroupManager>();
			ClientGameManager.LoginApiManager = base.gameObject.AddComponent<LoginApiManager>();
			base.gameObject.AddComponent<ClientPerformanceMonitor>();
			base.gameObject.AddComponent<VisualDestructionManager>();
			ClientGameManager.OverheadNameplateManager = base.gameObject.AddComponent<OverheadNameplateManager>();
			ClientGameManager.NotificationsManager = base.gameObject.AddComponent<NotificationsManager>();
			ClientGameManager.CampManager = base.gameObject.AddComponent<CampManager>();
			ClientGameManager.WaterTrailManager = base.gameObject.AddComponent<WaterTrailManager>();
			ClientGameManager.DelayedEventManager = base.gameObject.AddComponent<DelayedEventManager>();
			ClientGameManager.SteamManager = base.gameObject.AddComponent<SteamManager>();
			UnityEngine.Object.Instantiate<GameObject>(this.m_rewiredInputManager, base.gameObject.transform);
			UnityEngine.Object.Destroy(this.m_gmConsole);
		}

		// Token: 0x0600234C RID: 9036 RVA: 0x00128FE8 File Offset: 0x001271E8
		private void AudioSettingsOnOnAudioConfigurationChanged(bool devicewaschanged)
		{
			Debug.Log("Refreshing Audio Mixer Groups");
			List<GameObject> list = new List<GameObject>(100);
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				SceneManager.GetSceneAt(i).GetRootGameObjects(list);
				foreach (GameObject gameObject in list)
				{
					foreach (AudioSource audioSource in gameObject.GetComponentsInChildren<AudioSource>(true))
					{
						if (audioSource)
						{
							audioSource.RefreshMixerGroup();
						}
					}
				}
			}
		}

		// Token: 0x0600234D RID: 9037 RVA: 0x00129094 File Offset: 0x00127294
		public override void Reset()
		{
			base.Reset();
			if (ClientGameManager.UIManager != null)
			{
				ClientGameManager.UIManager.ResetUI();
			}
			if (ClientGameManager.SocialManager != null)
			{
				ClientGameManager.SocialManager.Reset();
			}
			if (ClientGameManager.NotificationsManager != null)
			{
				ClientGameManager.NotificationsManager.Reset();
			}
			ClientGameManager.SteamManager != null;
			LocalPlayer.DeleteProcessIsRunningKey();
		}

		// Token: 0x0600234E RID: 9038 RVA: 0x0004475B File Offset: 0x0004295B
		public static void SetJobCount()
		{
		}

		// Token: 0x0600234F RID: 9039 RVA: 0x00129100 File Offset: 0x00127300
		private void CheckPreviousCrash()
		{
			string fullName = Directory.GetParent(Application.dataPath).FullName;
			string text = string.Format("{0}{1}{2}", fullName, Path.DirectorySeparatorChar, "embersadrift.prev.log");
			string processIsRunningValue = LocalPlayer.GetProcessIsRunningValue();
			Debug.Log("PreviousDescription: " + processIsRunningValue + ", PreviousLogFile: " + text);
			if (!string.IsNullOrEmpty(processIsRunningValue) && !string.IsNullOrEmpty(text) && File.Exists(text))
			{
				string text2 = string.Empty;
				if (GlobalSettings.Values != null && GlobalSettings.Values.Configs != null && GlobalSettings.Values.Configs.Data != null)
				{
					text2 = GlobalSettings.Values.Configs.Data.DeploymentBranch;
				}
				if (!string.IsNullOrEmpty(text2))
				{
					string url = "http://38.140.214.158:14123/upload_crashlog/" + text2.ToLower();
					base.StartCoroutine(this.UploadFileCo(text, url));
				}
			}
		}

		// Token: 0x06002350 RID: 9040 RVA: 0x000596E8 File Offset: 0x000578E8
		private IEnumerator UploadFileCo(string logFilePath, string url)
		{
			yield return null;
			if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(logFilePath) && File.Exists(logFilePath))
			{
				byte[] bytes = null;
				try
				{
					bytes = File.ReadAllBytes(logFilePath);
				}
				catch (Exception arg)
				{
					Debug.LogWarning(string.Format("UploadPreviousLog read failed! {0}", arg));
					yield break;
				}
				yield return null;
				bytes = GZipStream.CompressBuffer(bytes);
				string fileName = DateTime.UtcNow.ToString("MMddyyHHmmss") + "_crashlog.zip";
				WWWForm wwwform = new WWWForm();
				wwwform.AddBinaryData("file", bytes, fileName);
				using (UnityWebRequest www = UnityWebRequest.Post(url, wwwform))
				{
					yield return www.SendWebRequest();
					if (www.IsWebError())
					{
						Debug.LogWarning("Error uploading previous log! " + www.error);
					}
				}
				UnityWebRequest www = null;
				bytes = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x040026D4 RID: 9940
		[SerializeField]
		private GameObject m_gmConsole;

		// Token: 0x040026D5 RID: 9941
		[SerializeField]
		private GameObject m_rewiredInputManager;

		// Token: 0x040026E5 RID: 9957
		private static Camera m_mainCamera;
	}
}
