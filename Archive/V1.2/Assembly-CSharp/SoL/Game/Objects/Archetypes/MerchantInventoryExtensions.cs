using System;
using System.Collections.Generic;
using SoL.Game.Objects.Containers;
using SoL.Utilities;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A1D RID: 2589
	public static class MerchantInventoryExtensions
	{
		// Token: 0x06004F9B RID: 20379 RVA: 0x001CA9A0 File Offset: 0x001C8BA0
		public static bool EntityCanAcquire(this IEnumerable<IMerchantInventory> acquisitions, GameEntity entity, out string errorMessage)
		{
			errorMessage = string.Empty;
			List<ItemArchetype> fromPool = StaticListPool<ItemArchetype>.GetFromPool();
			foreach (IMerchantInventory merchantInventory in acquisitions)
			{
				ItemArchetype itemArchetype = merchantInventory.Archetype as ItemArchetype;
				if (itemArchetype != null)
				{
					fromPool.Add(itemArchetype);
				}
			}
			CannotAcquireReason cannotAcquireReason;
			if (!entity.EntityCanAcquire(fromPool, out cannotAcquireReason))
			{
				string text;
				if (cannotAcquireReason != CannotAcquireReason.NoRoom)
				{
					if (cannotAcquireReason != CannotAcquireReason.Dead)
					{
						text = "Cannot receive item for unknown reason";
					}
					else
					{
						text = "Cannot receive while missing your bag";
					}
				}
				else
				{
					text = "Not enough room in inventory";
				}
				errorMessage = text;
				return false;
			}
			StaticListPool<ItemArchetype>.ReturnToPool(fromPool);
			using (IEnumerator<IMerchantInventory> enumerator = acquisitions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.EntityCanAcquire(entity, out errorMessage))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
