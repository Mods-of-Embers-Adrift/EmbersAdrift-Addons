using System;
using SoL.Game.Settings;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002DF RID: 735
	public static class SolMath
	{
		// Token: 0x0600152B RID: 5419 RVA: 0x000FC0EC File Offset: 0x000FA2EC
		public static float Gaussian()
		{
			float num;
			float num3;
			do
			{
				num = 2f * UnityEngine.Random.Range(0f, 1f) - 1f;
				float num2 = 2f * UnityEngine.Random.Range(0f, 1f) - 1f;
				num3 = num * num + num2 * num2;
			}
			while (num3 >= 1f);
			num3 = Mathf.Sqrt(-2f * Mathf.Log(num3) / num3);
			return num * num3 * 1f;
		}

		// Token: 0x0600152C RID: 5420 RVA: 0x00050D9C File Offset: 0x0004EF9C
		public static float GetAbsorption(float roll, float ac)
		{
			if (roll <= ac)
			{
				return 1f;
			}
			return ac / roll;
		}

		// Token: 0x0600152D RID: 5421 RVA: 0x00050DAB File Offset: 0x0004EFAB
		public static float GetDamageReduction(float armor, float attackerLevel, float maxReduction)
		{
			return Mathf.Clamp(1.5f * armor / (GlobalSettings.Values.Combat.DamageResistBase + GlobalSettings.Values.Combat.DamageResistPerLevel * attackerLevel), 0f, maxReduction);
		}

		// Token: 0x04001D63 RID: 7523
		public const float kSigma = 1f;

		// Token: 0x04001D64 RID: 7524
		public const float kMaximumGaussianMeanShift = 1f;
	}
}
