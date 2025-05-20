using System;

namespace SoL.Game.NPCs
{
	// Token: 0x02000819 RID: 2073
	[Flags]
	public enum NpcTags
	{
		// Token: 0x04003AC3 RID: 15043
		None = 0,
		// Token: 0x04003AC4 RID: 15044
		Player = 1,
		// Token: 0x04003AC5 RID: 15045
		Aggressive = 2,
		// Token: 0x04003AC6 RID: 15046
		Docile = 4,
		// Token: 0x04003AC7 RID: 15047
		Humanoid = 8,
		// Token: 0x04003AC8 RID: 15048
		Animal = 16,
		// Token: 0x04003AC9 RID: 15049
		Insect = 32,
		// Token: 0x04003ACA RID: 15050
		Ashen = 64,
		// Token: 0x04003ACB RID: 15051
		Npc = 128,
		// Token: 0x04003ACC RID: 15052
		Guard = 256,
		// Token: 0x04003ACD RID: 15053
		Exile = 512,
		// Token: 0x04003ACE RID: 15054
		Bat = 1024,
		// Token: 0x04003ACF RID: 15055
		Bear = 2048,
		// Token: 0x04003AD0 RID: 15056
		Boar = 4096,
		// Token: 0x04003AD1 RID: 15057
		DeerDoe = 8192,
		// Token: 0x04003AD2 RID: 15058
		DeerStag = 16384,
		// Token: 0x04003AD3 RID: 15059
		Rabbit = 32768,
		// Token: 0x04003AD4 RID: 15060
		Raccoon = 65536,
		// Token: 0x04003AD5 RID: 15061
		Wolf = 131072,
		// Token: 0x04003AD6 RID: 15062
		Ant = 262144,
		// Token: 0x04003AD7 RID: 15063
		Beetle = 524288,
		// Token: 0x04003AD8 RID: 15064
		Firefly = 1048576,
		// Token: 0x04003AD9 RID: 15065
		Spider = 2097152,
		// Token: 0x04003ADA RID: 15066
		Rat = 4194304,
		// Token: 0x04003ADB RID: 15067
		Basilisk = 8388608,
		// Token: 0x04003ADC RID: 15068
		Drybone = 16777216,
		// Token: 0x04003ADD RID: 15069
		Crocodile = 33554432,
		// Token: 0x04003ADE RID: 15070
		Snatchscale = 67108864,
		// Token: 0x04003ADF RID: 15071
		Crab = 134217728,
		// Token: 0x04003AE0 RID: 15072
		Named = 268435456,
		// Token: 0x04003AE1 RID: 15073
		Mangrove = 536870912,
		// Token: 0x04003AE2 RID: 15074
		Others = 1073741824
	}
}
