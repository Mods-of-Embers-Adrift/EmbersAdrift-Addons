using System;
using SoL.Game;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.Tests
{
	// Token: 0x02000DA9 RID: 3497
	[CreateAssetMenu(menuName = "SoL/Tests/Health Regen Data")]
	public class HealthRegenTestScriptable : ScriptableObject
	{
		// Token: 0x04005B26 RID: 23334
		public Stance Stance;

		// Token: 0x04005B27 RID: 23335
		[Range(1f, 50f)]
		public int Level = 50;

		// Token: 0x04005B28 RID: 23336
		[Range(0f, 350f)]
		public int AdditionalRegen;

		// Token: 0x04005B29 RID: 23337
		[FormerlySerializedAs("CombatHealthRegenMultiplier")]
		public float HealthRegenStatRateMultiplier;

		// Token: 0x04005B2A RID: 23338
		public float CombatExitTime;

		// Token: 0x04005B2B RID: 23339
		[Range(0f, 1f)]
		public float StartHealthFraction;

		// Token: 0x04005B2C RID: 23340
		public bool RegenStatAppliesToBase;

		// Token: 0x04005B2D RID: 23341
		private const string kFullyRestedGroup = "Fully Rested";

		// Token: 0x04005B2E RID: 23342
		[Tooltip("Time elapsed after leaving combat to apply bonus.Each pass results in another bonus applied. i.e. bonusTime=10s, 20s elapsed would return 2x bonusValue")]
		public int FullyRestedBonusTime = 10;

		// Token: 0x04005B2F RID: 23343
		[Tooltip("Amount of regen bonus to apply per fullyRestedBonusTime elapsed.")]
		public float FullyRestedBonusValue = 1f;

		// Token: 0x04005B30 RID: 23344
		[Tooltip("Maximum bonus that can be applied.")]
		public float FullyRestedBonusValueMax = 5f;

		// Token: 0x04005B31 RID: 23345
		[Tooltip("X-Axis is regen stat in percent (+150% = 1.5)\nY-Axis is N-Bonuses you start with.")]
		public AnimationCurve HealthRegenBonusCountCurve;

		// Token: 0x04005B32 RID: 23346
		public HealthRegenResult[] RegenResults;
	}
}
