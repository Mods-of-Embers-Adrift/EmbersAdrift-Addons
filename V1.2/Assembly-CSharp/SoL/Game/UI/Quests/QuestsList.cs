using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using SoL.Game.Quests;
using SoL.Utilities.Extensions;

namespace SoL.Game.UI.Quests
{
	// Token: 0x02000949 RID: 2377
	public class QuestsList : OSA<BaseParamsWithPrefab, QuestsItemViewsHolder>, ICategorizedList<Quest>
	{
		// Token: 0x17000FAD RID: 4013
		// (get) Token: 0x06004644 RID: 17988 RVA: 0x0006F44C File Offset: 0x0006D64C
		// (set) Token: 0x06004645 RID: 17989 RVA: 0x0006F454 File Offset: 0x0006D654
		public int SelectedIndex { get; private set; } = -1;

		// Token: 0x140000D5 RID: 213
		// (add) Token: 0x06004646 RID: 17990 RVA: 0x001A35A8 File Offset: 0x001A17A8
		// (remove) Token: 0x06004647 RID: 17991 RVA: 0x001A35E0 File Offset: 0x001A17E0
		public event Action<Quest> SelectionChanged;

		// Token: 0x06004648 RID: 17992 RVA: 0x001A3618 File Offset: 0x001A1818
		public void UpdateItems(ICollection<Quest> items)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.m_items == null)
			{
				this.m_items = new List<Quest>(items.Count);
			}
			this.m_items.Clear();
			this.m_items.AddRange(items);
			this.ResetItems(this.m_items.Count, false, false);
		}

		// Token: 0x06004649 RID: 17993 RVA: 0x001A3678 File Offset: 0x001A1878
		public void Select(int index)
		{
			int selectedIndex = this.SelectedIndex;
			this.SelectedIndex = index;
			QuestsItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(index);
			if (selectedIndex != index)
			{
				if (itemViewsHolderIfVisible != null)
				{
					Action<Quest> selectionChanged = this.SelectionChanged;
					if (selectionChanged != null)
					{
						selectionChanged(itemViewsHolderIfVisible.Data);
					}
				}
				if (selectedIndex != -1)
				{
					QuestsItemViewsHolder itemViewsHolderIfVisible2 = base.GetItemViewsHolderIfVisible(selectedIndex);
					if (itemViewsHolderIfVisible2 == null)
					{
						return;
					}
					itemViewsHolderIfVisible2.ListItem.RefreshVisuals();
				}
			}
		}

		// Token: 0x0600464A RID: 17994 RVA: 0x001A36D4 File Offset: 0x001A18D4
		public void DeselectAll(bool suppressEvents = false)
		{
			int selectedIndex = this.SelectedIndex;
			this.SelectedIndex = -1;
			if (selectedIndex != -1 && !suppressEvents)
			{
				Action<Quest> selectionChanged = this.SelectionChanged;
				if (selectionChanged != null)
				{
					selectionChanged(null);
				}
			}
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				QuestsItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.RefreshVisuals();
				}
			}
		}

		// Token: 0x0600464B RID: 17995 RVA: 0x001A3730 File Offset: 0x001A1930
		public void ReindexItems(Quest selectedItem)
		{
			this.SelectedIndex = -1;
			if (selectedItem != null)
			{
				for (int i = 0; i < this.m_items.Count; i++)
				{
					if (this.m_items[i].Id == selectedItem.Id)
					{
						this.SelectedIndex = i;
					}
				}
			}
			for (int j = 0; j < this.GetItemsCount(); j++)
			{
				QuestsItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(j);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.Reindex(j);
				}
			}
		}

		// Token: 0x0600464C RID: 17996 RVA: 0x0006F45D File Offset: 0x0006D65D
		public void Sort(List<Quest> unsorted)
		{
			unsorted.Sort((Quest x, Quest y) => x.Title.TitleCompare(y.Title));
		}

		// Token: 0x0600464D RID: 17997 RVA: 0x0006F484 File Offset: 0x0006D684
		protected override QuestsItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			QuestsItemViewsHolder questsItemViewsHolder = new QuestsItemViewsHolder();
			questsItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return questsItemViewsHolder;
		}

		// Token: 0x0600464E RID: 17998 RVA: 0x0006F4AA File Offset: 0x0006D6AA
		protected override void UpdateViewsHolder(QuestsItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateItem(this, this.m_items[newOrRecycled.ItemIndex]);
		}

		// Token: 0x04004262 RID: 16994
		private List<Quest> m_items;
	}
}
