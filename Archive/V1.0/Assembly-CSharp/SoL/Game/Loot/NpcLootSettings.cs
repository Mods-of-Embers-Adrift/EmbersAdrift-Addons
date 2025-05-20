using System;

namespace SoL.Game.Loot
{
	// Token: 0x02000B0F RID: 2831
	[Serializable]
	public class NpcLootSettings : ResourceLootSettings
	{
		// Token: 0x04004D17 RID: 19735
		private const string kItemTable = "Loot/Items";

		// Token: 0x04004D18 RID: 19736
		public LootTableSampleCount ItemLootTable;
	}
}
