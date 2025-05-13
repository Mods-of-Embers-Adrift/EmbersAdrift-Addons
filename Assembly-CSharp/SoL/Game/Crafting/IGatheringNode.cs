using System;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;

namespace SoL.Game.Crafting
{
	// Token: 0x02000CDA RID: 3290
	public interface IGatheringNode
	{
		// Token: 0x170017C7 RID: 6087
		// (get) Token: 0x0600638B RID: 25483
		CraftingToolType RequiredTool { get; }

		// Token: 0x170017C8 RID: 6088
		// (get) Token: 0x0600638C RID: 25484
		GameEntity GameEntity { get; }

		// Token: 0x170017C9 RID: 6089
		// (get) Token: 0x0600638D RID: 25485
		int ResourceLevel { get; }

		// Token: 0x170017CA RID: 6090
		// (get) Token: 0x0600638E RID: 25486
		float GatherTime { get; }

		// Token: 0x0600638F RID: 25487
		bool CanInteract(GameEntity entity, out ArchetypeInstance requiredItemInstance);

		// Token: 0x06006390 RID: 25488
		void BeginInteraction(GameEntity entity);

		// Token: 0x06006391 RID: 25489
		MasteryArchetype GetGatheringMastery();

		// Token: 0x170017CB RID: 6091
		// (get) Token: 0x06006392 RID: 25490
		UniqueId? RequiredItemId { get; }

		// Token: 0x170017CC RID: 6092
		// (get) Token: 0x06006393 RID: 25491
		bool RemoveRequiredItemOnUse { get; }

		// Token: 0x170017CD RID: 6093
		// (get) Token: 0x06006394 RID: 25492
		InteractiveFlags InteractiveFlags { get; }
	}
}
