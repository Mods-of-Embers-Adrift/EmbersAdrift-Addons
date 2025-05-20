using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Settings;

namespace SoL.Game
{
	// Token: 0x020005D6 RID: 1494
	public static class StanceExtensions
	{
		// Token: 0x06002F6D RID: 12141 RVA: 0x00157208 File Offset: 0x00155408
		public static StanceProfile GetStanceProfile(this Stance stance)
		{
			switch (stance)
			{
			case Stance.Crouch:
				return GlobalSettings.Values.Stance.CrouchStance;
			case Stance.Combat:
				return GlobalSettings.Values.Stance.CombatStance;
			case Stance.Sit:
				return GlobalSettings.Values.Stance.RestingStance;
			case Stance.Swim:
				return GlobalSettings.Values.Stance.SwimmingStance;
			}
			return GlobalSettings.Values.Stance.DefaultStance;
		}

		// Token: 0x06002F6E RID: 12142 RVA: 0x00157284 File Offset: 0x00155484
		public static float GetDetectionMultiplier(this Stance stance)
		{
			if (stance == Stance.Torch)
			{
				return 2f;
			}
			StanceProfile stanceProfile = stance.GetStanceProfile();
			if (!stanceProfile)
			{
				return 1f;
			}
			return stanceProfile.DetectionMultiplier;
		}

		// Token: 0x06002F6F RID: 12143 RVA: 0x00060BC3 File Offset: 0x0005EDC3
		public static bool CanExecute(this Stance stance, bool requireCombatStance)
		{
			return (stance == Stance.Idle || stance == Stance.Combat) && (!requireCombatStance || stance == Stance.Combat);
		}

		// Token: 0x06002F70 RID: 12144 RVA: 0x00060BD7 File Offset: 0x0005EDD7
		public static bool CanExecute(this Stance stance, StanceFlags requiredStanceFlags)
		{
			return (stance.GetFlagForStance() & requiredStanceFlags) > StanceFlags.None;
		}

		// Token: 0x06002F71 RID: 12145 RVA: 0x00060BE4 File Offset: 0x0005EDE4
		public static bool CanForgetMastery(this Stance stance)
		{
			return stance <= Stance.Crouch || stance - Stance.Torch <= 1;
		}

		// Token: 0x06002F72 RID: 12146 RVA: 0x00060BF3 File Offset: 0x0005EDF3
		public static bool CanJump(this Stance stance)
		{
			return stance != Stance.Crouch && stance - Stance.Sit > 3;
		}

		// Token: 0x06002F73 RID: 12147 RVA: 0x0004479C File Offset: 0x0004299C
		public static bool CanEmote(this Stance stance)
		{
			return true;
		}

		// Token: 0x06002F74 RID: 12148 RVA: 0x00060C02 File Offset: 0x0005EE02
		public static bool CanPlayEmoteAnimation(this Stance stance)
		{
			return stance == Stance.Idle || stance - Stance.Combat <= 1;
		}

		// Token: 0x06002F75 RID: 12149 RVA: 0x00060C10 File Offset: 0x0005EE10
		public static bool PlayerCanExit(this Stance stance)
		{
			return stance.GetStanceProfile().CanManuallyExit;
		}

		// Token: 0x06002F76 RID: 12150 RVA: 0x001572B8 File Offset: 0x001554B8
		private static StanceFlags GetFlagForStance(this Stance stance)
		{
			switch (stance)
			{
			case Stance.Idle:
				return StanceFlags.Idle;
			case Stance.Crouch:
				return StanceFlags.Crouch;
			case Stance.Combat:
				return StanceFlags.Combat;
			case Stance.Torch:
				return StanceFlags.Torch;
			case Stance.Sit:
				return StanceFlags.Sit;
			case Stance.Swim:
				return StanceFlags.Swim;
			case Stance.Unconscious:
				return StanceFlags.Unconscious;
			case Stance.Looting:
				return StanceFlags.Looting;
			default:
				return StanceFlags.None;
			}
		}

		// Token: 0x06002F77 RID: 12151 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this StanceFlags a, StanceFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x06002F78 RID: 12152 RVA: 0x00060C1D File Offset: 0x0005EE1D
		public static bool GivesNpcAdvantage(this Stance stance)
		{
			return stance == Stance.Crouch || stance == Stance.Sit;
		}

		// Token: 0x06002F79 RID: 12153 RVA: 0x00060C2A File Offset: 0x0005EE2A
		public static bool ResetFullyRestedState(this Stance stance)
		{
			return stance == Stance.Unconscious;
		}

		// Token: 0x06002F7A RID: 12154 RVA: 0x00060C33 File Offset: 0x0005EE33
		public static bool ApplyFullyRestedBonus(this Stance stance)
		{
			return stance != Stance.Combat && stance - Stance.Swim > 1;
		}

		// Token: 0x06002F7B RID: 12155 RVA: 0x00060C42 File Offset: 0x0005EE42
		public static bool ApplyRegenStat(this Stance stance)
		{
			return stance - Stance.Swim > 1;
		}

		// Token: 0x17000A0D RID: 2573
		// (get) Token: 0x06002F7C RID: 12156 RVA: 0x00060C4D File Offset: 0x0005EE4D
		private static StanceFlags[] AllStanceFlags
		{
			get
			{
				if (StanceExtensions.m_allStanceFlags == null)
				{
					StanceExtensions.m_allStanceFlags = (StanceFlags[])Enum.GetValues(typeof(StanceFlags));
				}
				return StanceExtensions.m_allStanceFlags;
			}
		}

		// Token: 0x06002F7D RID: 12157 RVA: 0x00157308 File Offset: 0x00155508
		public static string GetStanceFlagTooltipDescription(this StanceFlags flags)
		{
			string result = string.Empty;
			if (flags == StanceFlags.None)
			{
				return result;
			}
			if (StanceExtensions.m_stanceFlagsList == null)
			{
				StanceExtensions.m_stanceFlagsList = new List<StanceFlags>(StanceExtensions.AllStanceFlags.Length);
			}
			StanceExtensions.m_stanceFlagsList.Clear();
			for (int i = 0; i < StanceExtensions.AllStanceFlags.Length; i++)
			{
				if (StanceExtensions.AllStanceFlags[i] != StanceFlags.None && flags.HasBitFlag(StanceExtensions.AllStanceFlags[i]))
				{
					StanceExtensions.m_stanceFlagsList.Add(StanceExtensions.AllStanceFlags[i]);
				}
			}
			if (StanceExtensions.m_stanceFlagsList.Count > 0)
			{
				using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
				{
					for (int j = 0; j < StanceExtensions.m_stanceFlagsList.Count; j++)
					{
						if (j > 0)
						{
							if (j < StanceExtensions.m_stanceFlagsList.Count - 1)
							{
								utf16ValueStringBuilder.Append(", ");
							}
							else if (j == StanceExtensions.m_stanceFlagsList.Count - 1)
							{
								utf16ValueStringBuilder.Append(", or ");
							}
						}
						utf16ValueStringBuilder.AppendFormat<StanceFlags>("{0}", StanceExtensions.m_stanceFlagsList[j]);
					}
					result = utf16ValueStringBuilder.ToString();
				}
			}
			return result;
		}

		// Token: 0x06002F7E RID: 12158 RVA: 0x00060C74 File Offset: 0x0005EE74
		public static bool AllowOnRoad(this Stance stance)
		{
			switch (stance)
			{
			case Stance.Combat:
			case Stance.Swim:
			case Stance.Unconscious:
				return false;
			}
			return true;
		}

		// Token: 0x04002E84 RID: 11908
		private static StanceFlags[] m_allStanceFlags;

		// Token: 0x04002E85 RID: 11909
		private static List<StanceFlags> m_stanceFlagsList;
	}
}
