using System;
using Com.TheFallenGames.OSA.Core;
using SoL.Networking.Database;

namespace SoL.Game.UI.Social
{
	// Token: 0x02000918 RID: 2328
	[Serializable]
	public class PendingFriendsItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x06004497 RID: 17559 RVA: 0x0006E561 File Offset: 0x0006C761
		public override void CollectViews()
		{
			base.CollectViews();
			this.ListItem = this.root.GetComponent<PendingFriendsListItem>();
		}

		// Token: 0x06004498 RID: 17560 RVA: 0x0006E57A File Offset: 0x0006C77A
		public void UpdateItem(Mail item)
		{
			if (this.Data == item)
			{
				return;
			}
			this.Data = item;
			this.ListItem.Init(this.Data);
		}

		// Token: 0x04004129 RID: 16681
		public PendingFriendsListItem ListItem;

		// Token: 0x0400412A RID: 16682
		public Mail Data;
	}
}
