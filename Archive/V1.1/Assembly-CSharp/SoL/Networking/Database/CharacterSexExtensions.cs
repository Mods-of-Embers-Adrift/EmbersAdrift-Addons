using System;
using System.Collections.Generic;
using UMA.CharacterSystem;

namespace SoL.Networking.Database
{
	// Token: 0x0200043D RID: 1085
	public static class CharacterSexExtensions
	{
		// Token: 0x06001EE5 RID: 7909 RVA: 0x00056E2C File Offset: 0x0005502C
		public static bool IsAssigned(this CharacterSex sex)
		{
			return sex > CharacterSex.None;
		}

		// Token: 0x06001EE6 RID: 7910 RVA: 0x00056E32 File Offset: 0x00055032
		public static bool IsUnassigned(this CharacterSex sex)
		{
			return !sex.IsAssigned();
		}

		// Token: 0x06001EE7 RID: 7911 RVA: 0x00056E3D File Offset: 0x0005503D
		public static string GetUmaRace(this CharacterSex sex)
		{
			if (sex == CharacterSex.Male)
			{
				return "SolHumanMale";
			}
			if (sex != CharacterSex.Female)
			{
				throw new ArgumentException("Invalid CharacterSex! " + sex.ToString());
			}
			return "SolHumanFemale";
		}

		// Token: 0x06001EE8 RID: 7912 RVA: 0x0011D72C File Offset: 0x0011B92C
		public static string GetUmaRace(this CharacterSex sex, CharacterBuildType buildType)
		{
			if (CharacterSexExtensions.m_maleRaceCache == null)
			{
				CharacterSexExtensions.m_maleRaceCache = new Dictionary<CharacterBuildType, string>(default(CharacterBuildTypeComparer));
				CharacterSexExtensions.m_femaleRaceCache = new Dictionary<CharacterBuildType, string>(default(CharacterBuildTypeComparer));
			}
			Dictionary<CharacterBuildType, string> dictionary = (sex == CharacterSex.Male) ? CharacterSexExtensions.m_maleRaceCache : CharacterSexExtensions.m_femaleRaceCache;
			string text;
			if (!dictionary.TryGetValue(buildType, out text))
			{
				text = sex.ToString() + "_" + buildType.ToString();
				dictionary.Add(buildType, text);
			}
			return text;
		}

		// Token: 0x06001EE9 RID: 7913 RVA: 0x0011D7BC File Offset: 0x0011B9BC
		public static CharacterSex GetSexForUmaRace(this DynamicCharacterAvatar dca)
		{
			if (CharacterSexExtensions.m_raceToSexCache == null)
			{
				CharacterSexExtensions.m_raceToSexCache = new Dictionary<string, CharacterSex>(10);
			}
			string name = dca.activeRace.name;
			CharacterSex characterSex;
			if (CharacterSexExtensions.m_raceToSexCache.TryGetValue(name, out characterSex))
			{
				return characterSex;
			}
			characterSex = (name.Contains(CharacterSex.Male.ToString()) ? CharacterSex.Male : CharacterSex.Female);
			CharacterSexExtensions.m_raceToSexCache.Add(name, characterSex);
			return characterSex;
		}

		// Token: 0x0400244D RID: 9293
		private const string kUMAMaleRaceName = "SolHumanMale";

		// Token: 0x0400244E RID: 9294
		private const string kUMAFemaleRaceName = "SolHumanFemale";

		// Token: 0x0400244F RID: 9295
		private static Dictionary<CharacterBuildType, string> m_maleRaceCache;

		// Token: 0x04002450 RID: 9296
		private static Dictionary<CharacterBuildType, string> m_femaleRaceCache;

		// Token: 0x04002451 RID: 9297
		private static Dictionary<string, CharacterSex> m_raceToSexCache;
	}
}
