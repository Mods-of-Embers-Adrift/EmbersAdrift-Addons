using System;
using System.Collections.Generic;
using SoL.Game.Crafting;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AC3 RID: 2755
	[Serializable]
	public class OutputItemOverride
	{
		// Token: 0x060054F3 RID: 21747 RVA: 0x001DBEA8 File Offset: 0x001DA0A8
		public bool MeetsConditions(List<ItemUsage> itemsUsed)
		{
			switch (this.ConditionType)
			{
			case OverrideConditionType.WasMadeFromAll:
			{
				bool flag = true;
				foreach (ItemArchetype itemArchetype in this.WasMadeFrom)
				{
					bool flag2 = false;
					foreach (ItemUsage itemUsage in itemsUsed)
					{
						if (!(itemUsage.Instance.ArchetypeId == itemArchetype.Id))
						{
							ItemComponentTree itemComponentTree = itemUsage.Instance.ItemData.ItemComponentTree;
							if (itemComponentTree == null || !itemComponentTree.ContainsArchetype(itemArchetype))
							{
								continue;
							}
						}
						flag2 = true;
						break;
					}
					if (!flag2)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					return true;
				}
				break;
			}
			case OverrideConditionType.WasMadeFromAny:
				foreach (ItemArchetype itemArchetype2 in this.WasMadeFrom)
				{
					foreach (ItemUsage itemUsage2 in itemsUsed)
					{
						if (!(itemUsage2.Instance.ArchetypeId == itemArchetype2.Id))
						{
							ItemComponentTree itemComponentTree2 = itemUsage2.Instance.ItemData.ItemComponentTree;
							if (itemComponentTree2 == null || !itemComponentTree2.ContainsArchetype(itemArchetype2))
							{
								continue;
							}
						}
						return true;
					}
				}
				break;
			case OverrideConditionType.WasMadeFromOnly:
			{
				bool flag3 = false;
				bool flag4 = false;
				foreach (ItemArchetype itemArchetype3 in this.WasMadeFrom)
				{
					foreach (ItemUsage itemUsage3 in itemsUsed)
					{
						if (!(itemUsage3.Instance.ArchetypeId == itemArchetype3.Id))
						{
							ItemComponentTree itemComponentTree3 = itemUsage3.Instance.ItemData.ItemComponentTree;
							if (itemComponentTree3 == null || !itemComponentTree3.ContainsArchetype(itemArchetype3))
							{
								flag4 = true;
								continue;
							}
						}
						flag3 = true;
					}
					if (flag4)
					{
						break;
					}
				}
				if (flag3 && !flag4)
				{
					return true;
				}
				break;
			}
			}
			return false;
		}

		// Token: 0x060054F4 RID: 21748 RVA: 0x001DC0D0 File Offset: 0x001DA2D0
		public ItemArchetype GetOverrideItem(int materialLevel, float abilityLevel)
		{
			RecipeOutputType outputType = this.OutputType;
			if (outputType != RecipeOutputType.MaterialTiered)
			{
				if (outputType != RecipeOutputType.AbilityTiered)
				{
					return this.OverrideItem;
				}
				TieredItemProfile tierProfile = this.TierProfile;
				if (tierProfile == null)
				{
					return null;
				}
				return tierProfile.GetItemForLevel((int)abilityLevel);
			}
			else
			{
				TieredItemProfile tierProfile2 = this.TierProfile;
				if (tierProfile2 == null)
				{
					return null;
				}
				return tierProfile2.GetItemForLevel(materialLevel);
			}
		}

		// Token: 0x04004B81 RID: 19329
		public OverrideConditionType ConditionType;

		// Token: 0x04004B82 RID: 19330
		public ItemArchetype[] WasMadeFrom;

		// Token: 0x04004B83 RID: 19331
		public RecipeOutputType OutputType;

		// Token: 0x04004B84 RID: 19332
		public ItemArchetype OverrideItem;

		// Token: 0x04004B85 RID: 19333
		public ComponentSelection[] MaterialTierComponentSelection;

		// Token: 0x04004B86 RID: 19334
		public MaterialLevelAggregationType MaterialLevelAggregationOverride;

		// Token: 0x04004B87 RID: 19335
		public TieredItemProfile TierProfile;
	}
}
