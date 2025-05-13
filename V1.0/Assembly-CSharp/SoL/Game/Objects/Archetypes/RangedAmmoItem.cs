using System;
using Cysharp.Text;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Containers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A8E RID: 2702
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Ranged Ammo")]
	public class RangedAmmoItem : EquipableItem, IStackable, IHandheldItem
	{
		// Token: 0x17001313 RID: 4883
		// (get) Token: 0x060053A5 RID: 21413 RVA: 0x000707F0 File Offset: 0x0006E9F0
		public override EquipmentType Type
		{
			get
			{
				return EquipmentType.Ammo_Ranged;
			}
		}

		// Token: 0x17001314 RID: 4884
		// (get) Token: 0x060053A6 RID: 21414 RVA: 0x00077D0B File Offset: 0x00075F0B
		public int MaxStackCount
		{
			get
			{
				return this.m_maxStackCount;
			}
		}

		// Token: 0x17001315 RID: 4885
		// (get) Token: 0x060053A7 RID: 21415 RVA: 0x00077D13 File Offset: 0x00075F13
		public RangedAmmoType AmmoType
		{
			get
			{
				return this.m_ammoType;
			}
		}

		// Token: 0x17001316 RID: 4886
		// (get) Token: 0x060053A8 RID: 21416 RVA: 0x00077D1B File Offset: 0x00075F1B
		public DamageType DamageType
		{
			get
			{
				return this.m_damageType;
			}
		}

		// Token: 0x17001317 RID: 4887
		// (get) Token: 0x060053A9 RID: 21417 RVA: 0x00077D23 File Offset: 0x00075F23
		public VitalMods Mods
		{
			get
			{
				return this.m_mods;
			}
		}

		// Token: 0x060053AA RID: 21418 RVA: 0x00077D2B File Offset: 0x00075F2B
		public override EquipmentSlot GetTargetEquipmentSlot(GameEntity entity)
		{
			if (!entity.CharacterData.OffHand_SecondaryActive)
			{
				return EquipmentSlot.PrimaryWeapon_OffHand;
			}
			return EquipmentSlot.SecondaryWeapon_OffHand;
		}

		// Token: 0x060053AB RID: 21419 RVA: 0x001D85F8 File Offset: 0x001D67F8
		public override void AddWeaponDataToTooltip(ArchetypeTooltip tooltip, bool isAutoAttack = false)
		{
			base.AddWeaponDataToTooltip(tooltip, false);
			string modifiersLine = this.m_mods.GetModifiersLine(null);
			if (!string.IsNullOrEmpty(modifiersLine))
			{
				tooltip.CombatBlock.AppendLine(ZString.Format<string, string>("Weapon {0} {1}", "Dmg", modifiersLine), 0);
			}
			this.m_mods.FillCombatFlagsLine(tooltip.CombatBlock);
		}

		// Token: 0x17001318 RID: 4888
		// (get) Token: 0x060053AC RID: 21420 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IHandheldItem.RequiresFreeOffHand
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001319 RID: 4889
		// (get) Token: 0x060053AD RID: 21421 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IHandheldItem.AlternateAnimationSet
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700131A RID: 4890
		// (get) Token: 0x060053AE RID: 21422 RVA: 0x00077D3D File Offset: 0x00075F3D
		HandheldItemFlags IHandheldItem.HandheldItemFlag
		{
			get
			{
				return HandheldItemFlags.Ammo;
			}
		}

		// Token: 0x060053AF RID: 21423 RVA: 0x00077D44 File Offset: 0x00075F44
		public override bool IsAssignerHandled(ComponentEffectAssignerName assignerName)
		{
			return assignerName == ComponentEffectAssignerName.MaxStackCount || base.IsAssignerHandled(assignerName);
		}

		// Token: 0x060053B0 RID: 21424 RVA: 0x00077D53 File Offset: 0x00075F53
		public override bool PopulateDynamicValue(ComponentEffectAssignerName assignerName, float value, ComponentEffectOutputType type, MinMaxFloatRange? rangeOverride)
		{
			if (assignerName == ComponentEffectAssignerName.MaxStackCount)
			{
				this.m_maxStackCount = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_maxStackCount);
				return true;
			}
			return base.PopulateDynamicValue(assignerName, value, type, rangeOverride);
		}

		// Token: 0x060053B1 RID: 21425 RVA: 0x001D8650 File Offset: 0x001D6850
		public bool EntityHasMatchingAmmoEquipped(GameEntity entity, out ArchetypeInstance matchingAmmoInstance)
		{
			matchingAmmoInstance = null;
			if (!entity || entity.CollectionController == null || entity.CollectionController.Equipment == null || entity.CharacterData == null)
			{
				return false;
			}
			ContainerInstance equipment = entity.CollectionController.Equipment;
			EquipmentSlot index = EquipmentSlot.PrimaryWeapon_OffHand;
			EquipmentSlot index2 = EquipmentSlot.SecondaryWeapon_OffHand;
			if (entity.CharacterData.OffHand_SecondaryActive)
			{
				index = EquipmentSlot.SecondaryWeapon_OffHand;
				index2 = EquipmentSlot.PrimaryWeapon_OffHand;
			}
			ArchetypeInstance archetypeInstance;
			ArchetypeInstance archetypeInstance2;
			if (equipment.TryGetInstanceForIndex((int)index, out archetypeInstance) && archetypeInstance.ArchetypeId == base.Id)
			{
				matchingAmmoInstance = archetypeInstance;
			}
			else if (equipment.TryGetInstanceForIndex((int)index2, out archetypeInstance2) && archetypeInstance2.ArchetypeId == base.Id)
			{
				matchingAmmoInstance = archetypeInstance2;
			}
			return matchingAmmoInstance != null;
		}

		// Token: 0x060053B2 RID: 21426 RVA: 0x001D86FC File Offset: 0x001D68FC
		public bool EntityHasEmptyAmmoSlotForWeapon(GameEntity entity, out EquipmentSlot targetSlot)
		{
			targetSlot = EquipmentSlot.None;
			if (!entity || entity.CollectionController == null || entity.CollectionController.Equipment == null || entity.CharacterData == null)
			{
				return false;
			}
			ContainerInstance equipment = entity.CollectionController.Equipment;
			EquipmentSlot index = EquipmentSlot.PrimaryWeapon_MainHand;
			EquipmentSlot equipmentSlot = EquipmentSlot.PrimaryWeapon_OffHand;
			EquipmentSlot index2 = EquipmentSlot.SecondaryWeapon_MainHand;
			EquipmentSlot equipmentSlot2 = EquipmentSlot.SecondaryWeapon_OffHand;
			if (entity.CharacterData.OffHand_SecondaryActive)
			{
				index = EquipmentSlot.SecondaryWeapon_MainHand;
				equipmentSlot = EquipmentSlot.SecondaryWeapon_OffHand;
				index2 = EquipmentSlot.PrimaryWeapon_MainHand;
				equipmentSlot2 = EquipmentSlot.PrimaryWeapon_OffHand;
			}
			ArchetypeInstance archetypeInstance;
			RangedWeaponItem rangedWeaponItem;
			ArchetypeInstance archetypeInstance2;
			if (equipment.TryGetInstanceForIndex((int)index, out archetypeInstance) && archetypeInstance.Archetype.TryGetAsType(out rangedWeaponItem) && rangedWeaponItem.RequiredAmmoType == this.AmmoType && !equipment.TryGetInstanceForIndex((int)equipmentSlot, out archetypeInstance2))
			{
				targetSlot = equipmentSlot;
				return true;
			}
			ArchetypeInstance archetypeInstance3;
			ArchetypeInstance archetypeInstance4;
			if (equipment.TryGetInstanceForIndex((int)index2, out archetypeInstance3) && archetypeInstance3.Archetype.TryGetAsType(out rangedWeaponItem) && rangedWeaponItem.RequiredAmmoType == this.AmmoType && !equipment.TryGetInstanceForIndex((int)equipmentSlot2, out archetypeInstance4))
			{
				targetSlot = equipmentSlot2;
				return true;
			}
			return false;
		}

		// Token: 0x04004A96 RID: 19094
		[SerializeField]
		private int m_maxStackCount = 1;

		// Token: 0x04004A97 RID: 19095
		[SerializeField]
		private RangedAmmoType m_ammoType;

		// Token: 0x04004A98 RID: 19096
		[SerializeField]
		private DamageType m_damageType = DamageType.Ranged_Piercing;

		// Token: 0x04004A99 RID: 19097
		[SerializeField]
		private VitalMods m_mods;
	}
}
