using System;
using SoL.Game.Settings;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C79 RID: 3193
	public static class HitTypeExtensions
	{
		// Token: 0x17001756 RID: 5974
		// (get) Token: 0x06006171 RID: 24945 RVA: 0x00081ACA File Offset: 0x0007FCCA
		public static HitType[] HitTypes
		{
			get
			{
				if (HitTypeExtensions.m_hitTypes == null)
				{
					HitTypeExtensions.m_hitTypes = (HitType[])Enum.GetValues(typeof(HitType));
				}
				return HitTypeExtensions.m_hitTypes;
			}
		}

		// Token: 0x06006172 RID: 24946 RVA: 0x001FFDF8 File Offset: 0x001FDFF8
		public static float GetModifier(this HitType hitType)
		{
			switch (hitType)
			{
			case HitType.Miss:
				return 0f;
			case HitType.Glancing:
				return 0.75f;
			case HitType.Normal:
				return 1f;
			case HitType.Heavy:
				return 1.25f;
			case HitType.Critical:
				return 1.5f;
			default:
				throw new ArgumentNullException("Unrecognized HitType: " + hitType.ToString());
			}
		}

		// Token: 0x06006173 RID: 24947 RVA: 0x001FFE5C File Offset: 0x001FE05C
		public static float GetResilienceAdjustedModifier(this HitType hitType, GameEntity entity, bool negative)
		{
			float num = hitType.GetModifier();
			if (negative && hitType - HitType.Heavy <= 1 && entity && entity.Vitals)
			{
				StatType statType = StatType.Resilience;
				StatSettings.DiminishingCurveCollection diminishingCurves = GlobalSettings.Values.Stats.DiminishingCurves;
				int diminishedStatAsInt = entity.GetDiminishedStatAsInt(statType, (diminishingCurves != null) ? diminishingCurves.Resilience : null, null, false, 0, 0f);
				if ((float)diminishedStatAsInt > 0f)
				{
					float num2 = (float)diminishedStatAsInt * 0.01f;
					float num3 = Mathf.Clamp01(1f - num2 * 0.5f);
					float num4 = num - 1f;
					num = 1f + num4 * num3;
				}
			}
			return num;
		}

		// Token: 0x06006174 RID: 24948 RVA: 0x00081AF1 File Offset: 0x0007FCF1
		public static HitType GetHitTypeForRoll(float value)
		{
			if (value < -2f)
			{
				return HitType.Miss;
			}
			if (value < -1f)
			{
				return HitType.Glancing;
			}
			if (value < 1f)
			{
				return HitType.Normal;
			}
			if (value < 2f)
			{
				return HitType.Heavy;
			}
			return HitType.Critical;
		}

		// Token: 0x06006175 RID: 24949 RVA: 0x00081B1C File Offset: 0x0007FD1C
		public static EffectApplicationFlags GetEffectApplicationFlags(this HitType hitType)
		{
			switch (hitType)
			{
			case HitType.Miss:
				return EffectApplicationFlags.Miss;
			case HitType.Glancing:
				return EffectApplicationFlags.Glancing;
			case HitType.Normal:
				return EffectApplicationFlags.Normal;
			case HitType.Heavy:
				return EffectApplicationFlags.Heavy;
			case HitType.Critical:
				return EffectApplicationFlags.Critical;
			default:
				return EffectApplicationFlags.None;
			}
		}

		// Token: 0x040054DF RID: 21727
		private const float kMissMod = 0f;

		// Token: 0x040054E0 RID: 21728
		private const float kGlancingMod = 0.75f;

		// Token: 0x040054E1 RID: 21729
		private const float kNormalMod = 1f;

		// Token: 0x040054E2 RID: 21730
		private const float kHeavyMod = 1.25f;

		// Token: 0x040054E3 RID: 21731
		private const float kCriticalMod = 1.5f;

		// Token: 0x040054E4 RID: 21732
		private static HitType[] m_hitTypes;
	}
}
