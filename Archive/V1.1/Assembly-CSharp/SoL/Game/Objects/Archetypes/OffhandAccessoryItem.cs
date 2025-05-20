using System;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A8D RID: 2701
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Offhand Accessory")]
	public class OffhandAccessoryItem : WeaponItem
	{
		// Token: 0x17001310 RID: 4880
		// (get) Token: 0x060053A0 RID: 21408 RVA: 0x0006ADA7 File Offset: 0x00068FA7
		public override EquipmentType Type
		{
			get
			{
				return EquipmentType.Weapon_OffhandAccessory;
			}
		}

		// Token: 0x17001311 RID: 4881
		// (get) Token: 0x060053A1 RID: 21409 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool m_showWeaponDataOnTooltip
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001312 RID: 4882
		// (get) Token: 0x060053A2 RID: 21410 RVA: 0x00077CF2 File Offset: 0x00075EF2
		protected override HandheldItemFlags m_handheldItemFlags
		{
			get
			{
				return HandheldItemFlags.Accessory;
			}
		}

		// Token: 0x060053A3 RID: 21411 RVA: 0x00077CF9 File Offset: 0x00075EF9
		public override EquipmentSlot GetTargetEquipmentSlot(GameEntity entity)
		{
			if (!entity.CharacterData.MainHand_SecondaryActive)
			{
				return EquipmentSlot.PrimaryWeapon_OffHand;
			}
			return EquipmentSlot.SecondaryWeapon_OffHand;
		}
	}
}
