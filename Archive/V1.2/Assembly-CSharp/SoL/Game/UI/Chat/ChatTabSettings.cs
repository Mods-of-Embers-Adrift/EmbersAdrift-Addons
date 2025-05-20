using System;
using SoL.Game.Messages;

namespace SoL.Game.UI.Chat
{
	// Token: 0x020009A9 RID: 2473
	[Serializable]
	public class ChatTabSettings
	{
		// Token: 0x06004A04 RID: 18948 RVA: 0x00071CB1 File Offset: 0x0006FEB1
		public ChatTabSettings()
		{
		}

		// Token: 0x06004A05 RID: 18949 RVA: 0x001B1784 File Offset: 0x001AF984
		public ChatTabSettings(ChatTabSettings other)
		{
			this.Mode = other.Mode;
			this.Name = other.Name;
			this.ChatFilter = other.ChatFilter;
			this.CombatFilter = other.CombatFilter;
			this.InputChannel = other.InputChannel;
		}

		// Token: 0x04004500 RID: 17664
		public ChatTabMode Mode;

		// Token: 0x04004501 RID: 17665
		public string Name;

		// Token: 0x04004502 RID: 17666
		public ChatFilter ChatFilter = ChatFilter.All;

		// Token: 0x04004503 RID: 17667
		public CombatFilter CombatFilter = CombatFilter.All;

		// Token: 0x04004504 RID: 17668
		public MessageType InputChannel = MessageType.Say;
	}
}
