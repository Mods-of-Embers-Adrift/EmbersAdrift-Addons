using System;
using System.Collections.Generic;
using SoL.Game.Crafting;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AC8 RID: 2760
	[Serializable]
	public class RecipeComponent
	{
		// Token: 0x170013A3 RID: 5027
		// (get) Token: 0x0600552B RID: 21803 RVA: 0x00078E18 File Offset: 0x00077018
		public UniqueId Id
		{
			get
			{
				return this.m_id;
			}
		}

		// Token: 0x170013A4 RID: 5028
		// (get) Token: 0x0600552C RID: 21804 RVA: 0x00078E20 File Offset: 0x00077020
		public bool Enabled
		{
			get
			{
				return this.m_enabled;
			}
		}

		// Token: 0x170013A5 RID: 5029
		// (get) Token: 0x0600552D RID: 21805 RVA: 0x00078E28 File Offset: 0x00077028
		public string DisplayName
		{
			get
			{
				return this.m_displayName;
			}
		}

		// Token: 0x170013A6 RID: 5030
		// (get) Token: 0x0600552E RID: 21806 RVA: 0x00078E30 File Offset: 0x00077030
		public string Description
		{
			get
			{
				return this.m_description;
			}
		}

		// Token: 0x170013A7 RID: 5031
		// (get) Token: 0x0600552F RID: 21807 RVA: 0x00078E38 File Offset: 0x00077038
		public ComponentRequirementType RequirementType
		{
			get
			{
				return this.m_requirementType;
			}
		}

		// Token: 0x170013A8 RID: 5032
		// (get) Token: 0x06005530 RID: 21808 RVA: 0x00078E40 File Offset: 0x00077040
		public MaterialCategoryWithAmount[] MaterialCategories
		{
			get
			{
				return this.m_materialCategoriesWithAmounts;
			}
		}

		// Token: 0x170013A9 RID: 5033
		// (get) Token: 0x06005531 RID: 21809 RVA: 0x00078E48 File Offset: 0x00077048
		public ComponentMaterial[] RawAcceptableMaterials
		{
			get
			{
				return this.m_acceptableMaterials;
			}
		}

		// Token: 0x170013AA RID: 5034
		// (get) Token: 0x06005532 RID: 21810 RVA: 0x001DD49C File Offset: 0x001DB69C
		public ComponentMaterial[] AcceptableMaterials
		{
			get
			{
				if (this.m_allAcceptableMaterials == null)
				{
					List<ComponentMaterial> list = StaticListPool<ComponentMaterial>.GetFromPool();
					list.AddRange(this.m_acceptableMaterials);
					foreach (MaterialCategoryWithAmount materialCategoryWithAmount in this.m_materialCategoriesWithAmounts)
					{
						list = materialCategoryWithAmount.Category.AggregateMaterialList(materialCategoryWithAmount.Amount, list);
					}
					this.m_allAcceptableMaterials = list.ToArray();
					StaticListPool<ComponentMaterial>.ReturnToPool(list);
				}
				return this.m_allAcceptableMaterials;
			}
		}

		// Token: 0x06005533 RID: 21811 RVA: 0x001DD508 File Offset: 0x001DB708
		public ComponentMaterial GetComponentMaterialByArchetypeId(UniqueId archetypeId)
		{
			foreach (ComponentMaterial componentMaterial in this.m_acceptableMaterials)
			{
				if (componentMaterial.Archetype.Id == archetypeId)
				{
					return componentMaterial;
				}
			}
			foreach (MaterialCategoryWithAmount materialCategoryWithAmount in this.m_materialCategoriesWithAmounts)
			{
				ComponentMaterial componentMaterial2 = materialCategoryWithAmount.Category.GetComponentMaterial(archetypeId, materialCategoryWithAmount.Amount);
				if (componentMaterial2 != null)
				{
					return componentMaterial2;
				}
			}
			return null;
		}

		// Token: 0x06005534 RID: 21812 RVA: 0x001DD57C File Offset: 0x001DB77C
		public bool TryGetMatchingMaterials(List<ArchetypeInstance> inputItems, List<ItemUsage> itemsUsedByOtherComponents, bool forceOneComponentPerStack, out List<ItemUsage> itemsUsed)
		{
			itemsUsed = RecipeComponent.m_itemsUsed;
			itemsUsed.Clear();
			ComponentMaterial componentMaterial = this.FindFirstAcceptableMaterial(inputItems, itemsUsedByOtherComponents, forceOneComponentPerStack);
			if (componentMaterial == null)
			{
				return false;
			}
			this.GetInstanceUsagesForMaterial(inputItems, itemsUsedByOtherComponents, forceOneComponentPerStack, componentMaterial, out itemsUsed);
			return itemsUsed.Count != 0;
		}

		// Token: 0x06005535 RID: 21813 RVA: 0x001DD5C4 File Offset: 0x001DB7C4
		public bool TryGetOnlyAvailableMaterial(List<ArchetypeInstance> inputItems, List<ItemUsage> itemsUsedByOtherComponents, bool forceOneComponentPerStack, out List<ItemUsage> itemsUsed)
		{
			itemsUsed = RecipeComponent.m_itemsUsed;
			itemsUsed.Clear();
			ComponentMaterial componentMaterial = this.FindExactlyOneAcceptableMaterial(inputItems, itemsUsedByOtherComponents, forceOneComponentPerStack);
			if (componentMaterial == null)
			{
				return false;
			}
			this.GetInstanceUsagesForMaterial(inputItems, itemsUsedByOtherComponents, forceOneComponentPerStack, componentMaterial, out itemsUsed);
			return itemsUsed.Count != 0;
		}

		// Token: 0x06005536 RID: 21814 RVA: 0x001DD60C File Offset: 0x001DB80C
		public bool AnyAcceptableMaterials(List<ArchetypeInstance> inputItems, List<ItemUsage> itemsUsedByOtherComponents, bool forceOneComponentPerStack)
		{
			int num = 0;
			foreach (ComponentMaterial componentMaterial in this.AcceptableMaterials)
			{
				RecipeComponent.m_perDynamicArchetypeCounts.Clear();
				foreach (ArchetypeInstance archetypeInstance in inputItems)
				{
					if (((archetypeInstance != null) ? archetypeInstance.ItemData : null) == null)
					{
						if (!RecipeComponent.m_nonItemErrorReported)
						{
							string format = "Non-item found during crafting checks. CharacterID: {0}, InstanceID: {1}";
							GameEntity gameEntity = LocalPlayer.GameEntity;
							UniqueId? uniqueId;
							if (gameEntity == null)
							{
								uniqueId = null;
							}
							else
							{
								CharacterData characterData = gameEntity.CharacterData;
								uniqueId = ((characterData != null) ? new UniqueId?(characterData.CharacterId) : null);
							}
							Debug.LogError(string.Format(format, uniqueId, (archetypeInstance != null) ? new UniqueId?(archetypeInstance.InstanceId) : null));
							RecipeComponent.m_nonItemErrorReported = true;
						}
					}
					else if (archetypeInstance.ArchetypeId == componentMaterial.Archetype.Id)
					{
						int treeFingerprint = archetypeInstance.ItemData.GetTreeFingerprint();
						if (treeFingerprint != 0 && !RecipeComponent.m_perDynamicArchetypeCounts.ContainsKey(treeFingerprint))
						{
							RecipeComponent.m_perDynamicArchetypeCounts.Add(treeFingerprint, 0);
							num = 0;
						}
						else if (treeFingerprint != 0)
						{
							num = RecipeComponent.m_perDynamicArchetypeCounts[treeFingerprint];
						}
						if (!this.ShouldPreferAnotherStack(inputItems, itemsUsedByOtherComponents, archetypeInstance, num, forceOneComponentPerStack) && num < componentMaterial.AmountRequired)
						{
							int num2 = num;
							ItemInstanceData itemData = archetypeInstance.ItemData;
							num = num2 + (((itemData != null) ? itemData.Count : null) ?? 1);
							if (treeFingerprint != 0)
							{
								Dictionary<int, int> perDynamicArchetypeCounts = RecipeComponent.m_perDynamicArchetypeCounts;
								int num3 = treeFingerprint;
								Dictionary<int, int> dictionary = perDynamicArchetypeCounts;
								int key = num3;
								int num4 = perDynamicArchetypeCounts[num3];
								ItemInstanceData itemData2 = archetypeInstance.ItemData;
								dictionary[key] = num4 + (((itemData2 != null) ? itemData2.Count : null) ?? 1);
							}
							if (num >= componentMaterial.AmountRequired)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06005537 RID: 21815 RVA: 0x001DD834 File Offset: 0x001DBA34
		public ComponentMaterial FindExactlyOneAcceptableMaterial(List<ArchetypeInstance> inputItems, List<ItemUsage> itemsUsedByOtherComponents, bool forceOneComponentPerStack)
		{
			int num = 0;
			ComponentMaterial result = null;
			foreach (ComponentMaterial componentMaterial in this.AcceptableMaterials)
			{
				int num2 = 0;
				RecipeComponent.m_perDynamicArchetypeCounts.Clear();
				foreach (ArchetypeInstance archetypeInstance in inputItems)
				{
					if (((archetypeInstance != null) ? archetypeInstance.ItemData : null) == null)
					{
						if (!RecipeComponent.m_nonItemErrorReported)
						{
							string format = "Non-item found during crafting checks. CharacterID: {0}, InstanceID: {1}";
							GameEntity gameEntity = LocalPlayer.GameEntity;
							UniqueId? uniqueId;
							if (gameEntity == null)
							{
								uniqueId = null;
							}
							else
							{
								CharacterData characterData = gameEntity.CharacterData;
								uniqueId = ((characterData != null) ? new UniqueId?(characterData.CharacterId) : null);
							}
							Debug.LogError(string.Format(format, uniqueId, (archetypeInstance != null) ? new UniqueId?(archetypeInstance.InstanceId) : null));
							RecipeComponent.m_nonItemErrorReported = true;
						}
					}
					else if (archetypeInstance.ArchetypeId == componentMaterial.Archetype.Id)
					{
						int treeFingerprint = archetypeInstance.ItemData.GetTreeFingerprint();
						if (treeFingerprint != 0 && !RecipeComponent.m_perDynamicArchetypeCounts.ContainsKey(treeFingerprint))
						{
							RecipeComponent.m_perDynamicArchetypeCounts.Add(treeFingerprint, 0);
							num2 = 0;
						}
						else if (treeFingerprint != 0)
						{
							num2 = RecipeComponent.m_perDynamicArchetypeCounts[treeFingerprint];
						}
						if (!this.ShouldPreferAnotherStack(inputItems, itemsUsedByOtherComponents, archetypeInstance, num2, forceOneComponentPerStack) && num2 < componentMaterial.AmountRequired)
						{
							int num3 = num2;
							ItemInstanceData itemData = archetypeInstance.ItemData;
							num2 = num3 + (((itemData != null) ? itemData.Count : null) ?? 1);
							if (treeFingerprint != 0)
							{
								Dictionary<int, int> perDynamicArchetypeCounts = RecipeComponent.m_perDynamicArchetypeCounts;
								int num4 = treeFingerprint;
								Dictionary<int, int> dictionary = perDynamicArchetypeCounts;
								int key = num4;
								int num5 = perDynamicArchetypeCounts[num4];
								ItemInstanceData itemData2 = archetypeInstance.ItemData;
								dictionary[key] = num5 + (((itemData2 != null) ? itemData2.Count : null) ?? 1);
							}
							if (num2 >= componentMaterial.AmountRequired)
							{
								num++;
								result = componentMaterial;
								if (num > 1)
								{
									return null;
								}
							}
						}
					}
				}
			}
			if (num != 1)
			{
				return null;
			}
			return result;
		}

		// Token: 0x06005538 RID: 21816 RVA: 0x001DDA7C File Offset: 0x001DBC7C
		public ComponentMaterial FindFirstAcceptableMaterial(List<ArchetypeInstance> inputItems, List<ItemUsage> itemsUsedByOtherComponents, bool forceOneComponentPerStack)
		{
			int num = 0;
			foreach (ComponentMaterial componentMaterial in this.AcceptableMaterials)
			{
				RecipeComponent.m_perDynamicArchetypeCounts.Clear();
				foreach (ArchetypeInstance archetypeInstance in inputItems)
				{
					if (((archetypeInstance != null) ? archetypeInstance.ItemData : null) == null)
					{
						if (!RecipeComponent.m_nonItemErrorReported)
						{
							string format = "Non-item found during crafting checks. CharacterID: {0}, InstanceID: {1}";
							GameEntity gameEntity = LocalPlayer.GameEntity;
							UniqueId? uniqueId;
							if (gameEntity == null)
							{
								uniqueId = null;
							}
							else
							{
								CharacterData characterData = gameEntity.CharacterData;
								uniqueId = ((characterData != null) ? new UniqueId?(characterData.CharacterId) : null);
							}
							Debug.LogError(string.Format(format, uniqueId, (archetypeInstance != null) ? new UniqueId?(archetypeInstance.InstanceId) : null));
							RecipeComponent.m_nonItemErrorReported = true;
						}
					}
					else if (archetypeInstance.ArchetypeId == componentMaterial.Archetype.Id)
					{
						int treeFingerprint = archetypeInstance.ItemData.GetTreeFingerprint();
						if (treeFingerprint != 0 && !RecipeComponent.m_perDynamicArchetypeCounts.ContainsKey(treeFingerprint))
						{
							RecipeComponent.m_perDynamicArchetypeCounts.Add(treeFingerprint, 0);
							num = 0;
						}
						else if (treeFingerprint != 0)
						{
							num = RecipeComponent.m_perDynamicArchetypeCounts[treeFingerprint];
						}
						if (!this.ShouldPreferAnotherStack(inputItems, itemsUsedByOtherComponents, archetypeInstance, num, forceOneComponentPerStack) && num < componentMaterial.AmountRequired)
						{
							int num2 = num;
							ItemInstanceData itemData = archetypeInstance.ItemData;
							num = num2 + (((itemData != null) ? itemData.Count : null) ?? 1);
							if (treeFingerprint != 0)
							{
								Dictionary<int, int> perDynamicArchetypeCounts = RecipeComponent.m_perDynamicArchetypeCounts;
								int num3 = treeFingerprint;
								Dictionary<int, int> dictionary = perDynamicArchetypeCounts;
								int key = num3;
								int num4 = perDynamicArchetypeCounts[num3];
								ItemInstanceData itemData2 = archetypeInstance.ItemData;
								dictionary[key] = num4 + (((itemData2 != null) ? itemData2.Count : null) ?? 1);
							}
							if (num >= componentMaterial.AmountRequired)
							{
								return componentMaterial;
							}
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06005539 RID: 21817 RVA: 0x001DDCA4 File Offset: 0x001DBEA4
		private void GetInstanceUsagesForMaterial(List<ArchetypeInstance> inputItems, List<ItemUsage> itemsUsedByOtherComponents, bool forceOneComponentPerStack, ComponentMaterial acceptedMaterial, out List<ItemUsage> itemsUsed)
		{
			itemsUsed = RecipeComponent.m_itemsUsed;
			itemsUsed.Clear();
			if (acceptedMaterial != null)
			{
				int num = acceptedMaterial.AmountRequired;
				RecipeComponent.m_perDynamicArchetypeCounts.Clear();
				foreach (ArchetypeInstance archetypeInstance in inputItems)
				{
					if (((archetypeInstance != null) ? archetypeInstance.ItemData : null) == null)
					{
						if (!RecipeComponent.m_nonItemErrorReported)
						{
							string format = "Non-item found during crafting checks. CharacterID: {0}, InstanceID: {1}";
							GameEntity gameEntity = LocalPlayer.GameEntity;
							UniqueId? uniqueId;
							if (gameEntity == null)
							{
								uniqueId = null;
							}
							else
							{
								CharacterData characterData = gameEntity.CharacterData;
								uniqueId = ((characterData != null) ? new UniqueId?(characterData.CharacterId) : null);
							}
							Debug.LogError(string.Format(format, uniqueId, (archetypeInstance != null) ? new UniqueId?(archetypeInstance.InstanceId) : null));
							RecipeComponent.m_nonItemErrorReported = true;
						}
					}
					else
					{
						int treeFingerprint = archetypeInstance.ItemData.GetTreeFingerprint();
						if (treeFingerprint != 0 && !RecipeComponent.m_perDynamicArchetypeCounts.ContainsKey(treeFingerprint))
						{
							RecipeComponent.m_perDynamicArchetypeCounts.Add(treeFingerprint, acceptedMaterial.AmountRequired);
							num = acceptedMaterial.AmountRequired;
						}
						else if (treeFingerprint != 0)
						{
							num = RecipeComponent.m_perDynamicArchetypeCounts[treeFingerprint];
						}
						if (!this.ShouldPreferAnotherStack(inputItems, itemsUsedByOtherComponents, archetypeInstance, num, forceOneComponentPerStack) && archetypeInstance.ArchetypeId == acceptedMaterial.Archetype.Id && num > 0)
						{
							ItemInstanceData itemData = archetypeInstance.ItemData;
							int val = ((itemData != null) ? itemData.Count : null) ?? 1;
							int num2 = Math.Min(num, val);
							if (num2 > 0)
							{
								itemsUsed.Add(new ItemUsage
								{
									Instance = archetypeInstance,
									UsedFor = this,
									AmountUsed = num2
								});
								num -= num2;
								if (treeFingerprint != 0)
								{
									Dictionary<int, int> perDynamicArchetypeCounts = RecipeComponent.m_perDynamicArchetypeCounts;
									int key = treeFingerprint;
									perDynamicArchetypeCounts[key] -= num2;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600553A RID: 21818 RVA: 0x001DDEC0 File Offset: 0x001DC0C0
		public bool GetInstanceUsagesForTypeCode(List<ArchetypeInstance> inputItems, List<ItemUsage> itemsUsedByOtherComponents, bool forceOneComponentPerStack, int typeCode, out List<ItemUsage> itemsUsed)
		{
			itemsUsed = RecipeComponent.m_itemsUsed;
			itemsUsed.Clear();
			int num = -1;
			foreach (ArchetypeInstance archetypeInstance in inputItems)
			{
				if (((archetypeInstance != null) ? archetypeInstance.ItemData : null) == null)
				{
					if (!RecipeComponent.m_nonItemErrorReported)
					{
						string format = "Non-item found during crafting checks. CharacterID: {0}, InstanceID: {1}";
						GameEntity gameEntity = LocalPlayer.GameEntity;
						UniqueId? uniqueId;
						if (gameEntity == null)
						{
							uniqueId = null;
						}
						else
						{
							CharacterData characterData = gameEntity.CharacterData;
							uniqueId = ((characterData != null) ? new UniqueId?(characterData.CharacterId) : null);
						}
						Debug.LogError(string.Format(format, uniqueId, (archetypeInstance != null) ? new UniqueId?(archetypeInstance.InstanceId) : null));
						RecipeComponent.m_nonItemErrorReported = true;
					}
				}
				else if (archetypeInstance.CombinedTypeCode == typeCode)
				{
					if (num == -1)
					{
						foreach (ComponentMaterial componentMaterial in this.AcceptableMaterials)
						{
							if (archetypeInstance.ArchetypeId == componentMaterial.Archetype.Id)
							{
								num = componentMaterial.AmountRequired;
								break;
							}
						}
					}
					if (!this.ShouldPreferAnotherStack(inputItems, itemsUsedByOtherComponents, archetypeInstance, num, forceOneComponentPerStack))
					{
						if (num > 0)
						{
							ItemInstanceData itemData = archetypeInstance.ItemData;
							int val = ((itemData != null) ? itemData.Count : null) ?? 1;
							int num2 = Math.Min(num, val);
							if (num2 > 0)
							{
								itemsUsed.Add(new ItemUsage
								{
									Instance = archetypeInstance,
									UsedFor = this,
									AmountUsed = num2
								});
								num -= num2;
							}
						}
						if (num == 0)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x0600553B RID: 21819 RVA: 0x001DE098 File Offset: 0x001DC298
		public void GetInstanceUsagesForAllMaterials(List<ArchetypeInstance> inputItems, List<ItemUsage> itemsUsedByOtherComponents, bool forceOneComponentPerStack, out List<ItemUsage> itemsUsed)
		{
			itemsUsed = RecipeComponent.m_itemsUsed;
			itemsUsed.Clear();
			foreach (ComponentMaterial componentMaterial in this.AcceptableMaterials)
			{
				int num = componentMaterial.AmountRequired;
				RecipeComponent.m_perDynamicArchetypeCounts.Clear();
				foreach (ArchetypeInstance archetypeInstance in inputItems)
				{
					if (((archetypeInstance != null) ? archetypeInstance.ItemData : null) == null)
					{
						if (!RecipeComponent.m_nonItemErrorReported)
						{
							string format = "Non-item found during crafting checks. CharacterID: {0}, InstanceID: {1}";
							GameEntity gameEntity = LocalPlayer.GameEntity;
							UniqueId? uniqueId;
							if (gameEntity == null)
							{
								uniqueId = null;
							}
							else
							{
								CharacterData characterData = gameEntity.CharacterData;
								uniqueId = ((characterData != null) ? new UniqueId?(characterData.CharacterId) : null);
							}
							Debug.LogError(string.Format(format, uniqueId, (archetypeInstance != null) ? new UniqueId?(archetypeInstance.InstanceId) : null));
							RecipeComponent.m_nonItemErrorReported = true;
						}
					}
					else
					{
						int treeFingerprint = archetypeInstance.ItemData.GetTreeFingerprint();
						if (treeFingerprint != 0 && !RecipeComponent.m_perDynamicArchetypeCounts.ContainsKey(treeFingerprint))
						{
							RecipeComponent.m_perDynamicArchetypeCounts.Add(treeFingerprint, componentMaterial.AmountRequired);
							num = componentMaterial.AmountRequired;
						}
						else if (treeFingerprint != 0)
						{
							num = RecipeComponent.m_perDynamicArchetypeCounts[treeFingerprint];
						}
						if (!this.ShouldPreferAnotherStack(inputItems, itemsUsedByOtherComponents, archetypeInstance, num, forceOneComponentPerStack) && archetypeInstance.Archetype.Id == componentMaterial.Archetype.Id && num > 0)
						{
							ItemInstanceData itemData = archetypeInstance.ItemData;
							int val = ((itemData != null) ? itemData.Count : null) ?? 1;
							int num2 = Math.Min(num, val);
							if (num2 > 0)
							{
								itemsUsed.Add(new ItemUsage
								{
									Instance = archetypeInstance,
									UsedFor = this,
									AmountUsed = num2
								});
								num -= num2;
								if (treeFingerprint != 0)
								{
									Dictionary<int, int> perDynamicArchetypeCounts = RecipeComponent.m_perDynamicArchetypeCounts;
									int key = treeFingerprint;
									perDynamicArchetypeCounts[key] -= num2;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600553C RID: 21820 RVA: 0x001DE2E0 File Offset: 0x001DC4E0
		private bool ShouldPreferAnotherStack(List<ArchetypeInstance> inputItems, List<ItemUsage> itemsUsedByOtherComponents, ArchetypeInstance currentItem, int remainingAmountRequired, bool forceOneComponentPerStack)
		{
			bool flag = false;
			int num = 0;
			if (itemsUsedByOtherComponents != null)
			{
				foreach (ItemUsage itemUsage in itemsUsedByOtherComponents)
				{
					flag = (flag || itemUsage.Instance.InstanceId == currentItem.InstanceId);
					if (itemUsage.Instance.InstanceId == currentItem.InstanceId)
					{
						num += itemUsage.AmountUsed;
					}
				}
			}
			ItemInstanceData itemData = currentItem.ItemData;
			bool flag2 = (((itemData != null) ? itemData.Count : null) ?? 1) - num >= remainingAmountRequired;
			bool flag3 = false;
			if (!forceOneComponentPerStack)
			{
				foreach (ArchetypeInstance archetypeInstance in inputItems)
				{
					flag3 = (flag3 || (archetypeInstance.InstanceId != currentItem.InstanceId && archetypeInstance.ArchetypeId == currentItem.ArchetypeId));
				}
			}
			return (flag && forceOneComponentPerStack) || (flag && !flag2 && flag3);
		}

		// Token: 0x0600553D RID: 21821 RVA: 0x00078E50 File Offset: 0x00077050
		public string CreateEditorElementName()
		{
			if (!this.m_enabled)
			{
				return this.m_displayName + " (Disabled)";
			}
			return this.m_displayName;
		}

		// Token: 0x0600553E RID: 21822 RVA: 0x00078E71 File Offset: 0x00077071
		private void RegenerateId()
		{
			this.m_id = UniqueId.GenerateFromGuid();
			Recipe recipe = Recipe.FindRecipeForComponentId(this.m_id);
			if (recipe == null)
			{
				return;
			}
			recipe.UpdateAllComponentSelectionLists(false);
		}

		// Token: 0x0600553F RID: 21823 RVA: 0x001DE430 File Offset: 0x001DC630
		private bool ValidateAcceptableMaterialsList(ComponentMaterial[] value, ref string errorMessage)
		{
			List<UniqueId> fromPool = StaticListPool<UniqueId>.GetFromPool();
			foreach (ComponentMaterial componentMaterial in value)
			{
				foreach (UniqueId b in fromPool)
				{
					if (componentMaterial.Archetype.Id == b)
					{
						StaticListPool<UniqueId>.ReturnToPool(fromPool);
						errorMessage = "Duplicate archetype found among acceptable materials!";
						return false;
					}
				}
				fromPool.Add(componentMaterial.Archetype.Id);
			}
			StaticListPool<UniqueId>.ReturnToPool(fromPool);
			return true;
		}

		// Token: 0x04004BB2 RID: 19378
		[SerializeField]
		private UniqueId m_id = UniqueId.Empty;

		// Token: 0x04004BB3 RID: 19379
		[SerializeField]
		private bool m_enabled = true;

		// Token: 0x04004BB4 RID: 19380
		[SerializeField]
		private string m_displayName;

		// Token: 0x04004BB5 RID: 19381
		[SerializeField]
		private string m_description;

		// Token: 0x04004BB6 RID: 19382
		[SerializeField]
		private ComponentMaterial[] m_acceptableMaterials;

		// Token: 0x04004BB7 RID: 19383
		[SerializeField]
		private ComponentRequirementType m_requirementType;

		// Token: 0x04004BB8 RID: 19384
		[SerializeField]
		private MaterialCategoryWithAmount[] m_materialCategoriesWithAmounts;

		// Token: 0x04004BB9 RID: 19385
		[NonSerialized]
		private ComponentMaterial[] m_allAcceptableMaterials;

		// Token: 0x04004BBA RID: 19386
		[NonSerialized]
		private static List<ItemUsage> m_itemsUsed = new List<ItemUsage>();

		// Token: 0x04004BBB RID: 19387
		private static Dictionary<int, int> m_perDynamicArchetypeCounts = new Dictionary<int, int>();

		// Token: 0x04004BBC RID: 19388
		[NonSerialized]
		private static bool m_nonItemErrorReported = false;
	}
}
