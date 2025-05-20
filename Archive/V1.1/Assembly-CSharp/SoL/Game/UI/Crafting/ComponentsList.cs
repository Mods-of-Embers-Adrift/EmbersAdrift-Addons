using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using SoL.Game.Crafting;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Utilities;
using UnityEngine.EventSystems;

namespace SoL.Game.UI.Crafting
{
	// Token: 0x0200098F RID: 2447
	public class ComponentsList : GridAdapter<GridParams, ComponentsListItemViewsHolder>, IPointerDownHandler, IEventSystemHandler
	{
		// Token: 0x17001033 RID: 4147
		// (get) Token: 0x060048F7 RID: 18679 RVA: 0x0007108A File Offset: 0x0006F28A
		// (set) Token: 0x060048F8 RID: 18680 RVA: 0x00071092 File Offset: 0x0006F292
		public bool SelectionInvalid { get; private set; }

		// Token: 0x17001034 RID: 4148
		// (get) Token: 0x060048F9 RID: 18681 RVA: 0x0007109B File Offset: 0x0006F29B
		// (set) Token: 0x060048FA RID: 18682 RVA: 0x000710A3 File Offset: 0x0006F2A3
		public Recipe CurrentRecipe { get; private set; }

		// Token: 0x17001035 RID: 4149
		// (get) Token: 0x060048FB RID: 18683 RVA: 0x000710AC File Offset: 0x0006F2AC
		public List<ArchetypeInstance> AvailableItems { get; } = new List<ArchetypeInstance>();

		// Token: 0x060048FC RID: 18684 RVA: 0x001ABCE0 File Offset: 0x001A9EE0
		public void UpdateRecipe(Recipe newRecipe)
		{
			this.CloseDropdown();
			bool flag = this.CurrentRecipe != newRecipe;
			this.CurrentRecipe = newRecipe;
			if (base.IsInitialized)
			{
				if (this.CurrentRecipe != null && this.CurrentRecipe.Components != null && this.CurrentRecipe.Components.Length != 0)
				{
					List<RecipeComponent> fromPool = StaticListPool<RecipeComponent>.GetFromPool();
					ContainerInstance containerInstance;
					ContainerInstance containerInstance2;
					if (LocalPlayer.GameEntity != null && LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.Inventory, out containerInstance) && LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.Gathering, out containerInstance2))
					{
						this.AvailableItems.Clear();
						this.AvailableItems.AddRange(containerInstance2.Instances);
						this.AvailableItems.AddRange(containerInstance.Instances);
						foreach (RecipeComponent recipeComponent in this.CurrentRecipe.Components)
						{
							if (recipeComponent.Enabled && (recipeComponent.RequirementType != ComponentRequirementType.OptionalHidden || recipeComponent.AnyAcceptableMaterials(this.AvailableItems, null, this.CurrentRecipe.ForceOneComponentPerStack)))
							{
								fromPool.Add(recipeComponent);
							}
						}
					}
					if (flag || fromPool.Count != this.m_items.Count)
					{
						this.UpdateItems(Array.Empty<RecipeComponent>());
						this.UpdateItems(fromPool);
					}
					StaticListPool<RecipeComponent>.ReturnToPool(fromPool);
					return;
				}
				this.UpdateItems(Array.Empty<RecipeComponent>());
			}
		}

		// Token: 0x060048FD RID: 18685 RVA: 0x001ABE4C File Offset: 0x001AA04C
		public List<ItemUsage> FindItemsUsedInSelection()
		{
			if (!this.m_selectionDirty)
			{
				return this.m_selectionItems;
			}
			this.m_selectionItems.Clear();
			this.SelectionInvalid = false;
			ContainerInstance containerInstance;
			ContainerInstance containerInstance2;
			if (LocalPlayer.GameEntity != null && LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.Inventory, out containerInstance) && LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.Gathering, out containerInstance2))
			{
				this.AvailableItems.Clear();
				this.AvailableItems.AddRange(containerInstance2.Instances);
				this.AvailableItems.AddRange(containerInstance.Instances);
				for (int i = 0; i < this.Selections.Count; i++)
				{
					if (this.Selections[i] != 0)
					{
						foreach (ArchetypeInstance archetypeInstance in this.AvailableItems)
						{
							if (archetypeInstance.CombinedTypeCode == this.Selections[i])
							{
								List<ItemUsage> collection;
								if (this.m_items[i].GetInstanceUsagesForTypeCode(this.AvailableItems, this.m_selectionItems, this.CurrentRecipe.ForceOneComponentPerStack, this.Selections[i], out collection))
								{
									this.m_selectionItems.AddRange(collection);
								}
								else
								{
									this.SelectionInvalid = true;
								}
							}
							int num = 0;
							foreach (ItemUsage itemUsage in this.m_selectionItems)
							{
								if (itemUsage.Instance.CombinedTypeCode == this.Selections[i] && itemUsage.UsedFor.Id == this.m_items[i].Id)
								{
									num += itemUsage.AmountUsed;
								}
							}
							ComponentMaterial componentMaterialByArchetypeId = this.m_items[i].GetComponentMaterialByArchetypeId(archetypeInstance.ArchetypeId);
							int num2 = (componentMaterialByArchetypeId != null) ? componentMaterialByArchetypeId.AmountRequired : 1;
							if (num >= num2)
							{
								break;
							}
						}
					}
				}
			}
			this.m_selectionDirty = false;
			return this.m_selectionItems;
		}

		// Token: 0x060048FE RID: 18686 RVA: 0x000710B4 File Offset: 0x0006F2B4
		public void MarkSelectionDirty()
		{
			this.m_selectionDirty = true;
		}

		// Token: 0x060048FF RID: 18687 RVA: 0x001AC098 File Offset: 0x001AA298
		public void OnInventoryAddition(ArchetypeInstance instance)
		{
			this.AvailableItems.Add(instance);
			this.MarkSelectionDirty();
			for (int i = 0; i < base.CellsCount; i++)
			{
				ComponentsListItemViewsHolder cellViewsHolderIfVisible = this.GetCellViewsHolderIfVisible(i);
				if (cellViewsHolderIfVisible != null)
				{
					cellViewsHolderIfVisible.ListItem.OnInventoryItemAdded(instance);
				}
			}
		}

		// Token: 0x06004900 RID: 18688 RVA: 0x001AC0E0 File Offset: 0x001AA2E0
		public void OnInventoryRemoval(ArchetypeInstance instance)
		{
			this.AvailableItems.RemoveAll((ArchetypeInstance x) => x.InstanceId == instance.InstanceId);
			this.MarkSelectionDirty();
			for (int i = 0; i < base.CellsCount; i++)
			{
				ComponentsListItemViewsHolder cellViewsHolderIfVisible = this.GetCellViewsHolderIfVisible(i);
				if (cellViewsHolderIfVisible != null)
				{
					cellViewsHolderIfVisible.ListItem.OnInventoryItemRemoved(instance);
				}
			}
		}

		// Token: 0x06004901 RID: 18689 RVA: 0x001AC148 File Offset: 0x001AA348
		public void OnQuantityOfItemChanged()
		{
			this.MarkSelectionDirty();
			for (int i = 0; i < base.CellsCount; i++)
			{
				ComponentsListItemViewsHolder cellViewsHolderIfVisible = this.GetCellViewsHolderIfVisible(i);
				if (cellViewsHolderIfVisible != null)
				{
					cellViewsHolderIfVisible.ListItem.OnQuantityOfItemChanged();
				}
			}
		}

		// Token: 0x06004902 RID: 18690 RVA: 0x001AC184 File Offset: 0x001AA384
		public void CloseDropdown()
		{
			for (int i = 0; i < base.CellsCount; i++)
			{
				ComponentsListItemViewsHolder cellViewsHolderIfVisible = this.GetCellViewsHolderIfVisible(i);
				if (cellViewsHolderIfVisible != null)
				{
					cellViewsHolderIfVisible.ListItem.CloseDropdown();
				}
			}
		}

		// Token: 0x06004903 RID: 18691 RVA: 0x001AC1BC File Offset: 0x001AA3BC
		public void DeselectByComponent(RecipeComponent component)
		{
			for (int i = 0; i < this.m_items.Count; i++)
			{
				ComponentsListItemViewsHolder cellViewsHolderIfVisible = this.GetCellViewsHolderIfVisible(i);
				ComponentsListItem componentsListItem = (cellViewsHolderIfVisible != null) ? cellViewsHolderIfVisible.ListItem : null;
				if (((componentsListItem != null) ? componentsListItem.Component : null) == component)
				{
					componentsListItem.Deselect();
				}
				else if (this.m_items[i] == component)
				{
					this.Selections[i] = 0;
				}
			}
		}

		// Token: 0x06004904 RID: 18692 RVA: 0x001AC228 File Offset: 0x001AA428
		public void DeselectByTypeCode(int typeCode)
		{
			for (int i = 0; i < this.m_items.Count; i++)
			{
				if (this.Selections[i] == typeCode)
				{
					ComponentsListItemViewsHolder cellViewsHolderIfVisible = this.GetCellViewsHolderIfVisible(i);
					ComponentsListItem componentsListItem = (cellViewsHolderIfVisible != null) ? cellViewsHolderIfVisible.ListItem : null;
					if (componentsListItem != null)
					{
						componentsListItem.Deselect();
					}
					else
					{
						this.Selections[i] = 0;
					}
				}
			}
		}

		// Token: 0x06004905 RID: 18693 RVA: 0x001AC28C File Offset: 0x001AA48C
		public void LockUI()
		{
			this.StopScrollingIfAny();
			for (int i = 0; i < base.CellsCount; i++)
			{
				ComponentsListItemViewsHolder cellViewsHolderIfVisible = this.GetCellViewsHolderIfVisible(i);
				if (cellViewsHolderIfVisible != null)
				{
					cellViewsHolderIfVisible.ListItem.LockUI();
				}
			}
		}

		// Token: 0x06004906 RID: 18694 RVA: 0x001AC2C8 File Offset: 0x001AA4C8
		public void UnlockUI()
		{
			for (int i = 0; i < base.CellsCount; i++)
			{
				ComponentsListItemViewsHolder cellViewsHolderIfVisible = this.GetCellViewsHolderIfVisible(i);
				if (cellViewsHolderIfVisible != null)
				{
					cellViewsHolderIfVisible.ListItem.UnlockUI();
				}
			}
		}

		// Token: 0x06004907 RID: 18695 RVA: 0x001AC300 File Offset: 0x001AA500
		public bool AllComponentsHaveSelection()
		{
			bool flag = true;
			for (int i = 0; i < this.m_items.Count; i++)
			{
				flag = (flag && ((this.m_items[i] != null && this.m_items[i].RequirementType != ComponentRequirementType.Required) || (this.Selections.Count > 0 && this.Selections[i] != 0)));
			}
			return flag;
		}

		// Token: 0x06004908 RID: 18696 RVA: 0x001AC374 File Offset: 0x001AA574
		private void UpdateItems(ICollection<RecipeComponent> items)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.m_items == null)
			{
				this.m_items = new List<RecipeComponent>(items.Count);
			}
			this.m_items.Clear();
			this.m_items.AddRange(items);
			this.Selections.Clear();
			this.MarkSelectionDirty();
			while (this.Selections.Count < this.m_items.Count)
			{
				this.Selections.Add(0);
			}
			ContainerInstance containerInstance;
			ContainerInstance containerInstance2;
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.Inventory, out containerInstance) && LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.Gathering, out containerInstance2))
			{
				this.AvailableItems.Clear();
				this.AvailableItems.AddRange(containerInstance2.Instances);
				this.AvailableItems.AddRange(containerInstance.Instances);
			}
			this.ResetItems(this.m_items.Count, false, false);
		}

		// Token: 0x06004909 RID: 18697 RVA: 0x000710BD File Offset: 0x0006F2BD
		protected override void UpdateCellViewsHolder(ComponentsListItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateItem(this, this.m_items[newOrRecycled.ItemIndex]);
		}

		// Token: 0x0600490A RID: 18698 RVA: 0x000710D7 File Offset: 0x0006F2D7
		public void OnPointerDown(PointerEventData eventData)
		{
			this.CloseDropdown();
		}

		// Token: 0x0400441B RID: 17435
		private List<RecipeComponent> m_items;

		// Token: 0x0400441C RID: 17436
		private List<ItemUsage> m_selectionItems = new List<ItemUsage>();

		// Token: 0x0400441D RID: 17437
		public List<int> Selections = new List<int>();

		// Token: 0x0400441F RID: 17439
		public Action SelectionsChanged;

		// Token: 0x04004422 RID: 17442
		private bool m_selectionDirty;
	}
}
