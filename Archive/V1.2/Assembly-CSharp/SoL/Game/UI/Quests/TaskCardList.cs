using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using SoL.Game.Quests;

namespace SoL.Game.UI.Quests
{
	// Token: 0x0200094F RID: 2383
	public class TaskCardList : OSA<BaseParamsWithPrefab, TaskCardViewsHolder>
	{
		// Token: 0x06004683 RID: 18051 RVA: 0x001A4888 File Offset: 0x001A2A88
		public void UpdateItems(ICollection<BBTask> items)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.m_items == null)
			{
				this.m_items = new List<BBTask>(items.Count);
			}
			this.m_items.Clear();
			this.m_items.AddRange(items);
			this.ResetItems(this.m_items.Count, false, false);
		}

		// Token: 0x06004684 RID: 18052 RVA: 0x001A48E8 File Offset: 0x001A2AE8
		public void Refresh()
		{
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				TaskCardViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.UpdateItem(this.m_items[i]);
				}
			}
		}

		// Token: 0x06004685 RID: 18053 RVA: 0x0006F75D File Offset: 0x0006D95D
		protected override TaskCardViewsHolder CreateViewsHolder(int itemIndex)
		{
			TaskCardViewsHolder taskCardViewsHolder = new TaskCardViewsHolder();
			taskCardViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return taskCardViewsHolder;
		}

		// Token: 0x06004686 RID: 18054 RVA: 0x0006F783 File Offset: 0x0006D983
		protected override void UpdateViewsHolder(TaskCardViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateItem(this.m_items[newOrRecycled.ItemIndex]);
		}

		// Token: 0x0400429F RID: 17055
		private List<BBTask> m_items;
	}
}
