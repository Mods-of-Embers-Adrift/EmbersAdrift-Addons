using System;
using System.Collections.Generic;
using SoL.Game.Crafting;
using SoL.Game.NPCs;
using SoL.Game.Objects.Archetypes;

namespace SoL.Game.Loot
{
	// Token: 0x02000B05 RID: 2821
	public interface ILootTable
	{
		// Token: 0x06005727 RID: 22311
		List<ArchetypeInstance> GenerateLoot(int sampleCount, GameEntity interactionSource, SpawnTier spawnTier, IGatheringNode node);
	}
}
