using System;
using System.Collections.Generic;
using System.Text;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Quests.Objectives
{
	// Token: 0x020007A6 RID: 1958
	[CreateAssetMenu(menuName = "SoL/Quests/Objectives/LootObjective")]
	public class LootObjective : QuestObjective, IDelayedRefresh
	{
		// Token: 0x140000B6 RID: 182
		// (add) Token: 0x060039AD RID: 14765 RVA: 0x00173E24 File Offset: 0x00172024
		// (remove) Token: 0x060039AE RID: 14766 RVA: 0x00173E58 File Offset: 0x00172058
		public static event Action<LootObjective> LootAmountChanged;

		// Token: 0x17000D3F RID: 3391
		// (get) Token: 0x060039AF RID: 14767 RVA: 0x00067154 File Offset: 0x00065354
		private bool m_shouldShowItems
		{
			get
			{
				return this.m_criteria == LootCriteria.Archetype || this.m_criteria == LootCriteria.WasMadeFromAll || this.m_criteria == LootCriteria.WasMadeFromAny;
			}
		}

		// Token: 0x17000D40 RID: 3392
		// (get) Token: 0x060039B0 RID: 14768 RVA: 0x00067172 File Offset: 0x00065372
		public LootCriteria Criteria
		{
			get
			{
				return this.m_criteria;
			}
		}

		// Token: 0x17000D41 RID: 3393
		// (get) Token: 0x060039B1 RID: 14769 RVA: 0x0006717A File Offset: 0x0006537A
		public ItemArchetype[] Items
		{
			get
			{
				return this.m_items;
			}
		}

		// Token: 0x17000D42 RID: 3394
		// (get) Token: 0x060039B2 RID: 14770 RVA: 0x00067182 File Offset: 0x00065382
		public MaterialCategory Category
		{
			get
			{
				return this.m_category;
			}
		}

		// Token: 0x17000D43 RID: 3395
		// (get) Token: 0x060039B3 RID: 14771 RVA: 0x0006718A File Offset: 0x0006538A
		public int AmountRequired
		{
			get
			{
				return this.m_amountRequired;
			}
		}

		// Token: 0x17000D44 RID: 3396
		// (get) Token: 0x060039B4 RID: 14772 RVA: 0x00067192 File Offset: 0x00065392
		public List<DropLocation> DropLocations
		{
			get
			{
				return this.m_dropLocations;
			}
		}

		// Token: 0x060039B5 RID: 14773 RVA: 0x0006719A File Offset: 0x0006539A
		public override bool Validate(GameEntity sourceEntity, ObjectiveIterationCache cache, out string message)
		{
			if (cache.IterationsRequested > 1)
			{
				message = "Too many iterations requested.";
				return false;
			}
			if (this.GetAvailableItems(sourceEntity, null) < this.m_amountRequired)
			{
				message = "Item requirements not met!";
				return false;
			}
			message = string.Empty;
			return true;
		}

		// Token: 0x060039B6 RID: 14774 RVA: 0x000671CF File Offset: 0x000653CF
		public bool HasLoot(GameEntity entity)
		{
			return this.GetAvailableItems(entity, null) >= this.m_amountRequired;
		}

		// Token: 0x060039B7 RID: 14775 RVA: 0x000671E4 File Offset: 0x000653E4
		public override void OnComplete(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			base.OnComplete(cache, sourceEntity);
			if (!GameManager.IsServer)
			{
				this.UnsubscribeFromInventories(sourceEntity);
			}
			if (GameManager.IsServer && this.m_removeFromInventoryOnCompletion && this.m_dropLocations.Count > 0)
			{
				this.RemoveItemsFromInventory(sourceEntity);
			}
		}

		// Token: 0x060039B8 RID: 14776 RVA: 0x00067220 File Offset: 0x00065420
		public override void OnEnterStep(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			base.OnEnterStep(cache, sourceEntity);
			if (GameManager.IsServer && this.m_removeFromInventoryOnCompletion && this.m_dropLocations.Count == 0)
			{
				this.RemoveItemsFromInventory(sourceEntity);
			}
		}

		// Token: 0x060039B9 RID: 14777 RVA: 0x00173E8C File Offset: 0x0017208C
		public override void OnEntityInitializedWhenActive(GameEntity sourceEntity, UniqueId questOrTaskId)
		{
			base.OnEntityInitializedWhenActive(sourceEntity, questOrTaskId);
			if (GameManager.IsServer)
			{
				return;
			}
			this.m_previousAmount = Math.Min(this.GetAvailableItems(sourceEntity, null), this.m_amountRequired);
			Quest quest;
			if (InternalGameDatabase.Quests.TryGetItem(questOrTaskId, out quest))
			{
				this.m_muted = quest.IsMuted(sourceEntity);
			}
			this.SubscribeToInventories(sourceEntity);
		}

		// Token: 0x060039BA RID: 14778 RVA: 0x0006724D File Offset: 0x0006544D
		public override void OnEntityDestroyedWhenActive(GameEntity sourceEntity, UniqueId questOrTaskId)
		{
			base.OnEntityDestroyedWhenActive(sourceEntity, questOrTaskId);
			if (GameManager.IsServer)
			{
				return;
			}
			this.UnsubscribeFromInventories(sourceEntity);
		}

		// Token: 0x060039BB RID: 14779 RVA: 0x00173EE8 File Offset: 0x001720E8
		public override void OnActivate(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			base.OnActivate(cache, sourceEntity);
			if (GameManager.IsServer)
			{
				return;
			}
			Quest quest;
			if (InternalGameDatabase.Quests.TryGetItem(cache.QuestId, out quest))
			{
				this.m_muted = quest.IsMuted(sourceEntity);
			}
			int availableItems = this.GetAvailableItems(sourceEntity, null);
			if (availableItems > 0)
			{
				this.m_previousAmount = 0;
				this.AnnounceCountChange();
			}
			else
			{
				this.m_previousAmount = Math.Min(availableItems, this.m_amountRequired);
			}
			this.SubscribeToInventories(sourceEntity);
		}

		// Token: 0x060039BC RID: 14780 RVA: 0x00067266 File Offset: 0x00065466
		public override void OnDeactivate(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			base.OnDeactivate(cache, sourceEntity);
			if (GameManager.IsServer)
			{
				return;
			}
			this.UnsubscribeFromInventories(sourceEntity);
		}

		// Token: 0x060039BD RID: 14781 RVA: 0x0006727F File Offset: 0x0006547F
		public override void OnMuteChanged(GameEntity sourceEntity, bool mute)
		{
			base.OnMuteChanged(sourceEntity, mute);
			if (GameManager.IsServer)
			{
				return;
			}
			this.m_muted = mute;
		}

		// Token: 0x060039BE RID: 14782 RVA: 0x00173F5C File Offset: 0x0017215C
		private void SubscribeToInventories(GameEntity sourceEntity)
		{
			sourceEntity.CollectionController.Inventory.InstanceAdded += this.OnInventoryAdd;
			sourceEntity.CollectionController.Inventory.InstanceRemoved += this.OnInventoryRemove;
			sourceEntity.CollectionController.Inventory.QuantityOfItemChanged += this.OnInventoryCountChange;
			sourceEntity.CollectionController.Gathering.InstanceAdded += this.OnInventoryAdd;
			sourceEntity.CollectionController.Gathering.InstanceRemoved += this.OnInventoryRemove;
			sourceEntity.CollectionController.Gathering.QuantityOfItemChanged += this.OnInventoryCountChange;
			sourceEntity.CollectionController.Pouch.InstanceAdded += this.OnInventoryAdd;
			sourceEntity.CollectionController.Pouch.InstanceRemoved += this.OnInventoryRemove;
			sourceEntity.CollectionController.Pouch.QuantityOfItemChanged += this.OnInventoryCountChange;
			sourceEntity.CollectionController.ReagentPouch.InstanceAdded += this.OnInventoryAdd;
			sourceEntity.CollectionController.ReagentPouch.InstanceRemoved += this.OnInventoryRemove;
			sourceEntity.CollectionController.ReagentPouch.QuantityOfItemChanged += this.OnInventoryCountChange;
		}

		// Token: 0x060039BF RID: 14783 RVA: 0x001740BC File Offset: 0x001722BC
		private void UnsubscribeFromInventories(GameEntity sourceEntity)
		{
			sourceEntity.CollectionController.Inventory.InstanceAdded -= this.OnInventoryAdd;
			sourceEntity.CollectionController.Inventory.InstanceRemoved -= this.OnInventoryRemove;
			sourceEntity.CollectionController.Inventory.QuantityOfItemChanged -= this.OnInventoryCountChange;
			sourceEntity.CollectionController.Gathering.InstanceAdded -= this.OnInventoryAdd;
			sourceEntity.CollectionController.Gathering.InstanceRemoved -= this.OnInventoryRemove;
			sourceEntity.CollectionController.Gathering.QuantityOfItemChanged -= this.OnInventoryCountChange;
			sourceEntity.CollectionController.Pouch.InstanceAdded -= this.OnInventoryAdd;
			sourceEntity.CollectionController.Pouch.InstanceRemoved -= this.OnInventoryRemove;
			sourceEntity.CollectionController.Pouch.QuantityOfItemChanged -= this.OnInventoryCountChange;
			sourceEntity.CollectionController.ReagentPouch.InstanceAdded -= this.OnInventoryAdd;
			sourceEntity.CollectionController.ReagentPouch.InstanceRemoved -= this.OnInventoryRemove;
			sourceEntity.CollectionController.ReagentPouch.QuantityOfItemChanged -= this.OnInventoryCountChange;
		}

		// Token: 0x060039C0 RID: 14784 RVA: 0x00067298 File Offset: 0x00065498
		private void OnInventoryAdd(ArchetypeInstance instance)
		{
			if (this.Matches(instance))
			{
				GameManager.QuestManager.RequestDelayedRefresh(this);
			}
		}

		// Token: 0x060039C1 RID: 14785 RVA: 0x00067298 File Offset: 0x00065498
		private void OnInventoryRemove(ArchetypeInstance instance)
		{
			if (this.Matches(instance))
			{
				GameManager.QuestManager.RequestDelayedRefresh(this);
			}
		}

		// Token: 0x060039C2 RID: 14786 RVA: 0x000672AE File Offset: 0x000654AE
		private void OnInventoryCountChange()
		{
			GameManager.QuestManager.RequestDelayedRefresh(this);
		}

		// Token: 0x060039C3 RID: 14787 RVA: 0x0017421C File Offset: 0x0017241C
		private void AnnounceCountChange()
		{
			if (GameManager.IsServer)
			{
				return;
			}
			int availableItems = this.GetAvailableItems(LocalPlayer.GameEntity, null);
			if (Math.Min(availableItems, this.m_amountRequired) != this.m_previousAmount)
			{
				this.m_previousAmount = Math.Min(availableItems, this.m_amountRequired);
				if (!this.m_hideCountChanges && !this.m_muted)
				{
					ClientGameManager.UIManager.InitCenterScreenAnnouncement(new CenterScreenAnnouncementOptions
					{
						Title = string.Format("{0} {1}/{2}", this.BuildObjectiveDescription(), Math.Min(availableItems, this.m_amountRequired), this.m_amountRequired),
						Text = null,
						TimeShown = 5f,
						ShowDelay = 0f,
						SourceId = new UniqueId?(base.Id)
					});
				}
				Action<LootObjective> lootAmountChanged = LootObjective.LootAmountChanged;
				if (lootAmountChanged == null)
				{
					return;
				}
				lootAmountChanged(this);
			}
		}

		// Token: 0x060039C4 RID: 14788 RVA: 0x00174304 File Offset: 0x00172504
		public string BuildObjectiveDescription()
		{
			if (!string.IsNullOrEmpty(this.m_objectiveDescription))
			{
				return this.m_objectiveDescription;
			}
			switch (this.m_criteria)
			{
			case LootCriteria.Archetype:
				if (this.m_items.Length == 1)
				{
					this.m_objectiveDescription = this.m_items[0].DisplayName;
				}
				else if (this.m_items.Length > 1)
				{
					StringBuilder fromPool = StringBuilderExtensions.GetFromPool();
					fromPool.Append(this.m_items[0].DisplayName);
					for (int i = 1; i < this.m_items.Length; i++)
					{
						fromPool.Append(" OR ");
						fromPool.Append(this.m_items[i].DisplayName);
					}
					this.m_objectiveDescription = fromPool.ToString_ReturnToPool();
				}
				break;
			case LootCriteria.MaterialCategory:
				this.m_objectiveDescription = this.m_category.DisplayName;
				break;
			case LootCriteria.WasMadeFromAll:
				if (this.m_items.Length == 1)
				{
					this.m_objectiveDescription = "Items made from " + this.m_items[0].DisplayName;
				}
				else if (this.m_items.Length > 1)
				{
					StringBuilder fromPool2 = StringBuilderExtensions.GetFromPool();
					fromPool2.Append("Items made from ");
					fromPool2.Append(this.m_items[0].DisplayName);
					for (int j = 1; j < this.m_items.Length; j++)
					{
						fromPool2.Append(" AND ");
						fromPool2.Append(this.m_items[j].DisplayName);
					}
					this.m_objectiveDescription = fromPool2.ToString_ReturnToPool();
				}
				break;
			case LootCriteria.WasMadeFromAny:
				if (this.m_items.Length == 1)
				{
					this.m_objectiveDescription = "Items made from " + this.m_items[0].DisplayName;
				}
				else if (this.m_items.Length > 1)
				{
					StringBuilder fromPool3 = StringBuilderExtensions.GetFromPool();
					fromPool3.Append("Items made from ");
					fromPool3.Append(this.m_items[0].DisplayName);
					for (int k = 1; k < this.m_items.Length; k++)
					{
						fromPool3.Append(" OR ");
						fromPool3.Append(this.m_items[k].DisplayName);
					}
					this.m_objectiveDescription = fromPool3.ToString_ReturnToPool();
				}
				break;
			}
			return this.m_objectiveDescription;
		}

		// Token: 0x060039C5 RID: 14789 RVA: 0x00174544 File Offset: 0x00172744
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

		// Token: 0x060039C6 RID: 14790 RVA: 0x001747D8 File Offset: 0x001729D8
		public bool TryAdvance(UniqueId parentId, GameEntity entity)
		{
			if (this.HasLoot(entity))
			{
				Quest quest;
				int hash;
				BBTask bbtask;
				if (InternalGameDatabase.Quests.TryGetItem(parentId, out quest) && quest.TryGetObjectiveHashForActiveObjective(base.Id, entity, out hash))
				{
					GameManager.QuestManager.Progress(new ObjectiveIterationCache
					{
						QuestId = parentId,
						ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(hash)
					}, entity, false);
				}
				else if (InternalGameDatabase.BBTasks.TryGetItem(parentId, out bbtask))
				{
					GameManager.QuestManager.ProgressTask(new ObjectiveIterationCache
					{
						QuestId = parentId,
						ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(base.CombinedId(parentId))
					}, entity, false);
				}
				return true;
			}
			return false;
		}

		// Token: 0x060039C7 RID: 14791 RVA: 0x00174880 File Offset: 0x00172A80
		private bool Matches(ArchetypeInstance instance)
		{
			switch (this.m_criteria)
			{
			case LootCriteria.Archetype:
				foreach (ItemArchetype itemArchetype in this.m_items)
				{
					if (instance.ArchetypeId == itemArchetype.Id)
					{
						return true;
					}
				}
				return false;
			case LootCriteria.MaterialCategory:
				return ((ItemArchetype)instance.Archetype).MaterialCategory == this.m_category;
			case LootCriteria.WasMadeFromAll:
				return ((ItemArchetype)instance.Archetype).WasMadeFromAll(instance, this.m_items, null);
			case LootCriteria.WasMadeFromAny:
				return ((ItemArchetype)instance.Archetype).WasMadeFromAny(instance, this.m_items, null);
			default:
				return false;
			}
		}

		// Token: 0x060039C8 RID: 14792 RVA: 0x0004475B File Offset: 0x0004295B
		private void RemoveItemsFromInventory(GameEntity entity)
		{
		}

		// Token: 0x17000D45 RID: 3397
		// (get) Token: 0x060039C9 RID: 14793 RVA: 0x000672BB File Offset: 0x000654BB
		UniqueId IDelayedRefresh.Id
		{
			get
			{
				return base.Id;
			}
		}

		// Token: 0x060039CA RID: 14794 RVA: 0x000672C3 File Offset: 0x000654C3
		void IDelayedRefresh.ExecuteRefresh()
		{
			this.AnnounceCountChange();
		}

		// Token: 0x04003854 RID: 14420
		[SerializeField]
		private LootCriteria m_criteria;

		// Token: 0x04003855 RID: 14421
		[SerializeField]
		private ItemArchetype[] m_items;

		// Token: 0x04003856 RID: 14422
		[SerializeField]
		private MaterialCategory m_category;

		// Token: 0x04003857 RID: 14423
		[Min(1f)]
		[SerializeField]
		private int m_amountRequired = 1;

		// Token: 0x04003858 RID: 14424
		[SerializeField]
		private bool m_removeFromInventoryOnCompletion = true;

		// Token: 0x04003859 RID: 14425
		[SerializeField]
		private bool m_hideCountChanges;

		// Token: 0x0400385A RID: 14426
		[SerializeField]
		private List<DropLocation> m_dropLocations;

		// Token: 0x0400385B RID: 14427
		private bool m_muted;

		// Token: 0x0400385C RID: 14428
		private int m_previousAmount;

		// Token: 0x0400385D RID: 14429
		private string m_objectiveDescription;
	}
}
