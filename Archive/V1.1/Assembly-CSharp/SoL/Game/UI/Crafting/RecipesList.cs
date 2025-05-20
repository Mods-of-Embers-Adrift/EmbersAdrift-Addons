using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using SoL.Game.Objects.Archetypes;

namespace SoL.Game.UI.Crafting
{
	// Token: 0x0200099C RID: 2460
	public class RecipesList : OSA<BaseParamsWithPrefab, RecipeItemViewsHolder>
	{
		// Token: 0x17001049 RID: 4169
		// (get) Token: 0x06004996 RID: 18838 RVA: 0x000716EC File Offset: 0x0006F8EC
		// (set) Token: 0x06004997 RID: 18839 RVA: 0x000716F4 File Offset: 0x0006F8F4
		public int SelectedIndex { get; private set; }

		// Token: 0x140000E4 RID: 228
		// (add) Token: 0x06004998 RID: 18840 RVA: 0x001B0220 File Offset: 0x001AE420
		// (remove) Token: 0x06004999 RID: 18841 RVA: 0x001B0258 File Offset: 0x001AE458
		public event Action<Recipe> SelectionChanged;

		// Token: 0x0600499A RID: 18842 RVA: 0x001B0290 File Offset: 0x001AE490
		public void UpdateItems(ICollection<Recipe> items)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.m_items == null)
			{
				this.m_items = new List<Recipe>(items.Count);
			}
			this.m_items.Clear();
			this.m_items.AddRange(items);
			this.ResetItems(this.m_items.Count, false, false);
		}

		// Token: 0x0600499B RID: 18843 RVA: 0x001B02F0 File Offset: 0x001AE4F0
		public void Select(int index)
		{
			int selectedIndex = this.SelectedIndex;
			this.SelectedIndex = index;
			RecipeItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(index);
			if (itemViewsHolderIfVisible != null && selectedIndex != index)
			{
				Action<Recipe> selectionChanged = this.SelectionChanged;
				if (selectionChanged == null)
				{
					return;
				}
				selectionChanged(itemViewsHolderIfVisible.Data);
			}
		}

		// Token: 0x0600499C RID: 18844 RVA: 0x000716FD File Offset: 0x0006F8FD
		public void DeselectAll()
		{
			int selectedIndex = this.SelectedIndex;
			this.SelectedIndex = -1;
			if (selectedIndex != -1)
			{
				Action<Recipe> selectionChanged = this.SelectionChanged;
				if (selectionChanged == null)
				{
					return;
				}
				selectionChanged(null);
			}
		}

		// Token: 0x0600499D RID: 18845 RVA: 0x001B0330 File Offset: 0x001AE530
		public void ReindexItems(Recipe selectedRecipe)
		{
			if (selectedRecipe != null)
			{
				for (int i = 0; i < this.m_items.Count; i++)
				{
					if (this.m_items[i].Id == selectedRecipe.Id)
					{
						this.SelectedIndex = i;
					}
				}
			}
			else
			{
				this.SelectedIndex = -1;
			}
			for (int j = 0; j < this.GetItemsCount(); j++)
			{
				RecipeItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(j);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.Reindex(j);
				}
			}
		}

		// Token: 0x0600499E RID: 18846 RVA: 0x001B03B4 File Offset: 0x001AE5B4
		public void LockUI()
		{
			this.StopScrollingIfAny();
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				RecipeItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.LockUI();
				}
			}
		}

		// Token: 0x0600499F RID: 18847 RVA: 0x001B03F0 File Offset: 0x001AE5F0
		public void UnlockUI()
		{
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				RecipeItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.UnlockUI();
				}
			}
		}

		// Token: 0x060049A0 RID: 18848 RVA: 0x001B0428 File Offset: 0x001AE628
		public void OnInventoryChanged()
		{
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				RecipeItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.OnInventoryChanged();
				}
			}
		}

		// Token: 0x060049A1 RID: 18849 RVA: 0x00071720 File Offset: 0x0006F920
		protected override RecipeItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			RecipeItemViewsHolder recipeItemViewsHolder = new RecipeItemViewsHolder();
			recipeItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return recipeItemViewsHolder;
		}

		// Token: 0x060049A2 RID: 18850 RVA: 0x00071746 File Offset: 0x0006F946
		protected override void UpdateViewsHolder(RecipeItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateItem(this, this.m_items[newOrRecycled.ItemIndex]);
		}

		// Token: 0x040044A0 RID: 17568
		private List<Recipe> m_items;
	}
}
