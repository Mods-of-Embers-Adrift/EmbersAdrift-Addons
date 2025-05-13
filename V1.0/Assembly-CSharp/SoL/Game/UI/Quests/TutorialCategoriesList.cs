using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using SoL.Game.Notifications;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.UI.Quests
{
	// Token: 0x02000951 RID: 2385
	public class TutorialCategoriesList : OSA<BaseParamsWithPrefab, TutorialCategoriesItemViewsHolder>
	{
		// Token: 0x17000FB6 RID: 4022
		// (get) Token: 0x06004690 RID: 18064 RVA: 0x0006F818 File Offset: 0x0006DA18
		public List<bool> Expanded
		{
			get
			{
				return this.m_expanded;
			}
		}

		// Token: 0x17000FB7 RID: 4023
		// (get) Token: 0x06004691 RID: 18065 RVA: 0x001A48C8 File Offset: 0x001A2AC8
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
					TutorialCategoriesItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
					if (itemViewsHolderIfVisible != null && !itemViewsHolderIfVisible.ListItem.IsListInitialized)
					{
						return false;
					}
				}
				return true;
			}
		}

		// Token: 0x17000FB8 RID: 4024
		// (get) Token: 0x06004692 RID: 18066 RVA: 0x0006F820 File Offset: 0x0006DA20
		// (set) Token: 0x06004693 RID: 18067 RVA: 0x0006F828 File Offset: 0x0006DA28
		public int SelectedIndex { get; private set; } = -1;

		// Token: 0x17000FB9 RID: 4025
		// (get) Token: 0x06004694 RID: 18068 RVA: 0x0006F831 File Offset: 0x0006DA31
		// (set) Token: 0x06004695 RID: 18069 RVA: 0x0006F839 File Offset: 0x0006DA39
		public BaseNotification SelectedItem { get; private set; }

		// Token: 0x140000D9 RID: 217
		// (add) Token: 0x06004696 RID: 18070 RVA: 0x001A4920 File Offset: 0x001A2B20
		// (remove) Token: 0x06004697 RID: 18071 RVA: 0x001A4958 File Offset: 0x001A2B58
		public event Action<Category<BaseNotification>, BaseNotification> SelectionChanged;

		// Token: 0x140000DA RID: 218
		// (add) Token: 0x06004698 RID: 18072 RVA: 0x001A4990 File Offset: 0x001A2B90
		// (remove) Token: 0x06004699 RID: 18073 RVA: 0x001A49C8 File Offset: 0x001A2BC8
		public event Action FullyInitialized;

		// Token: 0x0600469A RID: 18074 RVA: 0x001A4A00 File Offset: 0x001A2C00
		public void UpdateCategories(List<Category<BaseNotification>> items)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.m_items == null)
			{
				this.m_items = new List<Category<BaseNotification>>(items.Count);
				this.m_expanded = new List<bool>(items.Count);
			}
			foreach (Category<BaseNotification> category in this.m_items)
			{
				StaticListPool<BaseNotification>.ReturnToPool(category.Data);
			}
			this.m_items.Clear();
			this.m_expanded.Clear();
			for (int i = 0; i < items.Count; i++)
			{
				this.m_items.Add(new Category<BaseNotification>
				{
					Name = items[i].Name,
					Data = StaticListPool<BaseNotification>.GetFromPool()
				});
				this.m_items[i].Data.AddRange(items[i].Data);
				this.m_expanded.Add(PlayerPrefs.GetInt(this.PlayerPrefsKey + "_" + items[i].Name + "_Expanded", 1) != 0);
			}
			this.ResetItems(this.m_items.Count, false, false);
			for (int j = 0; j < this.GetItemsCount(); j++)
			{
				TutorialCategoriesItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(j);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.Refresh();
				}
			}
		}

		// Token: 0x0600469B RID: 18075 RVA: 0x001A4B84 File Offset: 0x001A2D84
		public void UpdateCategory(int index, List<BaseNotification> items, bool expand = false)
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
			TutorialCategoriesItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(index);
			if (itemViewsHolderIfVisible == null)
			{
				return;
			}
			itemViewsHolderIfVisible.ListItem.Refresh();
		}

		// Token: 0x0600469C RID: 18076 RVA: 0x001A4C20 File Offset: 0x001A2E20
		public void OnSelectionChanged(int categoryIndex, BaseNotification item)
		{
			int selectedIndex = this.SelectedIndex;
			this.SelectedIndex = categoryIndex;
			this.SelectedItem = item;
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				TutorialCategoriesItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (i == categoryIndex)
				{
					if (itemViewsHolderIfVisible != null)
					{
						Action<Category<BaseNotification>, BaseNotification> selectionChanged = this.SelectionChanged;
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

		// Token: 0x0600469D RID: 18077 RVA: 0x001A4C8C File Offset: 0x001A2E8C
		public void Select(BaseNotification item)
		{
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				TutorialCategoriesItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (this.m_items[i].Data.Contains(item))
				{
					if (itemViewsHolderIfVisible != null)
					{
						itemViewsHolderIfVisible.ListItem.Expand();
					}
					if (itemViewsHolderIfVisible != null)
					{
						itemViewsHolderIfVisible.ListItem.Select(item);
					}
				}
				else if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.Deselect(true);
				}
			}
		}

		// Token: 0x0600469E RID: 18078 RVA: 0x001A4CFC File Offset: 0x001A2EFC
		public void DeselectAll(bool suppressEvents = false)
		{
			this.SelectedIndex = -1;
			this.SelectedItem = null;
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				TutorialCategoriesItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.Deselect(suppressEvents);
				}
			}
		}

		// Token: 0x0600469F RID: 18079 RVA: 0x001A4D40 File Offset: 0x001A2F40
		public int FindCategoryForItem(BaseNotification itemToFind)
		{
			for (int i = 0; i < this.m_items.Count; i++)
			{
				using (List<BaseNotification>.Enumerator enumerator = this.m_items[i].Data.GetEnumerator())
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

		// Token: 0x060046A0 RID: 18080 RVA: 0x001A4DC8 File Offset: 0x001A2FC8
		public void ReindexItems(BaseNotification selectedItem)
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
				TutorialCategoriesItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.Reindex(i, selectedItem);
				}
			}
		}

		// Token: 0x060046A1 RID: 18081 RVA: 0x0006F842 File Offset: 0x0006DA42
		public void ReindexInPlace()
		{
			this.ReindexItems(this.SelectedItem);
		}

		// Token: 0x060046A2 RID: 18082 RVA: 0x0006F850 File Offset: 0x0006DA50
		protected override TutorialCategoriesItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			TutorialCategoriesItemViewsHolder tutorialCategoriesItemViewsHolder = new TutorialCategoriesItemViewsHolder();
			tutorialCategoriesItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return tutorialCategoriesItemViewsHolder;
		}

		// Token: 0x060046A3 RID: 18083 RVA: 0x0006F876 File Offset: 0x0006DA76
		protected override void UpdateViewsHolder(TutorialCategoriesItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateItem(this, this.m_items[newOrRecycled.ItemIndex]);
		}

		// Token: 0x060046A4 RID: 18084 RVA: 0x001A4E20 File Offset: 0x001A3020
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

		// Token: 0x060046A5 RID: 18085 RVA: 0x0006F890 File Offset: 0x0006DA90
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

		// Token: 0x040042A2 RID: 17058
		private List<Category<BaseNotification>> m_items;

		// Token: 0x040042A3 RID: 17059
		private List<bool> m_expanded;

		// Token: 0x040042A4 RID: 17060
		public string PlayerPrefsKey = string.Empty;
	}
}
