using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Crafting;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.UI;
using SoL.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI.Crafting
{
	// Token: 0x02000992 RID: 2450
	public class ComponentsListItem : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x17001038 RID: 4152
		// (get) Token: 0x06004915 RID: 18709 RVA: 0x0007118E File Offset: 0x0006F38E
		public RecipeComponent Component
		{
			get
			{
				return this.m_component;
			}
		}

		// Token: 0x17001039 RID: 4153
		// (get) Token: 0x06004916 RID: 18710 RVA: 0x00071196 File Offset: 0x0006F396
		// (set) Token: 0x06004917 RID: 18711 RVA: 0x000711B4 File Offset: 0x0006F3B4
		public int SelectedTypeCode
		{
			get
			{
				ComponentsList parent = this.m_parent;
				if (parent == null)
				{
					return 0;
				}
				return parent.Selections[this.m_itemIndex];
			}
			private set
			{
				if (this.m_parent != null)
				{
					this.m_parent.Selections[this.m_itemIndex] = value;
				}
			}
		}

		// Token: 0x06004918 RID: 18712 RVA: 0x000711DB File Offset: 0x0006F3DB
		private void Start()
		{
			this.m_button.onClick.AddListener(new UnityAction(this.OnButtonClicked));
		}

		// Token: 0x06004919 RID: 18713 RVA: 0x001AC4D4 File Offset: 0x001AA6D4
		private void OnDestroy()
		{
			this.m_button.onClick.RemoveListener(new UnityAction(this.OnButtonClicked));
			this.m_itemsListUI.ItemPicked -= this.Select;
			this.m_itemsListUI.DropdownBackingClicked -= this.CloseDropdown;
			this.m_itemsListUI.Initialized -= this.PopulateItemList;
		}

		// Token: 0x0600491A RID: 18714 RVA: 0x001AC544 File Offset: 0x001AA744
		public void Init(ComponentsList parent, RecipeComponent component, int index)
		{
			this.m_parent = parent;
			this.m_component = component;
			this.m_itemIndex = index;
			this.Deselect();
			this.CloseDropdown();
			this.m_descriptionIcon.gameObject.SetActive(!string.IsNullOrEmpty(component.Description) || (component.MaterialCategories != null && component.MaterialCategories.Length != 0) || (component.RawAcceptableMaterials != null && component.RawAcceptableMaterials.Length != 0));
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				if (!string.IsNullOrEmpty(component.Description))
				{
					utf16ValueStringBuilder.AppendLine(component.Description);
					if (component.MaterialCategories != null && component.MaterialCategories.Length != 0)
					{
						utf16ValueStringBuilder.AppendLine();
					}
				}
				if (component.MaterialCategories != null && component.MaterialCategories.Length != 0)
				{
					utf16ValueStringBuilder.AppendLine("Material Categories:");
					foreach (MaterialCategoryWithAmount materialCategoryWithAmount in component.MaterialCategories)
					{
						if (materialCategoryWithAmount.Amount > 1)
						{
							utf16ValueStringBuilder.AppendLine(string.Format(" - {0}x {1}", materialCategoryWithAmount.Amount, materialCategoryWithAmount.Category.DisplayName));
						}
						else
						{
							utf16ValueStringBuilder.AppendLine(" - " + materialCategoryWithAmount.Category.DisplayName);
						}
					}
					if (component.RawAcceptableMaterials != null && component.RawAcceptableMaterials.Length != 0)
					{
						utf16ValueStringBuilder.AppendLine();
					}
				}
				if (component.RawAcceptableMaterials != null && component.RawAcceptableMaterials.Length != 0)
				{
					utf16ValueStringBuilder.AppendLine("Acceptable Materials:");
					foreach (ComponentMaterial componentMaterial in component.RawAcceptableMaterials)
					{
						if (componentMaterial.AmountRequired > 1)
						{
							utf16ValueStringBuilder.AppendLine(string.Format(" - {0}x {1}", componentMaterial.AmountRequired, componentMaterial.Archetype.DisplayName));
						}
						else
						{
							utf16ValueStringBuilder.AppendLine(" - " + componentMaterial.Archetype.DisplayName);
						}
					}
				}
				this.m_descriptionIcon.Text = utf16ValueStringBuilder.ToString();
			}
			TMP_Text componentName = this.m_componentName;
			string text;
			if (this.m_component.RequirementType != ComponentRequirementType.Required)
			{
				string str = "(Optional) ";
				RecipeComponent component2 = this.m_component;
				text = str + ((component2 != null) ? component2.DisplayName : null);
			}
			else
			{
				RecipeComponent component3 = this.m_component;
				text = ((component3 != null) ? component3.DisplayName : null);
			}
			componentName.text = text;
			this.RefreshInteractibility();
			List<ItemUsage> itemsUsedByOtherComponents = this.m_parent.FindItemsUsedInSelection();
			List<ItemUsage> list;
			if (this.SelectedTypeCode == 0 && this.m_canSelect && this.m_component.RequirementType == ComponentRequirementType.Required && this.m_component.TryGetOnlyAvailableMaterial(this.m_parent.AvailableItems, itemsUsedByOtherComponents, this.m_parent.CurrentRecipe.ForceOneComponentPerStack, out list))
			{
				this.Select(list[0].Instance.CombinedTypeCode);
				return;
			}
			this.RefreshVisuals(null);
		}

		// Token: 0x0600491B RID: 18715 RVA: 0x001AC83C File Offset: 0x001AAA3C
		public List<ArchetypeInstance> GetAllInstancesOfTypeInInventory()
		{
			if (this.SelectedTypeCode == 0)
			{
				return null;
			}
			this.m_tempSelection.Clear();
			foreach (ArchetypeInstance archetypeInstance in this.m_parent.AvailableItems)
			{
				if (archetypeInstance.CombinedTypeCode == this.SelectedTypeCode)
				{
					this.m_tempSelection.Add(archetypeInstance);
				}
			}
			return this.m_tempSelection;
		}

		// Token: 0x0600491C RID: 18716 RVA: 0x001AC8C4 File Offset: 0x001AAAC4
		public void Select(int typeCode)
		{
			if (this.m_parent == null)
			{
				return;
			}
			if (typeCode == 0)
			{
				this.Deselect();
				this.CloseDropdown();
				return;
			}
			this.m_tempAmounts.Clear();
			this.m_tempAmountsUsed.Clear();
			foreach (ArchetypeInstance archetypeInstance in LocalPlayer.GameEntity.CollectionController.Inventory.Instances)
			{
				if (!this.m_tempAmounts.ContainsKey(archetypeInstance.CombinedTypeCode))
				{
					this.m_tempAmounts.Add(archetypeInstance.CombinedTypeCode, 0);
				}
				Dictionary<int, int> dictionary = this.m_tempAmounts;
				int combinedTypeCode = archetypeInstance.CombinedTypeCode;
				Dictionary<int, int> dictionary2 = dictionary;
				int key = combinedTypeCode;
				int num = dictionary[combinedTypeCode];
				ItemInstanceData itemData = archetypeInstance.ItemData;
				dictionary2[key] = num + (((itemData != null) ? itemData.Count : null) ?? 1);
			}
			foreach (ArchetypeInstance archetypeInstance2 in LocalPlayer.GameEntity.CollectionController.Gathering.Instances)
			{
				if (!this.m_tempAmounts.ContainsKey(archetypeInstance2.CombinedTypeCode))
				{
					this.m_tempAmounts.Add(archetypeInstance2.CombinedTypeCode, 0);
				}
				Dictionary<int, int> dictionary = this.m_tempAmounts;
				int combinedTypeCode = archetypeInstance2.CombinedTypeCode;
				Dictionary<int, int> dictionary3 = dictionary;
				int key2 = combinedTypeCode;
				int num2 = dictionary[combinedTypeCode];
				ItemInstanceData itemData2 = archetypeInstance2.ItemData;
				dictionary3[key2] = num2 + (((itemData2 != null) ? itemData2.Count : null) ?? 1);
			}
			foreach (ItemUsage itemUsage in this.m_parent.FindItemsUsedInSelection())
			{
				if (!this.m_tempAmountsUsed.ContainsKey(itemUsage.Instance.CombinedTypeCode))
				{
					this.m_tempAmountsUsed.Add(itemUsage.Instance.CombinedTypeCode, 0);
				}
				Dictionary<int, int> dictionary = this.m_tempAmountsUsed;
				int combinedTypeCode = itemUsage.Instance.CombinedTypeCode;
				dictionary[combinedTypeCode] += itemUsage.AmountUsed;
			}
			UniqueId archetypeId = UniqueId.Empty;
			foreach (ArchetypeInstance archetypeInstance3 in this.m_parent.AvailableItems)
			{
				if (archetypeInstance3.CombinedTypeCode == typeCode)
				{
					archetypeId = archetypeInstance3.ArchetypeId;
					break;
				}
			}
			if (this.m_tempAmountsUsed.ContainsKey(typeCode))
			{
				int num3 = this.m_tempAmountsUsed[typeCode];
				ComponentMaterial componentMaterialByArchetypeId = this.m_component.GetComponentMaterialByArchetypeId(archetypeId);
				if (num3 + ((componentMaterialByArchetypeId != null) ? componentMaterialByArchetypeId.AmountRequired : 1) > this.m_tempAmounts[typeCode])
				{
					this.m_parent.DeselectByTypeCode(typeCode);
				}
			}
			this.SelectedTypeCode = typeCode;
			this.RefreshVisuals(this.GetAllInstancesOfTypeInInventory());
			this.CloseDropdown();
			Action selectionsChanged = this.m_parent.SelectionsChanged;
			if (selectionsChanged == null)
			{
				return;
			}
			selectionsChanged();
		}

		// Token: 0x0600491D RID: 18717 RVA: 0x000711F9 File Offset: 0x0006F3F9
		public void Deselect()
		{
			this.SelectedTypeCode = 0;
			this.RefreshVisuals(null);
			if (this.m_parent)
			{
				Action selectionsChanged = this.m_parent.SelectionsChanged;
				if (selectionsChanged == null)
				{
					return;
				}
				selectionsChanged();
			}
		}

		// Token: 0x0600491E RID: 18718 RVA: 0x0007122B File Offset: 0x0006F42B
		public void OnInventoryItemAdded(ArchetypeInstance instance)
		{
			this.RefreshInteractibility();
		}

		// Token: 0x0600491F RID: 18719 RVA: 0x001ACC00 File Offset: 0x001AAE00
		public void OnInventoryItemRemoved(ArchetypeInstance instance)
		{
			this.RefreshInteractibility();
			bool flag = false;
			if (this.SelectedTypeCode != 0)
			{
				List<ItemUsage> list = this.m_parent.FindItemsUsedInSelection();
				List<ItemUsage> fromPool = StaticListPool<ItemUsage>.GetFromPool();
				foreach (ItemUsage item in list)
				{
					if (item.UsedFor.Id != this.m_component.Id)
					{
						fromPool.Add(item);
					}
				}
				List<ItemUsage> list2;
				flag = !this.m_component.GetInstanceUsagesForTypeCode(this.m_parent.AvailableItems, fromPool, this.m_parent.CurrentRecipe.ForceOneComponentPerStack, this.SelectedTypeCode, out list2);
				StaticListPool<ItemUsage>.ReturnToPool(fromPool);
			}
			if (!this.m_canSelect || flag)
			{
				this.Deselect();
			}
		}

		// Token: 0x06004920 RID: 18720 RVA: 0x001ACCDC File Offset: 0x001AAEDC
		public void OnQuantityOfItemChanged()
		{
			this.RefreshInteractibility();
			bool flag = false;
			if (this.SelectedTypeCode != 0)
			{
				List<ItemUsage> list = this.m_parent.FindItemsUsedInSelection();
				List<ItemUsage> fromPool = StaticListPool<ItemUsage>.GetFromPool();
				foreach (ItemUsage item in list)
				{
					if (item.UsedFor.Id != this.m_component.Id)
					{
						fromPool.Add(item);
					}
				}
				List<ItemUsage> list2;
				flag = !this.m_component.GetInstanceUsagesForTypeCode(this.m_parent.AvailableItems, fromPool, this.m_parent.CurrentRecipe.ForceOneComponentPerStack, this.SelectedTypeCode, out list2);
				StaticListPool<ItemUsage>.ReturnToPool(fromPool);
			}
			if (!this.m_canSelect || flag)
			{
				this.Deselect();
			}
			this.RefreshVisuals(this.GetAllInstancesOfTypeInInventory());
		}

		// Token: 0x06004921 RID: 18721 RVA: 0x00071233 File Offset: 0x0006F433
		public void LockUI()
		{
			this.CloseDropdown();
			this.m_button.interactable = false;
		}

		// Token: 0x06004922 RID: 18722 RVA: 0x00071247 File Offset: 0x0006F447
		public void UnlockUI()
		{
			this.m_button.interactable = this.m_canSelect;
		}

		// Token: 0x06004923 RID: 18723 RVA: 0x001ACDC4 File Offset: 0x001AAFC4
		public void RefreshInteractibility()
		{
			this.m_canSelect = false;
			foreach (ArchetypeInstance archetypeInstance in this.m_parent.AvailableItems)
			{
				if (this.m_component.GetComponentMaterialByArchetypeId(archetypeInstance.ArchetypeId) != null)
				{
					this.m_canSelect = true;
					break;
				}
			}
			this.m_button.interactable = this.m_canSelect;
		}

		// Token: 0x06004924 RID: 18724 RVA: 0x001ACE4C File Offset: 0x001AB04C
		public void RefreshVisuals(List<ArchetypeInstance> instances)
		{
			int amountRequired = -1;
			bool optional = false;
			if (instances != null && instances.Count > 0)
			{
				foreach (ComponentMaterial componentMaterial in this.m_component.AcceptableMaterials)
				{
					if (instances[0].ArchetypeId == componentMaterial.Archetype.Id)
					{
						amountRequired = componentMaterial.AmountRequired;
						optional = (this.m_component.RequirementType > ComponentRequirementType.Required);
						break;
					}
				}
			}
			this.m_itemButton.SetItem(instances, amountRequired, optional);
		}

		// Token: 0x06004925 RID: 18725 RVA: 0x001ACED0 File Offset: 0x001AB0D0
		private void OnButtonClicked()
		{
			if (!this.m_isOpen)
			{
				this.m_parent.CloseDropdown();
				this.m_dropdownContainer.rectTransform.SetPositionAndRotation(new Vector3(this.m_itemButtonRect.position.x, this.m_itemButtonRect.position.y - this.m_itemButtonRect.rect.height, 0f), this.m_dropdownContainer.rectTransform.rotation);
				this.m_isOpen = true;
				this.m_dropdownContainer.gameObject.SetActive(true);
				if (this.m_itemsListUI.IsInitialized)
				{
					this.PopulateItemList();
				}
				else
				{
					this.m_itemsListUI.Initialized += this.PopulateItemList;
				}
				this.m_itemsListUI.ItemPicked += this.Select;
				this.m_itemsListUI.DropdownBackingClicked += this.CloseDropdown;
				return;
			}
			this.CloseDropdown();
		}

		// Token: 0x06004926 RID: 18726 RVA: 0x001ACFCC File Offset: 0x001AB1CC
		public void CloseDropdown()
		{
			this.m_isOpen = false;
			this.m_dropdownContainer.gameObject.SetActive(false);
			this.m_itemsListUI.ItemPicked -= this.Select;
			this.m_itemsListUI.DropdownBackingClicked -= this.CloseDropdown;
		}

		// Token: 0x06004927 RID: 18727 RVA: 0x001AD020 File Offset: 0x001AB220
		private void PopulateItemList()
		{
			List<ArchetypeInstance> fromPool = StaticListPool<ArchetypeInstance>.GetFromPool();
			foreach (ArchetypeInstance archetypeInstance in this.m_parent.AvailableItems)
			{
				if (archetypeInstance.CombinedTypeCode != this.SelectedTypeCode && this.m_component.GetComponentMaterialByArchetypeId(archetypeInstance.ArchetypeId) != null)
				{
					fromPool.Add(archetypeInstance);
				}
			}
			this.m_itemsListUI.UpdateItems(this, this.m_component, fromPool);
			StaticListPool<ArchetypeInstance>.ReturnToPool(fromPool);
		}

		// Token: 0x06004928 RID: 18728 RVA: 0x001AD0B8 File Offset: 0x001AB2B8
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.SelectedTypeCode == 0)
			{
				return null;
			}
			foreach (ArchetypeInstance archetypeInstance in this.m_parent.AvailableItems)
			{
				if (archetypeInstance.CombinedTypeCode == this.SelectedTypeCode)
				{
					return new ArchetypeTooltipParameter
					{
						Archetype = archetypeInstance.Archetype
					};
				}
			}
			return null;
		}

		// Token: 0x1700103A RID: 4154
		// (get) Token: 0x06004929 RID: 18729 RVA: 0x0007125A File Offset: 0x0006F45A
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x1700103B RID: 4155
		// (get) Token: 0x0600492A RID: 18730 RVA: 0x00071268 File Offset: 0x0006F468
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x1700103C RID: 4156
		// (get) Token: 0x0600492B RID: 18731 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600492D RID: 18733 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004426 RID: 17446
		private const string kDefaultItemText = "Choose an item...";

		// Token: 0x04004427 RID: 17447
		[SerializeField]
		private SolButton m_button;

		// Token: 0x04004428 RID: 17448
		[SerializeField]
		private ItemButton m_itemButton;

		// Token: 0x04004429 RID: 17449
		[SerializeField]
		private RectTransform m_itemButtonRect;

		// Token: 0x0400442A RID: 17450
		[SerializeField]
		private TextMeshProUGUI m_componentName;

		// Token: 0x0400442B RID: 17451
		[SerializeField]
		private TextTooltipTrigger m_descriptionIcon;

		// Token: 0x0400442C RID: 17452
		[SerializeField]
		private Image m_dropdownContainer;

		// Token: 0x0400442D RID: 17453
		[SerializeField]
		private ItemsList m_itemsListUI;

		// Token: 0x0400442E RID: 17454
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x0400442F RID: 17455
		private ComponentsList m_parent;

		// Token: 0x04004430 RID: 17456
		private RecipeComponent m_component;

		// Token: 0x04004431 RID: 17457
		private int m_itemIndex;

		// Token: 0x04004432 RID: 17458
		private bool m_canSelect = true;

		// Token: 0x04004433 RID: 17459
		private bool m_isOpen;

		// Token: 0x04004434 RID: 17460
		private List<ArchetypeInstance> m_tempSelection = new List<ArchetypeInstance>();

		// Token: 0x04004435 RID: 17461
		private Dictionary<int, int> m_tempAmounts = new Dictionary<int, int>();

		// Token: 0x04004436 RID: 17462
		private Dictionary<int, int> m_tempAmountsUsed = new Dictionary<int, int>();
	}
}
