using System;
using Com.TheFallenGames.OSA.CustomAdapters.GridView;

namespace SoL.Game.UI.Chat
{
	// Token: 0x020009AD RID: 2477
	public class ChatTabFilterListItemViewsHolder : CellViewsHolder
	{
		// Token: 0x1700106E RID: 4206
		// (get) Token: 0x06004A5D RID: 19037 RVA: 0x0007213E File Offset: 0x0007033E
		// (set) Token: 0x06004A5E RID: 19038 RVA: 0x00072146 File Offset: 0x00070346
		public ChatTabFilterListItem ListItem { get; private set; }

		// Token: 0x1700106F RID: 4207
		// (get) Token: 0x06004A5F RID: 19039 RVA: 0x0007214F File Offset: 0x0007034F
		// (set) Token: 0x06004A60 RID: 19040 RVA: 0x00072157 File Offset: 0x00070357
		public int Data { get; private set; }

		// Token: 0x06004A61 RID: 19041 RVA: 0x00072160 File Offset: 0x00070360
		public override void CollectViews()
		{
			base.CollectViews();
			this.ListItem = this.root.GetComponent<ChatTabFilterListItem>();
		}

		// Token: 0x06004A62 RID: 19042 RVA: 0x00072179 File Offset: 0x00070379
		public void UpdateItem(ChatTabFilterList parent, int item, int currentFilter)
		{
			if (this.Data == item)
			{
				return;
			}
			this.Data = item;
			this.ListItem.Init(parent, this.Data, currentFilter, this.ItemIndex);
		}
	}
}
