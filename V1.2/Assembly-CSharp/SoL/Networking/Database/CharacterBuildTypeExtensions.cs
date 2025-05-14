using System;
using System.Collections.Generic;

namespace SoL.Networking.Database
{
	// Token: 0x02000427 RID: 1063
	public static class CharacterBuildTypeExtensions
	{
		// Token: 0x06001E7D RID: 7805 RVA: 0x00056B56 File Offset: 0x00054D56
		public static CharacterBuildType GetRandomBuildType(Random seed)
		{
			return CharacterBuildTypeExtensions.m_buildTypes[seed.Next(CharacterBuildTypeExtensions.m_buildTypes.Length)];
		}

		// Token: 0x06001E7E RID: 7806 RVA: 0x0011AE20 File Offset: 0x00119020
		public static CharacterBuildType GetRandomBuildType(CharacterBuildTypeFlags restrictions, Random seed)
		{
			if (restrictions == CharacterBuildTypeFlags.None || restrictions == CharacterBuildTypeFlags.All)
			{
				return CharacterBuildTypeExtensions.GetRandomBuildType(seed);
			}
			if (CharacterBuildTypeExtensions.m_restrictedBuildTypes == null)
			{
				CharacterBuildTypeExtensions.m_restrictedBuildTypes = new List<CharacterBuildType>(CharacterBuildTypeExtensions.m_buildTypes.Length);
			}
			CharacterBuildTypeExtensions.m_restrictedBuildTypes.Clear();
			for (int i = 0; i < CharacterBuildTypeExtensions.m_buildTypeFlags.Length; i++)
			{
				if (restrictions.HasBitFlag(CharacterBuildTypeExtensions.m_buildTypeFlags[i]))
				{
					CharacterBuildType buildTypeForFlag = CharacterBuildTypeExtensions.m_buildTypeFlags[i].GetBuildTypeForFlag();
					if (buildTypeForFlag != CharacterBuildType.None)
					{
						CharacterBuildTypeExtensions.m_restrictedBuildTypes.Add(buildTypeForFlag);
					}
				}
			}
			if (CharacterBuildTypeExtensions.m_restrictedBuildTypes.Count <= 0)
			{
				return CharacterBuildTypeExtensions.GetRandomBuildType(seed);
			}
			return CharacterBuildTypeExtensions.m_restrictedBuildTypes[seed.Next(CharacterBuildTypeExtensions.m_restrictedBuildTypes.Count)];
		}

		// Token: 0x06001E7F RID: 7807 RVA: 0x0004FB40 File Offset: 0x0004DD40
		private static bool HasBitFlag(this CharacterBuildTypeFlags a, CharacterBuildTypeFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x06001E80 RID: 7808 RVA: 0x00056B6B File Offset: 0x00054D6B
		private static CharacterBuildType GetBuildTypeForFlag(this CharacterBuildTypeFlags flag)
		{
			switch (flag)
			{
			case CharacterBuildTypeFlags.Stoic:
				return CharacterBuildType.Stoic;
			case CharacterBuildTypeFlags.Brawny:
				return CharacterBuildType.Brawny;
			case CharacterBuildTypeFlags.Stoic | CharacterBuildTypeFlags.Brawny:
				break;
			case CharacterBuildTypeFlags.Lean:
				return CharacterBuildType.Lean;
			default:
				if (flag == CharacterBuildTypeFlags.Heavyset)
				{
					return CharacterBuildType.Heavyset;
				}
				if (flag == CharacterBuildTypeFlags.Rotund)
				{
					return CharacterBuildType.Rotund;
				}
				break;
			}
			return CharacterBuildType.None;
		}

		// Token: 0x040023DC RID: 9180
		private static List<CharacterBuildType> m_restrictedBuildTypes = null;

		// Token: 0x040023DD RID: 9181
		private static readonly CharacterBuildType[] m_buildTypes = new CharacterBuildType[]
		{
			CharacterBuildType.Stoic,
			CharacterBuildType.Brawny,
			CharacterBuildType.Lean,
			CharacterBuildType.Heavyset,
			CharacterBuildType.Rotund
		};

		// Token: 0x040023DE RID: 9182
		private static readonly CharacterBuildTypeFlags[] m_buildTypeFlags = new CharacterBuildTypeFlags[]
		{
			CharacterBuildTypeFlags.Stoic,
			CharacterBuildTypeFlags.Brawny,
			CharacterBuildTypeFlags.Lean,
			CharacterBuildTypeFlags.Heavyset,
			CharacterBuildTypeFlags.Rotund
		};
	}
}
