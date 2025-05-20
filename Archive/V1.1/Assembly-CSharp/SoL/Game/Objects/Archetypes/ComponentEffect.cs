using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Text;
using Sirenix.OdinInspector;
using SoL.Game.Crafting;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A4F RID: 2639
	[Serializable]
	public class ComponentEffect
	{
		// Token: 0x17001245 RID: 4677
		// (get) Token: 0x060051A8 RID: 20904 RVA: 0x000767E4 File Offset: 0x000749E4
		public ComponentEffectCondition ConditionType
		{
			get
			{
				return this.m_conditionType;
			}
		}

		// Token: 0x17001246 RID: 4678
		// (get) Token: 0x060051A9 RID: 20905 RVA: 0x000767EC File Offset: 0x000749EC
		public OperationType OperationType
		{
			get
			{
				return this.m_operationType;
			}
		}

		// Token: 0x17001247 RID: 4679
		// (get) Token: 0x060051AA RID: 20906 RVA: 0x000767F4 File Offset: 0x000749F4
		public ComponentEffectsProfile ComponentEffectsProfile
		{
			get
			{
				return this.m_componentEffectsProfile;
			}
		}

		// Token: 0x17001248 RID: 4680
		// (get) Token: 0x060051AB RID: 20907 RVA: 0x000767FC File Offset: 0x000749FC
		public RangeOverride[] RangeOverrides
		{
			get
			{
				return this.m_rangeOverrides;
			}
		}

		// Token: 0x17001249 RID: 4681
		// (get) Token: 0x060051AC RID: 20908 RVA: 0x00076804 File Offset: 0x00074A04
		public ComponentEffectOperation[] Operations
		{
			get
			{
				if (!(this.m_operationsProfile != null))
				{
					return this.m_operations;
				}
				return this.m_operationsProfile.Operations;
			}
		}

		// Token: 0x1700124A RID: 4682
		// (get) Token: 0x060051AD RID: 20909 RVA: 0x00076826 File Offset: 0x00074A26
		public MinMaxFloatRange OutputRange
		{
			get
			{
				return this.m_outputRange;
			}
		}

		// Token: 0x1700124B RID: 4683
		// (get) Token: 0x060051AE RID: 20910 RVA: 0x0007682E File Offset: 0x00074A2E
		public ComponentParentage[] ComponentFilters
		{
			get
			{
				return this.m_componentFilters;
			}
		}

		// Token: 0x1700124C RID: 4684
		// (get) Token: 0x060051AF RID: 20911 RVA: 0x00076836 File Offset: 0x00074A36
		private bool m_condition_always
		{
			get
			{
				return this.m_conditionType == ComponentEffectCondition.Always;
			}
		}

		// Token: 0x1700124D RID: 4685
		// (get) Token: 0x060051B0 RID: 20912 RVA: 0x00076841 File Offset: 0x00074A41
		private bool m_condition_wasMadeFromAll
		{
			get
			{
				return this.m_conditionType == ComponentEffectCondition.WasMadeFromAll;
			}
		}

		// Token: 0x1700124E RID: 4686
		// (get) Token: 0x060051B1 RID: 20913 RVA: 0x0007684C File Offset: 0x00074A4C
		private bool m_condition_wasMadeFromAny
		{
			get
			{
				return this.m_conditionType == ComponentEffectCondition.WasMadeFromAny;
			}
		}

		// Token: 0x1700124F RID: 4687
		// (get) Token: 0x060051B2 RID: 20914 RVA: 0x00076857 File Offset: 0x00074A57
		private bool m_condition_wasMadeFromOnly
		{
			get
			{
				return this.m_conditionType == ComponentEffectCondition.WasMadeFromOnly;
			}
		}

		// Token: 0x17001250 RID: 4688
		// (get) Token: 0x060051B3 RID: 20915 RVA: 0x00076862 File Offset: 0x00074A62
		private bool m_condition_componentWasMadeFromAll
		{
			get
			{
				return this.m_conditionType == ComponentEffectCondition.ComponentWasMadeFromAll;
			}
		}

		// Token: 0x17001251 RID: 4689
		// (get) Token: 0x060051B4 RID: 20916 RVA: 0x0007686D File Offset: 0x00074A6D
		private bool m_condition_componentWasMadeFromAny
		{
			get
			{
				return this.m_conditionType == ComponentEffectCondition.ComponentWasMadeFromAny;
			}
		}

		// Token: 0x17001252 RID: 4690
		// (get) Token: 0x060051B5 RID: 20917 RVA: 0x00076878 File Offset: 0x00074A78
		private bool m_condition_componentWasMadeFromOnly
		{
			get
			{
				return this.m_conditionType == ComponentEffectCondition.ComponentWasMadeFromOnly;
			}
		}

		// Token: 0x17001253 RID: 4691
		// (get) Token: 0x060051B6 RID: 20918 RVA: 0x00076883 File Offset: 0x00074A83
		private bool m_condition_componentWasMadeFrom
		{
			get
			{
				return this.m_condition_componentWasMadeFromAll || this.m_condition_componentWasMadeFromAny || this.m_condition_componentWasMadeFromOnly;
			}
		}

		// Token: 0x17001254 RID: 4692
		// (get) Token: 0x060051B7 RID: 20919 RVA: 0x0007689D File Offset: 0x00074A9D
		private bool m_condition_wasMadeFrom
		{
			get
			{
				return this.m_condition_wasMadeFromAll || this.m_condition_wasMadeFromAny || this.m_condition_wasMadeFromOnly || this.m_condition_componentWasMadeFrom;
			}
		}

		// Token: 0x17001255 RID: 4693
		// (get) Token: 0x060051B8 RID: 20920 RVA: 0x000768BF File Offset: 0x00074ABF
		private bool m_condition_hasAttribute
		{
			get
			{
				return this.m_conditionType == ComponentEffectCondition.HasAttribute;
			}
		}

		// Token: 0x17001256 RID: 4694
		// (get) Token: 0x060051B9 RID: 20921 RVA: 0x000768CA File Offset: 0x00074ACA
		private bool m_condition_attributeIs
		{
			get
			{
				return this.m_conditionType == ComponentEffectCondition.AttributeIs;
			}
		}

		// Token: 0x17001257 RID: 4695
		// (get) Token: 0x060051BA RID: 20922 RVA: 0x000768D5 File Offset: 0x00074AD5
		private bool m_condition_attributeOps
		{
			get
			{
				return this.m_condition_hasAttribute || this.m_condition_attributeIs;
			}
		}

		// Token: 0x17001258 RID: 4696
		// (get) Token: 0x060051BB RID: 20923 RVA: 0x000768E7 File Offset: 0x00074AE7
		private bool m_opType_numeric
		{
			get
			{
				return this.m_operationType == OperationType.Numeric;
			}
		}

		// Token: 0x17001259 RID: 4697
		// (get) Token: 0x060051BC RID: 20924 RVA: 0x000768F2 File Offset: 0x00074AF2
		private bool m_opType_name
		{
			get
			{
				return this.m_operationType == OperationType.CustomName;
			}
		}

		// Token: 0x1700125A RID: 4698
		// (get) Token: 0x060051BD RID: 20925 RVA: 0x000768FD File Offset: 0x00074AFD
		private bool m_opType_prefix
		{
			get
			{
				return this.m_operationType == OperationType.CustomPrefix;
			}
		}

		// Token: 0x1700125B RID: 4699
		// (get) Token: 0x060051BE RID: 20926 RVA: 0x00076908 File Offset: 0x00074B08
		private bool m_opType_suffix
		{
			get
			{
				return this.m_operationType == OperationType.CustomSuffix;
			}
		}

		// Token: 0x1700125C RID: 4700
		// (get) Token: 0x060051BF RID: 20927 RVA: 0x00076913 File Offset: 0x00074B13
		private bool m_opType_modifyName
		{
			get
			{
				return this.m_opType_name || this.m_opType_prefix || this.m_opType_suffix;
			}
		}

		// Token: 0x1700125D RID: 4701
		// (get) Token: 0x060051C0 RID: 20928 RVA: 0x0007692D File Offset: 0x00074B2D
		private bool m_opType_icon
		{
			get
			{
				return this.m_operationType == OperationType.CustomIcon;
			}
		}

		// Token: 0x1700125E RID: 4702
		// (get) Token: 0x060051C1 RID: 20929 RVA: 0x00076938 File Offset: 0x00074B38
		private bool m_opType_iconTint
		{
			get
			{
				return this.m_operationType == OperationType.IconTint;
			}
		}

		// Token: 0x1700125F RID: 4703
		// (get) Token: 0x060051C2 RID: 20930 RVA: 0x00076943 File Offset: 0x00074B43
		private bool m_opType_description
		{
			get
			{
				return this.m_operationType == OperationType.CustomDescription;
			}
		}

		// Token: 0x17001260 RID: 4704
		// (get) Token: 0x060051C3 RID: 20931 RVA: 0x0007694E File Offset: 0x00074B4E
		private bool m_opType_profile
		{
			get
			{
				return this.m_operationType == OperationType.Profile;
			}
		}

		// Token: 0x17001261 RID: 4705
		// (get) Token: 0x060051C4 RID: 20932 RVA: 0x00076959 File Offset: 0x00074B59
		private bool m_opType_visualIndex
		{
			get
			{
				return this.m_operationType == OperationType.CustomVisualIndex;
			}
		}

		// Token: 0x17001262 RID: 4706
		// (get) Token: 0x060051C5 RID: 20933 RVA: 0x00076964 File Offset: 0x00074B64
		private bool m_opType_colorIndex
		{
			get
			{
				return this.m_operationType == OperationType.CustomColorIndex;
			}
		}

		// Token: 0x17001263 RID: 4707
		// (get) Token: 0x060051C6 RID: 20934 RVA: 0x00076970 File Offset: 0x00074B70
		private bool m_opType_itemCategory
		{
			get
			{
				return this.m_operationType == OperationType.ItemCategory;
			}
		}

		// Token: 0x17001264 RID: 4708
		// (get) Token: 0x060051C7 RID: 20935 RVA: 0x0007697C File Offset: 0x00074B7C
		private bool m_opType_addFlags
		{
			get
			{
				return this.m_operationType == OperationType.AddFlags;
			}
		}

		// Token: 0x17001265 RID: 4709
		// (get) Token: 0x060051C8 RID: 20936 RVA: 0x00076988 File Offset: 0x00074B88
		private bool m_opType_setFlags
		{
			get
			{
				return this.m_operationType == OperationType.SetFlags;
			}
		}

		// Token: 0x17001266 RID: 4710
		// (get) Token: 0x060051C9 RID: 20937 RVA: 0x00076994 File Offset: 0x00074B94
		private bool m_opType_overrideSetBonus
		{
			get
			{
				return this.m_operationType == OperationType.OverrideSetBonus;
			}
		}

		// Token: 0x17001267 RID: 4711
		// (get) Token: 0x060051CA RID: 20938 RVA: 0x000769A0 File Offset: 0x00074BA0
		private bool m_opType_SetRoleLevelRequirement
		{
			get
			{
				return this.m_operationType == OperationType.SetRoleLevelRequirement;
			}
		}

		// Token: 0x17001268 RID: 4712
		// (get) Token: 0x060051CB RID: 20939 RVA: 0x000769AC File Offset: 0x00074BAC
		private bool m_hasOperationsProfile
		{
			get
			{
				return this.m_operationsProfile != null;
			}
		}

		// Token: 0x17001269 RID: 4713
		// (get) Token: 0x060051CC RID: 20940 RVA: 0x000769BA File Offset: 0x00074BBA
		private bool m_allowOperationsProfileCreation
		{
			get
			{
				return this.m_operationsProfile == null && this.m_opType_numeric && this.m_operations.Length != 0;
			}
		}

		// Token: 0x1700126A RID: 4714
		// (get) Token: 0x060051CD RID: 20941 RVA: 0x000769DE File Offset: 0x00074BDE
		private bool m_canFilter
		{
			get
			{
				return this.FindProfile() == null;
			}
		}

		// Token: 0x1700126B RID: 4715
		// (get) Token: 0x060051CE RID: 20942 RVA: 0x000769EC File Offset: 0x00074BEC
		private bool m_showOperations
		{
			get
			{
				return this.m_opType_numeric && this.m_operationsProfile == null;
			}
		}

		// Token: 0x1700126C RID: 4716
		// (get) Token: 0x060051CF RID: 20943 RVA: 0x00076A04 File Offset: 0x00074C04
		private IEnumerable m_items
		{
			get
			{
				return this.GetMaterialDropdownItems(this.FindArchetype());
			}
		}

		// Token: 0x1700126D RID: 4717
		// (get) Token: 0x060051D0 RID: 20944 RVA: 0x00076A12 File Offset: 0x00074C12
		private IEnumerable m_componentMaterials
		{
			get
			{
				return this.GetComponentMaterialDropdownItems(this.FindArchetype(), "", null);
			}
		}

		// Token: 0x1700126E RID: 4718
		// (get) Token: 0x060051D1 RID: 20945 RVA: 0x00076A26 File Offset: 0x00074C26
		private IEnumerable m_components
		{
			get
			{
				return this.GetComponentDropdownItems(this.FindArchetype(), "", null);
			}
		}

		// Token: 0x1700126F RID: 4719
		// (get) Token: 0x060051D2 RID: 20946 RVA: 0x00076A3A File Offset: 0x00074C3A
		private IEnumerable m_itemStats
		{
			get
			{
				return this.GetAssignerNames();
			}
		}

		// Token: 0x17001270 RID: 4720
		// (get) Token: 0x060051D3 RID: 20947 RVA: 0x00076A42 File Offset: 0x00074C42
		private IEnumerable m_rawComponentAttributes
		{
			get
			{
				return this.GetAttributeNames();
			}
		}

		// Token: 0x17001271 RID: 4721
		// (get) Token: 0x060051D4 RID: 20948 RVA: 0x00076A4A File Offset: 0x00074C4A
		private IEnumerable m_itemCategories
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<ItemCategory>();
			}
		}

		// Token: 0x17001272 RID: 4722
		// (get) Token: 0x060051D5 RID: 20949 RVA: 0x001D1074 File Offset: 0x001CF274
		private string m_outputRange_infoBox
		{
			get
			{
				return string.Format("Output Stat Maximum range: {0} to {1}", ComponentEffectAssigners.GetRange(this.m_outputStat).Min, ComponentEffectAssigners.GetRange(this.m_outputStat).Max);
			}
		}

		// Token: 0x060051D6 RID: 20950 RVA: 0x001D10BC File Offset: 0x001CF2BC
		private IEnumerable GetAssignerNames()
		{
			IEnumerable<ComponentEffectAssignerName> allAssignerNames = ComponentEffectAssigners.GetAllAssignerNames();
			if (this.FindArchetype() == null || !this.m_opType_numeric)
			{
				return allAssignerNames;
			}
			List<ComponentEffectAssignerName> list = new List<ComponentEffectAssignerName>();
			foreach (ComponentEffectAssignerName componentEffectAssignerName in allAssignerNames)
			{
				if (this.m_archetype.IsAssignerHandled(componentEffectAssignerName))
				{
					list.Add(componentEffectAssignerName);
				}
			}
			return list;
		}

		// Token: 0x060051D7 RID: 20951 RVA: 0x001D1138 File Offset: 0x001CF338
		private ItemArchetype FindArchetype()
		{
			if (this.FindProfile() != null)
			{
				return null;
			}
			if (this.m_archetype != null)
			{
				return this.m_archetype;
			}
			foreach (BaseArchetype baseArchetype in InternalGameDatabase.Archetypes.GetAllItems())
			{
				ItemArchetype itemArchetype = baseArchetype as ItemArchetype;
				if (itemArchetype != null && itemArchetype.HasComponentEffects)
				{
					ComponentEffect[] componentEffects = itemArchetype.ComponentEffects;
					for (int i = 0; i < componentEffects.Length; i++)
					{
						if (componentEffects[i] == this)
						{
							return this.m_archetype = itemArchetype;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x060051D8 RID: 20952 RVA: 0x00049FFA File Offset: 0x000481FA
		private ComponentEffectsProfile FindProfile()
		{
			return null;
		}

		// Token: 0x060051D9 RID: 20953 RVA: 0x001D11EC File Offset: 0x001CF3EC
		public void ResetAuthoringAwareness()
		{
			this.m_archetype = null;
			this.m_parentProfile = null;
			this.m_hasLookedForProfile = false;
			if (this.m_operations != null)
			{
				ComponentEffectOperation[] operations = this.m_operations;
				for (int i = 0; i < operations.Length; i++)
				{
					operations[i].ResetAuthoringAwareness();
				}
			}
		}

		// Token: 0x060051DA RID: 20954 RVA: 0x001D1234 File Offset: 0x001CF434
		private IEnumerable GetMaterialDropdownItems(ItemArchetype item)
		{
			if (item == null || !this.m_condition_wasMadeFrom)
			{
				return SolOdinUtilities.GetDropdownItems<ItemArchetype>();
			}
			List<ItemArchetype> items = StaticListPool<ItemArchetype>.GetFromPool();
			item.GetAllItemsInTree(items);
			IEnumerable dropdownItems = SolOdinUtilities.GetDropdownItems<ItemArchetype>((ItemArchetype x) => items.Contains(x));
			StaticListPool<ItemArchetype>.ReturnToPool(items);
			return dropdownItems;
		}

		// Token: 0x060051DB RID: 20955 RVA: 0x001D1294 File Offset: 0x001CF494
		private IEnumerable<IValueDropdownItem> GetComponentMaterialDropdownItems(ItemArchetype archetype, string rootPath = "", Stack<UniqueId> parentage = null)
		{
			if (!this.m_condition_componentWasMadeFrom)
			{
				return Array.Empty<IValueDropdownItem>();
			}
			if (this.m_componenMaterialDropdownItems != null)
			{
				return this.m_componenMaterialDropdownItems;
			}
			if (archetype == null)
			{
				return Array.Empty<IValueDropdownItem>();
			}
			if (!string.IsNullOrEmpty(rootPath))
			{
				rootPath += "/";
			}
			if (parentage == null)
			{
				parentage = new Stack<UniqueId>();
			}
			if (parentage.Count > 20)
			{
				return Array.Empty<IValueDropdownItem>();
			}
			List<IValueDropdownItem> list = new List<IValueDropdownItem>();
			List<Recipe> list2 = archetype.FindRecipesThatProduceThisItem();
			foreach (Recipe recipe in list2)
			{
				foreach (RecipeComponent recipeComponent in recipe.Components)
				{
					if (recipeComponent != null)
					{
						foreach (ComponentMaterial componentMaterial in recipeComponent.AcceptableMaterials)
						{
							if (componentMaterial != null && !parentage.Contains(componentMaterial.Archetype.Id))
							{
								string text = string.Concat(new string[]
								{
									rootPath,
									componentMaterial.Archetype.name,
									" (",
									recipe.DisplayName,
									")"
								});
								parentage.Push(componentMaterial.Archetype.Id);
								list.Add(new ValueDropdownItem(text, new Parentage
								{
									ArchetypeIdList = parentage.Reverse<UniqueId>().ToArray<UniqueId>()
								}));
								parentage.Pop();
							}
						}
					}
				}
			}
			StaticListPool<Recipe>.ReturnToPool(list2);
			if (this.m_componenMaterialDropdownItems == null && parentage.Count == 0)
			{
				this.m_componenMaterialDropdownItems = list;
			}
			return list;
		}

		// Token: 0x060051DC RID: 20956 RVA: 0x001D1460 File Offset: 0x001CF660
		private IEnumerable<IValueDropdownItem> GetComponentDropdownItems(ItemArchetype archetype, string rootPath = "", Stack<ComponentInfo> parentage = null)
		{
			if (archetype == null || !this.m_canFilter)
			{
				return Array.Empty<IValueDropdownItem>();
			}
			if (this.m_componenDropdownItems != null)
			{
				return this.m_componenDropdownItems;
			}
			if (!string.IsNullOrEmpty(rootPath))
			{
				rootPath += "/";
			}
			if (parentage == null)
			{
				parentage = new Stack<ComponentInfo>();
			}
			if (parentage.Count > 20)
			{
				return Array.Empty<IValueDropdownItem>();
			}
			List<IValueDropdownItem> list = new List<IValueDropdownItem>();
			List<Recipe> list2 = archetype.FindRecipesThatProduceThisItem();
			foreach (Recipe recipe in list2)
			{
				foreach (RecipeComponent recipeComponent in recipe.Components)
				{
					if (recipeComponent != null)
					{
						string text = string.Concat(new string[]
						{
							rootPath,
							recipeComponent.DisplayName,
							" (",
							recipe.DisplayName,
							")"
						});
						parentage.Push(new ComponentInfo
						{
							RecipeId = recipe.Id,
							ComponentId = recipeComponent.Id
						});
						list.Add(new ValueDropdownItem(text, new ComponentParentage
						{
							ComponentList = parentage.Reverse<ComponentInfo>().ToArray<ComponentInfo>()
						}));
						foreach (ComponentMaterial componentMaterial in recipeComponent.AcceptableMaterials)
						{
						}
						parentage.Pop();
					}
				}
			}
			StaticListPool<Recipe>.ReturnToPool(list2);
			if (this.m_componenDropdownItems == null && parentage.Count == 0)
			{
				this.m_componenDropdownItems = list;
			}
			return list;
		}

		// Token: 0x060051DD RID: 20957 RVA: 0x001D1608 File Offset: 0x001CF808
		private void OnComponentChanged()
		{
			if (this.m_wasMadeFrom_component != null)
			{
				Debug.Log("Component changed to: " + string.Join(", ", from x in this.m_wasMadeFrom_component.ArchetypeIdList
				select x.Value));
			}
		}

		// Token: 0x060051DE RID: 20958 RVA: 0x001D1668 File Offset: 0x001CF868
		private IEnumerable<IValueDropdownItem> GetAttributeNames()
		{
			if (!this.m_condition_attributeOps)
			{
				return Array.Empty<IValueDropdownItem>();
			}
			List<IValueDropdownItem> list = new List<IValueDropdownItem>(ItemAttributes.AttributeCount);
			foreach (ItemAttributes.Names names in ItemAttributes.AttributeNames)
			{
				list.Add(new ValueDropdownItem(names.ToString(), names));
			}
			return list;
		}

		// Token: 0x060051DF RID: 20959 RVA: 0x001D16EC File Offset: 0x001CF8EC
		public string CreateEditorElementName()
		{
			switch (this.m_operationType)
			{
			case OperationType.Numeric:
			{
				string text = (this.m_outputStat != ComponentEffectAssignerName.None) ? this.m_outputStat.ToString() : "???";
				string text2 = "???";
				if (this.Operations != null && this.Operations.Length != 0)
				{
					if (this.Operations[0].Type != ComponentEffectOperationType.Value)
					{
						text2 = this.Operations[0].Attribute.ToString();
					}
					else
					{
						text2 = "an arbitrary value";
					}
				}
				string text3 = string.Empty;
				switch (this.m_outputType)
				{
				case ComponentEffectOutputType.Add:
					text3 = "Add to";
					break;
				case ComponentEffectOutputType.Subtract:
					text3 = "Subtract from";
					break;
				case ComponentEffectOutputType.Multiply:
					text3 = "Multiply with";
					break;
				case ComponentEffectOutputType.Divide:
					text3 = "Divide";
					break;
				default:
					text3 = "Set";
					break;
				}
				return string.Concat(new string[]
				{
					text3,
					" ",
					text,
					" based on ",
					text2
				});
			}
			case OperationType.CustomName:
			{
				string str = string.IsNullOrEmpty(this.m_name) ? "???" : this.m_name;
				return "Set name to \"" + str + "\"";
			}
			case OperationType.CustomPrefix:
			{
				string str2 = string.IsNullOrEmpty(this.m_name) ? "???" : this.m_name;
				return "Add prefix \"" + str2 + "\" to name";
			}
			case OperationType.CustomSuffix:
			{
				string str3 = string.IsNullOrEmpty(this.m_name) ? "???" : this.m_name;
				return "Add suffix \"" + str3 + "\" to name";
			}
			case OperationType.CustomIcon:
				return "Set icon";
			case OperationType.IconTint:
				return "Set icon tint";
			case OperationType.CustomDescription:
			{
				string text4 = string.IsNullOrEmpty(this.m_description) ? "???" : this.m_description;
				if (text4.Length <= 20)
				{
					return "Set description to \"" + text4.Substring(0, text4.Length) + "\"";
				}
				return "Set description to \"" + text4.Substring(0, 20) + "...\"";
			}
			case OperationType.Profile:
			{
				string str4 = (this.m_componentEffectsProfile == null) ? "???" : this.m_componentEffectsProfile.name;
				return "Profile: " + str4;
			}
			case OperationType.CustomVisualIndex:
				return string.Format("INSTANCE - Set visual index to {0}", this.m_visualIndex);
			case OperationType.CustomColorIndex:
				return string.Format("INSTANCE - Set color index to {0}", this.m_colorIndex);
			case OperationType.ItemCategory:
			{
				string str5 = (this.m_itemCategory == null) ? "???" : this.m_itemCategory.Description;
				return "Set Item Category to " + str5;
			}
			case OperationType.AddFlags:
				return string.Format("INSTANCE - Add Item Flags: {0}", this.m_itemFlags);
			case OperationType.SetFlags:
				return string.Format("INSTANCE - Set Item Flags: {0}", this.m_itemSetFlags);
			case OperationType.OverrideSetBonus:
				return ZString.Format<string>("Override Set Bonus with {0}", this.m_setBonusOverride ? this.m_setBonusOverride.DisplayName : "???");
			case OperationType.SetRoleLevelRequirement:
				return "Set Role/Level Requirement";
			default:
				return "Effect";
			}
		}

		// Token: 0x060051E0 RID: 20960 RVA: 0x001D1A0C File Offset: 0x001CFC0C
		public bool WasMadeFromIsValidForItem(ItemArchetype item)
		{
			if (this.m_wasMadeFrom == null || this.m_wasMadeFrom.Length == 0)
			{
				return true;
			}
			foreach (ItemArchetype item2 in this.m_wasMadeFrom)
			{
				if (!item.CanBeMadeFrom(item2))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060051E1 RID: 20961 RVA: 0x00076A51 File Offset: 0x00074C51
		public bool WasMadeFromParentageIsValidForItem(ItemArchetype item)
		{
			return this.m_wasMadeFrom_component.ArchetypeIdList == null || this.m_wasMadeFrom_component.ArchetypeIdList.Length == 0 || item.IsValidComponentParentage(this.m_wasMadeFrom_component.ArchetypeIdList, 0);
		}

		// Token: 0x060051E2 RID: 20962 RVA: 0x00076A82 File Offset: 0x00074C82
		private IEnumerable GetComponentEffectsProfile()
		{
			return SolOdinUtilities.GetDropdownItems<ComponentEffectsProfile>();
		}

		// Token: 0x060051E3 RID: 20963 RVA: 0x001D1A54 File Offset: 0x001CFC54
		public bool ShouldAffectItem(ItemArchetype archetype, ArchetypeInstance instance, ComponentParentage[] componentFiltersOverride = null)
		{
			ComponentParentage[] componentFilters = componentFiltersOverride ?? this.m_componentFilters;
			switch (this.m_conditionType)
			{
			case ComponentEffectCondition.Always:
				return true;
			case ComponentEffectCondition.WasMadeFromAll:
				return archetype.WasMadeFromAll(instance, this.m_wasMadeFrom, componentFilters);
			case ComponentEffectCondition.WasMadeFromAny:
				return archetype.WasMadeFromAny(instance, this.m_wasMadeFrom, componentFilters);
			case ComponentEffectCondition.WasMadeFromOnly:
				return archetype.WasMadeFromOnly(instance, this.m_wasMadeFrom, componentFilters);
			case ComponentEffectCondition.ComponentWasMadeFromAll:
				return archetype.ComponentWasMadeFromAll(instance, this.m_wasMadeFrom_component.ArchetypeIdList, this.m_wasMadeFrom);
			case ComponentEffectCondition.ComponentWasMadeFromAny:
				return archetype.ComponentWasMadeFromAny(instance, this.m_wasMadeFrom_component.ArchetypeIdList, this.m_wasMadeFrom);
			case ComponentEffectCondition.ComponentWasMadeFromOnly:
				return archetype.ComponentWasMadeFromOnly(instance, this.m_wasMadeFrom_component.ArchetypeIdList, this.m_wasMadeFrom);
			case ComponentEffectCondition.HasAttribute:
				return archetype.IsAttributeInUtilizedComponents(instance, this.m_conditionAttribute, componentFilters);
			case ComponentEffectCondition.AttributeIs:
			{
				float[] attributeValuesFromUtilizedComponents = archetype.GetAttributeValuesFromUtilizedComponents(instance, this.m_conditionAttribute, componentFilters);
				if (attributeValuesFromUtilizedComponents == null)
				{
					return false;
				}
				switch (this.m_is)
				{
				case NumericComparator.EqualTo:
				{
					float[] array = attributeValuesFromUtilizedComponents;
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] != (float)this.m_conditionValue)
						{
							return false;
						}
					}
					return true;
				}
				case NumericComparator.GreaterThan:
				{
					float[] array = attributeValuesFromUtilizedComponents;
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] <= (float)this.m_conditionValue)
						{
							return false;
						}
					}
					return true;
				}
				case NumericComparator.GreaterThanOrEqualTo:
				{
					float[] array = attributeValuesFromUtilizedComponents;
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] < (float)this.m_conditionValue)
						{
							return false;
						}
					}
					return true;
				}
				case NumericComparator.LessThan:
				{
					float[] array = attributeValuesFromUtilizedComponents;
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] >= (float)this.m_conditionValue)
						{
							return false;
						}
					}
					return true;
				}
				case NumericComparator.LessThanOrEqualTo:
				{
					float[] array = attributeValuesFromUtilizedComponents;
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] > (float)this.m_conditionValue)
						{
							return false;
						}
					}
					return true;
				}
				default:
					return false;
				}
				break;
			}
			default:
				return false;
			}
		}

		// Token: 0x060051E4 RID: 20964 RVA: 0x001D1C34 File Offset: 0x001CFE34
		public void AffectItem(ItemArchetype archetype, ArchetypeInstance instance, RangeOverride[] rangeOverrides, ComponentParentage[] componentFiltersOverride = null)
		{
			ComponentParentage[] array = componentFiltersOverride ?? this.m_componentFilters;
			if (this.ShouldAffectItem(archetype, instance, array))
			{
				switch (this.m_operationType)
				{
				case OperationType.Numeric:
				{
					if (this.Operations == null || this.Operations.Length == 0 || this.m_outputStat == ComponentEffectAssignerName.None)
					{
						return;
					}
					ItemAttributes.Names attribute = this.Operations[0].Attribute;
					float[] array2;
					if (this.Operations[0].Type != ComponentEffectOperationType.Value)
					{
						array2 = archetype.GetAttributeValuesFromUtilizedComponents(instance, attribute, array);
						if (array2 == null)
						{
							return;
						}
					}
					else
					{
						this.m_tempSingleValueArray[0] = 0f;
						array2 = this.m_tempSingleValueArray;
					}
					ComponentEffectOperation[] operations = this.Operations;
					for (int i = 0; i < operations.Length; i++)
					{
						operations[i].Perform(array2, instance, array);
					}
					MinMaxFloatRange value = this.m_outputRange;
					if (rangeOverrides != null && this.Operations[0].Type != ComponentEffectOperationType.Value)
					{
						foreach (RangeOverride rangeOverride in rangeOverrides)
						{
							if (rangeOverride.Enabled && rangeOverride.Input == attribute && rangeOverride.Output == this.m_outputStat)
							{
								value = rangeOverride.Range;
								break;
							}
						}
					}
					if (!archetype.PopulateDynamicValue(this.m_outputStat, array2[0], this.m_outputType, value.IsZero ? null : new MinMaxFloatRange?(value)))
					{
						Debug.LogError(string.Format("Failed to assign value to {0}!", this.m_outputStat));
						return;
					}
					break;
				}
				case OperationType.CustomName:
					archetype.SetCustomName(this.m_name);
					return;
				case OperationType.CustomPrefix:
					archetype.SetCustomPrefix(this.m_name);
					return;
				case OperationType.CustomSuffix:
					archetype.SetCustomSuffix(this.m_name);
					return;
				case OperationType.CustomIcon:
					archetype.SetCustomIcon(this.m_icon);
					return;
				case OperationType.IconTint:
					archetype.SetCustomTint(this.m_iconTint);
					return;
				case OperationType.CustomDescription:
					archetype.SetCustomDescription(this.m_description);
					return;
				case OperationType.Profile:
				{
					ComponentEffectsProfile componentEffectsProfile = this.m_componentEffectsProfile;
					if (componentEffectsProfile == null)
					{
						return;
					}
					componentEffectsProfile.AffectItem(archetype, instance, rangeOverrides, array);
					return;
				}
				case OperationType.CustomVisualIndex:
					if (((instance != null) ? instance.ItemData : null) != null)
					{
						instance.ItemData.VisualIndex = new byte?(this.m_visualIndex);
						return;
					}
					break;
				case OperationType.CustomColorIndex:
					if (((instance != null) ? instance.ItemData : null) != null)
					{
						instance.ItemData.ColorIndex = new byte?(this.m_colorIndex);
						return;
					}
					break;
				case OperationType.ItemCategory:
					archetype.SetItemCategory(this.m_itemCategory);
					return;
				case OperationType.AddFlags:
					if (((instance != null) ? instance.ItemData : null) != null)
					{
						instance.ItemData.ItemFlags |= this.m_itemFlags;
						return;
					}
					break;
				case OperationType.SetFlags:
					if (((instance != null) ? instance.ItemData : null) != null)
					{
						instance.ItemData.ItemFlags = this.m_itemSetFlags;
						return;
					}
					break;
				case OperationType.OverrideSetBonus:
				{
					EquipableItem equipableItem = archetype as EquipableItem;
					if (equipableItem != null)
					{
						equipableItem.OverrideSetBonus(this.m_setBonusZoneId, this.m_setBonusOverride);
						return;
					}
					break;
				}
				case OperationType.SetRoleLevelRequirement:
				{
					EquipableItem equipableItem2 = archetype as EquipableItem;
					if (equipableItem2 != null)
					{
						equipableItem2.SetRoleLevelRequirement(this.m_roleLevelRequirement);
						return;
					}
					break;
				}
				default:
					throw new NotImplementedException(string.Format("ComponentEffect.AffectItem doesn't have a case defined for operation type {0}", this.m_operationType));
				}
			}
		}

		// Token: 0x040048ED RID: 18669
		private const string kGroup_Condition = "Condition";

		// Token: 0x040048EE RID: 18670
		private const string kGroup_ComponentWasMadeFrom = "Condition/ComponentWasMadeFrom";

		// Token: 0x040048EF RID: 18671
		private const string kGroup_AttributeComparison = "Condition/AttributeComparison";

		// Token: 0x040048F0 RID: 18672
		private const string kGroup_Operation = "Operation";

		// Token: 0x040048F1 RID: 18673
		private const string kGroup_ComponentFilter = "Component Filter";

		// Token: 0x040048F2 RID: 18674
		[SerializeField]
		private ComponentEffectCondition m_conditionType;

		// Token: 0x040048F3 RID: 18675
		[SerializeField]
		private Parentage m_wasMadeFrom_component;

		// Token: 0x040048F4 RID: 18676
		[SerializeField]
		private ItemArchetype[] m_wasMadeFrom;

		// Token: 0x040048F5 RID: 18677
		[SerializeField]
		private ItemAttributes.Names m_conditionAttribute = ItemAttributes.Names.Hardness;

		// Token: 0x040048F6 RID: 18678
		[SerializeField]
		private NumericComparator m_is;

		// Token: 0x040048F7 RID: 18679
		[Range(0f, 1000f)]
		[SerializeField]
		private int m_conditionValue;

		// Token: 0x040048F8 RID: 18680
		[SerializeField]
		private OperationType m_operationType;

		// Token: 0x040048F9 RID: 18681
		[SerializeField]
		private ComponentEffectsProfile m_componentEffectsProfile;

		// Token: 0x040048FA RID: 18682
		[SerializeField]
		private RangeOverride[] m_rangeOverrides;

		// Token: 0x040048FB RID: 18683
		[SerializeField]
		private ComponentEffectOperationsProfile m_operationsProfile;

		// Token: 0x040048FC RID: 18684
		[SerializeField]
		private ComponentEffectOperation[] m_operations;

		// Token: 0x040048FD RID: 18685
		[SerializeField]
		private ComponentEffectOutputType m_outputType;

		// Token: 0x040048FE RID: 18686
		[SerializeField]
		private ComponentEffectAssignerName m_outputStat;

		// Token: 0x040048FF RID: 18687
		[SerializeField]
		private MinMaxFloatRange m_outputRange = new MinMaxFloatRange(0f, 0f);

		// Token: 0x04004900 RID: 18688
		[SerializeField]
		private string m_name;

		// Token: 0x04004901 RID: 18689
		[TextArea]
		[SerializeField]
		private string m_description;

		// Token: 0x04004902 RID: 18690
		[SerializeField]
		private Sprite m_icon;

		// Token: 0x04004903 RID: 18691
		[SerializeField]
		private Color m_iconTint = Color.white;

		// Token: 0x04004904 RID: 18692
		[SerializeField]
		private byte m_visualIndex;

		// Token: 0x04004905 RID: 18693
		[SerializeField]
		private byte m_colorIndex;

		// Token: 0x04004906 RID: 18694
		[SerializeField]
		private ItemCategory m_itemCategory;

		// Token: 0x04004907 RID: 18695
		[SerializeField]
		private ItemFlags m_itemFlags;

		// Token: 0x04004908 RID: 18696
		[SerializeField]
		private ItemFlags m_itemSetFlags;

		// Token: 0x04004909 RID: 18697
		[SerializeField]
		private SetBonusProfile m_setBonusOverride;

		// Token: 0x0400490A RID: 18698
		[Tooltip("If this is set to ZoneId.None then SetBonusOverride is applied as the base Set Bonus")]
		[SerializeField]
		private ZoneId m_setBonusZoneId;

		// Token: 0x0400490B RID: 18699
		[SerializeField]
		private RoleLevelRequirement m_roleLevelRequirement;

		// Token: 0x0400490C RID: 18700
		[SerializeField]
		private ComponentParentage[] m_componentFilters;

		// Token: 0x0400490D RID: 18701
		[HideInInspector]
		[NonSerialized]
		private ItemArchetype m_archetype;

		// Token: 0x0400490E RID: 18702
		private ComponentEffectsProfile m_parentProfile;

		// Token: 0x0400490F RID: 18703
		private bool m_hasLookedForProfile;

		// Token: 0x04004910 RID: 18704
		private IEnumerable<IValueDropdownItem> m_componenMaterialDropdownItems;

		// Token: 0x04004911 RID: 18705
		private IEnumerable<IValueDropdownItem> m_componenDropdownItems;

		// Token: 0x04004912 RID: 18706
		private float[] m_tempSingleValueArray = new float[1];
	}
}
