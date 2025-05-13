using System;
using Com.TheFallenGames.OSA.Core;
using SoL.Networking.Database;

namespace SoL.Game.UI.Social
{
	// Token: 0x020008F3 RID: 2291
	[Serializable]
	public class BlockItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x0600432F RID: 17199 RVA: 0x0006D4ED File Offset: 0x0006B6ED
		public override void CollectViews()
		{
			base.CollectViews();
			this.ListItem = this.root.GetComponent<BlockListItem>();
		}

		// Token: 0x06004330 RID: 17200 RVA: 0x0006D506 File Offset: 0x0006B706
		public void UpdateItem(Relation item)
		{
			if (this.Data == item)
			{
				return;
			}
			this.Data = item;
			this.ListItem.Init(this.Data);
		}

		// Token: 0x04003FDB RID: 16347
		public BlockListItem ListItem;

		// Token: 0x04003FDC RID: 16348
		public Relation Data;
	}
}
