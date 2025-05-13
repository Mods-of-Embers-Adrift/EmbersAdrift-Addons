using System;
using SoL.Game.Settings;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C36 RID: 3126
	public static class EffectApplicationFlagsExtensions
	{
		// Token: 0x1700172C RID: 5932
		// (get) Token: 0x0600608A RID: 24714 RVA: 0x00081037 File Offset: 0x0007F237
		public static EffectApplicationFlags[] AllEffectApplicationFlags
		{
			get
			{
				if (EffectApplicationFlagsExtensions.m_effectApplicationFlags == null)
				{
					EffectApplicationFlagsExtensions.m_effectApplicationFlags = (EffectApplicationFlags[])Enum.GetValues(typeof(EffectApplicationFlags));
				}
				return EffectApplicationFlagsExtensions.m_effectApplicationFlags;
			}
		}

		// Token: 0x0600608B RID: 24715 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this EffectApplicationFlags a, EffectApplicationFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x0600608C RID: 24716 RVA: 0x000774DA File Offset: 0x000756DA
		public static bool HasAnyBitFlag(this EffectApplicationFlags a, EffectApplicationFlags b)
		{
			return (a & b) > EffectApplicationFlags.None;
		}

		// Token: 0x0600608D RID: 24717 RVA: 0x000578B5 File Offset: 0x00055AB5
		public static EffectApplicationFlags SetBitFlag(this EffectApplicationFlags a, EffectApplicationFlags b)
		{
			return a | b;
		}

		// Token: 0x0600608E RID: 24718 RVA: 0x000578BA File Offset: 0x00055ABA
		public static EffectApplicationFlags UnsetBitFlag(this EffectApplicationFlags a, EffectApplicationFlags b)
		{
			return a & ~b;
		}

		// Token: 0x0600608F RID: 24719 RVA: 0x0008105E File Offset: 0x0007F25E
		public static bool ShouldGiveCredit(this EffectApplicationFlags flags)
		{
			return flags != EffectApplicationFlags.None && (flags & (EffectApplicationFlags.Glancing | EffectApplicationFlags.Normal | EffectApplicationFlags.Heavy | EffectApplicationFlags.Critical | EffectApplicationFlags.PartialResist | EffectApplicationFlags.Applied | EffectApplicationFlags.Positive)) > EffectApplicationFlags.None;
		}

		// Token: 0x06006090 RID: 24720 RVA: 0x0008106F File Offset: 0x0007F26F
		public static bool IsValidCombat(this EffectApplicationFlags flags)
		{
			return !flags.HasBitFlag(EffectApplicationFlags.Positive) && (flags.ShouldGiveCredit() || flags.HasBitFlag(EffectApplicationFlags.Miss) || flags.HasBitFlag(EffectApplicationFlags.Absorb) || flags.HasBitFlag(EffectApplicationFlags.Resist));
		}

		// Token: 0x06006091 RID: 24721 RVA: 0x000810AB File Offset: 0x0007F2AB
		public static bool CanInterrupt(this EffectApplicationFlags flags)
		{
			return flags != EffectApplicationFlags.None && (flags & EffectApplicationFlags.ValidHit) > EffectApplicationFlags.None;
		}

		// Token: 0x06006092 RID: 24722 RVA: 0x000810B9 File Offset: 0x0007F2B9
		public static float GetInterruptModifier(this EffectApplicationFlags flags)
		{
			if (flags.HasBitFlag(EffectApplicationFlags.Critical))
			{
				return 1.5f;
			}
			if (flags.HasBitFlag(EffectApplicationFlags.Heavy))
			{
				return 1.1f;
			}
			if (flags.HasBitFlag(EffectApplicationFlags.Glancing))
			{
				return 0.5f;
			}
			return 1f;
		}

		// Token: 0x06006093 RID: 24723 RVA: 0x000810EE File Offset: 0x0007F2EE
		public static string GetPositiveFlagVerbiage(this EffectApplicationFlags flags)
		{
			if (flags.HasBitFlag(EffectApplicationFlags.Critical))
			{
				return "CRITICALLY ";
			}
			if (flags.HasBitFlag(EffectApplicationFlags.Heavy))
			{
				return "HEAVILY ";
			}
			return string.Empty;
		}

		// Token: 0x06006094 RID: 24724 RVA: 0x001FD7E0 File Offset: 0x001FB9E0
		public static string GetFlagText(this EffectApplicationFlags flags, bool isTarget)
		{
			if (flags.HasBitFlag(EffectApplicationFlags.Avoid))
			{
				if (isTarget)
				{
					return "AVOID";
				}
				if (flags.HasBitFlag(EffectApplicationFlags.CannotReach))
				{
					return "AVOIDS <link=\"text:Enemy is unable to reach their target!\">(CR)</link>";
				}
				return "AVOIDS";
			}
			else
			{
				if (flags.HasBitFlag(EffectApplicationFlags.Critical))
				{
					return "CRITS";
				}
				if (flags.HasBitFlag(EffectApplicationFlags.Heavy))
				{
					return "HEAVILY HITS";
				}
				if (flags.HasBitFlag(EffectApplicationFlags.Normal))
				{
					return "HITS";
				}
				if (flags.HasBitFlag(EffectApplicationFlags.Glancing))
				{
					return "GLANCES";
				}
				if (flags.HasBitFlag(EffectApplicationFlags.Miss))
				{
					return "MISSES";
				}
				if (flags.HasBitFlag(EffectApplicationFlags.Positive) && flags.HasBitFlag(EffectApplicationFlags.Applied))
				{
					return "APPLIED";
				}
				if (flags.HasBitFlag(EffectApplicationFlags.Positive))
				{
					if (!isTarget)
					{
						return "HEAL";
					}
					return "HEALS";
				}
				else
				{
					if (flags.HasBitFlag(EffectApplicationFlags.Applied))
					{
						return "APPLIED";
					}
					return "UNKNOWN";
				}
			}
		}

		// Token: 0x06006095 RID: 24725 RVA: 0x001FD8BC File Offset: 0x001FBABC
		public static string GetOverheadCombatText(this EffectApplicationFlags flags)
		{
			if (flags.HasBitFlag(EffectApplicationFlags.Block))
			{
				return "BLOCK";
			}
			if (flags.HasBitFlag(EffectApplicationFlags.Critical))
			{
				return "CRITICAL";
			}
			if (flags.HasBitFlag(EffectApplicationFlags.Heavy))
			{
				return "HEAVY";
			}
			if (flags.HasBitFlag(EffectApplicationFlags.Glancing))
			{
				return "GLANCING";
			}
			if (flags.HasBitFlag(EffectApplicationFlags.Miss))
			{
				return "MISS";
			}
			if (flags.HasBitFlag(EffectApplicationFlags.Avoid))
			{
				return "AVOID";
			}
			if (flags.HasBitFlag(EffectApplicationFlags.Block))
			{
				return "BLOCK";
			}
			if (flags.HasBitFlag(EffectApplicationFlags.Resist))
			{
				return "RESIST";
			}
			if (flags.HasBitFlag(EffectApplicationFlags.PartialResist))
			{
				return "PARTIAL RESIST";
			}
			return null;
		}

		// Token: 0x06006096 RID: 24726 RVA: 0x00081114 File Offset: 0x0007F314
		public static bool PlayHitAnimation(this EffectApplicationFlags flags)
		{
			return !flags.HasBitFlag(EffectApplicationFlags.Positive) && (flags.HasBitFlag(EffectApplicationFlags.Glancing) || flags.HasBitFlag(EffectApplicationFlags.Normal) || flags.HasBitFlag(EffectApplicationFlags.Heavy) || flags.HasBitFlag(EffectApplicationFlags.Critical));
		}

		// Token: 0x06006097 RID: 24727 RVA: 0x001FD960 File Offset: 0x001FBB60
		public static string GetAudioEvent(this EffectApplicationFlags flags)
		{
			if (flags.HasBitFlag(EffectApplicationFlags.Positive))
			{
				return null;
			}
			if (flags.HasBitFlag(EffectApplicationFlags.MeleePhysical))
			{
				return "MeleeHit";
			}
			if (flags.HasBitFlag(EffectApplicationFlags.RangedPhysical))
			{
				return "RangedHit";
			}
			if (flags.HasBitFlag(EffectApplicationFlags.Glancing) || flags.HasBitFlag(EffectApplicationFlags.Normal) || flags.HasBitFlag(EffectApplicationFlags.Heavy) || flags.HasBitFlag(EffectApplicationFlags.Critical))
			{
				return "Hit";
			}
			return null;
		}

		// Token: 0x06006098 RID: 24728 RVA: 0x0008114A File Offset: 0x0007F34A
		public static float GetAudioEventVolumeFraction(this EffectApplicationFlags flags)
		{
			if (flags.HasBitFlag(EffectApplicationFlags.Miss))
			{
				return 0f;
			}
			return 1f;
		}

		// Token: 0x06006099 RID: 24729 RVA: 0x001FD9D0 File Offset: 0x001FBBD0
		public static void GetThreatMultipliers(this EffectApplicationFlags flags, out float damageMultiplier, out float absorbedMultiplier)
		{
			damageMultiplier = 1f;
			absorbedMultiplier = 1f;
			if (flags.HasBitFlag(EffectApplicationFlags.Positive))
			{
				damageMultiplier = GlobalSettings.Values.Combat.HealThreatMultiplier;
				absorbedMultiplier = 0f;
				return;
			}
			if (flags.HasBitFlag(EffectApplicationFlags.Absorb))
			{
				damageMultiplier = 0.5f;
				absorbedMultiplier = 1f;
				return;
			}
			if (flags.HasBitFlag(EffectApplicationFlags.Applied))
			{
				damageMultiplier = 0.25f;
				absorbedMultiplier = 0.25f;
				return;
			}
			if (flags.HasBitFlag(EffectApplicationFlags.Critical))
			{
				damageMultiplier = 1.2f;
				absorbedMultiplier = 1.5f;
				return;
			}
			if (flags.HasBitFlag(EffectApplicationFlags.Glancing) || flags.HasBitFlag(EffectApplicationFlags.Normal) || flags.HasBitFlag(EffectApplicationFlags.Heavy))
			{
				absorbedMultiplier = 0.9f;
			}
		}

		// Token: 0x0600609A RID: 24730 RVA: 0x00081160 File Offset: 0x0007F360
		public static bool IsActiveDefense(this EffectApplicationFlags flags)
		{
			return flags.HasBitFlag(EffectApplicationFlags.Avoid) || flags.HasBitFlag(EffectApplicationFlags.Block) || flags.HasBitFlag(EffectApplicationFlags.Parry) || flags.HasBitFlag(EffectApplicationFlags.Riposte);
		}

		// Token: 0x0600609B RID: 24731 RVA: 0x00081190 File Offset: 0x0007F390
		public static bool SendResultsWithNoValues(this EffectApplicationFlags flags)
		{
			return flags.HasBitFlag(EffectApplicationFlags.Miss) || flags.HasBitFlag(EffectApplicationFlags.Resist) || flags.HasBitFlag(EffectApplicationFlags.Applied) || flags.IsActiveDefense();
		}

		// Token: 0x04005326 RID: 21286
		private static EffectApplicationFlags[] m_effectApplicationFlags;

		// Token: 0x04005327 RID: 21287
		public const EffectApplicationFlags kHitFlags = EffectApplicationFlags.ValidHit;

		// Token: 0x04005328 RID: 21288
		private const EffectApplicationFlags kFlagsThatShouldGiveCredit = EffectApplicationFlags.Glancing | EffectApplicationFlags.Normal | EffectApplicationFlags.Heavy | EffectApplicationFlags.Critical | EffectApplicationFlags.PartialResist | EffectApplicationFlags.Applied | EffectApplicationFlags.Positive;
	}
}
