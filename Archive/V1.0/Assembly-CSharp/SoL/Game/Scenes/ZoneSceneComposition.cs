using System;
using System.Collections.Generic;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Scenes
{
	// Token: 0x0200075B RID: 1883
	[CreateAssetMenu(menuName = "SoL/Scene Management/Zone Scene Config")]
	public class ZoneSceneComposition : SceneComposition
	{
		// Token: 0x17000CCA RID: 3274
		// (get) Token: 0x06003802 RID: 14338 RVA: 0x00066241 File Offset: 0x00064441
		private SceneReference m_networkScene
		{
			get
			{
				return this.m_activeScene;
			}
		}

		// Token: 0x17000CCB RID: 3275
		// (get) Token: 0x06003803 RID: 14339 RVA: 0x000662E1 File Offset: 0x000644E1
		protected override SceneReference ImposterScene
		{
			get
			{
				return this.m_imposterScene;
			}
		}

		// Token: 0x06003804 RID: 14340 RVA: 0x000662E9 File Offset: 0x000644E9
		protected override IEnumerable<SceneReference> GetAdditionalScenes(SceneInclusionFlags flags, bool beforeActive)
		{
			if (beforeActive)
			{
				if (this.m_settingsScene.IsValid())
				{
					yield return this.m_settingsScene;
				}
			}
			else
			{
				if (flags.HasBitFlag(SceneInclusionFlags.Client))
				{
					if (this.m_skyDomeScene.IsValid())
					{
						yield return this.m_skyDomeScene;
					}
					if (this.m_imposterScene.IsValid())
					{
						yield return this.m_imposterScene;
					}
				}
				if (flags.HasBitFlag(SceneInclusionFlags.Server) && this.m_serverSkyDomeScene.IsValid())
				{
					yield return this.m_serverSkyDomeScene;
				}
			}
			foreach (SceneReference sceneReference in base.GetAdditionalScenes(flags, beforeActive))
			{
				yield return sceneReference;
			}
			IEnumerator<SceneReference> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x040036E3 RID: 14051
		[SerializeField]
		private SceneReference m_settingsScene;

		// Token: 0x040036E4 RID: 14052
		[SerializeField]
		private SceneReference m_imposterScene;

		// Token: 0x040036E5 RID: 14053
		[SerializeField]
		private SceneReference m_skyDomeScene;

		// Token: 0x040036E6 RID: 14054
		[SerializeField]
		private SceneReference m_serverSkyDomeScene;

		// Token: 0x040036E7 RID: 14055
		[SerializeField]
		private SceneReference m_offlineScene;
	}
}
