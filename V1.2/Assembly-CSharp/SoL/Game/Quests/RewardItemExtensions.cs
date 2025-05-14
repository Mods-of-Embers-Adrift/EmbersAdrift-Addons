using System;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities;

namespace SoL.Game.Quests
{
	// Token: 0x02000793 RID: 1939
	public static class RewardItemExtensions
	{
		// Token: 0x0600393D RID: 14653 RVA: 0x001727EC File Offset: 0x001709EC
		public static bool EntityCanAcquire(this IEnumerable<RewardItem> rewards, GameEntity entity, out string errorMessage)
		{
			List<IMerchantInventory> fromPool = StaticListPool<IMerchantInventory>.GetFromPool();
			foreach (RewardItem rewardItem in rewards)
			{
				fromPool.Add(rewardItem.Acquisition(entity));
			}
			bool result = fromPool.EntityCanAcquire(entity, out errorMessage);
			StaticListPool<IMerchantInventory>.ReturnToPool(fromPool);
			return result;
		}
	}
}
