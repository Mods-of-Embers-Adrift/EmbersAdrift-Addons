using System;
using SoL.Game.EffectSystem;
using SoL.Game.NPCs;
using SoL.Game.Settings;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005EE RID: 1518
	public class NpcVitals_Server : ServerVitals
	{
		// Token: 0x17000A2D RID: 2605
		// (get) Token: 0x06002FF8 RID: 12280 RVA: 0x000610F4 File Offset: 0x0005F2F4
		// (set) Token: 0x06002FF9 RID: 12281 RVA: 0x000610FC File Offset: 0x0005F2FC
		public HealthThresholdActions ThresholdActions { get; set; }

		// Token: 0x17000A2E RID: 2606
		// (get) Token: 0x06002FFA RID: 12282 RVA: 0x00061105 File Offset: 0x0005F305
		public override bool ArmorScalesWithPlayerLevel
		{
			get
			{
				return this.m_armorScalesWithPlayerLevel;
			}
		}

		// Token: 0x06002FFB RID: 12283 RVA: 0x0006110D File Offset: 0x0005F30D
		public override int GetScaledArmorClass(int playerLevel)
		{
			if (!this.m_armorScalesWithPlayerLevel || !(GlobalSettings.Values != null) || GlobalSettings.Values.Npcs == null)
			{
				return this.ArmorClassInternal;
			}
			return GlobalSettings.Values.Npcs.GetScaledArmorClass(playerLevel);
		}

		// Token: 0x17000A2F RID: 2607
		// (get) Token: 0x06002FFC RID: 12284 RVA: 0x00061147 File Offset: 0x0005F347
		private VitalsReplicatorNpc NpcReplicator
		{
			get
			{
				if (!this.m_npcReplicator)
				{
					this.m_npcReplicator = (base.m_replicator as VitalsReplicatorNpc);
				}
				return this.m_npcReplicator;
			}
		}

		// Token: 0x06002FFD RID: 12285 RVA: 0x0006116D File Offset: 0x0005F36D
		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		// Token: 0x06002FFE RID: 12286 RVA: 0x00061175 File Offset: 0x0005F375
		public override int GetHaste()
		{
			if (this.m_cachedHaste == null)
			{
				this.m_cachedHaste = new int?(base.GetStatusEffectValue(StatType.Haste));
			}
			return this.m_cachedHaste.Value;
		}

		// Token: 0x06002FFF RID: 12287 RVA: 0x000611A2 File Offset: 0x0005F3A2
		protected override void UpdateCachedHaste()
		{
			this.m_cachedHaste = new int?(base.GetStatusEffectValue(StatType.Haste));
		}

		// Token: 0x17000A30 RID: 2608
		// (get) Token: 0x06003000 RID: 12288 RVA: 0x000611B7 File Offset: 0x0005F3B7
		// (set) Token: 0x06003001 RID: 12289 RVA: 0x0004475B File Offset: 0x0004295B
		public override Stance Stance
		{
			get
			{
				if (!this.m_npcTargetController || this.m_npcTargetController.HostileTargetCount <= 0)
				{
					return Stance.Idle;
				}
				return Stance.Combat;
			}
			set
			{
			}
		}

		// Token: 0x06003002 RID: 12290 RVA: 0x000611D7 File Offset: 0x0005F3D7
		public override float GetHealthPercent()
		{
			return this.HealthInternal / (float)this.m_maxHealthInternal;
		}

		// Token: 0x06003003 RID: 12291 RVA: 0x000611E7 File Offset: 0x0005F3E7
		public override float GetArmorClassPercent()
		{
			if (this.m_maxArmorClassInternal <= 0)
			{
				return 0f;
			}
			return (float)this.ArmorClassInternal / (float)this.m_maxArmorClassInternal;
		}

		// Token: 0x17000A31 RID: 2609
		// (get) Token: 0x06003004 RID: 12292 RVA: 0x00061207 File Offset: 0x0005F407
		public override float Health
		{
			get
			{
				return this.HealthInternal;
			}
		}

		// Token: 0x17000A32 RID: 2610
		// (get) Token: 0x06003005 RID: 12293 RVA: 0x0006108D File Offset: 0x0005F28D
		public override float HealthWound
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17000A33 RID: 2611
		// (get) Token: 0x06003006 RID: 12294 RVA: 0x0006120F File Offset: 0x0005F40F
		public override int MaxHealth
		{
			get
			{
				return this.m_maxHealthInternal;
			}
		}

		// Token: 0x17000A34 RID: 2612
		// (get) Token: 0x06003007 RID: 12295 RVA: 0x0006109C File Offset: 0x0005F29C
		public override float Stamina
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x17000A35 RID: 2613
		// (get) Token: 0x06003008 RID: 12296 RVA: 0x0006108D File Offset: 0x0005F28D
		public override float StaminaWound
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17000A36 RID: 2614
		// (get) Token: 0x06003009 RID: 12297 RVA: 0x00061217 File Offset: 0x0005F417
		public override int ArmorClass
		{
			get
			{
				return this.ArmorClassInternal;
			}
		}

		// Token: 0x17000A37 RID: 2615
		// (get) Token: 0x0600300A RID: 12298 RVA: 0x0006121F File Offset: 0x0005F41F
		public override int MaxArmorClass
		{
			get
			{
				return this.m_maxArmorClassInternal;
			}
		}

		// Token: 0x17000A38 RID: 2616
		// (get) Token: 0x0600300B RID: 12299 RVA: 0x00061227 File Offset: 0x0005F427
		protected override float CombatRecoveryTime
		{
			get
			{
				return 10f;
			}
		}

		// Token: 0x0600300C RID: 12300 RVA: 0x0006122E File Offset: 0x0005F42E
		public override float GetMaxAbsorbDamageReduction()
		{
			return this.m_maxAbsorbDamageReduction;
		}

		// Token: 0x17000A39 RID: 2617
		// (get) Token: 0x0600300D RID: 12301 RVA: 0x00061236 File Offset: 0x0005F436
		// (set) Token: 0x0600300E RID: 12302 RVA: 0x0015849C File Offset: 0x0015669C
		private int ArmorClassInternal
		{
			get
			{
				return this.m_armorClassInternal;
			}
			set
			{
				this.m_armorClassInternal = value;
				float num = ((float)this.m_maxArmorClassInternal > 0f) ? ((float)this.m_armorClassInternal / (float)this.m_maxArmorClassInternal) : 0f;
				this.NpcReplicator.ArmorClassPercent.Value = (byte)Mathf.CeilToInt(num * 100f);
			}
		}

		// Token: 0x0600300F RID: 12303 RVA: 0x001584F4 File Offset: 0x001566F4
		public override void AbsorbDamage(float delta)
		{
			if (this.m_preventArmorDegradation)
			{
				return;
			}
			if (delta == 0f)
			{
				return;
			}
			if (delta > 0f)
			{
				if (this.m_armorClassAbsorptionModifier <= 0f)
				{
					return;
				}
				delta *= this.m_armorClassAbsorptionModifier;
			}
			base.AbsorbDamage(delta);
			this.m_totalAbsorbed += delta;
			float num = this.m_totalAbsorbed / (float)this.m_maxDamageAbsorption;
			float f = Mathf.Clamp((float)this.m_maxArmorClassInternal * (1f - num), 0f, (float)this.m_maxArmorClassInternal);
			this.ArmorClassInternal = Mathf.CeilToInt(f);
		}

		// Token: 0x17000A3A RID: 2618
		// (get) Token: 0x06003010 RID: 12304 RVA: 0x0006123E File Offset: 0x0005F43E
		// (set) Token: 0x06003011 RID: 12305 RVA: 0x00158584 File Offset: 0x00156784
		private float HealthInternal
		{
			get
			{
				return this.m_healthInternal;
			}
			set
			{
				this.m_healthInternal = value;
				byte b = (byte)Mathf.FloorToInt(this.m_healthInternal / (float)this.m_maxHealthInternal * 100f);
				if (b == 0 && this.m_healthInternal > 0f)
				{
					b = 1;
				}
				this.NpcReplicator.HealthPercent.Value = b;
			}
		}

		// Token: 0x06003012 RID: 12306 RVA: 0x001585D8 File Offset: 0x001567D8
		public override void AlterHealth(float delta)
		{
			if (delta == 0f)
			{
				return;
			}
			if (delta < 0f)
			{
				if (this.m_healthReductionModifier <= 0f)
				{
					return;
				}
				delta *= this.m_healthReductionModifier;
			}
			float healthPercent = this.GetHealthPercent();
			this.HealthInternal = Mathf.Clamp(this.HealthInternal + delta, 0f, (float)this.m_maxHealthInternal);
			if (!this.m_preventDeath && this.HealthInternal <= 0f && this.NpcReplicator.CurrentHealthState.Value != HealthState.Dead)
			{
				if (base.CheckResilience())
				{
					this.HealthInternal = base.ResilienceRoll();
					return;
				}
				this.NpcReplicator.CurrentHealthState.Value = HealthState.Dead;
				if (base.GameEntity.TargetController != null)
				{
					base.GameEntity.TargetController.EntityDied();
				}
				if (base.GameEntity.EffectController != null)
				{
					base.GameEntity.EffectController.RemoveEffectsForDeath();
					return;
				}
			}
			else if (this.ThresholdActions)
			{
				this.ThresholdActions.ExecuteActions(base.GameEntity, healthPercent, this.GetHealthPercent());
			}
		}

		// Token: 0x06003013 RID: 12307 RVA: 0x00061246 File Offset: 0x0005F446
		public void SetMaxValues(int maxHealth, int maxArmorClass, int maxDamageAbsorption, float maxAbsorbDamageReduction)
		{
			this.m_maxHealthInternal = maxHealth;
			this.m_maxArmorClassInternal = maxArmorClass;
			this.m_maxDamageAbsorption = maxDamageAbsorption;
			this.HealthInternal = (float)this.m_maxHealthInternal;
			this.ArmorClassInternal = this.m_maxArmorClassInternal;
			this.m_maxAbsorbDamageReduction = maxAbsorbDamageReduction;
		}

		// Token: 0x06003014 RID: 12308 RVA: 0x001586F4 File Offset: 0x001568F4
		public void ApplyStatModifiers(StatModifier[] modifiers)
		{
			if (modifiers == null || modifiers.Length == 0)
			{
				return;
			}
			for (int i = 0; i < modifiers.Length; i++)
			{
				if (modifiers[i] != null)
				{
					modifiers[i].AddValue(this.m_baseStats);
				}
			}
		}

		// Token: 0x06003015 RID: 12309 RVA: 0x0015872C File Offset: 0x0015692C
		protected override void InitInternal()
		{
			base.InitInternal();
			this.ArmorClassInternal = this.m_maxArmorClassInternal;
			this.HealthInternal = (float)this.m_maxHealthInternal;
			if (base.GameEntity.TargetController)
			{
				this.m_npcTargetController = (base.GameEntity.TargetController as NpcTargetController);
			}
		}

		// Token: 0x06003016 RID: 12310 RVA: 0x00158780 File Offset: 0x00156980
		protected override void UpdateHealth()
		{
			if (this.HealthInternal >= (float)this.MaxHealth && this.ArmorClassInternal >= this.MaxArmorClass)
			{
				return;
			}
			if (this.NpcReplicator.CurrentHealthState.Value != HealthState.Alive)
			{
				return;
			}
			bool flag;
			if (this.m_bypassTargetCount)
			{
				flag = (this.m_recoveryFraction < 1f);
			}
			else
			{
				flag = (base.GameEntity && base.GameEntity.TargetController && base.GameEntity.TargetController.NTargets != null && base.GameEntity.TargetController.NTargets.Value > 0);
			}
			float regenFraction = GlobalSettings.Values.Npcs.GetRegenFraction(flag, this.m_recoveryFraction);
			float num = (float)this.MaxArmorClass * regenFraction;
			float num2 = (float)this.MaxHealth * regenFraction;
			float healthRegenBonusPercent = base.GetHealthRegenBonusPercent();
			num2 = num2.PercentModification(healthRegenBonusPercent);
			if (this.m_lastApplicator)
			{
				num2 += this.m_lastApplicator.HealthRegenRatePerSecond * this.m_recoveryFraction;
			}
			if (base.GameEntity.ServerNpcController && base.GameEntity.ServerNpcController.MovementMode == NpcMovementMode.Reset)
			{
				num2 += GlobalSettings.Values.Npcs.ResetRegenRate;
				num += GlobalSettings.Values.Npcs.ResetRegenRate;
			}
			else if (this.Stance.ApplyFullyRestedBonus())
			{
				num2 += GlobalSettings.Values.Combat.GetFullyRestedBonus(this.m_timeFullyRested, healthRegenBonusPercent);
			}
			this.AlterHealth(num2 * Time.deltaTime);
			if (!flag && this.ArmorClassInternal < this.MaxArmorClass)
			{
				int num3 = Mathf.Clamp(Mathf.CeilToInt(num * Time.deltaTime), 1, int.MaxValue);
				this.ArmorClassInternal = Mathf.Clamp(this.ArmorClassInternal + num3, 0, this.MaxArmorClass);
				this.m_totalAbsorbed = (float)this.m_maxDamageAbsorption * (1f - (float)this.ArmorClassInternal / (float)this.MaxArmorClass);
			}
		}

		// Token: 0x06003017 RID: 12311 RVA: 0x0006127E File Offset: 0x0005F47E
		protected override void Subscribe()
		{
			base.Subscribe();
			base.GameEntity.EffectController != null;
		}

		// Token: 0x06003018 RID: 12312 RVA: 0x00061298 File Offset: 0x0005F498
		protected override void Unsubscribe()
		{
			base.Unsubscribe();
			base.GameEntity.EffectController != null;
		}

		// Token: 0x04002EDB RID: 11995
		[Tooltip("If TRUE then the hasTargets variable is based on the recoveryFraction (recoveryFraction < 1 = hasTargets)")]
		[SerializeField]
		private bool m_bypassTargetCount;

		// Token: 0x04002EDC RID: 11996
		[SerializeField]
		private bool m_preventDeath;

		// Token: 0x04002EDD RID: 11997
		[SerializeField]
		private bool m_preventArmorDegradation;

		// Token: 0x04002EDE RID: 11998
		[SerializeField]
		private bool m_armorScalesWithPlayerLevel;

		// Token: 0x04002EDF RID: 11999
		[SerializeField]
		private float m_healthReductionModifier = 1f;

		// Token: 0x04002EE0 RID: 12000
		[SerializeField]
		private float m_armorClassAbsorptionModifier = 1f;

		// Token: 0x04002EE2 RID: 12002
		private NpcTargetController m_npcTargetController;

		// Token: 0x04002EE3 RID: 12003
		private int m_maxDamageAbsorption = 100;

		// Token: 0x04002EE4 RID: 12004
		private VitalsReplicatorNpc m_npcReplicator;

		// Token: 0x04002EE5 RID: 12005
		private int? m_cachedHaste;

		// Token: 0x04002EE6 RID: 12006
		private float m_maxAbsorbDamageReduction = 0.8f;

		// Token: 0x04002EE7 RID: 12007
		private float m_totalAbsorbed;

		// Token: 0x04002EE8 RID: 12008
		private int m_maxArmorClassInternal = 10;

		// Token: 0x04002EE9 RID: 12009
		private int m_armorClassInternal = 10;

		// Token: 0x04002EEA RID: 12010
		private int m_maxHealthInternal = 25;

		// Token: 0x04002EEB RID: 12011
		private float m_healthInternal;
	}
}
