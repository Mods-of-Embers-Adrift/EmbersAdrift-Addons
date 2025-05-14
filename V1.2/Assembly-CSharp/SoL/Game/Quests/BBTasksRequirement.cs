using System;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.Quests
{
	// Token: 0x0200077D RID: 1917
	[Serializable]
	public class BBTasksRequirement : IRequirement
	{
		// Token: 0x0600388D RID: 14477 RVA: 0x0016E0C8 File Offset: 0x0016C2C8
		public bool MeetsAllRequirements(GameEntity entity)
		{
			bool flag = true;
			if (entity && entity.CollectionController != null && entity.CollectionController.Record != null && entity.CollectionController.Record.Progression != null)
			{
				flag = (flag && entity.CollectionController.Record.Progression != null && entity.CollectionController.Record.Progression.BBTasks_AdventuringCompletionCount + entity.CollectionController.Record.Progression.BBTasks_CraftingCompletionCount + entity.CollectionController.Record.Progression.BBTasks_GatheringCompletionCount >= this.m_totalCompletionCount);
				flag = (flag && entity.CollectionController.Record.Progression != null && entity.CollectionController.Record.Progression.BBTasks_AdventuringCompletionCount >= this.m_adventuringCompletionCount);
				flag = (flag && entity.CollectionController.Record.Progression != null && entity.CollectionController.Record.Progression.BBTasks_CraftingCompletionCount >= this.m_craftingCompletionCount);
				flag = (flag && entity.CollectionController.Record.Progression != null && entity.CollectionController.Record.Progression.BBTasks_GatheringCompletionCount >= this.m_gatheringCompletionCount);
			}
			return flag;
		}

		// Token: 0x04003758 RID: 14168
		[SerializeField]
		private int m_totalCompletionCount;

		// Token: 0x04003759 RID: 14169
		[SerializeField]
		private int m_adventuringCompletionCount;

		// Token: 0x0400375A RID: 14170
		[SerializeField]
		private int m_craftingCompletionCount;

		// Token: 0x0400375B RID: 14171
		[SerializeField]
		private int m_gatheringCompletionCount;
	}
}
