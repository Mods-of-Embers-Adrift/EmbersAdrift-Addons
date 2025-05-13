using System;
using System.Collections.Generic;
using SoL.Game.Settings;
using SoL.Game.Spawning;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Scenes
{
	// Token: 0x02000750 RID: 1872
	public abstract class SceneComposition : ScriptableObject, ISceneComposition
	{
		// Token: 0x17000CBB RID: 3259
		// (get) Token: 0x060037D8 RID: 14296 RVA: 0x00066216 File Offset: 0x00064416
		public bool ActiveSceneIsValid
		{
			get
			{
				return this.m_activeScene.IsValid();
			}
		}

		// Token: 0x17000CBC RID: 3260
		// (get) Token: 0x060037D9 RID: 14297 RVA: 0x00049FFA File Offset: 0x000481FA
		protected virtual SceneReference ImposterScene
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060037DA RID: 14298 RVA: 0x00066223 File Offset: 0x00064423
		protected virtual IEnumerable<SceneReference> GetAdditionalScenes(SceneInclusionFlags flags, bool beforeActive)
		{
			if (!beforeActive)
			{
				int num;
				for (int i = 0; i < this.m_scenes.Length; i = num + 1)
				{
					if (this.m_scenes[i].InclusionFlags.HasBitFlag(flags) && this.m_scenes[i].IsProperDeploymentBranch(DeploymentBranchFlagsExtensions.GetBranchFlags()) && this.m_scenes[i].SceneReference.IsValid())
					{
						yield return this.m_scenes[i].SceneReference;
					}
					num = i;
				}
			}
			yield break;
		}

		// Token: 0x17000CBD RID: 3261
		// (get) Token: 0x060037DB RID: 14299 RVA: 0x00066241 File Offset: 0x00064441
		SceneReference ISceneComposition.ActiveScene
		{
			get
			{
				return this.m_activeScene;
			}
		}

		// Token: 0x17000CBE RID: 3262
		// (get) Token: 0x060037DC RID: 14300 RVA: 0x00066249 File Offset: 0x00064449
		SceneReference ISceneComposition.ImposterScene
		{
			get
			{
				return this.ImposterScene;
			}
		}

		// Token: 0x060037DD RID: 14301 RVA: 0x00066251 File Offset: 0x00064451
		IEnumerable<SceneReference> ISceneComposition.GetAdditionalScenes(SceneInclusionFlags flags, bool beforeActive)
		{
			return this.GetAdditionalScenes(flags, beforeActive);
		}

		// Token: 0x17000CBF RID: 3263
		// (get) Token: 0x060037DE RID: 14302 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool ISceneComposition.HasZoneSettings
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000CC0 RID: 3264
		// (get) Token: 0x060037DF RID: 14303 RVA: 0x00045BCA File Offset: 0x00043DCA
		ZoneId ISceneComposition.ZoneId
		{
			get
			{
				return ZoneId.None;
			}
		}

		// Token: 0x060037E0 RID: 14304 RVA: 0x00049FFA File Offset: 0x000481FA
		Sprite ISceneComposition.GetRandomLoadingImage()
		{
			return null;
		}

		// Token: 0x060037E1 RID: 14305 RVA: 0x00045BC3 File Offset: 0x00043DC3
		string ISceneComposition.GetLoadingTip()
		{
			return string.Empty;
		}

		// Token: 0x060037E2 RID: 14306 RVA: 0x0016C22C File Offset: 0x0016A42C
		string ISceneComposition.GetSceneLoadCompleteMessage()
		{
			if (GlobalSettings.Values && GlobalSettings.Values.UI != null && GlobalSettings.Values.UI.SceneLoadCompleteMessages)
			{
				StringProbabilityEntry entry = GlobalSettings.Values.UI.SceneLoadCompleteMessages.GetEntry();
				if (entry != null)
				{
					return entry.Obj;
				}
			}
			return "Reticulating splines";
		}

		// Token: 0x060037E3 RID: 14307 RVA: 0x0004479C File Offset: 0x0004299C
		public static bool ProceedIfDirtyScenes()
		{
			return true;
		}

		// Token: 0x040036B4 RID: 14004
		protected const int kActiveSceneOrder = -10;

		// Token: 0x040036B5 RID: 14005
		protected const int kSceneArrayOrder = 10;

		// Token: 0x040036B6 RID: 14006
		protected const string kSceneGroup = "Scenes";

		// Token: 0x040036B7 RID: 14007
		[SerializeField]
		protected SceneReference m_activeScene;

		// Token: 0x040036B8 RID: 14008
		[SerializeField]
		protected SceneSetting[] m_scenes;
	}
}
