using System;
using SoL.Game.Interactives;
using SoL.Game.NPCs;
using SoL.Game.Spawning;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Quests.Objectives
{
	// Token: 0x020007A9 RID: 1961
	[Serializable]
	public class NpcSelection
	{
		// Token: 0x17000D46 RID: 3398
		// (get) Token: 0x060039CC RID: 14796 RVA: 0x000672E1 File Offset: 0x000654E1
		private bool m_isMinimumLevelAbsolute
		{
			get
			{
				return this.m_levelMinimumType == LevelMinimumType.Absolute;
			}
		}

		// Token: 0x17000D47 RID: 3399
		// (get) Token: 0x060039CD RID: 14797 RVA: 0x000672EC File Offset: 0x000654EC
		private bool m_isMinimumLevelRelative
		{
			get
			{
				return this.m_levelMinimumType == LevelMinimumType.Relative;
			}
		}

		// Token: 0x060039CE RID: 14798 RVA: 0x0017494C File Offset: 0x00172B4C
		public bool IsValid(InteractiveNpc interactive, GameEntity selectingEntity)
		{
			if (interactive == null)
			{
				return false;
			}
			bool result = true;
			if (this.m_criteria == TargetCriteria.Name && interactive.GameEntity.CharacterData.Name != this.m_npcName)
			{
				result = false;
			}
			if (this.m_criteria == TargetCriteria.Tags)
			{
				NpcTagSet query = (interactive.GameEntity && interactive.GameEntity.CharacterData) ? interactive.GameEntity.CharacterData.NpcTagsSet : null;
				if (!this.m_npcTagMatch.Matches(query))
				{
					result = false;
				}
			}
			if (this.m_criteria == TargetCriteria.Spawn)
			{
				bool flag = false;
				foreach (NpcSpawnProfileV2 y in this.m_spawnProfiles)
				{
					if (interactive.SpawnProfile == y)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					result = false;
				}
			}
			if (interactive.ChallengeRating < this.m_minimumChallengeRating || interactive.ChallengeRating > this.m_maximumChallengeRating)
			{
				result = false;
			}
			LevelMinimumType levelMinimumType = this.m_levelMinimumType;
			if (levelMinimumType != LevelMinimumType.Absolute)
			{
				if (levelMinimumType == LevelMinimumType.Relative)
				{
					if (interactive.Level < selectingEntity.CharacterData.AdventuringLevel - this.m_minimumLevelRelative)
					{
						result = false;
					}
				}
			}
			else if (interactive.Level < this.m_minimumLevel)
			{
				result = false;
			}
			if (this.m_validZones != null && this.m_validZones.Length != 0 && LocalZoneManager.ZoneRecord.ZoneId > 2)
			{
				bool flag2 = false;
				foreach (ZoneId zoneId in this.m_validZones)
				{
					if (LocalZoneManager.ZoneRecord.ZoneId == (int)zoneId)
					{
						flag2 = true;
					}
				}
				if (!flag2)
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x04003865 RID: 14437
		[SerializeField]
		private TargetCriteria m_criteria;

		// Token: 0x04003866 RID: 14438
		[SerializeField]
		private string m_npcName;

		// Token: 0x04003867 RID: 14439
		[SerializeField]
		private NpcTagMatch m_npcTagMatch;

		// Token: 0x04003868 RID: 14440
		[SerializeField]
		private NpcSpawnProfileV2[] m_spawnProfiles;

		// Token: 0x04003869 RID: 14441
		[SerializeField]
		private ChallengeRating m_minimumChallengeRating;

		// Token: 0x0400386A RID: 14442
		[SerializeField]
		private ChallengeRating m_maximumChallengeRating = ChallengeRating.CRB;

		// Token: 0x0400386B RID: 14443
		[SerializeField]
		private LevelMinimumType m_levelMinimumType;

		// Token: 0x0400386C RID: 14444
		[SerializeField]
		private int m_minimumLevel = 1;

		// Token: 0x0400386D RID: 14445
		[SerializeField]
		private int m_minimumLevelRelative = 5;

		// Token: 0x0400386E RID: 14446
		[SerializeField]
		private ZoneId[] m_validZones;
	}
}
