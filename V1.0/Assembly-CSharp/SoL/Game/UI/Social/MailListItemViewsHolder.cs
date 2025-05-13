using System;
using Com.TheFallenGames.OSA.Core;
using SoL.Networking.Database;

namespace SoL.Game.UI.Social
{
	// Token: 0x02000915 RID: 2325
	[Serializable]
	public class MailListItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x06004481 RID: 17537 RVA: 0x0006E407 File Offset: 0x0006C607
		public override void CollectViews()
		{
			base.CollectViews();
			this.ListItem = this.root.GetComponent<MailListItem>();
		}

		// Token: 0x06004482 RID: 17538 RVA: 0x0006E420 File Offset: 0x0006C620
		public void UpdateItem(Mail item, MailList parent)
		{
			if (this.Data == item)
			{
				return;
			}
			this.Data = item;
			this.ListItem.Init(this.Data, parent);
		}

		// Token: 0x04004108 RID: 16648
		public MailListItem ListItem;

		// Token: 0x04004109 RID: 16649
		public Mail Data;
	}
}
