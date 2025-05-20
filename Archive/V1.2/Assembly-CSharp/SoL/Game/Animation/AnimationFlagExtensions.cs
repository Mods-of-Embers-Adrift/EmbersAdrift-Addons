using System;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Archetypes;

namespace SoL.Game.Animation
{
	// Token: 0x02000D73 RID: 3443
	public static class AnimationFlagExtensions
	{
		// Token: 0x060067B1 RID: 26545 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this AnimationFlags a, AnimationFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x060067B2 RID: 26546 RVA: 0x00213378 File Offset: 0x00211578
		public static AnimationFlags GetAnimationFlagsForHandHeldItems(this IHandHeldItems handHeldItems, bool includeOffhand)
		{
			AnimationFlags animationFlags = AnimationFlags.None;
			if (handHeldItems.MainHand != null && handHeldItems.MainHand.WeaponItem)
			{
				animationFlags |= AnimationFlagExtensions.GetFlagsForWeaponType(handHeldItems.MainHand.WeaponItem.GetWeaponType(), true);
			}
			if (includeOffhand && handHeldItems.OffHand != null && handHeldItems.OffHand.WeaponItem)
			{
				animationFlags |= AnimationFlagExtensions.GetFlagsForWeaponType(handHeldItems.OffHand.WeaponItem.GetWeaponType(), false);
			}
			return animationFlags;
		}

		// Token: 0x060067B3 RID: 26547 RVA: 0x00085B67 File Offset: 0x00083D67
		private static AnimationFlags GetFlagsForWeaponType(WeaponTypes weaponType, bool mainHand)
		{
			if (weaponType <= WeaponTypes.Mace1H)
			{
				if (weaponType != WeaponTypes.Sword1H && weaponType != WeaponTypes.Axe1H && weaponType != WeaponTypes.Mace1H)
				{
					return AnimationFlags.None;
				}
			}
			else if (weaponType != WeaponTypes.Hammer1H)
			{
				if (weaponType - WeaponTypes.Dagger > 2)
				{
					if (weaponType != WeaponTypes.Staff1H)
					{
						return AnimationFlags.None;
					}
				}
				else
				{
					if (!mainHand)
					{
						return AnimationFlags.LeftPoke;
					}
					return AnimationFlags.RightPoke;
				}
			}
			if (!mainHand)
			{
				return AnimationFlags.LeftSlash;
			}
			return AnimationFlags.RightSlash;
		}
	}
}
