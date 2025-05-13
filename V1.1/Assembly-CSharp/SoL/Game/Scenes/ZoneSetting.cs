using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.Settings;
using SoL.Game.Spawning;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Scenes
{
	// Token: 0x0200075D RID: 1885
	[Serializable]
	public class ZoneSetting : ISceneComposition
	{
		// Token: 0x17000CCE RID: 3278
		// (get) Token: 0x06003810 RID: 14352 RVA: 0x0016C9B4 File Offset: 0x0016ABB4
		public string IndexName
		{
			get
			{
				string text = this.m_zoneId.ToString();
				if (this.m_excludeFromBuild)
				{
					text += " (Excluded)";
				}
				else if (this.m_branchFlags != DeploymentBranchFlags.None)
				{
					text = text + " (" + this.m_branchFlags.ToString() + ")";
				}
				return text;
			}
		}

		// Token: 0x17000CCF RID: 3279
		// (get) Token: 0x06003811 RID: 14353 RVA: 0x00066357 File Offset: 0x00064557
		public bool ActiveSceneIsValid
		{
			get
			{
				return this.m_config != null && this.m_config.ActiveSceneIsValid;
			}
		}

		// Token: 0x17000CD0 RID: 3280
		// (get) Token: 0x06003812 RID: 14354 RVA: 0x00066374 File Offset: 0x00064574
		public ZoneId ZoneId
		{
			get
			{
				return this.m_zoneId;
			}
		}

		// Token: 0x06003813 RID: 14355 RVA: 0x0016CA14 File Offset: 0x0016AC14
		private Sprite GetRandomLoadingImage()
		{
			if (this.m_loadingImages == null || this.m_loadingImages.Length == 0)
			{
				return null;
			}
			int num = UnityEngine.Random.Range(0, this.m_loadingImages.Length);
			return this.m_loadingImages[num];
		}

		// Token: 0x06003814 RID: 14356 RVA: 0x0006637C File Offset: 0x0006457C
		private IEnumerable GetConfigs()
		{
			return SolOdinUtilities.GetDropdownItems<ZoneSceneComposition>();
		}

		// Token: 0x17000CD1 RID: 3281
		// (get) Token: 0x06003815 RID: 14357 RVA: 0x00066383 File Offset: 0x00064583
		SceneReference ISceneComposition.ActiveScene
		{
			get
			{
				if (!(this.m_config == null))
				{
					return ((ISceneComposition)this.m_config).ActiveScene;
				}
				return null;
			}
		}

		// Token: 0x17000CD2 RID: 3282
		// (get) Token: 0x06003816 RID: 14358 RVA: 0x000663A0 File Offset: 0x000645A0
		SceneReference ISceneComposition.ImposterScene
		{
			get
			{
				if (!(this.m_config == null))
				{
					return ((ISceneComposition)this.m_config).ImposterScene;
				}
				return null;
			}
		}

		// Token: 0x06003817 RID: 14359 RVA: 0x000663BD File Offset: 0x000645BD
		IEnumerable<SceneReference> ISceneComposition.GetAdditionalScenes(SceneInclusionFlags flags, bool beforeActive)
		{
			if (!this.m_config)
			{
				yield break;
			}
			foreach (SceneReference sceneReference in ((ISceneComposition)this.m_config).GetAdditionalScenes(flags, beforeActive))
			{
				yield return sceneReference;
			}
			IEnumerator<SceneReference> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x17000CD3 RID: 3283
		// (get) Token: 0x06003818 RID: 14360 RVA: 0x0004479C File Offset: 0x0004299C
		bool ISceneComposition.HasZoneSettings
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000CD4 RID: 3284
		// (get) Token: 0x06003819 RID: 14361 RVA: 0x00066374 File Offset: 0x00064574
		ZoneId ISceneComposition.ZoneId
		{
			get
			{
				return this.m_zoneId;
			}
		}

		// Token: 0x0600381A RID: 14362 RVA: 0x000663DB File Offset: 0x000645DB
		Sprite ISceneComposition.GetRandomLoadingImage()
		{
			return this.GetRandomLoadingImage();
		}

		// Token: 0x0600381B RID: 14363 RVA: 0x0016CA4C File Offset: 0x0016AC4C
		string ISceneComposition.GetLoadingTip()
		{
			if (this.DynamicLoadingTips == null)
			{
				List<StringProbabilityEntry> list = new List<StringProbabilityEntry>(10);
				if (GlobalSettings.Values && GlobalSettings.Values.UI != null && GlobalSettings.Values.UI.GeneralLoadingTips && GlobalSettings.Values.UI.GeneralLoadingTips.Entries != null)
				{
					for (int i = 0; i < GlobalSettings.Values.UI.GeneralLoadingTips.Entries.Length; i++)
					{
						StringProbabilityEntry stringProbabilityEntry = new StringProbabilityEntry();
						stringProbabilityEntry.CloneFrom(GlobalSettings.Values.UI.GeneralLoadingTips.Entries[i]);
						list.Add(stringProbabilityEntry);
					}
				}
				if (this.m_loadingTips && this.m_loadingTips.Entries != null)
				{
					for (int j = 0; j < this.m_loadingTips.Entries.Length; j++)
					{
						StringProbabilityEntry stringProbabilityEntry2 = new StringProbabilityEntry();
						stringProbabilityEntry2.CloneFrom(this.m_loadingTips.Entries[j]);
						list.Add(stringProbabilityEntry2);
					}
				}
				this.DynamicLoadingTips = new StringProbabilityCollection
				{
					Entries = list.ToArray()
				};
			}
			StringProbabilityCollection dynamicLoadingTips = this.DynamicLoadingTips;
			if (dynamicLoadingTips == null)
			{
				return null;
			}
			StringProbabilityEntry entry = dynamicLoadingTips.GetEntry(null, false);
			if (entry == null)
			{
				return null;
			}
			return entry.Obj;
		}

		// Token: 0x0600381C RID: 14364 RVA: 0x0016CB88 File Offset: 0x0016AD88
		string ISceneComposition.GetSceneLoadCompleteMessage()
		{
			if (this.DynamicSceneLoadCompleteMessages == null)
			{
				List<StringProbabilityEntry> list = new List<StringProbabilityEntry>(10);
				if (GlobalSettings.Values && GlobalSettings.Values.UI != null && GlobalSettings.Values.UI.SceneLoadCompleteMessages && GlobalSettings.Values.UI.SceneLoadCompleteMessages.Entries != null)
				{
					for (int i = 0; i < GlobalSettings.Values.UI.SceneLoadCompleteMessages.Entries.Length; i++)
					{
						StringProbabilityEntry stringProbabilityEntry = new StringProbabilityEntry();
						stringProbabilityEntry.CloneFrom(GlobalSettings.Values.UI.SceneLoadCompleteMessages.Entries[i]);
						list.Add(stringProbabilityEntry);
					}
				}
				if (this.m_sceneLoadCompleteMessages && this.m_sceneLoadCompleteMessages.Entries != null)
				{
					for (int j = 0; j < this.m_sceneLoadCompleteMessages.Entries.Length; j++)
					{
						StringProbabilityEntry stringProbabilityEntry2 = new StringProbabilityEntry();
						stringProbabilityEntry2.CloneFrom(this.m_sceneLoadCompleteMessages.Entries[j]);
						list.Add(stringProbabilityEntry2);
					}
				}
				this.DynamicSceneLoadCompleteMessages = new StringProbabilityCollection
				{
					Entries = list.ToArray()
				};
			}
			StringProbabilityCollection dynamicSceneLoadCompleteMessages = this.DynamicSceneLoadCompleteMessages;
			if (dynamicSceneLoadCompleteMessages == null)
			{
				return null;
			}
			StringProbabilityEntry entry = dynamicSceneLoadCompleteMessages.GetEntry(null, false);
			if (entry == null)
			{
				return null;
			}
			return entry.Obj;
		}

		// Token: 0x040036F1 RID: 14065
		private const string kDataGroup = "Data";

		// Token: 0x040036F2 RID: 14066
		private const string kMiscGroup = "Misc";

		// Token: 0x040036F3 RID: 14067
		[SerializeField]
		private bool m_excludeFromBuild;

		// Token: 0x040036F4 RID: 14068
		[SerializeField]
		private ZoneSceneComposition m_config;

		// Token: 0x040036F5 RID: 14069
		[SerializeField]
		private ZoneId m_zoneId;

		// Token: 0x040036F6 RID: 14070
		[SerializeField]
		private DeploymentBranchFlags m_branchFlags;

		// Token: 0x040036F7 RID: 14071
		[SerializeField]
		private Sprite[] m_loadingImages;

		// Token: 0x040036F8 RID: 14072
		[SerializeField]
		private StringScriptableProbabilityCollection m_loadingTips;

		// Token: 0x040036F9 RID: 14073
		[SerializeField]
		private StringScriptableProbabilityCollection m_sceneLoadCompleteMessages;

		// Token: 0x040036FA RID: 14074
		[NonSerialized]
		private StringProbabilityCollection DynamicLoadingTips;

		// Token: 0x040036FB RID: 14075
		[NonSerialized]
		private StringProbabilityCollection DynamicSceneLoadCompleteMessages;
	}
}
