using System;
using System.Collections.Generic;
using SoL.Game.Crafting;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Objects
{
	// Token: 0x020009EF RID: 2543
	[CreateAssetMenu(menuName = "SoL/Profiles/Tiered Item Profile")]
	public class TieredItemProfile : ScriptableObject
	{
		// Token: 0x17001112 RID: 4370
		// (get) Token: 0x06004D5B RID: 19803 RVA: 0x00074423 File Offset: 0x00072623
		public ItemTier[] Tiers
		{
			get
			{
				return this.m_tiers;
			}
		}

		// Token: 0x17001113 RID: 4371
		// (get) Token: 0x06004D5C RID: 19804 RVA: 0x0007442B File Offset: 0x0007262B
		public MaterialLevelAggregationType MaterialTieringHint
		{
			get
			{
				return this.m_materialTieringHint;
			}
		}

		// Token: 0x06004D5D RID: 19805 RVA: 0x001C02AC File Offset: 0x001BE4AC
		public List<ItemArchetype> GetAllItems()
		{
			List<ItemArchetype> fromPool = StaticListPool<ItemArchetype>.GetFromPool();
			if (this.m_tiers != null)
			{
				foreach (ItemTier itemTier in this.m_tiers)
				{
					if (itemTier != null)
					{
						fromPool.Add(itemTier.Item);
					}
				}
			}
			return fromPool;
		}

		// Token: 0x06004D5E RID: 19806 RVA: 0x001C02F0 File Offset: 0x001BE4F0
		public ItemArchetype GetItemForLevel(int level)
		{
			ItemArchetype result = null;
			if (this.m_tiers != null)
			{
				foreach (ItemTier itemTier in this.m_tiers)
				{
					if (itemTier != null && level >= itemTier.MinimumLevel)
					{
						result = itemTier.Item;
					}
				}
			}
			return result;
		}

		// Token: 0x04004715 RID: 18197
		[SerializeField]
		private ItemTier[] m_tiers;

		// Token: 0x04004716 RID: 18198
		[Tooltip("Provides the utilizing recipe with a hint as to how this profile should be processed if tiering by material.")]
		[SerializeField]
		private MaterialLevelAggregationType m_materialTieringHint;
	}
}
