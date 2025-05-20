using System;
using Cysharp.Text;
using SoL.Game.Messages;
using TMPro;
using UnityEngine;

namespace SoL.Game.UI.Chat
{
	// Token: 0x020009A6 RID: 2470
	public class ChatListItem : MonoBehaviour
	{
		// Token: 0x1700105B RID: 4187
		// (get) Token: 0x060049F6 RID: 18934 RVA: 0x00071BCC File Offset: 0x0006FDCC
		// (set) Token: 0x060049F7 RID: 18935 RVA: 0x00071BD3 File Offset: 0x0006FDD3
		public static Color? DefaultChatColor { get; set; }

		// Token: 0x060049F8 RID: 18936 RVA: 0x001B1570 File Offset: 0x001AF770
		public void Init(ChatWindowUI parentWindow, ChatMessage message, int index)
		{
			this.m_parentWindow = parentWindow;
			this.m_message = message;
			this.m_index = index;
			if (ChatListItem.DefaultChatColor == null)
			{
				ChatListItem.DefaultChatColor = new Color?(this.m_line.color);
			}
			this.RefreshLineContent(this.m_parentWindow.ShowTimestamps);
		}

		// Token: 0x060049F9 RID: 18937 RVA: 0x00071BDB File Offset: 0x0006FDDB
		public void UpdateFontSize()
		{
			if ((int)this.m_line.fontSize != Options.GameOptions.ChatFontSize.Value)
			{
				this.m_line.fontSize = (float)Options.GameOptions.ChatFontSize.Value;
			}
		}

		// Token: 0x060049FA RID: 18938 RVA: 0x00071C0B File Offset: 0x0006FE0B
		public void RefreshLineContent(bool showTimestamps)
		{
			this.m_line.ZStringSetText(this.m_message.GetCachedFormattedMessage(showTimestamps));
			this.m_line.fontSize = (float)Options.GameOptions.ChatFontSize.Value;
		}

		// Token: 0x040044F0 RID: 17648
		[SerializeField]
		private TextMeshProUGUI m_line;

		// Token: 0x040044F1 RID: 17649
		private ChatWindowUI m_parentWindow;

		// Token: 0x040044F2 RID: 17650
		private ChatMessage m_message;

		// Token: 0x040044F3 RID: 17651
		private int m_index;
	}
}
