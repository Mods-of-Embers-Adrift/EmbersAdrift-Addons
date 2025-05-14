using System;
using SoL.Game.Objects.Archetypes;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.Quests
{
	// Token: 0x02000789 RID: 1929
	[Serializable]
	public class ProgressionRequirement : IRequirement
	{
		// Token: 0x17000CFB RID: 3323
		// (get) Token: 0x060038DC RID: 14556 RVA: 0x000668FA File Offset: 0x00064AFA
		public QuestsRequirement Quests
		{
			get
			{
				return this.m_questsRequirement;
			}
		}

		// Token: 0x17000CFC RID: 3324
		// (get) Token: 0x060038DD RID: 14557 RVA: 0x00066902 File Offset: 0x00064B02
		public KnowledgeRequirement Knowledge
		{
			get
			{
				return this.m_knowledgeRequirement;
			}
		}

		// Token: 0x17000CFD RID: 3325
		// (get) Token: 0x060038DE RID: 14558 RVA: 0x0006690A File Offset: 0x00064B0A
		public RoleRequirementWithOverride Role
		{
			get
			{
				return this.m_roleRequirementWithOverride;
			}
		}

		// Token: 0x17000CFE RID: 3326
		// (get) Token: 0x060038DF RID: 14559 RVA: 0x00066912 File Offset: 0x00064B12
		public LevelRequirement Level
		{
			get
			{
				return this.m_levelRequirement;
			}
		}

		// Token: 0x17000CFF RID: 3327
		// (get) Token: 0x060038E0 RID: 14560 RVA: 0x0006691A File Offset: 0x00064B1A
		public BBTasksRequirement BBTasks
		{
			get
			{
				return this.m_bbTasksRequirement;
			}
		}

		// Token: 0x060038E1 RID: 14561 RVA: 0x00170DC4 File Offset: 0x0016EFC4
		public bool MeetsAllRequirements(GameEntity entity)
		{
			bool flag = true;
			if (this.m_questsRequirement != null)
			{
				flag = (flag && this.m_questsRequirement.MeetsAllRequirements(entity));
			}
			if (this.m_knowledgeRequirement != null)
			{
				flag = (flag && this.m_knowledgeRequirement.MeetsAllRequirements(entity));
			}
			if (this.m_roleRequirementWithOverride != null)
			{
				flag = (flag && this.m_roleRequirementWithOverride.MeetsAllRequirements(entity));
			}
			if (this.m_levelRequirement != null)
			{
				flag = (flag && this.m_levelRequirement.MeetsAllRequirements(entity));
			}
			if (this.m_bbTasksRequirement != null)
			{
				flag = (flag && this.m_bbTasksRequirement.MeetsAllRequirements(entity));
			}
			return flag;
		}

		// Token: 0x060038E2 RID: 14562 RVA: 0x00066922 File Offset: 0x00064B22
		public void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			TooltipExtensions.AddRoleLevelRequirement(tooltip, instance, entity, this.m_levelRequirement, this.m_roleRequirementWithOverride);
		}

		// Token: 0x040037C4 RID: 14276
		[SerializeField]
		private QuestsRequirement m_questsRequirement;

		// Token: 0x040037C5 RID: 14277
		[SerializeField]
		private KnowledgeRequirement m_knowledgeRequirement;

		// Token: 0x040037C6 RID: 14278
		[SerializeField]
		private RoleRequirementWithOverride m_roleRequirementWithOverride;

		// Token: 0x040037C7 RID: 14279
		[SerializeField]
		private LevelRequirement m_levelRequirement;

		// Token: 0x040037C8 RID: 14280
		[SerializeField]
		private BBTasksRequirement m_bbTasksRequirement;
	}
}
