using System;
using System.Collections.Generic;
using SoL.Game.Animation;
using SoL.Game.Crafting;
using SoL.Game.EffectSystem;
using SoL.Game.Messages;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Objects.Containers;
using SoL.Game.Targeting;
using SoL.Game.Transactions;
using SoL.Managers;
using SoL.Networking;
using SoL.Networking.Objects;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005BA RID: 1466
	public class ExecutionCache : IPoolable, IHandHeldItems, ITargetData
	{
		// Token: 0x170009B4 RID: 2484
		// (get) Token: 0x06002E32 RID: 11826 RVA: 0x00060072 File Offset: 0x0005E272
		public CachedHandHeldItem MainHand
		{
			get
			{
				return this.m_mainHandCache;
			}
		}

		// Token: 0x170009B5 RID: 2485
		// (get) Token: 0x06002E33 RID: 11827 RVA: 0x0006007A File Offset: 0x0005E27A
		public CachedHandHeldItem OffHand
		{
			get
			{
				return this.m_offHandCache;
			}
		}

		// Token: 0x170009B6 RID: 2486
		// (get) Token: 0x06002E34 RID: 11828 RVA: 0x00060082 File Offset: 0x0005E282
		public DynamicVitalMods ExternalMods
		{
			get
			{
				return this.m_externalMods;
			}
		}

		// Token: 0x170009B7 RID: 2487
		// (get) Token: 0x06002E35 RID: 11829 RVA: 0x0006008A File Offset: 0x0005E28A
		public GameEntity SourceEntity
		{
			get
			{
				return this.m_sourceEntity;
			}
		}

		// Token: 0x170009B8 RID: 2488
		// (get) Token: 0x06002E36 RID: 11830 RVA: 0x00060092 File Offset: 0x0005E292
		public GameEntity OffensiveTarget
		{
			get
			{
				return this.m_offensiveTarget;
			}
		}

		// Token: 0x170009B9 RID: 2489
		// (get) Token: 0x06002E37 RID: 11831 RVA: 0x0006009A File Offset: 0x0005E29A
		public GameEntity DefensiveTarget
		{
			get
			{
				return this.m_defensiveTarget;
			}
		}

		// Token: 0x170009BA RID: 2490
		// (get) Token: 0x06002E38 RID: 11832 RVA: 0x000600A2 File Offset: 0x0005E2A2
		public IExecutable Executable
		{
			get
			{
				return this.m_executable;
			}
		}

		// Token: 0x170009BB RID: 2491
		// (get) Token: 0x06002E39 RID: 11833 RVA: 0x000600AA File Offset: 0x0005E2AA
		public ExecutionParams ExecutionParams
		{
			get
			{
				return this.m_executionParams;
			}
		}

		// Token: 0x170009BC RID: 2492
		// (get) Token: 0x06002E3A RID: 11834 RVA: 0x000600B2 File Offset: 0x0005E2B2
		public ICombatEffectSource EffectSource
		{
			get
			{
				return this.m_effectSource;
			}
		}

		// Token: 0x170009BD RID: 2493
		// (get) Token: 0x06002E3B RID: 11835 RVA: 0x000600BA File Offset: 0x0005E2BA
		public TargetingParams TargetingParams
		{
			get
			{
				return this.m_targetingParams;
			}
		}

		// Token: 0x170009BE RID: 2494
		// (get) Token: 0x06002E3C RID: 11836 RVA: 0x000600C2 File Offset: 0x0005E2C2
		public KinematicParameters KinematicParameters
		{
			get
			{
				return this.m_kinematicParameters;
			}
		}

		// Token: 0x170009BF RID: 2495
		// (get) Token: 0x06002E3D RID: 11837 RVA: 0x000600CA File Offset: 0x0005E2CA
		public CombatEffect CombatEffect
		{
			get
			{
				return this.m_combatEffect;
			}
		}

		// Token: 0x170009C0 RID: 2496
		// (get) Token: 0x06002E3E RID: 11838 RVA: 0x000600D2 File Offset: 0x0005E2D2
		public GameEntity TargetEntity
		{
			get
			{
				return this.m_targetEntity;
			}
		}

		// Token: 0x170009C1 RID: 2497
		// (get) Token: 0x06002E3F RID: 11839 RVA: 0x000600DA File Offset: 0x0005E2DA
		public NetworkEntity TargetNetworkEntity
		{
			get
			{
				return this.m_targetNetworkEntity;
			}
		}

		// Token: 0x170009C2 RID: 2498
		// (get) Token: 0x06002E40 RID: 11840 RVA: 0x000600E2 File Offset: 0x0005E2E2
		public ArchetypeInstance Instance
		{
			get
			{
				return this.m_instance;
			}
		}

		// Token: 0x170009C3 RID: 2499
		// (get) Token: 0x06002E41 RID: 11841 RVA: 0x000600EA File Offset: 0x0005E2EA
		public UniqueId ArchetypeId
		{
			get
			{
				return this.m_archetypeId;
			}
		}

		// Token: 0x170009C4 RID: 2500
		// (get) Token: 0x06002E42 RID: 11842 RVA: 0x000600F2 File Offset: 0x0005E2F2
		public UniqueId? GroupId
		{
			get
			{
				return this.m_groupId;
			}
		}

		// Token: 0x170009C5 RID: 2501
		// (get) Token: 0x06002E43 RID: 11843 RVA: 0x000600FA File Offset: 0x0005E2FA
		public UniqueId? ReagentId
		{
			get
			{
				return this.m_reagentId;
			}
		}

		// Token: 0x170009C6 RID: 2502
		// (get) Token: 0x06002E44 RID: 11844 RVA: 0x00060102 File Offset: 0x0005E302
		public float MasteryLevel
		{
			get
			{
				return this.m_masteryLevel;
			}
		}

		// Token: 0x170009C7 RID: 2503
		// (get) Token: 0x06002E45 RID: 11845 RVA: 0x0006010A File Offset: 0x0005E30A
		public float AbilityLevel
		{
			get
			{
				return this.m_abilityLevel;
			}
		}

		// Token: 0x170009C8 RID: 2504
		// (get) Token: 0x06002E46 RID: 11846 RVA: 0x00060112 File Offset: 0x0005E312
		public byte AbilityLevelAsByte
		{
			get
			{
				return this.m_abilityLevelAsByte;
			}
		}

		// Token: 0x170009C9 RID: 2505
		// (get) Token: 0x06002E47 RID: 11847 RVA: 0x0006011A File Offset: 0x0005E31A
		public bool IsInstant
		{
			get
			{
				return this.m_isInstant;
			}
		}

		// Token: 0x170009CA RID: 2506
		// (get) Token: 0x06002E48 RID: 11848 RVA: 0x00060122 File Offset: 0x0005E322
		public bool UseTargetAtExecutionComplete
		{
			get
			{
				return this.m_useTargetAtExecutionComplete;
			}
		}

		// Token: 0x170009CB RID: 2507
		// (get) Token: 0x06002E49 RID: 11849 RVA: 0x0006012A File Offset: 0x0005E32A
		public AlchemyPowerLevel AlchemyPowerLevel
		{
			get
			{
				return this.m_alchemyPowerLevel;
			}
		}

		// Token: 0x170009CC RID: 2508
		// (get) Token: 0x06002E4A RID: 11850 RVA: 0x00060132 File Offset: 0x0005E332
		// (set) Token: 0x06002E4B RID: 11851 RVA: 0x0006013A File Offset: 0x0005E33A
		public bool ForceTargetingCheck { get; private set; }

		// Token: 0x170009CD RID: 2509
		// (get) Token: 0x06002E4C RID: 11852 RVA: 0x00060143 File Offset: 0x0005E343
		// (set) Token: 0x06002E4D RID: 11853 RVA: 0x0006014B File Offset: 0x0005E34B
		public TargetOverlayState TargetOverlay { get; set; }

		// Token: 0x170009CE RID: 2510
		// (get) Token: 0x06002E4E RID: 11854 RVA: 0x00151C9C File Offset: 0x0014FE9C
		public float ExecutionTime
		{
			get
			{
				float num = this.ToolItem ? this.ToolItem.ExecutionTimeMultiplier : 1f;
				return this.m_executionTime * num;
			}
		}

		// Token: 0x170009CF RID: 2511
		// (get) Token: 0x06002E4F RID: 11855 RVA: 0x00060154 File Offset: 0x0005E354
		// (set) Token: 0x06002E50 RID: 11856 RVA: 0x0006015C File Offset: 0x0005E35C
		public float? ApplicationTime
		{
			get
			{
				return this.m_applicationTime;
			}
			set
			{
				this.m_applicationTime = value;
			}
		}

		// Token: 0x170009D0 RID: 2512
		// (get) Token: 0x06002E51 RID: 11857 RVA: 0x00060165 File Offset: 0x0005E365
		// (set) Token: 0x06002E52 RID: 11858 RVA: 0x0006016D File Offset: 0x0005E36D
		public ArchetypeInstance MasteryInstance
		{
			get
			{
				return this.m_masteryInstance;
			}
			set
			{
				this.m_masteryInstance = value;
				ArchetypeInstance masteryInstance = this.m_masteryInstance;
				if (((masteryInstance != null) ? masteryInstance.MasteryData : null) != null)
				{
					this.m_masteryLevel = this.m_masteryInstance.MasteryData.BaseLevel;
				}
			}
		}

		// Token: 0x170009D1 RID: 2513
		// (get) Token: 0x06002E53 RID: 11859 RVA: 0x000601A0 File Offset: 0x0005E3A0
		// (set) Token: 0x06002E54 RID: 11860 RVA: 0x000601A8 File Offset: 0x0005E3A8
		public ReagentItem ReagentItem
		{
			get
			{
				return this.m_reagentItem;
			}
			set
			{
				this.m_reagentItem = value;
				if (this.m_reagentItem != null)
				{
					this.m_reagentId = new UniqueId?(this.m_reagentItem.Id);
					return;
				}
				this.m_reagentId = null;
			}
		}

		// Token: 0x06002E55 RID: 11861 RVA: 0x00151CD4 File Offset: 0x0014FED4
		public ExecutionCache()
		{
			this.m_mainHandCache = new CachedHandHeldItem(true);
			this.m_offHandCache = new CachedHandHeldItem(false);
			this.m_externalMods = new DynamicVitalMods();
			this.m_reductionTasks = new List<ExecutionCache.ReductionTask>(5);
			this.m_postReductionTasks = new List<ExecutionCache.PostReductionTask>(5);
			this.m_targets = new List<GameEntity>(5);
			this.m_targetsToExclude = new HashSet<GameEntity>();
		}

		// Token: 0x06002E56 RID: 11862 RVA: 0x000601E2 File Offset: 0x0005E3E2
		public void InitInstant(GameEntity sourceEntity, ArchetypeInstance instance)
		{
			this.m_isInstant = true;
			this.Init(sourceEntity, instance);
		}

		// Token: 0x06002E57 RID: 11863 RVA: 0x000601F3 File Offset: 0x0005E3F3
		public void Init(GameEntity sourceEntity, ArchetypeInstance instance)
		{
			this.Init(sourceEntity, instance, instance.Archetype as IExecutable, AlchemyPowerLevel.None, false);
		}

		// Token: 0x06002E58 RID: 11864 RVA: 0x0006020A File Offset: 0x0005E40A
		public void Init(GameEntity sourceEntity, ArchetypeInstance instance, IExecutable executable, AlchemyPowerLevel alchemyPowerLevel, bool useTargetAtExecutionComplete)
		{
			this.m_useTargetAtExecutionComplete = useTargetAtExecutionComplete;
			this.InitInternal(sourceEntity, instance, executable, alchemyPowerLevel);
			this.InitTargetsAndHandCache();
			this.LocalPlayerInit();
		}

		// Token: 0x06002E59 RID: 11865 RVA: 0x0006022B File Offset: 0x0005E42B
		public void InitForArchetypeInstanceUI(GameEntity sourceEntity, ArchetypeInstance instance, IExecutable executable, bool firstFrame)
		{
			this.InitInternal(sourceEntity, instance, executable, AlchemyPowerLevel.None);
			if (firstFrame)
			{
				this.InitTargetsAndHandCache();
			}
			this.ForceTargetingCheck = true;
		}

		// Token: 0x06002E5A RID: 11866 RVA: 0x00151D44 File Offset: 0x0014FF44
		private void InitInternal(GameEntity sourceEntity, ArchetypeInstance instance, IExecutable executable, AlchemyPowerLevel alchemyPowerLevel)
		{
			this.ForceTargetingCheck = false;
			this.TargetOverlay = TargetOverlayState.None;
			this.m_sourceEntity = sourceEntity;
			this.m_instance = instance;
			this.m_archetypeId = instance.ArchetypeId;
			this.m_abilityLevel = this.m_instance.GetAssociatedLevel(sourceEntity);
			this.m_abilityLevelAsByte = (byte)Mathf.FloorToInt(this.m_abilityLevel);
			if (sourceEntity.CharacterData)
			{
				this.m_masteryLevel = (float)sourceEntity.CharacterData.AdventuringLevel;
			}
			this.m_alchemyPowerLevel = (executable.HasValidAlchemyOverride(this.m_abilityLevel, alchemyPowerLevel) ? alchemyPowerLevel : AlchemyPowerLevel.None);
			this.m_executable = executable;
			this.m_executionParams = executable.GetExecutionParams(this.m_abilityLevel, this.m_alchemyPowerLevel);
			this.m_executionTime = this.m_executionParams.ExecutionTime + this.m_alchemyPowerLevel.GetAddedExecutionTime();
			this.m_staminaCost = (float)this.m_executionParams.StaminaCost;
			this.m_cooldown = (float)this.m_executionParams.Cooldown;
			this.m_isInstant = this.m_executionParams.IsInstant;
			this.m_sourceType = executable.Type;
			this.m_effectSource = (executable as ICombatEffectSource);
			if (this.m_effectSource != null)
			{
				this.m_targetingParams = this.m_effectSource.GetTargetingParams(this.m_abilityLevel, this.m_alchemyPowerLevel);
				this.m_kinematicParameters = this.m_effectSource.GetKinematicParams(this.m_abilityLevel, this.m_alchemyPowerLevel);
				this.m_combatEffect = this.m_effectSource.GetCombatEffect(this.m_abilityLevel, this.m_alchemyPowerLevel);
			}
			if (!this.m_sourceEntity.CharacterData.GroupId.IsEmpty)
			{
				this.m_groupId = new UniqueId?(this.m_sourceEntity.CharacterData.GroupId);
			}
		}

		// Token: 0x06002E5B RID: 11867 RVA: 0x00060248 File Offset: 0x0005E448
		public void Init(GameEntity sourceEntity, ScriptableCombatEffect scriptableCombatEffect)
		{
			this.m_sourceEntity = sourceEntity;
			this.m_archetypeId = scriptableCombatEffect.Id;
			this.InitTargetsAndHandCache();
		}

		// Token: 0x06002E5C RID: 11868 RVA: 0x00060263 File Offset: 0x0005E463
		public void Init(GameEntity sourceEntity, ICombatEffectSource effectSource)
		{
			this.m_sourceEntity = sourceEntity;
			this.m_archetypeId = effectSource.ArchetypeId;
			this.m_effectSource = effectSource;
			this.InitTargetsAndHandCache();
		}

		// Token: 0x06002E5D RID: 11869 RVA: 0x00060285 File Offset: 0x0005E485
		public void InitNpc(GameEntity sourceEntity)
		{
			this.m_sourceEntity = sourceEntity;
			this.m_mainHandCache.Init(sourceEntity);
			this.m_offHandCache.Init(sourceEntity);
		}

		// Token: 0x06002E5E RID: 11870 RVA: 0x000602A6 File Offset: 0x0005E4A6
		public void UpdateOffensiveDefensiveTargets()
		{
			if (!this.m_sourceEntity)
			{
				return;
			}
			this.m_offensiveTarget = this.m_sourceEntity.TargetController.OffensiveTarget;
			this.m_defensiveTarget = this.m_sourceEntity.TargetController.DefensiveTarget;
		}

		// Token: 0x06002E5F RID: 11871 RVA: 0x000602E2 File Offset: 0x0005E4E2
		private void InitTargetsAndHandCache()
		{
			this.UpdateOffensiveDefensiveTargets();
			this.m_mainHandCache.Init(this.m_sourceEntity);
			this.m_offHandCache.Init(this.m_sourceEntity);
		}

		// Token: 0x06002E60 RID: 11872 RVA: 0x0006030C File Offset: 0x0005E50C
		private bool SourceIsLocalPlayer()
		{
			return !GameManager.IsServer && this.m_sourceEntity && this.m_sourceEntity.NetworkEntity && this.m_sourceEntity.NetworkEntity.IsLocal;
		}

		// Token: 0x06002E61 RID: 11873 RVA: 0x00151EF4 File Offset: 0x001500F4
		private void LocalPlayerInit()
		{
			if (!this.SourceIsLocalPlayer())
			{
				return;
			}
			if (this.m_executionParams != null && this.m_sourceEntity.Vitals && this.m_sourceEntity.Vitals && LocalPlayer.Animancer)
			{
				this.m_isInstant = this.m_executionParams.IsInstant;
				if (this.m_executionParams.ValidStances == StanceFlags.Combat && this.m_sourceEntity.Vitals.Stance != Stance.Combat)
				{
					LocalPlayer.Animancer.ForceCombat(this.m_isInstant);
				}
				else if (this.m_executionParams.ValidStances.HasBitFlag(StanceFlags.Idle) && !this.m_executionParams.ValidStances.HasBitFlag(StanceFlags.Sit) && this.m_sourceEntity.Vitals.Stance == Stance.Sit)
				{
					LocalPlayer.Animancer.SetStance(Stance.Idle);
				}
				if (this.m_targetingParams != null && this.m_targetingParams.TargetType.RequiresTarget() && this.m_targetingParams.TargetType.IsFriendly() && !this.m_targetingParams.ExcludeSelf && this.m_sourceEntity.TargetController.DefensiveTarget == null)
				{
					this.m_sourceEntity.TargetController.SetTarget(TargetType.Defensive, this.m_sourceEntity.Targetable);
				}
			}
		}

		// Token: 0x06002E62 RID: 11874 RVA: 0x00152048 File Offset: 0x00150248
		public void LocalPlayerComplete(bool processEnable, bool processDisable)
		{
			if (!this.SourceIsLocalPlayer())
			{
				return;
			}
			if (this.m_executionParams != null && UIManager.AutoAttackButton)
			{
				AutoAttackStateChange autoAttackState = this.m_executionParams.AutoAttackState;
				if (autoAttackState != AutoAttackStateChange.Enable)
				{
					if (autoAttackState != AutoAttackStateChange.Disable)
					{
						return;
					}
					if (processDisable)
					{
						UIManager.AutoAttackButton.DisableAutoAttack();
					}
				}
				else if (processEnable)
				{
					UIManager.AutoAttackButton.InitiateCombat();
					return;
				}
			}
		}

		// Token: 0x06002E63 RID: 11875 RVA: 0x001520A4 File Offset: 0x001502A4
		public void Reset()
		{
			this.m_isInstant = false;
			this.m_useTargetAtExecutionComplete = false;
			this.ForceTargetingCheck = false;
			this.TargetOverlay = TargetOverlayState.None;
			this.m_sourceEntity = null;
			this.m_offensiveTarget = null;
			this.m_defensiveTarget = null;
			this.m_groundTarget = null;
			this.m_executable = null;
			this.m_executionParams = null;
			this.m_effectSource = null;
			this.m_targetingParams = null;
			this.m_kinematicParameters = null;
			this.m_combatEffect = null;
			this.m_targetEntity = null;
			this.m_targetNetworkEntity = null;
			this.m_instance = null;
			this.m_archetypeId = UniqueId.Empty;
			this.m_reagentId = null;
			this.m_masteryInstance = null;
			this.m_masteryLevel = 0f;
			this.m_abilityLevel = 0f;
			this.m_abilityLevelAsByte = 0;
			this.m_groupId = null;
			this.m_reagentItem = null;
			this.m_executionTime = 0f;
			this.m_staminaCost = 0f;
			this.m_cooldown = 0f;
			this.m_sourceType = EffectSourceType.None;
			this.m_alchemyPowerLevel = AlchemyPowerLevel.None;
			this.MsgType = MessageType.None;
			this.Message = string.Empty;
			this.MasteryInstance = null;
			this.ToolInstance = null;
			this.ToolItem = null;
			this.Refinement = null;
			this.m_mainHandCache.Reset();
			this.m_offHandCache.Reset();
			this.m_externalMods.Reset();
			this.m_reductionTasks.Clear();
			this.m_postReductionTasks.Clear();
			this.ResetApplication();
		}

		// Token: 0x06002E64 RID: 11876 RVA: 0x00060346 File Offset: 0x0005E546
		public void SetTargetNetworkEntity(NetworkEntity netEntity)
		{
			this.m_targetNetworkEntity = netEntity;
			if (netEntity)
			{
				this.m_targetEntity = netEntity.GameEntity;
			}
		}

		// Token: 0x06002E65 RID: 11877 RVA: 0x00060363 File Offset: 0x0005E563
		public void SetTargetGameEntity(GameEntity entity)
		{
			this.m_targetEntity = entity;
			if (entity)
			{
				this.m_targetNetworkEntity = entity.NetworkEntity;
			}
		}

		// Token: 0x06002E66 RID: 11878 RVA: 0x00060380 File Offset: 0x0005E580
		public void SetExecutionTime(float value)
		{
			this.m_executionTime = value;
		}

		// Token: 0x06002E67 RID: 11879 RVA: 0x0015221C File Offset: 0x0015041C
		public void PerformReduction()
		{
			if (!GameManager.IsServer)
			{
				return;
			}
			for (int i = 0; i < this.m_reductionTasks.Count; i++)
			{
				ContainerInstance containerInstance = (this.m_reductionTasks[i].Instance != null) ? this.m_reductionTasks[i].Instance.ContainerInstance : null;
				switch (this.m_reductionTasks[i].Type)
				{
				case ReductionTaskType.Count:
					this.m_reductionTasks[i].Instance.ReduceCountBy(this.m_reductionTasks[i].Value, this.SourceEntity);
					break;
				case ReductionTaskType.Charge:
					this.m_reductionTasks[i].Instance.ReduceChargeBy(this.m_reductionTasks[i].Value, this.SourceEntity);
					break;
				case ReductionTaskType.Consume:
					this.m_reductionTasks[i].Instance.Consume(this.SourceEntity);
					break;
				}
				if (containerInstance != null)
				{
					containerInstance.InvokeQuantityOfItemChanged();
				}
			}
			this.m_reductionTasks.Clear();
		}

		// Token: 0x06002E68 RID: 11880 RVA: 0x00152334 File Offset: 0x00150534
		public void PerformPostReduction()
		{
			if (!GameManager.IsServer)
			{
				return;
			}
			for (int i = 0; i < this.m_postReductionTasks.Count; i++)
			{
				if (this.m_postReductionTasks[i].Type == PostReductionTaskType.AddItem && this.m_postReductionTasks[i].ItemInstance != null && this.m_postReductionTasks[i].ContainterInstance != null)
				{
					int firstAvailableIndex = this.m_postReductionTasks[i].ContainterInstance.GetFirstAvailableIndex();
					if (firstAvailableIndex == -1)
					{
						throw new ArgumentException("No index to get for container!");
					}
					this.m_postReductionTasks[i].ItemInstance.Index = firstAvailableIndex;
					this.m_postReductionTasks[i].ContainterInstance.Add(this.m_postReductionTasks[i].ItemInstance, true);
					ArchetypeAddedTransaction response = new ArchetypeAddedTransaction
					{
						Op = OpCodes.Ok,
						Instance = this.m_postReductionTasks[i].ItemInstance,
						TargetContainer = this.m_postReductionTasks[i].ContainterInstance.Id,
						Context = this.m_postReductionTasks[i].Context
					};
					this.SourceEntity.NetworkEntity.PlayerRpcHandler.AddItemResponse(response);
				}
			}
		}

		// Token: 0x06002E69 RID: 11881 RVA: 0x00152484 File Offset: 0x00150684
		public ClientExecutionCache GetClientExecutionCache()
		{
			ClientExecutionCache clientExecutionCache = new ClientExecutionCache
			{
				ArchetypeId = this.m_instance.ArchetypeId,
				TargetEntity = this.m_targetNetworkEntity,
				SourceType = this.m_sourceType,
				AlchemyPowerLevel = this.m_alchemyPowerLevel,
				UseTargetAtExecutionComplete = Options.GameOptions.UseTargetAtExecutionComplete.Value
			};
			if (this.Refinement != null)
			{
				clientExecutionCache.RecipeId = new UniqueId?(this.Refinement.Value.Recipe.Id);
				if (this.Refinement.Value.ItemsUsed != null && this.Refinement.Value.ItemsUsed.Count > 0)
				{
					clientExecutionCache.ItemsUsed = new List<ItemUsageSerializable>(this.Refinement.Value.ItemsUsed.Count);
					foreach (ItemUsage itemUsage in this.Refinement.Value.ItemsUsed)
					{
						clientExecutionCache.ItemsUsed.Add(new ItemUsageSerializable
						{
							Instance = itemUsage.Instance.InstanceId,
							UsedFor = itemUsage.UsedFor.Id,
							AmountUsed = itemUsage.AmountUsed
						});
					}
				}
				clientExecutionCache.TargetAbilityLevel = new int?(this.Refinement.Value.TargetAbilityLevel);
			}
			if (clientExecutionCache.SourceType == EffectSourceType.Consumable)
			{
				clientExecutionCache.InstanceId = new UniqueId?(this.m_instance.InstanceId);
				clientExecutionCache.ContainerId = this.m_instance.ContainerInstance.Id;
			}
			return clientExecutionCache;
		}

		// Token: 0x06002E6A RID: 11882 RVA: 0x00060389 File Offset: 0x0005E589
		public void AddReductionTask(ReductionTaskType type, ArchetypeInstance instance, int value)
		{
			if (!GameManager.IsServer)
			{
				return;
			}
			this.m_reductionTasks.Add(new ExecutionCache.ReductionTask(type, instance, value));
		}

		// Token: 0x06002E6B RID: 11883 RVA: 0x000603A6 File Offset: 0x0005E5A6
		public void AddPostReductionTask(PostReductionTaskType type, ArchetypeInstance itemInstance, ContainerInstance containerInstance, ItemAddContext context)
		{
			if (!GameManager.IsServer)
			{
				return;
			}
			this.m_postReductionTasks.Add(new ExecutionCache.PostReductionTask(type, itemInstance, containerInstance, context));
		}

		// Token: 0x06002E6C RID: 11884 RVA: 0x000603C5 File Offset: 0x0005E5C5
		void IPoolable.Reset()
		{
			this.Reset();
		}

		// Token: 0x170009D2 RID: 2514
		// (get) Token: 0x06002E6D RID: 11885 RVA: 0x000603CD File Offset: 0x0005E5CD
		// (set) Token: 0x06002E6E RID: 11886 RVA: 0x000603D5 File Offset: 0x0005E5D5
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

		// Token: 0x170009D3 RID: 2515
		// (get) Token: 0x06002E6F RID: 11887 RVA: 0x00060072 File Offset: 0x0005E272
		CachedHandHeldItem IHandHeldItems.MainHand
		{
			get
			{
				return this.m_mainHandCache;
			}
		}

		// Token: 0x170009D4 RID: 2516
		// (get) Token: 0x06002E70 RID: 11888 RVA: 0x0006007A File Offset: 0x0005E27A
		CachedHandHeldItem IHandHeldItems.OffHand
		{
			get
			{
				return this.m_offHandCache;
			}
		}

		// Token: 0x170009D5 RID: 2517
		// (get) Token: 0x06002E71 RID: 11889 RVA: 0x0006008A File Offset: 0x0005E28A
		GameEntity ITargetData.Self
		{
			get
			{
				return this.m_sourceEntity;
			}
		}

		// Token: 0x170009D6 RID: 2518
		// (get) Token: 0x06002E72 RID: 11890 RVA: 0x00060092 File Offset: 0x0005E292
		GameEntity ITargetData.Offensive
		{
			get
			{
				return this.m_offensiveTarget;
			}
		}

		// Token: 0x170009D7 RID: 2519
		// (get) Token: 0x06002E73 RID: 11891 RVA: 0x0006009A File Offset: 0x0005E29A
		GameEntity ITargetData.Defensive
		{
			get
			{
				return this.m_defensiveTarget;
			}
		}

		// Token: 0x170009D8 RID: 2520
		// (get) Token: 0x06002E74 RID: 11892 RVA: 0x000603DE File Offset: 0x0005E5DE
		Vector3? ITargetData.GroundPosition
		{
			get
			{
				return this.m_groundTarget;
			}
		}

		// Token: 0x06002E75 RID: 11893 RVA: 0x00152654 File Offset: 0x00150854
		private static bool EntityHasValidHealthState(GameEntity target, HealthStateFlags validHealthStateFlags)
		{
			return target && target.VitalsReplicator && target.VitalsReplicator.CurrentHealthState.Value != HealthState.Dead && validHealthStateFlags.MeetsRequirements(target.VitalsReplicator.CurrentHealthState.Value);
		}

		// Token: 0x06002E76 RID: 11894 RVA: 0x000603E6 File Offset: 0x0005E5E6
		private static int SortBySqrMagnitudeDistance(ExecutionCache.PotentialAoeTarget a, ExecutionCache.PotentialAoeTarget b)
		{
			return a.SqrDistance.CompareTo(b.SqrDistance);
		}

		// Token: 0x170009D9 RID: 2521
		// (get) Token: 0x06002E77 RID: 11895 RVA: 0x000603FA File Offset: 0x0005E5FA
		// (set) Token: 0x06002E78 RID: 11896 RVA: 0x00060402 File Offset: 0x0005E602
		public AnimationFlags AnimationFlags { get; private set; }

		// Token: 0x170009DA RID: 2522
		// (get) Token: 0x06002E79 RID: 11897 RVA: 0x0006040B File Offset: 0x0005E60B
		// (set) Token: 0x06002E7A RID: 11898 RVA: 0x00060413 File Offset: 0x0005E613
		public BaseRole NpcBaseRole { get; set; }

		// Token: 0x06002E7B RID: 11899 RVA: 0x001526A4 File Offset: 0x001508A4
		private void ResetApplication()
		{
			this.m_applicationTime = null;
			this.m_lowestNpcTargetLevel = null;
			this.m_highestNpcTargetLevel = null;
			List<GameEntity> targets = this.m_targets;
			if (targets != null)
			{
				targets.Clear();
			}
			this.m_targetsToExclude.Clear();
			this.m_primaryApplied = false;
			this.AnimationFlags = AnimationFlags.None;
			this.NpcBaseRole = null;
		}

		// Token: 0x06002E7C RID: 11900 RVA: 0x00152708 File Offset: 0x00150908
		public void ApplyEffects()
		{
			this.AnimationFlags = AnimationFlags.None;
			if (this.m_effectSource != null)
			{
				CombatEffect combatEffect = this.m_combatEffect;
				EffectApplicationFlags flags = this.ApplyCombatEffect(this.TargetingParams, combatEffect, false);
				this.m_primaryApplied = true;
				TargetingParams targetingParams;
				CombatEffect effect;
				if (combatEffect.TryGetSecondary(flags, out targetingParams, out effect))
				{
					this.ApplyCombatEffect(targetingParams, effect, true);
				}
				if (this.m_executable is AutoAttackAbility && flags.ShouldGiveCredit())
				{
					BaseRole baseRole = null;
					GameEntityType type = this.m_sourceEntity.Type;
					if (type != GameEntityType.Player)
					{
						if (type == GameEntityType.Npc)
						{
							baseRole = this.NpcBaseRole;
						}
					}
					else if (this.m_masteryInstance != null && this.m_masteryInstance.Archetype)
					{
						baseRole = (this.m_masteryInstance.Archetype as BaseRole);
					}
					bool includeOffhand = false;
					if ((this.m_executionParams == null || this.m_executionParams.AutoAttackState != AutoAttackStateChange.Disable) && baseRole && baseRole.ShouldPerformOffhandAutoAttack(this.m_sourceEntity, this.m_masteryLevel))
					{
						includeOffhand = true;
						this.m_archetypeId = baseRole.OffhandAutoAttackCombatEffect.Id;
						this.ApplyCombatEffect(this.TargetingParams, baseRole.OffhandAutoAttackCombatEffect.Effect, false);
					}
					this.AnimationFlags = this.GetAnimationFlagsForHandHeldItems(includeOffhand);
				}
			}
		}

		// Token: 0x06002E7D RID: 11901 RVA: 0x0015283C File Offset: 0x00150A3C
		private EffectApplicationFlags ApplyCombatEffect(TargetingParams targetingParams, CombatEffect effect, bool isSecondary)
		{
			EffectApplicationFlags effectApplicationFlags = EffectApplicationFlags.None;
			this.GatherTargets(targetingParams, effect.Polarity == Polarity.Negative);
			for (int i = 0; i < this.m_targets.Count; i++)
			{
				EffectApplicationFlags effectApplicationFlags2 = this.m_targets[i].EffectController.ApplyEffect(this, effect, isSecondary);
				effectApplicationFlags |= effectApplicationFlags2;
			}
			ThreatParams threatParams;
			if (effect.TryGetAdditionalThreat(effectApplicationFlags, out threatParams))
			{
				if (threatParams.Aoe)
				{
					this.m_targets.Clear();
					this.GatherAoeTargets(TargetType.Offensive, GameEntityTypeFlags.Npc, HealthStateFlags.Alive, this.m_sourceEntity, threatParams.Radius, 8, null, false, false, effect.Polarity == Polarity.Negative, ConalTypes.None, false);
					for (int j = 0; j < this.m_targets.Count; j++)
					{
						this.AddThreatForTarget(this.m_targets[j], threatParams.Value);
					}
				}
				else
				{
					GameEntity initialTarget = targetingParams.GetInitialTarget(this);
					if (initialTarget && initialTarget.Type == GameEntityType.Npc)
					{
						this.AddThreatForTarget(initialTarget, threatParams.Value);
					}
				}
			}
			return effectApplicationFlags;
		}

		// Token: 0x06002E7E RID: 11902 RVA: 0x00152940 File Offset: 0x00150B40
		private void AddThreatForTarget(GameEntity target, float threatValue)
		{
			if (target && target.Type == GameEntityType.Npc && this.SourceEntity && target.TargetController && target.TargetController.IsHostileTo(this.SourceEntity.NetworkEntity, null))
			{
				target.TargetController.AddThreat(this.SourceEntity, threatValue, 0f, false);
			}
		}

		// Token: 0x06002E7F RID: 11903 RVA: 0x001529AC File Offset: 0x00150BAC
		private void GatherTargets(TargetingParams targetingParams, bool aoeConsiderLineOfSight)
		{
			if (targetingParams == null)
			{
				throw new ArgumentNullException("targetingParams");
			}
			this.m_targets.Clear();
			this.m_targetsToExclude.Clear();
			if (targetingParams.ExcludeSelf)
			{
				this.m_targetsToExclude.Add(this.m_sourceEntity);
			}
			switch (targetingParams.TargetType)
			{
			case EffectTargetType.Self:
				if (!this.m_sourceEntity)
				{
					throw new ArgumentException("Attempting to get Self target position but we have no Self target!");
				}
				this.AddTarget(this.m_sourceEntity, targetingParams.RequiredHealthState, false);
				return;
			case EffectTargetType.Self_Offensive_AOE:
			case EffectTargetType.Self_Defensive_AOE:
			case EffectTargetType.Offensive_AOE:
			case EffectTargetType.Offensive_AOE_Conal:
			case EffectTargetType.Offensive_AOE_Ground:
			case EffectTargetType.Defensive_AOE:
			case EffectTargetType.Defensive_AOE_Conal:
			case EffectTargetType.Defensive_AOE_Ground:
				this.GatherAoeTargets(targetingParams, targetingParams.GetInitialTarget(this), targetingParams.GetAoeRadius(this.m_sourceEntity, this), false, aoeConsiderLineOfSight);
				return;
			case (EffectTargetType)3:
			case (EffectTargetType)4:
			case (EffectTargetType)5:
			case (EffectTargetType)6:
			case (EffectTargetType)7:
			case (EffectTargetType)8:
			case (EffectTargetType)9:
			case (EffectTargetType)15:
			case (EffectTargetType)16:
			case (EffectTargetType)17:
			case (EffectTargetType)18:
			case (EffectTargetType)19:
				break;
			case EffectTargetType.Offensive:
				this.AddTarget(this.m_offensiveTarget, targetingParams.RequiredHealthState, false);
				return;
			case EffectTargetType.Offensive_Chain:
			case EffectTargetType.Defensive_Chain:
			{
				GameEntity initialTarget = targetingParams.GetInitialTarget(this);
				this.AddTarget(initialTarget, targetingParams.RequiredHealthState, false);
				if (this.m_targets.Count > 0)
				{
					this.m_targetsToExclude.Add(initialTarget);
					float aoeRadius = targetingParams.GetAoeRadius(this.m_sourceEntity, this);
					int aoeMaxTargets = targetingParams.AoeMaxTargets;
					for (int i = 1; i < aoeMaxTargets; i++)
					{
						int count = this.m_targets.Count;
						this.GatherAoeTargets(targetingParams, this.m_targets[i - 1], aoeRadius, true, aoeConsiderLineOfSight);
						if (count == this.m_targets.Count || this.m_targets.Count >= aoeMaxTargets)
						{
							break;
						}
					}
				}
				break;
			}
			case EffectTargetType.Defensive:
			{
				GameEntity gameEntity = (this.m_defensiveTarget != null) ? this.m_defensiveTarget : this.m_sourceEntity;
				if (!targetingParams.ExcludeSelf || gameEntity != this.m_sourceEntity)
				{
					this.AddTarget(gameEntity, targetingParams.RequiredHealthState, false);
					return;
				}
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x06002E80 RID: 11904 RVA: 0x0006041C File Offset: 0x0005E61C
		private void AddTarget(GameEntity target, HealthStateFlags validHealthStateFlags, bool addToExclusion = false)
		{
			if (ExecutionCache.EntityHasValidHealthState(target, validHealthStateFlags))
			{
				this.AddTarget(target, addToExclusion);
			}
		}

		// Token: 0x06002E81 RID: 11905 RVA: 0x00152BA4 File Offset: 0x00150DA4
		private void AddTarget(GameEntity target, bool addToExclusion = false)
		{
			if (!target)
			{
				return;
			}
			this.m_targets.Add(target);
			if (addToExclusion)
			{
				this.m_targetsToExclude.Add(target);
			}
			if (!this.m_primaryApplied && target.Type == GameEntityType.Npc)
			{
				int adventuringLevel = target.CharacterData.AdventuringLevel;
				if (this.m_lowestNpcTargetLevel == null || adventuringLevel < this.m_lowestNpcTargetLevel.Value)
				{
					this.m_lowestNpcTargetLevel = new int?(adventuringLevel);
				}
				if (this.m_highestNpcTargetLevel == null || adventuringLevel > this.m_highestNpcTargetLevel.Value)
				{
					this.m_highestNpcTargetLevel = new int?(adventuringLevel);
				}
			}
		}

		// Token: 0x06002E82 RID: 11906 RVA: 0x00152C44 File Offset: 0x00150E44
		private void GatherAoeTargets(TargetingParams targetingParams, GameEntity sampleSourceEntity, float radius, bool onlyGatherSingleClosest, bool considerLineOfSight)
		{
			TargetType validTargetType = targetingParams.TargetType.IsFriendly() ? TargetType.Defensive : TargetType.Offensive;
			GameEntityTypeFlags validEntityTypes = targetingParams.TargetType.IsFriendly() ? this.m_sourceEntity.Type.GetFlagForType() : this.m_sourceEntity.Type.GetOppositeFlagForType();
			float? requiredAngle = null;
			if (targetingParams.TargetType.CheckAngle())
			{
				requiredAngle = new float?(targetingParams.GetAoeAngle(this.m_sourceEntity, this) * 0.5f);
			}
			int num = targetingParams.AoeMaxTargets;
			if (this.m_reagentItem != null)
			{
				num += this.m_reagentItem.GetAoeTargetMod();
			}
			this.GatherAoeTargets(validTargetType, validEntityTypes, targetingParams.RequiredHealthState, sampleSourceEntity, radius, num, requiredAngle, targetingParams.AoeGroupOnly, onlyGatherSingleClosest, considerLineOfSight, targetingParams.ConalType, targetingParams.AoeRandomTargetSelection);
		}

		// Token: 0x06002E83 RID: 11907 RVA: 0x00152D10 File Offset: 0x00150F10
		private void GatherAoeTargets(TargetType validTargetType, GameEntityTypeFlags validEntityTypes, HealthStateFlags validHealthState, GameEntity sampleSourceEntity, float radius, int maxTargets, float? requiredAngle, bool groupOnly, bool onlyGatherSingleClosest, bool considerLineOfSight, ConalTypes conalType = ConalTypes.None, bool randomTargetSelection = false)
		{
			if (!sampleSourceEntity)
			{
				return;
			}
			bool flag = this.m_sourceEntity && this.m_sourceEntity.Type == GameEntityType.Player && this.m_sourceEntity.CharacterData && this.m_sourceEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.Pvp);
			if (flag)
			{
				if (validTargetType == TargetType.Defensive)
				{
					groupOnly = true;
				}
				else
				{
					groupOnly = false;
					validEntityTypes |= GameEntityTypeFlags.Player;
				}
			}
			ExecutionCache.PotentialAoeTargets.Clear();
			Vector3 position = sampleSourceEntity.PrimaryTargetPoint.transform.position;
			Collider[] colliders = Hits.Colliders50;
			int num = Physics.OverlapSphereNonAlloc(position, radius * 1.5f, colliders, LayerMap.Detection.LayerMask, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < num; i++)
			{
				GameEntity gameEntity;
				if (colliders[i] && DetectionCollider.TryGetEntityForCollider(colliders[i], out gameEntity) && gameEntity.CharacterData && !this.m_targetsToExclude.Contains(gameEntity))
				{
					bool flag2 = gameEntity.Type.TypeInFlags(validEntityTypes);
					if (flag2 && gameEntity.Type == GameEntityType.Player && gameEntity.CharacterData)
					{
						flag2 = !gameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.NoTarget);
					}
					if (flag && gameEntity.Type == GameEntityType.Player)
					{
						if (flag2 && validTargetType == TargetType.Defensive && groupOnly)
						{
							flag2 = ((this.m_groupId != null) ? (gameEntity.CharacterData.GroupId == this.m_groupId.Value) : (gameEntity == this.m_sourceEntity));
						}
						else if (flag2 && validTargetType == TargetType.Offensive && !groupOnly)
						{
							if (gameEntity == this.m_sourceEntity)
							{
								flag2 = false;
							}
							else if (this.m_groupId != null)
							{
								flag2 = (this.m_groupId.Value != gameEntity.CharacterData.GroupId);
							}
							if (flag2)
							{
								flag2 = gameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.Pvp);
							}
						}
					}
					else
					{
						if (flag2 && this.m_sourceEntity.Type == GameEntityType.Player)
						{
							flag2 = (validTargetType == gameEntity.CharacterData.Faction.GetPlayerTargetType());
						}
						if (flag2 && groupOnly && gameEntity.Type == GameEntityType.Player)
						{
							flag2 = ((this.m_groupId != null) ? (gameEntity.CharacterData.GroupId == this.m_groupId.Value) : (gameEntity == this.m_sourceEntity));
						}
					}
					if (flag2)
					{
						flag2 = ExecutionCache.EntityHasValidHealthState(gameEntity, validHealthState);
					}
					if (flag2)
					{
						flag2 = DistanceAngleChecks.MeetsDistanceRequirements(sampleSourceEntity, gameEntity, 0f, radius, null, null);
					}
					if (flag2 && requiredAngle != null)
					{
						flag2 = DistanceAngleChecks.MeetsAngleRequirements(sampleSourceEntity, gameEntity, requiredAngle.Value, null, null, conalType);
					}
					if (flag2 && considerLineOfSight)
					{
						flag2 = LineOfSight.AoeHasLineOfSight(position, gameEntity);
					}
					if (flag2)
					{
						ExecutionCache.PotentialAoeTargets.Add(new ExecutionCache.PotentialAoeTarget
						{
							Entity = gameEntity,
							SqrDistance = (position - gameEntity.PrimaryTargetPoint.transform.position).sqrMagnitude
						});
					}
				}
			}
			if (ExecutionCache.PotentialAoeTargets.Count > 0)
			{
				if (onlyGatherSingleClosest)
				{
					ExecutionCache.PotentialAoeTargets.Sort(ExecutionCache.AoeTargetSorter);
					this.AddTarget(ExecutionCache.PotentialAoeTargets[0].Entity, true);
				}
				else
				{
					if (randomTargetSelection)
					{
						ExecutionCache.PotentialAoeTargets.Shuffle<ExecutionCache.PotentialAoeTarget>();
					}
					else
					{
						ExecutionCache.PotentialAoeTargets.Sort(ExecutionCache.AoeTargetSorter);
					}
					for (int j = 0; j < ExecutionCache.PotentialAoeTargets.Count; j++)
					{
						this.AddTarget(ExecutionCache.PotentialAoeTargets[j].Entity, false);
						if (this.m_targets.Count >= maxTargets)
						{
							break;
						}
					}
				}
			}
			ExecutionCache.PotentialAoeTargets.Clear();
		}

		// Token: 0x04002DC1 RID: 11713
		private readonly List<ExecutionCache.ReductionTask> m_reductionTasks;

		// Token: 0x04002DC2 RID: 11714
		private readonly List<ExecutionCache.PostReductionTask> m_postReductionTasks;

		// Token: 0x04002DC3 RID: 11715
		private readonly CachedHandHeldItem m_mainHandCache;

		// Token: 0x04002DC4 RID: 11716
		private readonly CachedHandHeldItem m_offHandCache;

		// Token: 0x04002DC5 RID: 11717
		private readonly DynamicVitalMods m_externalMods;

		// Token: 0x04002DC6 RID: 11718
		private bool m_inPool;

		// Token: 0x04002DC7 RID: 11719
		private bool m_isInstant;

		// Token: 0x04002DC8 RID: 11720
		private bool m_useTargetAtExecutionComplete;

		// Token: 0x04002DC9 RID: 11721
		private GameEntity m_sourceEntity;

		// Token: 0x04002DCA RID: 11722
		private GameEntity m_offensiveTarget;

		// Token: 0x04002DCB RID: 11723
		private GameEntity m_defensiveTarget;

		// Token: 0x04002DCC RID: 11724
		private Vector3? m_groundTarget;

		// Token: 0x04002DCD RID: 11725
		private IExecutable m_executable;

		// Token: 0x04002DCE RID: 11726
		private ExecutionParams m_executionParams;

		// Token: 0x04002DCF RID: 11727
		private ICombatEffectSource m_effectSource;

		// Token: 0x04002DD0 RID: 11728
		private TargetingParams m_targetingParams;

		// Token: 0x04002DD1 RID: 11729
		private KinematicParameters m_kinematicParameters;

		// Token: 0x04002DD2 RID: 11730
		private CombatEffect m_combatEffect;

		// Token: 0x04002DD3 RID: 11731
		private GameEntity m_targetEntity;

		// Token: 0x04002DD4 RID: 11732
		private NetworkEntity m_targetNetworkEntity;

		// Token: 0x04002DD5 RID: 11733
		private ArchetypeInstance m_instance;

		// Token: 0x04002DD6 RID: 11734
		private UniqueId m_archetypeId = UniqueId.Empty;

		// Token: 0x04002DD7 RID: 11735
		private ArchetypeInstance m_masteryInstance;

		// Token: 0x04002DD8 RID: 11736
		private float m_masteryLevel;

		// Token: 0x04002DD9 RID: 11737
		private float m_abilityLevel;

		// Token: 0x04002DDA RID: 11738
		private byte m_abilityLevelAsByte;

		// Token: 0x04002DDB RID: 11739
		private UniqueId? m_groupId;

		// Token: 0x04002DDC RID: 11740
		private UniqueId? m_reagentId;

		// Token: 0x04002DDD RID: 11741
		private ReagentItem m_reagentItem;

		// Token: 0x04002DDE RID: 11742
		private float m_executionTime;

		// Token: 0x04002DDF RID: 11743
		private float m_staminaCost;

		// Token: 0x04002DE0 RID: 11744
		private float m_cooldown;

		// Token: 0x04002DE1 RID: 11745
		private EffectSourceType m_sourceType;

		// Token: 0x04002DE2 RID: 11746
		private AlchemyPowerLevel m_alchemyPowerLevel;

		// Token: 0x04002DE5 RID: 11749
		public MessageType MsgType;

		// Token: 0x04002DE6 RID: 11750
		public string Message;

		// Token: 0x04002DE7 RID: 11751
		public ArchetypeInstance ToolInstance;

		// Token: 0x04002DE8 RID: 11752
		public CraftingToolItem ToolItem;

		// Token: 0x04002DE9 RID: 11753
		public ExecutionCache.RefinementCache? Refinement;

		// Token: 0x04002DEA RID: 11754
		private const int kMaxAoeToCollect = 50;

		// Token: 0x04002DEB RID: 11755
		private static readonly List<ExecutionCache.PotentialAoeTarget> PotentialAoeTargets = new List<ExecutionCache.PotentialAoeTarget>(50);

		// Token: 0x04002DEC RID: 11756
		private static readonly Comparison<ExecutionCache.PotentialAoeTarget> AoeTargetSorter = new Comparison<ExecutionCache.PotentialAoeTarget>(ExecutionCache.SortBySqrMagnitudeDistance);

		// Token: 0x04002DED RID: 11757
		private float? m_applicationTime;

		// Token: 0x04002DEE RID: 11758
		private int? m_lowestNpcTargetLevel;

		// Token: 0x04002DEF RID: 11759
		private int? m_highestNpcTargetLevel;

		// Token: 0x04002DF0 RID: 11760
		private readonly List<GameEntity> m_targets;

		// Token: 0x04002DF1 RID: 11761
		private readonly HashSet<GameEntity> m_targetsToExclude;

		// Token: 0x04002DF2 RID: 11762
		private bool m_primaryApplied;

		// Token: 0x020005BB RID: 1467
		private struct ReductionTask
		{
			// Token: 0x06002E85 RID: 11909 RVA: 0x0006044E File Offset: 0x0005E64E
			public ReductionTask(ReductionTaskType type, ArchetypeInstance instance, int value)
			{
				this.Type = type;
				this.Instance = instance;
				this.Value = value;
			}

			// Token: 0x04002DF5 RID: 11765
			public ReductionTaskType Type;

			// Token: 0x04002DF6 RID: 11766
			public ArchetypeInstance Instance;

			// Token: 0x04002DF7 RID: 11767
			public int Value;
		}

		// Token: 0x020005BC RID: 1468
		private struct PostReductionTask
		{
			// Token: 0x06002E86 RID: 11910 RVA: 0x00060465 File Offset: 0x0005E665
			public PostReductionTask(PostReductionTaskType type, ArchetypeInstance itemInstance, ContainerInstance containerInstance, ItemAddContext context)
			{
				this.Type = type;
				this.ItemInstance = itemInstance;
				this.ContainterInstance = containerInstance;
				this.Context = context;
			}

			// Token: 0x04002DF8 RID: 11768
			public PostReductionTaskType Type;

			// Token: 0x04002DF9 RID: 11769
			public ArchetypeInstance ItemInstance;

			// Token: 0x04002DFA RID: 11770
			public ContainerInstance ContainterInstance;

			// Token: 0x04002DFB RID: 11771
			public ItemAddContext Context;
		}

		// Token: 0x020005BD RID: 1469
		public struct RefinementCache
		{
			// Token: 0x04002DFC RID: 11772
			public BaseArchetype Recipe;

			// Token: 0x04002DFD RID: 11773
			public ContainerInstance InputContainerInstance;

			// Token: 0x04002DFE RID: 11774
			public ContainerInstance OutputContainerInstance;

			// Token: 0x04002DFF RID: 11775
			public ArchetypeInstance CurrentOutputInstance;

			// Token: 0x04002E00 RID: 11776
			public List<ItemUsage> ItemsUsed;

			// Token: 0x04002E01 RID: 11777
			public int TargetAbilityLevel;
		}

		// Token: 0x020005BE RID: 1470
		private struct PotentialAoeTarget
		{
			// Token: 0x04002E02 RID: 11778
			public GameEntity Entity;

			// Token: 0x04002E03 RID: 11779
			public float SqrDistance;
		}
	}
}
