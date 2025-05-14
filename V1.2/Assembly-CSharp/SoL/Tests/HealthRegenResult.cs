using System;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000DA4 RID: 3492
	[Serializable]
	public class HealthRegenResult
	{
		// Token: 0x060068AA RID: 26794 RVA: 0x0004475B File Offset: 0x0004295B
		public void Init(HealthRegenTestScriptable config)
		{
		}

		// Token: 0x060068AB RID: 26795 RVA: 0x0004475B File Offset: 0x0004295B
		public void Update(float deltaTime)
		{
		}

		// Token: 0x060068AC RID: 26796 RVA: 0x000863A0 File Offset: 0x000845A0
		public bool IsFinishedProcessing()
		{
			return this.m_currentHealth >= (float)this.MaxHealth;
		}

		// Token: 0x060068AD RID: 26797 RVA: 0x00215534 File Offset: 0x00213734
		private float GetFullyRestedBonus(float timeFullyRested, float healthRegenBonusPercent)
		{
			if (this.m_config == null || timeFullyRested <= 0f || this.m_config.FullyRestedBonusValue <= 0f)
			{
				return 0f;
			}
			if (healthRegenBonusPercent > 0f && this.m_config.HealthRegenBonusCountCurve != null)
			{
				timeFullyRested += this.m_config.HealthRegenBonusCountCurve.Evaluate(healthRegenBonusPercent) * (float)this.m_config.FullyRestedBonusTime;
			}
			int num = Mathf.FloorToInt(timeFullyRested / (float)this.m_config.FullyRestedBonusTime);
			int num2 = num * this.m_config.FullyRestedBonusTime;
			float t = (timeFullyRested - (float)num2) / (float)this.m_config.FullyRestedBonusTime;
			return Mathf.Clamp((float)num * this.m_config.FullyRestedBonusValue + Mathf.Lerp(0f, this.m_config.FullyRestedBonusValue, t), 0f, this.m_config.FullyRestedBonusValueMax);
		}

		// Token: 0x04005AF9 RID: 23289
		public SpecializedRole Spec;

		// Token: 0x04005AFA RID: 23290
		public int Level = 50;

		// Token: 0x04005AFB RID: 23291
		public int StartingHealth;

		// Token: 0x04005AFC RID: 23292
		public float TotalTimeElapsed;

		// Token: 0x04005AFD RID: 23293
		public float TimeFullyRested;

		// Token: 0x04005AFE RID: 23294
		public int MaxHealth = 100;

		// Token: 0x04005AFF RID: 23295
		public int RegenStat;

		// Token: 0x04005B00 RID: 23296
		public float BonusValue;

		// Token: 0x04005B01 RID: 23297
		public float TimeHitMaxBonusValue;

		// Token: 0x04005B02 RID: 23298
		[NonSerialized]
		private HealthRegenTestScriptable m_config;

		// Token: 0x04005B03 RID: 23299
		[NonSerialized]
		private float m_currentHealth;

		// Token: 0x04005B04 RID: 23300
		[NonSerialized]
		private float m_combatExitTime;

		// Token: 0x04005B05 RID: 23301
		[NonSerialized]
		private float m_combatExitTimeRemaining;
	}
}
