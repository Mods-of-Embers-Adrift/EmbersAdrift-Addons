using System;
using SoL.Game.EffectSystem;
using SoL.Game.Interactives;
using SoL.Game.NPCs.Senses;
using SoL.Game.Settings;
using SoL.Networking.Objects;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.NPCs
{
	// Token: 0x0200081F RID: 2079
	public class NpcTarget : IPoolable
	{
		// Token: 0x17000DD3 RID: 3539
		// (get) Token: 0x06003C21 RID: 15393 RVA: 0x00068BC8 File Offset: 0x00066DC8
		// (set) Token: 0x06003C22 RID: 15394 RVA: 0x00068BD0 File Offset: 0x00066DD0
		public GameEntity Entity { get; private set; }

		// Token: 0x17000DD4 RID: 3540
		// (get) Token: 0x06003C23 RID: 15395 RVA: 0x00068BD9 File Offset: 0x00066DD9
		// (set) Token: 0x06003C24 RID: 15396 RVA: 0x00068BE1 File Offset: 0x00066DE1
		public uint NetworkId { get; private set; }

		// Token: 0x17000DD5 RID: 3541
		// (get) Token: 0x06003C25 RID: 15397 RVA: 0x00068BEA File Offset: 0x00066DEA
		// (set) Token: 0x06003C26 RID: 15398 RVA: 0x00068BF2 File Offset: 0x00066DF2
		public DateTime? EnragedUntil { get; set; }

		// Token: 0x06003C27 RID: 15399 RVA: 0x0017E2B8 File Offset: 0x0017C4B8
		public void Reset()
		{
			this.Entity = null;
			this.NetworkId = 0U;
			this.m_controller = null;
			this.m_targetType = NpcTargetType.None;
			this.m_maintainTargets = false;
			this.m_hasLOS = false;
			this.m_angle = 0f;
			this.m_sqrDistance = 0f;
			this.m_heardVolume = 0f;
			this.m_threat = 0f;
			this.m_lastInRange = DateTime.MinValue;
			this.m_alert = null;
			NpcTarget.VisualSensor visualImmediateSensor = this.m_visualImmediateSensor;
			if (visualImmediateSensor != null)
			{
				visualImmediateSensor.Reset();
			}
			NpcTarget.VisualSensor visualPeripheralSensor = this.m_visualPeripheralSensor;
			if (visualPeripheralSensor != null)
			{
				visualPeripheralSensor.Reset();
			}
			NpcTarget.AuditorySensor auditorySensor = this.m_auditorySensor;
			if (auditorySensor != null)
			{
				auditorySensor.Reset();
			}
			NpcTarget.OlfactorySensor olfactorySensor = this.m_olfactorySensor;
			if (olfactorySensor != null)
			{
				olfactorySensor.Reset();
			}
			this.m_overrideHostility = false;
		}

		// Token: 0x17000DD6 RID: 3542
		// (get) Token: 0x06003C28 RID: 15400 RVA: 0x00068BFB File Offset: 0x00066DFB
		// (set) Token: 0x06003C29 RID: 15401 RVA: 0x00068C03 File Offset: 0x00066E03
		bool IPoolable.InPool
		{
			get
			{
				return this.m_inPool;
			}
			set
			{
				this.m_inPool = value;
			}
		}

		// Token: 0x17000DD7 RID: 3543
		// (get) Token: 0x06003C2A RID: 15402 RVA: 0x00068C0C File Offset: 0x00066E0C
		// (set) Token: 0x06003C2B RID: 15403 RVA: 0x0017E378 File Offset: 0x0017C578
		public NpcTargetType TargetType
		{
			get
			{
				return this.m_targetType;
			}
			private set
			{
				if (value != this.m_targetType && this.m_targetType == NpcTargetType.Alert && this.m_alert != null)
				{
					StaticPool<NpcAlert>.ReturnToPool(this.m_alert);
					this.m_alert = null;
				}
				this.m_targetType = value;
				NpcTargetType targetType = this.m_targetType;
				if (targetType == NpcTargetType.Alert)
				{
					this.CreateUpdateAlert();
					return;
				}
				if (targetType != NpcTargetType.Hostile)
				{
					return;
				}
				if (this.m_threat <= 0f)
				{
					this.AddInitialThreat();
					this.m_threat += 1f;
				}
			}
		}

		// Token: 0x17000DD8 RID: 3544
		// (get) Token: 0x06003C2C RID: 15404 RVA: 0x00068C14 File Offset: 0x00066E14
		public float SqrDistance
		{
			get
			{
				return this.m_sqrDistance;
			}
		}

		// Token: 0x17000DD9 RID: 3545
		// (get) Token: 0x06003C2D RID: 15405 RVA: 0x00068C1C File Offset: 0x00066E1C
		public NpcAlert Alert
		{
			get
			{
				return this.m_alert;
			}
		}

		// Token: 0x06003C2F RID: 15407 RVA: 0x00068C37 File Offset: 0x00066E37
		public void ResetThreat()
		{
			this.InitializeExistingTarget(this.m_controller, this);
		}

		// Token: 0x06003C30 RID: 15408 RVA: 0x00068C46 File Offset: 0x00066E46
		public void InitializeExistingTarget(NpcTargetController controller, NpcTarget existingTarget)
		{
			if (existingTarget == null || existingTarget.Entity == null)
			{
				return;
			}
			this.m_threat = 1f;
			this.m_targetType = existingTarget.m_targetType;
			this.Initialize(controller, existingTarget.Entity);
		}

		// Token: 0x06003C31 RID: 15409 RVA: 0x0017E3FC File Offset: 0x0017C5FC
		public void Initialize(NpcTargetController controller, GameEntity entity)
		{
			this.m_controller = controller;
			this.Entity = entity;
			this.NetworkId = entity.NetworkEntity.NetworkId.Value;
			this.m_lastInRange = DateTime.UtcNow;
			if (this.m_controller.SensorProfile == null)
			{
				return;
			}
			NpcSensorProfile sensorProfile = this.m_controller.SensorProfile;
			bool flag = sensorProfile.HasSensor(SensorType.VisualImmediate);
			bool flag2 = sensorProfile.HasSensor(SensorType.VisualPeripheral);
			bool flag3 = sensorProfile.HasSensor(SensorType.Auditory);
			bool flag4 = sensorProfile.HasSensor(SensorType.Olfactory);
			if (flag && this.m_visualImmediateSensor == null)
			{
				this.m_visualImmediateSensor = new NpcTarget.VisualSensor(SensorType.VisualImmediate);
			}
			if (flag2 && this.m_visualPeripheralSensor == null)
			{
				this.m_visualPeripheralSensor = new NpcTarget.VisualSensor(SensorType.VisualPeripheral);
			}
			if (flag3 && this.m_auditorySensor == null)
			{
				this.m_auditorySensor = new NpcTarget.AuditorySensor(SensorType.Auditory);
			}
			if (flag4 && this.m_olfactorySensor == null)
			{
				this.m_olfactorySensor = new NpcTarget.OlfactorySensor(SensorType.Olfactory);
			}
			NpcTarget.VisualSensor visualImmediateSensor = this.m_visualImmediateSensor;
			if (visualImmediateSensor != null)
			{
				visualImmediateSensor.Init(this, flag ? sensorProfile : null);
			}
			NpcTarget.VisualSensor visualPeripheralSensor = this.m_visualPeripheralSensor;
			if (visualPeripheralSensor != null)
			{
				visualPeripheralSensor.Init(this, flag2 ? sensorProfile : null);
			}
			NpcTarget.AuditorySensor auditorySensor = this.m_auditorySensor;
			if (auditorySensor != null)
			{
				auditorySensor.Init(this, flag3 ? sensorProfile : null);
			}
			NpcTarget.OlfactorySensor olfactorySensor = this.m_olfactorySensor;
			if (olfactorySensor != null)
			{
				olfactorySensor.Init(this, flag4 ? sensorProfile : null);
			}
			this.RefreshTargetData();
		}

		// Token: 0x06003C32 RID: 15410 RVA: 0x0017E548 File Offset: 0x0017C748
		public void RefreshTargetData()
		{
			if (this.m_controller == null || this.Entity == null)
			{
				return;
			}
			Vector3 vector = this.m_controller.PrimaryTargetPoint.transform.position + Vector3.up * 1f;
			Vector3 vector2 = this.Entity.PrimaryTargetPoint.transform.position + Vector3.up * 1f;
			Vector3 vector3 = vector2 - vector;
			this.m_maintainTargets = (this.m_controller && this.m_controller.MaintainTargets());
			this.m_hasLOS = LineOfSight.HasLineOfSight(vector, vector2, vector3);
			this.m_angle = Vector3.Angle(vector3, this.m_controller.PrimaryTargetPoint.transform.forward);
			this.m_sqrDistance = vector3.sqrMagnitude;
			this.m_heardVolume = 0f;
			NpcTarget.VisualSensor visualImmediateSensor = this.m_visualImmediateSensor;
			if (visualImmediateSensor != null)
			{
				visualImmediateSensor.Refresh();
			}
			NpcTarget.VisualSensor visualPeripheralSensor = this.m_visualPeripheralSensor;
			if (visualPeripheralSensor != null)
			{
				visualPeripheralSensor.Refresh();
			}
			NpcTarget.AuditorySensor auditorySensor = this.m_auditorySensor;
			if (auditorySensor != null)
			{
				auditorySensor.Refresh();
			}
			NpcTarget.OlfactorySensor olfactorySensor = this.m_olfactorySensor;
			if (olfactorySensor != null)
			{
				olfactorySensor.Refresh();
			}
			if (this.m_maintainTargets || this.m_sqrDistance <= this.m_controller.CurrentSqrMaxDistance)
			{
				this.m_lastInRange = DateTime.UtcNow;
			}
		}

		// Token: 0x06003C33 RID: 15411 RVA: 0x00068C7E File Offset: 0x00066E7E
		private void AddInitialThreat()
		{
			if (this.m_threat <= 0f && this.m_controller && this.m_controller.HostileTargetCount <= 0)
			{
				this.m_threat += 4f;
			}
		}

		// Token: 0x06003C34 RID: 15412 RVA: 0x0017E6A0 File Offset: 0x0017C8A0
		public void ProcessTarget()
		{
			NpcTargetType targetType = this.m_maintainTargets ? this.TargetType : this.GetTargetType();
			if (this.m_alert != null)
			{
				this.m_alert.UpdateValue();
				if (this.m_alert.Value <= 0f)
				{
					targetType = NpcTargetType.Unknown;
				}
			}
			if (this.IsNoTarget())
			{
				targetType = NpcTargetType.Unknown;
			}
			this.TargetType = targetType;
			if (this.EnragedUntil != null && DateTime.UtcNow >= this.EnragedUntil.Value)
			{
				this.EnragedUntil = null;
			}
		}

		// Token: 0x06003C35 RID: 15413 RVA: 0x00068CBA File Offset: 0x00066EBA
		private NpcTargetType GetTargetTypeForFaction()
		{
			if (this.m_overrideHostility)
			{
				return NpcTargetType.Hostile;
			}
			if (!this.m_controller.IsHostileToTags.Matches(this.Entity.CharacterData.NpcTagsSet))
			{
				return NpcTargetType.Neutral;
			}
			return NpcTargetType.Hostile;
		}

		// Token: 0x06003C36 RID: 15414 RVA: 0x0017E738 File Offset: 0x0017C938
		private NpcTargetType GetTargetType()
		{
			if (this.TargetType == NpcTargetType.Hostile)
			{
				return this.TargetType;
			}
			if (this.TargetType == NpcTargetType.Neutral)
			{
				if (this.m_sqrDistance <= 625f)
				{
					return this.TargetType;
				}
				return NpcTargetType.Unknown;
			}
			else
			{
				if (this.TargetType == NpcTargetType.Unknown && this.m_controller.IsLulled)
				{
					return NpcTargetType.Unknown;
				}
				if (this.m_visualImmediateSensor != null && this.m_visualImmediateSensor.Active && this.m_visualImmediateSensor.Value >= this.m_visualImmediateSensor.SensorThreshold)
				{
					return this.GetTargetTypeForFaction();
				}
				if (this.m_visualPeripheralSensor != null && this.m_visualPeripheralSensor.Active && this.m_visualPeripheralSensor.Value >= this.m_visualPeripheralSensor.SensorThreshold)
				{
					float num = (this.m_visualImmediateSensor != null) ? this.m_visualImmediateSensor.SensorDistance : 0f;
					float distanceGradient = (this.m_sqrDistance - num * num) / (this.m_visualPeripheralSensor.SensorDistance - num);
					float angleGradient = this.m_angle / (this.m_visualPeripheralSensor.SensorAngle * 0.5f);
					float num2 = this.m_visualPeripheralSensor.Value / this.m_visualPeripheralSensor.SensorThreshold;
					if (this.Entity && this.Entity.VitalsReplicator)
					{
						num2 *= this.Entity.VitalsReplicator.CurrentStance.Value.GetDetectionMultiplier();
					}
					NpcTarget.DetectionType detectionType = this.GetDetectionType(SensorType.VisualPeripheral, distanceGradient, angleGradient, num2);
					if (detectionType == NpcTarget.DetectionType.Immediate)
					{
						return this.GetTargetTypeForFaction();
					}
					if (detectionType == NpcTarget.DetectionType.Extended)
					{
						if (this.GetTargetTypeForFaction() != NpcTargetType.Hostile)
						{
							return NpcTargetType.Neutral;
						}
						return NpcTargetType.Alert;
					}
				}
				if (this.m_auditorySensor != null && this.m_auditorySensor.Active && this.m_auditorySensor.Value >= this.m_auditorySensor.SensorThreshold)
				{
					float distanceGradient2 = this.m_sqrDistance / (this.m_auditorySensor.SensorDistance * this.m_auditorySensor.SensorDistance);
					float angleGradient2 = this.m_angle / (this.m_auditorySensor.SensorAngle * 0.5f);
					float detectionMultiplier = this.m_auditorySensor.Value / this.m_auditorySensor.SensorThreshold;
					NpcTarget.DetectionType detectionType2 = this.GetDetectionType(SensorType.Auditory, distanceGradient2, angleGradient2, detectionMultiplier);
					if (detectionType2 - NpcTarget.DetectionType.Immediate <= 1)
					{
						if (this.GetTargetTypeForFaction() != NpcTargetType.Hostile)
						{
							return NpcTargetType.Neutral;
						}
						return NpcTargetType.Alert;
					}
				}
				if (this.m_olfactorySensor != null && this.m_olfactorySensor.Active && this.m_olfactorySensor.Value >= this.m_olfactorySensor.SensorThreshold)
				{
					float distanceGradient3 = this.m_sqrDistance / (this.m_olfactorySensor.SensorDistance * this.m_olfactorySensor.SensorDistance);
					float angleGradient3 = this.m_angle / (this.m_olfactorySensor.SensorAngle * 0.5f);
					float detectionMultiplier2 = this.m_olfactorySensor.Value / this.m_olfactorySensor.SensorThreshold;
					NpcTarget.DetectionType detectionType3 = this.GetDetectionType(SensorType.Olfactory, distanceGradient3, angleGradient3, detectionMultiplier2);
					if (detectionType3 - NpcTarget.DetectionType.Immediate <= 1)
					{
						if (this.GetTargetTypeForFaction() != NpcTargetType.Hostile)
						{
							return NpcTargetType.Neutral;
						}
						return NpcTargetType.Alert;
					}
				}
				if (this.TargetType != NpcTargetType.Alert)
				{
					return NpcTargetType.Unknown;
				}
				return this.TargetType;
			}
		}

		// Token: 0x06003C37 RID: 15415 RVA: 0x0017EA28 File Offset: 0x0017CC28
		private NpcTarget.DetectionType GetDetectionType(SensorType sensorType, float distanceGradient, float angleGradient, float detectionMultiplier)
		{
			float num = 0f;
			float b = 0f;
			float a = 0f;
			switch (sensorType)
			{
			case SensorType.VisualPeripheral:
				b = Mathf.Lerp(0.05f, 0.1f, num);
				a = Mathf.Lerp(0.15f, 0.2f, num);
				break;
			case SensorType.Auditory:
				b = Mathf.Lerp(0.01f, 0.04f, num);
				a = Mathf.Lerp(0.04f, 0.08f, num);
				break;
			case SensorType.Olfactory:
				b = Mathf.Lerp(0.01f, 0.02f, num);
				a = Mathf.Lerp(0.02f, 0.04f, num);
				break;
			}
			float t = (distanceGradient + angleGradient) * 0.5f;
			float num2 = Mathf.Clamp01(Mathf.Lerp(a, b, t) * detectionMultiplier + this.m_heardVolume * num);
			float num3 = UnityEngine.Random.Range(0f, 1f);
			if (num3 < num2)
			{
				return NpcTarget.DetectionType.Immediate;
			}
			if (num3 < num2 * 1.25f)
			{
				return NpcTarget.DetectionType.Extended;
			}
			return NpcTarget.DetectionType.None;
		}

		// Token: 0x06003C38 RID: 15416 RVA: 0x0017EB14 File Offset: 0x0017CD14
		public bool IsExpired()
		{
			return (DateTime.UtcNow - this.m_lastInRange).TotalSeconds > (double)GlobalSettings.Values.Npcs.TargetExpirationTime;
		}

		// Token: 0x06003C39 RID: 15417 RVA: 0x0017EB4C File Offset: 0x0017CD4C
		public bool IsInvalid()
		{
			return !this.Entity || !this.Entity.Vitals || this.Entity.Vitals.Health <= 0f || this.Entity.Vitals.GetCurrentHealthState() != HealthState.Alive || this.IsNoTarget();
		}

		// Token: 0x06003C3A RID: 15418 RVA: 0x0017EBAC File Offset: 0x0017CDAC
		private bool IsNoTarget()
		{
			return this.Entity && this.Entity.Type == GameEntityType.Player && this.Entity.CharacterData && this.Entity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.NoTarget);
		}

		// Token: 0x06003C3B RID: 15419 RVA: 0x00068CEB File Offset: 0x00066EEB
		public void AddThreat(float threatValue, bool setTargetTypeToOffensive)
		{
			this.AddInitialThreat();
			this.m_threat += threatValue;
			this.TargetType = NpcTargetType.Hostile;
		}

		// Token: 0x06003C3C RID: 15420 RVA: 0x0017EC08 File Offset: 0x0017CE08
		public void AddThreat(NetworkEntity source, EffectProcessingResult results)
		{
			float num = 1f;
			float num2 = 1f;
			bool flag = true;
			if (results.Flags.HasBitFlag(EffectApplicationFlags.Positive))
			{
				num = GlobalSettings.Values.Combat.HealThreatMultiplier;
				num2 = 0f;
				flag = false;
			}
			else if (results.Flags.HasBitFlag(EffectApplicationFlags.Block))
			{
				num = 0.25f;
				num2 = 0.5f;
			}
			else if (results.Flags.HasBitFlag(EffectApplicationFlags.Avoid))
			{
				num = 0.1f;
				num2 = 0.1f;
				flag = false;
			}
			else if (results.Flags.HasBitFlag(EffectApplicationFlags.Absorb))
			{
				num = 0.5f;
				num2 = 1f;
			}
			else if (results.Flags.HasBitFlag(EffectApplicationFlags.Miss))
			{
				num = 0.2f;
				num2 = 0.2f;
				flag = false;
			}
			else if (results.Flags.HasBitFlag(EffectApplicationFlags.Applied))
			{
				num = 0.25f;
				num2 = 0.25f;
			}
			else if (results.Flags.HasBitFlag(EffectApplicationFlags.Critical))
			{
				num = 1.2f;
				num2 = 1.5f;
			}
			else if (results.Flags.HasBitFlag(EffectApplicationFlags.Glancing) || results.Flags.HasBitFlag(EffectApplicationFlags.Normal) || results.Flags.HasBitFlag(EffectApplicationFlags.Heavy))
			{
				num2 = 0.9f;
			}
			if (results.Flags.HasBitFlag(EffectApplicationFlags.Miss) || results.Flags.HasBitFlag(EffectApplicationFlags.PartialResist) || results.Flags.HasBitFlag(EffectApplicationFlags.Resist))
			{
				this.m_threat += 1f;
			}
			this.m_threat += Mathf.Abs(results.Threat * num) + Mathf.Abs(results.DamageAbsorbed * num2);
			if (flag && this.m_threat > 0f)
			{
				this.TargetType = NpcTargetType.Hostile;
			}
			if (flag && this.m_threat > 0f)
			{
				this.AddAsTagger(source.GameEntity);
			}
		}

		// Token: 0x06003C3D RID: 15421 RVA: 0x0017EDE4 File Offset: 0x0017CFE4
		public void AddAsTagger(GameEntity source)
		{
			InteractiveNpc interactiveNpc;
			if (this.m_controller.GameEntity.Interactive != null && this.m_controller.GameEntity.Interactive.TryGetAsType(out interactiveNpc))
			{
				GameEntityType type = source.Type;
				if (type == GameEntityType.Player)
				{
					interactiveNpc.AddAsTagger(source);
					return;
				}
				if (type != GameEntityType.Npc)
				{
					return;
				}
				interactiveNpc.NpcContributed = true;
			}
		}

		// Token: 0x06003C3E RID: 15422 RVA: 0x0017EE3C File Offset: 0x0017D03C
		public float GetDistanceWeightedThreat()
		{
			if (this.TargetType != NpcTargetType.Hostile)
			{
				return 0f;
			}
			if (this.m_threat <= 0f)
			{
				return 0f;
			}
			if (this.m_sqrDistance <= 25f)
			{
				return this.m_threat;
			}
			float t = 1f;
			float b = 1f;
			float a = 1f;
			if (this.m_sqrDistance <= 1600f)
			{
				t = this.m_sqrDistance / 1600f;
				b = 0.5f;
			}
			else if (this.m_sqrDistance > 1600f)
			{
				t = (Mathf.Clamp(this.m_sqrDistance, 1600f, 10000f) - 1600f) / 8400f;
				a = 0.5f;
				b = 0.01f;
			}
			float stanceThreatMultiplier = GlobalSettings.Values.Npcs.GetStanceThreatMultiplier(this.Entity.VitalsReplicator.CurrentStance.Value);
			return this.m_threat * stanceThreatMultiplier * Mathf.Lerp(a, b, t);
		}

		// Token: 0x06003C3F RID: 15423 RVA: 0x00068D08 File Offset: 0x00066F08
		public void OverrideHostility()
		{
			this.m_overrideHostility = true;
		}

		// Token: 0x06003C40 RID: 15424 RVA: 0x0017EF24 File Offset: 0x0017D124
		public void CreateUpdateAlertExternal()
		{
			NpcTargetType targetType = this.TargetType;
			if (targetType - NpcTargetType.Neutral > 2)
			{
				this.TargetType = NpcTargetType.Alert;
			}
		}

		// Token: 0x06003C41 RID: 15425 RVA: 0x0017EF48 File Offset: 0x0017D148
		private void CreateUpdateAlert()
		{
			float value = 0.5f * (1f - this.m_sqrDistance / this.m_controller.CurrentSqrMaxDistance) + 0.3f * (1f - this.m_angle / 360f) + 0.2f * this.m_heardVolume;
			value = Mathf.Clamp(value, 1f, float.MaxValue);
			if (this.m_alert != null)
			{
				this.m_alert.UpdateAlert(this.Entity, value);
				return;
			}
			this.m_alert = StaticPool<NpcAlert>.GetFromPool();
			this.m_alert.Initialize(this.Entity, value);
		}

		// Token: 0x06003C42 RID: 15426 RVA: 0x00068D11 File Offset: 0x00066F11
		public float GetAlertValue()
		{
			if (this.m_alert != null)
			{
				return this.m_alert.Value;
			}
			return 0f;
		}

		// Token: 0x06003C43 RID: 15427 RVA: 0x00068D2C File Offset: 0x00066F2C
		public void CompleteAlert()
		{
			if (this.TargetType == NpcTargetType.Alert)
			{
				this.TargetType = NpcTargetType.Unknown;
			}
		}

		// Token: 0x06003C44 RID: 15428 RVA: 0x00068D3E File Offset: 0x00066F3E
		public void Complete()
		{
			this.TargetType = NpcTargetType.Unknown;
		}

		// Token: 0x04003B0B RID: 15115
		private const float kMinDistanceForThreat = 25f;

		// Token: 0x04003B0C RID: 15116
		private const float knDistanceForFleeTargetDrop = 625f;

		// Token: 0x04003B0D RID: 15117
		private const float kMaxDistanceForInnerThreat = 1600f;

		// Token: 0x04003B0E RID: 15118
		private const float kMaxDistanceForOuterThreat = 10000f;

		// Token: 0x04003B0F RID: 15119
		private const float kMinReductionForInnerThreat = 0.5f;

		// Token: 0x04003B10 RID: 15120
		private const float kMinReductionForOuterThreat = 0.01f;

		// Token: 0x04003B11 RID: 15121
		private const float kInitialThreatAdditive = 4f;

		// Token: 0x04003B15 RID: 15125
		private bool m_inPool;

		// Token: 0x04003B16 RID: 15126
		private NpcTargetController m_controller;

		// Token: 0x04003B17 RID: 15127
		private NpcTargetType m_targetType;

		// Token: 0x04003B18 RID: 15128
		private bool m_maintainTargets;

		// Token: 0x04003B19 RID: 15129
		private bool m_hasLOS;

		// Token: 0x04003B1A RID: 15130
		private float m_angle;

		// Token: 0x04003B1B RID: 15131
		private float m_sqrDistance;

		// Token: 0x04003B1C RID: 15132
		private float m_heardVolume;

		// Token: 0x04003B1D RID: 15133
		private float m_threat;

		// Token: 0x04003B1E RID: 15134
		private NpcAlert m_alert;

		// Token: 0x04003B1F RID: 15135
		private DateTime m_lastInRange = DateTime.MinValue;

		// Token: 0x04003B20 RID: 15136
		private NpcTarget.VisualSensor m_visualImmediateSensor;

		// Token: 0x04003B21 RID: 15137
		private NpcTarget.VisualSensor m_visualPeripheralSensor;

		// Token: 0x04003B22 RID: 15138
		private NpcTarget.AuditorySensor m_auditorySensor;

		// Token: 0x04003B23 RID: 15139
		private NpcTarget.OlfactorySensor m_olfactorySensor;

		// Token: 0x04003B24 RID: 15140
		private bool m_overrideHostility;

		// Token: 0x02000820 RID: 2080
		private enum DetectionType
		{
			// Token: 0x04003B26 RID: 15142
			None,
			// Token: 0x04003B27 RID: 15143
			Immediate,
			// Token: 0x04003B28 RID: 15144
			Extended
		}

		// Token: 0x02000821 RID: 2081
		private abstract class InternalSensor
		{
			// Token: 0x17000DDA RID: 3546
			// (get) Token: 0x06003C45 RID: 15429 RVA: 0x00068D47 File Offset: 0x00066F47
			// (set) Token: 0x06003C46 RID: 15430 RVA: 0x00068D59 File Offset: 0x00066F59
			public bool Active
			{
				get
				{
					return this.m_isValid && this.m_active;
				}
				protected set
				{
					this.m_active = value;
				}
			}

			// Token: 0x17000DDB RID: 3547
			// (get) Token: 0x06003C47 RID: 15431 RVA: 0x00068D62 File Offset: 0x00066F62
			// (set) Token: 0x06003C48 RID: 15432 RVA: 0x00068D6A File Offset: 0x00066F6A
			public float TimeActive { get; private set; }

			// Token: 0x17000DDC RID: 3548
			// (get) Token: 0x06003C49 RID: 15433 RVA: 0x00068D73 File Offset: 0x00066F73
			public virtual float Value
			{
				get
				{
					return this.TimeActive;
				}
			}

			// Token: 0x17000DDD RID: 3549
			// (get) Token: 0x06003C4A RID: 15434 RVA: 0x00068D7B File Offset: 0x00066F7B
			public float SensorDistance
			{
				get
				{
					return this.m_sensorDistance;
				}
			}

			// Token: 0x17000DDE RID: 3550
			// (get) Token: 0x06003C4B RID: 15435 RVA: 0x00068D83 File Offset: 0x00066F83
			public float SensorAngle
			{
				get
				{
					return this.m_sensorAngle;
				}
			}

			// Token: 0x17000DDF RID: 3551
			// (get) Token: 0x06003C4C RID: 15436 RVA: 0x00068D8B File Offset: 0x00066F8B
			public float SensorThreshold
			{
				get
				{
					return this.m_sensorThreshold;
				}
			}

			// Token: 0x06003C4D RID: 15437 RVA: 0x00068D93 File Offset: 0x00066F93
			protected InternalSensor(SensorType sensorType)
			{
				this.m_sensorType = sensorType;
			}

			// Token: 0x06003C4E RID: 15438 RVA: 0x0017EFE4 File Offset: 0x0017D1E4
			public void Init(NpcTarget owner, NpcSensorProfile sensorProfile)
			{
				this.m_npcTarget = owner;
				this.m_isValid = (this.m_npcTarget != null && sensorProfile != null);
				if (this.m_isValid)
				{
					int level = this.GetLevel((this.m_npcTarget != null && this.m_npcTarget.m_controller != null) ? this.m_npcTarget.m_controller.GameEntity : null);
					int level2 = this.GetLevel(this.m_npcTarget.Entity);
					float num = (level2 > level) ? GlobalSettings.Values.Npcs.GetSensorDampenerMultiplier(level2 - level) : 1f;
					SensorSettings sensorSettingsForType = sensorProfile.GetSensorSettingsForType(this.m_sensorType);
					this.m_sensorDistance = sensorSettingsForType.Distance * num;
					this.m_sensorSqrDistance = this.m_sensorDistance * this.m_sensorDistance;
					this.m_sensorAngle = sensorSettingsForType.Angle;
					this.m_sensorThreshold = sensorSettingsForType.Threshold;
				}
			}

			// Token: 0x06003C4F RID: 15439 RVA: 0x0017F0C8 File Offset: 0x0017D2C8
			public virtual void Reset()
			{
				this.m_npcTarget = null;
				this.m_timeLastActive = 0f;
				this.m_isValid = false;
				this.m_sensorDistance = 0f;
				this.m_sensorSqrDistance = 0f;
				this.m_sensorAngle = 0f;
				this.m_sensorThreshold = 0f;
				this.Active = false;
				this.TimeActive = 0f;
			}

			// Token: 0x06003C50 RID: 15440 RVA: 0x0017F12C File Offset: 0x0017D32C
			public void Refresh()
			{
				if (!this.m_isValid)
				{
					return;
				}
				bool active = this.Active;
				if (this.m_npcTarget != null && this.m_npcTarget.m_maintainTargets)
				{
					this.Active = active;
				}
				else
				{
					this.Active = this.IsActive();
					this.TimeActive = ((active && this.Active) ? (this.TimeActive + (Time.time - this.m_timeLastActive)) : 0f);
				}
				this.m_timeLastActive = (this.Active ? Time.time : 0f);
			}

			// Token: 0x06003C51 RID: 15441 RVA: 0x00068DA2 File Offset: 0x00066FA2
			protected virtual bool IsActive()
			{
				return this.m_npcTarget != null && this.WithinDistanceAndArc(this.m_npcTarget.m_angle, this.m_npcTarget.m_sqrDistance);
			}

			// Token: 0x06003C52 RID: 15442 RVA: 0x00068DCA File Offset: 0x00066FCA
			private bool WithinDistanceAndArc(float angle, float sqrDistance)
			{
				return this.WithinArc(angle) && this.WithinDistance(sqrDistance);
			}

			// Token: 0x06003C53 RID: 15443 RVA: 0x00068DDE File Offset: 0x00066FDE
			private bool WithinDistance(float sqrDistance)
			{
				return sqrDistance <= this.m_sensorSqrDistance;
			}

			// Token: 0x06003C54 RID: 15444 RVA: 0x00068DEC File Offset: 0x00066FEC
			private bool WithinArc(float angle)
			{
				return angle <= this.m_sensorAngle * 0.5f;
			}

			// Token: 0x06003C55 RID: 15445 RVA: 0x00068E00 File Offset: 0x00067000
			private int GetLevel(GameEntity entity)
			{
				if (!(entity != null) || entity.Targetable == null)
				{
					return 1;
				}
				return entity.Targetable.Level;
			}

			// Token: 0x04003B29 RID: 15145
			protected readonly SensorType m_sensorType;

			// Token: 0x04003B2A RID: 15146
			protected NpcTarget m_npcTarget;

			// Token: 0x04003B2B RID: 15147
			protected float m_timeLastActive;

			// Token: 0x04003B2C RID: 15148
			private bool m_isValid;

			// Token: 0x04003B2D RID: 15149
			private float m_sensorDistance;

			// Token: 0x04003B2E RID: 15150
			private float m_sensorSqrDistance;

			// Token: 0x04003B2F RID: 15151
			private float m_sensorAngle;

			// Token: 0x04003B30 RID: 15152
			private float m_sensorThreshold;

			// Token: 0x04003B31 RID: 15153
			private bool m_active;
		}

		// Token: 0x02000822 RID: 2082
		private class VisualSensor : NpcTarget.InternalSensor
		{
			// Token: 0x06003C56 RID: 15446 RVA: 0x00068E20 File Offset: 0x00067020
			public VisualSensor(SensorType sensorType) : base(sensorType)
			{
			}

			// Token: 0x06003C57 RID: 15447 RVA: 0x00068E29 File Offset: 0x00067029
			protected override bool IsActive()
			{
				return this.m_npcTarget != null && ((this.m_sensorType == SensorType.VisualPeripheral && this.m_npcTarget.m_visualImmediateSensor.Active) || (this.m_npcTarget.m_hasLOS && base.IsActive()));
			}
		}

		// Token: 0x02000823 RID: 2083
		private class AuditorySensor : NpcTarget.InternalSensor
		{
			// Token: 0x17000DE0 RID: 3552
			// (get) Token: 0x06003C58 RID: 15448 RVA: 0x00068E67 File Offset: 0x00067067
			public override float Value
			{
				get
				{
					return this.m_accumulatedTimeActive;
				}
			}

			// Token: 0x06003C59 RID: 15449 RVA: 0x00068E20 File Offset: 0x00067020
			public AuditorySensor(SensorType sensorType) : base(sensorType)
			{
			}

			// Token: 0x06003C5A RID: 15450 RVA: 0x00068E6F File Offset: 0x0006706F
			public override void Reset()
			{
				base.Reset();
				this.m_accumulatedTimeActive = 0f;
				this.m_lastPosition = null;
			}

			// Token: 0x06003C5B RID: 15451 RVA: 0x0017F1B8 File Offset: 0x0017D3B8
			protected override bool IsActive()
			{
				bool flag = base.IsActive();
				if (flag && this.m_npcTarget != null)
				{
					Vector3 position = this.m_npcTarget.Entity.PrimaryTargetPoint.transform.position;
					if (this.m_lastPosition != null && (this.m_lastPosition.Value - position).sqrMagnitude > 2.25f)
					{
						this.m_accumulatedTimeActive += Time.time - this.m_timeLastActive;
					}
					this.m_lastPosition = new Vector3?(position);
					return flag;
				}
				this.m_accumulatedTimeActive = 0f;
				this.m_lastPosition = null;
				return flag;
			}

			// Token: 0x04003B33 RID: 15155
			private const float kDistanceThreshold = 1.5f;

			// Token: 0x04003B34 RID: 15156
			private const float kDistanceThresholdSqr = 2.25f;

			// Token: 0x04003B35 RID: 15157
			private float m_accumulatedTimeActive;

			// Token: 0x04003B36 RID: 15158
			private Vector3? m_lastPosition;
		}

		// Token: 0x02000824 RID: 2084
		private class OlfactorySensor : NpcTarget.InternalSensor
		{
			// Token: 0x06003C5C RID: 15452 RVA: 0x00068E20 File Offset: 0x00067020
			public OlfactorySensor(SensorType sensorType) : base(sensorType)
			{
			}
		}
	}
}
