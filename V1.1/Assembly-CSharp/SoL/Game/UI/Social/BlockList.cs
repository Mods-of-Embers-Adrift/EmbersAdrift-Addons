using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using SoL.Networking.Database;

namespace SoL.Game.UI.Social
{
	// Token: 0x020008F2 RID: 2290
	public class BlockList : OSA<BaseParamsWithPrefab, BlockItemViewsHolder>
	{
		// Token: 0x06004329 RID: 17193 RVA: 0x0006D49E File Offset: 0x0006B69E
		protected override void Start()
		{
			base.Start();
		}

		// Token: 0x0600432A RID: 17194 RVA: 0x00195230 File Offset: 0x00193430
		public void UpdateItems(ICollection<Relation> items)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.m_items == null)
			{
				this.m_items = new List<Relation>(items.Count);
			}
			this.m_items.Clear();
			this.m_items.AddRange(items);
			this.ResetItems(this.m_items.Count, false, false);
		}

		// Token: 0x0600432B RID: 17195 RVA: 0x0006D4A6 File Offset: 0x0006B6A6
		protected override BlockItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			BlockItemViewsHolder blockItemViewsHolder = new BlockItemViewsHolder();
			blockItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return blockItemViewsHolder;
		}

		// Token: 0x0600432C RID: 17196 RVA: 0x0006D4CC File Offset: 0x0006B6CC
		protected override void UpdateViewsHolder(BlockItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateItem(this.m_items[newOrRecycled.ItemIndex]);
		}

		// Token: 0x0600432D RID: 17197 RVA: 0x00195290 File Offset: 0x00193490
		public void Reindex()
		{
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				BlockItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.Init(this.m_items[i]);
				}
			}
		}

		// Token: 0x04003FDA RID: 16346
		private List<Relation> m_items;
	}
}
