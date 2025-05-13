using System;
using System.Collections.Generic;
using SoL.Game.Crafting;
using SoL.Game.EffectSystem;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Objects.Containers;
using SoL.Game.Settings;
using SoL.Networking.Objects;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using SoL.Utilities.Logging;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005C8 RID: 1480
	public class ServerPlayerSkillsController : SkillsController
	{
		// Token: 0x17000A09 RID: 2569
		// (get) Token: 0x06002F2C RID: 12076 RVA: 0x000608A9 File Offset: 0x0005EAA9
		private static object[] LogArgs
		{
			get
			{
				if (ServerPlayerSkillsController.m_logArgs == null)
				{
					ServerPlayerSkillsController.m_logArgs = new object[3];
				}
				return ServerPlayerSkillsController.m_logArgs;
			}
		}

		// Token: 0x17000A0A RID: 2570
		// (get) Token: 0x06002F2D RID: 12077 RVA: 0x000608C2 File Offset: 0x0005EAC2
		private List<ExecutionCache> DelayedExecutionCaches
		{
			get
			{
				if (this.m_delayedExecutionCaches == null)
				{
					this.m_delayedExecutionCaches = new List<ExecutionCache>(5);
				}
				return this.m_delayedExecutionCaches;
			}
		}

		// Token: 0x06002F2E RID: 12078 RVA: 0x0006084B File Offset: 0x0005EA4B
		protected override void Awake()
		{
			base.Awake();
			this.m_pending = new SkillsController.PendingExecution(this);
		}

		// Token: 0x06002F2F RID: 12079 RVA: 0x000608DE File Offset: 0x0005EADE
		private void Update()
		{
			this.UpdateDelayedExecutionCaches();
			base.UpdateCooldowns();
		}

		// Token: 0x06002F30 RID: 12080 RVA: 0x000608EC File Offset: 0x0005EAEC
		private void CancelSpawnNoTarget()
		{
			if (base.GameEntity && base.GameEntity.Vitals)
			{
				base.GameEntity.Vitals.CancelSpawnNoTarget();
			}
		}

		// Token: 0x06002F31 RID: 12081 RVA: 0x001558A0 File Offset: 0x00153AA0
		private void UpdateDelayedExecutionCaches()
		{
			if (this.m_delayedExecutionCaches == null || this.m_delayedExecutionCaches.Count <= 0)
			{
				return;
			}
			float time = Time.time;
			for (int i = 0; i < this.m_delayedExecutionCaches.Count; i++)
			{
				ExecutionCache executionCache = this.m_delayedExecutionCaches[i];
				if (executionCache.ApplicationTime != null && time >= executionCache.ApplicationTime.Value)
				{
					this.m_delayedExecutionCaches.RemoveAt(i);
					i--;
					executionCache.ApplyEffects();
					StaticPool<ExecutionCache>.ReturnToPool(executionCache);
				}
			}
		}

		// Token: 0x06002F32 RID: 12082 RVA: 0x0015592C File Offset: 0x00153B2C
		private bool TryGetExecutionData(ref ClientExecutionCache clientExecutionCache, out ArchetypeInstance instance, out IExecutable executable)
		{
			instance = null;
			executable = null;
			switch (clientExecutionCache.SourceType)
			{
			case EffectSourceType.Ability:
				base.GameEntity.CollectionController.Abilities.TryGetInstanceForArchetypeId(clientExecutionCache.ArchetypeId, out instance);
				break;
			case EffectSourceType.Consumable:
			{
				ContainerInstance containerInstance;
				if (clientExecutionCache.InstanceId != null && !string.IsNullOrEmpty(clientExecutionCache.ContainerId) && base.GameEntity.CollectionController.TryGetInstance(clientExecutionCache.ContainerId, out containerInstance))
				{
					containerInstance.TryGetInstanceForInstanceId(clientExecutionCache.InstanceId.Value, out instance);
				}
				if (instance != null && instance.ArchetypeId != clientExecutionCache.ArchetypeId)
				{
					instance = null;
				}
				break;
			}
			case EffectSourceType.Dynamic:
			{
				DynamicAbility dynamicAbility;
				if (InternalGameDatabase.Archetypes.GetDynamicAbilityByArchetypeId(clientExecutionCache.ArchetypeId, out dynamicAbility))
				{
					instance = dynamicAbility.DynamicallyLoad(base.GameEntity.CollectionController.Abilities);
				}
				break;
			}
			}
			return instance != null && instance.Archetype && instance.Archetype.TryGetAsType(out executable);
		}

		// Token: 0x06002F33 RID: 12083 RVA: 0x00155A3C File Offset: 0x00153C3C
		private void SetExecutionCacheTargets(ref ClientExecutionCache clientExecutionCache, ExecutionCache executionCache)
		{
			if (executionCache != null && clientExecutionCache.TargetEntity != null && clientExecutionCache.TargetEntity.GameEntity.Interactive != null)
			{
				if (clientExecutionCache.TargetEntity.GameEntity.Interactive is IGatheringNode)
				{
					executionCache.SetTargetNetworkEntity(clientExecutionCache.TargetEntity);
					return;
				}
				if (clientExecutionCache.TargetEntity.GameEntity == base.GameEntity && clientExecutionCache.ArchetypeId == GlobalSettings.Values.Subscribers.DeployPortableCraftingStationAbility.Id)
				{
					executionCache.SetTargetNetworkEntity(clientExecutionCache.TargetEntity);
				}
			}
		}

		// Token: 0x06002F34 RID: 12084 RVA: 0x00155AD8 File Offset: 0x00153CD8
		public override void Server_Execute_Instant(ClientExecutionCache clientExecutionCache)
		{
			this.CancelSpawnNoTarget();
			ArchetypeInstance instance;
			IExecutable executable;
			if (this.TryGetExecutionData(ref clientExecutionCache, out instance, out executable))
			{
				ExecutionCache fromPool = StaticPool<ExecutionCache>.GetFromPool();
				fromPool.InitInstant(base.GameEntity, instance);
				if (fromPool.IsInstant && fromPool.Executable != null && fromPool.Executable.PreExecution(fromPool))
				{
					this.SetExecutionCacheTargets(ref clientExecutionCache, fromPool);
					base.GameEntity.Vitals.AlterStamina((float)(-(float)fromPool.ExecutionParams.StaminaCost));
					fromPool.ApplyEffects();
					IExecutable executable2 = fromPool.Executable;
					if (executable2 != null)
					{
						executable2.PostExecution(fromPool);
					}
					base.GameEntity.NetworkEntity.PlayerRpcHandler.Server_Execute_Instant(fromPool.ArchetypeId, fromPool.TargetNetworkEntity, fromPool.AbilityLevelAsByte);
				}
				else
				{
					string message = string.IsNullOrEmpty(fromPool.Message) ? "Unknown Error" : fromPool.Message;
					base.GameEntity.NetworkEntity.PlayerRpcHandler.Server_Execute_Instant_Failed(message);
				}
				StaticPool<ExecutionCache>.ReturnToPool(fromPool);
			}
		}

		// Token: 0x06002F35 RID: 12085 RVA: 0x00155BCC File Offset: 0x00153DCC
		public override void Server_Execution_Begin(DateTime timestamp, ClientExecutionCache clientExecutionCache)
		{
			this.CancelSpawnNoTarget();
			if (this.m_pending.Active)
			{
				string text = (this.m_pending.Executable == null) ? "Unknown" : this.m_pending.Executable.DisplayName;
				double totalSeconds = (DateTime.UtcNow - this.m_pending.ServerTimestamp).TotalSeconds;
				if (totalSeconds <= (double)this.m_pending.ExecutionTime)
				{
					Debug.LogWarning(string.Concat(new string[]
					{
						"Execution of ",
						text,
						" is still on going, but ",
						base.GameEntity.CharacterData.Name.Value,
						" is attempting to execute another! ",
						totalSeconds.ToString(),
						" < ",
						this.m_pending.ExecutionTime.ToString()
					}));
					base.GameEntity.NetworkEntity.PlayerRpcHandler.Server_Execution_BeginFailed(clientExecutionCache.ArchetypeId, "Already pending!");
					return;
				}
				Debug.LogWarning(string.Concat(new string[]
				{
					"Execution of ",
					text,
					" failed or has exceeded it's time for ",
					base.GameEntity.CharacterData.Name.Value,
					"?  ",
					totalSeconds.ToString(),
					" > ",
					this.m_pending.ExecutionTime.ToString()
				}));
				this.m_pending.Reset();
			}
			ArchetypeInstance archetypeInstance;
			IExecutable executable;
			if (!this.TryGetExecutionData(ref clientExecutionCache, out archetypeInstance, out executable))
			{
				base.GameEntity.NetworkEntity.PlayerRpcHandler.Server_Execution_BeginFailed(clientExecutionCache.ArchetypeId, "Unknown Error");
				return;
			}
			AlchemyPowerLevel alchemyPowerLevel = AlchemyExtensions.AlchemyPowerLevelAvailable(base.GameEntity, archetypeInstance, clientExecutionCache.AlchemyPowerLevel) ? clientExecutionCache.AlchemyPowerLevel : AlchemyPowerLevel.None;
			ExecutionCache fromPool = StaticPool<ExecutionCache>.GetFromPool();
			fromPool.Init(base.GameEntity, archetypeInstance, executable, alchemyPowerLevel, clientExecutionCache.UseTargetAtExecutionComplete);
			BaseArchetype baseArchetype;
			if (clientExecutionCache.RecipeId != null && InternalGameDatabase.Archetypes.TryGetItem(clientExecutionCache.RecipeId.Value, out baseArchetype) && baseArchetype is Recipe)
			{
				ExecutionCache.RefinementCache refinementCache = new ExecutionCache.RefinementCache
				{
					Recipe = baseArchetype,
					TargetAbilityLevel = (clientExecutionCache.TargetAbilityLevel ?? -1)
				};
				if (clientExecutionCache.ItemsUsed != null && clientExecutionCache.ItemsUsed.Count > 0)
				{
					refinementCache.ItemsUsed = new List<ItemUsage>(clientExecutionCache.ItemsUsed.Count);
					foreach (ItemUsageSerializable itemUsageSerializable in clientExecutionCache.ItemsUsed)
					{
						RecipeComponent recipeComponent = null;
						foreach (RecipeComponent recipeComponent2 in ((Recipe)baseArchetype).Components)
						{
							if (recipeComponent2.Id == itemUsageSerializable.UsedFor)
							{
								recipeComponent = recipeComponent2;
							}
						}
						ArchetypeInstance instance = null;
						ContainerInstance containerInstance;
						ContainerInstance containerInstance2;
						if (recipeComponent != null && fromPool.SourceEntity.CollectionController.TryGetInstance(ContainerType.Inventory, out containerInstance) && fromPool.SourceEntity.CollectionController.TryGetInstance(ContainerType.Gathering, out containerInstance2) && (containerInstance2.TryGetInstanceForInstanceId(itemUsageSerializable.Instance, out instance) || containerInstance.TryGetInstanceForInstanceId(itemUsageSerializable.Instance, out instance)))
						{
							refinementCache.ItemsUsed.Add(new ItemUsage
							{
								Instance = instance,
								UsedFor = recipeComponent,
								AmountUsed = itemUsageSerializable.AmountUsed
							});
						}
					}
				}
				fromPool.Refinement = new ExecutionCache.RefinementCache?(refinementCache);
			}
			this.SetExecutionCacheTargets(ref clientExecutionCache, fromPool);
			if (!AlchemyExtensions.EntityHasRequiredEmberEssence(base.GameEntity, fromPool.AlchemyPowerLevel))
			{
				fromPool.Message = "Not enough Ember Essence!";
				base.GameEntity.NetworkEntity.PlayerRpcHandler.Server_Execution_BeginFailed(archetypeInstance.ArchetypeId, fromPool.Message);
				StaticPool<ExecutionCache>.ReturnToPool(fromPool);
				return;
			}
			if (!executable.PreExecution(fromPool) || fromPool.TargetNetworkEntity != clientExecutionCache.TargetEntity)
			{
				if (string.IsNullOrEmpty(fromPool.Message))
				{
					fromPool.Message = "Unknown Error";
				}
				base.GameEntity.NetworkEntity.PlayerRpcHandler.Server_Execution_BeginFailed(archetypeInstance.ArchetypeId, fromPool.Message);
				StaticPool<ExecutionCache>.ReturnToPool(fromPool);
				return;
			}
			this.m_pending.InitServer(fromPool, timestamp);
			base.GameEntity.NetworkEntity.PlayerRpcHandler.Server_Execution_Begin(archetypeInstance.ArchetypeId, this.m_pending.TargetNetworkEntity, this.m_pending.AbilityLevelAsByte, this.m_pending.AlchemyPowerLevel);
		}

		// Token: 0x06002F36 RID: 12086 RVA: 0x00156090 File Offset: 0x00154290
		public override void Server_Execute_AutoAttack(NetworkEntity targetEntity)
		{
			this.CancelSpawnNoTarget();
			if (this.m_autoAttackInstance == null && !base.GameEntity.CollectionController.Abilities.TryGetInstanceForInstanceId(ServerPlayerSkillsController.m_autoAttackId, out this.m_autoAttackInstance))
			{
				base.GameEntity.NetworkEntity.PlayerRpcHandler.Server_Execute_AutoAttack_Failed("Cannot find AutoAttackInstance??");
				return;
			}
			if (!this.m_activeCombatMastery || this.m_activeCombatMastery.Id != base.GameEntity.GetActiveWeaponMasteryId())
			{
				base.GameEntity.TryGetActiveWeaponMasteryAsType(out this.m_activeCombatMasteryInstance, out this.m_activeCombatMastery);
			}
			IExecutable executable;
			if (!this.m_activeCombatMastery || !this.m_activeCombatMastery.AutoAttackOverride)
			{
				executable = SkillsController.m_autoAttackExecutable;
			}
			else
			{
				IExecutable autoAttackOverride = this.m_activeCombatMastery.AutoAttackOverride;
				executable = autoAttackOverride;
			}
			IExecutable executable2 = executable;
			base.AutoAttackExecutionCache.Reset();
			base.AutoAttackExecutionCache.Init(base.GameEntity, this.m_autoAttackInstance, executable2, AlchemyPowerLevel.None, false);
			base.AutoAttackExecutionCache.MasteryInstance = this.m_activeCombatMasteryInstance;
			if (executable2.PreExecution(base.AutoAttackExecutionCache) && base.AutoAttackExecutionCache.TargetNetworkEntity == targetEntity)
			{
				executable2.PostExecution(base.AutoAttackExecutionCache);
				if (base.AutoAttackExecutionCache.TargetNetworkEntity)
				{
					base.AutoAttackExecutionCache.ApplyEffects();
				}
				base.GameEntity.NetworkEntity.PlayerRpcHandler.Server_Execute_AutoAttack(base.AutoAttackExecutionCache.TargetNetworkEntity, base.AutoAttackExecutionCache.AnimationFlags);
				return;
			}
			if (string.IsNullOrEmpty(base.AutoAttackExecutionCache.Message))
			{
				base.AutoAttackExecutionCache.Message = "No AutoAttack Error?!";
			}
			base.GameEntity.NetworkEntity.PlayerRpcHandler.Server_Execute_AutoAttack_Failed(base.AutoAttackExecutionCache.Message);
		}

		// Token: 0x06002F37 RID: 12087 RVA: 0x0006091D File Offset: 0x0005EB1D
		public override void Server_Execution_Cancel(UniqueId archetypeId)
		{
			if (this.m_pending.Active)
			{
				base.GameEntity.NetworkEntity.PlayerRpcHandler.Server_Execution_Cancel(archetypeId);
				this.m_pending.Reset();
			}
		}

		// Token: 0x06002F38 RID: 12088 RVA: 0x0015624C File Offset: 0x0015444C
		public override void Server_Execution_Complete(UniqueId archetypeId, DateTime timestamp)
		{
			if (!this.m_pending.Active || !this.m_pending.TargetNetworkEntity)
			{
				this.Server_Execution_Cancel(this.m_pending.ArchetypeId);
				return;
			}
			double totalSeconds = (DateTime.UtcNow - this.m_pending.ServerTimestamp).TotalSeconds;
			float num = (float)((double)this.m_pending.ExecutionTime - totalSeconds);
			bool flag = num <= 0f || Mathf.Abs(num) < 0.5f;
			if (!flag)
			{
				if (base.GameEntity.NetworkEntity.NetworkId.Peer.IsSet)
				{
					uint roundTripTime = base.GameEntity.NetworkEntity.NetworkId.Peer.RoundTripTime;
				}
				double totalSeconds2 = (timestamp - this.m_pending.ServerTimestamp).TotalSeconds;
				float num2 = (float)((double)this.m_pending.ExecutionTime - totalSeconds2);
				double num3 = Math.Abs(totalSeconds - totalSeconds2);
				bool flag2 = num2 <= 0f && num3 < 1.0;
				ServerPlayerSkillsController.LogArgs[0] = totalSeconds;
				ServerPlayerSkillsController.LogArgs[1] = totalSeconds2;
				ServerPlayerSkillsController.LogArgs[2] = num3;
				SolDebug.LogToIndex(LogLevel.Warning, LogIndex.Execution, "{@TimeElapsed} {@ClientTimeElapsed} {@ClientTimeDelta}", ServerPlayerSkillsController.LogArgs);
				flag = (num2 <= 0f);
			}
			if (!flag)
			{
				this.Server_Execution_Cancel(archetypeId);
				return;
			}
			bool flag3 = this.m_pending.Instance != null && this.m_pending.Instance.Archetype && this.m_pending.Instance.Archetype is AuraAbility;
			if (!flag3 && this.m_pending.Instance != null && this.m_pending.Executable != null && this.m_pending.ExecutionCache != null && !this.m_pending.Executable.ContinuedExecution(this.m_pending.ExecutionCache, 1f))
			{
				this.Server_Execution_Cancel(this.m_pending.Instance.ArchetypeId);
				return;
			}
			if (this.m_pending.StaminaDrained < this.m_pending.StaminaCost)
			{
				float num4 = this.m_pending.StaminaCost - this.m_pending.StaminaDrained;
				base.GameEntity.Vitals.AlterStamina(-num4);
			}
			if (this.m_pending.AlchemyPowerLevel != AlchemyPowerLevel.None)
			{
				int requiredEmberEssence = this.m_pending.AlchemyPowerLevel.GetRequiredEmberEssence();
				base.GameEntity.CollectionController.AdjustEmberEssenceCount(-requiredEmberEssence);
			}
			if (this.m_pending.Instance == null)
			{
				Debug.LogWarning("m_pending.Instance is null for ArchetypeId: " + this.m_pending.ArchetypeId.ToString() + "?!");
			}
			bool flag4 = false;
			if (this.m_pending.EffectsNew != null && this.m_pending.ExecutionCache != null && !flag3)
			{
				ExecutionCache executionCache = this.m_pending.ExecutionCache;
				if (this.m_pending.EffectsNew.DeliveryParams != null && this.m_pending.EffectsNew.DeliveryParams.HasDelay && this.m_pending.TargetingParams != null)
				{
					Vector3 targetPosition = this.m_pending.TargetingParams.GetTargetPosition(executionCache);
					float? delay = this.m_pending.EffectsNew.DeliveryParams.GetDelay(base.GameEntity.gameObject.transform.position, targetPosition);
					executionCache.ApplicationTime = Time.time + delay;
					this.DelayedExecutionCaches.Add(executionCache);
					flag4 = true;
				}
				else
				{
					executionCache.ApplyEffects();
				}
			}
			IExecutable executable = this.m_pending.Executable;
			if (executable != null)
			{
				executable.PostExecution(this.m_pending.ExecutionCache);
			}
			if (this.m_pending.ExecutionCache != null && this.m_pending.ExecutionCache.UseTargetAtExecutionComplete && this.m_pending.ExecutionCache.TargetNetworkEntity && this.m_pending.ExecutionCache.TargetNetworkEntity != this.m_pending.TargetNetworkEntity)
			{
				this.m_pending.UpdateTargetNetworkEntity(this.m_pending.ExecutionCache.TargetNetworkEntity);
				base.GameEntity.NetworkEntity.PlayerRpcHandler.Server_Execution_Complete_UpdateTarget(archetypeId, this.m_pending.TargetNetworkEntity);
			}
			else
			{
				base.GameEntity.NetworkEntity.PlayerRpcHandler.Server_Execution_Complete(archetypeId);
			}
			if (flag4)
			{
				this.m_pending.NullifyExecutionCache();
			}
			this.m_pending.Reset();
		}

		// Token: 0x04002E48 RID: 11848
		private const float kServerThreshold = 0.5f;

		// Token: 0x04002E49 RID: 11849
		private const string kUnknownError = "Unknown Error";

		// Token: 0x04002E4A RID: 11850
		private static object[] m_logArgs = null;

		// Token: 0x04002E4B RID: 11851
		private List<ExecutionCache> m_delayedExecutionCaches;

		// Token: 0x04002E4C RID: 11852
		private static readonly UniqueId m_autoAttackId = new UniqueId("AUTOATTACK");

		// Token: 0x04002E4D RID: 11853
		private ArchetypeInstance m_autoAttackInstance;

		// Token: 0x04002E4E RID: 11854
		private MasteryArchetype m_activeCombatMastery;

		// Token: 0x04002E4F RID: 11855
		private ArchetypeInstance m_activeCombatMasteryInstance;
	}
}
