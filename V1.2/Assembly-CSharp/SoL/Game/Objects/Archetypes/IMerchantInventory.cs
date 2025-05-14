using System;
using SoL.Game.Transactions;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A1C RID: 2588
	public interface IMerchantInventory
	{
		// Token: 0x1700117A RID: 4474
		// (get) Token: 0x06004F96 RID: 20374
		BaseArchetype Archetype { get; }

		// Token: 0x06004F97 RID: 20375
		ulong GetSellPrice(GameEntity entity);

		// Token: 0x06004F98 RID: 20376
		ulong GetEventCost(GameEntity entity);

		// Token: 0x06004F99 RID: 20377
		bool EntityCanAcquire(GameEntity entity, out string errorMessage);

		// Token: 0x06004F9A RID: 20378
		bool AddToPlayer(GameEntity entity, ItemAddContext context, uint quantity, ItemFlags itemFlags, bool markAsSoulbound, out ArchetypeInstance resultingInstance);
	}
}
