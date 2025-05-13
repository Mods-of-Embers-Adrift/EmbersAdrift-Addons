using System;
using SoL.Game;
using SoL.Game.Scenes;
using SoL.Networking.Managers;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoL.Managers
{
	// Token: 0x02000544 RID: 1348
	public class ServerSceneCompositionManager : SceneCompositionManager
	{
		// Token: 0x17000873 RID: 2163
		// (get) Token: 0x060028F7 RID: 10487 RVA: 0x00053500 File Offset: 0x00051700
		protected override SceneInclusionFlags m_requiredFlag
		{
			get
			{
				return SceneInclusionFlags.Server;
			}
		}

		// Token: 0x17000874 RID: 2164
		// (get) Token: 0x060028F8 RID: 10488 RVA: 0x0005C5AF File Offset: 0x0005A7AF
		protected override SceneReference m_managerSceneReference
		{
			get
			{
				return this.m_sceneConfiguration.ServerStartup.ManagerScene;
			}
		}

		// Token: 0x060028F9 RID: 10489 RVA: 0x0013F41C File Offset: 0x0013D61C
		protected override void LoadStartupScene()
		{
			base.LoadStartupScene();
			ISceneComposition composition = this.m_sceneConfiguration.ServerStartup;
			ZoneId zoneFromCommandLineArgs = this.GetZoneFromCommandLineArgs();
			if (zoneFromCommandLineArgs != ZoneId.None)
			{
				composition = this.m_sceneConfiguration.GetZone(zoneFromCommandLineArgs);
			}
			ServerNetworkManager.InstanceId = this.GetZoneInstanceId();
			this.m_loadRoutine = base.LoadCompositionAsync(composition);
			base.StartCoroutine(this.m_loadRoutine);
		}

		// Token: 0x060028FA RID: 10490 RVA: 0x0005C5C1 File Offset: 0x0005A7C1
		protected void UnloadPrevious()
		{
			base.StartCoroutine(base.UnloadPreviousCompositionAsync());
		}

		// Token: 0x060028FB RID: 10491 RVA: 0x0013F478 File Offset: 0x0013D678
		private ZoneId GetZoneFromCommandLineArgs()
		{
			ZoneId zoneId = ZoneId.None;
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			string text = string.Empty;
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				if ((commandLineArgs[i] == "-zone" || commandLineArgs[i] == "-scene") && i < commandLineArgs.Length - 1)
				{
					text = commandLineArgs[i + 1];
					Debug.Log("Settings sceneToLoad=" + text);
					break;
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				int num;
				if (int.TryParse(text, out num) && Enum.IsDefined(typeof(ZoneId), num))
				{
					zoneId = (ZoneId)num;
					Debug.Log(string.Format("Found replacement scene in CommandLineArgs!  {0}", zoneId));
				}
				else if (Enum.TryParse<ZoneId>(text, true, out zoneId))
				{
					Debug.Log(string.Format("Found replacement scene in CommandLineArgs!  {0}", zoneId));
				}
			}
			return zoneId;
		}

		// Token: 0x060028FC RID: 10492 RVA: 0x0013F544 File Offset: 0x0013D744
		private int GetZoneInstanceId()
		{
			int result = 0;
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				int num;
				if (commandLineArgs[i] == "-instance" && i < commandLineArgs.Length - 1 && int.TryParse(commandLineArgs[i + 1], out num))
				{
					result = num;
					Debug.Log("InstanceId set to " + result.ToString() + " from the command line.");
					break;
				}
			}
			return result;
		}

		// Token: 0x060028FD RID: 10493 RVA: 0x0005C5D0 File Offset: 0x0005A7D0
		protected override void ZoneLoadComplete()
		{
			if (LocalZoneManager.ZoneRecord != null)
			{
				new GameObject("ZoneManagers").AddComponent<ActiveZoneRecordUpdater>();
			}
			this.DisableItemsForServer();
			this.UpdateNavIterationsPerFrame();
		}

		// Token: 0x060028FE RID: 10494 RVA: 0x0013F5AC File Offset: 0x0013D7AC
		private void DisableItemsForServer()
		{
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				foreach (GameObject obj in SceneManager.GetSceneAt(i).GetRootGameObjects())
				{
					this.DisableBehaviors<AudioSource>(obj);
					this.DisableBehaviors<Light>(obj);
					this.DisableMeshRenderers(obj);
					this.DisableParticleSystems(obj);
				}
			}
		}

		// Token: 0x060028FF RID: 10495 RVA: 0x0005C5F5 File Offset: 0x0005A7F5
		private void UpdateNavIterationsPerFrame()
		{
			NavMeshUtilities.InitializePathfindingIterationsPerFrame();
		}

		// Token: 0x06002900 RID: 10496 RVA: 0x0013F60C File Offset: 0x0013D80C
		private void DisableBehaviors<T>(GameObject obj) where T : Behaviour
		{
			try
			{
				T[] componentsInChildren = obj.GetComponentsInChildren<T>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = false;
				}
			}
			catch (Exception message)
			{
				Debug.LogWarning(message);
			}
		}

		// Token: 0x06002901 RID: 10497 RVA: 0x0013F65C File Offset: 0x0013D85C
		private void DisableMeshRenderers(GameObject obj)
		{
			MeshRenderer[] componentsInChildren = obj.GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}

		// Token: 0x06002902 RID: 10498 RVA: 0x0013F688 File Offset: 0x0013D888
		private void DisableParticleSystems(GameObject obj)
		{
			ParticleSystem[] componentsInChildren = obj.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Stop(true);
			}
		}
	}
}
