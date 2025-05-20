using System;
using SoL.Game.EffectSystem;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AB7 RID: 2743
	public static class WeaponTypesExtensions
	{
		// Token: 0x060054CC RID: 21708 RVA: 0x00078AE5 File Offset: 0x00076CE5
		public static bool RequiresFreeOffHand(this WeaponTypes type)
		{
			if (type <= WeaponTypes.Polearm)
			{
				if (type != WeaponTypes.Sword2H && type != WeaponTypes.Axe2H && type != WeaponTypes.Polearm)
				{
					return false;
				}
			}
			else if (type <= WeaponTypes.Hammer2H)
			{
				if (type != WeaponTypes.Mace2H && type != WeaponTypes.Hammer2H)
				{
					return false;
				}
			}
			else if (type != WeaponTypes.Spear && type != WeaponTypes.Staff2H)
			{
				return false;
			}
			return true;
		}

		// Token: 0x060054CD RID: 21709 RVA: 0x00078B1A File Offset: 0x00076D1A
		public static bool ShowOffHandAbilityDamage(this WeaponTypes type)
		{
			return type == WeaponTypes.Sword1H || type == WeaponTypes.Axe1H || type == WeaponTypes.Dagger;
		}

		// Token: 0x060054CE RID: 21710 RVA: 0x00078B2D File Offset: 0x00076D2D
		public static StatType GetWeaponDamageType(this WeaponTypes type)
		{
			if (type.IsOneHandedMelee())
			{
				return StatType.Damage1H;
			}
			if (type.IsTwoHandedMelee())
			{
				return StatType.Damage2H;
			}
			if (type.IsRanged())
			{
				return StatType.DamageRanged;
			}
			return StatType.None;
		}

		// Token: 0x060054CF RID: 21711 RVA: 0x001DB748 File Offset: 0x001D9948
		public static bool IsOneHandedMelee(this WeaponTypes type)
		{
			if (type <= WeaponTypes.Hammer1H)
			{
				if (type <= WeaponTypes.Axe1H)
				{
					if (type != WeaponTypes.Sword1H && type != WeaponTypes.Axe1H)
					{
						return false;
					}
				}
				else if (type != WeaponTypes.Mace1H && type != WeaponTypes.Hammer1H)
				{
					return false;
				}
			}
			else if (type <= WeaponTypes.Staff1H)
			{
				if (type - WeaponTypes.Dagger > 2 && type != WeaponTypes.Staff1H)
				{
					return false;
				}
			}
			else if (type != WeaponTypes.Shield && type != WeaponTypes.OffhandAccessory)
			{
				return false;
			}
			return true;
		}

		// Token: 0x060054D0 RID: 21712 RVA: 0x00078B51 File Offset: 0x00076D51
		public static bool IsTwoHandedMelee(this WeaponTypes type)
		{
			if (type <= WeaponTypes.Polearm)
			{
				if (type != WeaponTypes.Sword2H && type != WeaponTypes.Axe2H && type != WeaponTypes.Polearm)
				{
					return false;
				}
			}
			else if (type != WeaponTypes.Mace2H && type != WeaponTypes.Hammer2H && type != WeaponTypes.Staff2H)
			{
				return false;
			}
			return true;
		}

		// Token: 0x060054D1 RID: 21713 RVA: 0x00078B7A File Offset: 0x00076D7A
		public static bool IsRanged(this WeaponTypes type)
		{
			return type == WeaponTypes.Bow || type == WeaponTypes.Crossbow;
		}
	}
}
