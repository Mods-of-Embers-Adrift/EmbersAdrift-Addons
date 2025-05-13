using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Spawning;

namespace SoL.Game.Loot
{
	// Token: 0x02000B0A RID: 2826
	public interface ILootRollSource
	{
		// Token: 0x17001475 RID: 5237
		// (get) Token: 0x0600572B RID: 22315
		bool LootRollIsPending { get; }

		// Token: 0x0600572C RID: 22316
		void SetLootRollCount(int count);

		// Token: 0x0600572D RID: 22317
		void LootRollCompleted();

		// Token: 0x0600572E RID: 22318
		void RemoveFromRecord(ArchetypeInstance instance);

		// Token: 0x17001476 RID: 5238
		// (get) Token: 0x0600572F RID: 22319
		bool IsRaid { get; }

		// Token: 0x17001477 RID: 5239
		// (get) Token: 0x06005730 RID: 22320
		ChallengeRating ChallengeRating { get; }
	}
}
