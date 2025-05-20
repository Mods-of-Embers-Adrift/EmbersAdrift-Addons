using System;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using UnityEngine;

namespace SoL.Game.Quests.Objectives
{
	// Token: 0x0200079C RID: 1948
	[CreateAssetMenu(menuName = "SoL/Quests/Objectives/CraftObjective")]
	public class CraftObjective : OrderDrivenObjective<CraftObjective>
	{
		// Token: 0x17000D34 RID: 3380
		// (get) Token: 0x0600397D RID: 14717 RVA: 0x00066E92 File Offset: 0x00065092
		private bool m_shouldShowItems
		{
			get
			{
				return this.m_type == CraftType.ItemPresent || this.m_type == CraftType.ItemWasMadeFromAll || this.m_type == CraftType.ItemWasMadeFromAny;
			}
		}

		// Token: 0x17000D35 RID: 3381
		// (get) Token: 0x0600397E RID: 14718 RVA: 0x00066EB1 File Offset: 0x000650B1
		private bool m_shouldShowAmountRequired
		{
			get
			{
				return this.m_type == CraftType.ItemPresent || this.m_type == CraftType.ItemFromMaterialCategories || this.m_type == CraftType.ItemWasMadeFromAll || this.m_type == CraftType.ItemWasMadeFromAny;
			}
		}

		// Token: 0x17000D36 RID: 3382
		// (get) Token: 0x0600397F RID: 14719 RVA: 0x00066ED9 File Offset: 0x000650D9
		public CraftType Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x17000D37 RID: 3383
		// (get) Token: 0x06003980 RID: 14720 RVA: 0x00066EE1 File Offset: 0x000650E1
		public Recipe Recipe
		{
			get
			{
				return this.m_recipe;
			}
		}

		// Token: 0x17000D38 RID: 3384
		// (get) Token: 0x06003981 RID: 14721 RVA: 0x00066EE9 File Offset: 0x000650E9
		public Recipe[] Recipes
		{
			get
			{
				return this.m_recipes;
			}
		}

		// Token: 0x17000D39 RID: 3385
		// (get) Token: 0x06003982 RID: 14722 RVA: 0x00066EF1 File Offset: 0x000650F1
		public CraftResult AllowedResults
		{
			get
			{
				return this.m_allowedResults;
			}
		}

		// Token: 0x06003983 RID: 14723 RVA: 0x00066EF9 File Offset: 0x000650F9
		public override bool Validate(GameEntity sourceEntity, ObjectiveIterationCache cache, out string message)
		{
			if (cache.IterationsRequested > 1)
			{
				message = "Too many iterations requested.";
				return false;
			}
			message = string.Empty;
			return false;
		}

		// Token: 0x06003984 RID: 14724 RVA: 0x00173048 File Offset: 0x00171248
		public bool IsValid(GameEntity entity, Recipe recipeUsed, CraftResult result)
		{
			if ((this.m_allowedResults & result) != result)
			{
				return false;
			}
			switch (this.m_type)
			{
			case CraftType.Any:
				return true;
			case CraftType.SpecificRecipes:
			{
				Recipe[] recipes = this.m_recipes;
				for (int i = 0; i < recipes.Length; i++)
				{
					if (recipes[i].Id == recipeUsed.Id)
					{
						return true;
					}
				}
				break;
			}
			case CraftType.RecipeIsOfMasteries:
			{
				MasteryArchetype[] masteries = this.m_masteries;
				for (int i = 0; i < masteries.Length; i++)
				{
					if (masteries[i].Id == recipeUsed.Mastery.Id)
					{
						return true;
					}
				}
				break;
			}
			case CraftType.RecipeIsOfAbilities:
			{
				AbilityArchetype[] abilities = this.m_abilities;
				for (int i = 0; i < abilities.Length; i++)
				{
					if (abilities[i].Id == recipeUsed.Ability.Id)
					{
						return true;
					}
				}
				break;
			}
			case CraftType.ItemPresent:
			case CraftType.ItemFromMaterialCategories:
			case CraftType.ItemWasMadeFromAll:
			case CraftType.ItemWasMadeFromAny:
				return this.HasItems(entity);
			}
			return false;
		}

		// Token: 0x06003985 RID: 14725 RVA: 0x00066F15 File Offset: 0x00065115
		public bool HasItems(GameEntity entity)
		{
			return this.GetAvailableItems(entity, null) >= this.m_amountRequired;
		}

		// Token: 0x06003986 RID: 14726 RVA: 0x00173138 File Offset: 0x00171338
		public int GetAvailableItems(GameEntity entity, List<ArchetypeInstance> availableItems = null)
		{
			ContainerInstance containerInstance;
			ContainerInstance containerInstance2;
			ContainerInstance containerInstance3;
			ContainerInstance containerInstance4;
			if (entity != null && entity.CollectionController.TryGetInstance(ContainerType.Inventory, out containerInstance) && entity.CollectionController.TryGetInstance(ContainerType.Gathering, out containerInstance2) && entity.CollectionController.TryGetInstance(ContainerType.Pouch, out containerInstance3) && entity.CollectionController.TryGetInstance(ContainerType.ReagentPouch, out containerInstance4))
			{
				int num = 0;
				if (availableItems != null)
				{
					availableItems.Clear();
				}
				foreach (ArchetypeInstance archetypeInstance in containerInstance.Instances)
				{
					if (this.Matches(archetypeInstance))
					{
						if (availableItems != null)
						{
							availableItems.Add(archetypeInstance);
						}
						int num2 = num;
						ItemInstanceData itemData = archetypeInstance.ItemData;
						num = num2 + (((itemData != null) ? itemData.Count : null) ?? 1);
					}
				}
				foreach (ArchetypeInstance archetypeInstance2 in containerInstance2.Instances)
				{
					if (this.Matches(archetypeInstance2))
					{
						if (availableItems != null)
						{
							availableItems.Add(archetypeInstance2);
						}
						int num3 = num;
						ItemInstanceData itemData2 = archetypeInstance2.ItemData;
						num = num3 + (((itemData2 != null) ? itemData2.Count : null) ?? 1);
					}
				}
				foreach (ArchetypeInstance archetypeInstance3 in containerInstance3.Instances)
				{
					if (this.Matches(archetypeInstance3))
					{
						if (availableItems != null)
						{
							availableItems.Add(archetypeInstance3);
						}
						int num4 = num;
						ItemInstanceData itemData3 = archetypeInstance3.ItemData;
						num = num4 + (((itemData3 != null) ? itemData3.Count : null) ?? 1);
					}
				}
				foreach (ArchetypeInstance archetypeInstance4 in containerInstance4.Instances)
				{
					if (this.Matches(archetypeInstance4))
					{
						if (availableItems != null)
						{
							availableItems.Add(archetypeInstance4);
						}
						int num5 = num;
						ItemInstanceData itemData4 = archetypeInstance4.ItemData;
						num = num5 + (((itemData4 != null) ? itemData4.Count : null) ?? 1);
					}
				}
				return num;
			}
			return 0;
		}

		// Token: 0x06003987 RID: 14727 RVA: 0x001733CC File Offset: 0x001715CC
		private bool Matches(ArchetypeInstance instance)
		{
			switch (this.m_type)
			{
			case CraftType.ItemPresent:
				foreach (ItemArchetype itemArchetype in this.m_items)
				{
					if (instance.ArchetypeId == itemArchetype.Id)
					{
						return true;
					}
				}
				return false;
			case CraftType.ItemFromMaterialCategories:
				foreach (MaterialCategory y in this.m_categories)
				{
					if (((ItemArchetype)instance.Archetype).MaterialCategory == y)
					{
						return true;
					}
				}
				return false;
			case CraftType.ItemWasMadeFromAll:
				return ((ItemArchetype)instance.Archetype).WasMadeFromAll(instance, this.m_items, null);
			case CraftType.ItemWasMadeFromAny:
				return ((ItemArchetype)instance.Archetype).WasMadeFromAny(instance, this.m_items, null);
			default:
				return false;
			}
		}

		// Token: 0x04003834 RID: 14388
		[SerializeField]
		private CraftType m_type;

		// Token: 0x04003835 RID: 14389
		[SerializeField]
		private Recipe m_recipe;

		// Token: 0x04003836 RID: 14390
		[SerializeField]
		private Recipe[] m_recipes;

		// Token: 0x04003837 RID: 14391
		[SerializeField]
		private MasteryArchetype[] m_masteries;

		// Token: 0x04003838 RID: 14392
		[SerializeField]
		private AbilityArchetype[] m_abilities;

		// Token: 0x04003839 RID: 14393
		[SerializeField]
		private ItemArchetype[] m_items;

		// Token: 0x0400383A RID: 14394
		[SerializeField]
		private MaterialCategory[] m_categories;

		// Token: 0x0400383B RID: 14395
		[Min(1f)]
		[SerializeField]
		private int m_amountRequired = 1;

		// Token: 0x0400383C RID: 14396
		[SerializeField]
		private CraftResult m_allowedResults = CraftResult.Success;
	}
}
