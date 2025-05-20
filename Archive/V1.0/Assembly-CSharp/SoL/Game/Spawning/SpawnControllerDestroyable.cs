using System;
using SoL.Game.EffectSystem;
using SoL.Game.NPCs;
using SoL.Game.Settings;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x020006CB RID: 1739
	public class SpawnControllerDestroyable : SpawnController
	{
		// Token: 0x17000B98 RID: 2968
		// (get) Token: 0x060034DF RID: 13535 RVA: 0x00064359 File Offset: 0x00062559
		protected override bool m_showTargetPopulation
		{
			get
			{
				return !this.m_dynamicTargetPopulation;
			}
		}

		// Token: 0x17000B99 RID: 2969
		// (get) Token: 0x060034E0 RID: 13536 RVA: 0x00166100 File Offset: 0x00164300
		protected override Vector3? m_currentPosition
		{
			get
			{
				if (!this.m_spawnsFollowController)
				{
					return null;
				}
				return new Vector3?(base.gameObject.transform.position);
			}
		}

		// Token: 0x17000B9A RID: 2970
		// (get) Token: 0x060034E1 RID: 13537 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool PerformOccupancyCheck
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000B9B RID: 2971
		// (get) Token: 0x060034E2 RID: 13538 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool DespawnOnDeath
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060034E3 RID: 13539 RVA: 0x00166134 File Offset: 0x00164334
		protected override void Awake()
		{
			base.Awake();
			if (!this.m_gameEntity || !this.m_gameEntity.Vitals)
			{
				Debug.LogError("No GameEntity on SpawnControllerDynamic!");
				base.enabled = false;
				return;
			}
			this.m_gameEntity.SpawnController = this;
		}

		// Token: 0x060034E4 RID: 13540 RVA: 0x00064364 File Offset: 0x00062564
		protected override void Start()
		{
			base.Start();
			this.m_gameEntity.VitalsReplicator.CurrentHealthState.Changed += this.CurrentHealthStateOnChanged;
		}

		// Token: 0x060034E5 RID: 13541 RVA: 0x00166184 File Offset: 0x00164384
		protected override void OnDestroy()
		{
			if (this.m_gameEntity && this.m_gameEntity.VitalsReplicator)
			{
				this.m_gameEntity.VitalsReplicator.CurrentHealthState.Changed -= this.CurrentHealthStateOnChanged;
			}
		}

		// Token: 0x060034E6 RID: 13542 RVA: 0x0006438D File Offset: 0x0006258D
		private void CurrentHealthStateOnChanged(HealthState obj)
		{
			if (obj == HealthState.Dead)
			{
				this.AddChargeToChildren();
				this.m_targetPopulation = 0;
				this.m_isDead = true;
			}
		}

		// Token: 0x060034E7 RID: 13543 RVA: 0x000643A7 File Offset: 0x000625A7
		protected override bool UnregisterSpawnLocationEarly(GameEntity entity)
		{
			if (!this.m_respawnImmediatelyAfterDeath || !entity || !entity.VitalsReplicator)
			{
				return base.UnregisterSpawnLocationEarly(entity);
			}
			return entity.VitalsReplicator.CurrentHealthState.Value != HealthState.Alive;
		}

		// Token: 0x060034E8 RID: 13544 RVA: 0x001661D4 File Offset: 0x001643D4
		protected override int GetTargetPopulation()
		{
			if (!this.m_gameEntity || !this.m_gameEntity.VitalsReplicator || this.m_gameEntity.VitalsReplicator.CurrentHealthState.Value != HealthState.Alive)
			{
				return 0;
			}
			if (!this.m_dynamicTargetPopulation)
			{
				return base.GetTargetPopulation();
			}
			float healthPercent = this.m_gameEntity.Vitals.GetHealthPercent();
			for (int i = 0; i < this.m_targetPopulationThresholds.Length; i++)
			{
				if (healthPercent >= this.m_targetPopulationThresholds[i].HealthPercent)
				{
					return this.m_targetPopulationThresholds[i].TargetPopulation;
				}
			}
			return 0;
		}

		// Token: 0x060034E9 RID: 13545 RVA: 0x000643E4 File Offset: 0x000625E4
		internal override void ReplaceTargetPopulationThresholds(TargetPopulationThreshold[] thresholds)
		{
			base.ReplaceTargetPopulationThresholds(thresholds);
			this.m_targetPopulationThresholds = thresholds;
		}

		// Token: 0x060034EA RID: 13546 RVA: 0x0016626C File Offset: 0x0016446C
		protected override void MidSpawnMonitorCycle()
		{
			base.MidSpawnMonitorCycle();
			if (this.m_gameEntity && this.m_gameEntity.Vitals && this.m_gameEntity.Vitals.GetHealthPercent() >= 1f)
			{
				int targetPopulation = this.GetTargetPopulation();
				int num = this.m_activeSpawns.Count + this.m_respawnTimes.Count;
				if (num > targetPopulation)
				{
					int num2 = 0;
					for (int i = this.m_activeSpawns.Count - 1; i >= 0; i--)
					{
						if (this.m_activeSpawns[i].Entity.NetworkEntity.NObservers <= 0 || !SpawnController.HasPlayersNearby(this.m_activeSpawns[i].Entity))
						{
							this.m_activeSpawns[i].Despawn();
							this.m_activeSpawns.RemoveAt(i);
							num--;
							num2++;
							if (num2 >= 1 || num <= targetPopulation)
							{
								break;
							}
						}
					}
				}
			}
		}

		// Token: 0x060034EB RID: 13547 RVA: 0x00166368 File Offset: 0x00164568
		protected override void AfterSpawnMonitorCycle()
		{
			base.AfterSpawnMonitorCycle();
			if (this.m_gameEntity == null || this.m_gameEntity.Vitals == null)
			{
				return;
			}
			int num = this.m_previousTargetPopulationThresholdIndex;
			float healthPercent = this.m_gameEntity.Vitals.GetHealthPercent();
			for (int i = 0; i < this.m_targetPopulationThresholds.Length; i++)
			{
				if (healthPercent >= this.m_targetPopulationThresholds[i].HealthPercent)
				{
					num = i;
					break;
				}
			}
			if (num != this.m_previousTargetPopulationThresholdIndex)
			{
				this.AddChargeToChildren();
			}
			if ((this.m_summonOnThreshold && num != this.m_previousTargetPopulationThresholdIndex) || (this.m_summonPeriodically && Time.time - this.m_timeOfLastSummon >= (float)this.m_periodicSummonFrequency))
			{
				this.SummonSpawns();
			}
			this.m_previousTargetPopulationThresholdIndex = num;
		}

		// Token: 0x060034EC RID: 13548 RVA: 0x00166428 File Offset: 0x00164628
		private void AddChargeToChildren()
		{
			if (this.m_activeSpawns.Count <= 0 || this.m_gameEntity == null || GlobalSettings.Values.Npcs.GlobalSpeedBuff == null)
			{
				return;
			}
			float num = (this.m_gameEntity.Targetable != null) ? ((float)this.m_gameEntity.Targetable.Level) : 50f;
			ICombatEffectSource globalSpeedBuff = GlobalSettings.Values.Npcs.GlobalSpeedBuff;
			CombatEffect effect = (globalSpeedBuff != null) ? globalSpeedBuff.GetCombatEffect(num, AlchemyPowerLevel.None) : null;
			for (int i = 0; i < this.m_activeSpawns.Count; i++)
			{
				if (this.m_activeSpawns[i].Entity && this.m_activeSpawns[i].Entity.VitalsReplicator && this.m_activeSpawns[i].Entity.VitalsReplicator.CurrentHealthState.Value == HealthState.Alive && this.m_activeSpawns[i].Entity.EffectController)
				{
					this.m_activeSpawns[i].Entity.EffectController.ApplyEffect(this.m_gameEntity, globalSpeedBuff.ArchetypeId, effect, num, num, false, false);
				}
			}
		}

		// Token: 0x060034ED RID: 13549 RVA: 0x00166570 File Offset: 0x00164770
		private void SummonSpawns()
		{
			if (this.m_isDead || this.m_activeSpawns == null || this.m_activeSpawns.Count <= 0 || !this.m_gameEntity || !this.m_gameEntity.NetworkEntity || this.m_gameEntity.NetworkEntity.NObservers <= 0 || !this.m_targetController || this.m_targetController.HostileTargetCount <= 0)
			{
				return;
			}
			Vector3 position = this.m_gameEntity.gameObject.transform.position;
			float num = this.m_summonExclusionDistance * this.m_summonExclusionDistance;
			for (int i = 0; i < this.m_activeSpawns.Count; i++)
			{
				if (this.m_activeSpawns[i].Entity && this.m_activeSpawns[i].Entity.VitalsReplicator && this.m_activeSpawns[i].Entity.VitalsReplicator.CurrentHealthState.Value == HealthState.Alive)
				{
					Transform transform = this.m_activeSpawns[i].Entity.gameObject.transform;
					if ((position - transform.position).sqrMagnitude > num)
					{
						Vector3 vector = this.m_summonTargetPosition ? this.m_summonTargetPosition.GetPosition() : position;
						float y = Quaternion.LookRotation(position - vector).eulerAngles.y;
						Quaternion rotation = Quaternion.Euler(new Vector3(0f, y, 0f));
						transform.SetPositionAndRotation(vector, rotation);
						if (this.m_activeSpawns[i].Entity.EffectController)
						{
							this.m_activeSpawns[i].Entity.EffectController.RemoveCrowdControlForSummon();
						}
						if (this.m_resetThreatOnSummon)
						{
							this.m_targetController.ResetThreatForAllTargets();
						}
						else if (this.m_activeSpawns[i].Entity.ServerNpcController)
						{
							this.m_activeSpawns[i].Entity.ServerNpcController.InterruptBehavior();
						}
					}
				}
			}
			this.m_timeOfLastSummon = Time.time;
		}

		// Token: 0x04003312 RID: 13074
		private const int kMaxDespawnPerCycleDestroyable = 1;

		// Token: 0x04003313 RID: 13075
		[SerializeField]
		private GameEntity m_gameEntity;

		// Token: 0x04003314 RID: 13076
		[SerializeField]
		private bool m_respawnImmediatelyAfterDeath;

		// Token: 0x04003315 RID: 13077
		[SerializeField]
		private bool m_spawnsFollowController;

		// Token: 0x04003316 RID: 13078
		private const string kSummonGrp = "Summon";

		// Token: 0x04003317 RID: 13079
		[SerializeField]
		private NpcTargetController m_targetController;

		// Token: 0x04003318 RID: 13080
		[SerializeField]
		private TargetPosition m_summonTargetPosition;

		// Token: 0x04003319 RID: 13081
		[SerializeField]
		private float m_summonExclusionDistance = 5f;

		// Token: 0x0400331A RID: 13082
		[SerializeField]
		private bool m_resetThreatOnSummon;

		// Token: 0x0400331B RID: 13083
		[SerializeField]
		private bool m_summonOnThreshold;

		// Token: 0x0400331C RID: 13084
		[SerializeField]
		private bool m_summonPeriodically;

		// Token: 0x0400331D RID: 13085
		[SerializeField]
		private int m_periodicSummonFrequency = 20;

		// Token: 0x0400331E RID: 13086
		private float m_timeOfLastSummon = -1f;

		// Token: 0x0400331F RID: 13087
		[SerializeField]
		private bool m_dynamicTargetPopulation;

		// Token: 0x04003320 RID: 13088
		[SerializeField]
		private TargetPopulationThreshold[] m_targetPopulationThresholds;

		// Token: 0x04003321 RID: 13089
		private int m_previousTargetPopulationThresholdIndex = -1;

		// Token: 0x04003322 RID: 13090
		private bool m_isDead;
	}
}
