using System;
using System.Collections.Generic;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x0200085F RID: 2143
	public class CodexJournalSummary : MonoBehaviour
	{
		// Token: 0x06003DDF RID: 15839 RVA: 0x00069E0A File Offset: 0x0006800A
		private void Start()
		{
			this.QuestManagerOnQuestsUpdated();
			GameManager.QuestManager.QuestsUpdated += this.QuestManagerOnQuestsUpdated;
		}

		// Token: 0x06003DE0 RID: 15840 RVA: 0x00069E28 File Offset: 0x00068028
		private void OnDestroy()
		{
			GameManager.QuestManager.QuestsUpdated -= this.QuestManagerOnQuestsUpdated;
		}

		// Token: 0x06003DE1 RID: 15841 RVA: 0x00183C10 File Offset: 0x00181E10
		private void QuestManagerOnQuestsUpdated()
		{
			if (LocalPlayer.GameEntity == null || LocalPlayer.GameEntity.CollectionController == null || LocalPlayer.GameEntity.CollectionController.Record == null || LocalPlayer.GameEntity.CollectionController.Record.Progression == null || LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests == null)
			{
				this.RefreshEntries();
				return;
			}
			foreach (UniqueId uniqueId in LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests.Keys)
			{
				JournalEntryUI journalEntryUI;
				if (this.m_journalEntries.TryGetValue(uniqueId, out journalEntryUI))
				{
					journalEntryUI.RefreshEntry();
				}
				else
				{
					JournalEntryUI emptyJournalEntry = this.GetEmptyJournalEntry();
					if (emptyJournalEntry != null && emptyJournalEntry.Init(uniqueId))
					{
						this.m_journalEntries.Add(uniqueId, emptyJournalEntry);
					}
				}
			}
			this.RefreshEntries();
		}

		// Token: 0x06003DE2 RID: 15842 RVA: 0x00183D1C File Offset: 0x00181F1C
		private void RefreshEntries()
		{
			for (int i = 0; i < this.m_journalEntryUis.Length; i++)
			{
				this.m_journalEntryUis[i].gameObject.SetActive(!this.m_journalEntryUis[i].QuestId.IsEmpty);
			}
		}

		// Token: 0x06003DE3 RID: 15843 RVA: 0x00183D68 File Offset: 0x00181F68
		private JournalEntryUI GetEmptyJournalEntry()
		{
			for (int i = 0; i < this.m_journalEntryUis.Length; i++)
			{
				if (this.m_journalEntryUis[i].QuestId == UniqueId.Empty)
				{
					return this.m_journalEntryUis[i];
				}
			}
			return null;
		}

		// Token: 0x04003C52 RID: 15442
		[SerializeField]
		private JournalEntryUI[] m_journalEntryUis;

		// Token: 0x04003C53 RID: 15443
		private readonly Dictionary<UniqueId, JournalEntryUI> m_journalEntries = new Dictionary<UniqueId, JournalEntryUI>(default(UniqueIdComparer));
	}
}
