using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.CustomAdapters.GridView;

namespace SoL.Game.UI.Chat
{
	// Token: 0x020009AC RID: 2476
	public class ChatTabFilterList : GridAdapter<GridParams, ChatTabFilterListItemViewsHolder>
	{
		// Token: 0x1700106D RID: 4205
		// (get) Token: 0x06004A54 RID: 19028 RVA: 0x0007210E File Offset: 0x0007030E
		// (set) Token: 0x06004A55 RID: 19029 RVA: 0x001B2FF8 File Offset: 0x001B11F8
		public int Filter
		{
			get
			{
				return this.m_filter;
			}
			set
			{
				this.m_filter = value;
				for (int i = 0; i < this.GetNumVisibleCells(); i++)
				{
					ChatTabFilterListItemViewsHolder cellViewsHolder = this.GetCellViewsHolder(i);
					if (cellViewsHolder != null)
					{
						ChatTabFilterListItem listItem = cellViewsHolder.ListItem;
						if (listItem != null)
						{
							listItem.UpdateToggle(this.m_filter);
						}
					}
				}
			}
		}

		// Token: 0x140000EB RID: 235
		// (add) Token: 0x06004A56 RID: 19030 RVA: 0x001B3040 File Offset: 0x001B1240
		// (remove) Token: 0x06004A57 RID: 19031 RVA: 0x001B3078 File Offset: 0x001B1278
		public event Action<int> FilterChanged;

		// Token: 0x06004A58 RID: 19032 RVA: 0x001B30B0 File Offset: 0x001B12B0
		public void UpdateItems(ICollection<ChatFilter> items)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.m_items == null)
			{
				this.m_items = new List<int>(items.Count);
			}
			this.m_items.Clear();
			foreach (ChatFilter item in items)
			{
				this.m_items.Add((int)item);
			}
			this.Mode = ChatTabMode.Chat;
			this.ResetItems(this.m_items.Count, false, false);
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				ChatTabFilterListItemViewsHolder cellViewsHolderIfVisible = this.GetCellViewsHolderIfVisible(i);
				if (cellViewsHolderIfVisible != null)
				{
					cellViewsHolderIfVisible.ListItem.Refresh();
				}
			}
		}

		// Token: 0x06004A59 RID: 19033 RVA: 0x001B3174 File Offset: 0x001B1374
		public void UpdateItems(ICollection<CombatFilter> items)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.m_items == null)
			{
				this.m_items = new List<int>(items.Count);
			}
			this.m_items.Clear();
			foreach (CombatFilter item in items)
			{
				this.m_items.Add((int)item);
			}
			this.Mode = ChatTabMode.Combat;
			this.ResetItems(this.m_items.Count, false, false);
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				ChatTabFilterListItemViewsHolder cellViewsHolderIfVisible = this.GetCellViewsHolderIfVisible(i);
				if (cellViewsHolderIfVisible != null)
				{
					cellViewsHolderIfVisible.ListItem.Refresh();
				}
			}
		}

		// Token: 0x06004A5A RID: 19034 RVA: 0x001B3238 File Offset: 0x001B1438
		public void InternalFilterUpdate(int type, bool isOn)
		{
			if (isOn)
			{
				this.m_filter = (this.m_filter |= type);
			}
			else
			{
				this.m_filter = (this.m_filter &= ~type);
			}
			this.Filter = this.m_filter;
			Action<int> filterChanged = this.FilterChanged;
			if (filterChanged == null)
			{
				return;
			}
			filterChanged(this.m_filter);
		}

		// Token: 0x06004A5B RID: 19035 RVA: 0x00072116 File Offset: 0x00070316
		protected override void UpdateCellViewsHolder(ChatTabFilterListItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateItem(this, this.m_items[newOrRecycled.ItemIndex], this.m_filter);
		}

		// Token: 0x0400452C RID: 17708
		private List<int> m_items;

		// Token: 0x0400452D RID: 17709
		private int m_filter;

		// Token: 0x0400452E RID: 17710
		public ChatTabMode Mode;
	}
}
