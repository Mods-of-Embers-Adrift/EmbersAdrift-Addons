using System;
using Cysharp.Text;
using SoL.Managers;
using SoL.Utilities.Extensions;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A9F RID: 2719
	public static class RoleFlagExtensions
	{
		// Token: 0x1700134F RID: 4943
		// (get) Token: 0x0600541C RID: 21532 RVA: 0x000783F5 File Offset: 0x000765F5
		private static BaseRoleFlags[] AllBaseRoleFlags
		{
			get
			{
				if (RoleFlagExtensions.m_baseRoleFlags == null)
				{
					RoleFlagExtensions.m_baseRoleFlags = (BaseRoleFlags[])Enum.GetValues(typeof(BaseRoleFlags));
				}
				return RoleFlagExtensions.m_baseRoleFlags;
			}
		}

		// Token: 0x17001350 RID: 4944
		// (get) Token: 0x0600541D RID: 21533 RVA: 0x0007841C File Offset: 0x0007661C
		private static SpecializedRoleFlags[] AllSpecializedRoleFlags
		{
			get
			{
				if (RoleFlagExtensions.m_specializedRoleFlags == null)
				{
					RoleFlagExtensions.m_specializedRoleFlags = (SpecializedRoleFlags[])Enum.GetValues(typeof(SpecializedRoleFlags));
				}
				return RoleFlagExtensions.m_specializedRoleFlags;
			}
		}

		// Token: 0x0600541E RID: 21534 RVA: 0x00078443 File Offset: 0x00076643
		internal static string GetBaseRoleAbbreviation(this BaseRoleFlags flag)
		{
			switch (flag)
			{
			case BaseRoleFlags.Defender:
				return "DEF";
			case BaseRoleFlags.Striker:
				return "STK";
			case BaseRoleFlags.Supporter:
				return "SUP";
			}
			return string.Empty;
		}

		// Token: 0x0600541F RID: 21535 RVA: 0x00078476 File Offset: 0x00076676
		internal static string GetSpecializationRoleAbbreviation(this SpecializedRoleFlags flag)
		{
			return flag.ToString();
		}

		// Token: 0x06005420 RID: 21536 RVA: 0x001D9EF4 File Offset: 0x001D80F4
		internal static string GetSecondaryRoleAbbreviation(this SpecializedRoleFlags specializedRoleFlags)
		{
			if (specializedRoleFlags <= SpecializedRoleFlags.Brigand)
			{
				switch (specializedRoleFlags)
				{
				case SpecializedRoleFlags.Juggernaut:
					goto IL_52;
				case SpecializedRoleFlags.Knight:
					goto IL_59;
				case SpecializedRoleFlags.Juggernaut | SpecializedRoleFlags.Knight:
					goto IL_66;
				case SpecializedRoleFlags.Marshal:
					goto IL_60;
				default:
					if (specializedRoleFlags != SpecializedRoleFlags.Berserker)
					{
						if (specializedRoleFlags != SpecializedRoleFlags.Brigand)
						{
							goto IL_66;
						}
						goto IL_60;
					}
					break;
				}
			}
			else if (specializedRoleFlags <= SpecializedRoleFlags.Duelist)
			{
				if (specializedRoleFlags == SpecializedRoleFlags.Warden)
				{
					goto IL_59;
				}
				if (specializedRoleFlags != SpecializedRoleFlags.Duelist)
				{
					goto IL_66;
				}
				goto IL_52;
			}
			else if (specializedRoleFlags != SpecializedRoleFlags.Sentinel)
			{
				if (specializedRoleFlags != SpecializedRoleFlags.Warlord)
				{
					goto IL_66;
				}
				goto IL_60;
			}
			return BaseRoleFlags.Defender.GetBaseRoleAbbreviation();
			IL_52:
			return BaseRoleFlags.Striker.GetBaseRoleAbbreviation();
			IL_59:
			return BaseRoleFlags.Supporter.GetBaseRoleAbbreviation();
			IL_60:
			return "CON";
			IL_66:
			return "Unknown";
		}

		// Token: 0x06005421 RID: 21537 RVA: 0x001D9F6C File Offset: 0x001D816C
		public static bool MatchesNameFilter(this BaseRoleFlags flags, string filter)
		{
			for (int i = 0; i < RoleFlagExtensions.AllBaseRoleFlags.Length; i++)
			{
				if (RoleFlagExtensions.AllBaseRoleFlags[i] != BaseRoleFlags.None && flags.HasBitFlag(RoleFlagExtensions.AllBaseRoleFlags[i]) && RoleFlagExtensions.AllBaseRoleFlags[i].ToString().Contains(filter, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005422 RID: 21538 RVA: 0x001D9FC8 File Offset: 0x001D81C8
		public static bool MatchesNameFilter(this SpecializedRoleFlags flags, string filter)
		{
			for (int i = 0; i < RoleFlagExtensions.AllSpecializedRoleFlags.Length; i++)
			{
				if (RoleFlagExtensions.AllSpecializedRoleFlags[i] != SpecializedRoleFlags.None && flags.HasBitFlag(RoleFlagExtensions.AllSpecializedRoleFlags[i]) && RoleFlagExtensions.AllSpecializedRoleFlags[i].ToString().Contains(filter, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005423 RID: 21539 RVA: 0x001DA024 File Offset: 0x001D8224
		public static string GetBaseRoleDescription(this BaseRoleFlags flags, BaseRoleFlags selectedFlag, bool isCompatible)
		{
			string result = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				int num = 0;
				for (int i = 0; i < RoleFlagExtensions.AllBaseRoleFlags.Length; i++)
				{
					if (RoleFlagExtensions.AllBaseRoleFlags[i] != BaseRoleFlags.None && flags.HasBitFlag(RoleFlagExtensions.AllBaseRoleFlags[i]))
					{
						num++;
					}
				}
				int num2 = 0;
				for (int j = 0; j < RoleFlagExtensions.AllBaseRoleFlags.Length; j++)
				{
					if (RoleFlagExtensions.AllBaseRoleFlags[j] != BaseRoleFlags.None && flags.HasBitFlag(RoleFlagExtensions.AllBaseRoleFlags[j]))
					{
						BaseRoleFlags baseRoleFlags = RoleFlagExtensions.AllBaseRoleFlags[j];
						if (isCompatible)
						{
							if (selectedFlag != BaseRoleFlags.None && selectedFlag.HasBitFlag(RoleFlagExtensions.AllBaseRoleFlags[j]))
							{
								utf16ValueStringBuilder.AppendFormat<string, BaseRoleFlags>("<color={0}>{1}</color>", UIManager.RequirementsMetColor.ToHex(), baseRoleFlags);
							}
							else
							{
								utf16ValueStringBuilder.AppendFormat<BaseRoleFlags>("{0}", baseRoleFlags);
							}
						}
						else
						{
							utf16ValueStringBuilder.AppendFormat<string, BaseRoleFlags>("<color={0}>{1}</color>", UIManager.RequirementsNotMetColor.ToHex(), baseRoleFlags);
						}
						num2++;
						if (num > 1 && num2 < num)
						{
							utf16ValueStringBuilder.Append(", ");
						}
					}
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06005424 RID: 21540 RVA: 0x001DA158 File Offset: 0x001D8358
		public static string GetSpecializationRoleDescription(this SpecializedRoleFlags flags, SpecializedRoleFlags selectedFlag, bool isCompatible)
		{
			string result = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				int num = 0;
				for (int i = 0; i < RoleFlagExtensions.AllSpecializedRoleFlags.Length; i++)
				{
					if (RoleFlagExtensions.AllSpecializedRoleFlags[i] != SpecializedRoleFlags.None && flags.HasBitFlag(RoleFlagExtensions.AllSpecializedRoleFlags[i]))
					{
						num++;
					}
				}
				int num2 = 0;
				for (int j = 0; j < RoleFlagExtensions.AllSpecializedRoleFlags.Length; j++)
				{
					if (RoleFlagExtensions.AllSpecializedRoleFlags[j] != SpecializedRoleFlags.None && flags.HasBitFlag(RoleFlagExtensions.AllSpecializedRoleFlags[j]))
					{
						if (isCompatible)
						{
							if (selectedFlag != SpecializedRoleFlags.None && selectedFlag.HasBitFlag(RoleFlagExtensions.AllSpecializedRoleFlags[j]))
							{
								utf16ValueStringBuilder.AppendFormat<string, SpecializedRoleFlags>("<color={0}>{1}</color>", UIManager.RequirementsMetColor.ToHex(), RoleFlagExtensions.AllSpecializedRoleFlags[j]);
							}
							else
							{
								utf16ValueStringBuilder.AppendFormat<string>("{0}", RoleFlagExtensions.AllSpecializedRoleFlags[j].GetSpecializationRoleAbbreviation());
							}
						}
						else
						{
							utf16ValueStringBuilder.AppendFormat<string, SpecializedRoleFlags>("<color={0}>{1}</color>", UIManager.RequirementsNotMetColor.ToHex(), RoleFlagExtensions.AllSpecializedRoleFlags[j]);
						}
						num2++;
						if (num > 1 && num2 < num)
						{
							utf16ValueStringBuilder.Append(", ");
						}
					}
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06005425 RID: 21541 RVA: 0x00078485 File Offset: 0x00076685
		public static RolePacked GetPacked(this BaseRoleFlags flags)
		{
			switch (flags)
			{
			case BaseRoleFlags.Defender:
				return RolePacked.Defender;
			case BaseRoleFlags.Striker:
				return RolePacked.Striker;
			case BaseRoleFlags.Supporter:
				return RolePacked.Supporter;
			}
			return RolePacked.Invalid;
		}

		// Token: 0x06005426 RID: 21542 RVA: 0x001DA2A8 File Offset: 0x001D84A8
		public static RolePacked GetPacked(this SpecializedRoleFlags flags)
		{
			if (flags <= SpecializedRoleFlags.Brigand)
			{
				switch (flags)
				{
				case SpecializedRoleFlags.Juggernaut:
					return RolePacked.Juggernaut;
				case SpecializedRoleFlags.Knight:
					return RolePacked.Knight;
				case SpecializedRoleFlags.Juggernaut | SpecializedRoleFlags.Knight:
					break;
				case SpecializedRoleFlags.Marshal:
					return RolePacked.Marshal;
				default:
					if (flags == SpecializedRoleFlags.Berserker)
					{
						return RolePacked.Berserker;
					}
					if (flags == SpecializedRoleFlags.Brigand)
					{
						return RolePacked.Brigand;
					}
					break;
				}
			}
			else if (flags <= SpecializedRoleFlags.Duelist)
			{
				if (flags == SpecializedRoleFlags.Warden)
				{
					return RolePacked.Warden;
				}
				if (flags == SpecializedRoleFlags.Duelist)
				{
					return RolePacked.Duelist;
				}
			}
			else
			{
				if (flags == SpecializedRoleFlags.Sentinel)
				{
					return RolePacked.Sentinel;
				}
				if (flags == SpecializedRoleFlags.Warlord)
				{
					return RolePacked.Warlord;
				}
			}
			return RolePacked.Invalid;
		}

		// Token: 0x06005427 RID: 21543 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this BaseRoleFlags a, BaseRoleFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x06005428 RID: 21544 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this SpecializedRoleFlags a, SpecializedRoleFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x04004AF4 RID: 19188
		private static BaseRoleFlags[] m_baseRoleFlags;

		// Token: 0x04004AF5 RID: 19189
		private static SpecializedRoleFlags[] m_specializedRoleFlags;
	}
}
