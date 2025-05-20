using System;
using System.Collections.Generic;
using SoL.Game.EffectSystem;
using SoL.Game.Settings;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005F6 RID: 1526
	public abstract class ServerVitals : Vitals
	{
		// Token: 0x06003072 RID: 12402 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void UpdateArmorAugments()
		{
		}

		// Token: 0x06003073 RID: 12403 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void UpdateWounds()
		{
		}

		// Token: 0x06003074 RID: 12404 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void UpdateHealth()
		{
		}

		// Token: 0x06003075 RID: 12405 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void UpdateStamina()
		{
		}

		// Token: 0x06003076 RID: 12406 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void UpdateHealthState()
		{
		}

		// Token: 0x17000A52 RID: 2642
		// (get) Token: 0x06003077 RID: 12407
		protected abstract float CombatRecoveryTime { get; }

		// Token: 0x06003078 RID: 12408 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void UpdateSpawnNoTarget()
		{
		}

		// Token: 0x06003079 RID: 12409 RVA: 0x0015A304 File Offset: 0x00158504
		protected virtual void UpdateCombatState()
		{
			float num = Mathf.Clamp01((float)(DateTime.UtcNow - this.m_lastCombatTimestamp).TotalSeconds / this.CombatRecoveryTime);
			if (num <= 0f)
			{
				this.m_recoveryFraction = 0f;
				return;
			}
			if (num >= 1f)
			{
				this.m_recoveryFraction = 1f;
				return;
			}
			this.m_recoveryFraction = GlobalSettings.Values.Combat.GetCombatRecovery(num);
		}

		// Token: 0x0600307A RID: 12410 RVA: 0x00061855 File Offset: 0x0005FA55
		protected virtual bool ShouldResetFullyRestedTimer()
		{
			return this.Stance.ResetFullyRestedState() || this.m_recoveryFraction < 1f;
		}

		// Token: 0x0600307B RID: 12411 RVA: 0x00061873 File Offset: 0x0005FA73
		private void UpdateFullyRestedState()
		{
			if (this.ShouldResetFullyRestedTimer())
			{
				this.m_timeFullyRested = 0f;
				return;
			}
			this.m_timeFullyRested += Time.deltaTime * this.Stance.GetStanceProfile().FullyRestedMultiplier;
		}

		// Token: 0x0600307C RID: 12412 RVA: 0x0015A378 File Offset: 0x00158578
		private void Update()
		{
			if (!this.m_initialized || this.m_finalized)
			{
				return;
			}
			this.UpdateCombatState();
			this.UpdateFullyRestedState();
			this.UpdateWounds();
			this.UpdateHealth();
			this.UpdateStamina();
			this.UpdateHealthState();
			this.UpdateArmorAugments();
			this.UpdateSpawnNoTarget();
		}

		// Token: 0x17000A53 RID: 2643
		// (get) Token: 0x0600307D RID: 12413 RVA: 0x000618AC File Offset: 0x0005FAAC
		private List<CampfireEffectApplicator> m_effectApplicators
		{
			get
			{
				if (this._effectApplicators == null)
				{
					this._effectApplicators = new List<CampfireEffectApplicator>();
				}
				return this._effectApplicators;
			}
		}

		// Token: 0x0600307E RID: 12414 RVA: 0x0015A3C8 File Offset: 0x001585C8
		public override void AddCampfireEffect(CampfireEffectApplicator applicator)
		{
			base.AddCampfireEffect(applicator);
			if (!this.m_effectApplicators.Contains(applicator))
			{
				this.m_effectApplicators.Add(applicator);
				this.m_lastApplicator = applicator;
				if (base.GameEntity.Type == GameEntityType.Player)
				{
					base.GameEntity.CharacterData.CharacterFlags.Value |= PlayerFlags.InCampfire;
				}
			}
		}

		// Token: 0x0600307F RID: 12415 RVA: 0x0015A428 File Offset: 0x00158628
		public override void RemoveCampfireEffect(CampfireEffectApplicator applicator)
		{
			base.RemoveCampfireEffect(applicator);
			if (this.m_effectApplicators.Remove(applicator))
			{
				if (this.m_lastApplicator == applicator)
				{
					this.m_lastApplicator = ((this.m_effectApplicators.Count > 0) ? this.m_effectApplicators[this.m_effectApplicators.Count - 1] : null);
				}
				if (this.m_lastApplicator == null && base.GameEntity.Type == GameEntityType.Player)
				{
					base.GameEntity.CharacterData.CharacterFlags.Value &= ~PlayerFlags.InCampfire;
				}
			}
		}

		// Token: 0x06003080 RID: 12416 RVA: 0x0015A4C4 File Offset: 0x001586C4
		protected bool CheckResilience()
		{
			int num = 0;
			if (base.GameEntity)
			{
				GameEntity gameEntity = base.GameEntity;
				StatType statType = StatType.Resilience;
				StatSettings.DiminishingCurveCollection diminishingCurves = GlobalSettings.Values.Stats.DiminishingCurves;
				num = gameEntity.GetDiminishedStatAsInt(statType, (diminishingCurves != null) ? diminishingCurves.Resilience : null, null, false, 0, 0f);
			}
			return num > 0 && UnityEngine.Random.Range(1, 101) <= num;
		}

		// Token: 0x06003081 RID: 12417 RVA: 0x000618C7 File Offset: 0x0005FAC7
		protected float ResilienceRoll()
		{
			return UnityEngine.Random.Range(2f, 5f);
		}

		// Token: 0x06003082 RID: 12418 RVA: 0x000618D8 File Offset: 0x0005FAD8
		protected float GetHealthRegenBonusPercent()
		{
			return (float)base.GetStatusEffectValue(StatType.RegenHealth) * 0.01f;
		}

		// Token: 0x06003083 RID: 12419 RVA: 0x000618E9 File Offset: 0x0005FAE9
		protected float GetStaminaRegenBonusPercent()
		{
			return (float)base.GetStatusEffectValue(StatType.RegenStamina) * 0.01f;
		}

		// Token: 0x04002F10 RID: 12048
		protected CampfireEffectApplicator m_lastApplicator;

		// Token: 0x04002F11 RID: 12049
		private List<CampfireEffectApplicator> _effectApplicators;

		// Token: 0x04002F12 RID: 12050
		protected float m_recoveryFraction = 1f;

		// Token: 0x04002F13 RID: 12051
		protected float m_timeFullyRested;
	}
}
