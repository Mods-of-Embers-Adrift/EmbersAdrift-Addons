using System;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A7F RID: 2687
	public static class HandheldItemFlagExtensions
	{
		// Token: 0x06005311 RID: 21265 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this HandheldItemFlags a, HandheldItemFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x06005312 RID: 21266 RVA: 0x000569AD File Offset: 0x00054BAD
		public static bool ContainsAnyFlag(this HandheldItemFlags flagsA, HandheldItemFlags flagsB)
		{
			return (flagsA & flagsB) > HandheldItemFlags.None;
		}

		// Token: 0x06005313 RID: 21267 RVA: 0x001D6ED0 File Offset: 0x001D50D0
		public static HandheldItemFlags GetHandheldItemFlagsForWeaponType(this WeaponTypes type)
		{
			if (type <= WeaponTypes.Hammer1H)
			{
				if (type <= WeaponTypes.Axe2H)
				{
					if (type <= WeaponTypes.Sword2H)
					{
						if (type != WeaponTypes.Sword1H)
						{
							if (type != WeaponTypes.Sword2H)
							{
								return HandheldItemFlags.None;
							}
							return HandheldItemFlags.Blade2H;
						}
					}
					else if (type != WeaponTypes.Axe1H)
					{
						if (type != WeaponTypes.Axe2H)
						{
							return HandheldItemFlags.None;
						}
						return HandheldItemFlags.Blade2H;
					}
					return HandheldItemFlags.Blade1H;
				}
				if (type <= WeaponTypes.Mace1H)
				{
					if (type == WeaponTypes.Polearm)
					{
						return HandheldItemFlags.Pierce2H;
					}
					if (type != WeaponTypes.Mace1H)
					{
						return HandheldItemFlags.None;
					}
				}
				else
				{
					if (type == WeaponTypes.Mace2H)
					{
						return HandheldItemFlags.Blunt2H;
					}
					if (type != WeaponTypes.Hammer1H)
					{
						return HandheldItemFlags.None;
					}
				}
				return HandheldItemFlags.Blunt1H;
			}
			else if (type <= WeaponTypes.Staff1H)
			{
				if (type <= WeaponTypes.Rapier)
				{
					if (type == WeaponTypes.Hammer2H)
					{
						return HandheldItemFlags.Blunt2H;
					}
					if (type - WeaponTypes.Dagger > 1)
					{
						return HandheldItemFlags.None;
					}
					return HandheldItemFlags.Pierce1H;
				}
				else if (type != WeaponTypes.Spear)
				{
					if (type != WeaponTypes.Staff1H)
					{
						return HandheldItemFlags.None;
					}
					return HandheldItemFlags.Staff1H;
				}
			}
			else if (type <= WeaponTypes.Bow)
			{
				if (type == WeaponTypes.Staff2H)
				{
					return HandheldItemFlags.Staff2H;
				}
				if (type != WeaponTypes.Bow)
				{
					return HandheldItemFlags.None;
				}
				return HandheldItemFlags.Bow;
			}
			else
			{
				if (type == WeaponTypes.Crossbow)
				{
					return HandheldItemFlags.Crossbow;
				}
				if (type == WeaponTypes.Shield)
				{
					return HandheldItemFlags.Shield;
				}
				if (type != WeaponTypes.OffhandAccessory)
				{
					return HandheldItemFlags.None;
				}
				return HandheldItemFlags.Accessory;
			}
			return HandheldItemFlags.Pierce2H;
		}
	}
}
