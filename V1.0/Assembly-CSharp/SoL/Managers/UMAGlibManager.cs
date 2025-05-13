using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game;
using SoL.Game.Login.Client.Creation.NewCreation;
using SoL.Game.UMA;
using UMA;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.Managers
{
	// Token: 0x02000520 RID: 1312
	public class UMAGlibManager : MonoBehaviour
	{
		// Token: 0x17000825 RID: 2085
		// (get) Token: 0x0600274A RID: 10058 RVA: 0x0005B825 File Offset: 0x00059A25
		// (set) Token: 0x0600274B RID: 10059 RVA: 0x0005B82C File Offset: 0x00059A2C
		public static bool AtCreation { get; set; } = false;

		// Token: 0x17000826 RID: 2086
		// (get) Token: 0x0600274C RID: 10060 RVA: 0x0005B834 File Offset: 0x00059A34
		// (set) Token: 0x0600274D RID: 10061 RVA: 0x0005B83B File Offset: 0x00059A3B
		private static int CurrentResolution { get; set; } = 2048;

		// Token: 0x0600274E RID: 10062 RVA: 0x00137140 File Offset: 0x00135340
		public static void RegisterController(IDcaSource controller)
		{
			if (UMAGlibManager.m_controllers == null)
			{
				UMAGlibManager.m_controllers = new List<IDcaSource>(1000);
			}
			if (!UMAGlibManager.m_controllers.Contains(controller))
			{
				controller.SetResolution(new int?(UMAGlibManager.CurrentResolution), false);
				UMAGlibManager.m_controllers.Add(controller);
			}
		}

		// Token: 0x0600274F RID: 10063 RVA: 0x0013718C File Offset: 0x0013538C
		public static void UnregisterController(IDcaSource controller)
		{
			controller.SetResolution(null, false);
			List<IDcaSource> controllers = UMAGlibManager.m_controllers;
			if (controllers == null)
			{
				return;
			}
			controllers.Remove(controller);
		}

		// Token: 0x06002750 RID: 10064 RVA: 0x0005B843 File Offset: 0x00059A43
		public static void ValidateUmaResolution()
		{
			if (Options.VideoOptions.UMAResolution.Value < 0 || Options.VideoOptions.UMAResolution.Value >= UMAGlibManager.AtlasSizes.Count)
			{
				Options.VideoOptions.UMAResolution.Value = Options.VideoOptions.UMAResolution.DefaultValue;
			}
		}

		// Token: 0x06002751 RID: 10065 RVA: 0x0005B87C File Offset: 0x00059A7C
		private static UMAGlibManager.AtlasResolutionSize GetNextResolution(UMAGlibManager.AtlasResolutionSize size)
		{
			if (size == UMAGlibManager.AtlasResolutionSize.k512)
			{
				return UMAGlibManager.AtlasResolutionSize.k1024;
			}
			if (size != UMAGlibManager.AtlasResolutionSize.k1024)
			{
				return UMAGlibManager.AtlasResolutionSize.k4096;
			}
			return UMAGlibManager.AtlasResolutionSize.k2048;
		}

		// Token: 0x06002752 RID: 10066 RVA: 0x001371BC File Offset: 0x001353BC
		private void Awake()
		{
			if (this.m_generator)
			{
				this.m_generator.fastGeneration = true;
				this.m_generator.InitialScaleFactor = this.m_initialScaleFactor;
				this.m_generator.IterationCount = this.m_iterationCount;
				this.m_generator.collectGarbage = this.m_collectGarbage;
				this.m_generator.garbageCollectionRate = this.m_garbageCollectionRate;
				UMAGlibManager.ValidateUmaResolution();
				this.UpdateCurrentResolution();
				SceneCompositionManager.ZoneLoadStarted += this.SceneCompositionManagerOnZoneLoadStarted;
				SceneCompositionManager.LoadingStartupScene += this.ClientSceneCompositionManagerOnStartupSceneLoaded;
			}
		}

		// Token: 0x06002753 RID: 10067 RVA: 0x0005B8A1 File Offset: 0x00059AA1
		private void Start()
		{
			this.m_wait = new WaitForSeconds(0.5f);
			this.m_updateCo = this.UpdateCo();
			base.StartCoroutine(this.m_updateCo);
		}

		// Token: 0x06002754 RID: 10068 RVA: 0x00137254 File Offset: 0x00135454
		private void OnDestroy()
		{
			if (this.m_generator)
			{
				SceneCompositionManager.ZoneLoadStarted -= this.SceneCompositionManagerOnZoneLoadStarted;
				SceneCompositionManager.LoadingStartupScene -= this.ClientSceneCompositionManagerOnStartupSceneLoaded;
			}
			if (this.m_updateCo != null)
			{
				base.StopCoroutine(this.m_updateCo);
			}
		}

		// Token: 0x06002755 RID: 10069 RVA: 0x001372A4 File Offset: 0x001354A4
		private void UpdateCurrentResolution()
		{
			if (this.m_generator)
			{
				UMAGlibManager.AtlasResolutionSize atlasResolutionSize2;
				UMAGlibManager.AtlasResolutionSize atlasResolutionSize = UMAGlibManager.AtlasSizes.TryGetValue(Options.VideoOptions.UMAResolution.Value, out atlasResolutionSize2) ? atlasResolutionSize2 : UMAGlibManager.AtlasResolutionSize.k2048;
				if (UMAGlibManager.AtCreation)
				{
					atlasResolutionSize = UMAGlibManager.GetNextResolution(atlasResolutionSize);
				}
				UMAGlibManager.CurrentResolution = (int)atlasResolutionSize;
				this.m_generator.atlasResolution = UMAGlibManager.CurrentResolution;
				return;
			}
			UMAGlibManager.CurrentResolution = 2048;
		}

		// Token: 0x06002756 RID: 10070 RVA: 0x0005B8CC File Offset: 0x00059ACC
		private void SceneCompositionManagerOnZoneLoadStarted(ZoneId obj)
		{
			if (this.m_generator)
			{
				this.m_generator.fastGeneration = false;
			}
		}

		// Token: 0x06002757 RID: 10071 RVA: 0x0005B8E7 File Offset: 0x00059AE7
		private void ClientSceneCompositionManagerOnStartupSceneLoaded()
		{
			if (this.m_generator)
			{
				this.m_generator.fastGeneration = true;
			}
		}

		// Token: 0x06002758 RID: 10072 RVA: 0x0005B902 File Offset: 0x00059B02
		private int GetAtlasResolution(UMAGlibManager.AtlasResolutionSize resolutionSize)
		{
			return Mathf.Clamp((int)resolutionSize, 512, 4096);
		}

		// Token: 0x06002759 RID: 10073 RVA: 0x0005B914 File Offset: 0x00059B14
		private IEnumerator UpdateCo()
		{
			for (;;)
			{
				if (UMAGlibManager.m_controllers != null && this.m_generator)
				{
					int num;
					for (int i = 0; i < UMAGlibManager.m_controllers.Count; i = num + 1)
					{
						if (UMAGlibManager.m_controllers[i] == null)
						{
							UMAGlibManager.m_controllers.RemoveAt(i);
							i--;
						}
						else
						{
							this.UpdateCurrentResolution();
							if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.DCAController && LocalPlayer.GameEntity.DCAController.Resolution != null && LocalPlayer.GameEntity.DCAController.Resolution.Value != UMAGlibManager.CurrentResolution)
							{
								LocalPlayer.GameEntity.DCAController.SetResolution(new int?(UMAGlibManager.CurrentResolution), true);
								yield return this.m_wait;
							}
							if ((UMAGlibManager.m_controllers[i].Resolution == null || UMAGlibManager.m_controllers[i].Resolution.Value != UMAGlibManager.CurrentResolution) && (!UMAGlibManager.AtCreation || UMAGlibManager.m_controllers[i] is NewCharacter))
							{
								UMAGlibManager.m_controllers[i].SetResolution(new int?(UMAGlibManager.CurrentResolution), true);
								yield return this.m_wait;
							}
						}
						num = i;
					}
				}
				yield return this.m_wait;
			}
			yield break;
		}

		// Token: 0x04002910 RID: 10512
		private const int kMinAtlasResolution = 512;

		// Token: 0x04002913 RID: 10515
		public static readonly List<string> UmaResolutionOptions = new List<string>
		{
			"Low",
			"Medium",
			"High"
		};

		// Token: 0x04002914 RID: 10516
		private static readonly Dictionary<int, UMAGlibManager.AtlasResolutionSize> AtlasSizes = new Dictionary<int, UMAGlibManager.AtlasResolutionSize>
		{
			{
				0,
				UMAGlibManager.AtlasResolutionSize.k512
			},
			{
				1,
				UMAGlibManager.AtlasResolutionSize.k1024
			},
			{
				2,
				UMAGlibManager.AtlasResolutionSize.k2048
			}
		};

		// Token: 0x04002915 RID: 10517
		private static List<IDcaSource> m_controllers = null;

		// Token: 0x04002916 RID: 10518
		private const int kMaxAtlasResolution = 4096;

		// Token: 0x04002917 RID: 10519
		[SerializeField]
		private UMAGenerator m_generator;

		// Token: 0x04002918 RID: 10520
		[SerializeField]
		private int m_initialScaleFactor = 1;

		// Token: 0x04002919 RID: 10521
		[SerializeField]
		private int m_iterationCount = 1;

		// Token: 0x0400291A RID: 10522
		[SerializeField]
		private bool m_collectGarbage;

		// Token: 0x0400291B RID: 10523
		[SerializeField]
		private int m_garbageCollectionRate = 8;

		// Token: 0x0400291C RID: 10524
		[SerializeField]
		private UMAGlibManager.AtlasResolutionSize m_loginAtlasResolution = UMAGlibManager.AtlasResolutionSize.k4096;

		// Token: 0x0400291D RID: 10525
		[FormerlySerializedAs("m_atlasResolutionSize")]
		[SerializeField]
		private UMAGlibManager.AtlasResolutionSize m_inGameAtlasResolution = UMAGlibManager.AtlasResolutionSize.k2048;

		// Token: 0x0400291E RID: 10526
		private const float kCadence = 0.5f;

		// Token: 0x0400291F RID: 10527
		private WaitForSeconds m_wait;

		// Token: 0x04002920 RID: 10528
		private IEnumerator m_updateCo;

		// Token: 0x02000521 RID: 1313
		private enum AtlasResolutionSize
		{
			// Token: 0x04002922 RID: 10530
			k512 = 512,
			// Token: 0x04002923 RID: 10531
			k1024 = 1024,
			// Token: 0x04002924 RID: 10532
			k2048 = 2048,
			// Token: 0x04002925 RID: 10533
			k4096 = 4096,
			// Token: 0x04002926 RID: 10534
			k8192 = 8192
		}
	}
}
