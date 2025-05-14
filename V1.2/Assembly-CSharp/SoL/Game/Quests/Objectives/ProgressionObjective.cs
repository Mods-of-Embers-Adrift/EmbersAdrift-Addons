using System;
using UnityEngine;

namespace SoL.Game.Quests.Objectives
{
	// Token: 0x020007AF RID: 1967
	[CreateAssetMenu(menuName = "SoL/Quests/Objectives/ProgressionObjective")]
	public class ProgressionObjective : QuestObjective
	{
		// Token: 0x17000D4A RID: 3402
		// (get) Token: 0x060039DD RID: 14813 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool CanBePassive
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060039DE RID: 14814 RVA: 0x000673B8 File Offset: 0x000655B8
		public override bool Validate(GameEntity sourceEntity, ObjectiveIterationCache cache, out string message)
		{
			if (cache.IterationsRequested > 1)
			{
				message = "Too many iterations requested.";
				return false;
			}
			if (!this.m_progression.MeetsAllRequirements(sourceEntity))
			{
				message = "Progression requirement(s) unmet.";
				return false;
			}
			message = string.Empty;
			return true;
		}

		// Token: 0x04003878 RID: 14456
		[SerializeField]
		private ProgressionRequirement m_progression;
	}
}
