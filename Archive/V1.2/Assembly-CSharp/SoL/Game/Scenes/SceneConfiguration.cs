using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Game.Scenes
{
	// Token: 0x02000752 RID: 1874
	[CreateAssetMenu(menuName = "SoL/Scene Management/New Config Yay")]
	public class SceneConfiguration : ScriptableObject
	{
		// Token: 0x17000CC3 RID: 3267
		// (get) Token: 0x060037ED RID: 14317 RVA: 0x00066285 File Offset: 0x00064485
		public StartupSceneComposition ServerStartup
		{
			get
			{
				return this.m_serverStartup;
			}
		}

		// Token: 0x17000CC4 RID: 3268
		// (get) Token: 0x060037EE RID: 14318 RVA: 0x0006628D File Offset: 0x0006448D
		public StartupSceneComposition ClientStartup
		{
			get
			{
				return this.m_clientStartup;
			}
		}

		// Token: 0x060037EF RID: 14319 RVA: 0x0016C3EC File Offset: 0x0016A5EC
		public ISceneComposition GetZone(ZoneId zoneId)
		{
			if (!Application.isPlaying || this.m_zoneDict == null)
			{
				this.BuildZoneDict();
			}
			ZoneSetting result;
			this.m_zoneDict.TryGetValue(zoneId, out result);
			return result;
		}

		// Token: 0x060037F0 RID: 14320 RVA: 0x0016C420 File Offset: 0x0016A620
		private void BuildZoneDict()
		{
			this.m_zoneDict = new Dictionary<ZoneId, ZoneSetting>(default(ZoneIdComparer));
			for (int i = 0; i < this.m_zones.Length; i++)
			{
				if (this.m_zones[i].ZoneId == ZoneId.None)
				{
					Debug.LogWarning(string.Format("Zone index {0}'s ZoneId is None!", i));
				}
				else if (this.m_zoneDict.ContainsKey(this.m_zones[i].ZoneId))
				{
					Debug.LogWarning(string.Format("Zone index {0}'s ZoneId of {1} has already been added!", i, this.m_zones[i].ZoneId));
				}
				else if (!this.m_zones[i].ActiveSceneIsValid)
				{
					Debug.LogWarning(string.Format("Zone index {0}'s ZoneId of {1} has no valid active scene!", i, this.m_zones[i].ZoneId));
				}
				else
				{
					this.m_zoneDict.Add(this.m_zones[i].ZoneId, this.m_zones[i]);
				}
			}
		}

		// Token: 0x040036C2 RID: 14018
		[SerializeField]
		private SceneConfigurationOptions m_serverBuildOptions;

		// Token: 0x040036C3 RID: 14019
		[SerializeField]
		private SceneConfigurationOptions m_gmBuildOptions;

		// Token: 0x040036C4 RID: 14020
		[SerializeField]
		private SceneConfigurationOptions m_stdBuildOptions;

		// Token: 0x040036C5 RID: 14021
		[SerializeField]
		private StartupSceneComposition m_serverStartup;

		// Token: 0x040036C6 RID: 14022
		[SerializeField]
		private StartupSceneComposition m_clientStartup;

		// Token: 0x040036C7 RID: 14023
		[SerializeField]
		private ZoneSetting[] m_zones;

		// Token: 0x040036C8 RID: 14024
		[NonSerialized]
		private Dictionary<ZoneId, ZoneSetting> m_zoneDict;
	}
}
