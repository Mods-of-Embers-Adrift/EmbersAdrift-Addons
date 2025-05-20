using System;
using Com.TheFallenGames.OSA.Core;
using SoL.Networking.Database;

namespace SoL.Game.UI.Social
{
	// Token: 0x020008FD RID: 2301
	[Serializable]
	public class GuildInvitesItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x0600437A RID: 17274 RVA: 0x0006D84F File Offset: 0x0006BA4F
		public override void CollectViews()
		{
			base.CollectViews();
			this.ListItem = this.root.GetComponent<GuildInvitesListItem>();
		}

		// Token: 0x0600437B RID: 17275 RVA: 0x0006D868 File Offset: 0x0006BA68
		public void UpdateItem(Mail item)
		{
			if (this.Data == item)
			{
				return;
			}
			this.Data = item;
			this.ListItem.Init(this.Data);
		}

		// Token: 0x04004018 RID: 16408
		public GuildInvitesListItem ListItem;

		// Token: 0x04004019 RID: 16409
		public Mail Data;
	}
}
