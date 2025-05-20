using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game;
using SoL.Game.EffectSystem;
using SoL.Game.NPCs;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Quests;
using SoL.Game.Quests.Objectives;
using SoL.Game.Settings;
using SoL.Game.UI;
using SoL.Networking;
using SoL.Networking.Database;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Managers
{
	// Token: 0x0200052C RID: 1324
	public abstract class QuestManager : MonoBehaviour
	{
		// Token: 0x14000079 RID: 121
		// (add) Token: 0x060027F7 RID: 10231 RVA: 0x0013959C File Offset: 0x0013779C
		// (remove) Token: 0x060027F8 RID: 10232 RVA: 0x001395D4 File Offset: 0x001377D4
		public event Action QuestsUpdated;

		// Token: 0x1400007A RID: 122
		// (add) Token: 0x060027F9 RID: 10233 RVA: 0x0013960C File Offset: 0x0013780C
		// (remove) Token: 0x060027FA RID: 10234 RVA: 0x00139644 File Offset: 0x00137844
		public event Action<ObjectiveIterationCache> QuestUpdated;

		// Token: 0x1400007B RID: 123
		// (add) Token: 0x060027FB RID: 10235 RVA: 0x0013967C File Offset: 0x0013787C
		// (remove) Token: 0x060027FC RID: 10236 RVA: 0x001396B4 File Offset: 0x001378B4
		public event Action BBTasksUpdated;

		// Token: 0x1400007C RID: 124
		// (add) Token: 0x060027FD RID: 10237 RVA: 0x001396EC File Offset: 0x001378EC
		// (remove) Token: 0x060027FE RID: 10238 RVA: 0x00139724 File Offset: 0x00137924
		public event Action<ObjectiveIterationCache> BBTaskUpdated;

		// Token: 0x1400007D RID: 125
		// (add) Token: 0x060027FF RID: 10239 RVA: 0x0013975C File Offset: 0x0013795C
		// (remove) Token: 0x06002800 RID: 10240 RVA: 0x00139794 File Offset: 0x00137994
		public event Action<NpcProfile, int> KnowledgeUpdated;

		// Token: 0x06002801 RID: 10241 RVA: 0x001397CC File Offset: 0x001379CC
		protected virtual void Start()
		{
			foreach (object obj in Enum.GetValues(typeof(BBTaskType)))
			{
				BBTaskType key = (BBTaskType)obj;
				QuestManager.TasksAvailable.Add(key, false);
			}
			foreach (BBTask bbtask in InternalGameDatabase.BBTasks.GetAllItems())
			{
				QuestManager.TasksAvailable[bbtask.Type] = (QuestManager.TasksAvailable[bbtask.Type] || bbtask.Enabled);
			}
		}

		// Token: 0x06002802 RID: 10242 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void OnDestroy()
		{
		}

		// Token: 0x06002803 RID: 10243 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void ManuallyInvokeLocalPlayerInitialized()
		{
		}

		// Token: 0x06002804 RID: 10244 RVA: 0x0005BFF3 File Offset: 0x0005A1F3
		public virtual bool TryGetQuestByTag(string tag, out Quest quest)
		{
			quest = null;
			return false;
		}

		// Token: 0x06002805 RID: 10245 RVA: 0x0005BFF3 File Offset: 0x0005A1F3
		public virtual bool TryGetDialogueState(UniqueId sourceId, out List<InkEntry> state)
		{
			state = null;
			return false;
		}

		// Token: 0x06002806 RID: 10246 RVA: 0x0013989C File Offset: 0x00137A9C
		public bool TryGetNpcProfileByKnowledgeLabel(string label, out NpcProfile npcProfile)
		{
			npcProfile = null;
			foreach (BaseArchetype baseArchetype in InternalGameDatabase.Archetypes.GetAllItems())
			{
				NpcProfile npcProfile2 = baseArchetype as NpcProfile;
				if (npcProfile2 != null && npcProfile2.KnowledgeLabels != null)
				{
					string[] knowledgeLabels = npcProfile2.KnowledgeLabels;
					for (int i = 0; i < knowledgeLabels.Length; i++)
					{
						if (knowledgeLabels[i] == label)
						{
							if (!(npcProfile == null))
							{
								Debug.LogError("Duplicate knowledge labels found on profiles: " + npcProfile2.name + " and " + npcProfile.name);
								npcProfile = null;
								return false;
							}
							npcProfile = npcProfile2;
						}
					}
				}
			}
			return npcProfile != null;
		}

		// Token: 0x06002807 RID: 10247 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool HasDialogueState(UniqueId sourceId)
		{
			return false;
		}

		// Token: 0x06002808 RID: 10248 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void Reset()
		{
		}

		// Token: 0x06002809 RID: 10249 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void Progress(ObjectiveIterationCache cache, GameEntity sourceEntity = null, bool failQuietly = false)
		{
		}

		// Token: 0x0600280A RID: 10250 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void RequestDelayedRefresh(IDelayedRefresh delayedRefresh)
		{
		}

		// Token: 0x0600280B RID: 10251 RVA: 0x0013995C File Offset: 0x00137B5C
		public virtual void NotifyQuestsUpdated(ObjectiveIterationCache cache, GameEntity sourceEntity = null)
		{
			Quest quest;
			QuestProgressionData questProgressionData;
			if (InternalGameDatabase.Quests.TryGetItem(cache.QuestId, out quest) && quest.TryGetProgress(sourceEntity, out questProgressionData))
			{
				if (cache.ObjectiveHashes == null && cache.StartQuestIfNotPresent && questProgressionData.CurrentNodeId == quest.Start.Id)
				{
					quest.Start.OnEntry(cache, sourceEntity);
				}
				else
				{
					foreach (int hash in cache.ObjectiveHashes)
					{
						QuestStep questStep;
						QuestObjective questObjective;
						if (quest.TryGetObjectiveUsage(hash, out questStep, out questObjective) && questProgressionData.CurrentNodeId == questStep.Id)
						{
							questStep.OnEntry(cache, sourceEntity);
							break;
						}
					}
				}
			}
			Action questsUpdated = this.QuestsUpdated;
			if (questsUpdated != null)
			{
				questsUpdated();
			}
			Action<ObjectiveIterationCache> questUpdated = this.QuestUpdated;
			if (questUpdated != null)
			{
				questUpdated(cache);
			}
			UIManager.InvokeTriggerControlPanelUsageHighlight(WindowToggler.WindowType.Log);
		}

		// Token: 0x0600280C RID: 10252 RVA: 0x00139A34 File Offset: 0x00137C34
		public virtual void ResetQuests(GameEntity entity)
		{
			PlayerProgressionData progression = entity.CollectionController.Record.Progression;
			if (((progression != null) ? progression.Quests : null) != null)
			{
				PlayerProgressionData progression2 = entity.CollectionController.Record.Progression;
				foreach (KeyValuePair<UniqueId, QuestProgressionData> keyValuePair in ((progression2 != null) ? progression2.Quests : null))
				{
					Quest quest;
					QuestStep questStep;
					if (InternalGameDatabase.Quests.TryGetItem(keyValuePair.Key, out quest) && quest.TryGetStep(keyValuePair.Value.CurrentNodeId, out questStep))
					{
						questStep.OnExit(new ObjectiveIterationCache
						{
							QuestId = keyValuePair.Key
						}, entity);
					}
				}
				if (entity.CollectionController.Record.Progression.NpcKnowledge == null && entity.CollectionController.Record.Progression.BBTasks == null)
				{
					entity.CollectionController.Record.Progression = null;
					if (GameManager.IsServer)
					{
						entity.CollectionController.Record.UpdateQuests(ExternalGameDatabase.Database);
					}
				}
				else
				{
					entity.CollectionController.Record.Progression.Quests = null;
					if (GameManager.IsServer)
					{
						entity.CollectionController.Record.UpdateQuests(ExternalGameDatabase.Database);
					}
				}
			}
			Action questsUpdated = this.QuestsUpdated;
			if (questsUpdated == null)
			{
				return;
			}
			questsUpdated();
		}

		// Token: 0x0600280D RID: 10253 RVA: 0x00139BA4 File Offset: 0x00137DA4
		public virtual void ResetQuest(GameEntity entity, Quest quest)
		{
			if (quest != null)
			{
				PlayerProgressionData progression = entity.CollectionController.Record.Progression;
				if (((progression != null) ? progression.Quests : null) != null)
				{
					QuestProgressionData questProgressionData;
					QuestStep questStep;
					if (entity.CollectionController.Record.Progression.Quests.TryGetValue(quest.Id, out questProgressionData) && quest.TryGetStep(questProgressionData.CurrentNodeId, out questStep))
					{
						questStep.OnExit(new ObjectiveIterationCache
						{
							QuestId = quest.Id
						}, entity);
					}
					entity.CollectionController.Record.Progression.Quests.Remove(quest.Id);
					if (GameManager.IsServer)
					{
						entity.CollectionController.Record.UpdateQuests(ExternalGameDatabase.Database);
					}
				}
			}
			Action questsUpdated = this.QuestsUpdated;
			if (questsUpdated == null)
			{
				return;
			}
			questsUpdated();
		}

		// Token: 0x0600280E RID: 10254 RVA: 0x00139C80 File Offset: 0x00137E80
		public virtual void MuteQuest(GameEntity entity, UniqueId questId, bool mute)
		{
			Quest quest;
			if (entity && entity.CollectionController != null && entity.CollectionController.Record != null && InternalGameDatabase.Quests.TryGetItem(questId, out quest))
			{
				quest.Mute(mute, entity);
				if (GameManager.IsServer)
				{
					entity.CollectionController.Record.UpdateQuests(ExternalGameDatabase.Database);
					return;
				}
				entity.NetworkEntity.PlayerRpcHandler.MuteQuest(questId, mute);
			}
		}

		// Token: 0x0600280F RID: 10255 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void DrawBBTask(BBTaskDrawCache cache, GameEntity entity = null)
		{
		}

		// Token: 0x06002810 RID: 10256 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void ProgressTask(ObjectiveIterationCache cache, GameEntity sourceEntity = null, bool failQuietly = false)
		{
		}

		// Token: 0x06002811 RID: 10257 RVA: 0x0005BFF9 File Offset: 0x0005A1F9
		public virtual void NotifyTaskUpdated(ObjectiveIterationCache cache, GameEntity sourceEntity = null)
		{
			Action bbtasksUpdated = this.BBTasksUpdated;
			if (bbtasksUpdated != null)
			{
				bbtasksUpdated();
			}
			Action<ObjectiveIterationCache> bbtaskUpdated = this.BBTaskUpdated;
			if (bbtaskUpdated != null)
			{
				bbtaskUpdated(cache);
			}
			UIManager.InvokeTriggerControlPanelUsageHighlight(WindowToggler.WindowType.Log);
		}

		// Token: 0x06002812 RID: 10258 RVA: 0x00139CF4 File Offset: 0x00137EF4
		public virtual void ResetTasks(GameEntity entity)
		{
			PlayerProgressionData progression = entity.CollectionController.Record.Progression;
			if (((progression != null) ? progression.BBTasks : null) != null)
			{
				PlayerProgressionData progression2 = entity.CollectionController.Record.Progression;
				foreach (KeyValuePair<UniqueId, BBTaskProgressionData> keyValuePair in ((progression2 != null) ? progression2.BBTasks : null))
				{
					BBTask bbtask;
					if (InternalGameDatabase.BBTasks.TryGetItem(keyValuePair.Key, out bbtask))
					{
						bbtask.OnExit(new ObjectiveIterationCache
						{
							QuestId = keyValuePair.Key
						}, entity);
					}
				}
				if (entity.CollectionController.Record.Progression.Quests == null && entity.CollectionController.Record.Progression.NpcKnowledge == null)
				{
					entity.CollectionController.Record.Progression = null;
					if (GameManager.IsServer)
					{
						entity.CollectionController.Record.UpdateQuests(ExternalGameDatabase.Database);
					}
				}
				else
				{
					entity.CollectionController.Record.Progression.BBTasks = null;
					if (GameManager.IsServer)
					{
						entity.CollectionController.Record.UpdateQuests(ExternalGameDatabase.Database);
					}
				}
			}
			Action bbtasksUpdated = this.BBTasksUpdated;
			if (bbtasksUpdated == null)
			{
				return;
			}
			bbtasksUpdated();
		}

		// Token: 0x06002813 RID: 10259 RVA: 0x00139E4C File Offset: 0x0013804C
		public virtual void DropTask(UniqueId taskId, GameEntity entity = null)
		{
			if (this.DropBBTask(taskId, entity) && GameManager.IsServer)
			{
				entity.CollectionController.Record.UpdateQuests(ExternalGameDatabase.Database);
			}
			Action bbtasksUpdated = this.BBTasksUpdated;
			if (bbtasksUpdated != null)
			{
				bbtasksUpdated();
			}
			Action<ObjectiveIterationCache> bbtaskUpdated = this.BBTaskUpdated;
			if (bbtaskUpdated == null)
			{
				return;
			}
			bbtaskUpdated(new ObjectiveIterationCache
			{
				QuestId = taskId
			});
		}

		// Token: 0x06002814 RID: 10260 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void Learn(NpcLearningCache cache, GameEntity sourceEntity = null)
		{
		}

		// Token: 0x06002815 RID: 10261 RVA: 0x0005C025 File Offset: 0x0005A225
		public virtual void NotifyLearned(NpcLearningCache cache, GameEntity sourceEntity = null)
		{
			Action<NpcProfile, int> knowledgeUpdated = this.KnowledgeUpdated;
			if (knowledgeUpdated == null)
			{
				return;
			}
			knowledgeUpdated(cache.NpcProfile, cache.KnowledgeIndex);
		}

		// Token: 0x06002816 RID: 10262 RVA: 0x00139EB4 File Offset: 0x001380B4
		public bool HasReissuableReward(UniqueId questId, GameEntity sourceEntity)
		{
			Quest quest;
			QuestStep questStep;
			RewardChoiceObjective rewardChoiceObjective;
			return InternalGameDatabase.Quests.TryGetItem(questId, out quest) && quest.TryGetMostRecentReward(sourceEntity, out questStep, out rewardChoiceObjective, null) && rewardChoiceObjective.Reward.HasReissuableRewardsForEntity(sourceEntity);
		}

		// Token: 0x06002817 RID: 10263 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void ReissueReward(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
		}

		// Token: 0x06002818 RID: 10264 RVA: 0x00139EEC File Offset: 0x001380EC
		public bool DropQuestsAndTasksForMastery(UniqueId archetypeId, GameEntity entity)
		{
			bool result = false;
			if (entity && entity.CollectionController != null && entity.CollectionController.Record != null && entity.CollectionController.Record.Progression != null)
			{
				if (entity.CollectionController.Record.Progression.Quests != null)
				{
					List<UniqueId> fromPool = StaticListPool<UniqueId>.GetFromPool();
					foreach (KeyValuePair<UniqueId, QuestProgressionData> keyValuePair in entity.CollectionController.Record.Progression.Quests)
					{
						Quest quest;
						QuestStep questStep;
						if (InternalGameDatabase.Quests.TryGetItem(keyValuePair.Key, out quest) && quest.TryGetStep(keyValuePair.Value.CurrentNodeId, out questStep) && quest.AssociatedMastery != null && quest.AssociatedMastery.Id == archetypeId)
						{
							fromPool.Add(quest.Id);
							questStep.OnExit(new ObjectiveIterationCache
							{
								QuestId = quest.Id
							}, entity);
						}
					}
					foreach (UniqueId key in fromPool)
					{
						entity.CollectionController.Record.Progression.Quests.Remove(key);
					}
					if (fromPool.Count > 0)
					{
						result = true;
						Action questsUpdated = this.QuestsUpdated;
						if (questsUpdated != null)
						{
							questsUpdated();
						}
					}
					StaticListPool<UniqueId>.ReturnToPool(fromPool);
				}
				if (entity.CollectionController.Record.Progression.BBTasks != null)
				{
					List<UniqueId> fromPool2 = StaticListPool<UniqueId>.GetFromPool();
					foreach (KeyValuePair<UniqueId, BBTaskProgressionData> keyValuePair2 in entity.CollectionController.Record.Progression.BBTasks)
					{
						BBTask bbtask;
						if (InternalGameDatabase.BBTasks.TryGetItem(keyValuePair2.Key, out bbtask) && bbtask.AssociatedMastery != null && bbtask.AssociatedMastery.Id == archetypeId)
						{
							fromPool2.Add(bbtask.Id);
							bbtask.OnExit(new ObjectiveIterationCache
							{
								QuestId = bbtask.Id
							}, entity);
							List<BBTask> localTaskDiscard = entity.CollectionController.LocalTaskDiscard;
							if (localTaskDiscard != null)
							{
								localTaskDiscard.Add(bbtask);
							}
						}
					}
					foreach (UniqueId key2 in fromPool2)
					{
						entity.CollectionController.Record.Progression.BBTasks.Remove(key2);
					}
					if (fromPool2.Count > 0)
					{
						result = true;
						Action bbtasksUpdated = this.BBTasksUpdated;
						if (bbtasksUpdated != null)
						{
							bbtasksUpdated();
						}
					}
					StaticListPool<UniqueId>.ReturnToPool(fromPool2);
				}
			}
			return result;
		}

		// Token: 0x06002819 RID: 10265 RVA: 0x0013A20C File Offset: 0x0013840C
		protected bool StartQuest(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			Quest quest;
			if (InternalGameDatabase.Quests.TryGetItem(cache.QuestId, out quest) && quest.CanStartQuest(sourceEntity) && cache.StartQuestIfNotPresent)
			{
				CharacterRecord record = sourceEntity.CollectionController.Record;
				if (record.Progression == null)
				{
					record.Progression = new PlayerProgressionData();
				}
				if (record.Progression.Quests == null)
				{
					record.Progression.Quests = new Dictionary<UniqueId, QuestProgressionData>();
				}
				QuestProgressionData questProgressionData = new QuestProgressionData();
				questProgressionData.CurrentNodeId = quest.Start.Id;
				record.Progression.Quests.Add(quest.Id, questProgressionData);
				return true;
			}
			return false;
		}

		// Token: 0x0600281A RID: 10266 RVA: 0x0013A2AC File Offset: 0x001384AC
		protected bool UpdateQuest(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			if (cache.ObjectiveHashes == null)
			{
				throw new InvalidOperationException("cache.ObjectiveHashes should never be null when calling QuestManager.UpdateQuest");
			}
			bool result = false;
			Quest quest;
			if (InternalGameDatabase.Quests.TryGetItem(cache.QuestId, out quest) && ((quest.CanStartQuest(sourceEntity) && cache.StartQuestIfNotPresent) || quest.AreObjectivesActive(cache.ObjectiveHashes, sourceEntity)))
			{
				foreach (int num in cache.ObjectiveHashes)
				{
					QuestStep questStep;
					QuestObjective questObjective;
					if (num != 0 && quest.TryGetObjectiveUsage(num, out questStep, out questObjective))
					{
						QuestProgressionData questProgressionData = null;
						CharacterRecord record = sourceEntity.CollectionController.Record;
						PlayerProgressionData progression = record.Progression;
						bool? flag;
						if (progression == null)
						{
							flag = null;
						}
						else
						{
							Dictionary<UniqueId, QuestProgressionData> quests = progression.Quests;
							flag = ((quests != null) ? new bool?(quests.TryGetValue(quest.Id, out questProgressionData)) : null);
						}
						bool? flag2 = flag;
						bool valueOrDefault = flag2.GetValueOrDefault();
						if (!valueOrDefault && cache.StartQuestIfNotPresent && (questStep.AllowStartHere || questStep.Id == quest.Start.Id))
						{
							if (record.Progression == null)
							{
								record.Progression = new PlayerProgressionData();
							}
							if (record.Progression.Quests == null)
							{
								record.Progression.Quests = new Dictionary<UniqueId, QuestProgressionData>();
							}
							questProgressionData = new QuestProgressionData
							{
								CurrentNodeId = questStep.Id,
								Objectives = new List<ObjectiveProgressionData>
								{
									new ObjectiveProgressionData
									{
										ObjectiveId = questObjective.Id
									}
								}
							};
							record.Progression.Quests.Add(quest.Id, questProgressionData);
							result = true;
						}
						else if (!valueOrDefault)
						{
							if (GameManager.IsServer)
							{
								sourceEntity.NetworkEntity.PlayerRpcHandler.NotifyObjectiveIteration(OpCodes.Error, "You are not on that quest!", cache);
								break;
							}
							break;
						}
						ObjectiveProgressionData objectiveProgressionData;
						if (questProgressionData.TryGetObjective(questObjective.Id, out objectiveProgressionData))
						{
							if (objectiveProgressionData.IterationsCompleted < questObjective.IterationsRequired)
							{
								objectiveProgressionData.IterationsCompleted += Math.Min(questObjective.IterationsRequired - objectiveProgressionData.IterationsCompleted, Math.Max((int)cache.IterationsRequested, 1));
								result = true;
							}
						}
						else
						{
							if (questProgressionData.Objectives == null)
							{
								questProgressionData.Objectives = new List<ObjectiveProgressionData>();
							}
							objectiveProgressionData = new ObjectiveProgressionData
							{
								ObjectiveId = questObjective.Id,
								IterationsCompleted = Math.Min(questObjective.IterationsRequired, Math.Max((int)cache.IterationsRequested, 1))
							};
							questProgressionData.Objectives.Add(objectiveProgressionData);
							result = true;
						}
						foreach (QuestObjective questObjective2 in questStep.Objectives)
						{
							if (questObjective2.Passive && questObjective2.ActiveParent.Id == questObjective.Id)
							{
								if (questProgressionData.TryGetObjective(questObjective2.Id, out objectiveProgressionData))
								{
									if (objectiveProgressionData.IterationsCompleted < questObjective2.IterationsRequired)
									{
										objectiveProgressionData.IterationsCompleted++;
										result = true;
									}
								}
								else
								{
									if (questProgressionData.Objectives == null)
									{
										questProgressionData.Objectives = new List<ObjectiveProgressionData>();
									}
									objectiveProgressionData = new ObjectiveProgressionData
									{
										ObjectiveId = questObjective2.Id,
										IterationsCompleted = 1
									};
									questProgressionData.Objectives.Add(objectiveProgressionData);
									result = true;
								}
							}
						}
						bool flag3 = true;
						foreach (QuestObjective questObjective3 in questStep.Objectives)
						{
							ObjectiveProgressionData objectiveProgressionData2;
							if (questProgressionData.TryGetObjective(questObjective3.Id, out objectiveProgressionData2))
							{
								if (objectiveProgressionData2.IterationsCompleted < questObjective3.IterationsRequired)
								{
									flag3 = false;
								}
							}
							else
							{
								flag3 = false;
							}
						}
						if (flag3)
						{
							questProgressionData.CurrentNodeId = questStep.Id;
							questProgressionData.Objectives = null;
							result = true;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600281B RID: 10267 RVA: 0x0013A6A4 File Offset: 0x001388A4
		protected bool StartBBTask(ObjectiveIterationCache cache, GameEntity entity)
		{
			BBTask bbtask;
			if (InternalGameDatabase.BBTasks.TryGetItem(cache.QuestId, out bbtask) && bbtask.CanStart(entity))
			{
				CharacterRecord record = entity.CollectionController.Record;
				if (record.Progression == null)
				{
					record.Progression = new PlayerProgressionData();
				}
				if (record.Progression.BBTasks == null)
				{
					record.Progression.BBTasks = new Dictionary<UniqueId, BBTaskProgressionData>();
				}
				BBTaskProgressionData value = new BBTaskProgressionData();
				record.Progression.BBTasks.Add(bbtask.Id, value);
				bbtask.OnEntry(cache, entity);
				return true;
			}
			return false;
		}

		// Token: 0x0600281C RID: 10268 RVA: 0x0013A734 File Offset: 0x00138934
		protected bool UpdateBBTask(ObjectiveIterationCache cache, GameEntity entity, out bool allObjectivesComplete)
		{
			if (cache.ObjectiveHashes == null)
			{
				throw new InvalidOperationException("cache.ObjectiveHashes should never be null when calling QuestManager.UpdateBBTask");
			}
			allObjectivesComplete = false;
			bool result = false;
			BBTask bbtask;
			if (InternalGameDatabase.BBTasks.TryGetItem(cache.QuestId, out bbtask))
			{
				foreach (int num in cache.ObjectiveHashes)
				{
					QuestObjective questObjective;
					if (num != 0 && bbtask.TryGetObjective(num, out questObjective))
					{
						BBTaskProgressionData bbtaskProgressionData = null;
						CharacterRecord record = entity.CollectionController.Record;
						if (record.Progression == null || record.Progression.BBTasks == null || !record.Progression.BBTasks.TryGetValue(cache.QuestId, out bbtaskProgressionData))
						{
							if (GameManager.IsServer)
							{
								entity.NetworkEntity.PlayerRpcHandler.NotifyBBTaskIterated(OpCodes.Error, "You do not have that task!", cache);
								break;
							}
							break;
						}
						else
						{
							ObjectiveProgressionData objectiveProgressionData;
							if (bbtaskProgressionData.TryGetObjective(questObjective.Id, out objectiveProgressionData))
							{
								if (objectiveProgressionData.IterationsCompleted < questObjective.IterationsRequired)
								{
									objectiveProgressionData.IterationsCompleted += Math.Min(questObjective.IterationsRequired - objectiveProgressionData.IterationsCompleted, Math.Max((int)cache.IterationsRequested, 1));
									result = true;
								}
							}
							else
							{
								if (bbtaskProgressionData.Objectives == null)
								{
									bbtaskProgressionData.Objectives = new List<ObjectiveProgressionData>();
								}
								objectiveProgressionData = new ObjectiveProgressionData
								{
									ObjectiveId = questObjective.Id,
									IterationsCompleted = Math.Min(questObjective.IterationsRequired, Math.Max((int)cache.IterationsRequested, 1))
								};
								bbtaskProgressionData.Objectives.Add(objectiveProgressionData);
								result = true;
							}
							foreach (QuestObjective questObjective2 in bbtask.Objectives)
							{
								if (questObjective2.Passive && questObjective2.ActiveParent.Id == questObjective.Id)
								{
									if (bbtaskProgressionData.TryGetObjective(questObjective2.Id, out objectiveProgressionData))
									{
										if (objectiveProgressionData.IterationsCompleted < questObjective2.IterationsRequired)
										{
											objectiveProgressionData.IterationsCompleted++;
											result = true;
										}
									}
									else
									{
										if (bbtaskProgressionData.Objectives == null)
										{
											bbtaskProgressionData.Objectives = new List<ObjectiveProgressionData>();
										}
										objectiveProgressionData = new ObjectiveProgressionData
										{
											ObjectiveId = questObjective2.Id,
											IterationsCompleted = 1
										};
										bbtaskProgressionData.Objectives.Add(objectiveProgressionData);
										result = true;
									}
								}
							}
							allObjectivesComplete = true;
							foreach (QuestObjective questObjective3 in bbtask.Objectives)
							{
								ObjectiveProgressionData objectiveProgressionData2;
								if (bbtaskProgressionData.TryGetObjective(questObjective3.Id, out objectiveProgressionData2))
								{
									if (objectiveProgressionData2.IterationsCompleted < questObjective3.IterationsRequired)
									{
										allObjectivesComplete = false;
									}
								}
								else
								{
									allObjectivesComplete = false;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600281D RID: 10269 RVA: 0x0013AA10 File Offset: 0x00138C10
		protected bool TurnInBBTask(ObjectiveIterationCache cache, GameEntity entity)
		{
			bool result = false;
			BBTask bbtask;
			if (InternalGameDatabase.BBTasks.TryGetItem(cache.QuestId, out bbtask))
			{
				if (!bbtask.IsReadyForTurnIn(entity))
				{
					if (GameManager.IsServer)
					{
						entity.NetworkEntity.PlayerRpcHandler.NotifyBBTaskIterated(OpCodes.Error, "Task not ready for turn-in!", cache);
					}
					return result;
				}
				if (entity && entity.CollectionController != null && entity.CollectionController.Record != null && entity.CollectionController.Record.Progression != null && entity.CollectionController.Record.Progression.BBTasks != null)
				{
					entity.CollectionController.Record.Progression.BBTasks.Remove(bbtask.Id);
					switch (bbtask.Type)
					{
					case BBTaskType.Adventuring:
						entity.CollectionController.Record.Progression.BBTasks_AdventuringCompletionCount++;
						break;
					case BBTaskType.Crafting:
						entity.CollectionController.Record.Progression.BBTasks_CraftingCompletionCount++;
						break;
					case BBTaskType.Gathering:
						entity.CollectionController.Record.Progression.BBTasks_GatheringCompletionCount++;
						break;
					}
					result = true;
					bbtask.OnExit(cache, entity);
					if (GameManager.IsServer)
					{
						int num = 1;
						BaseRole baseRole = null;
						BBTaskType type = bbtask.Type;
						if (type != BBTaskType.Adventuring)
						{
							if (type - BBTaskType.Crafting <= 1)
							{
								ArchetypeInstance archetypeInstance;
								if (bbtask.AssociatedMastery && entity.CollectionController.Masteries != null && entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(bbtask.AssociatedMastery.Id, out archetypeInstance) && archetypeInstance.MasteryData != null)
								{
									num = Mathf.FloorToInt(archetypeInstance.MasteryData.BaseLevel);
									baseRole = (bbtask.AssociatedMastery as BaseRole);
								}
							}
						}
						else
						{
							num = entity.CharacterData.AdventuringLevel;
						}
						int taskLevel = (num <= bbtask.LevelRange.y) ? num : bbtask.LevelRange.y;
						type = bbtask.Type;
						if (type != BBTaskType.Adventuring)
						{
							if (type - BBTaskType.Crafting <= 1)
							{
								ProgressionCalculator.OnGatheringCraftingTaskCompleted(entity, taskLevel, num, baseRole);
							}
						}
						else
						{
							ProgressionCalculator.OnAdventuringTaskCompleted(entity, taskLevel, num);
						}
						ScriptableEffectData scriptableEffectData;
						if (entity.EffectController && entity.VitalsReplicator && entity.VitalsReplicator.CurrentHealthState.Value == HealthState.Alive && GlobalSettings.Values && GlobalSettings.Values.Tasks != null && GlobalSettings.Values.Tasks.TryGetTaskEffect(taskLevel, out scriptableEffectData))
						{
							entity.EffectController.ApplyEffect(entity, scriptableEffectData.Id, scriptableEffectData.Effect, 50f, 50f, false, true);
						}
						List<BBTask> localTaskDiscard = entity.CollectionController.LocalTaskDiscard;
						if (localTaskDiscard != null)
						{
							localTaskDiscard.Add(bbtask);
						}
					}
					else if (ClientGameManager.UIManager)
					{
						ClientGameManager.UIManager.PlayTaskCompleteAudio();
					}
				}
			}
			return result;
		}

		// Token: 0x0600281E RID: 10270 RVA: 0x0013ACEC File Offset: 0x00138EEC
		protected bool DropBBTask(UniqueId taskId, GameEntity entity)
		{
			bool result = false;
			BBTask bbtask;
			if (InternalGameDatabase.BBTasks.TryGetItem(taskId, out bbtask) && entity && entity.CollectionController != null && entity.CollectionController.Record != null && entity.CollectionController.Record.Progression != null && entity.CollectionController.Record.Progression.BBTasks != null)
			{
				entity.CollectionController.Record.Progression.BBTasks.Remove(taskId);
				result = true;
				bbtask.OnExit(new ObjectiveIterationCache
				{
					QuestId = taskId
				}, entity);
				List<BBTask> localTaskDiscard = entity.CollectionController.LocalTaskDiscard;
				if (localTaskDiscard != null)
				{
					localTaskDiscard.Add(bbtask);
				}
			}
			return result;
		}

		// Token: 0x0600281F RID: 10271 RVA: 0x0013ADA8 File Offset: 0x00138FA8
		public void UpdateNpcKnowledge(NpcLearningCache cache, GameEntity sourceEntity)
		{
			CharacterRecord record = sourceEntity.CollectionController.Record;
			if (record.Progression == null)
			{
				record.Progression = new PlayerProgressionData();
			}
			if (record.Progression.NpcKnowledge == null)
			{
				record.Progression.NpcKnowledge = new Dictionary<UniqueId, BitArray>();
			}
			Dictionary<UniqueId, BitArray> npcKnowledge = record.Progression.NpcKnowledge;
			if (!npcKnowledge.ContainsKey(cache.NpcProfile.Id))
			{
				npcKnowledge.Add(cache.NpcProfile.Id, new BitArray(cache.NpcProfile.KnowledgeLabels.Length));
			}
			if (npcKnowledge[cache.NpcProfile.Id].Length != cache.NpcProfile.KnowledgeLabels.Length)
			{
				if (cache.NpcProfile.KnowledgeLabels.Length < npcKnowledge[cache.NpcProfile.Id].Length)
				{
					Debug.LogError("Knowledge list shrinking, knowledge will be lost...");
				}
				npcKnowledge[cache.NpcProfile.Id].Length = cache.NpcProfile.KnowledgeLabels.Length;
			}
			if (cache.KnowledgeIndex < npcKnowledge[cache.NpcProfile.Id].Count)
			{
				npcKnowledge[cache.NpcProfile.Id][cache.KnowledgeIndex] = true;
			}
		}

		// Token: 0x0400296F RID: 10607
		public static readonly Dictionary<BBTaskType, bool> TasksAvailable = new Dictionary<BBTaskType, bool>();
	}
}
