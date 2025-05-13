using System;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Containers;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AA4 RID: 2724
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Runic Battery")]
	public class RunicBattery : EquipableItem, IHandheldItem, IDamageSource
	{
		// Token: 0x17001352 RID: 4946
		// (get) Token: 0x06005431 RID: 21553 RVA: 0x000447AE File Offset: 0x000429AE
		public override EquipmentType Type
		{
			get
			{
				return EquipmentType.EmberStone;
			}
		}

		// Token: 0x17001353 RID: 4947
		// (get) Token: 0x06005432 RID: 21554 RVA: 0x0007852E File Offset: 0x0007672E
		public RuneMasteryArchetype Mastery
		{
			get
			{
				return this.m_mastery;
			}
		}

		// Token: 0x06005433 RID: 21555 RVA: 0x001DA2BC File Offset: 0x001D84BC
		public override EquipmentSlot GetTargetEquipmentSlot(GameEntity entity)
		{
			EquipmentSlot equipmentSlot = entity.CharacterData.OffHand_SecondaryActive ? EquipmentSlot.SecondaryWeapon_OffHand : EquipmentSlot.PrimaryWeapon_OffHand;
			ContainerInstance containerInstance;
			ArchetypeInstance archetypeInstance;
			if (!entity.CollectionController.TryGetInstance(ContainerType.Equipment, out containerInstance) || !containerInstance.TryGetInstanceForIndex((int)equipmentSlot, out archetypeInstance))
			{
				return equipmentSlot;
			}
			EquipmentSlot equipmentSlot2 = entity.CharacterData.MainHand_SecondaryActive ? EquipmentSlot.SecondaryWeapon_MainHand : EquipmentSlot.PrimaryWeapon_MainHand;
			ArchetypeInstance archetypeInstance2;
			if (containerInstance.TryGetInstanceForIndex((int)equipmentSlot2, out archetypeInstance2))
			{
				return equipmentSlot;
			}
			return equipmentSlot2;
		}

		// Token: 0x06005434 RID: 21556 RVA: 0x001DA318 File Offset: 0x001D8518
		public override void AddWeaponDataToTooltip(ArchetypeTooltip tooltip, bool isAutoAttack = false)
		{
			base.AddWeaponDataToTooltip(tooltip, false);
			TooltipTextBlock requirementsBlock = tooltip.RequirementsBlock;
			if (this.Mastery != null)
			{
				requirementsBlock.AppendLine("Mastery: " + this.Mastery.DisplayName, 0);
			}
			if (this.RequiresFreeOffHand)
			{
				requirementsBlock.AppendLine("Empty off hand", 0);
			}
		}

		// Token: 0x17001354 RID: 4948
		// (get) Token: 0x06005435 RID: 21557 RVA: 0x00061BE2 File Offset: 0x0005FDE2
		DamageType IDamageSource.Type
		{
			get
			{
				return DamageType.Elemental_Air;
			}
		}

		// Token: 0x17001355 RID: 4949
		// (get) Token: 0x06005436 RID: 21558 RVA: 0x00078536 File Offset: 0x00076736
		public bool RequiresFreeOffHand
		{
			get
			{
				return this.m_requireFreeOffHand;
			}
		}

		// Token: 0x17001356 RID: 4950
		// (get) Token: 0x06005437 RID: 21559 RVA: 0x00045BCA File Offset: 0x00043DCA
		public bool AlternateAnimationSet
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001357 RID: 4951
		// (get) Token: 0x06005438 RID: 21560 RVA: 0x0007853E File Offset: 0x0007673E
		HandheldItemFlags IHandheldItem.HandheldItemFlag
		{
			get
			{
				return HandheldItemFlags.EmberStone;
			}
		}

		// Token: 0x04004B02 RID: 19202
		[SerializeField]
		private RuneMasteryArchetype m_mastery;

		// Token: 0x04004B03 RID: 19203
		[SerializeField]
		private bool m_requireFreeOffHand;
	}
}
