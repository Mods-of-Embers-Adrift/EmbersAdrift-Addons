using System;
using System.Collections.Generic;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Quests.Objectives
{
	// Token: 0x020007A2 RID: 1954
	[CreateAssetMenu(menuName = "SoL/Quests/Objectives/GatherObjective")]
	public class GatherObjective : OrderDrivenObjective<GatherObjective>
	{
		// Token: 0x17000D3D RID: 3389
		// (get) Token: 0x060039A1 RID: 14753 RVA: 0x000670A9 File Offset: 0x000652A9
		private bool m_shouldShowItems
		{
			get
			{
				return this.m_criteria == LootCriteria.Archetype || this.m_criteria == LootCriteria.WasMadeFromAll || this.m_criteria == LootCriteria.WasMadeFromAny;
			}
		}

		// Token: 0x060039A2 RID: 14754 RVA: 0x001739E8 File Offset: 0x00171BE8
		public override bool Validate(GameEntity sourceEntity, ObjectiveIterationCache cache, out string message)
		{
			byte b = Math.Max(cache.IterationsRequested, 1);
			if (this.GetAvailableItems(sourceEntity, null) < (int)b)
			{
				message = "Item requirements not met!";
				return false;
			}
			message = string.Empty;
			return true;
		}

		// Token: 0x060039A3 RID: 14755 RVA: 0x00173A20 File Offset: 0x00171C20
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

		// Token: 0x060039A4 RID: 14756 RVA: 0x00173CB4 File Offset: 0x00171EB4
		public bool Matches(ArchetypeInstance instance)
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

		// Token: 0x060039A5 RID: 14757 RVA: 0x00173D60 File Offset: 0x00171F60
		public bool TryAdvance(UniqueId parentId, GameEntity entity, byte iterationsRequested)
		{
			Quest quest;
			int hash;
			if (this.GetAvailableItems(entity, null) >= 1 && InternalGameDatabase.Quests.TryGetItem(parentId, out quest) && quest.TryGetObjectiveHashForActiveObjective(base.Id, entity, out hash))
			{
				GameManager.QuestManager.Progress(new ObjectiveIterationCache
				{
					QuestId = parentId,
					ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(hash),
					IterationsRequested = iterationsRequested
				}, entity, false);
				return true;
			}
			BBTask bbtask;
			if (this.GetAvailableItems(entity, null) >= 1 && InternalGameDatabase.BBTasks.TryGetItem(parentId, out bbtask))
			{
				GameManager.QuestManager.ProgressTask(new ObjectiveIterationCache
				{
					QuestId = parentId,
					ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(base.CombinedId(parentId)),
					IterationsRequested = iterationsRequested
				}, entity, false);
				return true;
			}
			return false;
		}

		// Token: 0x04003848 RID: 14408
		[SerializeField]
		private LootCriteria m_criteria;

		// Token: 0x04003849 RID: 14409
		[SerializeField]
		private ItemArchetype[] m_items;

		// Token: 0x0400384A RID: 14410
		[SerializeField]
		private MaterialCategory m_category;
	}
}
