using System;
using SoL.Game.Messages;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Quests
{
	// Token: 0x02000795 RID: 1941
	public class InteractiveQuestTriggerClick : InteractiveQuestTriggerBase
	{
		// Token: 0x17000D26 RID: 3366
		// (get) Token: 0x06003950 RID: 14672 RVA: 0x00066CDA File Offset: 0x00064EDA
		public bool TriggerIfQuestNotPresent
		{
			get
			{
				return this.m_triggerIfQuestNotPresent;
			}
		}

		// Token: 0x06003951 RID: 14673 RVA: 0x00172970 File Offset: 0x00170B70
		protected override void DoInternalInteraction()
		{
			int hash;
			if (this.m_quest.TryGetObjectiveHashForActiveObjective(this.m_objective.Id, out hash) || this.m_triggerIfQuestNotPresent)
			{
				GameManager.QuestManager.Progress(new ObjectiveIterationCache
				{
					QuestId = this.m_quest.Id,
					ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(hash),
					WorldId = base.WorldId,
					StartQuestIfNotPresent = this.m_triggerIfQuestNotPresent
				}, null, false);
				return;
			}
			MessageManager.ChatQueue.AddToQueue(MessageType.Quest, "Nothing to see here...");
		}

		// Token: 0x0400380F RID: 14351
		[SerializeField]
		private bool m_triggerIfQuestNotPresent;
	}
}
