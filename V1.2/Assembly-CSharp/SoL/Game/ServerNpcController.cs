using System;
using System.Collections;
using BehaviorDesigner.Runtime;
using SoL.Behavior.Tasks;
using SoL.Game.Behavior;
using SoL.Game.EffectSystem;
using SoL.Game.Interactives;
using SoL.Game.NPCs;
using SoL.Game.NPCs.Interactions;
using SoL.Game.Settings;
using SoL.Game.Spawning;
using SoL.Managers;
using SoL.Networking;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000571 RID: 1393
	public class ServerNpcController : GameEntityComponent, ISpawnable
	{
		// Token: 0x170008E5 RID: 2277
		// (get) Token: 0x06002AEE RID: 10990 RVA: 0x0005DC4B File Offset: 0x0005BE4B
		// (set) Token: 0x06002AEF RID: 10991 RVA: 0x0005DC53 File Offset: 0x0005BE53
		public NpcInteractionFlags InteractionFlags { get; set; }

		// Token: 0x170008E6 RID: 2278
		// (get) Token: 0x06002AF0 RID: 10992 RVA: 0x0005DC5C File Offset: 0x0005BE5C
		// (set) Token: 0x06002AF1 RID: 10993 RVA: 0x0005DC64 File Offset: 0x0005BE64
		public bool DespawnOnDeath { get; set; }

		// Token: 0x170008E7 RID: 2279
		// (get) Token: 0x06002AF2 RID: 10994 RVA: 0x0005DC6D File Offset: 0x0005BE6D
		public bool BypassCanReachCheck
		{
			get
			{
				return !this.m_requireSpawn && this.m_bypassCanReachCheck;
			}
		}

		// Token: 0x170008E8 RID: 2280
		// (get) Token: 0x06002AF3 RID: 10995 RVA: 0x0005DC7F File Offset: 0x0005BE7F
		public NpcMotor Motor
		{
			get
			{
				return this.m_motor;
			}
		}

		// Token: 0x06002AF4 RID: 10996 RVA: 0x00145178 File Offset: 0x00143378
		private bool ShouldDespawnOnDeath()
		{
			if (this.DespawnOnDeath)
			{
				return true;
			}
			if (base.GameEntity && base.GameEntity.Interactive != null)
			{
				InteractiveNpc interactiveNpc = base.GameEntity.Interactive as InteractiveNpc;
				return interactiveNpc != null && interactiveNpc.NpcContributed;
			}
			return false;
		}

		// Token: 0x170008E9 RID: 2281
		// (get) Token: 0x06002AF5 RID: 10997 RVA: 0x0005DC87 File Offset: 0x0005BE87
		public BehaviorTree Tree
		{
			get
			{
				return this.m_behaviorTree;
			}
		}

		// Token: 0x170008EA RID: 2282
		// (get) Token: 0x06002AF6 RID: 10998 RVA: 0x0005DC8F File Offset: 0x0005BE8F
		public bool EnableBehavior
		{
			get
			{
				if (!this.m_requireObservers)
				{
					return true;
				}
				if (base.GameEntity.NetworkEntity.NObservers > 0)
				{
					this.m_timeOfLastEnabled = Time.time;
					return true;
				}
				return Time.time - this.m_timeOfLastEnabled < 300f;
			}
		}

		// Token: 0x170008EB RID: 2283
		// (get) Token: 0x06002AF7 RID: 10999 RVA: 0x0005DCCE File Offset: 0x0005BECE
		// (set) Token: 0x06002AF8 RID: 11000 RVA: 0x001451D0 File Offset: 0x001433D0
		public BehaviorEffectTypeFlags BehaviorFlagsInternal
		{
			get
			{
				return this.m_behaviorFlagsInternal;
			}
			set
			{
				if (this.m_behaviorFlagsInternal == value)
				{
					return;
				}
				bool flag = this.IsStunned();
				bool flag2 = this.IsFeared();
				this.m_behaviorFlagsInternal = value;
				if (this.m_behaviorFlagsWrapper != null)
				{
					this.m_behaviorFlagsWrapper.Value.Flags = this.m_behaviorFlagsInternal;
				}
				bool flag3 = this.IsStunned();
				bool flag4 = this.IsFeared();
				if (flag3 != flag || flag4 != flag2)
				{
					this.InterruptBehavior();
					if (flag3 && this.m_motor && this.m_motor.UnityNavAgent && this.m_motor.UnityNavAgent.enabled && this.m_motor.UnityNavAgent.isOnNavMesh)
					{
						this.m_motor.UnityNavAgent.isStopped = true;
					}
				}
			}
		}

		// Token: 0x06002AF9 RID: 11001 RVA: 0x0005DCD6 File Offset: 0x0005BED6
		public bool PreventSkillExecution()
		{
			return this.IsStunned() || this.IsFeared();
		}

		// Token: 0x06002AFA RID: 11002 RVA: 0x0005DCE8 File Offset: 0x0005BEE8
		public bool IsStunned()
		{
			return this.m_behaviorFlagsInternal.HasBitFlag(BehaviorEffectTypeFlags.Stunned);
		}

		// Token: 0x06002AFB RID: 11003 RVA: 0x0005DCF6 File Offset: 0x0005BEF6
		public bool IsDazed()
		{
			return this.m_behaviorFlagsInternal.HasBitFlag(BehaviorEffectTypeFlags.Dazed);
		}

		// Token: 0x06002AFC RID: 11004 RVA: 0x0005DD04 File Offset: 0x0005BF04
		public bool IsFeared()
		{
			return !this.IsStunned() && this.m_behaviorFlagsInternal.HasBitFlag(BehaviorEffectTypeFlags.Feared);
		}

		// Token: 0x170008EC RID: 2284
		// (get) Token: 0x06002AFD RID: 11005 RVA: 0x0005DD1C File Offset: 0x0005BF1C
		// (set) Token: 0x06002AFE RID: 11006 RVA: 0x00145290 File Offset: 0x00143490
		public NpcMovementMode MovementMode
		{
			get
			{
				return this.m_movementMode;
			}
			set
			{
				if (this.m_movementMode == value)
				{
					return;
				}
				this.m_movementMode = value;
				if (this.m_motor)
				{
					this.m_motor.UpdateMovementMode(this.m_movementMode);
				}
				if (this.m_targetController)
				{
					this.m_targetController.UpdateMovementMode(this.m_movementMode);
				}
				if (this.m_skillsController)
				{
					this.m_skillsController.UpdateMovementMode(this.m_movementMode);
				}
				if (this.m_movementMode != NpcMovementMode.Interact && this.Interactive != null)
				{
					this.Interactive = null;
				}
			}
		}

		// Token: 0x170008ED RID: 2285
		// (get) Token: 0x06002AFF RID: 11007 RVA: 0x0005DD24 File Offset: 0x0005BF24
		// (set) Token: 0x06002B00 RID: 11008 RVA: 0x00145328 File Offset: 0x00143528
		public NpcInteractive Interactive
		{
			get
			{
				return this.m_npcInteractive;
			}
			set
			{
				if (this.m_npcInteractive == value)
				{
					return;
				}
				if (this.m_npcInteractive != null)
				{
					this.m_npcInteractive.DeregisterOccupant(base.GameEntity);
				}
				this.m_npcInteractive = value;
				if (this.m_npcInteractive != null)
				{
					this.m_npcInteractive.RegisterOccupant(base.GameEntity);
				}
			}
		}

		// Token: 0x06002B01 RID: 11009 RVA: 0x0014538C File Offset: 0x0014358C
		private void Awake()
		{
			base.GameEntity.Spawnable = this;
			base.GameEntity.ServerNpcController = this;
			if (ServerNpcController.m_despawnWait == null)
			{
				ServerNpcController.m_despawnWait = new WaitForSeconds(GlobalSettings.Values.Npcs.CorpseDecayTime);
				ServerNpcController.m_endInteractionsWait = new WaitForSeconds(5f);
			}
		}

		// Token: 0x06002B02 RID: 11010 RVA: 0x0005DD2C File Offset: 0x0005BF2C
		private void Start()
		{
			if (!this.m_requireSpawn)
			{
				NpcSpawnProfileV2 npcSpawnProfileV = ScriptableObject.CreateInstance<NpcSpawnProfileV2>();
				npcSpawnProfileV.LoadStaticSpawnData(this.m_staticSpawnData);
				npcSpawnProfileV.StaticSpawn(this.m_staticSpawnData, base.GameEntity);
			}
		}

		// Token: 0x06002B03 RID: 11011 RVA: 0x001453E0 File Offset: 0x001435E0
		private void OnDestroy()
		{
			if (base.GameEntity.TargetController)
			{
				base.GameEntity.TargetController.ClearBehaviorReferences();
			}
			this.DisableBehaviorTree();
			if (base.GameEntity.VitalsReplicator != null)
			{
				base.GameEntity.VitalsReplicator.CurrentHealthState.Changed -= this.CurrentHealthStateOnChanged;
			}
			if (this.Interactive != null)
			{
				this.Interactive.DeregisterOccupant(base.GameEntity);
			}
			if (NullifyMemoryLeakSettings.CleanServerNpcController)
			{
				this.m_manager = null;
				this.m_interrupt = null;
				this.m_behaviorFlagsWrapper = null;
				this.m_npcInteractive = null;
			}
		}

		// Token: 0x06002B04 RID: 11012 RVA: 0x0005DD58 File Offset: 0x0005BF58
		private void OnValidate()
		{
			StaticSpawnData staticSpawnData = this.m_staticSpawnData;
			if (staticSpawnData == null)
			{
				return;
			}
			PortraitConfig portraitConfig = staticSpawnData.PortraitConfig;
			if (portraitConfig == null)
			{
				return;
			}
			portraitConfig.Normalize();
		}

		// Token: 0x06002B05 RID: 11013 RVA: 0x0014548C File Offset: 0x0014368C
		private void Spawned()
		{
			if (base.GameEntity.VitalsReplicator != null)
			{
				base.GameEntity.VitalsReplicator.CurrentHealthState.Changed += this.CurrentHealthStateOnChanged;
			}
			if (this.m_behaviorTree != null)
			{
				this.m_interrupt = this.m_behaviorTree.GetSharedVariable("Interrupt");
				this.m_behaviorFlagsWrapper = this.m_behaviorTree.GetSharedVariable("BehaviorFlags");
			}
			this.EnableBehaviorTree();
		}

		// Token: 0x06002B06 RID: 11014 RVA: 0x0005DD74 File Offset: 0x0005BF74
		private void EnableBehaviorTree()
		{
			if (this.m_behaviorTree != null && !this.m_registeredWithBehaviorManager)
			{
				this.m_behaviorTree.EnableBehavior();
				this.m_manager = ServerGameManager.NpcBehaviorManager.RegisterNpc(this);
				this.m_registeredWithBehaviorManager = true;
			}
		}

		// Token: 0x06002B07 RID: 11015 RVA: 0x0005DDAF File Offset: 0x0005BFAF
		private void DisableBehaviorTree()
		{
			if (this.m_behaviorTree != null && this.m_registeredWithBehaviorManager)
			{
				this.m_behaviorTree.DisableBehavior();
				ServerGameManager.NpcBehaviorManager.UnregisterNpc(this);
				this.m_manager = null;
				this.m_registeredWithBehaviorManager = false;
			}
		}

		// Token: 0x06002B08 RID: 11016 RVA: 0x0005DDEB File Offset: 0x0005BFEB
		public bool BehaviorTick()
		{
			if (!this.EnableBehavior || this.m_manager == null || this.IsStunned())
			{
				return false;
			}
			this.m_manager.Tick(this.m_behaviorTree);
			return true;
		}

		// Token: 0x06002B09 RID: 11017 RVA: 0x0005DE1F File Offset: 0x0005C01F
		private void CurrentHealthStateOnChanged(HealthState obj)
		{
			if (obj == HealthState.Dead)
			{
				this.DisableBehaviorTree();
				base.StartCoroutine("DestroyCo");
			}
		}

		// Token: 0x06002B0A RID: 11018 RVA: 0x0005DE37 File Offset: 0x0005C037
		public void ResetDestructionTimer()
		{
			this.m_destroyWaitElapsed = 0f;
		}

		// Token: 0x06002B0B RID: 11019 RVA: 0x0005DE44 File Offset: 0x0005C044
		private IEnumerator DestroyCo()
		{
			float num = this.ShouldDespawnOnDeath() ? 5f : GlobalSettings.Values.Npcs.CorpseDecayTime;
			float waitTime = num;
			InteractiveNpc interactiveNpc;
			int num2;
			if (base.GameEntity.Interactive != null && base.GameEntity.Interactive.TryGetAsType(out interactiveNpc) && interactiveNpc.TryGetAdditionalCorpseWaitTimes(out num2))
			{
				waitTime += num * (float)num2;
			}
			this.m_destroyWaitElapsed = 0f;
			while (this.m_destroyWaitElapsed < waitTime)
			{
				this.m_destroyWaitElapsed += Time.deltaTime;
				yield return null;
			}
			if (base.GameEntity.Interactive != null)
			{
				base.GameEntity.Interactive.EndAllInteractions();
				yield return ServerNpcController.m_endInteractionsWait;
			}
			UnityEngine.Object.Destroy(base.GameEntity.gameObject);
			yield break;
		}

		// Token: 0x06002B0C RID: 11020 RVA: 0x00145510 File Offset: 0x00143710
		public void InterruptBehavior()
		{
			if (this.m_interrupt != null && this.m_manager != null && this.m_behaviorTree != null)
			{
				this.m_interrupt.Value = true;
				this.m_manager.Tick(this.m_behaviorTree);
			}
		}

		// Token: 0x06002B0D RID: 11021 RVA: 0x0005DE53 File Offset: 0x0005C053
		void ISpawnable.Spawned()
		{
			this.Spawned();
		}

		// Token: 0x04002B2D RID: 11053
		[SerializeField]
		private bool m_requireObservers = true;

		// Token: 0x04002B2E RID: 11054
		[SerializeField]
		private bool m_requireSpawn = true;

		// Token: 0x04002B2F RID: 11055
		[SerializeField]
		private StaticSpawnData m_staticSpawnData;

		// Token: 0x04002B30 RID: 11056
		[SerializeField]
		private bool m_bypassCanReachCheck;

		// Token: 0x04002B31 RID: 11057
		[SerializeField]
		private BehaviorTree m_behaviorTree;

		// Token: 0x04002B32 RID: 11058
		[SerializeField]
		private NpcMotor m_motor;

		// Token: 0x04002B33 RID: 11059
		[SerializeField]
		private NpcTargetController m_targetController;

		// Token: 0x04002B34 RID: 11060
		[SerializeField]
		private NpcSkillsController m_skillsController;

		// Token: 0x04002B37 RID: 11063
		public const float kBehaviorDisableDelay = 300f;

		// Token: 0x04002B38 RID: 11064
		private float m_timeOfLastEnabled = -300f;

		// Token: 0x04002B39 RID: 11065
		private bool m_registeredWithBehaviorManager;

		// Token: 0x04002B3A RID: 11066
		private BehaviorManager m_manager;

		// Token: 0x04002B3B RID: 11067
		private SharedBool m_interrupt;

		// Token: 0x04002B3C RID: 11068
		private SharedBehaviorEffectFlags m_behaviorFlagsWrapper;

		// Token: 0x04002B3D RID: 11069
		private BehaviorEffectTypeFlags m_behaviorFlagsInternal;

		// Token: 0x04002B3E RID: 11070
		private NpcMovementMode m_movementMode;

		// Token: 0x04002B3F RID: 11071
		private NpcInteractive m_npcInteractive;

		// Token: 0x04002B40 RID: 11072
		private const float kEndInteractionWaitTime = 5f;

		// Token: 0x04002B41 RID: 11073
		private static WaitForSeconds m_despawnWait;

		// Token: 0x04002B42 RID: 11074
		private static WaitForSeconds m_endInteractionsWait;

		// Token: 0x04002B43 RID: 11075
		private float m_destroyWaitElapsed;
	}
}
