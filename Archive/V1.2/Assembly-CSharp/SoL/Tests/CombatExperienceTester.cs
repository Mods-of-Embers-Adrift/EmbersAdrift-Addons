using System;
using SoL.Game.Settings;
using SoL.Game.Spawning;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000D92 RID: 3474
	[CreateAssetMenu(menuName = "SoL/Tests/CombatExperienceTester")]
	public class CombatExperienceTester : ScriptableObject
	{
		// Token: 0x0600686A RID: 26730 RVA: 0x002143D0 File Offset: 0x002125D0
		private void GetExperience()
		{
			float num = this.m_useManualXp ? ((float)this.m_manualXp) : GlobalSettings.Values.Progression.AdventuringLevelCurve.GetRewardForLevel(this.m_level);
			float num2 = num * this.m_challengeRating.GetDefaultExperienceModifier();
			if (this.m_groupSize <= 1)
			{
				ulong num3 = (ulong)Mathf.Clamp(num2, 1f, float.MaxValue);
				Debug.Log("Unmodified XP: " + Mathf.FloorToInt(num).ToString() + ", Total XP: " + num3.ToString());
				return;
			}
			ulong experiencePerGroupMember = ProgressionCalculator.GetExperiencePerGroupMember(num2, this.m_groupSize, this.m_challengeRating);
			Debug.Log(string.Concat(new string[]
			{
				"Unmodified XP: ",
				Mathf.FloorToInt(num).ToString(),
				", Total XP: ",
				(experiencePerGroupMember * (ulong)((long)this.m_groupSize)).ToString(),
				", Per Member: ",
				experiencePerGroupMember.ToString()
			}));
		}

		// Token: 0x04005A9B RID: 23195
		private const string kNpc = "NPC";

		// Token: 0x04005A9C RID: 23196
		private const string kManual = "Manual";

		// Token: 0x04005A9D RID: 23197
		[Range(1f, 6f)]
		[SerializeField]
		private int m_groupSize = 1;

		// Token: 0x04005A9E RID: 23198
		[Range(1f, 50f)]
		[SerializeField]
		private int m_level = 1;

		// Token: 0x04005A9F RID: 23199
		[SerializeField]
		private ChallengeRating m_challengeRating = ChallengeRating.CR1;

		// Token: 0x04005AA0 RID: 23200
		[SerializeField]
		private bool m_useManualXp;

		// Token: 0x04005AA1 RID: 23201
		[SerializeField]
		private int m_manualXp = 1000;
	}
}
