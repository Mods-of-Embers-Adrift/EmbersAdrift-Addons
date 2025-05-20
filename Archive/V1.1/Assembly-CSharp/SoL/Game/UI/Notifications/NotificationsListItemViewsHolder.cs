using System;
using Com.TheFallenGames.OSA.Core;
using SoL.Game.Notifications;

namespace SoL.Game.UI.Notifications
{
	// Token: 0x0200095F RID: 2399
	[Serializable]
	public class NotificationsListItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x17000FCF RID: 4047
		// (get) Token: 0x0600471C RID: 18204 RVA: 0x0006FE6D File Offset: 0x0006E06D
		// (set) Token: 0x0600471D RID: 18205 RVA: 0x0006FE75 File Offset: 0x0006E075
		public NotificationsListItem ListItem { get; private set; }

		// Token: 0x17000FD0 RID: 4048
		// (get) Token: 0x0600471E RID: 18206 RVA: 0x0006FE7E File Offset: 0x0006E07E
		// (set) Token: 0x0600471F RID: 18207 RVA: 0x0006FE86 File Offset: 0x0006E086
		public Notification Data { get; private set; }

		// Token: 0x06004720 RID: 18208 RVA: 0x0006FE8F File Offset: 0x0006E08F
		public override void CollectViews()
		{
			base.CollectViews();
			this.ListItem = this.root.GetComponent<NotificationsListItem>();
		}

		// Token: 0x06004721 RID: 18209 RVA: 0x0006FEA8 File Offset: 0x0006E0A8
		public void UpdateItem(NotificationsList parent, Notification item)
		{
			if (this.Data == item)
			{
				return;
			}
			this.Data = item;
			this.ListItem.Init(parent, this.ItemIndex, this.Data);
		}
	}
}
