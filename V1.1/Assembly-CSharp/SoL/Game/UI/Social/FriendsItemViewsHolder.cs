using System;
using Com.TheFallenGames.OSA.Core;
using SoL.Networking.Database;

namespace SoL.Game.UI.Social
{
	// Token: 0x020008F6 RID: 2294
	[Serializable]
	public class FriendsItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x06004342 RID: 17218 RVA: 0x0006D5E8 File Offset: 0x0006B7E8
		public override void CollectViews()
		{
			base.CollectViews();
			this.ListItem = this.root.GetComponent<FriendsListItem>();
		}

		// Token: 0x06004343 RID: 17219 RVA: 0x0006D601 File Offset: 0x0006B801
		public void UpdateItem(Relation item)
		{
			if (this.Data == item)
			{
				return;
			}
			this.Data = item;
			this.ListItem.Init(this.Data);
		}

		// Token: 0x04003FE2 RID: 16354
		public FriendsListItem ListItem;

		// Token: 0x04003FE3 RID: 16355
		public Relation Data;
	}
}
