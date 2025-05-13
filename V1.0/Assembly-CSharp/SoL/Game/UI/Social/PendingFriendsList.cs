using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using SoL.Networking.Database;

namespace SoL.Game.UI.Social
{
	// Token: 0x02000917 RID: 2327
	public class PendingFriendsList : OSA<BaseParamsWithPrefab, PendingFriendsItemViewsHolder>
	{
		// Token: 0x06004491 RID: 17553 RVA: 0x0006E512 File Offset: 0x0006C712
		protected override void Start()
		{
			base.Start();
		}

		// Token: 0x06004492 RID: 17554 RVA: 0x0019CD08 File Offset: 0x0019AF08
		public void UpdateItems(List<Mail> items)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.m_items == null)
			{
				this.m_items = new List<Mail>(items.Count);
			}
			this.m_items.Clear();
			this.m_items.AddRange(items);
			this.ResetItems(this.m_items.Count, false, false);
		}

		// Token: 0x06004493 RID: 17555 RVA: 0x0006E51A File Offset: 0x0006C71A
		protected override PendingFriendsItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			PendingFriendsItemViewsHolder pendingFriendsItemViewsHolder = new PendingFriendsItemViewsHolder();
			pendingFriendsItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return pendingFriendsItemViewsHolder;
		}

		// Token: 0x06004494 RID: 17556 RVA: 0x0006E540 File Offset: 0x0006C740
		protected override void UpdateViewsHolder(PendingFriendsItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateItem(this.m_items[newOrRecycled.ItemIndex]);
		}

		// Token: 0x06004495 RID: 17557 RVA: 0x0019CD68 File Offset: 0x0019AF68
		public void Reindex()
		{
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				PendingFriendsItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.Init(this.m_items[i]);
				}
			}
		}

		// Token: 0x04004128 RID: 16680
		private List<Mail> m_items;
	}
}
