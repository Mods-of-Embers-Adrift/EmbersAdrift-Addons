using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using SoL.Game.Grouping;

namespace SoL.Game.UI.Social
{
	// Token: 0x02000909 RID: 2313
	public class LfgLfmList : OSA<BaseParamsWithPrefab, LfgLfmItemViewsHolder>
	{
		// Token: 0x060043F8 RID: 17400 RVA: 0x0006DE9C File Offset: 0x0006C09C
		protected override void Start()
		{
			base.Start();
		}

		// Token: 0x060043F9 RID: 17401 RVA: 0x00198FC4 File Offset: 0x001971C4
		public void UpdateItems(LookingFor[] items)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.m_items == null)
			{
				this.m_items = new List<LookingFor>(items.Length);
			}
			this.m_items.Clear();
			this.m_items.AddRange(items);
			this.ResetItems(this.m_items.Count, false, false);
		}

		// Token: 0x060043FA RID: 17402 RVA: 0x0006DEA4 File Offset: 0x0006C0A4
		protected override LfgLfmItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			LfgLfmItemViewsHolder lfgLfmItemViewsHolder = new LfgLfmItemViewsHolder();
			lfgLfmItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return lfgLfmItemViewsHolder;
		}

		// Token: 0x060043FB RID: 17403 RVA: 0x0006DECA File Offset: 0x0006C0CA
		protected override void UpdateViewsHolder(LfgLfmItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateItem(this.m_items[newOrRecycled.ItemIndex]);
		}

		// Token: 0x060043FC RID: 17404 RVA: 0x00199020 File Offset: 0x00197220
		public void Reindex()
		{
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				LfgLfmItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.Init(this.m_items[i]);
				}
			}
		}

		// Token: 0x04004079 RID: 16505
		private List<LookingFor> m_items;
	}
}
