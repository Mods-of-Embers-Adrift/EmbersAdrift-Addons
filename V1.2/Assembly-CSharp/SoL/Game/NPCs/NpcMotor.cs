using System;
using System.Collections.Generic;
using SoL.Game.EffectSystem;
using SoL.Game.NPCs.Interactions;
using SoL.Game.Settings;
using SoL.Game.Targeting;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.AI;

namespace SoL.Game.NPCs
{
	// Token: 0x02000806 RID: 2054
	public class NpcMotor : GameEntityComponent, IMotor
	{
		// Token: 0x17000D9D RID: 3485
		// (get) Token: 0x06003B79 RID: 15225 RVA: 0x000683DE File Offset: 0x000665DE
		// (set) Token: 0x06003B7A RID: 15226 RVA: 0x000683E6 File Offset: 0x000665E6
		public float OverallSpeedMultiplier { get; set; } = 1f;

		// Token: 0x17000D9E RID: 3486
		// (get) Token: 0x06003B7B RID: 15227 RVA: 0x000683EF File Offset: 0x000665EF
		// (set) Token: 0x06003B7C RID: 15228 RVA: 0x000683F7 File Offset: 0x000665F7
		public float PathSpeedMultiplier { get; set; } = 1f;

		// Token: 0x17000D9F RID: 3487
		// (get) Token: 0x06003B7D RID: 15229 RVA: 0x00068400 File Offset: 0x00066600
		// (set) Token: 0x06003B7E RID: 15230 RVA: 0x00068408 File Offset: 0x00066608
		public bool RunOnPath { get; set; }

		// Token: 0x06003B7F RID: 15231 RVA: 0x0017B70C File Offset: 0x0017990C
		public void UpdateMovementMode(NpcMovementMode value)
		{
			if (this.m_movementMode == value)
			{
				return;
			}
			this.m_movementMode = value;
			this.m_timeInCurrentMovementMode = 0f;
			if (this.UnityNavAgent != null)
			{
				this.UnityNavAgent.obstacleAvoidanceType = this.m_movementMode.GetAvoidanceType();
				this.UpdateAvoidancePriority();
			}
		}

		// Token: 0x06003B80 RID: 15232 RVA: 0x00068411 File Offset: 0x00066611
		private void UpdateAvoidancePriority()
		{
			if (this.UnityNavAgent != null)
			{
				this.UnityNavAgent.avoidancePriority = this.m_movementMode.GetNavMeshAgentPriority();
			}
			this.m_lastAvoidancePriorityUpdate = Time.time;
		}

		// Token: 0x17000DA0 RID: 3488
		// (get) Token: 0x06003B81 RID: 15233 RVA: 0x00068442 File Offset: 0x00066642
		// (set) Token: 0x06003B82 RID: 15234 RVA: 0x0006844A File Offset: 0x0006664A
		public NavMeshAgent UnityNavAgent { get; private set; }

		// Token: 0x17000DA1 RID: 3489
		// (get) Token: 0x06003B83 RID: 15235 RVA: 0x00068453 File Offset: 0x00066653
		private bool m_showAreaCostOverrides
		{
			get
			{
				return this.m_areaCostOverrideProfile == null;
			}
		}

		// Token: 0x06003B84 RID: 15236 RVA: 0x0017B760 File Offset: 0x00179960
		private void Awake()
		{
			if (base.GameEntity != null)
			{
				base.GameEntity.Motor = this;
			}
			if (this.m_navMeshAgent)
			{
				this.m_navMeshAgent = this.m_navMeshAgent.CopyNavMeshAgentTo(base.GameEntity.gameObject);
				this.m_navMeshAgent.obstacleAvoidanceType = GlobalSettings.Values.Npcs.MotorObstacleAvoidanceType;
				this.m_navMeshAgent.avoidancePriority = UnityEngine.Random.Range(0, 100);
				this.UnityNavAgent = this.m_navMeshAgent;
				this.m_navAgentBaseOffset = this.m_navMeshAgent.baseOffset;
				if (this.m_navAgentSwimOffset == 0f)
				{
					this.m_navAgentSwimOffset = -0.05f;
				}
				this.m_canSwim = ((this.m_navMeshAgent.areaMask & 64) == 64);
				this.m_runSpeed = this.m_navMeshAgent.speed;
				this.m_walkSpeed = this.m_runSpeed * 0.5f;
				this.m_alertLerp = UnityEngine.Random.Range(0.3f, 0.7f);
				this.SetNavMeshAreaCosts();
			}
		}

		// Token: 0x06003B85 RID: 15237 RVA: 0x0017B86C File Offset: 0x00179A6C
		private void Start()
		{
			if (base.GameEntity == null)
			{
				return;
			}
			if (this.m_speedSetter)
			{
				this.m_walkSpeed = this.m_speedSetter.GetWalkSpeed();
				this.m_runSpeed = this.m_speedSetter.GetRunSpeed();
			}
			float? num = base.GameEntity.CharacterData ? base.GameEntity.CharacterData.TransformScale : null;
			if (num != null)
			{
				this.m_walkSpeed = this.m_walkSpeed.PercentModification(num.Value);
				this.m_runSpeed = this.m_runSpeed.PercentModification(num.Value);
			}
			bool flag = this.UnityNavAgent != null;
			if (flag && num != null)
			{
				this.m_navMeshAgent.height = this.m_navMeshAgent.height.PercentModification(num.Value);
				this.m_navAgentBaseOffset = this.m_navAgentBaseOffset.PercentModification(num.Value);
				this.m_navAgentSwimOffset = this.m_navAgentSwimOffset.PercentModification(num.Value);
			}
			if (flag && base.GameEntity.VitalsReplicator != null)
			{
				base.GameEntity.VitalsReplicator.CurrentHealthState.Changed += this.CurrentHealthStateOnChanged;
			}
			if (flag)
			{
				this.m_defaultAcceleration = this.m_navMeshAgent.acceleration;
				this.m_walkableCost = this.UnityNavAgent.GetAreaCost(0);
				this.m_pathCost = this.UnityNavAgent.GetAreaCost(3);
				this.m_currentPathCost = this.m_pathCost;
			}
			if (base.GameEntity.TargetController != null)
			{
				base.GameEntity.TargetController.TryGetAsType(out this.m_npcTargetController);
			}
			else
			{
				this.m_npcTargetController = base.gameObject.GetComponent<FlatwormController>();
			}
			this.m_initialized = flag;
		}

		// Token: 0x06003B86 RID: 15238 RVA: 0x00068461 File Offset: 0x00066661
		private void OnDestroy()
		{
			if (base.GameEntity.VitalsReplicator != null)
			{
				base.GameEntity.VitalsReplicator.CurrentHealthState.Changed -= this.CurrentHealthStateOnChanged;
			}
		}

		// Token: 0x06003B87 RID: 15239 RVA: 0x0017BA4C File Offset: 0x00179C4C
		private void Update()
		{
			if (!this.m_initialized)
			{
				return;
			}
			if (!base.GameEntity.ServerNpcController.EnableBehavior)
			{
				if (this.UnityNavAgent.pathPending)
				{
					this.UnityNavAgent.isStopped = true;
					this.UnityNavAgent.ResetPath();
				}
				return;
			}
			float num = this.GetSpeed();
			if (this.m_modulateSpeed)
			{
				if (this.m_modulateX == null)
				{
					this.m_modulateX = new float?((float)base.GetInstanceID());
				}
				float t = Mathf.PerlinNoise(this.m_modulateX.Value, Time.time * 0.1f);
				num = Mathf.Lerp(num * this.m_modulateMinimum, num, t);
			}
			Vector3 vector = Vector3.zero;
			vector = this.UnityNavAgent.velocity;
			Vector3 normalized = base.gameObject.transform.InverseTransformVector(vector).normalized;
			Vector2 rawLocomotion = new Vector2(normalized.x, normalized.z) * num;
			base.GameEntity.AnimatorReplicator.Speed = 1f;
			base.GameEntity.AnimatorReplicator.RawLocomotion = rawLocomotion;
			this.UnityNavAgent.speed = num;
			if (this.UnityNavAgent.enabled && !this.UnityNavAgent.isStopped && this.UnityNavAgent.isOnNavMesh)
			{
				float remainingDistance = this.UnityNavAgent.remainingDistance;
				float num2 = (base.GameEntity.Targetable != null) ? base.GameEntity.Targetable.DistanceBuffer : (this.UnityNavAgent.stoppingDistance * 1.2f);
				float acceleration = (remainingDistance < float.PositiveInfinity && remainingDistance < num2) ? 500f : this.m_defaultAcceleration;
				this.UnityNavAgent.acceleration = acceleration;
			}
			if (this.m_movementMode.GetAvoidanceType() != ObstacleAvoidanceType.NoObstacleAvoidance && Time.time > this.m_lastAvoidancePriorityUpdate + 30f)
			{
				this.UpdateAvoidancePriority();
			}
			this.m_timeInCurrentMovementMode += Time.deltaTime;
			this.UpdateWaterOffset();
			this.UpdateResetTime();
			this.UpdatePathAreaCost();
		}

		// Token: 0x06003B88 RID: 15240 RVA: 0x0017BC4C File Offset: 0x00179E4C
		private float GetSpeed()
		{
			if (this.m_movementMode == NpcMovementMode.Reset)
			{
				return this.m_runSpeed * 2f;
			}
			bool flag = base.GameEntity.ServerNpcController != null;
			bool flag2 = flag && base.GameEntity.ServerNpcController.Interactive != null && base.GameEntity.ServerNpcController.Interactive is NpcWaypoint;
			float num = (this.RunOnPath && flag2) ? this.m_runSpeed : this.m_walkSpeed;
			if (this.m_npcTargetController != null)
			{
				if (this.m_npcTargetController.HostileTargetCount > 0)
				{
					num = Mathf.Lerp(this.m_walkSpeed, this.m_runSpeed, this.m_npcTargetController.GetHostileSpeedPercentage());
				}
				else if (this.m_npcTargetController.AlertCount > 0)
				{
					num = Mathf.Lerp(this.m_walkSpeed, this.m_runSpeed, this.m_alertLerp);
				}
			}
			if (this.m_movementMode == NpcMovementMode.MoveBack)
			{
				num = this.m_walkSpeed;
			}
			num = ((this.m_rootEffects.Count > 0 || this.m_speedModifier <= -1f) ? 0f : num.PercentModification(this.m_speedModifier));
			num *= this.OverallSpeedMultiplier;
			if (flag)
			{
				if (base.GameEntity.ServerNpcController.IsFeared())
				{
					num *= 0.25f;
				}
				else if (flag2)
				{
					num *= this.PathSpeedMultiplier;
				}
			}
			return num;
		}

		// Token: 0x06003B89 RID: 15241 RVA: 0x0017BDA0 File Offset: 0x00179FA0
		private void UpdateWaterOffset()
		{
			if (!this.m_canSwim || ZoneSettings.Instance == null || ZoneSettings.Instance.Profile == null || !ZoneSettings.SettingsProfile.NpcsCheckForWater || !base.GameEntity || !base.GameEntity.CharacterData || !this.m_navMeshAgent)
			{
				return;
			}
			if (Time.time >= this.m_timeOfNextWaterCheck)
			{
				this.m_isUnderWater = false;
				this.m_waterObject = null;
				this.m_timeOfNextWaterCheck = Time.time + 1f;
				RaycastHit raycastHit;
				if (Physics.Raycast(base.GameEntity.gameObject.transform.position, Vector3.up, out raycastHit, 8f, LayerMap.Water.LayerMask, QueryTriggerInteraction.Ignore))
				{
					this.m_isUnderWater = (raycastHit.collider != null && raycastHit.collider.gameObject.CompareTag("Water"));
					if (this.m_isUnderWater)
					{
						this.m_waterObject = raycastHit.collider.gameObject;
					}
				}
			}
			float baseOffset = this.m_navAgentBaseOffset;
			if (this.m_isUnderWater && this.m_waterObject)
			{
				Vector3 position = base.GameEntity.gameObject.transform.position;
				position.y -= this.m_navMeshAgent.baseOffset;
				float num = this.m_waterObject.transform.position.y - position.y;
				baseOffset = num + this.m_navAgentSwimOffset;
				if (num < this.m_navMeshAgent.height * 0.75f)
				{
					baseOffset = this.m_navAgentBaseOffset;
					this.m_isUnderWater = false;
					this.m_waterObject = null;
				}
			}
			this.m_navMeshAgent.baseOffset = baseOffset;
			if (base.GameEntity.CharacterData.IsSwimming != this.m_isUnderWater)
			{
				base.GameEntity.CharacterData.IsSwimming = this.m_isUnderWater;
			}
		}

		// Token: 0x06003B8A RID: 15242 RVA: 0x0017BF90 File Offset: 0x0017A190
		private void UpdateResetTime()
		{
			if (this.m_movementMode == NpcMovementMode.Reset && this.m_timeInCurrentMovementMode > 60f && this.m_npcTargetController != null && this.m_npcTargetController.ResetPosition != null)
			{
				base.GameEntity.gameObject.transform.position = this.m_npcTargetController.ResetPosition.Value;
				this.m_timeInCurrentMovementMode = 0f;
				if (base.GameEntity && base.GameEntity.ServerNpcController)
				{
					base.GameEntity.ServerNpcController.InterruptBehavior();
				}
			}
		}

		// Token: 0x06003B8B RID: 15243 RVA: 0x0017C038 File Offset: 0x0017A238
		private void UpdatePathAreaCost()
		{
			if (!this.UnityNavAgent || this.m_npcTargetController == null || !this.UnityNavAgent.enabled || !this.UnityNavAgent.isOnNavMesh)
			{
				return;
			}
			if (this.m_npcTargetController.HostileTargetCount > 0)
			{
				if (!Mathf.Approximately(this.m_currentPathCost, this.m_walkableCost))
				{
					this.UnityNavAgent.SetAreaCost(3, this.m_walkableCost);
					this.m_currentPathCost = this.m_walkableCost;
					return;
				}
			}
			else if (!Mathf.Approximately(this.m_currentPathCost, this.m_pathCost))
			{
				this.UnityNavAgent.SetAreaCost(3, this.m_pathCost);
				this.m_currentPathCost = this.m_pathCost;
			}
		}

		// Token: 0x06003B8C RID: 15244 RVA: 0x0017C0E8 File Offset: 0x0017A2E8
		private void SetNavMeshAreaCosts()
		{
			if (!this.UnityNavAgent)
			{
				return;
			}
			NpcMotor.NavMeshAreaCost[] array = this.m_areaCostOverrideProfile ? this.m_areaCostOverrideProfile.AreaCostOverrides : this.m_areaCostOverrides;
			if (array == null || array.Length == 0)
			{
				return;
			}
			foreach (NpcMotor.NavMeshAreaCost navMeshAreaCost in array)
			{
				if (navMeshAreaCost != null)
				{
					this.UnityNavAgent.SetAreaCost(navMeshAreaCost.Index, navMeshAreaCost.Cost);
					if (navMeshAreaCost.Index == 3)
					{
						this.m_currentPathCost = navMeshAreaCost.Cost;
					}
				}
			}
		}

		// Token: 0x06003B8D RID: 15245 RVA: 0x0017C170 File Offset: 0x0017A370
		private void CurrentHealthStateOnChanged(HealthState obj)
		{
			if (obj == HealthState.Unconscious || obj == HealthState.Dead)
			{
				if (this.UnityNavAgent && this.UnityNavAgent.enabled)
				{
					this.UnityNavAgent.enabled = false;
					return;
				}
			}
			else
			{
				if (this.UnityNavAgent && !this.UnityNavAgent.enabled)
				{
					this.UnityNavAgent.enabled = true;
				}
				this.SetNavMeshAreaCosts();
			}
		}

		// Token: 0x17000DA2 RID: 3490
		// (get) Token: 0x06003B8E RID: 15246 RVA: 0x00068497 File Offset: 0x00066697
		// (set) Token: 0x06003B8F RID: 15247 RVA: 0x0006849F File Offset: 0x0006669F
		float IMotor.SpeedModifier
		{
			get
			{
				return this.m_speedModifier;
			}
			set
			{
				this.m_speedModifier = value;
			}
		}

		// Token: 0x06003B90 RID: 15248 RVA: 0x000684A8 File Offset: 0x000666A8
		void IMotor.ApplyRootEffect(bool adding, CombatEffect effect)
		{
			if (effect == null)
			{
				return;
			}
			if (adding)
			{
				this.m_rootEffects.Add(effect);
				return;
			}
			this.m_rootEffects.Remove(effect);
		}

		// Token: 0x17000DA3 RID: 3491
		// (get) Token: 0x06003B91 RID: 15249 RVA: 0x000684CC File Offset: 0x000666CC
		bool IMotor.IsRooted
		{
			get
			{
				return this.m_rootEffects.Count > 0;
			}
		}

		// Token: 0x040039E4 RID: 14820
		private const int kWaterNavMask = 64;

		// Token: 0x040039E5 RID: 14821
		private const float kMaxTimeInResetMode = 60f;

		// Token: 0x040039E6 RID: 14822
		private const int kWalkableIndex = 0;

		// Token: 0x040039E7 RID: 14823
		private float m_walkableCost = 2f;

		// Token: 0x040039E8 RID: 14824
		private const int kPathIndex = 3;

		// Token: 0x040039E9 RID: 14825
		private float m_pathCost = 1f;

		// Token: 0x040039EA RID: 14826
		private float m_currentPathCost = 1f;

		// Token: 0x040039EB RID: 14827
		[SerializeField]
		private bool m_showDebugData;

		// Token: 0x040039EC RID: 14828
		[SerializeField]
		private float m_navAgentSwimOffset;

		// Token: 0x040039ED RID: 14829
		[SerializeField]
		private bool m_modulateSpeed;

		// Token: 0x040039EE RID: 14830
		[SerializeField]
		private float m_modulateMinimum = 0.6f;

		// Token: 0x040039EF RID: 14831
		private float? m_modulateX;

		// Token: 0x040039F3 RID: 14835
		private const float kAvoidancePriorityUpdateTime = 30f;

		// Token: 0x040039F4 RID: 14836
		private float m_lastAvoidancePriorityUpdate;

		// Token: 0x040039F5 RID: 14837
		private float m_timeInCurrentMovementMode;

		// Token: 0x040039F6 RID: 14838
		private bool m_canSwim;

		// Token: 0x040039F7 RID: 14839
		private float m_navAgentBaseOffset;

		// Token: 0x040039F8 RID: 14840
		private NpcMovementMode m_movementMode;

		// Token: 0x040039F9 RID: 14841
		private bool m_initialized;

		// Token: 0x040039FB RID: 14843
		[SerializeField]
		private NpcSpeedSetter m_speedSetter;

		// Token: 0x040039FC RID: 14844
		[SerializeField]
		private NavMeshAgent m_navMeshAgent;

		// Token: 0x040039FD RID: 14845
		[SerializeField]
		private AreaCostOverrideProfile m_areaCostOverrideProfile;

		// Token: 0x040039FE RID: 14846
		[SerializeField]
		private NpcMotor.NavMeshAreaCost[] m_areaCostOverrides;

		// Token: 0x040039FF RID: 14847
		private readonly HashSet<CombatEffect> m_rootEffects = new HashSet<CombatEffect>(10);

		// Token: 0x04003A00 RID: 14848
		private INpcTargetController m_npcTargetController;

		// Token: 0x04003A01 RID: 14849
		private float m_speedModifier;

		// Token: 0x04003A02 RID: 14850
		private float m_walkSpeed = 1.56595f;

		// Token: 0x04003A03 RID: 14851
		private float m_runSpeed = 5f;

		// Token: 0x04003A04 RID: 14852
		private float m_alertLerp = 0.5f;

		// Token: 0x04003A05 RID: 14853
		private float m_defaultAcceleration;

		// Token: 0x04003A06 RID: 14854
		private const float kDeceleration = 500f;

		// Token: 0x04003A07 RID: 14855
		private const float kAgentHeightFraction = 0.75f;

		// Token: 0x04003A08 RID: 14856
		private const float kWaterCheckCadence = 1f;

		// Token: 0x04003A09 RID: 14857
		private float m_timeOfNextWaterCheck;

		// Token: 0x04003A0A RID: 14858
		private bool m_isUnderWater;

		// Token: 0x04003A0B RID: 14859
		private GameObject m_waterObject;

		// Token: 0x02000807 RID: 2055
		[Serializable]
		internal class NavMeshAreaCost
		{
			// Token: 0x17000DA4 RID: 3492
			// (get) Token: 0x06003B93 RID: 15251 RVA: 0x000684DC File Offset: 0x000666DC
			public int Index
			{
				get
				{
					return this.m_area;
				}
			}

			// Token: 0x17000DA5 RID: 3493
			// (get) Token: 0x06003B94 RID: 15252 RVA: 0x000684E4 File Offset: 0x000666E4
			public float Cost
			{
				get
				{
					return this.m_cost;
				}
			}

			// Token: 0x04003A0C RID: 14860
			private const string kCostGroupName = "Cost";

			// Token: 0x04003A0D RID: 14861
			[SerializeField]
			private int m_area;

			// Token: 0x04003A0E RID: 14862
			[SerializeField]
			private float m_cost = 1f;
		}
	}
}
