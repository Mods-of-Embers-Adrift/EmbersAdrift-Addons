using System;
using SoL.Game.Animation;
using SoL.Game.Settings;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005D7 RID: 1495
	[CreateAssetMenu(menuName = "SoL/Profiles/Stance")]
	public class StanceProfile : ScriptableObject
	{
		// Token: 0x17000A0E RID: 2574
		// (get) Token: 0x06002F7F RID: 12159 RVA: 0x00060CA1 File Offset: 0x0005EEA1
		public float DetectionMultiplier
		{
			get
			{
				return this.m_detectionMultiplier;
			}
		}

		// Token: 0x17000A0F RID: 2575
		// (get) Token: 0x06002F80 RID: 12160 RVA: 0x00060CA9 File Offset: 0x0005EEA9
		public float FullyRestedMultiplier
		{
			get
			{
				return this.m_fullyRestedMultiplier;
			}
		}

		// Token: 0x17000A10 RID: 2576
		// (get) Token: 0x06002F81 RID: 12161 RVA: 0x00060CB1 File Offset: 0x0005EEB1
		public virtual AnimancerAnimationSet AnimationSet
		{
			get
			{
				return this.m_animationSet;
			}
		}

		// Token: 0x06002F82 RID: 12162 RVA: 0x0015742C File Offset: 0x0015562C
		public float GetHealthRegenRate(float recoveryFraction)
		{
			float result;
			if (recoveryFraction >= 1f)
			{
				result = this.m_outOfCombat.HealthRegenRate;
			}
			else if (recoveryFraction <= 0f)
			{
				result = this.m_inCombat.HealthRegenRate;
			}
			else
			{
				result = Mathf.Lerp(this.m_inCombat.HealthRegenRate, this.m_outOfCombat.HealthRegenRate, recoveryFraction);
			}
			return result;
		}

		// Token: 0x06002F83 RID: 12163 RVA: 0x0015748C File Offset: 0x0015568C
		public float GetStaminaRegenMultiplier(float recoveryFraction)
		{
			float result;
			if (recoveryFraction >= 1f)
			{
				result = this.m_outOfCombat.StaminaMultiplier;
			}
			else if (recoveryFraction <= 0f)
			{
				result = this.m_inCombat.StaminaMultiplier;
			}
			else
			{
				result = Mathf.Lerp(this.m_inCombat.StaminaMultiplier, this.m_outOfCombat.StaminaMultiplier, recoveryFraction);
			}
			return result;
		}

		// Token: 0x04002E86 RID: 11910
		private const string kRegenGroup = "Regen";

		// Token: 0x04002E87 RID: 11911
		private static int[] kHealthValues = new int[]
		{
			1,
			10,
			20,
			30,
			40,
			50,
			100
		};

		// Token: 0x04002E88 RID: 11912
		[SerializeField]
		private StanceProfile.RegenData m_outOfCombat;

		// Token: 0x04002E89 RID: 11913
		[SerializeField]
		private StanceProfile.RegenData m_inCombat;

		// Token: 0x04002E8A RID: 11914
		[SerializeField]
		private DummyClass m_dummy;

		// Token: 0x04002E8B RID: 11915
		public float HealthRegenRate;

		// Token: 0x04002E8C RID: 11916
		[Range(0f, 2f)]
		public float StaminaRegenMultiplier;

		// Token: 0x04002E8D RID: 11917
		[Range(-1f, 1f)]
		public float MovementModifier = 1f;

		// Token: 0x04002E8E RID: 11918
		[Range(0f, 2f)]
		[SerializeField]
		private float m_detectionMultiplier = 1f;

		// Token: 0x04002E8F RID: 11919
		[Range(0f, 1f)]
		[SerializeField]
		private float m_fullyRestedMultiplier;

		// Token: 0x04002E90 RID: 11920
		[SerializeField]
		private AnimancerAnimationSet m_animationSet;

		// Token: 0x04002E91 RID: 11921
		public bool CanManuallyExit = true;

		// Token: 0x020005D8 RID: 1496
		[Serializable]
		public class RegenData
		{
			// Token: 0x17000A11 RID: 2577
			// (get) Token: 0x06002F86 RID: 12166 RVA: 0x00060CF6 File Offset: 0x0005EEF6
			public float HealthRegenRate
			{
				get
				{
					return this.m_healthRegenRate;
				}
			}

			// Token: 0x17000A12 RID: 2578
			// (get) Token: 0x06002F87 RID: 12167 RVA: 0x00060CFE File Offset: 0x0005EEFE
			public float StaminaMultiplier
			{
				get
				{
					return this.m_staminaMultiplier;
				}
			}

			// Token: 0x06002F88 RID: 12168 RVA: 0x00060D06 File Offset: 0x0005EF06
			public RegenData(float healthRegenRate, float staminaMultiplier)
			{
				this.m_healthRegenRate = healthRegenRate;
				this.m_staminaMultiplier = staminaMultiplier;
			}

			// Token: 0x06002F89 RID: 12169 RVA: 0x001574EC File Offset: 0x001556EC
			private string GetHealthRegenDescription()
			{
				if (this.HealthRegenRate <= 0f)
				{
					return "No Regen";
				}
				string text = string.Empty;
				for (int i = 0; i < StanceProfile.kHealthValues.Length; i++)
				{
					float value = (float)StanceProfile.kHealthValues[i] / this.HealthRegenRate;
					text = string.Concat(new string[]
					{
						text,
						StanceProfile.kHealthValues[i].ToString(),
						" health takes ",
						value.FormattedTime(1),
						" to regenerate"
					});
					if (i < StanceProfile.kHealthValues.Length - 1)
					{
						text += "\n";
					}
				}
				return text;
			}

			// Token: 0x06002F8A RID: 12170 RVA: 0x0015758C File Offset: 0x0015578C
			private string GetStaminaRegenDescription()
			{
				if (this.StaminaMultiplier <= 0f)
				{
					return "No Regen";
				}
				string text = string.Empty;
				float num = this.StaminaMultiplier * (100f / GlobalSettings.Values.Player.StaminaRecoveryTime);
				for (int i = 0; i < StanceProfile.kHealthValues.Length; i++)
				{
					float value = (float)StanceProfile.kHealthValues[i] / num;
					text = string.Concat(new string[]
					{
						text,
						value.FormattedTime(1),
						" to regenerate ",
						StanceProfile.kHealthValues[i].ToString(),
						" stamina"
					});
					if (i < StanceProfile.kHealthValues.Length - 1)
					{
						text += "\n";
					}
				}
				return text;
			}

			// Token: 0x04002E92 RID: 11922
			[SerializeField]
			private float m_healthRegenRate;

			// Token: 0x04002E93 RID: 11923
			[SerializeField]
			private DummyClass m_healthRegenDummy;

			// Token: 0x04002E94 RID: 11924
			[Range(0f, 2f)]
			[SerializeField]
			private float m_staminaMultiplier;

			// Token: 0x04002E95 RID: 11925
			[SerializeField]
			private DummyClass m_staminaRegenDummy;
		}
	}
}
