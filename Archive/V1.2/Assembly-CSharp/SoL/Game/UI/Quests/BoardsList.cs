using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using SoL.Game.Quests;

namespace SoL.Game.UI.Quests
{
	// Token: 0x02000939 RID: 2361
	public class BoardsList : OSA<BaseParamsWithPrefab, TasksItemViewsHolder>, ICategorizedList<BulletinBoard>
	{
		// Token: 0x17000F96 RID: 3990
		// (get) Token: 0x060045AB RID: 17835 RVA: 0x0006EDEE File Offset: 0x0006CFEE
		// (set) Token: 0x060045AC RID: 17836 RVA: 0x0006EDF6 File Offset: 0x0006CFF6
		public int SelectedIndex { get; private set; } = -1;

		// Token: 0x17000F97 RID: 3991
		// (get) Token: 0x060045AD RID: 17837 RVA: 0x0006EDFF File Offset: 0x0006CFFF
		// (set) Token: 0x060045AE RID: 17838 RVA: 0x0006EE07 File Offset: 0x0006D007
		public BulletinBoard SelectedItem { get; private set; }

		// Token: 0x140000D0 RID: 208
		// (add) Token: 0x060045AF RID: 17839 RVA: 0x001A0F74 File Offset: 0x0019F174
		// (remove) Token: 0x060045B0 RID: 17840 RVA: 0x001A0FAC File Offset: 0x0019F1AC
		public event Action<BulletinBoard> SelectionChanged;

		// Token: 0x060045B1 RID: 17841 RVA: 0x001A0FE4 File Offset: 0x0019F1E4
		public void UpdateItems(ICollection<BulletinBoard> items)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.m_items == null)
			{
				this.m_items = new List<BulletinBoard>(items.Count);
			}
			this.m_items.Clear();
			this.m_items.AddRange(items);
			this.ResetItems(this.m_items.Count, false, false);
		}

		// Token: 0x060045B2 RID: 17842 RVA: 0x001A1044 File Offset: 0x0019F244
		public void Select(int index)
		{
			int selectedIndex = this.SelectedIndex;
			this.SelectedIndex = index;
			this.SelectedItem = this.m_items[index];
			TasksItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(index);
			if (selectedIndex != index)
			{
				if (itemViewsHolderIfVisible != null)
				{
					Action<BulletinBoard> selectionChanged = this.SelectionChanged;
					if (selectionChanged != null)
					{
						selectionChanged(itemViewsHolderIfVisible.Data);
					}
				}
				if (selectedIndex != -1)
				{
					TasksItemViewsHolder itemViewsHolderIfVisible2 = base.GetItemViewsHolderIfVisible(selectedIndex);
					if (itemViewsHolderIfVisible2 == null)
					{
						return;
					}
					itemViewsHolderIfVisible2.ListItem.RefreshVisuals();
				}
			}
		}

		// Token: 0x060045B3 RID: 17843 RVA: 0x001A10B4 File Offset: 0x0019F2B4
		public void DeselectAll(bool suppressEvents = false)
		{
			int selectedIndex = this.SelectedIndex;
			this.SelectedIndex = -1;
			this.SelectedItem = null;
			if (selectedIndex != -1 && !suppressEvents)
			{
				Action<BulletinBoard> selectionChanged = this.SelectionChanged;
				if (selectionChanged != null)
				{
					selectionChanged(null);
				}
			}
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				TasksItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.RefreshVisuals();
				}
			}
		}

		// Token: 0x060045B4 RID: 17844 RVA: 0x001A1118 File Offset: 0x0019F318
		public void ReindexItems(BulletinBoard selectedItem)
		{
			this.SelectedIndex = -1;
			this.SelectedItem = null;
			if (selectedItem != null)
			{
				for (int i = 0; i < this.m_items.Count; i++)
				{
					if (this.m_items[i].Id == selectedItem.Id)
					{
						this.SelectedIndex = i;
						this.SelectedItem = this.m_items[i];
					}
				}
			}
			for (int j = 0; j < this.GetItemsCount(); j++)
			{
				TasksItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(j);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.Reindex(j);
				}
			}
		}

		// Token: 0x060045B5 RID: 17845 RVA: 0x0006EE10 File Offset: 0x0006D010
		public void Sort(List<BulletinBoard> unsorted)
		{
			unsorted.Sort((BulletinBoard x, BulletinBoard y) => x.Title.CompareTo(y.Title));
		}

		// Token: 0x060045B6 RID: 17846 RVA: 0x0006EE37 File Offset: 0x0006D037
		protected override TasksItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			TasksItemViewsHolder tasksItemViewsHolder = new TasksItemViewsHolder();
			tasksItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return tasksItemViewsHolder;
		}

		// Token: 0x060045B7 RID: 17847 RVA: 0x0006EE5D File Offset: 0x0006D05D
		protected override void UpdateViewsHolder(TasksItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateItem(this, this.m_items[newOrRecycled.ItemIndex]);
		}

		// Token: 0x040041FF RID: 16895
		private List<BulletinBoard> m_items;
	}
}
