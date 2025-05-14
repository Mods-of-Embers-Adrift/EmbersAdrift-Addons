using System;
using Cysharp.Text;
using SoL.Game.Quests;
using SoL.Game.Settings;
using SoL.Game.Spawning;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000DA0 RID: 3488
	[CreateAssetMenu(menuName = "SoL/Tests/ExperienceProgression")]
	public class ExperienceProgressionTester : ScriptableObject
	{
		// Token: 0x0600689C RID: 26780 RVA: 0x00215110 File Offset: 0x00213310
		private void KillTable()
		{
			int num = 0;
			for (int i = 1; i < 50; i++)
			{
				int num2 = i;
				int num3 = Mathf.Clamp(i + 1, 1, 50);
				ulong num4 = (ulong)this.m_levelCurve.GetTotalForLevel(num3);
				int level = Mathf.Clamp(num2 + this.m_levelDelta, 1, 50);
				double num5 = (double)this.m_levelCurve.GetRewardForLevel(level);
				if (!this.m_bypassChevronMultiplier)
				{
					num5 *= (double)this.m_killTableCR.GetDefaultExperienceModifier();
				}
				num5 = ProgressionCalculator.ModifyAdventuringExperienceBasedOnLevel(num2, num3, num5);
				int num6 = Mathf.CeilToInt(num4 / (float)num5);
				Debug.Log(string.Format("Level from {0}-->{1} took {2} kills ({3} delta, {4:F02} per opponent)", new object[]
				{
					num2.ToString(),
					num3.ToString(),
					num6.ToString(),
					num6 - num,
					num5
				}));
				num = num6;
			}
		}

		// Token: 0x0600689D RID: 26781 RVA: 0x002151EC File Offset: 0x002133EC
		private void GetXp()
		{
			float rewardForLevel = this.m_levelCurve.GetRewardForLevel(this.m_targetLevel);
			float num = (float)ProgressionCalculator.ModifyAdventuringExperienceBasedOnLevel(Mathf.FloorToInt(this.m_sourceLevel), this.m_targetLevel, (double)rewardForLevel);
			Debug.Log("Input XP: " + rewardForLevel.ToString() + "  Output XP: " + num.ToString());
		}

		// Token: 0x0600689E RID: 26782 RVA: 0x0004475B File Offset: 0x0004295B
		private void AddXp()
		{
		}

		// Token: 0x0600689F RID: 26783 RVA: 0x00215248 File Offset: 0x00213448
		private void Calculate()
		{
			float num = (float)this.m_experience * this.m_challengeRating.GetDefaultExperienceModifier();
			float f = (this.m_groupSize > 1 && num > 0f) ? ProgressionCalculator.GetExperiencePerGroupMember(num, this.m_groupSize, this.m_challengeRating) : num;
			Debug.Log(string.Format("XP: {0}", Mathf.FloorToInt(f)));
		}

		// Token: 0x060068A0 RID: 26784 RVA: 0x002152AC File Offset: 0x002134AC
		private void GetTaskDisplayXp()
		{
			if (this.m_task)
			{
				float totalForLevel = GlobalSettings.Values.Progression.AdventuringLevelCurve.GetTotalForLevel(this.m_taskPlayerLevel + 1);
				int num = (this.m_taskPlayerLevel <= this.m_task.LevelRange.y) ? this.m_taskPlayerLevel : this.m_task.LevelRange.y;
				float adventuringTaskExperience = GlobalSettings.Values.Progression.GetAdventuringTaskExperience(num, this.m_taskPlayerLevel);
				float arg = adventuringTaskExperience / totalForLevel * 100f;
				float rewardForLevel = GlobalSettings.Values.Progression.AdventuringLevelCurve.GetRewardForLevel(num);
				float arg2 = adventuringTaskExperience / rewardForLevel;
				Debug.Log(ZString.Format<int, int, float, BBTaskType, float, float, float, float>("[PlayerLevel: {0}, TaskLevel: {1}] {2}% {3} XP ({4} TaskXP / {5} Total XP), KillCountWorth: {6} ({7})", this.m_taskPlayerLevel, num, arg, this.m_task.Type, adventuringTaskExperience, totalForLevel, arg2, rewardForLevel));
			}
		}

		// Token: 0x04005AE4 RID: 23268
		private const int kMaxLevel = 50;

		// Token: 0x04005AE5 RID: 23269
		private const string kLevelCurveGroup = "Level Curve";

		// Token: 0x04005AE6 RID: 23270
		private const string kKillTableGroup = "Kill Table";

		// Token: 0x04005AE7 RID: 23271
		private const string kIndividualGroup = "Individual XP";

		// Token: 0x04005AE8 RID: 23272
		[SerializeField]
		private SuccessRewardCurve m_levelCurve;

		// Token: 0x04005AE9 RID: 23273
		[Range(-5f, 5f)]
		[SerializeField]
		private int m_levelDelta;

		// Token: 0x04005AEA RID: 23274
		[SerializeField]
		private bool m_bypassChevronMultiplier;

		// Token: 0x04005AEB RID: 23275
		[SerializeField]
		private ChallengeRating m_killTableCR = ChallengeRating.CR1;

		// Token: 0x04005AEC RID: 23276
		[Range(1f, 50f)]
		[SerializeField]
		private float m_sourceLevel = 1f;

		// Token: 0x04005AED RID: 23277
		[Range(1f, 50f)]
		[SerializeField]
		private int m_targetLevel = 5;

		// Token: 0x04005AEE RID: 23278
		private const string kConstantXp = "Constant XP";

		// Token: 0x04005AEF RID: 23279
		[SerializeField]
		private int m_experience = 1000;

		// Token: 0x04005AF0 RID: 23280
		[Range(1f, 6f)]
		[SerializeField]
		private int m_groupSize = 1;

		// Token: 0x04005AF1 RID: 23281
		[SerializeField]
		private ChallengeRating m_challengeRating = ChallengeRating.CR1;

		// Token: 0x04005AF2 RID: 23282
		private const string kTaskXp = "Task XP";

		// Token: 0x04005AF3 RID: 23283
		[SerializeField]
		private BBTask m_task;

		// Token: 0x04005AF4 RID: 23284
		[Range(1f, 50f)]
		[SerializeField]
		private int m_taskPlayerLevel = 10;
	}
}
