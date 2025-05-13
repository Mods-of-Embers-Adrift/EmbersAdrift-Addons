using System;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A7E RID: 2686
	[Flags]
	public enum HandheldItemFlags
	{
		// Token: 0x04004A4C RID: 19020
		None = 0,
		// Token: 0x04004A4D RID: 19021
		Empty = 1,
		// Token: 0x04004A4E RID: 19022
		Blade1H = 2,
		// Token: 0x04004A4F RID: 19023
		Blade2H = 4,
		// Token: 0x04004A50 RID: 19024
		Pierce1H = 8,
		// Token: 0x04004A51 RID: 19025
		Pierce2H = 16,
		// Token: 0x04004A52 RID: 19026
		Blunt1H = 32,
		// Token: 0x04004A53 RID: 19027
		Blunt2H = 64,
		// Token: 0x04004A54 RID: 19028
		Staff1H = 128,
		// Token: 0x04004A55 RID: 19029
		Staff2H = 256,
		// Token: 0x04004A56 RID: 19030
		Bow = 512,
		// Token: 0x04004A57 RID: 19031
		Crossbow = 1024,
		// Token: 0x04004A58 RID: 19032
		Ammo = 2048,
		// Token: 0x04004A59 RID: 19033
		Shield = 4096,
		// Token: 0x04004A5A RID: 19034
		Accessory = 8192,
		// Token: 0x04004A5B RID: 19035
		EmberStone = 16384
	}
}
