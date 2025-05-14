using System;
using Com.TheFallenGames.OSA.Core;
using SoL.Game.Messages;

namespace SoL.Game.UI.Chat
{
	// Token: 0x020009A5 RID: 2469
	[Serializable]
	public class ChatItemViewsHolder : BaseItemViewsHolder
	{
		// Token: 0x060049F1 RID: 18929 RVA: 0x00071B6D File Offset: 0x0006FD6D
		public override void CollectViews()
		{
			base.CollectViews();
			this.m_listItem = this.root.GetComponent<ChatListItem>();
		}

		// Token: 0x060049F2 RID: 18930 RVA: 0x00071B86 File Offset: 0x0006FD86
		public void UpdateItem(ChatWindowUI parentWindow, ChatMessage item)
		{
			if (this.m_data == item)
			{
				return;
			}
			this.m_data = item;
			this.m_listItem.Init(parentWindow, this.m_data, this.ItemIndex);
		}

		// Token: 0x060049F3 RID: 18931 RVA: 0x00071BB1 File Offset: 0x0006FDB1
		public void RefreshLineContent(bool showTimestamps)
		{
			this.m_listItem.RefreshLineContent(showTimestamps);
		}

		// Token: 0x060049F4 RID: 18932 RVA: 0x00071BBF File Offset: 0x0006FDBF
		public void UpdateFontSize()
		{
			this.m_listItem.UpdateFontSize();
		}

		// Token: 0x040044ED RID: 17645
		private ChatListItem m_listItem;

		// Token: 0x040044EE RID: 17646
		private ChatMessage m_data;
	}
}
