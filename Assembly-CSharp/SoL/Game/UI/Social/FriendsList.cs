using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using SoL.Networking.Database;

namespace SoL.Game.UI.Social
{
	// Token: 0x020008F5 RID: 2293
	public class FriendsList : OSA<BaseParamsWithPrefab, FriendsItemViewsHolder>
	{
		// Token: 0x0600433C RID: 17212 RVA: 0x0006D599 File Offset: 0x0006B799
		protected override void Start()
		{
			base.Start();
		}

		// Token: 0x0600433D RID: 17213 RVA: 0x0019539C File Offset: 0x0019359C
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

		// Token: 0x0600433E RID: 17214 RVA: 0x0006D5A1 File Offset: 0x0006B7A1
		protected override FriendsItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			FriendsItemViewsHolder friendsItemViewsHolder = new FriendsItemViewsHolder();
			friendsItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return friendsItemViewsHolder;
		}

		// Token: 0x0600433F RID: 17215 RVA: 0x0006D5C7 File Offset: 0x0006B7C7
		protected override void UpdateViewsHolder(FriendsItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateItem(this.m_items[newOrRecycled.ItemIndex]);
		}

		// Token: 0x06004340 RID: 17216 RVA: 0x001953FC File Offset: 0x001935FC
		public void Reindex()
		{
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				FriendsItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.Init(this.m_items[i]);
				}
			}
		}

		// Token: 0x04003FE1 RID: 16353
		private List<Relation> m_items;
	}
}
