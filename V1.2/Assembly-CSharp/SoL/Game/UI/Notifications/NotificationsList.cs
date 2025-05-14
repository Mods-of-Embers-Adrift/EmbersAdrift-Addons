using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using SoL.Game.Notifications;

namespace SoL.Game.UI.Notifications
{
	// Token: 0x0200095E RID: 2398
	public class NotificationsList : OSA<BaseParamsWithPrefab, NotificationsListItemViewsHolder>
	{
		// Token: 0x06004718 RID: 18200 RVA: 0x001A5EA0 File Offset: 0x001A40A0
		public void UpdateItems(ICollection<Notification> items)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.m_items == null)
			{
				this.m_items = new List<Notification>(items.Count);
			}
			this.m_items.Clear();
			this.m_items.AddRange(items);
			this.ResetItems(this.m_items.Count, false, false);
		}

		// Token: 0x06004719 RID: 18201 RVA: 0x0006FE25 File Offset: 0x0006E025
		protected override NotificationsListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			NotificationsListItemViewsHolder notificationsListItemViewsHolder = new NotificationsListItemViewsHolder();
			notificationsListItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return notificationsListItemViewsHolder;
		}

		// Token: 0x0600471A RID: 18202 RVA: 0x0006FE4B File Offset: 0x0006E04B
		protected override void UpdateViewsHolder(NotificationsListItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateItem(this, this.m_items[newOrRecycled.ItemIndex]);
		}

		// Token: 0x04004309 RID: 17161
		private List<Notification> m_items;
	}
}
