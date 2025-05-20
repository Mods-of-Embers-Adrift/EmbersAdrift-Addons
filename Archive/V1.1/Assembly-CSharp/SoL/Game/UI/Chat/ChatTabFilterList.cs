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
		// (set) Token: 0x06004A55 RID: 19029 RVA: 0x001B2F9C File Offset: 0x001B119C
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
		// (add) Token: 0x06004A56 RID: 19030 RVA: 0x001B2FE4 File Offset: 0x001B11E4
		// (remove) Token: 0x06004A57 RID: 19031 RVA: 0x001B301C File Offset: 0x001B121C
		public event Action<int> FilterChanged;

		// Token: 0x06004A58 RID: 19032 RVA: 0x001B3054 File Offset: 0x001B1254
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

		// Token: 0x06004A59 RID: 19033 RVA: 0x001B3118 File Offset: 0x001B1318
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

		// Token: 0x06004A5A RID: 19034 RVA: 0x001B31DC File Offset: 0x001B13DC
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
