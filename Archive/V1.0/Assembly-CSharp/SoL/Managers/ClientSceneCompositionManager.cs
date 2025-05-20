using System;
using System.Collections;
using SoL.Game;
using SoL.Game.Scenes;
using SoL.Game.Settings;
using SoL.Networking.Database;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Audio;

namespace SoL.Managers
{
	// Token: 0x020004F1 RID: 1265
	public class ClientSceneCompositionManager : SceneCompositionManager
	{
		// Token: 0x14000043 RID: 67
		// (add) Token: 0x0600237A RID: 9082 RVA: 0x0012B044 File Offset: 0x00129244
		// (remove) Token: 0x0600237B RID: 9083 RVA: 0x0012B078 File Offset: 0x00129278
		public static event Action StartupSceneLoaded;

		// Token: 0x0600237C RID: 9084 RVA: 0x000598A9 File Offset: 0x00057AA9
		protected override void Start()
		{
			base.Start();
			Options.VideoOptions.UseImposterBillboards.Changed += this.UseImposterBillboardsOnChanged;
		}

		// Token: 0x0600237D RID: 9085 RVA: 0x000598C7 File Offset: 0x00057AC7
		private void OnDestroy()
		{
			Options.VideoOptions.UseImposterBillboards.Changed -= this.UseImposterBillboardsOnChanged;
		}

		// Token: 0x0600237E RID: 9086 RVA: 0x0012B0AC File Offset: 0x001292AC
		private void UseImposterBillboardsOnChanged()
		{
			if (LocalZoneManager.ZoneRecord != null)
			{
				ZoneId zoneId = (ZoneId)LocalZoneManager.ZoneRecord.ZoneId;
				ISceneComposition zone = this.m_sceneConfiguration.GetZone(zoneId);
				if (zone != null && zone.ImposterScene != null && zone.ImposterScene.IsValid())
				{
					IEnumerator routine = Options.VideoOptions.UseImposterBillboards.Value ? base.LoadSingleScene(zone.ImposterScene) : base.UnloadSingleScene(zone.ImposterScene, true);
					base.StartCoroutine(routine);
				}
			}
		}

		// Token: 0x0600237F RID: 9087 RVA: 0x0012B120 File Offset: 0x00129320
		protected override void LoadStartupScene()
		{
			base.LoadStartupScene();
			SceneComposition clientStartup = this.m_sceneConfiguration.ClientStartup;
			this.m_loadRoutine = base.LoadCompositionAsync(clientStartup);
			base.StartCoroutine(this.m_loadRoutine);
		}

		// Token: 0x06002380 RID: 9088 RVA: 0x000598DF File Offset: 0x00057ADF
		public override void ReloadStartupScene()
		{
			LocalPlayer.ResetSessionStart();
			base.ReloadStartupScene();
			base.StartCoroutine("ReloadStartupSceneCo");
		}

		// Token: 0x06002381 RID: 9089 RVA: 0x000598F8 File Offset: 0x00057AF8
		private IEnumerator ReloadStartupSceneCo()
		{
			yield return base.LoadCompositionAsync(this.m_sceneConfiguration.ClientStartup);
			Action startupSceneLoaded = ClientSceneCompositionManager.StartupSceneLoaded;
			if (startupSceneLoaded != null)
			{
				startupSceneLoaded();
			}
			GameManager.Instance.Reset();
			UIManager.LoadingScreenUI.HideLoadingScreen();
			yield break;
		}

		// Token: 0x17000727 RID: 1831
		// (get) Token: 0x06002382 RID: 9090 RVA: 0x0004479C File Offset: 0x0004299C
		protected override SceneInclusionFlags m_requiredFlag
		{
			get
			{
				return SceneInclusionFlags.Client;
			}
		}

		// Token: 0x17000728 RID: 1832
		// (get) Token: 0x06002383 RID: 9091 RVA: 0x00059907 File Offset: 0x00057B07
		protected override SceneReference m_managerSceneReference
		{
			get
			{
				return this.m_sceneConfiguration.ClientStartup.ManagerScene;
			}
		}

		// Token: 0x06002384 RID: 9092 RVA: 0x00059919 File Offset: 0x00057B19
		protected override bool BypassLoadUnload(ISceneComposition composition, SceneReference sceneRef)
		{
			if (composition == null || sceneRef == null || sceneRef != composition.ImposterScene)
			{
				return base.BypassLoadUnload(composition, sceneRef);
			}
			return !Options.VideoOptions.UseImposterBillboards.Value;
		}

		// Token: 0x06002385 RID: 9093 RVA: 0x00059940 File Offset: 0x00057B40
		protected override void SetLoadStatus(string message)
		{
			base.SetLoadStatus(message);
			UIManager.LoadingScreenUI.SetLoadingStatus(message);
		}

		// Token: 0x06002386 RID: 9094 RVA: 0x00059954 File Offset: 0x00057B54
		protected override void SetLoadPercent(float percent)
		{
			base.SetLoadPercent(percent);
			UIManager.LoadingScreenUI.SetLoadingPercent(percent);
		}

		// Token: 0x06002387 RID: 9095 RVA: 0x0012B15C File Offset: 0x0012935C
		protected override void SetDestination(ZoneId zoneId)
		{
			base.SetDestination(zoneId);
			ZoneRecord zoneRecord = SessionData.GetZoneRecord(zoneId);
			if (zoneRecord != null)
			{
				UIManager.LoadingScreenUI.SetDestination(zoneRecord.DisplayName);
			}
		}

		// Token: 0x06002388 RID: 9096 RVA: 0x00059968 File Offset: 0x00057B68
		protected override void SetDestinationText(string txt)
		{
			base.SetDestinationText(txt);
			UIManager.LoadingScreenUI.SetDestination(txt);
		}

		// Token: 0x06002389 RID: 9097 RVA: 0x0005997C File Offset: 0x00057B7C
		protected override void SetLoadingImage(Sprite img)
		{
			base.SetLoadingImage(img);
			UIManager.LoadingScreenUI.SetLoadingImage(img);
		}

		// Token: 0x0600238A RID: 9098 RVA: 0x00059990 File Offset: 0x00057B90
		protected override void SetLoadingTip(string tip)
		{
			base.SetLoadingTip(tip);
			UIManager.LoadingScreenUI.SetLoadingTip(tip);
		}

		// Token: 0x0600238B RID: 9099 RVA: 0x000599A4 File Offset: 0x00057BA4
		protected override void ToggleEllipsis(bool show)
		{
			base.ToggleEllipsis(show);
			UIManager.LoadingScreenUI.ToggleEllipsis(show);
		}

		// Token: 0x0600238C RID: 9100 RVA: 0x000599B8 File Offset: 0x00057BB8
		protected override void ToggleLoadingScreenIgnoreEvents(bool ignore)
		{
			UIManager.LoadingScreenUI.IgnoreEvents = ignore;
		}

		// Token: 0x0600238D RID: 9101 RVA: 0x0012B18C File Offset: 0x0012938C
		protected override void ZoneLoadBegin()
		{
			base.ZoneLoadBegin();
			if (this.m_mixersToDuck == null)
			{
				this.m_mixersToDuck = new ClientSceneCompositionManager.MixerToDuck[]
				{
					new ClientSceneCompositionManager.MixerToDuck(GlobalSettings.Values.Audio.SfxParentMixerGroup),
					new ClientSceneCompositionManager.MixerToDuck(GlobalSettings.Values.Audio.AmbientMixerGroup)
				};
			}
			for (int i = 0; i < this.m_mixersToDuck.Length; i++)
			{
				this.m_mixersToDuck[i].DuckMixer();
			}
		}

		// Token: 0x0600238E RID: 9102 RVA: 0x0012B204 File Offset: 0x00129404
		protected override void ZoneLoadComplete()
		{
			base.ZoneLoadComplete();
			if (this.m_mixersToDuck != null)
			{
				for (int i = 0; i < this.m_mixersToDuck.Length; i++)
				{
					this.m_mixersToDuck[i].RestoreMixer();
				}
			}
		}

		// Token: 0x0600238F RID: 9103 RVA: 0x000599C5 File Offset: 0x00057BC5
		protected override IEnumerator RefreshSessionKey()
		{
			LoginApiManager.ManuallyRefreshingSessionKey = true;
			yield return LoginApiManager.RefreshSessionKey();
			LoginApiManager.ManuallyRefreshingSessionKey = false;
			yield break;
		}

		// Token: 0x040026F6 RID: 9974
		private ClientSceneCompositionManager.MixerToDuck[] m_mixersToDuck;

		// Token: 0x020004F2 RID: 1266
		private class MixerToDuck
		{
			// Token: 0x06002391 RID: 9105 RVA: 0x000599D5 File Offset: 0x00057BD5
			public MixerToDuck(AudioMixerGroup mixerGroup)
			{
				this.m_mixerGroup = mixerGroup;
				this.m_parameterName = mixerGroup.name + "Volume";
			}

			// Token: 0x06002392 RID: 9106 RVA: 0x0012B240 File Offset: 0x00129440
			public void DuckMixer()
			{
				float value;
				if (GlobalSettings.Values.Audio.Mixer.GetFloat(this.m_parameterName, out value) && GlobalSettings.Values.Audio.Mixer.SetFloat(this.m_parameterName, -80f))
				{
					this.m_decibels = new float?(value);
				}
			}

			// Token: 0x06002393 RID: 9107 RVA: 0x0012B298 File Offset: 0x00129498
			public void RestoreMixer()
			{
				if (this.m_decibels != null && GlobalSettings.Values.Audio.Mixer.SetFloat(this.m_parameterName, this.m_decibels.Value))
				{
					this.m_decibels = null;
				}
			}

			// Token: 0x040026F7 RID: 9975
			private const float kDuckedVolume = -80f;

			// Token: 0x040026F8 RID: 9976
			private readonly AudioMixerGroup m_mixerGroup;

			// Token: 0x040026F9 RID: 9977
			private readonly string m_parameterName;

			// Token: 0x040026FA RID: 9978
			private float? m_decibels;
		}
	}
}
