using System;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A90 RID: 2704
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Ranged Weapon")]
	public class RangedWeaponItem : WeaponItem
	{
		// Token: 0x1700131B RID: 4891
		// (get) Token: 0x060053B4 RID: 21428 RVA: 0x000580DD File Offset: 0x000562DD
		public override EquipmentType Type
		{
			get
			{
				return EquipmentType.Weapon_Ranged;
			}
		}

		// Token: 0x1700131C RID: 4892
		// (get) Token: 0x060053B5 RID: 21429 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool RequiresAmmo
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700131D RID: 4893
		// (get) Token: 0x060053B6 RID: 21430 RVA: 0x00077D92 File Offset: 0x00075F92
		public override RangedAmmoType RequiredAmmoType
		{
			get
			{
				return this.m_ammoType;
			}
		}

		// Token: 0x1700131E RID: 4894
		// (get) Token: 0x060053B7 RID: 21431 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool ShowGroundGizmos
		{
			get
			{
				return true;
			}
		}

		// Token: 0x04004A9E RID: 19102
		[SerializeField]
		private RangedAmmoType m_ammoType;
	}
}
