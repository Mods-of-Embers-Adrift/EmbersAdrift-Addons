using System;

namespace SoL.Game.UI
{
	// Token: 0x020008C4 RID: 2244
	[Flags]
	public enum ToggleAllWindowFlags
	{
		// Token: 0x04003EE3 RID: 16099
		None = 0,
		// Token: 0x04003EE4 RID: 16100
		Time = 1,
		// Token: 0x04003EE5 RID: 16101
		Bag = 2,
		// Token: 0x04003EE6 RID: 16102
		Gathering = 4,
		// Token: 0x04003EE7 RID: 16103
		Inventory = 8,
		// Token: 0x04003EE8 RID: 16104
		Skills = 16,
		// Token: 0x04003EE9 RID: 16105
		Recipes = 32,
		// Token: 0x04003EEA RID: 16106
		Log = 64,
		// Token: 0x04003EEB RID: 16107
		Map = 128,
		// Token: 0x04003EEC RID: 16108
		Social = 256,
		// Token: 0x04003EED RID: 16109
		Notifications = 512,
		// Token: 0x04003EEE RID: 16110
		Default = 14
	}
}
