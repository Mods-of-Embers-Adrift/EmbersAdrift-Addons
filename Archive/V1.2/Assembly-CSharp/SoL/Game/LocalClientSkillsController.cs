using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Animation;
using SoL.Game.EffectSystem;
using SoL.Game.Messages;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Objects.Containers;
using SoL.Game.Settings;
using SoL.Game.UI;
using SoL.Managers;
using SoL.Networking.Objects;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005C1 RID: 1473
	public class LocalClientSkillsController : SkillsController
	{
		// Token: 0x140000A0 RID: 160
		// (add) Token: 0x06002EA2 RID: 11938 RVA: 0x0015319C File Offset: 0x0015139C
		// (remove) Token: 0x06002EA3 RID: 11939 RVA: 0x001531D0 File Offset: 0x001513D0
		public static event Action<ArchetypeInstance> MasteryLevelChangedEvent;

		// Token: 0x140000A1 RID: 161
		// (add) Token: 0x06002EA4 RID: 11940 RVA: 0x00153204 File Offset: 0x00151404
		// (remove) Token: 0x06002EA5 RID: 11941 RVA: 0x00153238 File Offset: 0x00151438
		public static event Action<AlchemyPowerLevel> PendingCancelled;

		// Token: 0x140000A2 RID: 162
		// (add) Token: 0x06002EA6 RID: 11942 RVA: 0x0015326C File Offset: 0x0015146C
		// (remove) Token: 0x06002EA7 RID: 11943 RVA: 0x001532A0 File Offset: 0x001514A0
		public static event Action AlchemyExecutionComplete;

		// Token: 0x06002EA8 RID: 11944 RVA: 0x001532D4 File Offset: 0x001514D4
		private void Start()
		{
			this.m_pendingPool = new LocalClientSkillsController.PendingPool(this);
			this.m_pendingExecutions = new Dictionary<UniqueId, SkillsController.PendingExecution>(default(UniqueIdComparer));
			if (InternalGameDatabase.GlobalSettings.GroundGizmosPrefab)
			{
				UnityEngine.Object.Instantiate<GameObject>(InternalGameDatabase.GlobalSettings.GroundGizmosPrefab, base.GameEntity.gameObject.transform).transform.localPosition = Vector3.zero + Vector3.up * 0.3f;
			}
		}

		// Token: 0x06002EA9 RID: 11945 RVA: 0x000604E7 File Offset: 0x0005E6E7
		protected override void OnDestroy()
		{
			base.OnDestroy();
			SkillsController.PendingExecution pending = this.m_pending;
			if (pending == null)
			{
				return;
			}
			pending.Reset();
		}

		// Token: 0x06002EAA RID: 11946 RVA: 0x0015335C File Offset: 0x0015155C
		private void Update()
		{
			if (GameManager.IsServer || !base.GameEntity.NetworkEntity.IsLocal)
			{
				return;
			}
			if (this.m_pending != null)
			{
				if (this.m_pending.Active && SolInput.GetButtonDown(103))
				{
					this.RequestCancel(this.m_pending);
				}
				this.m_pending.UpdateDelayedAnimation();
			}
			this.UpdatePending();
			base.UpdateCooldowns();
			if (base.AutoAttackServerFailedDelayUntil != null && Time.time >= base.AutoAttackServerFailedDelayUntil.Value)
			{
				base.AutoAttackServerFailedDelayUntil = null;
			}
		}

		// Token: 0x06002EAB RID: 11947 RVA: 0x001533F8 File Offset: 0x001515F8
		private void UpdatePending()
		{
			if (this.m_pendingExecutions.Count <= 0)
			{
				return;
			}
			bool flag = false;
			foreach (KeyValuePair<UniqueId, SkillsController.PendingExecution> keyValuePair in this.m_pendingExecutions)
			{
				SkillsController.PendingExecution value = keyValuePair.Value;
				if (value.Active)
				{
					if (base.GameEntity.VitalsReplicator.BehaviorFlags.Value.CancelExecutionForFlag())
					{
						if (!flag)
						{
							this.LogInfo(base.GameEntity.VitalsReplicator.BehaviorFlags.Value.CancelExecutionForFlagDescription());
							flag = true;
						}
						this.RequestCancel(value);
					}
					else
					{
						value.UpdateLocalClientTimeRemaining();
						if (value.Instance != null && value.Executable != null)
						{
							if (value.ExecutionCache != null && !value.Executable.ContinuedExecution(value.ExecutionCache, 1f - value.ExecutionTimeRemaining / value.ExecutionTime))
							{
								this.LogInfo(value.ExecutionCache.Message);
								this.RequestCancel(value);
							}
							else if (value.ExecutionTimeRemaining <= 0f)
							{
								base.GameEntity.NetworkEntity.PlayerRpcHandler.Client_Execution_Complete(value.Instance.ArchetypeId, DateTime.UtcNow);
								if (value.Executable.TriggerGlobalCooldown)
								{
									base.OnGlobalCooldown();
								}
								value.Status = SkillsController.PendingExecution.PendingStatus.CompleteSent;
								base.OnPendingExecutionChanged(value);
								ExecutionCache executionCache = value.ExecutionCache;
								if (executionCache != null)
								{
									executionCache.LocalPlayerComplete(false, true);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06002EAC RID: 11948 RVA: 0x00153594 File Offset: 0x00151794
		private void RequestCancel(SkillsController.PendingExecution pending)
		{
			base.GameEntity.NetworkEntity.PlayerRpcHandler.Client_Execution_Cancel(pending.ArchetypeId);
			AlchemyPowerLevel alchemyPowerLevel = pending.AlchemyPowerLevel;
			pending.Status = SkillsController.PendingExecution.PendingStatus.CancelSent;
			base.OnPendingExecutionChanged(pending);
			Action<AlchemyPowerLevel> pendingCancelled = LocalClientSkillsController.PendingCancelled;
			if (pendingCancelled == null)
			{
				return;
			}
			pendingCancelled(alchemyPowerLevel);
		}

		// Token: 0x06002EAD RID: 11949 RVA: 0x001535E4 File Offset: 0x001517E4
		public override bool EscapePressed()
		{
			bool result = false;
			if (this.m_pending != null && this.m_pending.Active)
			{
				this.RequestCancel(this.m_pending);
				result = true;
			}
			return result;
		}

		// Token: 0x06002EAE RID: 11950 RVA: 0x00153618 File Offset: 0x00151818
		public override bool BeginExecution(ArchetypeInstance instance)
		{
			LocalPlayer.UpdateTimeOfLastInput();
			if (this.m_pending != null)
			{
				if ((DateTime.UtcNow - this.m_pending.ClientTimestamp).TotalSeconds <= (double)(this.m_pending.ExecutionTime * 1.1f))
				{
					this.LogInfo("Currently Executing!");
					return false;
				}
				if (this.m_pending.Executable != null)
				{
					Debug.LogWarning("Execution of " + this.m_pending.Executable.DisplayName + " failed?");
				}
				this.ResetPending(this.m_pending);
			}
			if (this.m_instantPending != null)
			{
				if ((DateTime.UtcNow - this.m_instantPending.ClientTimestamp).TotalSeconds <= 1.100000023841858)
				{
					return false;
				}
				if (this.m_instantPending.Executable != null)
				{
					Debug.LogWarning("InstantExecution of " + this.m_instantPending.Executable.DisplayName + " failed?");
				}
				this.m_instantPending.Status = SkillsController.PendingExecution.PendingStatus.Failed;
				this.m_pendingPool.ReturnToPool(this.m_instantPending);
				this.m_instantPending = null;
			}
			IExecutable executable;
			if (!instance.Archetype.TryGetAsType(out executable))
			{
				Debug.LogWarning("Could not execute Instance " + instance.InstanceId.ToString() + "!");
				return false;
			}
			AlchemyPowerLevel alchemyPowerLevel = AlchemyPowerLevel.None;
			if (executable.AllowAlchemy && AlchemySelectionUI.LevelFlags.HasBitFlag(AlchemyPowerLevelFlags.II) && AlchemyExtensions.AlchemyPowerLevelAvailable(base.GameEntity, instance, AlchemyPowerLevel.II))
			{
				alchemyPowerLevel = AlchemyPowerLevel.II;
			}
			else if (executable.AllowAlchemy && AlchemySelectionUI.LevelFlags.HasBitFlag(AlchemyPowerLevelFlags.I) && AlchemyExtensions.AlchemyPowerLevelAvailable(base.GameEntity, instance, AlchemyPowerLevel.I))
			{
				alchemyPowerLevel = AlchemyPowerLevel.I;
			}
			ExecutionCache fromPool = StaticPool<ExecutionCache>.GetFromPool();
			fromPool.Init(base.GameEntity, instance, executable, alchemyPowerLevel, Options.GameOptions.UseTargetAtExecutionComplete.Value);
			if (!executable.PreExecution(fromPool))
			{
				this.LogInfo(fromPool.Message);
				if (fromPool.MsgType != MessageType.None)
				{
					MessageManager.ChatQueue.AddToQueue(fromPool.MsgType, fromPool.Message);
				}
				StaticPool<ExecutionCache>.ReturnToPool(fromPool);
				return false;
			}
			object obj = fromPool.ExecutionTime <= 0f && fromPool.AlchemyPowerLevel == AlchemyPowerLevel.None;
			SkillsController.PendingExecution fromPool2 = this.m_pendingPool.GetFromPool();
			fromPool2.InitLocalClient(fromPool);
			object obj2 = obj;
			if (obj2 != null)
			{
				this.m_instantPending = fromPool2;
				base.GameEntity.NetworkEntity.PlayerRpcHandler.Client_Execution_Instant(fromPool2.ExecutionCache.GetClientExecutionCache());
				ExecutionCache executionCache = this.m_instantPending.ExecutionCache;
				if (executionCache != null)
				{
					executionCache.LocalPlayerComplete(false, true);
				}
			}
			else
			{
				this.m_pendingExecutions.Add(instance.ArchetypeId, fromPool2);
				this.m_pending = fromPool2;
				base.GameEntity.NetworkEntity.PlayerRpcHandler.Client_Execution_Begin(fromPool2.ClientTimestamp, fromPool2.ExecutionCache.GetClientExecutionCache());
			}
			fromPool2.Client_Execution_Begin(executable, fromPool2.TargetNetworkEntity);
			if (obj2 == null)
			{
				base.OnPendingExecutionChanged(fromPool2);
			}
			LocalPlayer.ClearFollowTarget();
			return true;
		}

		// Token: 0x06002EAF RID: 11951 RVA: 0x001538F8 File Offset: 0x00151AF8
		public override void Client_Execute_Instant(UniqueId archetypeId, NetworkEntity targetEntity, byte abilityLevel)
		{
			if (this.m_instantPending != null)
			{
				if (this.m_instantPending.Executable != null)
				{
					this.m_instantPending.Executable.PostExecution(this.m_instantPending.ExecutionCache);
					if (this.m_instantPending.Executable.TriggerGlobalCooldown)
					{
						base.OnGlobalCooldown();
					}
				}
				this.m_instantPending.Status = SkillsController.PendingExecution.PendingStatus.CompleteReceived;
				this.m_pendingPool.ReturnToPool(this.m_instantPending);
				this.m_instantPending = null;
			}
		}

		// Token: 0x06002EB0 RID: 11952 RVA: 0x000604FF File Offset: 0x0005E6FF
		public override void Client_Execute_Instant_Failed(string message)
		{
			if (this.m_instantPending != null)
			{
				this.m_instantPending.Status = SkillsController.PendingExecution.PendingStatus.Failed;
				this.m_pendingPool.ReturnToPool(this.m_instantPending);
				this.m_instantPending = null;
			}
			this.LogInfo(message);
		}

		// Token: 0x06002EB1 RID: 11953 RVA: 0x00153974 File Offset: 0x00151B74
		public override void Client_Execution_Begin(UniqueId archetypeId, NetworkEntity targetEntity, byte abilityLevel, AlchemyPowerLevel alchemyPowerLevel)
		{
			SkillsController.PendingExecution pendingExecution;
			if (this.m_pendingExecutions.TryGetValue(archetypeId, out pendingExecution))
			{
				pendingExecution.Status = SkillsController.PendingExecution.PendingStatus.ActiveAcknowledged;
			}
		}

		// Token: 0x06002EB2 RID: 11954 RVA: 0x00153998 File Offset: 0x00151B98
		public override void Client_BeginExecution_Failed(UniqueId archetypeId, string message)
		{
			SkillsController.PendingExecution pendingExecution;
			if (this.m_pendingExecutions.TryGetValue(archetypeId, out pendingExecution))
			{
				pendingExecution.Status = SkillsController.PendingExecution.PendingStatus.Failed;
				this.ResetPending(pendingExecution);
			}
			this.LogInfo(message);
		}

		// Token: 0x06002EB3 RID: 11955 RVA: 0x001539CC File Offset: 0x00151BCC
		public override void Client_Execution_Complete(UniqueId archetypeId, NetworkEntity updatedTargetEntity)
		{
			SkillsController.PendingExecution pendingExecution;
			if (this.m_pendingExecutions.TryGetValue(archetypeId, out pendingExecution))
			{
				int? num = null;
				if (updatedTargetEntity)
				{
					pendingExecution.UpdateTargetNetworkEntity(updatedTargetEntity);
				}
				if (pendingExecution.Executable != null)
				{
					if (pendingExecution.Executable.AllowAlchemy && pendingExecution.AlchemyPowerLevel == AlchemyPowerLevel.I && pendingExecution.Instance != null && pendingExecution.Instance.AbilityData != null)
					{
						num = new int?(pendingExecution.Instance.AbilityData.GetUsageCount(pendingExecution.AlchemyPowerLevel));
					}
					pendingExecution.Executable.PostExecution(pendingExecution.ExecutionCache);
				}
				if (pendingExecution.AlchemyPowerLevel != AlchemyPowerLevel.None)
				{
					if (num != null)
					{
						int alchemyUsageThreshold = GlobalSettings.Values.Ashen.GetAlchemyUsageThreshold(AlchemyPowerLevel.II);
						int usageCount = pendingExecution.Instance.AbilityData.GetUsageCount(pendingExecution.AlchemyPowerLevel);
						if (num.Value < alchemyUsageThreshold && usageCount >= alchemyUsageThreshold)
						{
							string content = ZString.Format<string, string>("{0} {1} has been unlocked!", AlchemyPowerLevel.II.GetAlchemyPowerLevelDescription(), pendingExecution.Instance.Archetype.GetModifiedDisplayName(pendingExecution.Instance));
							MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content);
						}
					}
					LocalClientSkillsController.AlchemyExecutionCompleteFrame = Time.frameCount;
					Action alchemyExecutionComplete = LocalClientSkillsController.AlchemyExecutionComplete;
					if (alchemyExecutionComplete != null)
					{
						alchemyExecutionComplete();
					}
				}
				pendingExecution.Status = SkillsController.PendingExecution.PendingStatus.CompleteReceived;
				this.ResetPending(pendingExecution);
			}
		}

		// Token: 0x06002EB4 RID: 11956 RVA: 0x00153B0C File Offset: 0x00151D0C
		public override void Client_Execution_Cancelled(UniqueId archetypeId)
		{
			SkillsController.PendingExecution pendingExecution;
			if (this.m_pendingExecutions.TryGetValue(archetypeId, out pendingExecution))
			{
				pendingExecution.Status = SkillsController.PendingExecution.PendingStatus.CancelReceived;
				this.ResetPending(pendingExecution);
			}
		}

		// Token: 0x06002EB5 RID: 11957 RVA: 0x00153B38 File Offset: 0x00151D38
		private void ResetPending(SkillsController.PendingExecution pending)
		{
			base.OnPendingExecutionChanged(pending);
			if (this.m_pending != null && pending.ArchetypeId == this.m_pending.ArchetypeId)
			{
				this.m_pending = null;
			}
			this.m_pendingExecutions.Remove(pending.ArchetypeId);
			this.m_pendingPool.ReturnToPool(pending);
		}

		// Token: 0x06002EB6 RID: 11958 RVA: 0x00153B94 File Offset: 0x00151D94
		public override float? ExecuteAutoAttack(ArchetypeInstance instance, bool logError)
		{
			base.AutoAttackExecutionCache.Init(base.GameEntity, instance, SkillsController.m_autoAttackExecutable, AlchemyPowerLevel.None, false);
			this.m_autoAttackInstance = instance;
			if (SkillsController.m_autoAttackExecutable.PreExecution(base.AutoAttackExecutionCache))
			{
				base.AutoAttackPending = true;
				base.GameEntity.NetworkEntity.PlayerRpcHandler.Client_Execute_AutoAttack(base.AutoAttackExecutionCache.TargetNetworkEntity);
				return new float?((float)base.AutoAttackExecutionCache.ExecutionParams.Cooldown);
			}
			if (logError)
			{
				this.LogInfo(base.AutoAttackExecutionCache.Message);
			}
			base.AutoAttackExecutionCache.Reset();
			return null;
		}

		// Token: 0x06002EB7 RID: 11959 RVA: 0x00060534 File Offset: 0x0005E734
		public override void ExecuteAutoAttackFailed(string message)
		{
			this.LogInfo(message);
			base.AutoAttackExecutionCache.Reset();
			base.AutoAttackPending = false;
			base.AutoAttackServerFailedDelayUntil = new float?(Time.time + 0.1f);
		}

		// Token: 0x06002EB8 RID: 11960 RVA: 0x00060565 File Offset: 0x0005E765
		public override void Client_AutoAttack(NetworkEntity targetEntity, AnimationFlags animFlags)
		{
			SkillsController.m_autoAttackExecutable.PostExecution(base.AutoAttackExecutionCache);
			base.AutoAttackExecutionCache.Reset();
			base.AutoAttackPending = false;
			base.Client_AutoAttack(targetEntity, animFlags);
		}

		// Token: 0x06002EB9 RID: 11961 RVA: 0x00153C3C File Offset: 0x00151E3C
		private void LogInfo(string txt)
		{
			if (string.IsNullOrEmpty(txt))
			{
				return;
			}
			Debug.LogWarning(txt);
			if (ClientGameManager.CombatTextManager && base.GameEntity && base.GameEntity.gameObject && base.GameEntity.gameObject.transform)
			{
				ClientGameManager.CombatTextManager.InitializeOverheadCombatText(txt, base.GameEntity, Color.yellow, null);
			}
		}

		// Token: 0x06002EBA RID: 11962 RVA: 0x00153CB4 File Offset: 0x00151EB4
		public void AutoAttack()
		{
			AutoAttackAbility autoAttack = GlobalSettings.Values.Combat.AutoAttack;
			ArchetypeInstance instance;
			if (base.GameEntity.CollectionController.Abilities.TryGetInstanceForArchetypeId(autoAttack.Id, out instance))
			{
				this.BeginExecution(instance);
			}
		}

		// Token: 0x06002EBB RID: 11963 RVA: 0x00153CF8 File Offset: 0x00151EF8
		public override void MasteryLevelChanged(UniqueId masteryArchetypeId, float newLevel)
		{
			base.MasteryLevelChanged(masteryArchetypeId, newLevel);
			ContainerInstance containerInstance;
			ArchetypeInstance archetypeInstance;
			if (base.GameEntity.CollectionController.TryGetInstance(ContainerType.Masteries, out containerInstance) && containerInstance.TryGetInstanceForArchetypeId(masteryArchetypeId, out archetypeInstance))
			{
				float baseLevel = archetypeInstance.MasteryData.BaseLevel;
				archetypeInstance.MasteryData.BaseLevel = newLevel;
				bool flag = false;
				string content = "";
				float num = Mathf.Floor(baseLevel);
				float num2 = Mathf.Floor(newLevel);
				if (num < num2)
				{
					content = string.Concat(new string[]
					{
						"You have increased your mastery in ",
						archetypeInstance.Archetype.DisplayName,
						"! (",
						((int)num2).ToString(),
						")"
					});
					flag = true;
				}
				else if (num > num2)
				{
					content = string.Concat(new string[]
					{
						"You have decreased your mastery in ",
						archetypeInstance.Archetype.DisplayName,
						"! (",
						((int)num2).ToString(),
						")"
					});
					flag = true;
				}
				if (flag)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Skills, content);
					Action<ArchetypeInstance> masteryLevelChangedEvent = LocalClientSkillsController.MasteryLevelChangedEvent;
					if (masteryLevelChangedEvent != null)
					{
						masteryLevelChangedEvent(archetypeInstance);
					}
					ClientGameManager.UIManager.PlayClip(GlobalSettings.Values.Audio.LevelUpClip, new float?(1f), new float?(GlobalSettings.Values.Audio.LevelUpVolume));
					base.GameEntity.Vitals.MasteryLevelChanged(archetypeInstance, baseLevel, newLevel);
				}
			}
		}

		// Token: 0x06002EBC RID: 11964 RVA: 0x00153E68 File Offset: 0x00152068
		public override void MasteryAbilityLevelChanged(InstanceNewLevelData newLevelData)
		{
			base.MasteryAbilityLevelChanged(newLevelData);
			bool flag = false;
			ArchetypeInstance instance;
			if (newLevelData.AbilityId != null && base.GameEntity.CollectionController.Abilities.TryGetInstanceForArchetypeId(newLevelData.AbilityId.Value, out instance))
			{
				bool flag2 = this.LevelInstance(instance, newLevelData.NewAbilityLevel);
				flag = (flag || flag2);
			}
			ArchetypeInstance instance2;
			if (newLevelData.MasteryId != null && base.GameEntity.CollectionController.Masteries.TryGetInstanceForArchetypeId(newLevelData.MasteryId.Value, out instance2))
			{
				bool flag3 = this.LevelInstance(instance2, newLevelData.NewMasteryLevel);
				flag = (flag || flag3);
			}
			if (flag)
			{
				ClientGameManager.UIManager.PlayClip(GlobalSettings.Values.Audio.LevelUpClip, new float?(1f), new float?(GlobalSettings.Values.Audio.LevelUpVolume));
			}
		}

		// Token: 0x06002EBD RID: 11965 RVA: 0x00153F44 File Offset: 0x00152144
		public override void LevelProgressionEvent(LevelProgressionEvent levelProgressionEvent)
		{
			base.LevelProgressionEvent(levelProgressionEvent);
			bool flag = false;
			for (int i = 0; i < levelProgressionEvent.EventData.Count; i++)
			{
				ArchetypeInstance archetypeInstance;
				if (((i == 0) ? base.GameEntity.CollectionController.Masteries : base.GameEntity.CollectionController.Abilities).TryGetInstanceForArchetypeId(levelProgressionEvent.EventData[i].ArchetypeId, out archetypeInstance))
				{
					bool flag2 = this.LevelInstance(archetypeInstance, levelProgressionEvent.EventData[i].NewLevel);
					flag = (flag || flag2);
					MasteryArchetype masteryArchetype;
					if (i == 0 && base.GameEntity.CharacterData && archetypeInstance.Archetype.TryGetAsType(out masteryArchetype))
					{
						base.GameEntity.CharacterData.UpdateHighestMasteryLevel(archetypeInstance);
						MessageManager.CombatQueue.AddToQueue(MessageType.MyCombatIn, GlobalSettings.Values.Progression.GetExperienceGainedChatMessage(masteryArchetype.Type, false));
					}
				}
			}
			if (flag)
			{
				ClientGameManager.UIManager.PlayClip(GlobalSettings.Values.Audio.LevelUpClip, new float?(1f), new float?(GlobalSettings.Values.Audio.LevelUpVolume));
			}
			StaticListPool<LevelProgressionData>.ReturnToPool(levelProgressionEvent.EventData);
		}

		// Token: 0x06002EBE RID: 11966 RVA: 0x00045BCA File Offset: 0x00043DCA
		private bool LevelInstance(ArchetypeInstance instance, float newLevel)
		{
			return false;
		}

		// Token: 0x06002EBF RID: 11967 RVA: 0x00154074 File Offset: 0x00152274
		public override void LevelProgressionUpdate(LevelProgressionUpdate levelProgressionUpdate)
		{
			base.LevelProgressionUpdate(levelProgressionUpdate);
			bool flag = false;
			MasteryType masteryType = MasteryType.None;
			ArchetypeInstance archetypeInstance;
			if (base.GameEntity && base.GameEntity.CharacterData && base.GameEntity.CollectionController != null && base.GameEntity.CollectionController.Masteries != null && base.GameEntity.CollectionController.Masteries.TryGetInstanceForArchetypeId(levelProgressionUpdate.ArchetypeId, out archetypeInstance) && archetypeInstance.Mastery != null)
			{
				masteryType = archetypeInstance.Mastery.Type;
				float baseLevel = archetypeInstance.MasteryData.BaseLevel;
				archetypeInstance.MasteryData.BaseLevel = levelProgressionUpdate.BaseLevel;
				base.GameEntity.CharacterData.UpdateHighestMasteryLevel(archetypeInstance);
				flag = this.LevelUpdate(baseLevel, levelProgressionUpdate.BaseLevel, archetypeInstance.Archetype.GetModifiedDisplayName(archetypeInstance), archetypeInstance);
				MessageManager.CombatQueue.AddToQueue(MessageType.MyCombatIn, GlobalSettings.Values.Progression.GetExperienceGainedChatMessage(archetypeInstance.Mastery.Type, levelProgressionUpdate.HasBonusXp));
				if (archetypeInstance.Mastery.Type == MasteryType.Combat && ClientGameManager.NotificationsManager)
				{
					if (!Options.GameOptions.HideTutorialPopups.Value && !ClientGameManager.NotificationsManager.HasSeenTutorial(TutorialProgress.CombatPositioning))
					{
						ClientGameManager.NotificationsManager.TryShowTutorial(TutorialProgress.CombatPositioning);
					}
					if ((int)levelProgressionUpdate.BaseLevel == 2 && !Options.GameOptions.HideTutorialPopups.Value && !ClientGameManager.NotificationsManager.HasSeenTutorial(TutorialProgress.ActionBar))
					{
						ClientGameManager.NotificationsManager.TryShowTutorial(TutorialProgress.ActionBar);
					}
					if ((int)levelProgressionUpdate.BaseLevel == 6 && archetypeInstance.MasteryData.Specialization == null)
					{
						ClientGameManager.NotificationsManager.TryShowTutorial(TutorialProgress.Specializations);
					}
				}
				float? num = null;
				SpecializedRole specializedRole;
				if (levelProgressionUpdate.SpecializationLevel != null && archetypeInstance.MasteryData.Specialization != null && InternalGameDatabase.Archetypes.TryGetAsType<SpecializedRole>(archetypeInstance.MasteryData.Specialization.Value, out specializedRole))
				{
					num = new float?(archetypeInstance.MasteryData.SpecializationLevel);
					archetypeInstance.MasteryData.SpecializationLevel = levelProgressionUpdate.SpecializationLevel.Value;
					flag = (this.LevelUpdate(num.Value, levelProgressionUpdate.SpecializationLevel.Value, specializedRole.DisplayName, archetypeInstance) || flag);
					if ((num.Value >= 50f && levelProgressionUpdate.SpecializationLevel.Value < 50f) || (levelProgressionUpdate.SpecializationLevel.Value >= 50f && num.Value < 50f))
					{
						LocalPlayer.InvokeSpecializationMaxLevelChanged();
					}
				}
				if (flag && base.GameEntity.CollectionController.Abilities != null)
				{
					foreach (ArchetypeInstance archetypeInstance2 in base.GameEntity.CollectionController.Abilities.Instances)
					{
						if (archetypeInstance2 != null && archetypeInstance2.Ability && archetypeInstance2.Ability.Mastery == archetypeInstance.Mastery)
						{
							float num2 = baseLevel;
							float num3 = archetypeInstance.MasteryData.BaseLevel;
							if (archetypeInstance2.Ability.Specialization != null && num != null)
							{
								num2 = num.Value;
								num3 = archetypeInstance.MasteryData.SpecializationLevel;
							}
							string tierDisplayLevel = archetypeInstance2.Ability.GetTierDisplayLevel(num2);
							string tierDisplayLevel2 = archetypeInstance2.Ability.GetTierDisplayLevel(num3);
							if (tierDisplayLevel != tierDisplayLevel2)
							{
								string arg = (num2 < num3) ? "increased" : "decreased";
								MessageManager.ChatQueue.AddToQueue(MessageType.Skills, ZString.Format<string, string, string, string>("{0} {1} has {2} in power to {0} {3}!", archetypeInstance2.Ability.DisplayName, tierDisplayLevel, arg, tierDisplayLevel2));
							}
						}
					}
				}
			}
			if (flag)
			{
				MessageEventType messageEventType = masteryType.GetMessageEventType();
				float value = (messageEventType == MessageEventType.LevelUpGatheringCrafting) ? 1.2f : 1f;
				if (ClientGameManager.UIManager)
				{
					ClientGameManager.UIManager.PlayClip(GlobalSettings.Values.Audio.LevelUpClip, new float?(value), new float?(GlobalSettings.Values.Audio.LevelUpVolume));
				}
				GlobalSettings.Values.Progression.InitLevelUpVfxForEntity(base.GameEntity, messageEventType);
			}
		}

		// Token: 0x06002EC0 RID: 11968 RVA: 0x001544EC File Offset: 0x001526EC
		private bool LevelUpdate(float previousLevel, float newLevel, string displayName, ArchetypeInstance masteryInstance)
		{
			bool result = false;
			bool flag = false;
			string text = "";
			float num = Mathf.Floor(previousLevel);
			float num2 = Mathf.Floor(newLevel);
			if (num < num2)
			{
				text = "increased";
				flag = true;
				result = true;
			}
			else if (num > num2)
			{
				text = "decreased";
				flag = true;
			}
			if (flag)
			{
				string content = string.Concat(new string[]
				{
					"You have ",
					text,
					" your level in ",
					displayName,
					"! (",
					((int)num2).ToString(),
					")"
				});
				MessageManager.ChatQueue.AddToQueue(MessageType.Skills, content);
				if (masteryInstance.IsMastery)
				{
					Action<ArchetypeInstance> masteryLevelChangedEvent = LocalClientSkillsController.MasteryLevelChangedEvent;
					if (masteryLevelChangedEvent != null)
					{
						masteryLevelChangedEvent(masteryInstance);
					}
					base.GameEntity.Vitals.MasteryLevelChanged(masteryInstance, num, newLevel);
				}
			}
			return result;
		}

		// Token: 0x04002E0B RID: 11787
		public static int AlchemyExecutionCompleteFrame;

		// Token: 0x04002E0C RID: 11788
		private const float kPendingBuffer = 1.1f;

		// Token: 0x04002E0D RID: 11789
		private const float kAutoAttackFailureDelay = 0.1f;

		// Token: 0x04002E0E RID: 11790
		private ArchetypeInstance m_autoAttackInstance;

		// Token: 0x04002E0F RID: 11791
		private SkillsController.PendingExecution m_instantPending;

		// Token: 0x04002E10 RID: 11792
		private Dictionary<UniqueId, SkillsController.PendingExecution> m_pendingExecutions;

		// Token: 0x04002E11 RID: 11793
		private LocalClientSkillsController.PendingPool m_pendingPool;

		// Token: 0x020005C2 RID: 1474
		private class PendingPool
		{
			// Token: 0x06002EC2 RID: 11970 RVA: 0x00060599 File Offset: 0x0005E799
			public PendingPool(SkillsController controller)
			{
				this.m_pool = new Stack<SkillsController.PendingExecution>();
				this.m_controller = controller;
			}

			// Token: 0x06002EC3 RID: 11971 RVA: 0x000605B3 File Offset: 0x0005E7B3
			public SkillsController.PendingExecution GetFromPool()
			{
				if (this.m_pool.Count > 0)
				{
					return this.m_pool.Pop();
				}
				return new SkillsController.PendingExecution(this.m_controller);
			}

			// Token: 0x06002EC4 RID: 11972 RVA: 0x000605DA File Offset: 0x0005E7DA
			public void ReturnToPool(SkillsController.PendingExecution pending)
			{
				pending.Reset();
				if (this.m_pool.Count < 32)
				{
					this.m_pool.Push(pending);
				}
			}

			// Token: 0x04002E12 RID: 11794
			private const int kMaxPoolSize = 32;

			// Token: 0x04002E13 RID: 11795
			private readonly Stack<SkillsController.PendingExecution> m_pool;

			// Token: 0x04002E14 RID: 11796
			private readonly SkillsController m_controller;
		}
	}
}
