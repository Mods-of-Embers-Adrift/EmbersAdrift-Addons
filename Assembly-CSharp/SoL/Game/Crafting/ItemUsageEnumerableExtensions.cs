using System;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.Crafting
{
	// Token: 0x02000CE7 RID: 3303
	public static class ItemUsageEnumerableExtensions
	{
		// Token: 0x06006404 RID: 25604 RVA: 0x002081F4 File Offset: 0x002063F4
		public static bool AllItemsStillExist(this List<ItemUsage> list)
		{
			foreach (ItemUsage itemUsage in list)
			{
				if (itemUsage.Instance.InstanceId == UniqueId.Empty)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06006405 RID: 25605 RVA: 0x0020825C File Offset: 0x0020645C
		public static List<RecipeComponent> ComponentsFulfilled(this List<ItemUsage> list)
		{
			ItemUsageEnumerableExtensions.m_componentsUsed.Clear();
			foreach (ItemUsage itemUsage in list)
			{
				if (ItemUsageEnumerableExtensions.m_componentsUsed.IndexOf(itemUsage.UsedFor) == -1)
				{
					ItemUsageEnumerableExtensions.m_componentsUsed.Add(itemUsage.UsedFor);
				}
			}
			return ItemUsageEnumerableExtensions.m_componentsUsed;
		}

		// Token: 0x06006406 RID: 25606 RVA: 0x002082D8 File Offset: 0x002064D8
		public static void GetAggregateMaterialLevel(this List<ItemUsage> list, bool skipMissingMaterialLevels, out int lowestMinMaterialLevel, out int lowestMaxMaterialLevel, out int highestMinMaterialLevel, out int highestMaxMaterialLevel, out int averageMinMaterialLevel, out int averageMaxMaterialLevel)
		{
			lowestMinMaterialLevel = int.MaxValue;
			lowestMaxMaterialLevel = int.MaxValue;
			highestMinMaterialLevel = 0;
			highestMaxMaterialLevel = 0;
			averageMinMaterialLevel = 0;
			averageMaxMaterialLevel = 0;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (ItemUsage itemUsage in list)
			{
				ItemArchetype itemArchetype = itemUsage.Instance.Archetype as ItemArchetype;
				if (skipMissingMaterialLevels && (itemArchetype.MinimumMaterialLevel == null || itemArchetype.MaximumMaterialLevel == null))
				{
					num3++;
				}
				else
				{
					num += itemArchetype.MinimumMaterialLevel.GetValueOrDefault();
					num2 += itemArchetype.MaximumMaterialLevel.GetValueOrDefault();
					lowestMinMaterialLevel = Math.Min(itemArchetype.MinimumMaterialLevel ?? int.MaxValue, lowestMinMaterialLevel);
					lowestMaxMaterialLevel = Math.Min(itemArchetype.MaximumMaterialLevel ?? int.MaxValue, lowestMaxMaterialLevel);
					highestMinMaterialLevel = Math.Max(itemArchetype.MinimumMaterialLevel.GetValueOrDefault(), highestMinMaterialLevel);
					highestMaxMaterialLevel = Math.Max(itemArchetype.MaximumMaterialLevel.GetValueOrDefault(), highestMaxMaterialLevel);
				}
			}
			if (list.Count - num3 > 0)
			{
				averageMinMaterialLevel = Mathf.FloorToInt((float)num / (float)(list.Count - num3));
				averageMaxMaterialLevel = Mathf.FloorToInt((float)num2 / (float)(list.Count - num3));
			}
			if (lowestMinMaterialLevel == 2147483647)
			{
				lowestMinMaterialLevel = 0;
			}
			if (lowestMaxMaterialLevel == 2147483647)
			{
				lowestMaxMaterialLevel = 0;
			}
		}

		// Token: 0x040056F4 RID: 22260
		private static List<RecipeComponent> m_componentsUsed = new List<RecipeComponent>();
	}
}
