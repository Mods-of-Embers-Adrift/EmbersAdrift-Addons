using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C54 RID: 3156
	public static class DamageTypeExtensions
	{
		// Token: 0x0600611C RID: 24860 RVA: 0x001FF16C File Offset: 0x001FD36C
		public static string GetDamageTypeSubDisplay(this DamageType dmgType)
		{
			switch (dmgType)
			{
			case DamageType.Melee_Slashing:
				return "Slashing";
			case DamageType.Melee_Crushing:
			case DamageType.Ranged_Crushing:
				return "Crushing";
			case DamageType.Melee_Piercing:
			case DamageType.Ranged_Piercing:
				return "Piercing";
			case DamageType.Ranged_Auditory:
				return "Auditory";
			case DamageType.Natural_Life:
				return "Life";
			case DamageType.Natural_Death:
				return "Death";
			case DamageType.Natural_Chemical:
				return "Chemical";
			case DamageType.Natural_Spirit:
				return "Spirit";
			case DamageType.Elemental_Air:
				return "Air";
			case DamageType.Elemental_Earth:
				return "Earth";
			case DamageType.Elemental_Fire:
				return "Fire";
			case DamageType.Elemental_Water:
				return "Water";
			default:
				throw new ArgumentException("dmgType");
			}
		}

		// Token: 0x0600611D RID: 24861 RVA: 0x001FF20C File Offset: 0x001FD40C
		public static EffectSubChannelType GetSubChannelType(this DamageType channel)
		{
			switch (channel)
			{
			case DamageType.Melee_Slashing:
			case DamageType.Melee_Crushing:
			case DamageType.Melee_Piercing:
				return EffectSubChannelType.Melee;
			case DamageType.Ranged_Auditory:
			case DamageType.Ranged_Crushing:
			case DamageType.Ranged_Piercing:
				return EffectSubChannelType.Ranged;
			case DamageType.Natural_Life:
			case DamageType.Natural_Death:
			case DamageType.Natural_Chemical:
			case DamageType.Natural_Spirit:
				return EffectSubChannelType.Natural;
			case DamageType.Elemental_Air:
			case DamageType.Elemental_Earth:
			case DamageType.Elemental_Fire:
			case DamageType.Elemental_Water:
				return EffectSubChannelType.Elemental;
			default:
				throw new ArgumentException("channel");
			}
		}

		// Token: 0x0600611E RID: 24862 RVA: 0x000816F5 File Offset: 0x0007F8F5
		public static bool IsMeleePhysical(this DamageType dmgType)
		{
			return dmgType <= DamageType.Melee_Piercing;
		}

		// Token: 0x0600611F RID: 24863 RVA: 0x0007527E File Offset: 0x0007347E
		public static bool IsRangedPhysical(this DamageType dmgType)
		{
			return dmgType - DamageType.Ranged_Crushing <= 1;
		}

		// Token: 0x06006120 RID: 24864 RVA: 0x000816FE File Offset: 0x0007F8FE
		public static bool IsPhysical(this DamageType dmgType)
		{
			return dmgType.IsMeleePhysical() || dmgType.IsRangedPhysical();
		}

		// Token: 0x06006121 RID: 24865 RVA: 0x001FF26C File Offset: 0x001FD46C
		// Note: this type is marked as 'beforefieldinit'.
		static DamageTypeExtensions()
		{
			DamageType[] array = new DamageType[3];
			array[0] = DamageType.Melee_Piercing;
			array[1] = DamageType.Melee_Crushing;
			DamageTypeExtensions.MeleeDamageChannels = array;
			DamageTypeExtensions.RangedDamageChannels = new DamageType[]
			{
				DamageType.Ranged_Piercing,
				DamageType.Ranged_Crushing,
				DamageType.Ranged_Auditory
			};
			DamageTypeExtensions.NaturalDamageChannels = new DamageType[]
			{
				DamageType.Natural_Life,
				DamageType.Natural_Death,
				DamageType.Natural_Spirit,
				DamageType.Natural_Chemical
			};
			DamageTypeExtensions.ElementalDamageChannels = new DamageType[]
			{
				DamageType.Elemental_Air,
				DamageType.Elemental_Fire,
				DamageType.Elemental_Earth,
				DamageType.Elemental_Water
			};
		}

		// Token: 0x040053FE RID: 21502
		public static readonly DamageType[] MeleeDamageChannels;

		// Token: 0x040053FF RID: 21503
		public static readonly DamageType[] RangedDamageChannels;

		// Token: 0x04005400 RID: 21504
		public static readonly DamageType[] NaturalDamageChannels;

		// Token: 0x04005401 RID: 21505
		public static readonly DamageType[] ElementalDamageChannels;
	}
}
