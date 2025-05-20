using System;

namespace SoL.Game.Messages
{
	// Token: 0x020009E6 RID: 2534
	[Flags]
	public enum MessageType
	{
		// Token: 0x040046E1 RID: 18145
		None = 0,
		// Token: 0x040046E2 RID: 18146
		Notification = 1,
		// Token: 0x040046E3 RID: 18147
		System = 2,
		// Token: 0x040046E4 RID: 18148
		Motd = 4,
		// Token: 0x040046E5 RID: 18149
		Skills = 8,
		// Token: 0x040046E6 RID: 18150
		Say = 16,
		// Token: 0x040046E7 RID: 18151
		Yell = 32,
		// Token: 0x040046E8 RID: 18152
		Tell = 64,
		// Token: 0x040046E9 RID: 18153
		Party = 128,
		// Token: 0x040046EA RID: 18154
		Guild = 256,
		// Token: 0x040046EB RID: 18155
		Officer = 1048576,
		// Token: 0x040046EC RID: 18156
		Zone = 512,
		// Token: 0x040046ED RID: 18157
		World = 524288,
		// Token: 0x040046EE RID: 18158
		Trade = 2097152,
		// Token: 0x040046EF RID: 18159
		Subscriber = 4194304,
		// Token: 0x040046F0 RID: 18160
		Help = 8388608,
		// Token: 0x040046F1 RID: 18161
		Raid = 16777216,
		// Token: 0x040046F2 RID: 18162
		MyCombatOut = 1024,
		// Token: 0x040046F3 RID: 18163
		MyCombatIn = 2048,
		// Token: 0x040046F4 RID: 18164
		OtherCombat = 4096,
		// Token: 0x040046F5 RID: 18165
		WarlordSong = 8192,
		// Token: 0x040046F6 RID: 18166
		Quest = 16384,
		// Token: 0x040046F7 RID: 18167
		Loot = 32768,
		// Token: 0x040046F8 RID: 18168
		Emote = 65536,
		// Token: 0x040046F9 RID: 18169
		Discovery = 131072,
		// Token: 0x040046FA RID: 18170
		Social = 262144,
		// Token: 0x040046FB RID: 18171
		PreFormatted = 1073741824,
		// Token: 0x040046FC RID: 18172
		ChatChannel = 33031088,
		// Token: 0x040046FD RID: 18173
		Chat = 33031152,
		// Token: 0x040046FE RID: 18174
		Combat = 15360,
		// Token: 0x040046FF RID: 18175
		AllNotifications = 507919,
		// Token: 0x04004700 RID: 18176
		All = 1107296255
	}
}
