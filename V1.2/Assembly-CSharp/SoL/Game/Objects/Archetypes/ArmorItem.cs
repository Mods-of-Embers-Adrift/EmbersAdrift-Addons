using System;
using System.Collections;
using Cysharp.Text;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using UMA.CharacterSystem;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A45 RID: 2629
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Armor")]
	public class ArmorItem : EquipableItem, IArmorClass, IDurability
	{
		// Token: 0x1700122D RID: 4653
		// (get) Token: 0x06005170 RID: 20848 RVA: 0x000765E4 File Offset: 0x000747E4
		public override EquipmentType Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x1700122E RID: 4654
		// (get) Token: 0x06005171 RID: 20849 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool AllowHandheldItems
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700122F RID: 4655
		// (get) Token: 0x06005172 RID: 20850 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool IsAugmentable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17001230 RID: 4656
		// (get) Token: 0x06005173 RID: 20851 RVA: 0x000765EC File Offset: 0x000747EC
		private bool ShowVisualsSingle
		{
			get
			{
				return this.m_visualsType == EquipableItem.VisualsType.Single;
			}
		}

		// Token: 0x17001231 RID: 4657
		// (get) Token: 0x06005174 RID: 20852 RVA: 0x000765F7 File Offset: 0x000747F7
		private bool ShowVisualsArray
		{
			get
			{
				return this.m_visualsType > EquipableItem.VisualsType.Single;
			}
		}

		// Token: 0x17001232 RID: 4658
		// (get) Token: 0x06005175 RID: 20853 RVA: 0x00076602 File Offset: 0x00074802
		private bool m_showArmorWeight
		{
			get
			{
				return this.m_type.HasArmorCost();
			}
		}

		// Token: 0x17001233 RID: 4659
		// (get) Token: 0x06005176 RID: 20854 RVA: 0x0007660F File Offset: 0x0007480F
		private bool m_showArmorWeightOverrideValue
		{
			get
			{
				return this.m_showArmorWeight && this.m_overrideArmorWeight;
			}
		}

		// Token: 0x17001234 RID: 4660
		// (get) Token: 0x06005177 RID: 20855 RVA: 0x00076621 File Offset: 0x00074821
		private bool m_showArmorWeightInterpolator
		{
			get
			{
				return this.m_showArmorWeight && !this.m_overrideArmorWeight;
			}
		}

		// Token: 0x06005178 RID: 20856 RVA: 0x001D0960 File Offset: 0x001CEB60
		private string GetArmorWeightDetails()
		{
			string empty = string.Empty;
			if (!this.m_type.HasArmorCost())
			{
				return empty;
			}
			MinMaxIntRange rangeForSlot = GlobalSettings.Values.Armor.GetRangeForSlot(this.m_type);
			int armorWeight = GlobalSettings.Values.Armor.GetArmorWeight(this.m_type, this.m_armorWeightInterpolator);
			return string.Concat(new string[]
			{
				"EquipmentType ",
				this.m_type.ToString(),
				" has an Armor Weight of ",
				rangeForSlot.Min.ToString(),
				" to ",
				rangeForSlot.Max.ToString(),
				"\nTotal Weight: ",
				armorWeight.ToString()
			});
		}

		// Token: 0x06005179 RID: 20857 RVA: 0x00076636 File Offset: 0x00074836
		private string GetDetails()
		{
			return "Armor Weight for Slot";
		}

		// Token: 0x0600517A RID: 20858 RVA: 0x0007663D File Offset: 0x0007483D
		private int GetArmorCost()
		{
			if (!this.m_type.HasArmorCost())
			{
				return 0;
			}
			if (!this.m_overrideArmorWeight)
			{
				return GlobalSettings.Values.Armor.GetArmorWeight(this.m_type, this.m_armorWeightInterpolator);
			}
			return this.m_overrideArmorWeightValue;
		}

		// Token: 0x17001235 RID: 4661
		// (get) Token: 0x0600517B RID: 20859 RVA: 0x00076602 File Offset: 0x00074802
		private bool m_showArmorCostMultiplier
		{
			get
			{
				return this.m_type.HasArmorCost();
			}
		}

		// Token: 0x17001236 RID: 4662
		// (get) Token: 0x0600517C RID: 20860 RVA: 0x00076678 File Offset: 0x00074878
		private bool m_showDualRecipePairs
		{
			get
			{
				return this.m_type.HasLeftRightRecipe();
			}
		}

		// Token: 0x0600517D RID: 20861 RVA: 0x00076685 File Offset: 0x00074885
		private string GetLeftRecipePairName()
		{
			if (!this.m_showDualRecipePairs)
			{
				return "Recipe Pair";
			}
			return "Left Recipe Pair";
		}

		// Token: 0x17001237 RID: 4663
		// (get) Token: 0x0600517E RID: 20862 RVA: 0x0004F9FB File Offset: 0x0004DBFB
		private IEnumerable GetColorValues
		{
			get
			{
				return SolOdinUtilities.GetColorValues();
			}
		}

		// Token: 0x0600517F RID: 20863 RVA: 0x001D0A24 File Offset: 0x001CEC24
		public override string GetModifiedDisplayName(ArchetypeInstance instance)
		{
			if ((this.m_visualsType == EquipableItem.VisualsType.Single && !this.m_colorOverride.UsesIndex) || instance == null || instance.ItemData == null || (instance.ItemData.VisualIndex == null && instance.ItemData.ColorIndex == null))
			{
				return base.GetModifiedDisplayName(instance);
			}
			string result = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				if (this.m_colorOverride != null && this.m_colorOverride.UsesIndex)
				{
					string colorName = this.m_colorOverride.GetColorName(instance.ItemData.ColorIndex);
					if (!string.IsNullOrEmpty(colorName))
					{
						utf16ValueStringBuilder.AppendFormat<string>("{0} ", colorName);
					}
				}
				utf16ValueStringBuilder.Append(base.GetModifiedDisplayName(instance));
				if (this.m_visualsType != EquipableItem.VisualsType.Single && instance.ItemData.VisualIndex != null && this.m_randomVisuals != null && this.m_randomVisuals.Length != 0)
				{
					ArmorItem.ArmorItemVisuals armorItemVisuals = ((int)instance.ItemData.VisualIndex.Value < this.m_randomVisuals.Length) ? this.m_randomVisuals[(int)instance.ItemData.VisualIndex.Value] : this.m_randomVisuals[0];
					if (armorItemVisuals != null && !string.IsNullOrEmpty(armorItemVisuals.Description))
					{
						utf16ValueStringBuilder.AppendFormat<string>(" {0}", armorItemVisuals.Description);
					}
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06005180 RID: 20864 RVA: 0x001D0B94 File Offset: 0x001CED94
		public override void OnInstanceCreated(ArchetypeInstance instance)
		{
			base.OnInstanceCreated(instance);
			instance.ItemData.Durability = new ItemDamage();
			if (this.m_visualsType == EquipableItem.VisualsType.Random && this.m_randomVisuals != null)
			{
				instance.ItemData.VisualIndex = new byte?((byte)UnityEngine.Random.Range(0, this.m_randomVisuals.Length));
			}
			if (this.m_colorOverride != null && this.m_colorOverride.IsRandom && this.m_colorOverride.IndexCount > 0)
			{
				instance.ItemData.ColorIndex = new byte?((byte)UnityEngine.Random.Range(0, this.m_colorOverride.IndexCount));
			}
		}

		// Token: 0x06005181 RID: 20865 RVA: 0x001D0C30 File Offset: 0x001CEE30
		public override void ResetSlotColor(CharacterSex sex, DynamicCharacterAvatar dca, int index, byte? visualIndex, bool refresh = true)
		{
			if (GameManager.IsServer || !this.Type.IsVisible())
			{
				return;
			}
			WardrobeRecipePair wardrobeRecipePairBySlotIndex = this.GetWardrobeRecipePairBySlotIndex(index, visualIndex);
			if (wardrobeRecipePairBySlotIndex)
			{
				wardrobeRecipePairBySlotIndex.ResetColor(sex, dca, refresh);
			}
		}

		// Token: 0x06005182 RID: 20866 RVA: 0x001D0C70 File Offset: 0x001CEE70
		public override void OnEquipVisuals(CharacterSex sex, DynamicCharacterAvatar dca, int index, byte? visualIndex, byte? colorIndex, bool refresh = true)
		{
			if (GameManager.IsServer || !this.Type.IsVisible())
			{
				return;
			}
			WardrobeRecipePair wardrobeRecipePairBySlotIndex = this.GetWardrobeRecipePairBySlotIndex(index, visualIndex);
			if (wardrobeRecipePairBySlotIndex)
			{
				wardrobeRecipePairBySlotIndex.OnEquipVisuals(sex, dca, refresh, this.m_colorOverride.GetColorOverride(colorIndex));
			}
		}

		// Token: 0x06005183 RID: 20867 RVA: 0x001D0CBC File Offset: 0x001CEEBC
		public override void OnUnequipVisuals(CharacterSex sex, DynamicCharacterAvatar dca, int index, byte? visualIndex, byte? colorIndex, bool refresh = true)
		{
			if (GameManager.IsServer || !this.Type.IsVisible())
			{
				return;
			}
			WardrobeRecipePair wardrobeRecipePairBySlotIndex = this.GetWardrobeRecipePairBySlotIndex(index, visualIndex);
			if (wardrobeRecipePairBySlotIndex)
			{
				wardrobeRecipePairBySlotIndex.OnUnequipVisuals(sex, dca, refresh);
			}
		}

		// Token: 0x06005184 RID: 20868 RVA: 0x001D0CFC File Offset: 0x001CEEFC
		public override EquipmentSlot GetTargetEquipmentSlot(GameEntity entity)
		{
			EquipmentSlot equipmentSlot = EquipmentSlot.None;
			foreach (EquipmentSlot equipmentSlot2 in this.m_type.GetCachedCompatibleSlots())
			{
				if (equipmentSlot == EquipmentSlot.None)
				{
					if (equipmentSlot2.IsLeftVariant() || equipmentSlot2.IsRightVariant())
					{
						equipmentSlot = (ClientGameManager.InputManager.HoldingShift ? equipmentSlot2.GetRightSlotVariant() : equipmentSlot2.GetLeftSlotVariant());
					}
					else
					{
						equipmentSlot = equipmentSlot2;
					}
				}
				ArchetypeInstance archetypeInstance;
				if (!entity.CollectionController.Equipment.TryGetInstanceForIndex((int)equipmentSlot2, out archetypeInstance))
				{
					return equipmentSlot2;
				}
			}
			return equipmentSlot;
		}

		// Token: 0x06005185 RID: 20869 RVA: 0x001D0D9C File Offset: 0x001CEF9C
		private WardrobeRecipePair GetWardrobeRecipePairBySlotIndex(int index, byte? visualIndex)
		{
			ArmorItem.ArmorItemVisuals armorItemVisuals = this.m_primaryVisuals;
			if (visualIndex != null && this.m_randomVisuals != null && this.m_randomVisuals.Length != 0)
			{
				armorItemVisuals = (((int)visualIndex.Value < this.m_randomVisuals.Length) ? this.m_randomVisuals[(int)visualIndex.Value] : this.m_randomVisuals[0]);
			}
			WardrobeRecipePair result = armorItemVisuals.RecipePair;
			EquipmentSlot equipmentSlot;
			if (this.m_type.HasLeftRightRecipe() && EquipmentExtensions.IndexToSlotDict.TryGetValue(index, out equipmentSlot) && equipmentSlot.IsRightVariant())
			{
				result = armorItemVisuals.RightRecipePair;
			}
			return result;
		}

		// Token: 0x06005186 RID: 20870 RVA: 0x001D0E28 File Offset: 0x001CF028
		public override bool TryGetSalePrice(ArchetypeInstance instance, out ulong value)
		{
			if (!base.TryGetSalePrice(instance, out value))
			{
				return false;
			}
			float f = value * this.GetCurrentDurability((float)instance.ItemData.Durability.Absorbed);
			value = (ulong)((long)Mathf.FloorToInt(f));
			return true;
		}

		// Token: 0x06005187 RID: 20871 RVA: 0x0007669A File Offset: 0x0007489A
		private float GetCurrentDurability(float dmgAbsorbed)
		{
			return 1f - dmgAbsorbed / (float)this.m_maxDamageAbsorption;
		}

		// Token: 0x17001238 RID: 4664
		// (get) Token: 0x06005188 RID: 20872 RVA: 0x000766AB File Offset: 0x000748AB
		EquipmentType IArmorClass.Type
		{
			get
			{
				return this.Type;
			}
		}

		// Token: 0x17001239 RID: 4665
		// (get) Token: 0x06005189 RID: 20873 RVA: 0x000766B3 File Offset: 0x000748B3
		int IArmorClass.BaseArmorClass
		{
			get
			{
				return this.m_baseArmorClass;
			}
		}

		// Token: 0x1700123A RID: 4666
		// (get) Token: 0x0600518A RID: 20874 RVA: 0x000766BB File Offset: 0x000748BB
		int IArmorClass.MaxDamageAbsorption
		{
			get
			{
				return this.m_maxDamageAbsorption;
			}
		}

		// Token: 0x1700123B RID: 4667
		// (get) Token: 0x0600518B RID: 20875 RVA: 0x000766C3 File Offset: 0x000748C3
		int IArmorClass.ArmorCost
		{
			get
			{
				return this.GetArmorCost();
			}
		}

		// Token: 0x0600518C RID: 20876 RVA: 0x000766CB File Offset: 0x000748CB
		int IArmorClass.GetCurrentArmorClass(float damageAbsorbed)
		{
			return this.GetArmorClass(damageAbsorbed);
		}

		// Token: 0x0600518D RID: 20877 RVA: 0x000766D4 File Offset: 0x000748D4
		float IArmorClass.GetCurrentDurability(float damageAbsorbed)
		{
			return this.GetCurrentDurability(damageAbsorbed);
		}

		// Token: 0x1700123C RID: 4668
		// (get) Token: 0x0600518E RID: 20878 RVA: 0x000766BB File Offset: 0x000748BB
		int IDurability.MaxDamageAbsorption
		{
			get
			{
				return this.m_maxDamageAbsorption;
			}
		}

		// Token: 0x0600518F RID: 20879 RVA: 0x000766D4 File Offset: 0x000748D4
		float IDurability.GetCurrentDurability(float dmgAbsorbed)
		{
			return this.GetCurrentDurability(dmgAbsorbed);
		}

		// Token: 0x1700123D RID: 4669
		// (get) Token: 0x06005190 RID: 20880 RVA: 0x0004479C File Offset: 0x0004299C
		bool IDurability.DegradeOnHit
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06005191 RID: 20881 RVA: 0x001D0E68 File Offset: 0x001CF068
		protected override void AddArmorCostToTooltip(ArchetypeTooltip tooltip)
		{
			base.AddArmorCostToTooltip(tooltip);
			if (this.m_type.HasArmorCost())
			{
				int armorCost = this.GetArmorCost();
				if (armorCost > 0)
				{
					tooltip.DataBlock.AppendLine("Armor Weight:", armorCost.ToString());
				}
			}
		}

		// Token: 0x06005192 RID: 20882 RVA: 0x000766DD File Offset: 0x000748DD
		private IEnumerable GetWardrobeRecipePair()
		{
			return SolOdinUtilities.GetDropdownItems<WardrobeRecipePair>();
		}

		// Token: 0x06005193 RID: 20883 RVA: 0x000766E4 File Offset: 0x000748E4
		public override bool IsAssignerHandled(ComponentEffectAssignerName assignerName)
		{
			return assignerName - ComponentEffectAssignerName.BaseArmorClass <= 1 || assignerName == ComponentEffectAssignerName.ArmorCostMultiplier || assignerName - ComponentEffectAssignerName.ArmorWeightOverride <= 1 || base.IsAssignerHandled(assignerName);
		}

		// Token: 0x06005194 RID: 20884 RVA: 0x001D0EAC File Offset: 0x001CF0AC
		public override bool PopulateDynamicValue(ComponentEffectAssignerName assignerName, float value, ComponentEffectOutputType type, MinMaxFloatRange? rangeOverride)
		{
			if (assignerName <= ComponentEffectAssignerName.MaxDamageAbsorption)
			{
				if (assignerName == ComponentEffectAssignerName.BaseArmorClass)
				{
					this.m_baseArmorClass = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_baseArmorClass);
					return true;
				}
				if (assignerName == ComponentEffectAssignerName.MaxDamageAbsorption)
				{
					this.m_maxDamageAbsorption = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_maxDamageAbsorption);
					return true;
				}
			}
			else
			{
				if (assignerName == ComponentEffectAssignerName.ArmorCostMultiplier)
				{
					this.m_armorCostMultiplier = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_armorCostMultiplier);
					return true;
				}
				if (assignerName == ComponentEffectAssignerName.ArmorWeightOverride)
				{
					this.m_overrideArmorWeight = true;
					this.m_overrideArmorWeightValue = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_overrideArmorWeightValue);
					this.m_armorWeightInterpolator = 0;
					return true;
				}
				if (assignerName == ComponentEffectAssignerName.ArmorWeightInterpolator)
				{
					this.m_overrideArmorWeight = false;
					this.m_overrideArmorWeightValue = 0;
					this.m_armorWeightInterpolator = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_armorWeightInterpolator);
					return true;
				}
			}
			return base.PopulateDynamicValue(assignerName, value, type, rangeOverride);
		}

		// Token: 0x040048AF RID: 18607
		public const string kDurabilityGroupName = "Durability";

		// Token: 0x040048B0 RID: 18608
		private const string kVisualColorGroup = "Visuals/CustomColor";

		// Token: 0x040048B1 RID: 18609
		[SerializeField]
		private EquipmentType m_type;

		// Token: 0x040048B2 RID: 18610
		[SerializeField]
		private ArmorItem.ArmorItemVisuals m_primaryVisuals;

		// Token: 0x040048B3 RID: 18611
		[SerializeField]
		private ArmorItem.ArmorItemVisuals[] m_randomVisuals;

		// Token: 0x040048B4 RID: 18612
		[SerializeField]
		private ColorOverride m_colorOverride;

		// Token: 0x040048B5 RID: 18613
		[SerializeField]
		private int m_baseArmorClass;

		// Token: 0x040048B6 RID: 18614
		[SerializeField]
		private int m_maxDamageAbsorption = 1000;

		// Token: 0x040048B7 RID: 18615
		[SerializeField]
		private float m_armorCostMultiplier = 1f;

		// Token: 0x040048B8 RID: 18616
		private const string kArmorWeight = "Armor Weight";

		// Token: 0x040048B9 RID: 18617
		private const string kArmorWeightOverride = "Armor Weight/Override";

		// Token: 0x040048BA RID: 18618
		[SerializeField]
		private DummyClass m_armorWeightDummy1;

		// Token: 0x040048BB RID: 18619
		[SerializeField]
		private bool m_overrideArmorWeight;

		// Token: 0x040048BC RID: 18620
		[SerializeField]
		private int m_overrideArmorWeightValue;

		// Token: 0x040048BD RID: 18621
		[Range(0f, 100f)]
		[SerializeField]
		private int m_armorWeightInterpolator;

		// Token: 0x040048BE RID: 18622
		[SerializeField]
		private DummyClass m_armorWeightDummy2;

		// Token: 0x040048BF RID: 18623
		private static EquipmentType[] kValidTypes = new EquipmentType[]
		{
			EquipmentType.Head,
			EquipmentType.Mask,
			EquipmentType.Back,
			EquipmentType.Waist,
			EquipmentType.Clothing_Chest,
			EquipmentType.Clothing_Hands,
			EquipmentType.Clothing_Legs,
			EquipmentType.Clothing_Feet,
			EquipmentType.Armor_Shoulders,
			EquipmentType.Armor_Chest,
			EquipmentType.Armor_Hands,
			EquipmentType.Armor_Legs,
			EquipmentType.Armor_Feet
		};

		// Token: 0x02000A46 RID: 2630
		[Serializable]
		private class ArmorItemVisuals
		{
			// Token: 0x1700123E RID: 4670
			// (get) Token: 0x06005197 RID: 20887 RVA: 0x00076738 File Offset: 0x00074938
			// (set) Token: 0x06005198 RID: 20888 RVA: 0x00076740 File Offset: 0x00074940
			public ArmorItem ArmorItem { get; set; }

			// Token: 0x1700123F RID: 4671
			// (get) Token: 0x06005199 RID: 20889 RVA: 0x00076749 File Offset: 0x00074949
			public WardrobeRecipePair RecipePair
			{
				get
				{
					return this.m_recipePair;
				}
			}

			// Token: 0x17001240 RID: 4672
			// (get) Token: 0x0600519A RID: 20890 RVA: 0x00076751 File Offset: 0x00074951
			public WardrobeRecipePair RightRecipePair
			{
				get
				{
					return this.m_rightRecipePair;
				}
			}

			// Token: 0x17001241 RID: 4673
			// (get) Token: 0x0600519B RID: 20891 RVA: 0x00076759 File Offset: 0x00074959
			public string Description
			{
				get
				{
					return this.m_description;
				}
			}

			// Token: 0x0600519C RID: 20892 RVA: 0x00076761 File Offset: 0x00074961
			public ArmorItemVisuals(WardrobeRecipePair left, WardrobeRecipePair right)
			{
				this.m_recipePair = left;
				this.m_rightRecipePair = right;
			}

			// Token: 0x0600519D RID: 20893 RVA: 0x000766DD File Offset: 0x000748DD
			private IEnumerable GetWardrobeRecipePair()
			{
				return SolOdinUtilities.GetDropdownItems<WardrobeRecipePair>();
			}

			// Token: 0x17001242 RID: 4674
			// (get) Token: 0x0600519E RID: 20894 RVA: 0x00076777 File Offset: 0x00074977
			private bool ShowDualRecipePairs
			{
				get
				{
					return this.ArmorItem != null && this.ArmorItem.Type.HasLeftRightRecipe();
				}
			}

			// Token: 0x17001243 RID: 4675
			// (get) Token: 0x0600519F RID: 20895 RVA: 0x00076799 File Offset: 0x00074999
			private bool ShowDescription
			{
				get
				{
					return this.ArmorItem != null && this != this.ArmorItem.m_primaryVisuals;
				}
			}

			// Token: 0x060051A0 RID: 20896 RVA: 0x000767BC File Offset: 0x000749BC
			private string GetLeftRecipePairName()
			{
				if (!this.ShowDualRecipePairs)
				{
					return "Recipe Pair";
				}
				return "Left Recipe Pair";
			}

			// Token: 0x040048C1 RID: 18625
			[SerializeField]
			private string m_description;

			// Token: 0x040048C2 RID: 18626
			[SerializeField]
			private WardrobeRecipePair m_recipePair;

			// Token: 0x040048C3 RID: 18627
			[SerializeField]
			private WardrobeRecipePair m_rightRecipePair;
		}
	}
}
