using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.Crafting;
using SoL.Game.Objects.Containers;
using SoL.Game.Transactions;
using SoL.Managers;
using SoL.Networking;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AC4 RID: 2756
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Crafting/Recipe")]
	public class Recipe : LearnableArchetype
	{
		// Token: 0x1700138A RID: 5002
		// (get) Token: 0x060054F6 RID: 21750 RVA: 0x001DC0C0 File Offset: 0x001DA2C0
		public override Sprite Icon
		{
			get
			{
				if (base.Icon != null)
				{
					return base.Icon;
				}
				switch (this.m_outputType)
				{
				case RecipeOutputType.Static:
					return this.m_itemToCreate.Icon;
				case RecipeOutputType.MaterialTiered:
				{
					TieredItemProfile tierProfile = this.m_tierProfile;
					if (tierProfile == null)
					{
						return null;
					}
					ItemArchetype itemForLevel = tierProfile.GetItemForLevel(1);
					if (itemForLevel == null)
					{
						return null;
					}
					return itemForLevel.Icon;
				}
				case RecipeOutputType.AbilityTiered:
				{
					ArchetypeInstance archetypeInstance;
					if (LocalPlayer.GameEntity != null && LocalPlayer.GameEntity.CollectionController.Abilities.TryGetInstanceForArchetypeId(this.m_ability.Id, out archetypeInstance))
					{
						TieredItemProfile tierProfile2 = this.m_tierProfile;
						if (tierProfile2 == null)
						{
							return null;
						}
						ItemArchetype itemForLevel2 = tierProfile2.GetItemForLevel(archetypeInstance.GetAssociatedLevelInteger(LocalPlayer.GameEntity));
						if (itemForLevel2 == null)
						{
							return null;
						}
						return itemForLevel2.Icon;
					}
					else
					{
						TieredItemProfile tierProfile3 = this.m_tierProfile;
						if (tierProfile3 == null)
						{
							return null;
						}
						ItemArchetype itemForLevel3 = tierProfile3.GetItemForLevel(1);
						if (itemForLevel3 == null)
						{
							return null;
						}
						return itemForLevel3.Icon;
					}
					break;
				}
				default:
					return null;
				}
			}
		}

		// Token: 0x1700138B RID: 5003
		// (get) Token: 0x060054F7 RID: 21751 RVA: 0x00078CC6 File Offset: 0x00076EC6
		private bool m_outputType_static
		{
			get
			{
				return this.m_outputType == RecipeOutputType.Static;
			}
		}

		// Token: 0x1700138C RID: 5004
		// (get) Token: 0x060054F8 RID: 21752 RVA: 0x00078CD1 File Offset: 0x00076ED1
		private bool m_outputType_materialTiered
		{
			get
			{
				return this.m_outputType == RecipeOutputType.MaterialTiered;
			}
		}

		// Token: 0x1700138D RID: 5005
		// (get) Token: 0x060054F9 RID: 21753 RVA: 0x00078CDC File Offset: 0x00076EDC
		private bool m_outputType_abilityTiered
		{
			get
			{
				return this.m_outputType == RecipeOutputType.AbilityTiered;
			}
		}

		// Token: 0x1700138E RID: 5006
		// (get) Token: 0x060054FA RID: 21754 RVA: 0x00078CE7 File Offset: 0x00076EE7
		private bool m_outputType_tiered
		{
			get
			{
				return this.m_outputType_materialTiered || this.m_outputType_abilityTiered;
			}
		}

		// Token: 0x1700138F RID: 5007
		// (get) Token: 0x060054FB RID: 21755 RVA: 0x00078CF9 File Offset: 0x00076EF9
		public MasteryArchetype Mastery
		{
			get
			{
				if (!this.m_ability)
				{
					return null;
				}
				return this.m_ability.Mastery;
			}
		}

		// Token: 0x17001390 RID: 5008
		// (get) Token: 0x060054FC RID: 21756 RVA: 0x00078D15 File Offset: 0x00076F15
		public AbilityArchetype Ability
		{
			get
			{
				return this.m_ability;
			}
		}

		// Token: 0x17001391 RID: 5009
		// (get) Token: 0x060054FD RID: 21757 RVA: 0x00078D1D File Offset: 0x00076F1D
		public OutputItemOverride[] OutputOverrides
		{
			get
			{
				return this.m_outputOverrides;
			}
		}

		// Token: 0x17001392 RID: 5010
		// (get) Token: 0x060054FE RID: 21758 RVA: 0x00078D25 File Offset: 0x00076F25
		public RecipeOutputType OutputType
		{
			get
			{
				return this.m_outputType;
			}
		}

		// Token: 0x17001393 RID: 5011
		// (get) Token: 0x060054FF RID: 21759 RVA: 0x00078D2D File Offset: 0x00076F2D
		public TieredItemProfile TierProfile
		{
			get
			{
				return this.m_tierProfile;
			}
		}

		// Token: 0x17001394 RID: 5012
		// (get) Token: 0x06005500 RID: 21760 RVA: 0x00078D35 File Offset: 0x00076F35
		public ItemArchetype ItemToCreateOnFailure
		{
			get
			{
				return this.m_itemToCreateOnFailure;
			}
		}

		// Token: 0x17001395 RID: 5013
		// (get) Token: 0x06005501 RID: 21761 RVA: 0x00078D3D File Offset: 0x00076F3D
		public bool ForceOneComponentPerStack
		{
			get
			{
				return this.m_forceOneComponentPerStack;
			}
		}

		// Token: 0x17001396 RID: 5014
		// (get) Token: 0x06005502 RID: 21762 RVA: 0x00078D45 File Offset: 0x00076F45
		public RecipeComponent[] Components
		{
			get
			{
				return this.m_components;
			}
		}

		// Token: 0x17001397 RID: 5015
		// (get) Token: 0x06005503 RID: 21763 RVA: 0x00078D4D File Offset: 0x00076F4D
		public CraftingStationCategory StationCategory
		{
			get
			{
				return this.m_stationCategory;
			}
		}

		// Token: 0x17001398 RID: 5016
		// (get) Token: 0x06005504 RID: 21764 RVA: 0x00078D55 File Offset: 0x00076F55
		public bool DisableCraftedTag
		{
			get
			{
				return this.m_disableCraftedTag;
			}
		}

		// Token: 0x17001399 RID: 5017
		// (get) Token: 0x06005505 RID: 21765 RVA: 0x00078D5D File Offset: 0x00076F5D
		public bool DisableHistory
		{
			get
			{
				return this.m_disableHistory;
			}
		}

		// Token: 0x1700139A RID: 5018
		// (get) Token: 0x06005506 RID: 21766 RVA: 0x00078D65 File Offset: 0x00076F65
		public bool QualityModifierEnabled
		{
			get
			{
				return this.m_qualityModifierEnabled;
			}
		}

		// Token: 0x1700139B RID: 5019
		// (get) Token: 0x06005507 RID: 21767 RVA: 0x00078D6D File Offset: 0x00076F6D
		public RecipeSubstitution[] Substitutions
		{
			get
			{
				return this.m_substitutions;
			}
		}

		// Token: 0x1700139C RID: 5020
		// (get) Token: 0x06005508 RID: 21768 RVA: 0x00078D75 File Offset: 0x00076F75
		public int MinimumAbilityLevel
		{
			get
			{
				return this.m_minimumAbilityLevel;
			}
		}

		// Token: 0x1700139D RID: 5021
		// (get) Token: 0x06005509 RID: 21769 RVA: 0x00078D7D File Offset: 0x00076F7D
		public RecipeProductionType ProductionType
		{
			get
			{
				return this.m_productionType;
			}
		}

		// Token: 0x1700139E RID: 5022
		// (get) Token: 0x0600550A RID: 21770 RVA: 0x00078D85 File Offset: 0x00076F85
		public int ConstantAmountToProduce
		{
			get
			{
				return this.m_amountToProduce;
			}
		}

		// Token: 0x1700139F RID: 5023
		// (get) Token: 0x0600550B RID: 21771 RVA: 0x00078D8D File Offset: 0x00076F8D
		public int LevelDeltaMinimumAmountToProduce
		{
			get
			{
				return this.m_minimumQuantity;
			}
		}

		// Token: 0x170013A0 RID: 5024
		// (get) Token: 0x0600550C RID: 21772 RVA: 0x00078D95 File Offset: 0x00076F95
		public int LevelDeltaMaximumAmountToProduce
		{
			get
			{
				return this.m_maximumQuantity;
			}
		}

		// Token: 0x170013A1 RID: 5025
		// (get) Token: 0x0600550D RID: 21773 RVA: 0x00078D9D File Offset: 0x00076F9D
		public float ExperienceMultiplier
		{
			get
			{
				return this.m_experienceMultiplier;
			}
		}

		// Token: 0x0600550E RID: 21774 RVA: 0x001DC1A4 File Offset: 0x001DA3A4
		public override bool EntityCanAcquire(GameEntity entity, out string errorMessage)
		{
			errorMessage = string.Empty;
			if (entity == null || entity.CollectionController == null)
			{
				return false;
			}
			ArchetypeInstance archetypeInstance;
			ArchetypeInstance archetypeInstance2;
			if (!entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(this.Mastery.Id, out archetypeInstance) || !entity.CollectionController.Abilities.TryGetInstanceForArchetypeId(this.Ability.Id, out archetypeInstance2))
			{
				errorMessage = "Mastery or ability requirements not met!";
				return false;
			}
			if (archetypeInstance2.GetAssociatedLevelInteger(entity) < this.m_minimumAbilityLevel)
			{
				errorMessage = "Ability level requirement not met!";
				return false;
			}
			LearnableContainerInstance learnableContainerInstance;
			LearnableArchetype learnableArchetype;
			if (entity.CollectionController.TryGetLearnableInstance(ContainerType.Recipes, out learnableContainerInstance) && learnableContainerInstance.TryGetLearnableForId(base.Id, out learnableArchetype))
			{
				errorMessage = "Recipe already known!";
				return false;
			}
			return true;
		}

		// Token: 0x0600550F RID: 21775 RVA: 0x001DC258 File Offset: 0x001DA458
		public override bool AddToPlayer(GameEntity entity, ItemAddContext context, uint quantity, ItemFlags itemFlags, bool markAsSoulbound, out ArchetypeInstance resultingInstance)
		{
			resultingInstance = null;
			if (!GameManager.IsServer)
			{
				return false;
			}
			LearnableContainerInstance learnableContainerInstance;
			string text;
			if (entity.CollectionController.TryGetLearnableInstance(ContainerType.Recipes, out learnableContainerInstance) && this.EntityCanAcquire(entity, out text))
			{
				learnableContainerInstance.Add(this, true);
				this.m_tempIdArray[0] = base.Id;
				LearnablesAddedTransaction transaction = new LearnablesAddedTransaction
				{
					Op = OpCodes.Ok,
					LearnableIds = this.m_tempIdArray,
					TargetContainer = learnableContainerInstance.Id
				};
				entity.NetworkEntity.PlayerRpcHandler.LearnablesAdded(transaction);
				return true;
			}
			return false;
		}

		// Token: 0x170013A2 RID: 5026
		// (get) Token: 0x06005510 RID: 21776 RVA: 0x00078DA5 File Offset: 0x00076FA5
		public ItemArchetype DefaultItemToCreate
		{
			get
			{
				ItemArchetype result;
				if ((result = this.m_itemToCreate) == null)
				{
					TieredItemProfile tierProfile = this.m_tierProfile;
					if (tierProfile == null)
					{
						return null;
					}
					result = tierProfile.GetItemForLevel(100);
				}
				return result;
			}
		}

		// Token: 0x06005511 RID: 21777 RVA: 0x001DC2E8 File Offset: 0x001DA4E8
		public static Recipe FindRecipeForComponentId(UniqueId componentId)
		{
			foreach (BaseArchetype baseArchetype in InternalGameDatabase.Archetypes.GetAllItems())
			{
				Recipe recipe = baseArchetype as Recipe;
				if (recipe != null)
				{
					RecipeComponent[] components = recipe.Components;
					for (int i = 0; i < components.Length; i++)
					{
						if (components[i].Id == componentId)
						{
							return recipe;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06005512 RID: 21778 RVA: 0x001DC368 File Offset: 0x001DA568
		public OutputItemOverride FindApplicableOverride(List<ItemUsage> itemsUsed)
		{
			if (this.m_outputOverrides != null)
			{
				foreach (OutputItemOverride outputItemOverride in this.m_outputOverrides)
				{
					if (outputItemOverride.WasMadeFrom != null && outputItemOverride.WasMadeFrom.Length != 0 && outputItemOverride.MeetsConditions(itemsUsed))
					{
						return outputItemOverride;
					}
				}
			}
			return null;
		}

		// Token: 0x06005513 RID: 21779 RVA: 0x001DC3B4 File Offset: 0x001DA5B4
		public bool IsApplicableOverrideAbilityTiered(List<ItemUsage> itemsUsed)
		{
			OutputItemOverride outputItemOverride = this.FindApplicableOverride(itemsUsed);
			if (outputItemOverride != null)
			{
				return outputItemOverride.OutputType == RecipeOutputType.AbilityTiered;
			}
			return this.OutputType == RecipeOutputType.AbilityTiered;
		}

		// Token: 0x06005514 RID: 21780 RVA: 0x001DC3E0 File Offset: 0x001DA5E0
		public ItemArchetype GetItemToCreate(List<ItemUsage> itemsUsed, float abilityLevel)
		{
			if (itemsUsed == null || itemsUsed.Count == 0)
			{
				return this.m_itemToCreate;
			}
			int level = 1;
			OutputItemOverride outputItemOverride = this.FindApplicableOverride(itemsUsed);
			TieredItemProfile tieredItemProfile = (outputItemOverride != null) ? outputItemOverride.TierProfile : this.m_tierProfile;
			RecipeOutputType recipeOutputType = (outputItemOverride != null) ? outputItemOverride.OutputType : this.m_outputType;
			ItemArchetype result = (outputItemOverride != null) ? outputItemOverride.OverrideItem : this.m_itemToCreate;
			if (recipeOutputType == RecipeOutputType.MaterialTiered)
			{
				ComponentSelection[] array = (outputItemOverride != null) ? outputItemOverride.MaterialTierComponentSelection : this.m_materialTierComponentSelection;
				MaterialLevelAggregationType materialLevelAggregationType = (outputItemOverride != null) ? outputItemOverride.MaterialLevelAggregationOverride : this.m_materialLevelAggregationOverride;
				this.m_tempMaterialAggregationLists.Clear();
				foreach (ItemUsage item in itemsUsed)
				{
					foreach (ComponentSelection componentSelection in array)
					{
						if (componentSelection.ComponentId == item.UsedFor.Id && componentSelection.Selected)
						{
							this.m_tempMaterialAggregationLists.Add(item);
						}
					}
				}
				int num;
				int num2;
				int num3;
				int num4;
				int num5;
				int num6;
				this.m_tempMaterialAggregationLists.GetAggregateMaterialLevel(false, out num, out num2, out num3, out num4, out num5, out num6);
				if (num == 0 && num2 == 0 && num3 == 0 && num4 == 0)
				{
					Debug.LogError(string.Format("Material-level aggregation returned all 0's, please regenerate the component selection list for this recipe! (Recipe ID: {0})", this.m_id));
				}
				MaterialLevelAggregationType? materialLevelAggregationType2 = (materialLevelAggregationType != MaterialLevelAggregationType.Unspecified) ? new MaterialLevelAggregationType?(materialLevelAggregationType) : ((tieredItemProfile == null || tieredItemProfile.MaterialTieringHint > MaterialLevelAggregationType.Unspecified) ? ((tieredItemProfile != null) ? new MaterialLevelAggregationType?(tieredItemProfile.MaterialTieringHint) : null) : new MaterialLevelAggregationType?(MaterialLevelAggregationType.HighestMinimum));
				if (materialLevelAggregationType2 != null)
				{
					switch (materialLevelAggregationType2.GetValueOrDefault())
					{
					case MaterialLevelAggregationType.LowestMinimum:
						level = num;
						break;
					case MaterialLevelAggregationType.LowestMaximum:
						level = num2;
						break;
					case MaterialLevelAggregationType.HighestMinimum:
						level = num3;
						break;
					case MaterialLevelAggregationType.HighestMaximum:
						level = num4;
						break;
					case MaterialLevelAggregationType.AverageMinimum:
						level = num5;
						break;
					case MaterialLevelAggregationType.AverageMaximum:
						level = num6;
						break;
					}
				}
			}
			if (recipeOutputType != RecipeOutputType.MaterialTiered)
			{
				if (recipeOutputType != RecipeOutputType.AbilityTiered)
				{
					return result;
				}
				if (tieredItemProfile == null)
				{
					return null;
				}
				return tieredItemProfile.GetItemForLevel((int)abilityLevel);
			}
			else
			{
				if (tieredItemProfile == null)
				{
					return null;
				}
				return tieredItemProfile.GetItemForLevel(level);
			}
		}

		// Token: 0x06005515 RID: 21781 RVA: 0x001DC600 File Offset: 0x001DA800
		public List<ItemArchetype> GetAllPossibleOutputItems()
		{
			List<ItemArchetype> fromPool = StaticListPool<ItemArchetype>.GetFromPool();
			if (this.m_outputType == RecipeOutputType.Static && this.m_itemToCreate)
			{
				fromPool.Add(this.m_itemToCreate);
			}
			else if (this.m_outputType_tiered && this.m_tierProfile)
			{
				List<ItemArchetype> allItems = this.m_tierProfile.GetAllItems();
				fromPool.AddRange(allItems);
				StaticListPool<ItemArchetype>.ReturnToPool(allItems);
			}
			if (this.m_outputOverrides != null)
			{
				foreach (OutputItemOverride outputItemOverride in this.m_outputOverrides)
				{
					if (outputItemOverride.OutputType == RecipeOutputType.Static && outputItemOverride.OverrideItem)
					{
						fromPool.Add(outputItemOverride.OverrideItem);
					}
					else if ((outputItemOverride.OutputType == RecipeOutputType.MaterialTiered || outputItemOverride.OutputType == RecipeOutputType.AbilityTiered) && outputItemOverride.TierProfile)
					{
						List<ItemArchetype> allItems2 = outputItemOverride.TierProfile.GetAllItems();
						fromPool.AddRange(allItems2);
						StaticListPool<ItemArchetype>.ReturnToPool(allItems2);
					}
				}
			}
			return fromPool;
		}

		// Token: 0x06005516 RID: 21782 RVA: 0x001DC6EC File Offset: 0x001DA8EC
		public bool PerformFailureCheck(float level, List<ItemUsage> itemsUsed)
		{
			RecipeProductionType productionType = this.m_productionType;
			if (productionType == RecipeProductionType.Stepwise)
			{
				int num = 0;
				foreach (QuantityStep quantityStep in this.m_steps)
				{
					if (level >= (float)quantityStep.Level.x && level <= (float)quantityStep.Level.y)
					{
						num = Math.Max(num, quantityStep.AmountToProduce);
					}
				}
				return num != 0;
			}
			if (productionType != RecipeProductionType.LevelDelta)
			{
				return true;
			}
			int i;
			int num2;
			int num3;
			int num4;
			int num5;
			int num6;
			itemsUsed.GetAggregateMaterialLevel(false, out i, out num2, out num3, out num4, out num5, out num6);
			if (num3 >= num4)
			{
				return true;
			}
			float num7 = (level - (float)num3) / (float)(num4 - num3);
			if (num7 < 0f)
			{
				return false;
			}
			if (num7 > 1f)
			{
				return true;
			}
			float num8 = (1f - num7) * this.m_maximumFailureRate;
			return UnityEngine.Random.Range(0f, 1f) >= num8;
		}

		// Token: 0x06005517 RID: 21783 RVA: 0x001DC7C4 File Offset: 0x001DA9C4
		public int GetAmountToProduce(float level, List<ItemUsage> itemsUsed)
		{
			RecipeProductionType productionType = this.m_productionType;
			if (productionType == RecipeProductionType.Stepwise)
			{
				int num = 1;
				foreach (QuantityStep quantityStep in this.m_steps)
				{
					if (level >= (float)quantityStep.Level.x && level <= (float)quantityStep.Level.y)
					{
						num = Math.Max(num, quantityStep.AmountToProduce);
					}
				}
				return num;
			}
			if (productionType != RecipeProductionType.LevelDelta)
			{
				return this.m_amountToProduce;
			}
			int i;
			int num2;
			int num3;
			int num4;
			int num5;
			int num6;
			itemsUsed.GetAggregateMaterialLevel(false, out i, out num2, out num3, out num4, out num5, out num6);
			if (num3 >= num4)
			{
				return this.m_maximumQuantity;
			}
			float t = (level - (float)num3) / (float)(num4 - num3);
			return (int)Mathf.Lerp((float)this.m_minimumQuantity, (float)this.m_maximumQuantity, t);
		}

		// Token: 0x06005518 RID: 21784 RVA: 0x001DC87C File Offset: 0x001DAA7C
		public bool Match(IEnumerable<ArchetypeInstance> inputItems, out List<ItemUsage> itemsUsed, out RecipeComponent failedComponent)
		{
			itemsUsed = Recipe.m_itemsUsed;
			itemsUsed.Clear();
			failedComponent = null;
			if ((this.m_outputType_static && !this.m_itemToCreate) || (this.m_outputType_tiered && !this.m_tierProfile) || (this.m_outputType_tiered && !this.m_tierProfile.GetItemForLevel(100)))
			{
				Debug.LogError("Could not find an item to create. This recipe is misconfigured!");
				return false;
			}
			if (this.m_components == null)
			{
				Debug.LogError("Components is null on recipe: " + base.name);
				return false;
			}
			List<ArchetypeInstance> fromPool = StaticListPool<ArchetypeInstance>.GetFromPool();
			fromPool.AddRange(inputItems);
			bool result = true;
			foreach (RecipeComponent recipeComponent in this.m_components)
			{
				if (recipeComponent != null && recipeComponent.Enabled)
				{
					List<ItemUsage> collection;
					if (recipeComponent.TryGetMatchingMaterials(fromPool, itemsUsed, this.m_forceOneComponentPerStack, out collection))
					{
						itemsUsed.AddRange(collection);
					}
					else if (recipeComponent.RequirementType != ComponentRequirementType.Optional)
					{
						failedComponent = recipeComponent;
						result = false;
						break;
					}
				}
			}
			StaticListPool<ArchetypeInstance>.ReturnToPool(fromPool);
			return result;
		}

		// Token: 0x06005519 RID: 21785 RVA: 0x001DC97C File Offset: 0x001DAB7C
		public bool IsMatch(IEnumerable<ArchetypeInstance> inputItems, List<ItemUsage> itemsUsed, out string failureMessage)
		{
			failureMessage = null;
			if ((this.m_outputType_static && !this.m_itemToCreate) || (this.m_outputType_tiered && !this.m_tierProfile) || (this.m_outputType_tiered && !this.m_tierProfile.GetItemForLevel(100)))
			{
				Debug.LogError("Could not find an item to create. This recipe is misconfigured!");
				failureMessage = "This recipe is misconfigured, please report this bug!";
				return false;
			}
			if (this.m_components == null)
			{
				Debug.LogError("Components is null on recipe: " + base.name);
				failureMessage = "This recipe is misconfigured, please report this bug!";
				return false;
			}
			this.m_amountsConsumed.Clear();
			foreach (ArchetypeInstance archetypeInstance in inputItems)
			{
				this.m_amountsConsumed.Add(archetypeInstance.InstanceId, 0);
			}
			RecipeComponent recipeComponent = null;
			foreach (RecipeComponent recipeComponent2 in this.m_components)
			{
				if (recipeComponent2 != null && recipeComponent2.Enabled)
				{
					ComponentMaterial componentMaterial = null;
					int num = -1;
					foreach (ItemUsage itemUsage in itemsUsed)
					{
						if (itemUsage.UsedFor == recipeComponent2)
						{
							if (componentMaterial == null)
							{
								componentMaterial = itemUsage.FindFulfilledMaterial();
							}
							else if (componentMaterial != itemUsage.FindFulfilledMaterial())
							{
								Debug.LogError("Switched materials mid-component... that shouldn't happen.");
								failureMessage = "An error occured processing the " + recipeComponent2.DisplayName + " component.";
								return false;
							}
							if (componentMaterial == null)
							{
								Debug.LogError("Iterating over an item usage whose fulfilled material cannot be deduced... that shouldn't happen.");
								failureMessage = "An error occured processing the " + recipeComponent2.DisplayName + " component.";
								return false;
							}
							if (num == -1)
							{
								num = componentMaterial.AmountRequired;
							}
							Dictionary<UniqueId, int> amountsConsumed = this.m_amountsConsumed;
							UniqueId instanceId = itemUsage.Instance.InstanceId;
							amountsConsumed[instanceId] += itemUsage.AmountUsed;
							num -= itemUsage.AmountUsed;
							if (num >= 0)
							{
								int num2 = this.m_amountsConsumed[itemUsage.Instance.InstanceId];
								ItemInstanceData itemData = itemUsage.Instance.ItemData;
								if (num2 <= (((itemData != null) ? itemData.Count : null) ?? 1))
								{
									continue;
								}
							}
							failureMessage = "There aren't enough materials available to make a " + recipeComponent2.DisplayName + " for this recipe!";
							return false;
						}
					}
					if ((componentMaterial == null || num != 0) && recipeComponent2.RequirementType == ComponentRequirementType.Required)
					{
						failureMessage = "A " + recipeComponent2.DisplayName + " is required for this recipe!";
						return false;
					}
					if (componentMaterial == null || num != 0 || recipeComponent2.RequirementType != ComponentRequirementType.OptionalExclusive)
					{
						goto IL_2E0;
					}
					if (recipeComponent != null)
					{
						failureMessage = string.Concat(new string[]
						{
							"A ",
							recipeComponent2.DisplayName,
							" and a ",
							recipeComponent.DisplayName,
							" cannot be used at the same time!"
						});
						return false;
					}
					recipeComponent = recipeComponent2;
				}
				IL_2E0:;
			}
			return true;
		}

		// Token: 0x0600551A RID: 21786 RVA: 0x001DCCB4 File Offset: 0x001DAEB4
		private int GetMinimumMasteryLevel()
		{
			if (this.m_productionType == RecipeProductionType.Stepwise)
			{
				int num = 100;
				foreach (QuantityStep quantityStep in this.m_steps)
				{
					num = ((num < quantityStep.Level.x) ? num : quantityStep.Level.x);
				}
				return num;
			}
			return this.m_minimumAbilityLevel;
		}

		// Token: 0x0600551B RID: 21787 RVA: 0x001DCD0C File Offset: 0x001DAF0C
		public bool IsHighEnoughLevel(GameEntity entity)
		{
			ArchetypeInstance archetypeInstance;
			return entity.CollectionController.Abilities.TryGetInstanceForArchetypeId(this.Ability.Id, out archetypeInstance) && archetypeInstance.GetAssociatedLevelInteger(entity) >= this.MinimumAbilityLevel;
		}

		// Token: 0x0600551C RID: 21788 RVA: 0x001DCD4C File Offset: 0x001DAF4C
		public bool IsAtCorrectCraftingStation(GameEntity entity)
		{
			if (entity.CollectionController.RefinementStation || this.m_showWithoutCraftingStation)
			{
				InteractiveRefinementStation refinementStation = entity.CollectionController.RefinementStation;
				return ((refinementStation != null) ? refinementStation.Profile : null) == null || this.StationCategory.HasBitFlag(entity.CollectionController.RefinementStation.Profile.Category);
			}
			return false;
		}

		// Token: 0x0600551D RID: 21789 RVA: 0x00077B72 File Offset: 0x00075D72
		private IEnumerable GetItems()
		{
			return SolOdinUtilities.GetDropdownItems<ItemArchetype>();
		}

		// Token: 0x0600551E RID: 21790 RVA: 0x00078DC4 File Offset: 0x00076FC4
		private IEnumerable GetCraftingStationProfiles()
		{
			return SolOdinUtilities.GetDropdownItems<CraftingStationProfile>();
		}

		// Token: 0x0600551F RID: 21791 RVA: 0x00078DCB File Offset: 0x00076FCB
		public override ArchetypeInstance CreateNewInstance()
		{
			throw new InvalidOperationException("Attempted to instantiate recipe: " + base.name + "! Recipes are not instantiable!");
		}

		// Token: 0x06005520 RID: 21792 RVA: 0x00078DE7 File Offset: 0x00076FE7
		private IEnumerable GetAbilities()
		{
			return SolOdinUtilities.GetDropdownItems<RecipeAbility>();
		}

		// Token: 0x06005521 RID: 21793 RVA: 0x001DCDB8 File Offset: 0x001DAFB8
		private bool ValidateComponentList(RecipeComponent[] value, ref string errorMessage)
		{
			errorMessage = string.Empty;
			if (this.m_disableHistory)
			{
				return true;
			}
			bool flag = value.Length <= 20;
			int componentDepth = this.GetComponentDepth(null);
			if (componentDepth == -1)
			{
				errorMessage = this.m_loop;
				return false;
			}
			bool flag2 = componentDepth <= 10;
			if (!flag && !flag2)
			{
				errorMessage = this.m_tooManyComponents + "\n" + this.m_componentHierarchyTooDeep;
			}
			else if (!flag)
			{
				errorMessage = this.m_tooManyComponents;
			}
			else if (!flag2)
			{
				errorMessage = this.m_componentHierarchyTooDeep;
			}
			return flag && flag2;
		}

		// Token: 0x06005522 RID: 21794 RVA: 0x001DCE3C File Offset: 0x001DB03C
		private int GetComponentDepth(Stack<ItemArchetype> history = null)
		{
			if (this.m_components == null || this.m_components.Length == 0)
			{
				return 0;
			}
			if (history == null)
			{
				history = new Stack<ItemArchetype>();
			}
			int num = 0;
			foreach (RecipeComponent recipeComponent in this.m_components)
			{
				if (recipeComponent != null && recipeComponent.AcceptableMaterials != null && recipeComponent.Enabled)
				{
					int num2 = 0;
					foreach (ComponentMaterial componentMaterial in recipeComponent.AcceptableMaterials)
					{
						if (componentMaterial != null && !(componentMaterial.Archetype == null))
						{
							if (history.Contains(componentMaterial.Archetype))
							{
								return -1;
							}
							history.Push(componentMaterial.Archetype);
							List<Recipe> list = componentMaterial.Archetype.FindRecipesThatProduceThisItem();
							foreach (Recipe recipe in list)
							{
								int componentDepth = recipe.GetComponentDepth(history);
								if (componentDepth == -1)
								{
									StaticListPool<Recipe>.ReturnToPool(list);
									return -1;
								}
								if (componentDepth > 10)
								{
									StaticListPool<Recipe>.ReturnToPool(list);
									return int.MaxValue;
								}
								num2 = ((num2 > componentDepth) ? num2 : componentDepth);
								while (history.Pop() != componentMaterial.Archetype && history.Count > 0)
								{
								}
							}
							StaticListPool<Recipe>.ReturnToPool(list);
							if (history.Count > 0)
							{
								history.Pop();
							}
						}
					}
					num = ((num > num2) ? num : num2);
				}
			}
			return num;
		}

		// Token: 0x06005523 RID: 21795 RVA: 0x001DCFD4 File Offset: 0x001DB1D4
		public ComponentSelection[] BuildComponentSelectionList(ComponentSelection[] existingSelection = null, bool ordinalPreservation = false)
		{
			if (this.m_components == null)
			{
				return null;
			}
			ComponentSelection[] array = new ComponentSelection[this.m_components.Length];
			for (int i = 0; i < this.m_components.Length; i++)
			{
				array[i] = new ComponentSelection
				{
					ComponentId = this.m_components[i].Id,
					ComponentName = this.m_components[i].DisplayName,
					Selected = true
				};
			}
			if (ordinalPreservation)
			{
				if (existingSelection != null)
				{
					List<bool> fromPool = StaticListPool<bool>.GetFromPool();
					foreach (ComponentSelection componentSelection in existingSelection)
					{
						fromPool.Add(componentSelection.Selected);
					}
					int num = 0;
					while (num < fromPool.Count && num < array.Length)
					{
						array[num].Selected = fromPool[num];
						num++;
					}
					StaticListPool<bool>.ReturnToPool(fromPool);
				}
			}
			else if (existingSelection != null)
			{
				foreach (ComponentSelection componentSelection2 in existingSelection)
				{
					foreach (ComponentSelection componentSelection3 in array)
					{
						if (componentSelection2.ComponentId == componentSelection3.ComponentId)
						{
							componentSelection3.Selected = componentSelection2.Selected;
						}
					}
				}
			}
			return array;
		}

		// Token: 0x06005524 RID: 21796 RVA: 0x001DD10C File Offset: 0x001DB30C
		public void GetAllItemsInTree(List<ItemArchetype> items)
		{
			if (this.m_components != null)
			{
				foreach (RecipeComponent recipeComponent in this.m_components)
				{
					if (recipeComponent != null && recipeComponent.AcceptableMaterials != null && recipeComponent.Enabled)
					{
						foreach (ComponentMaterial componentMaterial in recipeComponent.AcceptableMaterials)
						{
							if (componentMaterial != null && componentMaterial.Archetype)
							{
								items.Add(componentMaterial.Archetype);
								componentMaterial.Archetype.GetAllItemsInTree(items);
							}
						}
					}
				}
			}
		}

		// Token: 0x06005525 RID: 21797 RVA: 0x001DD198 File Offset: 0x001DB398
		public void UpdateAllComponentSelectionLists(bool ordinalPreservation = false)
		{
			this.m_materialTierComponentSelection = this.BuildComponentSelectionList(this.m_materialTierComponentSelection, ordinalPreservation);
			if (this.m_outputOverrides != null && this.m_outputOverrides.Length != 0)
			{
				foreach (OutputItemOverride outputItemOverride in this.m_outputOverrides)
				{
					outputItemOverride.MaterialTierComponentSelection = this.BuildComponentSelectionList(outputItemOverride.MaterialTierComponentSelection, ordinalPreservation);
				}
			}
		}

		// Token: 0x06005526 RID: 21798 RVA: 0x001DD1F8 File Offset: 0x001DB3F8
		public override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			base.FillTooltipBlocks(tooltip, instance, entity);
			TooltipTextBlock requirementsBlock = tooltip.RequirementsBlock;
			ArchetypeInstance archetypeInstance;
			Color color = entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(this.Mastery.Id, out archetypeInstance) ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor;
			requirementsBlock.AppendLine("Required Profession:", this.Mastery.DisplayName.Color(color));
			if (this.Ability.Specialization != null)
			{
				color = ((archetypeInstance != null && archetypeInstance.MasteryData != null && archetypeInstance.MasteryData.Specialization != null && archetypeInstance.MasteryData.Specialization.Value == this.Ability.Specialization.Id) ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor);
				requirementsBlock.AppendLine("Required Specialization:", this.Ability.Specialization.DisplayName.Color(color));
			}
			ArchetypeInstance archetypeInstance2;
			color = ((entity.CollectionController.Abilities.TryGetInstanceForArchetypeId(this.Ability.Id, out archetypeInstance2) && archetypeInstance2.GetAssociatedLevelInteger(entity) >= this.MinimumAbilityLevel) ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor);
			requirementsBlock.AppendLine("Required Ability:", (this.Ability.DisplayName + " (" + this.MinimumAbilityLevel.ToString() + ")").Color(color));
			LearnableArchetype learnableArchetype;
			if (entity.CollectionController.Recipes.TryGetLearnableForId(base.Id, out learnableArchetype))
			{
				requirementsBlock.AppendLine("", 0);
				requirementsBlock.AppendLine("Already known".Color(UIManager.RequirementsNotMetColor), 0);
			}
		}

		// Token: 0x04004B88 RID: 19336
		public const int kMaxComponentBreadth = 20;

		// Token: 0x04004B89 RID: 19337
		public const int kMaxComponentDepth = 10;

		// Token: 0x04004B8A RID: 19338
		[SerializeField]
		private RecipeAbility m_ability;

		// Token: 0x04004B8B RID: 19339
		[SerializeField]
		private OutputItemOverride[] m_outputOverrides;

		// Token: 0x04004B8C RID: 19340
		[SerializeField]
		private RecipeOutputType m_outputType;

		// Token: 0x04004B8D RID: 19341
		[SerializeField]
		private ItemArchetype m_itemToCreate;

		// Token: 0x04004B8E RID: 19342
		[SerializeField]
		private ComponentSelection[] m_materialTierComponentSelection;

		// Token: 0x04004B8F RID: 19343
		[SerializeField]
		private MaterialLevelAggregationType m_materialLevelAggregationOverride;

		// Token: 0x04004B90 RID: 19344
		[SerializeField]
		private TieredItemProfile m_tierProfile;

		// Token: 0x04004B91 RID: 19345
		[Tooltip("The item to create if the crafting process fails. Leave blank for no output on failure.")]
		[SerializeField]
		private ItemArchetype m_itemToCreateOnFailure;

		// Token: 0x04004B92 RID: 19346
		[SerializeField]
		private bool m_forceOneComponentPerStack;

		// Token: 0x04004B93 RID: 19347
		[SerializeField]
		private RecipeComponent[] m_components;

		// Token: 0x04004B94 RID: 19348
		[Tooltip("What stations should show this recipe. Crafting stations without profiles will always show all recipes.")]
		[SerializeField]
		private CraftingStationCategory m_stationCategory;

		// Token: 0x04004B95 RID: 19349
		[Tooltip("Whether the Crafting UI should show this recipe if the player is not at a crafting station.")]
		[SerializeField]
		private bool m_showWithoutCraftingStation = true;

		// Token: 0x04004B96 RID: 19350
		[SerializeField]
		private bool m_disableCraftedTag;

		// Token: 0x04004B97 RID: 19351
		[SerializeField]
		private bool m_disableHistory;

		// Token: 0x04004B98 RID: 19352
		[SerializeField]
		private bool m_qualityModifierEnabled = true;

		// Token: 0x04004B99 RID: 19353
		[Tooltip("Upon creation, these will be referenced by the RecipeAbility. If all the listed archetypes on any entry were used in this item's creation, the resulting item's history will be replaced wholesale with the substitute archetype. The first entry to fulfill this requirement is the only entry that matters, so sort by priority.")]
		[SerializeField]
		private RecipeSubstitution[] m_substitutions;

		// Token: 0x04004B9A RID: 19354
		[Range(1f, 100f)]
		[SerializeField]
		private int m_minimumAbilityLevel = 1;

		// Token: 0x04004B9B RID: 19355
		private string m_loop = "Component loop detected!";

		// Token: 0x04004B9C RID: 19356
		private string m_tooManyComponents = string.Format("Component count must not exceed {0}.", 20);

		// Token: 0x04004B9D RID: 19357
		private string m_componentHierarchyTooDeep = string.Format("Component hierarchy must not exceed a max depth of {0}", 10);

		// Token: 0x04004B9E RID: 19358
		[SerializeField]
		private RecipeProductionType m_productionType;

		// Token: 0x04004B9F RID: 19359
		[Min(0f)]
		[SerializeField]
		private int m_amountToProduce = 1;

		// Token: 0x04004BA0 RID: 19360
		[SerializeField]
		private QuantityStep[] m_steps;

		// Token: 0x04004BA1 RID: 19361
		[Min(1f)]
		[SerializeField]
		private int m_minimumQuantity = 1;

		// Token: 0x04004BA2 RID: 19362
		[Min(1f)]
		[SerializeField]
		private int m_maximumQuantity = 1;

		// Token: 0x04004BA3 RID: 19363
		[Range(0f, 1f)]
		[SerializeField]
		private float m_maximumFailureRate;

		// Token: 0x04004BA4 RID: 19364
		[Range(0f, 1f)]
		[SerializeField]
		private float m_experienceMultiplier = 1f;

		// Token: 0x04004BA5 RID: 19365
		private UniqueId[] m_tempIdArray = new UniqueId[1];

		// Token: 0x04004BA6 RID: 19366
		private List<ItemUsage> m_tempMaterialAggregationLists = new List<ItemUsage>();

		// Token: 0x04004BA7 RID: 19367
		private static readonly List<ItemUsage> m_itemsUsed = new List<ItemUsage>();

		// Token: 0x04004BA8 RID: 19368
		private Dictionary<UniqueId, int> m_amountsConsumed = new Dictionary<UniqueId, int>();
	}
}
