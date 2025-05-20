using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x02000720 RID: 1824
	[Serializable]
	public class CombatSettings
	{
		// Token: 0x17000C2F RID: 3119
		// (get) Token: 0x060036CE RID: 14030 RVA: 0x0006583D File Offset: 0x00063A3D
		public ReactionAbility PlayerRiposte
		{
			get
			{
				return this.m_playerRiposte;
			}
		}

		// Token: 0x17000C30 RID: 3120
		// (get) Token: 0x060036CF RID: 14031 RVA: 0x00065845 File Offset: 0x00063A45
		public ReactionAbility NpcRiposte
		{
			get
			{
				return this.m_npcRiposte;
			}
		}

		// Token: 0x060036D0 RID: 14032 RVA: 0x0006584D File Offset: 0x00063A4D
		public float GetHealthFractionBonus(float healthFraction)
		{
			return this.m_healthFractionBonusCurve.Evaluate(healthFraction);
		}

		// Token: 0x17000C31 RID: 3121
		// (get) Token: 0x060036D1 RID: 14033 RVA: 0x0006585B File Offset: 0x00063A5B
		public float DamageResistBase
		{
			get
			{
				return this.m_damageResistBase;
			}
		}

		// Token: 0x17000C32 RID: 3122
		// (get) Token: 0x060036D2 RID: 14034 RVA: 0x00065863 File Offset: 0x00063A63
		public float DamageResistPerLevel
		{
			get
			{
				return this.m_damageResistPerLevel;
			}
		}

		// Token: 0x060036D3 RID: 14035 RVA: 0x0006586B File Offset: 0x00063A6B
		public float GetLevelDeltaMaxResist()
		{
			return this.m_levelDeltaMaxResist / 0.8f;
		}

		// Token: 0x060036D4 RID: 14036 RVA: 0x00065879 File Offset: 0x00063A79
		public float GetNpcHigherLevelResist(int levelDelta)
		{
			return this.m_npcHigherLevelResists.Evaluate((float)levelDelta);
		}

		// Token: 0x17000C33 RID: 3123
		// (get) Token: 0x060036D5 RID: 14037 RVA: 0x00065888 File Offset: 0x00063A88
		public float CoyoteTimeDistance
		{
			get
			{
				return this.m_coyoteTimeDistance;
			}
		}

		// Token: 0x17000C34 RID: 3124
		// (get) Token: 0x060036D6 RID: 14038 RVA: 0x00065890 File Offset: 0x00063A90
		public float CoyoteTimeAngle
		{
			get
			{
				return this.m_coyoteTimeAngle;
			}
		}

		// Token: 0x060036D7 RID: 14039 RVA: 0x00065898 File Offset: 0x00063A98
		public float GetCombatRecovery(float recoveryPercent)
		{
			return this.m_combatRecoveryCurve.Evaluate(recoveryPercent);
		}

		// Token: 0x060036D8 RID: 14040 RVA: 0x0016A9A8 File Offset: 0x00168BA8
		private void GenerateCombatRecoveryCurve()
		{
			this.m_combatRecoveryCurve = new AnimationCurve();
			for (int i = 0; i <= this.m_combatRecoveryResolution; i++)
			{
				float num = (float)i / (float)this.m_combatRecoveryResolution;
				float value = Mathf.Pow(num, this.m_combatRecoveryExponent);
				this.m_combatRecoveryCurve.AddKey(num, value);
			}
		}

		// Token: 0x060036D9 RID: 14041 RVA: 0x0016A9F8 File Offset: 0x00168BF8
		public float GetFullyRestedBonus(float timeFullyRested, float healthRegenStatPercent)
		{
			if (timeFullyRested <= 0f || this.m_fullyRestedBonusValue <= 0f)
			{
				return 0f;
			}
			if (healthRegenStatPercent > 0f && this.m_healthRegenBonusCountCurve != null)
			{
				timeFullyRested += this.m_healthRegenBonusCountCurve.Evaluate(healthRegenStatPercent) * (float)this.m_fullyRestedBonusTime;
			}
			int num = Mathf.FloorToInt(timeFullyRested / (float)this.m_fullyRestedBonusTime);
			int num2 = num * this.m_fullyRestedBonusTime;
			float t = (timeFullyRested - (float)num2) / (float)this.m_fullyRestedBonusTime;
			return Mathf.Clamp((float)num * this.m_fullyRestedBonusValue + Mathf.Lerp(0f, this.m_fullyRestedBonusValue, t), 0f, this.m_fullyRestedBonusValueMax);
		}

		// Token: 0x17000C35 RID: 3125
		// (get) Token: 0x060036DA RID: 14042 RVA: 0x000658A6 File Offset: 0x00063AA6
		public int ActiveDefenseCooldown
		{
			get
			{
				return this.m_activeDefenseCooldown;
			}
		}

		// Token: 0x17000C36 RID: 3126
		// (get) Token: 0x060036DB RID: 14043 RVA: 0x000658AE File Offset: 0x00063AAE
		public int ParryBlockValue
		{
			get
			{
				return this.m_parryBlockValue;
			}
		}

		// Token: 0x17000C37 RID: 3127
		// (get) Token: 0x060036DC RID: 14044 RVA: 0x000658B6 File Offset: 0x00063AB6
		public float HitMultiplier
		{
			get
			{
				return this.m_hitMultiplier;
			}
		}

		// Token: 0x17000C38 RID: 3128
		// (get) Token: 0x060036DD RID: 14045 RVA: 0x000658BE File Offset: 0x00063ABE
		public int PvpResistControl
		{
			get
			{
				return this.m_pvpResistControl;
			}
		}

		// Token: 0x17000C39 RID: 3129
		// (get) Token: 0x060036DE RID: 14046 RVA: 0x000658C6 File Offset: 0x00063AC6
		public int PvpResistDamage
		{
			get
			{
				return this.m_pvpResistDamage;
			}
		}

		// Token: 0x17000C3A RID: 3130
		// (get) Token: 0x060036DF RID: 14047 RVA: 0x000658CE File Offset: 0x00063ACE
		public int PvpResistDebuff
		{
			get
			{
				return this.m_pvpResistDebuff;
			}
		}

		// Token: 0x040034F4 RID: 13556
		public AutoAttackAbility AutoAttack;

		// Token: 0x040034F5 RID: 13557
		public AutoAttackHealthFractionAbility AutoAttackHealthFraction;

		// Token: 0x040034F6 RID: 13558
		public WeaponItem FallbackWeapon;

		// Token: 0x040034F7 RID: 13559
		[SerializeField]
		private ReactionAbility m_playerRiposte;

		// Token: 0x040034F8 RID: 13560
		[SerializeField]
		private ReactionAbility m_npcRiposte;

		// Token: 0x040034F9 RID: 13561
		public float HealThreatMultiplier = 2f;

		// Token: 0x040034FA RID: 13562
		[SerializeField]
		private AnimationCurve m_combatRecoveryCurve;

		// Token: 0x040034FB RID: 13563
		[SerializeField]
		private float m_combatRecoveryExponent = 2f;

		// Token: 0x040034FC RID: 13564
		[SerializeField]
		private int m_combatRecoveryResolution = 10;

		// Token: 0x040034FD RID: 13565
		public float CombatRecoveryTime = 30f;

		// Token: 0x040034FE RID: 13566
		public float GlobalCooldown = 1f;

		// Token: 0x040034FF RID: 13567
		public HealthThreatSettings HealThreatSettings;

		// Token: 0x04003500 RID: 13568
		[Tooltip("X-Axis is health fraction.  Y-Axis is bonus %")]
		[SerializeField]
		private AnimationCurve m_healthFractionBonusCurve;

		// Token: 0x04003501 RID: 13569
		[SerializeField]
		private float m_damageResistBase = 200f;

		// Token: 0x04003502 RID: 13570
		[SerializeField]
		private float m_damageResistPerLevel = 10f;

		// Token: 0x04003503 RID: 13571
		[Range(0f, 1f)]
		[SerializeField]
		private float m_levelDeltaMaxResist = 0.3f;

		// Token: 0x04003504 RID: 13572
		private const float kResistRange = 5f;

		// Token: 0x04003505 RID: 13573
		private const float kMaxResist = 0.8f;

		// Token: 0x04003506 RID: 13574
		[SerializeField]
		private AnimationCurve m_npcHigherLevelResists = AnimationCurve.Linear(0f, 0f, 0f, 0f);

		// Token: 0x04003507 RID: 13575
		[SerializeField]
		private float m_coyoteTimeDistance = 2f;

		// Token: 0x04003508 RID: 13576
		[SerializeField]
		private float m_coyoteTimeAngle = 30f;

		// Token: 0x04003509 RID: 13577
		private const string kFullyRestedGroup = "Fully Rested";

		// Token: 0x0400350A RID: 13578
		private const string kHealthBonusSuffix = "health/second";

		// Token: 0x0400350B RID: 13579
		[Tooltip("Time elapsed after leaving combat to apply bonus.Each pass results in another bonus applied. i.e. bonusTime=10s, 20s elapsed would return 2x bonusValue")]
		[SerializeField]
		private int m_fullyRestedBonusTime = 10;

		// Token: 0x0400350C RID: 13580
		[Tooltip("Amount of regen bonus to apply per fullyRestedBonusTime elapsed.")]
		[SerializeField]
		private float m_fullyRestedBonusValue = 1f;

		// Token: 0x0400350D RID: 13581
		[Tooltip("Maximum bonus that can be applied.")]
		[SerializeField]
		private float m_fullyRestedBonusValueMax = 5f;

		// Token: 0x0400350E RID: 13582
		[Tooltip("X-Axis is regen stat in percent (+150% = 1.5)\nY-Axis is N-Bonuses you start with.")]
		[SerializeField]
		private AnimationCurve m_healthRegenBonusCountCurve;

		// Token: 0x0400350F RID: 13583
		[SerializeField]
		private int m_activeDefenseCooldown = 2;

		// Token: 0x04003510 RID: 13584
		[Range(0f, 100f)]
		[SerializeField]
		private int m_parryBlockValue = 10;

		// Token: 0x04003511 RID: 13585
		[Tooltip("+HIT values are multiplied by this value!")]
		[SerializeField]
		private float m_hitMultiplier = 1f;

		// Token: 0x04003512 RID: 13586
		private const string kPvp = "PvP";

		// Token: 0x04003513 RID: 13587
		[Tooltip("CC type effects including movement")]
		[SerializeField]
		private int m_pvpResistControl = 50;

		// Token: 0x04003514 RID: 13588
		[Tooltip("Damage types")]
		[SerializeField]
		private int m_pvpResistDamage = 25;

		// Token: 0x04003515 RID: 13589
		[Tooltip("Debuffs")]
		[SerializeField]
		private int m_pvpResistDebuff = 25;
	}
}
