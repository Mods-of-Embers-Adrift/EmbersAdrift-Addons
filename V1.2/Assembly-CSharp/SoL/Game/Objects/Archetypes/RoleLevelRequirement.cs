using System;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A97 RID: 2711
	[Serializable]
	public class RoleLevelRequirement : IRequirement
	{
		// Token: 0x17001349 RID: 4937
		// (get) Token: 0x06005404 RID: 21508 RVA: 0x00078271 File Offset: 0x00076471
		public int LevelRequirementLevel
		{
			get
			{
				if (this.m_levelRequirement == null)
				{
					return 0;
				}
				return this.m_levelRequirement.Level;
			}
		}

		// Token: 0x06005405 RID: 21509 RVA: 0x00078288 File Offset: 0x00076488
		public bool MatchesNameFilter(string filter)
		{
			return this.m_roleRequirementWithOverride != null && this.m_roleRequirementWithOverride.MatchesNameFilter(filter);
		}

		// Token: 0x06005406 RID: 21510 RVA: 0x000782A0 File Offset: 0x000764A0
		public bool MeetsAllRequirements(GameEntity entity)
		{
			return this.MeetsRoleRequirements(entity) && this.MeetsLevelRequirements(entity);
		}

		// Token: 0x06005407 RID: 21511 RVA: 0x000782B4 File Offset: 0x000764B4
		public bool MeetsLevelRequirements(GameEntity entity)
		{
			return this.m_levelRequirement != null && this.m_levelRequirement.MeetsAllRequirements(entity);
		}

		// Token: 0x06005408 RID: 21512 RVA: 0x000782CC File Offset: 0x000764CC
		public bool MeetsRoleRequirements(GameEntity entity)
		{
			return this.m_roleRequirementWithOverride != null && this.m_roleRequirementWithOverride.MeetsAllRequirements(entity);
		}

		// Token: 0x06005409 RID: 21513 RVA: 0x000782E4 File Offset: 0x000764E4
		public bool HasRequiredTrade(GameEntity entity)
		{
			return this.m_levelRequirement != null && this.m_levelRequirement.HasRequiredTrade(entity);
		}

		// Token: 0x0600540A RID: 21514 RVA: 0x000782FC File Offset: 0x000764FC
		public void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			TooltipExtensions.AddRoleLevelRequirement(tooltip, instance, entity, this.m_levelRequirement, this.m_roleRequirementWithOverride);
		}

		// Token: 0x04004AC8 RID: 19144
		[SerializeField]
		private LevelRequirement m_levelRequirement;

		// Token: 0x04004AC9 RID: 19145
		[SerializeField]
		private RoleRequirementWithOverride m_roleRequirementWithOverride;
	}
}
