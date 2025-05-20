using System;

namespace SoL.Utilities
{
	// Token: 0x0200028A RID: 650
	public static class GlobalCounters
	{
		// Token: 0x060013FA RID: 5114 RVA: 0x000F8958 File Offset: 0x000F6B58
		private static void ResetValues()
		{
			GlobalCounters.SpawnedPlayers = 0U;
			GlobalCounters.SpawnedNpcs = 0U;
			GlobalCounters.SpawnedNodes = 0U;
			GlobalCounters.LootGenerated = 0U;
			GlobalCounters.ItemsCrafted = 0U;
			GlobalCounters.ItemsSold = 0U;
			GlobalCounters.ItemsPurchased = 0U;
			GlobalCounters.EffectApplicators = 0U;
			GlobalCounters.CharQuestWrites = 0U;
			GlobalCounters.CharLearnableWrites = 0U;
			GlobalCounters.CharDiscoveryWrites = 0U;
			GlobalCounters.CharStorageWrites = 0U;
			GlobalCounters.CharTitleWrites = 0U;
			GlobalCounters.CharRecordWrites = 0U;
			GlobalCounters.ContainerLoads = 0U;
			GlobalCounters.ContainerSaves = 0U;
		}

		// Token: 0x04001C32 RID: 7218
		public static uint SpawnedPlayers;

		// Token: 0x04001C33 RID: 7219
		public static uint SpawnedNpcs;

		// Token: 0x04001C34 RID: 7220
		public static uint SpawnedNodes;

		// Token: 0x04001C35 RID: 7221
		public static uint LootGenerated;

		// Token: 0x04001C36 RID: 7222
		public static uint ItemsCrafted;

		// Token: 0x04001C37 RID: 7223
		public static uint ItemsSold;

		// Token: 0x04001C38 RID: 7224
		public static uint ItemsPurchased;

		// Token: 0x04001C39 RID: 7225
		public static uint EffectApplicators;

		// Token: 0x04001C3A RID: 7226
		public static uint CharQuestWrites;

		// Token: 0x04001C3B RID: 7227
		public static uint CharLearnableWrites;

		// Token: 0x04001C3C RID: 7228
		public static uint CharDiscoveryWrites;

		// Token: 0x04001C3D RID: 7229
		public static uint CharStorageWrites;

		// Token: 0x04001C3E RID: 7230
		public static uint CharTitleWrites;

		// Token: 0x04001C3F RID: 7231
		public static uint CharRecordWrites;

		// Token: 0x04001C40 RID: 7232
		public static uint ContainerLoads;

		// Token: 0x04001C41 RID: 7233
		public static uint ContainerSaves;
	}
}
