using System;
using Com.TheFallenGames.OSA.Core;
using SoL.Networking.Database;

namespace SoL.Game.UI.Social
{
	// Token: 0x02000903 RID: 2307
	[Serializable]
	public class GuildRankListItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x17000F4E RID: 3918
		// (get) Token: 0x060043B8 RID: 17336 RVA: 0x0006DC2F File Offset: 0x0006BE2F
		// (set) Token: 0x060043B9 RID: 17337 RVA: 0x0006DC37 File Offset: 0x0006BE37
		public GuildRankListItem ListItem { get; private set; }

		// Token: 0x17000F4F RID: 3919
		// (get) Token: 0x060043BA RID: 17338 RVA: 0x0006DC40 File Offset: 0x0006BE40
		// (set) Token: 0x060043BB RID: 17339 RVA: 0x0006DC48 File Offset: 0x0006BE48
		public GuildRank Data { get; private set; }

		// Token: 0x060043BC RID: 17340 RVA: 0x0006DC51 File Offset: 0x0006BE51
		public override void CollectViews()
		{
			base.CollectViews();
			this.ListItem = this.root.GetComponent<GuildRankListItem>();
		}

		// Token: 0x060043BD RID: 17341 RVA: 0x0006DC6A File Offset: 0x0006BE6A
		public void UpdateItem(GuildRankList parent, GuildRank item)
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
