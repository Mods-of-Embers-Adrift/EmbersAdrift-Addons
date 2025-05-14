using System;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A5C RID: 2652
	public static class ConsumableCategoryExtensions
	{
		// Token: 0x17001290 RID: 4752
		// (get) Token: 0x06005220 RID: 21024 RVA: 0x001D31A4 File Offset: 0x001D13A4
		public static ConsumableCategory[] ConsumableCategories
		{
			get
			{
				if (ConsumableCategoryExtensions.m_consumableCategories == null)
				{
					ConsumableCategory[] array = (ConsumableCategory[])Enum.GetValues(typeof(ConsumableCategory));
					ConsumableCategoryExtensions.m_consumableCategories = new ConsumableCategory[array.Length - 1];
					int num = 0;
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] != ConsumableCategory.None)
						{
							ConsumableCategoryExtensions.m_consumableCategories[num] = array[i];
							num++;
						}
					}
				}
				return ConsumableCategoryExtensions.m_consumableCategories;
			}
		}

		// Token: 0x06005221 RID: 21025 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this ConsumableCategory a, ConsumableCategory b)
		{
			return (a & b) == b;
		}

		// Token: 0x06005222 RID: 21026 RVA: 0x000578B5 File Offset: 0x00055AB5
		public static ConsumableCategory SetBitFlag(this ConsumableCategory a, ConsumableCategory b)
		{
			return a | b;
		}

		// Token: 0x06005223 RID: 21027 RVA: 0x000578BA File Offset: 0x00055ABA
		public static ConsumableCategory UnsetBitFlag(this ConsumableCategory a, ConsumableCategory b)
		{
			return a & ~b;
		}

		// Token: 0x06005224 RID: 21028 RVA: 0x00076CFC File Offset: 0x00074EFC
		public static float GetCategoryCooldown(this ConsumableCategory category)
		{
			if (category == ConsumableCategory.HealthPotion)
			{
				return 120f;
			}
			if (category == ConsumableCategory.ResistPotion)
			{
				return 240f;
			}
			if (category != ConsumableCategory.CraftingStation)
			{
				return 60f;
			}
			return 300f;
		}

		// Token: 0x04004992 RID: 18834
		private static ConsumableCategory[] m_consumableCategories;
	}
}
