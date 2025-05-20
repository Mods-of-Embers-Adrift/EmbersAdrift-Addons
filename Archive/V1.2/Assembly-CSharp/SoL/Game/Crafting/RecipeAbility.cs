using System;
using System.Collections.Generic;
using SoL.Game.Animation;
using SoL.Game.Messages;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Quests;
using SoL.Game.Quests.Objectives;
using SoL.Game.Transactions;
using SoL.Managers;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Crafting
{
	// Token: 0x02000CEB RID: 3307
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Abilities/Crafting/Recipe")]
	public class RecipeAbility : AbilityArchetype
	{
		// Token: 0x17001803 RID: 6147
		// (get) Token: 0x06006421 RID: 25633 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool MustBeMemorizedToExecute
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001804 RID: 6148
		// (get) Token: 0x06006422 RID: 25634 RVA: 0x00045BCA File Offset: 0x00043DCA
		public override bool ConsiderHaste
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001805 RID: 6149
		// (get) Token: 0x06006423 RID: 25635 RVA: 0x00045BCA File Offset: 0x00043DCA
		public override bool AllowAlchemy
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001806 RID: 6150
		// (get) Token: 0x06006424 RID: 25636 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool m_addGroupBonus
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001807 RID: 6151
		// (get) Token: 0x06006425 RID: 25637 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool m_allowStaminaRegenDuringExecution
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06006426 RID: 25638 RVA: 0x002086F8 File Offset: 0x002068F8
		private ArchetypeInstance GetInstanceInOutputContainer(ContainerInstance containerInstance, ItemArchetype archetype, List<ItemUsage> itemsUsed)
		{
			if (archetype == null || !(archetype is IStackable))
			{
				return null;
			}
			ArchetypeInstance result = null;
			foreach (ArchetypeInstance archetypeInstance in containerInstance.Instances)
			{
				if (archetypeInstance.ArchetypeId == archetype.Id && (!archetypeInstance.ItemData.HasComponents || this.InstanceMaterialsMatch(archetypeInstance, itemsUsed)))
				{
					result = archetypeInstance;
					break;
				}
			}
			return result;
		}

		// Token: 0x06006427 RID: 25639 RVA: 0x00208784 File Offset: 0x00206984
		private bool InstanceMaterialsMatch(ArchetypeInstance instance, List<ItemUsage> itemsUsed)
		{
			foreach (ItemComponentData itemComponentData in instance.ItemData.ItemComponentTree.Components)
			{
				bool flag = false;
				foreach (ItemUsage itemUsage in itemsUsed)
				{
					flag = (flag || (itemComponentData.ArchetypeId == itemUsage.Instance.ArchetypeId && itemComponentData.RecipeComponentId == itemUsage.UsedFor.Id && (!itemUsage.Instance.ItemData.HasComponents || itemUsage.Instance.ItemData.ItemComponentTree.Equals(itemComponentData))));
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06006428 RID: 25640 RVA: 0x00208868 File Offset: 0x00206A68
		protected override bool ExecutionCheck(ExecutionCache executionCache, float executionProgress)
		{
			if (!base.ExecutionCheck(executionCache, executionProgress))
			{
				return false;
			}
			bool flag = executionProgress <= 0f;
			executionCache.MsgType = MessageType.Notification;
			ContainerInstance containerInstance;
			ContainerInstance containerInstance2;
			if (!executionCache.SourceEntity.CollectionController.TryGetInstance(ContainerType.Inventory, out containerInstance) || !executionCache.SourceEntity.CollectionController.TryGetInstance(ContainerType.Gathering, out containerInstance2))
			{
				executionCache.Message = "Unable to find combination container!";
				return false;
			}
			if (containerInstance.GetMaxCapacity() - containerInstance.Count < 1)
			{
				executionCache.Message = "No free slots in inventory!";
				return false;
			}
			if (executionCache.SourceEntity.CollectionController.RefinementStation == null)
			{
				executionCache.Message = "Invalid combination station!";
				return false;
			}
			executionCache.SetTargetGameEntity(executionCache.SourceEntity.CollectionController.RefinementStation.GameEntity);
			Recipe recipe = GameManager.IsServer ? (executionCache.Refinement.Value.Recipe as Recipe) : ClientGameManager.UIManager.CraftingUI.ExecutingRecipe;
			if (recipe == null)
			{
				executionCache.Message = "Invalid recipe!";
				return false;
			}
			CraftingStationProfile profile = executionCache.SourceEntity.CollectionController.RefinementStation.Profile;
			CraftingStationCategory craftingStationCategory = (profile != null) ? profile.Category : CraftingStationCategory.None;
			if (craftingStationCategory != CraftingStationCategory.None && !recipe.StationCategory.HasBitFlag(craftingStationCategory))
			{
				executionCache.Message = "Invalid station category!";
				return false;
			}
			ArchetypeInstance masteryInstance;
			if (!executionCache.SourceEntity.CollectionController.Masteries.TryGetInstanceForArchetypeId(recipe.Mastery.Id, out masteryInstance))
			{
				executionCache.Message = recipe.Mastery.DisplayName + " is required for use of the " + recipe.DisplayName + " recipe!";
				return false;
			}
			float associatedLevel = executionCache.Instance.GetAssociatedLevel(executionCache.SourceEntity);
			if (associatedLevel < (float)recipe.MinimumAbilityLevel)
			{
				executionCache.Message = string.Format("{0} must be at least level {1} for use of the {2} recipe!", executionCache.Instance.Ability.DisplayName, recipe.MinimumAbilityLevel, recipe.DisplayName);
				return false;
			}
			if (flag)
			{
				if (recipe.Components == null)
				{
					executionCache.Message = "Invalid recipe; no components specified!";
					return false;
				}
				List<ItemUsage> list = GameManager.IsServer ? executionCache.Refinement.Value.ItemsUsed : ClientGameManager.UIManager.CraftingUI.ExecutingComponentSelection;
				if (!list.AllItemsStillExist())
				{
					executionCache.Message = "Items used no longer exist!";
					return false;
				}
				List<ArchetypeInstance> fromPool = StaticListPool<ArchetypeInstance>.GetFromPool();
				fromPool.Capacity = containerInstance.Count + containerInstance2.Count;
				fromPool.AddRange(containerInstance2.Instances);
				fromPool.AddRange(containerInstance.Instances);
				string message;
				if (!recipe.IsMatch(fromPool, list, out message))
				{
					executionCache.Message = message;
					return false;
				}
				StaticListPool<ArchetypeInstance>.ReturnToPool(fromPool);
				foreach (ItemUsage itemUsage in list)
				{
					float num = associatedLevel;
					int? minimumMaterialLevel = ((ItemArchetype)itemUsage.Instance.Archetype).MinimumMaterialLevel;
					float? num2 = (minimumMaterialLevel != null) ? new float?((float)minimumMaterialLevel.GetValueOrDefault()) : null;
					if (num < num2.GetValueOrDefault() & num2 != null)
					{
						executionCache.Message = "The materials are too advanced for you!";
						return false;
					}
				}
				foreach (ItemUsage itemUsage2 in list)
				{
					if (itemUsage2.Instance.Archetype.ArchetypeHasCount() && itemUsage2.Instance.ItemData != null && itemUsage2.Instance.ItemData.Count != null)
					{
						executionCache.AddReductionTask(ReductionTaskType.Count, itemUsage2.Instance, itemUsage2.AmountUsed);
					}
					else
					{
						executionCache.AddReductionTask(ReductionTaskType.Consume, itemUsage2.Instance, itemUsage2.AmountUsed);
					}
				}
				int num3 = GameManager.IsServer ? executionCache.Refinement.Value.TargetAbilityLevel : ClientGameManager.UIManager.CraftingUI.ExecutingTargetAbilityLevel;
				float num4 = (num3 > 0) ? ((float)num3) : executionCache.Instance.GetAssociatedLevel(executionCache.SourceEntity);
				int num5;
				int num6;
				int num7;
				int num8;
				int num9;
				int num10;
				list.GetAggregateMaterialLevel(false, out num5, out num6, out num7, out num8, out num9, out num10);
				num4 = Math.Min(Math.Max(num4, (float)num7), (float)((int)associatedLevel));
				ArchetypeInstance instanceInOutputContainer = this.GetInstanceInOutputContainer(containerInstance, recipe.GetItemToCreate(list, num4), list);
				if (instanceInOutputContainer == null && containerInstance.Count >= containerInstance.GetMaxCapacity())
				{
					executionCache.Message = "Output full!";
					return false;
				}
				executionCache.MasteryInstance = masteryInstance;
				executionCache.Refinement = new ExecutionCache.RefinementCache?(new ExecutionCache.RefinementCache
				{
					CurrentOutputInstance = instanceInOutputContainer,
					Recipe = recipe,
					ItemsUsed = list,
					TargetAbilityLevel = num3
				});
				return true;
			}
			return true;
		}

		// Token: 0x06006429 RID: 25641 RVA: 0x00208D28 File Offset: 0x00206F28
		protected override void PostExecution(ExecutionCache executionCache)
		{
			float associatedLevel = executionCache.Instance.GetAssociatedLevel(executionCache.SourceEntity);
			List<ItemUsage> itemsUsed = executionCache.Refinement.Value.ItemsUsed;
			int targetLevel = 0;
			if (!itemsUsed.AllItemsStillExist())
			{
				executionCache.Message = "Items used no longer exist!";
				return;
			}
			Recipe recipe = null;
			ContainerInstance containerInstance;
			if (GameManager.IsServer && executionCache.Refinement != null && executionCache.Refinement.Value.Recipe != null && executionCache.Refinement.Value.Recipe.TryGetAsType(out recipe) && executionCache.SourceEntity.CollectionController.TryGetInstance(ContainerType.Inventory, out containerInstance))
			{
				GlobalCounters.ItemsCrafted += 1U;
				bool flag = recipe.PerformFailureCheck(associatedLevel, itemsUsed);
				int amountToProduce = recipe.GetAmountToProduce(associatedLevel, itemsUsed);
				if (flag)
				{
					if (!this.AddInstanceOrUpdateCount(executionCache, amountToProduce, recipe, null, null, true))
					{
						return;
					}
					this.AdvanceCraftOrders(executionCache.SourceEntity, recipe, CraftResult.Success);
				}
				else if (recipe.ItemToCreateOnFailure != null)
				{
					ArchetypeInstance instanceInOutputContainer = this.GetInstanceInOutputContainer(containerInstance, recipe.ItemToCreateOnFailure, itemsUsed);
					if (!this.AddInstanceOrUpdateCount(executionCache, 1, recipe, recipe.ItemToCreateOnFailure, instanceInOutputContainer, false))
					{
						return;
					}
					this.AdvanceCraftOrders(executionCache.SourceEntity, recipe, CraftResult.FailureItem);
				}
				else
				{
					executionCache.Message = "Failed to create " + recipe.GetItemToCreate(itemsUsed, associatedLevel).DisplayName + "!";
					this.AdvanceCraftOrders(executionCache.SourceEntity, recipe, CraftResult.FailureNoItem);
				}
			}
			if (GameManager.IsServer)
			{
				int num;
				int num2;
				int num3;
				int num4;
				int num5;
				itemsUsed.GetAggregateMaterialLevel(true, out num, out num2, out num3, out num4, out num5, out targetLevel);
			}
			executionCache.PerformReduction();
			executionCache.PerformPostReduction();
			MasteryArchetype masteryArchetype;
			if (!GameManager.IsServer || executionCache.MasteryInstance == null || !(executionCache.MasteryInstance.Archetype != null) || !executionCache.MasteryInstance.Archetype.TryGetAsType(out masteryArchetype))
			{
				return;
			}
			float experienceMultiplier = (recipe != null) ? recipe.ExperienceMultiplier : 1f;
			MasteryType type = masteryArchetype.Type;
			if (type == MasteryType.Trade)
			{
				bool progressSpecialization = recipe != null && recipe.Ability != null && recipe.Ability.Specialization != null;
				ProgressionCalculator.OnCraftingSuccess(executionCache.SourceEntity, executionCache.MasteryInstance, targetLevel, experienceMultiplier, progressSpecialization);
				return;
			}
			if (type == MasteryType.Harvesting)
			{
				ProgressionCalculator.OnGatheringSuccess(executionCache.SourceEntity, executionCache.MasteryInstance, targetLevel, experienceMultiplier);
				return;
			}
		}

		// Token: 0x0600642A RID: 25642 RVA: 0x00208F7C File Offset: 0x0020717C
		private void AdvanceCraftOrders(GameEntity entity, Recipe recipe, CraftResult result)
		{
			if (entity.CharacterData.ObjectiveOrders.HasOrders<CraftObjective>())
			{
				List<ValueTuple<UniqueId, CraftObjective>> pooledOrderList = entity.CharacterData.ObjectiveOrders.GetPooledOrderList<CraftObjective>();
				foreach (ValueTuple<UniqueId, CraftObjective> valueTuple in pooledOrderList)
				{
					Quest quest;
					int hash;
					if (valueTuple.Item2.IsValid(entity, recipe, result) && InternalGameDatabase.Quests.TryGetItem(valueTuple.Item1, out quest) && quest.TryGetObjectiveHashForActiveObjective(valueTuple.Item2.Id, entity, out hash))
					{
						GameManager.QuestManager.Progress(new ObjectiveIterationCache
						{
							QuestId = valueTuple.Item1,
							ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(hash),
							NpcEntity = entity.CollectionController.RefinementStation.GameEntity.NetworkEntity
						}, entity, false);
					}
					BBTask bbtask;
					if (valueTuple.Item2.IsValid(entity, recipe, result) && InternalGameDatabase.BBTasks.TryGetItem(valueTuple.Item1, out bbtask))
					{
						GameManager.QuestManager.ProgressTask(new ObjectiveIterationCache
						{
							QuestId = valueTuple.Item1,
							ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(valueTuple.Item2.CombinedId(valueTuple.Item1)),
							NpcEntity = entity.CollectionController.RefinementStation.GameEntity.NetworkEntity
						}, entity, false);
					}
				}
				entity.CharacterData.ObjectiveOrders.ReturnPooledOrderList<CraftObjective>(pooledOrderList);
			}
		}

		// Token: 0x0600642B RID: 25643 RVA: 0x00209114 File Offset: 0x00207314
		private bool AddInstanceOrUpdateCount(ExecutionCache executionCache, int amountProduced, Recipe recipe, BaseArchetype archetypeOverride = null, ArchetypeInstance instanceOverride = null, bool crafted = true)
		{
			float num = (executionCache.Refinement.Value.TargetAbilityLevel > 0) ? ((float)executionCache.Refinement.Value.TargetAbilityLevel) : executionCache.Instance.GetAssociatedLevel(executionCache.SourceEntity);
			BaseArchetype baseArchetype = archetypeOverride ?? recipe.GetItemToCreate(executionCache.Refinement.Value.ItemsUsed, num);
			ArchetypeInstance archetypeInstance = (archetypeOverride != null) ? instanceOverride : executionCache.Refinement.Value.CurrentOutputInstance;
			ContainerInstance containerInstance;
			if (executionCache.SourceEntity.CollectionController.TryGetInstance(ContainerType.Inventory, out containerInstance))
			{
				if (archetypeInstance != null && archetypeInstance.InstanceId.IsEmpty)
				{
					archetypeInstance = this.GetInstanceInOutputContainer(containerInstance, (ItemArchetype)baseArchetype, executionCache.Refinement.Value.ItemsUsed);
					if (archetypeInstance == null && containerInstance.Count >= containerInstance.GetMaxCapacity())
					{
						executionCache.Message = "Output full!";
						return false;
					}
				}
				if (archetypeInstance != null)
				{
					archetypeInstance.ItemData.Count = new int?((archetypeInstance.ItemData.Count != null) ? (archetypeInstance.ItemData.Count.Value + amountProduced) : (1 + amountProduced));
					ItemCountUpdatedTransaction transaction = new ItemCountUpdatedTransaction
					{
						InstanceId = archetypeInstance.InstanceId,
						Container = containerInstance.Id,
						NewCount = archetypeInstance.ItemData.Count.Value
					};
					executionCache.SourceEntity.NetworkEntity.PlayerRpcHandler.UpdateItemCount(transaction);
				}
				else
				{
					ArchetypeInstance archetypeInstance2 = baseArchetype.CreateNewInstance();
					if (baseArchetype is IStackable)
					{
						archetypeInstance2.ItemData.Count = new int?(amountProduced);
					}
					if (crafted)
					{
						archetypeInstance2.ItemData.DeriveComponentData(archetypeInstance2, recipe, executionCache.Refinement.Value.ItemsUsed, num);
						if (!recipe.DisableCraftedTag)
						{
							archetypeInstance2.AddItemFlag(ItemFlags.Crafted, executionCache.SourceEntity.CollectionController.Record);
						}
						((ItemArchetype)baseArchetype).ApplyInstanceComponentEffects(archetypeInstance2);
					}
					executionCache.AddPostReductionTask(PostReductionTaskType.AddItem, archetypeInstance2, containerInstance, ItemAddContext.Crafting);
				}
				return true;
			}
			executionCache.Message = "Output container no longer accessible!";
			return false;
		}

		// Token: 0x04005704 RID: 22276
		[SerializeField]
		protected AbilityAnimation m_defaultAnimation;
	}
}
