using System;
using SoL.Game.Loot;

namespace SoL.Utilities
{
	// Token: 0x0200027B RID: 635
	public class LootTableItemDrawBag : DrawBag<LootTableItem, LootTableItemProbabilityEntry, LootTableItemProbabilityCollection>
	{
		// Token: 0x060013D4 RID: 5076 RVA: 0x0004FEA5 File Offset: 0x0004E0A5
		public LootTableItemDrawBag(LootTableItemProbabilityCollection collection, int sampleSize, float reshuffleThreshold) : base(collection, sampleSize, reshuffleThreshold)
		{
		}
	}
}
