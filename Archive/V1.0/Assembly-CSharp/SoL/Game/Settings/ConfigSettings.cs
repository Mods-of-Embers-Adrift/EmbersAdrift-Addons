using System;
using System.Collections;
using SoL.Game.Scenes;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x02000722 RID: 1826
	[Serializable]
	public class ConfigSettings
	{
		// Token: 0x17000C41 RID: 3137
		// (get) Token: 0x060036E8 RID: 14056 RVA: 0x00065945 File Offset: 0x00063B45
		public GameData Data
		{
			get
			{
				return this.m_data;
			}
		}

		// Token: 0x060036E9 RID: 14057 RVA: 0x0006594D File Offset: 0x00063B4D
		public SceneConfiguration GetSceneConfiguration(bool allowEditorConfig = false)
		{
			return this.m_primary;
		}

		// Token: 0x060036EA RID: 14058 RVA: 0x00065955 File Offset: 0x00063B55
		private IEnumerable GetSceneConfigurationFiles()
		{
			return SolOdinUtilities.GetDropdownItems<SceneConfiguration>();
		}

		// Token: 0x060036EB RID: 14059 RVA: 0x0006595C File Offset: 0x00063B5C
		private IEnumerable GetGameDataFiles()
		{
			return SolOdinUtilities.GetDropdownItems<GameData>();
		}

		// Token: 0x0400351C RID: 13596
		[SerializeField]
		private SceneConfiguration m_primary;

		// Token: 0x0400351D RID: 13597
		[SerializeField]
		private SceneConfiguration m_editor;

		// Token: 0x0400351E RID: 13598
		[SerializeField]
		private GameData m_data;
	}
}
