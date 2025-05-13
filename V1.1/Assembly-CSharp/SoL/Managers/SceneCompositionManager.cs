using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game;
using SoL.Game.Messages;
using SoL.Game.Scenes;
using SoL.Game.Settings;
using SoL.Networking.Managers;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace SoL.Managers
{
	// Token: 0x0200052D RID: 1325
	public abstract class SceneCompositionManager : MonoBehaviour
	{
		// Token: 0x1400007E RID: 126
		// (add) Token: 0x06002822 RID: 10274 RVA: 0x0013AEC8 File Offset: 0x001390C8
		// (remove) Token: 0x06002823 RID: 10275 RVA: 0x0013AEFC File Offset: 0x001390FC
		public static event Action SceneCompositionLoaded;

		// Token: 0x1400007F RID: 127
		// (add) Token: 0x06002824 RID: 10276 RVA: 0x0013AF30 File Offset: 0x00139130
		// (remove) Token: 0x06002825 RID: 10277 RVA: 0x0013AF64 File Offset: 0x00139164
		public static event Action<ZoneId> ZoneLoaded;

		// Token: 0x14000080 RID: 128
		// (add) Token: 0x06002826 RID: 10278 RVA: 0x0013AF98 File Offset: 0x00139198
		// (remove) Token: 0x06002827 RID: 10279 RVA: 0x0013AFCC File Offset: 0x001391CC
		public static event Action<ZoneId> ZoneLoadStarted;

		// Token: 0x14000081 RID: 129
		// (add) Token: 0x06002828 RID: 10280 RVA: 0x0013B000 File Offset: 0x00139200
		// (remove) Token: 0x06002829 RID: 10281 RVA: 0x0013B034 File Offset: 0x00139234
		public static event Action LoadingStartupScene;

		// Token: 0x17000841 RID: 2113
		// (get) Token: 0x0600282A RID: 10282 RVA: 0x0005C04F File Offset: 0x0005A24F
		public static bool IsLoading
		{
			get
			{
				return SceneCompositionManager.m_isLoading;
			}
		}

		// Token: 0x0600282B RID: 10283 RVA: 0x0005C056 File Offset: 0x0005A256
		public static void InvokeSceneCompositionLoaded()
		{
			Action sceneCompositionLoaded = SceneCompositionManager.SceneCompositionLoaded;
			if (sceneCompositionLoaded == null)
			{
				return;
			}
			sceneCompositionLoaded();
		}

		// Token: 0x0600282C RID: 10284 RVA: 0x0005C067 File Offset: 0x0005A267
		private Scene GetSceneFromSceneReference(SceneReference reference)
		{
			return this.GetSceneByPath(reference.ScenePath);
		}

		// Token: 0x0600282D RID: 10285 RVA: 0x0005C075 File Offset: 0x0005A275
		private Scene GetSceneByPath(string scenePath)
		{
			return SceneManager.GetSceneByPath(scenePath);
		}

		// Token: 0x17000842 RID: 2114
		// (get) Token: 0x0600282E RID: 10286
		protected abstract SceneReference m_managerSceneReference { get; }

		// Token: 0x17000843 RID: 2115
		// (get) Token: 0x0600282F RID: 10287
		protected abstract SceneInclusionFlags m_requiredFlag { get; }

		// Token: 0x06002830 RID: 10288 RVA: 0x0005C07D File Offset: 0x0005A27D
		private void Awake()
		{
			this.m_sceneConfiguration = GlobalSettings.Values.Configs.GetSceneConfiguration(false);
		}

		// Token: 0x06002831 RID: 10289 RVA: 0x0005C095 File Offset: 0x0005A295
		protected virtual void Start()
		{
			this.m_managerScene = this.GetSceneFromSceneReference(this.m_managerSceneReference);
			this.ToggleLoadingScreenIgnoreEvents(true);
			this.LoadStartupScene();
			this.ToggleLoadingScreenIgnoreEvents(false);
		}

		// Token: 0x06002832 RID: 10290 RVA: 0x0005C0BD File Offset: 0x0005A2BD
		protected virtual void LoadStartupScene()
		{
			Action loadingStartupScene = SceneCompositionManager.LoadingStartupScene;
			if (loadingStartupScene == null)
			{
				return;
			}
			loadingStartupScene();
		}

		// Token: 0x06002833 RID: 10291 RVA: 0x0005C0BD File Offset: 0x0005A2BD
		public virtual void ReloadStartupScene()
		{
			Action loadingStartupScene = SceneCompositionManager.LoadingStartupScene;
			if (loadingStartupScene == null)
			{
				return;
			}
			loadingStartupScene();
		}

		// Token: 0x06002834 RID: 10292 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void SetLoadStatus(string message)
		{
		}

		// Token: 0x06002835 RID: 10293 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void SetLoadPercent(float percent)
		{
		}

		// Token: 0x06002836 RID: 10294 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void SetDestination(ZoneId zoneId)
		{
		}

		// Token: 0x06002837 RID: 10295 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void SetDestinationText(string txt)
		{
		}

		// Token: 0x06002838 RID: 10296 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void SetLoadingImage(Sprite img)
		{
		}

		// Token: 0x06002839 RID: 10297 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void SetLoadingTip(string tip)
		{
		}

		// Token: 0x0600283A RID: 10298 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void ToggleEllipsis(bool show)
		{
		}

		// Token: 0x0600283B RID: 10299 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void ToggleLoadingScreenIgnoreEvents(bool ignore)
		{
		}

		// Token: 0x0600283C RID: 10300 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void ZoneLoadBegin()
		{
		}

		// Token: 0x0600283D RID: 10301 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool BypassLoadUnload(ISceneComposition composition, SceneReference sceneRef)
		{
			return false;
		}

		// Token: 0x0600283E RID: 10302 RVA: 0x0005C056 File Offset: 0x0005A256
		protected virtual void ZoneLoadComplete()
		{
			Action sceneCompositionLoaded = SceneCompositionManager.SceneCompositionLoaded;
			if (sceneCompositionLoaded == null)
			{
				return;
			}
			sceneCompositionLoaded();
		}

		// Token: 0x0600283F RID: 10303 RVA: 0x0005C0CE File Offset: 0x0005A2CE
		public ISceneComposition GetSceneComposition(ZoneId zoneId)
		{
			return this.m_sceneConfiguration.GetZone(zoneId);
		}

		// Token: 0x06002840 RID: 10304 RVA: 0x0005C0DC File Offset: 0x0005A2DC
		protected virtual IEnumerator RefreshSessionKey()
		{
			yield break;
		}

		// Token: 0x06002841 RID: 10305 RVA: 0x0013B068 File Offset: 0x00139268
		public void LoadZoneId(ZoneId zoneId)
		{
			ISceneComposition zone = this.m_sceneConfiguration.GetZone(zoneId);
			if (zone != null)
			{
				this.ZoneLoadBegin();
				Action<ZoneId> zoneLoadStarted = SceneCompositionManager.ZoneLoadStarted;
				if (zoneLoadStarted != null)
				{
					zoneLoadStarted(zoneId);
				}
				this.m_loadRoutine = this.LoadCompositionAsync(zone);
				base.StartCoroutine(this.m_loadRoutine);
				return;
			}
			Debug.LogError(string.Format("Could not find SceneComposition for ZoneId: {0}", zoneId));
		}

		// Token: 0x06002842 RID: 10306 RVA: 0x0013B0CC File Offset: 0x001392CC
		private void SceneLoadUnloadStatus(string msg, int current, int total)
		{
			string loadStatus = ZString.Format<string, int, int>("{0} <size=80%>({1}/{2})</size>", msg, current, total);
			this.SetLoadStatus(loadStatus);
		}

		// Token: 0x06002843 RID: 10307 RVA: 0x0013B0F0 File Offset: 0x001392F0
		private void SetSceneLoadPercent(float totalScenesLoaded)
		{
			float t = totalScenesLoaded / (float)this.m_scenesReferencesToLoad.Count;
			this.SetLoadPercent(Mathf.Lerp(0f, 0.9f, t));
		}

		// Token: 0x06002844 RID: 10308 RVA: 0x0005C0E4 File Offset: 0x0005A2E4
		protected IEnumerator LoadCompositionAsync(ISceneComposition composition)
		{
			SceneCompositionManager.m_isLoading = true;
			Debug.Log("Setting BackgroundLoadingPriority to High");
			ThreadPriority prevBackgroundLoadingPriority = Application.backgroundLoadingPriority;
			Application.backgroundLoadingPriority = ThreadPriority.High;
			string finalLoadStatus = string.Empty;
			if (composition.HasZoneSettings)
			{
				this.SetDestination(composition.ZoneId);
				this.SetLoadingImage(composition.GetRandomLoadingImage());
				this.SetLoadingTip(composition.GetLoadingTip());
				finalLoadStatus = "Connecting";
			}
			else
			{
				this.SetDestinationText(string.Empty);
				this.SetLoadingImage(null);
				this.SetLoadingTip(string.Empty);
			}
			this.m_currentLoadOp = null;
			this.m_currentScenePath = string.Empty;
			this.m_scenesReferencesToLoad.Clear();
			this.SetLoadStatus("Unloading");
			this.ToggleEllipsis(true);
			this.SetLoadPercent(0f);
			if (!GameManager.IsServer)
			{
				yield return new WaitForSeconds(0.5f);
			}
			this.m_timer.Start();
			yield return this.UnloadPreviousCompositionAsync();
			this.m_timer.StopAndPrintElapsed("UnloadPreviousComposition");
			this.m_timer.Start();
			yield return Resources.UnloadUnusedAssets();
			yield return this.RefreshSessionKey();
			this.m_timer.StopAndPrintElapsed("UnloadUnusedAssets");
			if (composition.HasZoneSettings)
			{
				this.SetLoadStatus("Loading");
			}
			foreach (SceneReference sceneReference in composition.GetAdditionalScenes(this.m_requiredFlag, true))
			{
				if (!this.BypassLoadUnload(composition, sceneReference))
				{
					this.m_scenesReferencesToLoad.Add(sceneReference);
				}
			}
			this.m_scenesReferencesToLoad.Add(composition.ActiveScene);
			foreach (SceneReference sceneReference2 in composition.GetAdditionalScenes(this.m_requiredFlag, false))
			{
				if (!this.BypassLoadUnload(composition, sceneReference2))
				{
					this.m_scenesReferencesToLoad.Add(sceneReference2);
				}
			}
			this.m_timer.Start();
			int num;
			for (int i = 0; i < this.m_scenesReferencesToLoad.Count; i = num + 1)
			{
				yield return this.AssignCurrentLoadOp(this.m_scenesReferencesToLoad[i]);
				if (this.m_currentLoadOp != null)
				{
					SceneLoadOp currentLoadOp = this.m_currentLoadOp.Value;
					this.SceneLoadUnloadStatus("Loading", i + 1, this.m_scenesReferencesToLoad.Count);
					this.SetSceneLoadPercent((float)i);
					while (!currentLoadOp.IsDone())
					{
						yield return null;
						float progress = currentLoadOp.GetProgress();
						this.SetSceneLoadPercent((float)i + progress);
						if (!currentLoadOp.Activated && progress >= 0.9f)
						{
							currentLoadOp.AllowActivation();
						}
						yield return this.RefreshSessionKey();
					}
					this.SetSceneLoadPercent((float)(i + 1));
					if (currentLoadOp.Type == SceneLoadOpType.Addressable)
					{
						this.m_addressableScenes.Add(currentLoadOp.AddressableOp.Result);
					}
					currentLoadOp = default(SceneLoadOp);
				}
				num = i;
			}
			this.m_timer.StopAndPrintElapsed("LoadSceneReferences");
			yield return this.AssignCurrentScenePath(composition.ActiveScene);
			Scene sceneByPath = this.GetSceneByPath(this.m_currentScenePath);
			Debug.Log("Activating: " + this.m_currentScenePath);
			SceneManager.SetActiveScene(sceneByPath);
			this.m_currentComposition = composition;
			this.m_loadRoutine = null;
			if (!GameManager.IsServer)
			{
				this.SetLoadStatus(ZString.Format<string>("{0}...", composition.GetSceneLoadCompleteMessage()));
				this.m_timer.Start();
				yield return Resources.UnloadUnusedAssets();
				yield return this.RefreshSessionKey();
				this.m_timer.StopAndPrintElapsed("UnloadUnusedAssets");
			}
			this.SetLoadStatus(finalLoadStatus);
			Debug.Log("Resetting BackgroundLoadingPriority to " + prevBackgroundLoadingPriority.ToString());
			Application.backgroundLoadingPriority = prevBackgroundLoadingPriority;
			SceneCompositionManager.m_isLoading = false;
			if (composition.HasZoneSettings)
			{
				Action<ZoneId> zoneLoaded = SceneCompositionManager.ZoneLoaded;
				if (zoneLoaded != null)
				{
					zoneLoaded(composition.ZoneId);
				}
			}
			this.ZoneLoadComplete();
			yield break;
		}

		// Token: 0x06002845 RID: 10309 RVA: 0x0005C0FA File Offset: 0x0005A2FA
		private IEnumerator AssignCurrentScenePath(SceneReference sceneReference)
		{
			string currentScenePath = sceneReference.ScenePath;
			if (sceneReference.IsAddressable())
			{
				AsyncOperationHandle<IList<IResourceLocation>> loc = Addressables.LoadResourceLocationsAsync(sceneReference.AddressableReference, null);
				yield return loc;
				currentScenePath = loc.Result[0].InternalId;
				loc = default(AsyncOperationHandle<IList<IResourceLocation>>);
			}
			this.m_currentScenePath = currentScenePath;
			yield break;
		}

		// Token: 0x06002846 RID: 10310 RVA: 0x0005C110 File Offset: 0x0005A310
		private IEnumerator AssignCurrentLoadOp(SceneReference sceneReference)
		{
			this.m_currentLoadOp = null;
			this.m_currentScenePath = string.Empty;
			yield return this.AssignCurrentScenePath(sceneReference);
			if (!this.GetSceneByPath(this.m_currentScenePath).isLoaded)
			{
				string str = sceneReference.IsAddressable() ? "(Addressable)" : "(Standard)";
				Debug.Log("Loading " + this.m_currentScenePath + " " + str);
				this.m_currentLoadOp = new SceneLoadOp?(new SceneLoadOp(sceneReference));
			}
			yield break;
		}

		// Token: 0x06002847 RID: 10311 RVA: 0x0005C126 File Offset: 0x0005A326
		protected IEnumerator UnloadPreviousCompositionAsync()
		{
			if (this.m_currentComposition == null)
			{
				yield break;
			}
			Debug.Log("Activating scene for unload: " + this.m_managerScene.path);
			SceneManager.SetActiveScene(this.m_managerScene);
			ISceneComposition currentComposition = this.m_currentComposition;
			this.m_scenesReferencesToUnload.Clear();
			foreach (SceneReference sceneReference in currentComposition.GetAdditionalScenes(this.m_requiredFlag, false))
			{
				if (!this.BypassLoadUnload(currentComposition, sceneReference) && !sceneReference.IsAddressable())
				{
					this.m_scenesReferencesToUnload.Add(sceneReference);
				}
			}
			this.m_scenesReferencesToUnload.Add(currentComposition.ActiveScene);
			foreach (SceneReference sceneReference2 in currentComposition.GetAdditionalScenes(this.m_requiredFlag, true))
			{
				if (!this.BypassLoadUnload(currentComposition, sceneReference2) && !sceneReference2.IsAddressable())
				{
					this.m_scenesReferencesToUnload.Add(sceneReference2);
				}
			}
			int sceneCount = this.m_scenesReferencesToUnload.Count + this.m_addressableScenes.Count;
			int num;
			for (int i = 0; i < this.m_scenesReferencesToUnload.Count; i = num + 1)
			{
				Debug.Log("Unloading " + this.m_scenesReferencesToUnload[i].ScenePath + " (Standard)");
				this.SceneLoadUnloadStatus("Unloading", i + 1, sceneCount);
				AsyncOperation unloadOp = this.GetUnloadSceneOp(this.m_scenesReferencesToUnload[i]);
				while (unloadOp != null && unloadOp.progress < 1f)
				{
					yield return null;
					float loadPercent = ((float)i + unloadOp.progress) / (float)sceneCount;
					this.SetLoadPercent(loadPercent);
					yield return this.RefreshSessionKey();
				}
				unloadOp = null;
				num = i;
			}
			for (int i = 0; i < this.m_addressableScenes.Count; i = num + 1)
			{
				if (this.m_addressableScenes[i].Scene.isLoaded)
				{
					Debug.Log("Unloading " + this.m_addressableScenes[i].Scene.path + " (Addressable)");
					this.SceneLoadUnloadStatus("Unloading", i + 1 + this.m_scenesReferencesToUnload.Count, sceneCount);
					AsyncOperationHandle<SceneInstance> unloadOp2 = Addressables.UnloadSceneAsync(this.m_addressableScenes[i], true);
					while (!unloadOp2.IsDone)
					{
						yield return null;
						float loadPercent2 = ((float)(this.m_scenesReferencesToUnload.Count + i) + unloadOp2.PercentComplete) / (float)sceneCount;
						this.SetLoadPercent(loadPercent2);
						yield return this.RefreshSessionKey();
					}
					unloadOp2 = default(AsyncOperationHandle<SceneInstance>);
				}
				num = i;
			}
			this.m_addressableScenes.Clear();
			this.m_scenesReferencesToUnload.Clear();
			this.m_currentComposition = null;
			yield break;
		}

		// Token: 0x06002848 RID: 10312 RVA: 0x0005C135 File Offset: 0x0005A335
		protected IEnumerator LoadSingleScene(SceneReference sceneReference)
		{
			if (!this.GetSceneFromSceneReference(sceneReference).isLoaded)
			{
				string str = sceneReference.IsAddressable() ? "(Addressable)" : "(Standard)";
				Debug.Log("Loading " + sceneReference.ScenePath + " " + str);
				SceneLoadOp currentLoadOp = new SceneLoadOp(sceneReference);
				while (!currentLoadOp.IsDone())
				{
					yield return null;
					float progress = currentLoadOp.GetProgress();
					if (!currentLoadOp.Activated && progress >= 0.9f)
					{
						currentLoadOp.AllowActivation();
					}
					yield return this.RefreshSessionKey();
				}
				currentLoadOp = default(SceneLoadOp);
			}
			yield break;
		}

		// Token: 0x06002849 RID: 10313 RVA: 0x0005C14B File Offset: 0x0005A34B
		protected IEnumerator UnloadSingleScene(SceneReference sceneReference, bool unloadUnused)
		{
			if (sceneReference.IsAddressable())
			{
				int num;
				for (int i = 0; i < this.m_addressableScenes.Count; i = num + 1)
				{
					if (this.m_addressableScenes[i].Scene.isLoaded && sceneReference.ScenePath == this.m_addressableScenes[i].Scene.path)
					{
						Debug.Log("Unloading " + this.m_addressableScenes[i].Scene.path + " (Addressable)");
						while (!Addressables.UnloadSceneAsync(this.m_addressableScenes[i], true).IsDone)
						{
							yield return null;
							yield return this.RefreshSessionKey();
						}
						this.m_addressableScenes.RemoveAt(i);
						break;
					}
					num = i;
				}
			}
			else
			{
				Debug.Log("Unloading " + sceneReference.ScenePath + " (Standard)");
				AsyncOperation unloadOp = this.GetUnloadSceneOp(sceneReference);
				while (unloadOp != null && unloadOp.progress < 1f)
				{
					yield return null;
					yield return this.RefreshSessionKey();
				}
				unloadOp = null;
			}
			if (unloadUnused)
			{
				yield return Resources.UnloadUnusedAssets();
			}
			yield return this.RefreshSessionKey();
			yield break;
		}

		// Token: 0x0600284A RID: 10314 RVA: 0x0013B124 File Offset: 0x00139324
		private AsyncOperation GetUnloadSceneOp(SceneReference sceneReference)
		{
			if (!sceneReference.IsAddressable())
			{
				Scene sceneFromSceneReference = this.GetSceneFromSceneReference(sceneReference);
				if (sceneFromSceneReference.isLoaded)
				{
					return SceneManager.UnloadSceneAsync(sceneFromSceneReference);
				}
			}
			return null;
		}

		// Token: 0x0600284B RID: 10315 RVA: 0x0013B154 File Offset: 0x00139354
		public void DisconnectAndLoadZone(int zoneId)
		{
			IEnumerator routine = this.DisconnectAndLoadZoneRoutine((ZoneId)zoneId);
			base.StartCoroutine(routine);
		}

		// Token: 0x0600284C RID: 10316 RVA: 0x0005C168 File Offset: 0x0005A368
		private IEnumerator DisconnectAndLoadZoneRoutine(ZoneId zoneId)
		{
			ClientNetworkManager.DisconnectInitiatedByClient = true;
			GameManager.NetworkManager.Disconnect();
			while (GameManager.NetworkManager.ConnectionIsActive)
			{
				yield return null;
			}
			this.LoadZoneId(zoneId);
			yield break;
		}

		// Token: 0x0600284D RID: 10317 RVA: 0x0013B174 File Offset: 0x00139374
		public void RequestInstanceChange(int instanceId)
		{
			if (this.m_instanceChangePending)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "An instance change request is already pending!");
				return;
			}
			if (instanceId < 0)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Invalid instance request!");
				return;
			}
			if (instanceId == LocalZoneManager.ZoneRecord.InstanceId)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "You are already in that instance!");
				return;
			}
			if (LocalPlayer.GameEntity == null || LocalPlayer.GameEntity.CharacterData == null)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Invalid request!");
				return;
			}
			if (!LocalPlayer.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.InCampfire))
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "You must be near an Ember Ring to change instances!");
				return;
			}
			if (LocalPlayer.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.InCombat))
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "You cannot change instances while in combat!");
				return;
			}
			this.m_instanceChangePending = true;
			LoginApiManager.RequestInstanceChange((ZoneId)LocalZoneManager.ZoneRecord.ZoneId, instanceId, new Action<bool>(this.InstanceChangeResponse));
		}

		// Token: 0x0600284E RID: 10318 RVA: 0x0013B280 File Offset: 0x00139480
		private void InstanceChangeResponse(bool result)
		{
			this.m_instanceChangePending = false;
			if (!result)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Failed to change instances!");
				return;
			}
			MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Switching to instance " + LocalZoneManager.ZoneRecord.InstanceId.ToString());
			GameManager.SceneCompositionManager.DisconnectAndLoadZone(LocalZoneManager.ZoneRecord.ZoneId);
		}

		// Token: 0x04002974 RID: 10612
		private static bool m_isLoading;

		// Token: 0x04002975 RID: 10613
		private const float kSceneLoadIsDone = 0.9f;

		// Token: 0x04002976 RID: 10614
		private const float kMaxSceneLoadPercent = 0.9f;

		// Token: 0x04002977 RID: 10615
		public const float kRequestingCharacter = 0.9f;

		// Token: 0x04002978 RID: 10616
		public const float kInstantiatingCharacter = 0.92f;

		// Token: 0x04002979 RID: 10617
		public const float kUiLoad = 0.94f;

		// Token: 0x0400297A RID: 10618
		public const float kPostUiLoad = 0.97f;

		// Token: 0x0400297B RID: 10619
		public const float kQuestValue = 0.98f;

		// Token: 0x0400297C RID: 10620
		public const float kBulkSpawn = 0.99f;

		// Token: 0x0400297D RID: 10621
		private const string kStandardDescription = "(Standard)";

		// Token: 0x0400297E RID: 10622
		private const string kAddressableDescription = "(Addressable)";

		// Token: 0x0400297F RID: 10623
		private Scene m_managerScene;

		// Token: 0x04002980 RID: 10624
		protected ISceneComposition m_currentComposition;

		// Token: 0x04002981 RID: 10625
		protected SceneConfiguration m_sceneConfiguration;

		// Token: 0x04002982 RID: 10626
		protected IEnumerator m_loadRoutine;

		// Token: 0x04002983 RID: 10627
		private SceneLoadOp? m_currentLoadOp;

		// Token: 0x04002984 RID: 10628
		private string m_currentScenePath = string.Empty;

		// Token: 0x04002985 RID: 10629
		private readonly List<SceneReference> m_scenesReferencesToLoad = new List<SceneReference>(10);

		// Token: 0x04002986 RID: 10630
		private readonly List<SceneReference> m_scenesReferencesToUnload = new List<SceneReference>(10);

		// Token: 0x04002987 RID: 10631
		private readonly List<SceneInstance> m_addressableScenes = new List<SceneInstance>(10);

		// Token: 0x04002988 RID: 10632
		private readonly StopwatchTimer m_timer = new StopwatchTimer();

		// Token: 0x04002989 RID: 10633
		private bool m_instanceChangePending;
	}
}
