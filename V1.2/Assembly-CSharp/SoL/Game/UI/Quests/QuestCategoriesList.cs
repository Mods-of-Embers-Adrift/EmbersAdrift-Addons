using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using SoL.Game.Quests;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.UI.Quests
{
	// Token: 0x02000945 RID: 2373
	public class QuestCategoriesList : OSA<BaseParamsWithPrefab, QuestCategoriesItemViewsHolder>
	{
		// Token: 0x17000FA4 RID: 4004
		// (get) Token: 0x06004607 RID: 17927 RVA: 0x0006F1A9 File Offset: 0x0006D3A9
		public List<bool> Expanded
		{
			get
			{
				return this.m_expanded;
			}
		}

		// Token: 0x17000FA5 RID: 4005
		// (get) Token: 0x06004608 RID: 17928 RVA: 0x001A20F8 File Offset: 0x001A02F8
		public bool IsFullyInitialized
		{
			get
			{
				if (!base.IsInitialized)
				{
					this.Initialized += this.OnListInitialized;
					return false;
				}
				for (int i = 0; i < this.GetItemsCount(); i++)
				{
					QuestCategoriesItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
					if (itemViewsHolderIfVisible != null && !itemViewsHolderIfVisible.ListItem.IsListInitialized)
					{
						return false;
					}
				}
				return true;
			}
		}

		// Token: 0x17000FA6 RID: 4006
		// (get) Token: 0x06004609 RID: 17929 RVA: 0x0006F1B1 File Offset: 0x0006D3B1
		// (set) Token: 0x0600460A RID: 17930 RVA: 0x0006F1B9 File Offset: 0x0006D3B9
		public int SelectedIndex { get; private set; } = -1;

		// Token: 0x17000FA7 RID: 4007
		// (get) Token: 0x0600460B RID: 17931 RVA: 0x0006F1C2 File Offset: 0x0006D3C2
		// (set) Token: 0x0600460C RID: 17932 RVA: 0x0006F1CA File Offset: 0x0006D3CA
		public Quest SelectedItem { get; private set; }

		// Token: 0x140000D3 RID: 211
		// (add) Token: 0x0600460D RID: 17933 RVA: 0x001A2150 File Offset: 0x001A0350
		// (remove) Token: 0x0600460E RID: 17934 RVA: 0x001A2188 File Offset: 0x001A0388
		public event Action<Category<Quest>, Quest> SelectionChanged;

		// Token: 0x140000D4 RID: 212
		// (add) Token: 0x0600460F RID: 17935 RVA: 0x001A21C0 File Offset: 0x001A03C0
		// (remove) Token: 0x06004610 RID: 17936 RVA: 0x001A21F8 File Offset: 0x001A03F8
		public event Action FullyInitialized;

		// Token: 0x06004611 RID: 17937 RVA: 0x001A2230 File Offset: 0x001A0430
		public void UpdateCategories(List<Category<Quest>> items)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.m_items == null)
			{
				this.m_items = new List<Category<Quest>>(items.Count);
				this.m_expanded = new List<bool>(items.Count);
			}
			foreach (Category<Quest> category in this.m_items)
			{
				StaticListPool<Quest>.ReturnToPool(category.Data);
			}
			this.m_items.Clear();
			this.m_expanded.Clear();
			for (int i = 0; i < items.Count; i++)
			{
				this.m_items.Add(new Category<Quest>
				{
					Name = items[i].Name,
					Data = StaticListPool<Quest>.GetFromPool()
				});
				this.m_items[i].Data.AddRange(items[i].Data);
				this.m_expanded.Add(PlayerPrefs.GetInt(this.PlayerPrefsKey + "_" + items[i].Name + "_Expanded", 1) != 0);
			}
			this.ResetItems(this.m_items.Count, false, false);
			for (int j = 0; j < this.GetItemsCount(); j++)
			{
				QuestCategoriesItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(j);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.Refresh(this.m_items[j].Data);
				}
			}
		}

		// Token: 0x06004612 RID: 17938 RVA: 0x001A23C4 File Offset: 0x001A05C4
		public void UpdateCategory(int index, List<Quest> items, bool expand = false)
		{
			if (!base.gameObject.activeInHierarchy || this.m_items == null || index < 0 || index >= this.m_items.Count)
			{
				return;
			}
			this.m_items[index].Data.Clear();
			this.m_items[index].Data.AddRange(items);
			if (expand)
			{
				this.m_expanded[index] = true;
			}
			this.ResetItems(this.m_items.Count, false, false);
			QuestCategoriesItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(index);
			if (itemViewsHolderIfVisible == null)
			{
				return;
			}
			itemViewsHolderIfVisible.ListItem.Refresh(this.m_items[index].Data);
		}

		// Token: 0x06004613 RID: 17939 RVA: 0x001A2470 File Offset: 0x001A0670
		public void Select(int categoryIndex, Quest item)
		{
			int selectedIndex = this.SelectedIndex;
			this.SelectedIndex = categoryIndex;
			this.SelectedItem = item;
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				QuestCategoriesItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (i == categoryIndex)
				{
					if (itemViewsHolderIfVisible != null)
					{
						Action<Category<Quest>, Quest> selectionChanged = this.SelectionChanged;
						if (selectionChanged != null)
						{
							selectionChanged(itemViewsHolderIfVisible.Data, item);
						}
					}
				}
				else if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.Deselect(true);
				}
			}
		}

		// Token: 0x06004614 RID: 17940 RVA: 0x001A24DC File Offset: 0x001A06DC
		public void DeselectAll(bool suppressEvents = false)
		{
			int selectedIndex = this.SelectedIndex;
			this.SelectedIndex = -1;
			this.SelectedItem = null;
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				QuestCategoriesItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.Deselect(suppressEvents);
				}
			}
		}

		// Token: 0x06004615 RID: 17941 RVA: 0x001A2528 File Offset: 0x001A0728
		public int FindCategoryForItem(Quest itemToFind)
		{
			for (int i = 0; i < this.m_items.Count; i++)
			{
				using (List<Quest>.Enumerator enumerator = this.m_items[i].Data.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Id == itemToFind.Id)
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		// Token: 0x06004616 RID: 17942 RVA: 0x001A25B0 File Offset: 0x001A07B0
		public void ReindexItems(Quest selectedItem)
		{
			if (selectedItem != null)
			{
				int selectedIndex = this.FindCategoryForItem(selectedItem);
				this.SelectedIndex = selectedIndex;
			}
			else
			{
				this.SelectedIndex = -1;
			}
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				QuestCategoriesItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.Reindex(i, selectedItem);
				}
			}
		}

		// Token: 0x06004617 RID: 17943 RVA: 0x0006F1D3 File Offset: 0x0006D3D3
		public void ReindexInPlace()
		{
			this.ReindexItems(this.SelectedItem);
		}

		// Token: 0x06004618 RID: 17944 RVA: 0x0006F1E1 File Offset: 0x0006D3E1
		protected override QuestCategoriesItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			QuestCategoriesItemViewsHolder questCategoriesItemViewsHolder = new QuestCategoriesItemViewsHolder();
			questCategoriesItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return questCategoriesItemViewsHolder;
		}

		// Token: 0x06004619 RID: 17945 RVA: 0x0006F207 File Offset: 0x0006D407
		protected override void UpdateViewsHolder(QuestCategoriesItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateItem(this, this.m_items[newOrRecycled.ItemIndex]);
		}

		// Token: 0x0600461A RID: 17946 RVA: 0x001A2608 File Offset: 0x001A0808
		protected override void CollectItemsSizes(ItemCountChangeMode changeMode, int count, int indexIfInsertingOrRemoving, ItemsDescriptor itemsDesc)
		{
			base.CollectItemsSizes(changeMode, count, indexIfInsertingOrRemoving, itemsDesc);
			if (changeMode != ItemCountChangeMode.RESET)
			{
				return;
			}
			if (count == 0)
			{
				return;
			}
			int num = 0;
			int num2 = num + count;
			itemsDesc.BeginChangingItemsSizes(num);
			for (int i = num; i < num2; i++)
			{
				itemsDesc[i] = (double)(this.m_expanded[i] ? ((float)this.m_items[i].Data.Count * 20f + 20f) : 20f);
			}
			itemsDesc.EndChangingItemsSizes();
		}

		// Token: 0x0600461B RID: 17947 RVA: 0x0006F221 File Offset: 0x0006D421
		public void OnListInitialized()
		{
			this.Initialized -= this.OnListInitialized;
			if (this.IsFullyInitialized)
			{
				Action fullyInitialized = this.FullyInitialized;
				if (fullyInitialized == null)
				{
					return;
				}
				fullyInitialized();
			}
		}

		// Token: 0x0400423D RID: 16957
		private List<Category<Quest>> m_items;

		// Token: 0x0400423E RID: 16958
		private List<bool> m_expanded;

		// Token: 0x0400423F RID: 16959
		public string PlayerPrefsKey = string.Empty;
	}
}
