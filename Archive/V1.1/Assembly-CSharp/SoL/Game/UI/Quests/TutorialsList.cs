using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using SoL.Game.Notifications;

namespace SoL.Game.UI.Quests
{
	// Token: 0x02000954 RID: 2388
	public class TutorialsList : OSA<BaseParamsWithPrefab, TutorialsItemViewsHolder>, ICategorizedList<BaseNotification>
	{
		// Token: 0x17000FBE RID: 4030
		// (get) Token: 0x060046C1 RID: 18113 RVA: 0x0006FA4B File Offset: 0x0006DC4B
		// (set) Token: 0x060046C2 RID: 18114 RVA: 0x0006FA53 File Offset: 0x0006DC53
		public int SelectedIndex { get; private set; } = -1;

		// Token: 0x140000DB RID: 219
		// (add) Token: 0x060046C3 RID: 18115 RVA: 0x001A5194 File Offset: 0x001A3394
		// (remove) Token: 0x060046C4 RID: 18116 RVA: 0x001A51CC File Offset: 0x001A33CC
		public event Action<BaseNotification> SelectionChanged;

		// Token: 0x060046C5 RID: 18117 RVA: 0x001A5204 File Offset: 0x001A3404
		public void UpdateItems(ICollection<BaseNotification> items)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.m_items == null)
			{
				this.m_items = new List<BaseNotification>(items.Count);
			}
			this.m_items.Clear();
			this.m_items.AddRange(items);
			this.ResetItems(this.m_items.Count, false, false);
		}

		// Token: 0x060046C6 RID: 18118 RVA: 0x001A5264 File Offset: 0x001A3464
		public void OnSelect(int index)
		{
			int selectedIndex = this.SelectedIndex;
			this.SelectedIndex = index;
			TutorialsItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(index);
			if (selectedIndex != index)
			{
				Action<BaseNotification> selectionChanged = this.SelectionChanged;
				if (selectionChanged != null)
				{
					selectionChanged(this.m_items[index]);
				}
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.RefreshVisuals();
				}
				if (selectedIndex != -1)
				{
					TutorialsItemViewsHolder itemViewsHolderIfVisible2 = base.GetItemViewsHolderIfVisible(selectedIndex);
					if (itemViewsHolderIfVisible2 == null)
					{
						return;
					}
					itemViewsHolderIfVisible2.ListItem.RefreshVisuals();
				}
			}
		}

		// Token: 0x060046C7 RID: 18119 RVA: 0x001A52D0 File Offset: 0x001A34D0
		public void Select(BaseNotification item)
		{
			for (int i = 0; i < this.m_items.Count; i++)
			{
				if (this.m_items[i] == item)
				{
					this.OnSelect(i);
				}
			}
		}

		// Token: 0x060046C8 RID: 18120 RVA: 0x001A5310 File Offset: 0x001A3510
		public void DeselectAll(bool suppressEvents = false)
		{
			int selectedIndex = this.SelectedIndex;
			this.SelectedIndex = -1;
			if (selectedIndex != -1 && !suppressEvents)
			{
				Action<BaseNotification> selectionChanged = this.SelectionChanged;
				if (selectionChanged != null)
				{
					selectionChanged(null);
				}
			}
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				TutorialsItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.RefreshVisuals();
				}
			}
		}

		// Token: 0x060046C9 RID: 18121 RVA: 0x001A536C File Offset: 0x001A356C
		public void ReindexItems(BaseNotification selectedItem)
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
				TutorialsItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(j);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.Reindex(j);
				}
			}
		}

		// Token: 0x060046CA RID: 18122 RVA: 0x0006FA5C File Offset: 0x0006DC5C
		public void Sort(List<BaseNotification> unsorted)
		{
			unsorted.Sort((BaseNotification x, BaseNotification y) => x.Sort.CompareTo(y.Sort));
		}

		// Token: 0x060046CB RID: 18123 RVA: 0x0006FA83 File Offset: 0x0006DC83
		protected override TutorialsItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			TutorialsItemViewsHolder tutorialsItemViewsHolder = new TutorialsItemViewsHolder();
			tutorialsItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return tutorialsItemViewsHolder;
		}

		// Token: 0x060046CC RID: 18124 RVA: 0x0006FAA9 File Offset: 0x0006DCA9
		protected override void UpdateViewsHolder(TutorialsItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateItem(this, this.m_items[newOrRecycled.ItemIndex]);
		}

		// Token: 0x040042B9 RID: 17081
		private List<BaseNotification> m_items;
	}
}
