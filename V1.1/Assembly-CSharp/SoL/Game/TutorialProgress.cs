using System;

namespace SoL.Game
{
	// Token: 0x020005B4 RID: 1460
	[Flags]
	public enum TutorialProgress
	{
		// Token: 0x04002D97 RID: 11671
		None = 0,
		// Token: 0x04002D98 RID: 11672
		MobDifficulty1 = 1,
		// Token: 0x04002D99 RID: 11673
		MobDifficulty2 = 2,
		// Token: 0x04002D9A RID: 11674
		UsefulHotkeys = 4,
		// Token: 0x04002D9B RID: 11675
		KnockedSenseless = 8,
		// Token: 0x04002D9C RID: 11676
		LiveToPlayAnotherDay = 16,
		// Token: 0x04002D9D RID: 11677
		ALostAdventuringBag = 32,
		// Token: 0x04002D9E RID: 11678
		ActionBar = 64,
		// Token: 0x04002D9F RID: 11679
		CombatPositioning = 128,
		// Token: 0x04002DA0 RID: 11680
		ArmorWeight = 256,
		// Token: 0x04002DA1 RID: 11681
		FindingAGroup = 512,
		// Token: 0x04002DA2 RID: 11682
		Specializations = 1024,
		// Token: 0x04002DA3 RID: 11683
		CombatStance = 2048,
		// Token: 0x04002DA4 RID: 11684
		Crafting = 4096,
		// Token: 0x04002DA5 RID: 11685
		FurtherProfessions = 8192,
		// Token: 0x04002DA6 RID: 11686
		GatheringBag = 16384,
		// Token: 0x04002DA7 RID: 11687
		Movement = 32768,
		// Token: 0x04002DA8 RID: 11688
		PlayerInteractions = 65536,
		// Token: 0x04002DA9 RID: 11689
		Reagents = 131072,
		// Token: 0x04002DAA RID: 11690
		SheatheWeapon = 262144,
		// Token: 0x04002DAB RID: 11691
		Wounds = 524288,
		// Token: 0x04002DAC RID: 11692
		BulletinBoards = 1048576
	}
}
