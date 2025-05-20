using System;

namespace SoL.Game.UI.Chat
{
	// Token: 0x020009A2 RID: 2466
	[Flags]
	public enum ChatFilter
	{
		// Token: 0x040044CE RID: 17614
		All = 1048575,
		// Token: 0x040044CF RID: 17615
		Say = 1,
		// Token: 0x040044D0 RID: 17616
		Yell = 2,
		// Token: 0x040044D1 RID: 17617
		Tell = 4,
		// Token: 0x040044D2 RID: 17618
		Party = 8,
		// Token: 0x040044D3 RID: 17619
		Raid = 524288,
		// Token: 0x040044D4 RID: 17620
		Guild = 16,
		// Token: 0x040044D5 RID: 17621
		Officer = 32,
		// Token: 0x040044D6 RID: 17622
		Zone = 64,
		// Token: 0x040044D7 RID: 17623
		World = 128,
		// Token: 0x040044D8 RID: 17624
		Trade = 256,
		// Token: 0x040044D9 RID: 17625
		Emote = 512,
		// Token: 0x040044DA RID: 17626
		Notification = 1024,
		// Token: 0x040044DB RID: 17627
		Loot = 2048,
		// Token: 0x040044DC RID: 17628
		Motd = 4096,
		// Token: 0x040044DD RID: 17629
		Skills = 8192,
		// Token: 0x040044DE RID: 17630
		Quest = 16384,
		// Token: 0x040044DF RID: 17631
		Discovery = 32768,
		// Token: 0x040044E0 RID: 17632
		Social = 65536,
		// Token: 0x040044E1 RID: 17633
		Subscriber = 131072,
		// Token: 0x040044E2 RID: 17634
		Help = 262144
	}
}
