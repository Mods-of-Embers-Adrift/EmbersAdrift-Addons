using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using SoL.Networking.Database;

namespace SoL.Game.UI.Social
{
	// Token: 0x02000902 RID: 2306
	public class GuildRankList : OSA<BaseParamsWithPrefab, GuildRankListItemViewsHolder>
	{
		// Token: 0x140000C8 RID: 200
		// (add) Token: 0x060043B0 RID: 17328 RVA: 0x00197484 File Offset: 0x00195684
		// (remove) Token: 0x060043B1 RID: 17329 RVA: 0x001974BC File Offset: 0x001956BC
		public event Action<string> RankEditOpenRequested;

		// Token: 0x060043B2 RID: 17330 RVA: 0x0006DBD4 File Offset: 0x0006BDD4
		public void OpenRankEdit(string rankId)
		{
			Action<string> rankEditOpenRequested = this.RankEditOpenRequested;
			if (rankEditOpenRequested == null)
			{
				return;
			}
			rankEditOpenRequested(rankId);
		}

		// Token: 0x060043B3 RID: 17331 RVA: 0x001974F4 File Offset: 0x001956F4
		public void UpdateItems(ICollection<GuildRank> items)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.m_items == null)
			{
				this.m_items = new List<GuildRank>(items.Count);
			}
			this.m_items.Clear();
			this.m_items.AddRange(items);
			this.ResetItems(this.m_items.Count, false, false);
		}

		// Token: 0x060043B4 RID: 17332 RVA: 0x0006DBE7 File Offset: 0x0006BDE7
		protected override GuildRankListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			GuildRankListItemViewsHolder guildRankListItemViewsHolder = new GuildRankListItemViewsHolder();
			guildRankListItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return guildRankListItemViewsHolder;
		}

		// Token: 0x060043B5 RID: 17333 RVA: 0x0006DC0D File Offset: 0x0006BE0D
		protected override void UpdateViewsHolder(GuildRankListItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateItem(this, this.m_items[newOrRecycled.ItemIndex]);
		}

		// Token: 0x060043B6 RID: 17334 RVA: 0x00197554 File Offset: 0x00195754
		public void Reindex()
		{
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				GuildRankListItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.Init(this, i, this.m_items[i]);
				}
			}
		}

		// Token: 0x04004034 RID: 16436
		private List<GuildRank> m_items;
	}
}
