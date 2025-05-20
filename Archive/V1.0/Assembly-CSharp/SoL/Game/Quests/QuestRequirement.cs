using System;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.Quests
{
	// Token: 0x0200078C RID: 1932
	[Serializable]
	public class QuestRequirement : IRequirement
	{
		// Token: 0x17000D0B RID: 3339
		// (get) Token: 0x06003908 RID: 14600 RVA: 0x00066A27 File Offset: 0x00064C27
		private bool m_hasQuest
		{
			get
			{
				return this.m_quest != null;
			}
		}

		// Token: 0x06003909 RID: 14601 RVA: 0x001718D0 File Offset: 0x0016FAD0
		public bool MeetsAllRequirements(GameEntity entity)
		{
			bool flag = true;
			if (this.m_quest != null)
			{
				flag = (flag && this.m_quest.IsOnQuest(entity));
			}
			if (this.m_quest != null && this.m_completed)
			{
				flag = (flag && this.m_quest.IsComplete(entity));
			}
			if (this.m_quest != null && this.m_onQuestStep != null)
			{
				QuestProgressionData questProgressionData;
				flag = (flag && this.m_quest.TryGetProgress(entity, out questProgressionData) && questProgressionData.CurrentNodeId == this.m_onQuestStep.Id);
			}
			return flag;
		}

		// Token: 0x040037DD RID: 14301
		[SerializeField]
		private Quest m_quest;

		// Token: 0x040037DE RID: 14302
		[SerializeField]
		private bool m_completed;

		// Token: 0x040037DF RID: 14303
		[SerializeField]
		private QuestStep m_onQuestStep;
	}
}
