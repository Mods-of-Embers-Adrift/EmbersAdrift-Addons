using System;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A5A RID: 2650
	[Flags]
	public enum ConsumableCategory
	{
		// Token: 0x04004984 RID: 18820
		None = 0,
		// Token: 0x04004985 RID: 18821
		HealthPotion = 1,
		// Token: 0x04004986 RID: 18822
		ResistPotion = 2,
		// Token: 0x04004987 RID: 18823
		UtilityConsumable = 4,
		// Token: 0x04004988 RID: 18824
		Food = 8,
		// Token: 0x04004989 RID: 18825
		SmellingSalts = 16,
		// Token: 0x0400498A RID: 18826
		GroundTorch = 32,
		// Token: 0x0400498B RID: 18827
		Recipe = 64,
		// Token: 0x0400498C RID: 18828
		ThrowingAmmo = 128,
		// Token: 0x0400498D RID: 18829
		Drink = 256,
		// Token: 0x0400498E RID: 18830
		Alcohol = 512,
		// Token: 0x0400498F RID: 18831
		Ability = 1024,
		// Token: 0x04004990 RID: 18832
		CraftingStation = 2048,
		// Token: 0x04004991 RID: 18833
		Fireworks = 4096
	}
}
