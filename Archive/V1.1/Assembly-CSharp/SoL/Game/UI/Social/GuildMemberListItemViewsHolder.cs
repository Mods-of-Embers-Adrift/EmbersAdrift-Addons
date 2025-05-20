using System;
using Com.TheFallenGames.OSA.Core;
using SoL.Networking.Database;

namespace SoL.Game.UI.Social
{
	// Token: 0x02000900 RID: 2304
	[Serializable]
	public class GuildMemberListItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x17000F49 RID: 3913
		// (get) Token: 0x06004391 RID: 17297 RVA: 0x0006D9DD File Offset: 0x0006BBDD
		// (set) Token: 0x06004392 RID: 17298 RVA: 0x0006D9E5 File Offset: 0x0006BBE5
		public GuildMemberListItem ListItem { get; private set; }

		// Token: 0x17000F4A RID: 3914
		// (get) Token: 0x06004393 RID: 17299 RVA: 0x0006D9EE File Offset: 0x0006BBEE
		// (set) Token: 0x06004394 RID: 17300 RVA: 0x0006D9F6 File Offset: 0x0006BBF6
		public GuildMember Data { get; private set; }

		// Token: 0x06004395 RID: 17301 RVA: 0x0006D9FF File Offset: 0x0006BBFF
		public override void CollectViews()
		{
			base.CollectViews();
			this.ListItem = this.root.GetComponent<GuildMemberListItem>();
		}

		// Token: 0x06004396 RID: 17302 RVA: 0x0006DA18 File Offset: 0x0006BC18
		public void UpdateItem(GuildMemberList parent, GuildMember item)
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
