using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using SoL.Behavior.Tasks;
using SoL.Game.Behavior;
using SoL.Game.EffectSystem;
using SoL.Game.Interactives;
using SoL.Game.NPCs.Senses;
using SoL.Game.Settings;
using SoL.Game.Spawning;
using SoL.Game.Targeting;
using SoL.Managers;
using SoL.Networking;
using SoL.Networking.Managers;
using SoL.Networking.Objects;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.NPCs
{
	// Token: 0x02000825 RID: 2085
	public class NpcTargetController : BaseTargetController, INpcTargetController
	{
		// Token: 0x17000DE1 RID: 3553
		// (get) Token: 0x06003C5D RID: 15453 RVA: 0x00068E8E File Offset: 0x0006708E
		private static Collider[] m_nearbyColliders
		{
			get
			{
				return Hits.Colliders100;
			}
		}

		// Token: 0x17000DE2 RID: 3554
		// (get) Token: 0x06003C5E RID: 15454 RVA: 0x00068E95 File Offset: 0x00067095
		// (set) Token: 0x06003C5F RID: 15455 RVA: 0x00068E9D File Offset: 0x0006709D
		public NpcSensorProfile SensorProfile { get; set; }

		// Token: 0x17000DE3 RID: 3555
		// (get) Token: 0x06003C60 RID: 15456 RVA: 0x00068EA6 File Offset: 0x000670A6
		// (set) Token: 0x06003C61 RID: 15457 RVA: 0x00068EAE File Offset: 0x000670AE
		public bool RotateAwayInCombat { get; set; }

		// Token: 0x17000DE4 RID: 3556
		// (get) Token: 0x06003C62 RID: 15458 RVA: 0x00068EB7 File Offset: 0x000670B7
		// (set) Token: 0x06003C63 RID: 15459 RVA: 0x00068EBF File Offset: 0x000670BF
		public bool CallForHelpRequiresLos { get; set; }

		// Token: 0x17000DE5 RID: 3557
		// (get) Token: 0x06003C64 RID: 15460 RVA: 0x00068EC8 File Offset: 0x000670C8
		// (set) Token: 0x06003C65 RID: 15461 RVA: 0x00068ED0 File Offset: 0x000670D0
		public NpcTagMatch IsHostileToTags { get; set; }

		// Token: 0x17000DE6 RID: 3558
		// (get) Token: 0x06003C66 RID: 15462 RVA: 0x00068ED9 File Offset: 0x000670D9
		public NpcTagSet NpcTagsSet
		{
			get
			{
				if (!base.GameEntity || !base.GameEntity.CharacterData)
				{
					return null;
				}
				return base.GameEntity.CharacterData.NpcTagsSet;
			}
		}

		// Token: 0x17000DE7 RID: 3559
		// (get) Token: 0x06003C67 RID: 15463 RVA: 0x00068855 File Offset: 0x00066A55
		internal GameObject PrimaryTargetPoint
		{
			get
			{
				if (!base.GameEntity)
				{
					return base.gameObject;
				}
				return base.GameEntity.PrimaryTargetPoint;
			}
		}

		// Token: 0x17000DE8 RID: 3560
		// (get) Token: 0x06003C68 RID: 15464 RVA: 0x00068F0C File Offset: 0x0006710C
		// (set) Token: 0x06003C69 RID: 15465 RVA: 0x00068F14 File Offset: 0x00067114
		public bool NpcIgnoreGuards { get; set; }

		// Token: 0x17000DE9 RID: 3561
		// (get) Token: 0x06003C6A RID: 15466 RVA: 0x00068F1D File Offset: 0x0006711D
		public override bool IgnoreGuards
		{
			get
			{
				return this.NpcIgnoreGuards;
			}
		}

		// Token: 0x17000DEA RID: 3562
		// (get) Token: 0x06003C6B RID: 15467 RVA: 0x00068F25 File Offset: 0x00067125
		public override bool IsLulled
		{
			get
			{
				return this.AlertCount <= 0 && this.HostileTargetCount <= 0 && base.GameEntity && base.GameEntity.IsLulled;
			}
		}

		// Token: 0x06003C6C RID: 15468 RVA: 0x00068F53 File Offset: 0x00067153
		internal bool MaintainTargets()
		{
			return base.GameEntity && (base.GameEntity.IsStunned || base.GameEntity.IsFeared || base.GameEntity.IsRooted);
		}

		// Token: 0x17000DEB RID: 3563
		// (get) Token: 0x06003C6D RID: 15469 RVA: 0x00068F8B File Offset: 0x0006718B
		// (set) Token: 0x06003C6E RID: 15470 RVA: 0x00068F93 File Offset: 0x00067193
		public GameEntity CurrentTarget
		{
			get
			{
				return this.m_currentTarget;
			}
			private set
			{
				this.m_currentTarget != value;
				this.m_currentTarget = value;
			}
		}

		// Token: 0x17000DEC RID: 3564
		// (get) Token: 0x06003C6F RID: 15471 RVA: 0x0017F2B8 File Offset: 0x0017D4B8
		public Vector3? ResetPosition
		{
			get
			{
				SharedVector3 resetPosition = this.m_resetPosition;
				if (resetPosition == null)
				{
					return null;
				}
				return new Vector3?(resetPosition.Value);
			}
		}

		// Token: 0x17000DED RID: 3565
		// (get) Token: 0x06003C70 RID: 15472 RVA: 0x00068FA9 File Offset: 0x000671A9
		// (set) Token: 0x06003C71 RID: 15473 RVA: 0x00068FB1 File Offset: 0x000671B1
		public int HostileTargetCount { get; private set; }

		// Token: 0x17000DEE RID: 3566
		// (get) Token: 0x06003C72 RID: 15474 RVA: 0x00068FBA File Offset: 0x000671BA
		// (set) Token: 0x06003C73 RID: 15475 RVA: 0x00068FC2 File Offset: 0x000671C2
		public int NeutralTargetCount { get; private set; }

		// Token: 0x17000DEF RID: 3567
		// (get) Token: 0x06003C74 RID: 15476 RVA: 0x00068FCB File Offset: 0x000671CB
		// (set) Token: 0x06003C75 RID: 15477 RVA: 0x00068FD3 File Offset: 0x000671D3
		public int AlertCount { get; private set; }

		// Token: 0x17000DF0 RID: 3568
		// (get) Token: 0x06003C76 RID: 15478 RVA: 0x00068FDC File Offset: 0x000671DC
		// (set) Token: 0x06003C77 RID: 15479 RVA: 0x00068FE4 File Offset: 0x000671E4
		public ISpawnController SpawnControllerPositionUpdater { get; set; }

		// Token: 0x17000DF1 RID: 3569
		// (get) Token: 0x06003C78 RID: 15480 RVA: 0x00068FED File Offset: 0x000671ED
		public float CurrentSqrMaxDistance
		{
			get
			{
				return this.m_currentSqrMaxDistance;
			}
		}

		// Token: 0x17000DF2 RID: 3570
		// (get) Token: 0x06003C79 RID: 15481 RVA: 0x00068FF5 File Offset: 0x000671F5
		public NpcTarget AlertTarget
		{
			get
			{
				return this.m_alertTarget;
			}
		}

		// Token: 0x17000DF3 RID: 3571
		// (get) Token: 0x06003C7A RID: 15482 RVA: 0x00068FFD File Offset: 0x000671FD
		public bool IsStaticDestroyable
		{
			get
			{
				return this.m_isStaticDestroyable;
			}
		}

		// Token: 0x17000DF4 RID: 3572
		// (get) Token: 0x06003C7B RID: 15483 RVA: 0x00069005 File Offset: 0x00067205
		// (set) Token: 0x06003C7C RID: 15484 RVA: 0x0006900D File Offset: 0x0006720D
		public float LastUpdateTargetFrame { get; private set; }

		// Token: 0x17000DF5 RID: 3573
		// (get) Token: 0x06003C7D RID: 15485 RVA: 0x00069016 File Offset: 0x00067216
		// (set) Token: 0x06003C7E RID: 15486 RVA: 0x0017F2E4 File Offset: 0x0017D4E4
		public ICallForHelpSettings CallForHelpSettings
		{
			get
			{
				return this.m_callForHelpSettings;
			}
			set
			{
				this.m_callForHelpSettings = value;
				if (this.m_callForHelpSettings != null)
				{
					if (this.m_regularPeriodic == null && this.m_callForHelpSettings.Periodic != null)
					{
						this.m_regularPeriodic = new NpcTargetController.CallForHelpPeriodic(this, this.m_callForHelpSettings.Periodic);
					}
					if (this.m_fleeingPeriodic == null && this.m_callForHelpSettings.Fleeing != null)
					{
						this.m_fleeingPeriodic = new NpcTargetController.CallForHelpPeriodic(this, this.m_callForHelpSettings.Fleeing);
					}
				}
			}
		}

		// Token: 0x06003C7F RID: 15487 RVA: 0x0006901E File Offset: 0x0006721E
		private void Start()
		{
			this.m_behaviorTree = base.gameObject.GetComponent<BehaviorTree>();
			ServerGameManager.NpcTargetManager.RegisterNpc(this);
			IInteractive interactive = base.GameEntity.Interactive;
			if (interactive == null)
			{
				return;
			}
			interactive.TryGetAsType(out this.m_interactiveNpc);
		}

		// Token: 0x06003C80 RID: 15488 RVA: 0x0017F358 File Offset: 0x0017D558
		private void Update()
		{
			if (BaseNetworkEntityManager.PlayerConnectedCount <= 0)
			{
				return;
			}
			if (base.GameEntity.Vitals.Health <= 0f || base.GameEntity.IsStunned)
			{
				return;
			}
			if (this.HostileTargetCount > 0 && this.CurrentTarget)
			{
				if (Time.time > this.m_timeOfNextCombatRefreshCheck)
				{
					List<NetworkEntity> list = ServerGameManager.SpatialManager.QueryRadius(base.GameEntity.PrimaryTargetPoint.transform.position, 25f);
					for (int i = 0; i < list.Count; i++)
					{
						NetworkEntity networkEntity = list[i];
						if (networkEntity && networkEntity.GameEntity && networkEntity.GameEntity.Type == GameEntityType.Player && networkEntity.GameEntity.Vitals && this.IsHostileTo(networkEntity, null))
						{
							networkEntity.GameEntity.Vitals.UpdateLastCombatTimestamp();
						}
					}
					this.m_timeOfNextCombatRefreshCheck = Time.time + GlobalSettings.Values.Combat.CombatRecoveryTime * 0.5f;
				}
				if (!this.RotateAwayInCombat && this.CurrentTarget.Type == GameEntityType.Player)
				{
					NpcTagSet npcTagsSet = base.GameEntity.CharacterData.NpcTagsSet;
					if (npcTagsSet != null)
					{
						npcTagsSet.AddAggressive();
					}
				}
				else
				{
					NpcTagSet npcTagsSet2 = base.GameEntity.CharacterData.NpcTagsSet;
					if (npcTagsSet2 != null)
					{
						npcTagsSet2.RevertAggressive();
					}
				}
				if (this.m_lastFrameHostileCount == 0 || Time.time >= this.m_timeOfNextCanReachCheck)
				{
					bool flag = (base.GameEntity.CharacterData.IsSwimming && this.CurrentTarget.CharacterData.IsSwimming) || CanReachCache.CanNpcReach(this.CurrentTarget, base.GameEntity);
					base.GameEntity.HasUnreachableTarget = !flag;
					this.m_timeOfNextCanReachCheck = Time.time + 1f;
					if (!flag)
					{
						this.CurrentTarget.Vitals.TriggerUndonesWrath(base.GameEntity);
					}
				}
				if (this.m_lastFrameHostileCount == 0)
				{
					ICallForHelpSettings callForHelpSettings = this.CallForHelpSettings;
					if (((callForHelpSettings != null) ? callForHelpSettings.InitialThreat : null) != null)
					{
						if (!this.m_bypassInitialCallForHelp)
						{
							ICallForHelpSettings callForHelpSettings2 = this.CallForHelpSettings;
							this.ChanceAlertNearbyNpcs((callForHelpSettings2 != null) ? callForHelpSettings2.InitialThreat : null, this.CurrentTarget.NetworkEntity);
						}
						this.m_bypassInitialCallForHelp = false;
						goto IL_2C6;
					}
				}
				if (this.m_lastFrameWasFeared || base.GameEntity.IsFeared)
				{
					NpcTargetController.CallForHelpPeriodic regularPeriodic = this.m_regularPeriodic;
					if (regularPeriodic != null)
					{
						regularPeriodic.ResetTime();
					}
					if (this.m_lastFrameWasFeared && !base.GameEntity.IsFeared)
					{
						ICallForHelpSettings callForHelpSettings3 = this.CallForHelpSettings;
						if (((callForHelpSettings3 != null) ? callForHelpSettings3.InitialThreat : null) != null)
						{
							ICallForHelpSettings callForHelpSettings4 = this.CallForHelpSettings;
							this.ChanceAlertNearbyNpcs((callForHelpSettings4 != null) ? callForHelpSettings4.InitialThreat : null, this.CurrentTarget.NetworkEntity);
						}
					}
				}
				else
				{
					NpcTargetController.CallForHelpPeriodic fleeingPeriodic = this.m_fleeingPeriodic;
					if (fleeingPeriodic != null)
					{
						fleeingPeriodic.ResetTime();
					}
					NpcTargetController.CallForHelpPeriodic regularPeriodic2 = this.m_regularPeriodic;
					if (regularPeriodic2 != null)
					{
						regularPeriodic2.Update();
					}
				}
				IL_2C6:
				if (!this.m_isStaticDestroyable && this.m_chargeOnHostileAcquired && this.m_lastFrameHostileCount == 0 && base.GameEntity.EffectController && GlobalSettings.Values.Npcs.GlobalSpeedBuff != null)
				{
					ICombatEffectSource globalSpeedBuff = GlobalSettings.Values.Npcs.GlobalSpeedBuff;
					if (globalSpeedBuff != null)
					{
						float num = (base.GameEntity.Targetable != null) ? ((float)base.GameEntity.Targetable.Level) : 50f;
						CombatEffect combatEffect = globalSpeedBuff.GetCombatEffect(num, AlchemyPowerLevel.None);
						base.GameEntity.EffectController.ApplyEffect(base.GameEntity, globalSpeedBuff.ArchetypeId, combatEffect, num, num, false, true);
					}
				}
			}
			else
			{
				base.GameEntity.HasUnreachableTarget = false;
				NpcTagSet npcTagsSet3 = base.GameEntity.CharacterData.NpcTagsSet;
				if (npcTagsSet3 != null)
				{
					npcTagsSet3.RevertAggressive();
				}
				NpcTargetController.CallForHelpPeriodic regularPeriodic3 = this.m_regularPeriodic;
				if (regularPeriodic3 != null)
				{
					regularPeriodic3.ResetTime();
				}
				NpcTargetController.CallForHelpPeriodic fleeingPeriodic2 = this.m_fleeingPeriodic;
				if (fleeingPeriodic2 != null)
				{
					fleeingPeriodic2.ResetTime();
				}
			}
			this.m_lastFrameHostileCount = this.HostileTargetCount;
			this.m_lastFrameWasFeared = base.GameEntity.IsFeared;
		}

		// Token: 0x06003C81 RID: 15489 RVA: 0x0017F754 File Offset: 0x0017D954
		protected override void OnDestroy()
		{
			if (NullifyMemoryLeakSettings.CleanNpcTargetController)
			{
				this.SensorProfile = null;
				this.IsHostileToTags = null;
				for (int i = 0; i < this.m_currentTargets.Count; i++)
				{
					StaticPool<NpcTarget>.ReturnToPool(this.m_currentTargets[i]);
				}
				this.m_currentTargets.Clear();
				this.m_behaviorTree = null;
				this.m_interactiveNpc = null;
				this.m_currentTarget = null;
				if (this.m_treeTargetEntity != null)
				{
					this.m_treeTargetEntity.Value = null;
					this.m_treeTargetEntity = null;
				}
				if (this.m_treeTargetGameObject != null)
				{
					this.m_treeTargetGameObject.Value = null;
					this.m_treeTargetGameObject = null;
				}
				if (this.m_treeNpcTarget != null)
				{
					this.m_treeNpcTarget.Value = null;
					this.m_treeNpcTarget = null;
				}
				this.m_hostileTargetCount = null;
				this.m_neutralTargetCount = null;
				this.m_alertCount = null;
				this.m_stoppingDistance = null;
				this.m_initialPosition = null;
				this.m_initialRotation = null;
				this.m_resetPosition = null;
				this.m_isInitialized = null;
				this.m_initialPositionOverride = null;
				this.m_initialRotationOverride = null;
				this.SpawnControllerPositionUpdater = null;
				if (this.m_hostileTarget != null)
				{
					StaticPool<NpcTarget>.ReturnToPool(this.m_hostileTarget);
					this.m_hostileTarget = null;
				}
				if (this.m_neutralTarget != null)
				{
					StaticPool<NpcTarget>.ReturnToPool(this.m_neutralTarget);
					this.m_neutralTarget = null;
				}
				if (this.m_alertTarget != null)
				{
					StaticPool<NpcTarget>.ReturnToPool(this.m_alertTarget);
					this.m_alertTarget = null;
				}
				this.m_callForHelpSettings = null;
				this.m_regularPeriodic = null;
				this.m_fleeingPeriodic = null;
			}
			base.OnDestroy();
		}

		// Token: 0x06003C82 RID: 15490 RVA: 0x0017F8D4 File Offset: 0x0017DAD4
		public void UpdateExternal()
		{
			if (base.GameEntity.Vitals.Health <= 0f || this.m_movementMode == NpcMovementMode.Reset)
			{
				return;
			}
			ISpawnController spawnControllerPositionUpdater = this.SpawnControllerPositionUpdater;
			if (spawnControllerPositionUpdater != null && spawnControllerPositionUpdater.CurrentPosition != null && this.m_initialPosition != null)
			{
				this.m_initialPosition.Value = this.SpawnControllerPositionUpdater.CurrentPosition.Value;
			}
			if ((this.m_initialPositionOverride != null || this.m_initialRotationOverride != null) && this.m_isInitialized != null && this.m_isInitialized.Value)
			{
				if (this.m_initialPositionOverride != null && this.m_initialPosition != null)
				{
					this.m_initialPosition.Value = this.m_initialPositionOverride.Value;
				}
				this.m_initialPositionOverride = null;
				if (this.m_initialRotationOverride != null && this.m_initialRotation != null)
				{
					this.m_initialRotation.Value = this.m_initialRotationOverride.Value;
				}
				this.m_initialRotationOverride = null;
			}
			this.UpdateTargets();
		}

		// Token: 0x06003C83 RID: 15491 RVA: 0x00069058 File Offset: 0x00067258
		private void UpdateTargets()
		{
			this.LastUpdateTargetFrame = (float)Time.frameCount;
			this.CollectTargets();
			this.ProcessTargets();
		}

		// Token: 0x06003C84 RID: 15492 RVA: 0x0017F9EC File Offset: 0x0017DBEC
		private void CollectTargets()
		{
			if (this.m_isStaticDestroyable || !base.GameEntity || base.GameEntity.IsFeared || base.GameEntity.IsStunned || this.IsLulled)
			{
				return;
			}
			float maxDistance = this.SensorProfile.GetMaxDistance();
			this.m_currentSqrMaxDistance = maxDistance * maxDistance;
			int num = Physics.OverlapSphereNonAlloc(this.PrimaryTargetPoint.transform.position, maxDistance, NpcTargetController.m_nearbyColliders, LayerMap.Detection.LayerMask, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < num; i++)
			{
				GameEntity entity;
				if (this.IsValidTarget(NpcTargetController.m_nearbyColliders[i], out entity))
				{
					this.AddGetTarget(entity);
				}
			}
		}

		// Token: 0x06003C85 RID: 15493 RVA: 0x0017FA98 File Offset: 0x0017DC98
		private bool IsValidTarget(Collider col, out GameEntity entity)
		{
			entity = null;
			return this.IsHostileToTags != null && !(col == null) && DetectionCollider.TryGetEntityForCollider(col, out entity) && entity && entity.VitalsReplicator && entity.VitalsReplicator.CurrentHealthState.Value == HealthState.Alive && entity.CharacterData && !this.ShouldIgnoreTarget(entity) && this.IsHostileToTags.Matches(entity.CharacterData.NpcTagsSet);
		}

		// Token: 0x06003C86 RID: 15494 RVA: 0x0017FB28 File Offset: 0x0017DD28
		private void ProcessTargets()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < this.m_currentTargets.Count; i++)
			{
				NpcTarget npcTarget = this.m_currentTargets[i];
				if (npcTarget.IsInvalid())
				{
					this.m_currentTargets.Remove(npcTarget.NetworkId);
					StaticPool<NpcTarget>.ReturnToPool(npcTarget);
					i--;
				}
				else
				{
					npcTarget.RefreshTargetData();
					if (npcTarget.IsExpired())
					{
						this.m_currentTargets.Remove(npcTarget.NetworkId);
						StaticPool<NpcTarget>.ReturnToPool(npcTarget);
						i--;
					}
					else
					{
						npcTarget.ProcessTarget();
						switch (npcTarget.TargetType)
						{
						case NpcTargetType.Alert:
							num3++;
							break;
						case NpcTargetType.Neutral:
							num2++;
							break;
						case NpcTargetType.Hostile:
							num++;
							break;
						}
					}
				}
			}
			this.HostileTargetCount = num;
			this.NeutralTargetCount = num2;
			this.AlertCount = num3;
			base.NTargets = new int?(num);
			if (this.m_behaviorTree)
			{
				if (this.m_hostileTargetCount == null)
				{
					this.m_hostileTargetCount = this.m_behaviorTree.GetSharedVariable("HostileTargetCount");
					this.m_neutralTargetCount = this.m_behaviorTree.GetSharedVariable("NeutralTargetCount");
					this.m_alertCount = this.m_behaviorTree.GetSharedVariable("AlertCount");
					this.m_treeTargetEntity = this.m_behaviorTree.GetSharedVariable("TargetEntity");
					this.m_treeTargetGameObject = this.m_behaviorTree.GetSharedVariable("TargetGameObject");
					this.m_treeNpcTarget = this.m_behaviorTree.GetSharedVariable("Target");
					this.m_stoppingDistance = this.m_behaviorTree.GetSharedVariable("StoppingDistance");
					this.m_initialPosition = this.m_behaviorTree.GetSharedVariable("InitialPosition");
					this.m_initialRotation = this.m_behaviorTree.GetSharedVariable("InitialRotation");
					this.m_resetPosition = this.m_behaviorTree.GetSharedVariable("ResetPosition");
					this.m_isInitialized = this.m_behaviorTree.GetSharedVariable("IsInitialized");
				}
				if ((this.m_hostileTargetCount.Value <= 0 && this.HostileTargetCount > 0) || (this.m_alertCount.Value <= 0 && this.AlertCount > 0))
				{
					this.m_resetPosition.Value = base.GameEntity.gameObject.transform.position;
				}
				bool flag = (this.m_hostileTargetCount.Value > 0 || this.m_neutralTargetCount.Value > 0 || this.m_alertCount.Value > 0) && this.HostileTargetCount == 0 && this.NeutralTargetCount == 0 && this.AlertCount == 0;
				this.m_hostileTargetCount.Value = this.HostileTargetCount;
				this.m_neutralTargetCount.Value = this.NeutralTargetCount;
				this.m_alertCount.Value = this.AlertCount;
				this.EvaluateTargets();
				NpcTarget currentTarget = this.GetCurrentTarget();
				GameEntity gameEntity = (currentTarget != null) ? currentTarget.Entity : null;
				this.CurrentTarget = gameEntity;
				bool flag2 = this.m_treeTargetEntity.Value != gameEntity || flag;
				this.m_treeNpcTarget.Value = currentTarget;
				this.m_treeTargetEntity.Value = gameEntity;
				this.m_treeTargetGameObject.Value = (gameEntity ? gameEntity.PrimaryTargetPoint : null);
				if (flag2 && base.GameEntity.ServerNpcController)
				{
					base.GameEntity.ServerNpcController.InterruptBehavior();
				}
			}
			if (this.HostileTargetCount <= 0)
			{
				this.CurrentTarget = null;
				this.m_offensiveTargetNetworkId.Value = 0U;
				if (this.m_interactiveNpc && this.m_interactiveNpc.IsTagged)
				{
					this.m_interactiveNpc.ClearTaggers();
				}
			}
		}

		// Token: 0x06003C87 RID: 15495 RVA: 0x0017FECC File Offset: 0x0017E0CC
		private void EvaluateTargets()
		{
			NpcTarget npcTarget = null;
			float num = float.MinValue;
			NpcTarget neutralTarget = null;
			float num2 = float.MaxValue;
			NpcTarget alertTarget = null;
			float num3 = float.MinValue;
			NpcTarget npcTarget2 = null;
			float num4 = float.MaxValue;
			NpcTarget npcTarget3 = null;
			float num5 = float.MaxValue;
			bool flag = base.GameEntity && base.GameEntity.IsRooted;
			NpcTarget npcTarget4 = null;
			float num6 = float.MaxValue;
			NpcTarget npcTarget5 = null;
			float num7 = float.MaxValue;
			NpcTarget npcTarget6 = null;
			float num8 = float.MaxValue;
			for (int i = 0; i < this.m_currentTargets.Count; i++)
			{
				NpcTarget npcTarget7 = this.m_currentTargets[i];
				switch (npcTarget7.TargetType)
				{
				case NpcTargetType.Alert:
				{
					float alertValue = npcTarget7.GetAlertValue();
					if (alertValue > num3)
					{
						num3 = alertValue;
						alertTarget = npcTarget7;
					}
					break;
				}
				case NpcTargetType.Neutral:
					if (npcTarget7.SqrDistance < num2)
					{
						num2 = npcTarget7.SqrDistance;
						neutralTarget = npcTarget7;
					}
					break;
				case NpcTargetType.Hostile:
					if (npcTarget7.Entity.IsStunned)
					{
						if (npcTarget7.SqrDistance < num5)
						{
							num5 = npcTarget7.SqrDistance;
							npcTarget3 = npcTarget7;
						}
					}
					else
					{
						float distanceWeightedThreat = npcTarget7.GetDistanceWeightedThreat();
						if (distanceWeightedThreat > num)
						{
							num = distanceWeightedThreat;
							npcTarget = npcTarget7;
						}
						if (npcTarget7.EnragedUntil != null && npcTarget7.SqrDistance < num4)
						{
							num4 = npcTarget7.SqrDistance;
							npcTarget2 = npcTarget7;
						}
						if (flag)
						{
							if (npcTarget7.SqrDistance < num8)
							{
								num8 = npcTarget7.SqrDistance;
								npcTarget6 = npcTarget7;
							}
							bool flag2 = distanceWeightedThreat > num6;
							bool flag3 = npcTarget7.EnragedUntil != null && npcTarget7.SqrDistance < num7;
							if ((flag2 || flag3) && this.CanAttackTarget(npcTarget7))
							{
								if (flag2)
								{
									num6 = distanceWeightedThreat;
									npcTarget4 = npcTarget7;
								}
								if (flag3)
								{
									num7 = npcTarget7.SqrDistance;
									npcTarget5 = npcTarget7;
								}
							}
						}
					}
					break;
				}
			}
			if (flag && npcTarget5 != null)
			{
				this.m_hostileTarget = npcTarget5;
			}
			else if (flag && npcTarget4 != null)
			{
				this.m_hostileTarget = npcTarget4;
			}
			else if (flag && npcTarget6 != null)
			{
				this.m_hostileTarget = npcTarget6;
			}
			else if (npcTarget2 != null)
			{
				this.m_hostileTarget = npcTarget2;
			}
			else if (npcTarget != null)
			{
				this.m_hostileTarget = npcTarget;
			}
			else if (npcTarget3 != null)
			{
				this.m_hostileTarget = npcTarget3;
			}
			else
			{
				this.m_hostileTarget = null;
			}
			this.m_neutralTarget = neutralTarget;
			this.m_alertTarget = alertTarget;
			if (this.m_hostileTarget != null)
			{
				this.m_offensiveTargetNetworkId.Value = this.m_hostileTarget.NetworkId;
				return;
			}
			this.m_offensiveTargetNetworkId.Value = 0U;
		}

		// Token: 0x06003C88 RID: 15496 RVA: 0x00180148 File Offset: 0x0017E348
		private bool CanAttackTarget(NpcTarget target)
		{
			return target != null && target.Entity && GlobalSettings.Values && GlobalSettings.Values.Combat != null && GlobalSettings.Values.Combat.AutoAttack && DistanceAngleChecks.MeetsDistanceRequirements(base.GameEntity, target.Entity, GlobalSettings.Values.Combat.AutoAttack.Targeting, base.GameEntity.HandHeldItemCache, null, null);
		}

		// Token: 0x06003C89 RID: 15497 RVA: 0x001801D8 File Offset: 0x0017E3D8
		private bool ShouldIgnoreTarget(GameEntity targetEntity)
		{
			return !targetEntity || (this.IgnoreGuards && targetEntity.CharacterData != null && targetEntity.CharacterData.NpcTagsSet != null && targetEntity.CharacterData.NpcTagsSet.IsGuard()) || (base.GameEntity && base.GameEntity.CharacterData && base.GameEntity.CharacterData.NpcTagsSet != null && base.GameEntity.CharacterData.NpcTagsSet.IsGuard() && targetEntity.TargetController != null && targetEntity.TargetController.IgnoreGuards);
		}

		// Token: 0x06003C8A RID: 15498 RVA: 0x0018028C File Offset: 0x0017E48C
		private NpcTarget AddGetTarget(GameEntity entity)
		{
			bool flag;
			return this.AddGetTarget(entity, out flag);
		}

		// Token: 0x06003C8B RID: 15499 RVA: 0x001802A4 File Offset: 0x0017E4A4
		private NpcTarget AddGetTarget(GameEntity entity, out bool added)
		{
			added = false;
			if (this.m_movementMode == NpcMovementMode.Reset)
			{
				return null;
			}
			if (entity == base.GameEntity)
			{
				return null;
			}
			if (entity.Type == GameEntityType.Player && entity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.NoTarget))
			{
				return null;
			}
			NpcTarget fromPool;
			if (!this.m_currentTargets.TryGetValue(entity.NetworkEntity.NetworkId.Value, out fromPool) && entity.Vitals.Health > 0f)
			{
				fromPool = StaticPool<NpcTarget>.GetFromPool();
				fromPool.Initialize(this, entity);
				this.m_currentTargets.Add(fromPool.NetworkId, fromPool);
				added = true;
			}
			return fromPool;
		}

		// Token: 0x06003C8C RID: 15500 RVA: 0x00180350 File Offset: 0x0017E550
		public override void InitializeAsAshen(BaseTargetController otherTargetController)
		{
			base.InitializeAsAshen(otherTargetController);
			if (!otherTargetController)
			{
				return;
			}
			NpcTargetController npcTargetController;
			if (otherTargetController.TryGetAsType(out npcTargetController))
			{
				this.m_currentSqrMaxDistance = npcTargetController.m_currentSqrMaxDistance;
				for (int i = 0; i < npcTargetController.m_currentTargets.Count; i++)
				{
					NpcTarget npcTarget = npcTargetController.m_currentTargets[i];
					NpcTarget npcTarget2;
					if (npcTarget != null && npcTarget.Entity && !this.m_currentTargets.TryGetValue(npcTarget.NetworkId, out npcTarget2))
					{
						NpcTarget fromPool = StaticPool<NpcTarget>.GetFromPool();
						fromPool.InitializeExistingTarget(this, npcTarget);
						this.m_currentTargets.Add(fromPool.NetworkId, fromPool);
					}
				}
				this.ProcessTargets();
				if (npcTargetController.m_isInitialized != null && npcTargetController.m_isInitialized.Value)
				{
					if (npcTargetController.m_initialPosition != null)
					{
						this.m_initialPositionOverride = new Vector3?(npcTargetController.m_initialPosition.Value);
					}
					if (npcTargetController.m_initialRotation != null)
					{
						this.m_initialRotationOverride = new Vector3?(npcTargetController.m_initialRotation.Value);
					}
				}
			}
		}

		// Token: 0x06003C8D RID: 15501 RVA: 0x0018044C File Offset: 0x0017E64C
		public void ResetThreatForAllTargets()
		{
			for (int i = 0; i < this.m_currentTargets.Count; i++)
			{
				this.m_currentTargets[i].ResetThreat();
			}
			this.ProcessTargets();
		}

		// Token: 0x06003C8E RID: 15502 RVA: 0x00180488 File Offset: 0x0017E688
		public override float GetTopThreatValue(GameEntity source, out float sourceThreat)
		{
			float num = base.GetTopThreatValue(source, out sourceThreat);
			for (int i = 0; i < this.m_currentTargets.Count; i++)
			{
				float distanceWeightedThreat = this.m_currentTargets[i].GetDistanceWeightedThreat();
				if (distanceWeightedThreat > num)
				{
					num = distanceWeightedThreat;
				}
				if (this.m_currentTargets[i].Entity == source)
				{
					sourceThreat = distanceWeightedThreat;
				}
			}
			return num;
		}

		// Token: 0x06003C8F RID: 15503 RVA: 0x001804EC File Offset: 0x0017E6EC
		public override bool IsTopThreat(GameEntity source, out float sourceThreat, out float? nextLowerThreat)
		{
			base.IsTopThreat(source, out sourceThreat, out nextLowerThreat);
			NpcTarget npcTarget = null;
			float num = 0f;
			for (int i = 0; i < this.m_currentTargets.Count; i++)
			{
				float distanceWeightedThreat = this.m_currentTargets[i].GetDistanceWeightedThreat();
				if (distanceWeightedThreat > num)
				{
					nextLowerThreat = new float?(num);
					npcTarget = this.m_currentTargets[i];
					num = distanceWeightedThreat;
				}
				else
				{
					float num2 = distanceWeightedThreat;
					float? num3 = nextLowerThreat;
					if (num2 > num3.GetValueOrDefault() & num3 != null)
					{
						nextLowerThreat = new float?(distanceWeightedThreat);
					}
				}
			}
			sourceThreat = num;
			return npcTarget != null && npcTarget.Entity == source;
		}

		// Token: 0x06003C90 RID: 15504 RVA: 0x00069072 File Offset: 0x00067272
		public override bool IsHostileTo(NetworkEntity sourceEntity, NetworkEntity alternateSourceEntity = null)
		{
			return this.IsHostile(sourceEntity, alternateSourceEntity);
		}

		// Token: 0x06003C91 RID: 15505 RVA: 0x00180590 File Offset: 0x0017E790
		public bool IsHostile(NetworkEntity sourceEntity, NetworkEntity alternateSourceEntity = null)
		{
			NpcTarget npcTarget;
			NpcTarget npcTarget2;
			return sourceEntity && sourceEntity.GameEntity && !this.ShouldIgnoreTarget(sourceEntity.GameEntity) && (this.IsHostileToTags.Matches(sourceEntity.GameEntity.CharacterData.NpcTagsSet) || (this.m_currentTargets.TryGetValue(sourceEntity.NetworkId.Value, out npcTarget) && npcTarget.TargetType == NpcTargetType.Hostile) || (alternateSourceEntity != null && alternateSourceEntity.GameEntity != null && sourceEntity.GameEntity.Type == GameEntityType.Player && alternateSourceEntity.GameEntity.Type == GameEntityType.Player && this.m_currentTargets.TryGetValue(alternateSourceEntity.NetworkId.Value, out npcTarget2) && npcTarget2.TargetType == NpcTargetType.Hostile));
		}

		// Token: 0x06003C92 RID: 15506 RVA: 0x00180668 File Offset: 0x0017E868
		public override bool InTargetList(NetworkEntity sourceEntity)
		{
			return sourceEntity && this.m_currentTargets.ContainsKey(sourceEntity.NetworkId.Value);
		}

		// Token: 0x06003C93 RID: 15507 RVA: 0x0006907C File Offset: 0x0006727C
		public override bool IsCurrentHostileTarget(NetworkEntity sourceEntity)
		{
			return sourceEntity && this.m_hostileTarget != null && this.m_hostileTarget.Entity != null && sourceEntity == this.m_hostileTarget.Entity.NetworkEntity;
		}

		// Token: 0x06003C94 RID: 15508 RVA: 0x00180698 File Offset: 0x0017E898
		private void ChanceAlertNearbyNpcs(CallForHelpData callForHelpData, NetworkEntity source)
		{
			if (callForHelpData == null || !source || !base.GameEntity || !base.GameEntity.Vitals)
			{
				return;
			}
			if (callForHelpData.CallsForHelp(base.GameEntity.Vitals.HealthPercent))
			{
				float range = callForHelpData.GetRange();
				if (this.AlertNearbyNpcs(source, range, callForHelpData))
				{
					callForHelpData.EmoteCallForHelp(base.GameEntity, range);
				}
			}
		}

		// Token: 0x06003C95 RID: 15509 RVA: 0x00180708 File Offset: 0x0017E908
		private bool AlertNearbyNpcs(NetworkEntity source, float range, CallForHelpData callForHelpData)
		{
			if (NpcTargetController.m_nearbyEntities == null)
			{
				NpcTargetController.m_nearbyEntities = new List<GameEntity>(100);
			}
			NpcTargetController.m_nearbyEntities.Clear();
			int num = 0;
			int num2 = 0;
			int num3 = Physics.OverlapSphereNonAlloc(base.gameObject.transform.position, range, NpcTargetController.m_nearbyColliders, LayerMap.Detection.LayerMask, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < num3; i++)
			{
				GameEntity gameEntity;
				if (DetectionCollider.TryGetEntityForCollider(NpcTargetController.m_nearbyColliders[i], out gameEntity) && gameEntity.Type == GameEntityType.Npc && gameEntity != base.GameEntity && (!this.CallForHelpRequiresLos || LineOfSight.NpcHasLineOfSight(base.GameEntity, gameEntity)))
				{
					NpcTargetController.m_nearbyEntities.Add(gameEntity);
				}
			}
			CallForHelpExtensions.SortListBySqrMagnitudeDistance(base.GameEntity, NpcTargetController.m_nearbyEntities);
			for (int j = 0; j < NpcTargetController.m_nearbyEntities.Count; j++)
			{
				GameEntity gameEntity2 = NpcTargetController.m_nearbyEntities[j];
				NpcTargetController npcTargetController = gameEntity2.TargetController as NpcTargetController;
				if (npcTargetController && callForHelpData.CallToTags.Matches(npcTargetController.NpcTagsSet) && !npcTargetController.IsLulled)
				{
					if (callForHelpData.MaxHostileCount > 0 && callForHelpData.MaxHostileCount > num2 && callForHelpData.ChanceHostileForEntity(gameEntity2))
					{
						npcTargetController.AddSocialThreat(source.GameEntity, 1f);
						num2++;
						num++;
					}
					else if (callForHelpData.MaxAlertCount > num && npcTargetController.ApplyAlert(source, true))
					{
						num++;
					}
				}
			}
			NpcTargetController.m_nearbyEntities.Clear();
			return num > 0;
		}

		// Token: 0x06003C96 RID: 15510 RVA: 0x0018088C File Offset: 0x0017EA8C
		public float GetHostileSpeedPercentage()
		{
			if (!this.CurrentTarget || this.m_stoppingDistance == null)
			{
				return 1f;
			}
			float sqrMagnitude = (base.GameEntity.PrimaryTargetPoint.transform.position - this.CurrentTarget.PrimaryTargetPoint.transform.position).sqrMagnitude;
			float num = this.m_stoppingDistance.Value * GlobalSettings.Values.Npcs.WeaponDistanceWalkThresh;
			float num2 = num * num;
			if (sqrMagnitude <= num2)
			{
				return 0.25f;
			}
			float num3 = this.m_stoppingDistance.Value * GlobalSettings.Values.Npcs.WeaponDistanceRunThresh;
			float num4 = num3 * num3;
			if (sqrMagnitude >= num4)
			{
				return 1f;
			}
			return ((sqrMagnitude - num2) / (num4 - num2)).Remap(0f, 1f, 0.25f, 1f);
		}

		// Token: 0x06003C97 RID: 15511 RVA: 0x000690B9 File Offset: 0x000672B9
		public override void AddThreat(GameEntity source, float threat, float dmg, bool addAsTagger)
		{
			base.AddThreat(source, threat, dmg, addAsTagger);
			this.AddThreatInternal(source, threat, addAsTagger, false);
		}

		// Token: 0x06003C98 RID: 15512 RVA: 0x000690D1 File Offset: 0x000672D1
		public void AddSocialThreat(GameEntity source, float threat)
		{
			this.AddThreatInternal(source, threat, false, true);
		}

		// Token: 0x06003C99 RID: 15513 RVA: 0x0018095C File Offset: 0x0017EB5C
		private void AddThreatInternal(GameEntity source, float threat, bool addAsTagger, bool isSocialThreat)
		{
			if (!source)
			{
				return;
			}
			NpcTarget npcTarget = this.AddGetTarget(source);
			if (npcTarget != null)
			{
				npcTarget.AddThreat(threat, true);
				if (addAsTagger)
				{
					npcTarget.AddAsTagger(source);
				}
				if (this.HostileTargetCount <= 0)
				{
					this.m_bypassInitialCallForHelp = isSocialThreat;
					this.ProcessTargets();
				}
			}
		}

		// Token: 0x06003C9A RID: 15514 RVA: 0x000690DD File Offset: 0x000672DD
		public void AddThreatToTarget(NetworkEntity source, float threat)
		{
			if (source == null)
			{
				return;
			}
			NpcTarget npcTarget = this.AddGetTarget(source.GameEntity);
			if (npcTarget == null)
			{
				return;
			}
			npcTarget.AddThreat(threat, true);
		}

		// Token: 0x06003C9B RID: 15515 RVA: 0x001809A8 File Offset: 0x0017EBA8
		public bool ApplyAlert(NetworkEntity source, bool overrideHostility)
		{
			bool flag;
			NpcTarget npcTarget = this.AddGetTarget(source.GameEntity, out flag);
			if (npcTarget != null)
			{
				NpcTargetType targetType = npcTarget.TargetType;
				npcTarget.CreateUpdateAlertExternal();
				if (overrideHostility)
				{
					npcTarget.OverrideHostility();
				}
				return flag || targetType != NpcTargetType.Hostile;
			}
			return false;
		}

		// Token: 0x06003C9C RID: 15516 RVA: 0x0004475B File Offset: 0x0004295B
		public void CompleteAlert(NpcAlert alert)
		{
		}

		// Token: 0x06003C9D RID: 15517 RVA: 0x001809EC File Offset: 0x0017EBEC
		public bool CompleteAction(uint targetNetworkId)
		{
			NpcTarget npcTarget;
			if (this.m_currentTargets.TryGetValue(targetNetworkId, out npcTarget))
			{
				npcTarget.CompleteAlert();
				return true;
			}
			return false;
		}

		// Token: 0x06003C9E RID: 15518 RVA: 0x00180A14 File Offset: 0x0017EC14
		public override void EntityDied()
		{
			base.EntityDied();
			if (this.CurrentTarget)
			{
				ICallForHelpSettings callForHelpSettings = this.CallForHelpSettings;
				this.ChanceAlertNearbyNpcs((callForHelpSettings != null) ? callForHelpSettings.Death : null, this.CurrentTarget.NetworkEntity);
			}
			this.m_offensiveTargetNetworkId.Value = 0U;
			this.m_defensiveTargetNetworkId.Value = 0U;
		}

		// Token: 0x06003C9F RID: 15519 RVA: 0x00180A70 File Offset: 0x0017EC70
		public override void BehaviorFlagsUpdated(GameEntity source, DateTime expiration, BehaviorEffectTypeFlags flags, bool adding)
		{
			base.BehaviorFlagsUpdated(source, expiration, flags, adding);
			if (!source || !flags.HasBitFlag(BehaviorEffectTypeFlags.Enraged))
			{
				return;
			}
			NpcTarget npcTarget = this.AddGetTarget(source);
			if (npcTarget != null)
			{
				bool flag;
				if (adding)
				{
					npcTarget.AddThreat(1f, true);
					npcTarget.EnragedUntil = new DateTime?(expiration);
					flag = (this.CurrentTarget != npcTarget.Entity);
				}
				else
				{
					npcTarget.EnragedUntil = null;
					flag = (this.CurrentTarget == npcTarget.Entity);
				}
				if (flag)
				{
					this.ProcessTargets();
				}
			}
		}

		// Token: 0x06003CA0 RID: 15520 RVA: 0x00180B04 File Offset: 0x0017ED04
		public void UpdateMovementMode(NpcMovementMode value)
		{
			if (this.m_movementMode == value)
			{
				return;
			}
			this.m_movementMode = value;
			if (this.m_movementMode == NpcMovementMode.Reset)
			{
				for (int i = 0; i < this.m_currentTargets.Count; i++)
				{
					NpcTarget npcTarget = this.m_currentTargets[i];
					this.m_currentTargets.Remove(npcTarget.NetworkId);
					StaticPool<NpcTarget>.ReturnToPool(npcTarget);
					i--;
				}
				this.HostileTargetCount = 0;
				this.NeutralTargetCount = 0;
				this.AlertCount = 0;
				base.NTargets = new int?(0);
			}
		}

		// Token: 0x06003CA1 RID: 15521 RVA: 0x00069101 File Offset: 0x00067301
		public NpcTarget GetCurrentTarget()
		{
			if (this.m_hostileTarget != null)
			{
				return this.m_hostileTarget;
			}
			if (this.m_neutralTarget != null)
			{
				return this.m_neutralTarget;
			}
			return this.m_alertTarget;
		}

		// Token: 0x06003CA2 RID: 15522 RVA: 0x00180B8C File Offset: 0x0017ED8C
		public GameEntity GetHighestThreatTarget()
		{
			float num = 0f;
			GameEntity gameEntity = null;
			for (int i = 0; i < this.m_currentTargets.Count; i++)
			{
				NpcTarget npcTarget = this.m_currentTargets[i];
				if (npcTarget.TargetType == NpcTargetType.Hostile)
				{
					float distanceWeightedThreat = npcTarget.GetDistanceWeightedThreat();
					if (distanceWeightedThreat > num)
					{
						num = distanceWeightedThreat;
						gameEntity = npcTarget.Entity;
					}
				}
			}
			if (gameEntity)
			{
				this.m_offensiveTargetNetworkId.Value = gameEntity.NetworkEntity.NetworkId.Value;
				this.CurrentTarget = gameEntity;
			}
			else
			{
				this.m_offensiveTargetNetworkId.Value = 0U;
				this.CurrentTarget = null;
			}
			return gameEntity;
		}

		// Token: 0x06003CA3 RID: 15523 RVA: 0x00180C28 File Offset: 0x0017EE28
		public GameEntity GetClosestNeutralTarget()
		{
			GameEntity result = null;
			float maxValue = float.MaxValue;
			for (int i = 0; i < this.m_currentTargets.Count; i++)
			{
				if (this.m_currentTargets[i].TargetType == NpcTargetType.Neutral && this.m_currentTargets[i].SqrDistance < maxValue)
				{
					result = this.m_currentTargets[i].Entity;
				}
			}
			return result;
		}

		// Token: 0x06003CA4 RID: 15524 RVA: 0x00180C90 File Offset: 0x0017EE90
		public NpcAlert GetHighestAlert()
		{
			float num = 0f;
			NpcAlert result = null;
			for (int i = 0; i < this.m_currentTargets.Count; i++)
			{
				NpcTarget npcTarget = this.m_currentTargets[i];
				if (npcTarget.Alert != null)
				{
					float alertValue = npcTarget.GetAlertValue();
					if (alertValue > num)
					{
						num = alertValue;
						result = npcTarget.Alert;
					}
				}
			}
			return result;
		}

		// Token: 0x17000DF6 RID: 3574
		// (get) Token: 0x06003CA5 RID: 15525 RVA: 0x00069127 File Offset: 0x00067327
		internal bool DrawSensesOnlyWhenSelected
		{
			get
			{
				return this.m_drawSensesOnlyWhenSelected;
			}
		}

		// Token: 0x04003B37 RID: 15159
		private const float kTickRate = 1f;

		// Token: 0x04003B38 RID: 15160
		private const float kCanReachCadence = 1f;

		// Token: 0x04003B3E RID: 15166
		private float m_currentSqrMaxDistance = float.MaxValue;

		// Token: 0x04003B3F RID: 15167
		private float m_timeOfNextCombatRefreshCheck;

		// Token: 0x04003B40 RID: 15168
		private float m_timeOfNextCanReachCheck;

		// Token: 0x04003B41 RID: 15169
		private bool m_bypassInitialCallForHelp;

		// Token: 0x04003B42 RID: 15170
		private readonly DictionaryList<uint, NpcTarget> m_currentTargets = new DictionaryList<uint, NpcTarget>(false);

		// Token: 0x04003B43 RID: 15171
		private BehaviorTree m_behaviorTree;

		// Token: 0x04003B44 RID: 15172
		private InteractiveNpc m_interactiveNpc;

		// Token: 0x04003B45 RID: 15173
		private GameEntity m_currentTarget;

		// Token: 0x04003B46 RID: 15174
		private SharedGameEntity m_treeTargetEntity;

		// Token: 0x04003B47 RID: 15175
		private SharedGameObject m_treeTargetGameObject;

		// Token: 0x04003B48 RID: 15176
		private SharedNpcTarget m_treeNpcTarget;

		// Token: 0x04003B49 RID: 15177
		private SharedInt m_hostileTargetCount;

		// Token: 0x04003B4A RID: 15178
		private SharedInt m_neutralTargetCount;

		// Token: 0x04003B4B RID: 15179
		private SharedInt m_alertCount;

		// Token: 0x04003B4C RID: 15180
		private SharedFloat m_stoppingDistance;

		// Token: 0x04003B4D RID: 15181
		private SharedVector3 m_initialPosition;

		// Token: 0x04003B4E RID: 15182
		private SharedVector3 m_initialRotation;

		// Token: 0x04003B4F RID: 15183
		private SharedVector3 m_resetPosition;

		// Token: 0x04003B50 RID: 15184
		private SharedBool m_isInitialized;

		// Token: 0x04003B51 RID: 15185
		private Vector3? m_initialPositionOverride;

		// Token: 0x04003B52 RID: 15186
		private Vector3? m_initialRotationOverride;

		// Token: 0x04003B53 RID: 15187
		private int m_lastFrameHostileCount;

		// Token: 0x04003B54 RID: 15188
		private bool m_lastFrameWasFeared;

		// Token: 0x04003B59 RID: 15193
		private NpcTarget m_hostileTarget;

		// Token: 0x04003B5A RID: 15194
		private NpcTarget m_neutralTarget;

		// Token: 0x04003B5B RID: 15195
		private NpcTarget m_alertTarget;

		// Token: 0x04003B5D RID: 15197
		[SerializeField]
		private bool m_chargeOnHostileAcquired;

		// Token: 0x04003B5E RID: 15198
		[Tooltip("Select this for static destroyable spawners such as Ant Hills and Rat Nests. Will prevent movement and collecting of targets.")]
		[SerializeField]
		private bool m_isStaticDestroyable;

		// Token: 0x04003B5F RID: 15199
		private ICallForHelpSettings m_callForHelpSettings;

		// Token: 0x04003B60 RID: 15200
		private NpcTargetController.CallForHelpPeriodic m_regularPeriodic;

		// Token: 0x04003B61 RID: 15201
		private NpcTargetController.CallForHelpPeriodic m_fleeingPeriodic;

		// Token: 0x04003B62 RID: 15202
		private static List<GameEntity> m_nearbyEntities;

		// Token: 0x04003B63 RID: 15203
		private NpcMovementMode m_movementMode;

		// Token: 0x04003B64 RID: 15204
		[SerializeField]
		private bool m_drawSensesOnlyWhenSelected = true;

		// Token: 0x02000826 RID: 2086
		private class CallForHelpPeriodic
		{
			// Token: 0x06003CA7 RID: 15527 RVA: 0x00069155 File Offset: 0x00067355
			public CallForHelpPeriodic(NpcTargetController controller, CallForHelpPeriodicData periodicData)
			{
				this.m_controller = controller;
				this.m_periodicData = periodicData;
			}

			// Token: 0x06003CA8 RID: 15528 RVA: 0x00180CE8 File Offset: 0x0017EEE8
			public void Update()
			{
				if (this.m_nextCallForHelp == null)
				{
					this.IterateTime();
					return;
				}
				if (Time.time > this.m_nextCallForHelp.Value)
				{
					this.IterateTime();
					this.m_controller.ChanceAlertNearbyNpcs(this.m_periodicData, this.m_controller.CurrentTarget.NetworkEntity);
				}
			}

			// Token: 0x06003CA9 RID: 15529 RVA: 0x00180D44 File Offset: 0x0017EF44
			private void IterateTime()
			{
				this.m_nextCallForHelp = new float?(Time.time + (float)this.m_periodicData.Frequency.RandomWithinRange());
			}

			// Token: 0x06003CAA RID: 15530 RVA: 0x0006916B File Offset: 0x0006736B
			public void ResetTime()
			{
				this.m_nextCallForHelp = null;
			}

			// Token: 0x06003CAB RID: 15531 RVA: 0x00069179 File Offset: 0x00067379
			public void OnDestroy()
			{
				this.m_controller = null;
				this.m_periodicData = null;
				this.m_nextCallForHelp = null;
			}

			// Token: 0x04003B65 RID: 15205
			private NpcTargetController m_controller;

			// Token: 0x04003B66 RID: 15206
			private CallForHelpPeriodicData m_periodicData;

			// Token: 0x04003B67 RID: 15207
			private float? m_nextCallForHelp;
		}
	}
}
