using System;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A8C RID: 2700
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Melee Weapon")]
	public class MeleeWeaponItem : WeaponItem
	{
		// Token: 0x1700130E RID: 4878
		// (get) Token: 0x0600539C RID: 21404 RVA: 0x00077CCD File Offset: 0x00075ECD
		public override EquipmentType Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x1700130F RID: 4879
		// (get) Token: 0x0600539D RID: 21405 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool ShowGroundGizmos
		{
			get
			{
				return true;
			}
		}

		// Token: 0x04004A94 RID: 19092
		[SerializeField]
		private EquipmentType m_type;

		// Token: 0x04004A95 RID: 19093
		private static EquipmentType[] kValidTypes = new EquipmentType[]
		{
			EquipmentType.Weapon_Melee_1H,
			EquipmentType.Weapon_Melee_2H
		};
	}
}
