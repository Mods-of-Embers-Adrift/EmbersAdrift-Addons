using System;
using System.Collections.Generic;
using System.Text;
using SoL.Game;
using SoL.Game.Discovery;
using SoL.Game.Messages;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Quests;
using SoL.Game.Quests.Objectives;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Managers
{
	// Token: 0x020004EE RID: 1262
	public class ClientQuestManager : QuestManager
	{
		// Token: 0x14000042 RID: 66
		// (add) Token: 0x06002359 RID: 9049 RVA: 0x001293D0 File Offset: 0x001275D0
		// (remove) Token: 0x0600235A RID: 9050 RVA: 0x00129408 File Offset: 0x00127608
		public event Action InkLookupUpdated;

		// Token: 0x0600235B RID: 9051 RVA: 0x00059739 File Offset: 0x00057939
		protected override void Start()
		{
			base.Start();
			LocalPlayer.LocalPlayerDestroyed += this.OnLocalPlayerDestroyed;
			this.InitInternalDbLookups();
			this.RegisterCommands();
		}

		// Token: 0x0600235C RID: 9052 RVA: 0x0005975E File Offset: 0x0005795E
		protected override void OnDestroy()
		{
			base.OnDestroy();
			LocalPlayer.LocalPlayerDestroyed -= this.OnLocalPlayerDestroyed;
		}

		// Token: 0x0600235D RID: 9053 RVA: 0x00129440 File Offset: 0x00127640
		private void Update()
		{
			if (this.m_delayedRefreshDict.Count > 0)
			{
				this.m_delayedRefreshesToRemove.Clear();
				int frameCount = Time.frameCount;
				foreach (KeyValuePair<UniqueId, ClientQuestManager.DelayedRefreshData> keyValuePair in this.m_delayedRefreshDict)
				{
					if (keyValuePair.Value.ToRefresh == null)
					{
						this.m_delayedRefreshesToRemove.Add(keyValuePair.Key);
					}
					else if (keyValuePair.Value.FrameToRefresh >= frameCount)
					{
						keyValuePair.Value.ToRefresh.ExecuteRefresh();
						this.m_delayedRefreshesToRemove.Add(keyValuePair.Key);
					}
				}
				if (this.m_delayedRefreshesToRemove.Count > 0)
				{
					for (int i = 0; i < this.m_delayedRefreshesToRemove.Count; i++)
					{
						this.m_delayedRefreshDict.Remove(this.m_delayedRefreshesToRemove[i]);
					}
				}
			}
		}

		// Token: 0x0600235E RID: 9054 RVA: 0x00059777 File Offset: 0x00057977
		public override void ManuallyInvokeLocalPlayerInitialized()
		{
			base.ManuallyInvokeLocalPlayerInitialized();
			this.RefreshQuestLookups();
			this.OnLocalPlayerInitialized();
		}

		// Token: 0x0600235F RID: 9055 RVA: 0x00129540 File Offset: 0x00127740
		public override void RequestDelayedRefresh(IDelayedRefresh delayedRefresh)
		{
			if (delayedRefresh != null)
			{
				ClientQuestManager.DelayedRefreshData value;
				if (this.m_delayedRefreshDict.TryGetValue(delayedRefresh.Id, out value))
				{
					value.FrameToRefresh = Time.frameCount + 1;
					this.m_delayedRefreshDict[delayedRefresh.Id] = value;
					return;
				}
				this.m_delayedRefreshDict.Add(delayedRefresh.Id, new ClientQuestManager.DelayedRefreshData
				{
					FrameToRefresh = Time.frameCount + 1,
					ToRefresh = delayedRefresh
				});
			}
		}

		// Token: 0x06002360 RID: 9056 RVA: 0x001295B8 File Offset: 0x001277B8
		private void InitInternalDbLookups()
		{
			foreach (Quest quest in InternalGameDatabase.Quests.GetAllItems())
			{
				if (!string.IsNullOrEmpty(quest.DialogueTag))
				{
					this.m_questLookupByTag.Add(quest.DialogueTag, quest);
				}
			}
		}

		// Token: 0x06002361 RID: 9057 RVA: 0x00129624 File Offset: 0x00127824
		private void RefreshQuestLookups()
		{
			foreach (KeyValuePair<UniqueId, List<InkEntry>> keyValuePair in this.m_inkLookup)
			{
				StaticListPool<InkEntry>.ReturnToPool(keyValuePair.Value);
			}
			this.m_inkLookup.Clear();
			if (!LocalPlayer.GameEntity || LocalPlayer.GameEntity.CollectionController == null || LocalPlayer.GameEntity.CollectionController.Record == null || LocalPlayer.GameEntity.CollectionController.Record.Progression == null)
			{
				return;
			}
			Dictionary<UniqueId, QuestProgressionData> quests = LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests;
			if (quests != null)
			{
				foreach (KeyValuePair<UniqueId, QuestProgressionData> keyValuePair2 in quests)
				{
					Quest quest;
					QuestStep questStep;
					if (InternalGameDatabase.Quests.TryGetItem(keyValuePair2.Key, out quest) && quest.Enabled && quest.TryGetStep(keyValuePair2.Value.CurrentNodeId, out questStep))
					{
						foreach (InkEntry inkEntry in questStep.InkEntries)
						{
							if (!this.m_inkLookup.ContainsKey(inkEntry.Source.Id))
							{
								this.m_inkLookup.Add(inkEntry.Source.Id, StaticListPool<InkEntry>.GetFromPool());
							}
							this.m_inkLookup[inkEntry.Source.Id].Add(inkEntry);
						}
					}
				}
			}
			Action inkLookupUpdated = this.InkLookupUpdated;
			if (inkLookupUpdated == null)
			{
				return;
			}
			inkLookupUpdated();
		}

		// Token: 0x06002362 RID: 9058 RVA: 0x0005978B File Offset: 0x0005798B
		public override bool TryGetQuestByTag(string questTag, out Quest quest)
		{
			return this.m_questLookupByTag.TryGetValue(questTag, out quest);
		}

		// Token: 0x06002363 RID: 9059 RVA: 0x0005979A File Offset: 0x0005799A
		public override bool TryGetDialogueState(UniqueId sourceId, out List<InkEntry> state)
		{
			return this.m_inkLookup.TryGetValue(sourceId, out state);
		}

		// Token: 0x06002364 RID: 9060 RVA: 0x00129804 File Offset: 0x00127A04
		public override bool HasDialogueState(UniqueId sourceId)
		{
			List<InkEntry> list;
			return this.m_inkLookup.TryGetValue(sourceId, out list) && list != null && list.Count > 0;
		}

		// Token: 0x06002365 RID: 9061 RVA: 0x000597A9 File Offset: 0x000579A9
		public override void Reset()
		{
			base.Reset();
			this.m_inkLookup.Clear();
		}

		// Token: 0x06002366 RID: 9062 RVA: 0x00129834 File Offset: 0x00127A34
		public override void Progress(ObjectiveIterationCache cache, GameEntity sourceEntity = null, bool failQuietly = false)
		{
			if (!sourceEntity)
			{
				sourceEntity = LocalPlayer.GameEntity;
			}
			Quest quest;
			if (InternalGameDatabase.Quests.TryGetItem(cache.QuestId, out quest) && quest.Enabled && ((quest.CanStartQuest(null) && cache.StartQuestIfNotPresent) || quest.AreObjectivesActive(cache.ObjectiveHashes, null)))
			{
				if (cache.ObjectiveHashes != null)
				{
					foreach (int num in cache.ObjectiveHashes)
					{
						QuestStep questStep;
						QuestObjective questObjective;
						if (num != 0 && quest.TryGetObjectiveUsage(num, out questStep, out questObjective))
						{
							string text;
							if (!questObjective.Validate(sourceEntity, cache, out text))
							{
								return;
							}
							foreach (QuestObjective questObjective2 in questStep.Objectives)
							{
								if (questObjective2.Passive && questObjective2.ActiveParent.Id == questObjective.Id && !questObjective2.Validate(sourceEntity, cache, out text))
								{
									return;
								}
							}
							questObjective.OnComplete(cache, sourceEntity);
							foreach (QuestObjective questObjective3 in questStep.Objectives)
							{
								if (questObjective3.Passive && questObjective3.ActiveParent.Id == questObjective.Id)
								{
									questObjective3.OnComplete(cache, sourceEntity);
								}
							}
						}
					}
				}
				LocalPlayer.NetworkEntity.PlayerRpcHandler.RequestObjectiveIteration(cache);
			}
		}

		// Token: 0x06002367 RID: 9063 RVA: 0x001299E4 File Offset: 0x00127BE4
		public override void NotifyQuestsUpdated(ObjectiveIterationCache cache, GameEntity sourceEntity = null)
		{
			if (!sourceEntity)
			{
				sourceEntity = LocalPlayer.GameEntity;
			}
			Quest quest;
			if (!InternalGameDatabase.Quests.TryGetItem(cache.QuestId, out quest))
			{
				return;
			}
			QuestStep questStep = null;
			QuestProgressionData questProgressionData;
			if (quest.TryGetProgress(out questProgressionData))
			{
				quest.TryGetStep(questProgressionData.CurrentNodeId, out questStep);
			}
			if (cache.ObjectiveHashes == null)
			{
				base.StartQuest(cache, sourceEntity);
			}
			else
			{
				base.UpdateQuest(cache, sourceEntity);
			}
			if (questStep != null && questStep.Id != questProgressionData.CurrentNodeId)
			{
				questStep.OnExit(cache, sourceEntity);
			}
			this.RefreshQuestLookups();
			base.NotifyQuestsUpdated(cache, sourceEntity);
			ObjectiveProgressionData objectiveProgressionData = null;
			QuestObjective questObjective = null;
			if (questProgressionData != null && cache.ObjectiveHashes != null)
			{
				foreach (int hash in cache.ObjectiveHashes)
				{
					QuestStep questStep2;
					if ((quest.TryGetObjectiveUsage(hash, out questStep2, out questObjective) && !questObjective.IsQuiet && !questProgressionData.Muted && questProgressionData.CurrentNodeId == questStep2.Id) || questProgressionData.TryGetObjective(questObjective.Id, out objectiveProgressionData))
					{
						bool flag = questProgressionData.CurrentNodeId == questStep2.Id;
						int num = 1;
						int num2 = (objectiveProgressionData != null) ? objectiveProgressionData.IterationsCompleted : 0;
						if (questStep2.CombineLikeObjectives)
						{
							foreach (QuestObjective questObjective2 in questStep2.Objectives)
							{
								if (questObjective2.Id != questObjective.Id && questObjective2.IterationsRequired == 1 && questObjective2.Description == questObjective.Description)
								{
									num++;
									ObjectiveProgressionData objectiveProgressionData2;
									if (questProgressionData.TryGetObjective(questObjective2.Id, out objectiveProgressionData2) && objectiveProgressionData2.IterationsCompleted > 0)
									{
										num2++;
									}
								}
							}
							if (flag)
							{
								num2 = num;
							}
						}
						string text = null;
						int num3 = (objectiveProgressionData != null) ? objectiveProgressionData.IterationsCompleted : questObjective.IterationsRequired;
						string title;
						if (questStep2.Next.Count == 0 && flag)
						{
							title = "Quest Complete";
							text = quest.Title;
						}
						else if (questStep2.CombineLikeObjectives)
						{
							if (num == 1)
							{
								title = questObjective.Description + " (Complete)";
							}
							else
							{
								title = string.Format("{0} {1}/{2}", questObjective.Description, num2, num);
							}
						}
						else if (questObjective.IterationsRequired == 1)
						{
							title = questObjective.Description + " (Complete)";
						}
						else
						{
							title = string.Format("{0} {1}/{2}", questObjective.Description, num3, questObjective.IterationsRequired);
						}
						ClientGameManager.UIManager.InitCenterScreenAnnouncement(new CenterScreenAnnouncementOptions
						{
							Title = title,
							Text = text,
							TimeShown = 5f,
							ShowDelay = 0f,
							SourceId = new UniqueId?(questObjective.Id)
						});
					}
				}
				return;
			}
			QuestStep questStep3;
			if (questStep == null && (cache.ObjectiveHashes == null || cache.ObjectiveHashes.Length == 0 || quest.TryGetObjectiveUsage(cache.ObjectiveHashes[0], out questStep3, out questObjective)) && !quest.StartHints.HasFlag(ObjectiveBehaviorHint.Quiet) && (!(questObjective != null) || !questObjective.IsQuiet))
			{
				ClientGameManager.UIManager.InitCenterScreenAnnouncement(new CenterScreenAnnouncementOptions
				{
					Title = "Quest Started",
					Text = quest.Title,
					TimeShown = 5f,
					ShowDelay = 0f
				});
			}
		}

		// Token: 0x06002368 RID: 9064 RVA: 0x000597BC File Offset: 0x000579BC
		public override void ResetQuests(GameEntity entity)
		{
			base.ResetQuests(entity);
			this.RefreshQuestLookups();
		}

		// Token: 0x06002369 RID: 9065 RVA: 0x000597CB File Offset: 0x000579CB
		public override void ResetQuest(GameEntity entity, Quest quest)
		{
			base.ResetQuest(entity, quest);
			this.RefreshQuestLookups();
		}

		// Token: 0x0600236A RID: 9066 RVA: 0x00129D98 File Offset: 0x00127F98
		public override void DrawBBTask(BBTaskDrawCache cache, GameEntity entity = null)
		{
			entity = LocalPlayer.GameEntity;
			if (!entity || cache.BulletinBoard == null)
			{
				return;
			}
			if (entity && entity.CollectionController != null && entity.CollectionController.Record != null && entity.CollectionController.Record.Progression != null && entity.CollectionController.Record.Progression.BBTasks != null)
			{
				foreach (UniqueId id in entity.CollectionController.Record.Progression.BBTasks.Keys)
				{
					BBTask bbtask;
					if (InternalGameDatabase.BBTasks.TryGetItem(id, out bbtask) && bbtask.Enabled && bbtask.BulletinBoard.Id == cache.BulletinBoard.Id && bbtask.Type == cache.Type)
					{
						Debug.LogError("Task already accepted for this bulletin board and task type!");
						return;
					}
				}
			}
			entity.NetworkEntity.PlayerRpcHandler.DrawBBTask(cache);
		}

		// Token: 0x0600236B RID: 9067 RVA: 0x00129ECC File Offset: 0x001280CC
		public override void ProgressTask(ObjectiveIterationCache cache, GameEntity sourceEntity = null, bool failQuietly = false)
		{
			if (!sourceEntity)
			{
				sourceEntity = LocalPlayer.GameEntity;
			}
			BBTask bbtask;
			if (InternalGameDatabase.BBTasks.TryGetItem(cache.QuestId, out bbtask) && bbtask.Enabled)
			{
				if (cache.ObjectiveHashes != null)
				{
					foreach (int num in cache.ObjectiveHashes)
					{
						QuestObjective questObjective;
						if (num != 0 && bbtask.TryGetObjective(num, out questObjective))
						{
							string text;
							if (!questObjective.Validate(sourceEntity, cache, out text))
							{
								return;
							}
							foreach (QuestObjective questObjective2 in bbtask.Objectives)
							{
								if (questObjective2.Passive && questObjective2.ActiveParent.Id == questObjective.Id && !questObjective2.Validate(sourceEntity, cache, out text))
								{
									return;
								}
							}
							questObjective.OnComplete(cache, sourceEntity);
							foreach (QuestObjective questObjective3 in bbtask.Objectives)
							{
								if (questObjective3.Passive && questObjective3.ActiveParent.Id == questObjective.Id)
								{
									questObjective3.OnComplete(cache, sourceEntity);
								}
							}
						}
					}
				}
				sourceEntity.NetworkEntity.PlayerRpcHandler.IterateBBTask(cache);
			}
		}

		// Token: 0x0600236C RID: 9068 RVA: 0x0012A050 File Offset: 0x00128250
		public override void NotifyTaskUpdated(ObjectiveIterationCache cache, GameEntity sourceEntity = null)
		{
			BBTask bbtask;
			if (InternalGameDatabase.BBTasks.TryGetItem(cache.QuestId, out bbtask))
			{
				bool flag;
				if (cache.ObjectiveHashes == null)
				{
					if (!bbtask.IsReadyForTurnIn(sourceEntity))
					{
						if (base.StartBBTask(cache, sourceEntity))
						{
							ClientGameManager.UIManager.InitCenterScreenAnnouncement(new CenterScreenAnnouncementOptions
							{
								Title = "Task Accepted",
								Text = bbtask.Title,
								TimeShown = 5f,
								ShowDelay = 0f,
								SourceId = new UniqueId?(bbtask.Id)
							});
						}
					}
					else
					{
						base.TurnInBBTask(cache, sourceEntity);
					}
				}
				else if (base.UpdateBBTask(cache, sourceEntity, out flag))
				{
					if (flag)
					{
						ClientGameManager.UIManager.InitCenterScreenAnnouncement(new CenterScreenAnnouncementOptions
						{
							Title = "Task Complete",
							Text = "Return to the relevant bulletin board to turn in",
							TimeShown = 5f,
							ShowDelay = 0f,
							SourceId = new UniqueId?(bbtask.Id)
						});
					}
					else if (!bbtask.OverrideObjectiveText)
					{
						foreach (int hash in cache.ObjectiveHashes)
						{
							QuestObjective questObjective;
							BBTaskProgressionData bbtaskProgressionData;
							ObjectiveProgressionData objectiveProgressionData;
							if (bbtask.TryGetObjective(hash, out questObjective) && !questObjective.IsQuiet && bbtask.TryGetProgress(sourceEntity, out bbtaskProgressionData) && bbtaskProgressionData.TryGetObjective(questObjective.Id, out objectiveProgressionData))
							{
								string text = bbtask.OverrideObjectiveText ? bbtask.ObjectiveOverrideText : questObjective.Description;
								int num = (objectiveProgressionData != null) ? objectiveProgressionData.IterationsCompleted : questObjective.IterationsRequired;
								string title;
								if (questObjective.IterationsRequired == 1)
								{
									title = text + " (Complete)";
								}
								else
								{
									title = string.Format("{0} {1}/{2}", text, num, questObjective.IterationsRequired);
								}
								ClientGameManager.UIManager.InitCenterScreenAnnouncement(new CenterScreenAnnouncementOptions
								{
									Title = title,
									Text = null,
									TimeShown = 5f,
									ShowDelay = 0f,
									SourceId = new UniqueId?(bbtask.Id)
								});
							}
						}
					}
				}
			}
			base.NotifyTaskUpdated(cache, sourceEntity);
		}

		// Token: 0x0600236D RID: 9069 RVA: 0x0012A298 File Offset: 0x00128498
		public override void Learn(NpcLearningCache cache, GameEntity sourceEntity = null)
		{
			if (!sourceEntity)
			{
				sourceEntity = LocalPlayer.GameEntity;
			}
			bool flag = false;
			if (!cache.NpcEntity && cache.WorldId.IsEmpty)
			{
				return;
			}
			IDialogueNpc dialogueNpc;
			if (cache.NpcEntity && cache.NpcEntity.TryGetComponent<IDialogueNpc>(out dialogueNpc))
			{
				if (!dialogueNpc.CanConverseWith(sourceEntity))
				{
					return;
				}
				if (cache.NpcEntity.GameEntity)
				{
					NpcCharacterData npcCharacterData = cache.NpcEntity.GameEntity.CharacterData as NpcCharacterData;
					if (npcCharacterData != null)
					{
						if (npcCharacterData.NpcInitData.ProfileId != cache.NpcProfile.Id)
						{
							return;
						}
						flag = true;
						goto IL_9E;
					}
				}
				return;
			}
			IL_9E:
			if (!cache.WorldId.IsEmpty)
			{
				IWorldObject worldObject;
				if (!LocalZoneManager.TryGetWorldObject(cache.WorldId, out worldObject))
				{
					return;
				}
				if (!worldObject.Validate(sourceEntity))
				{
					return;
				}
				IKnowledgeCapable knowledgeCapable = worldObject as IKnowledgeCapable;
				if (knowledgeCapable == null)
				{
					return;
				}
				if (knowledgeCapable != null && knowledgeCapable.KnowledgeHolder != null && knowledgeCapable.KnowledgeHolder.Id != cache.NpcProfile.Id)
				{
					return;
				}
				flag = true;
			}
			if (flag)
			{
				sourceEntity.NetworkEntity.PlayerRpcHandler.RequestNpcLearn(cache);
			}
		}

		// Token: 0x0600236E RID: 9070 RVA: 0x000597DB File Offset: 0x000579DB
		public override void NotifyLearned(NpcLearningCache cache, GameEntity sourceEntity = null)
		{
			if (!sourceEntity)
			{
				sourceEntity = LocalPlayer.GameEntity;
			}
			base.UpdateNpcKnowledge(cache, sourceEntity);
			base.NotifyLearned(cache, sourceEntity);
		}

		// Token: 0x0600236F RID: 9071 RVA: 0x000597FC File Offset: 0x000579FC
		public override void ReissueReward(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			if (base.HasReissuableReward(cache.QuestId, sourceEntity))
			{
				sourceEntity.NetworkEntity.PlayerRpcHandler.RequestRewardReissue(cache);
			}
		}

		// Token: 0x06002370 RID: 9072 RVA: 0x0005981E File Offset: 0x00057A1E
		private void RegisterCommands()
		{
			CommandRegistry.Register("listobjectives", delegate(string[] args)
			{
				if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Record != null && LocalPlayer.GameEntity.CollectionController.Record.Progression != null && LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests != null)
				{
					StringBuilder fromPool = StringBuilderExtensions.GetFromPool();
					fromPool.AppendLine("Active Objectives:");
					foreach (KeyValuePair<UniqueId, QuestProgressionData> keyValuePair in LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests)
					{
						Quest quest;
						QuestStep questStep;
						if (InternalGameDatabase.Quests.TryGetItem(keyValuePair.Key, out quest) && quest.TryGetStep(keyValuePair.Value.CurrentNodeId, out questStep) && questStep.Next.Count > 0)
						{
							fromPool.AppendLine("Quest: " + quest.Title);
							fromPool.AppendLine("Active Objectives: [");
							for (int i = 0; i < questStep.NextSteps.Count; i++)
							{
								if (i > 0)
								{
									fromPool.AppendLine("    <OR>");
								}
								foreach (QuestObjective questObjective in questStep.NextSteps[i].Objectives)
								{
									DiscoveryObjective discoveryObjective = questObjective as DiscoveryObjective;
									if (discoveryObjective != null)
									{
										fromPool.AppendLine("    Discover:");
										using (List<DiscoveryProfile>.Enumerator enumerator3 = discoveryObjective.ListUndiscovered(LocalPlayer.GameEntity).GetEnumerator())
										{
											while (enumerator3.MoveNext())
											{
												DiscoveryProfile discoveryProfile = enumerator3.Current;
												fromPool.AppendLine("        " + discoveryProfile.DisplayName);
											}
											continue;
										}
									}
									LootObjective lootObjective = questObjective as LootObjective;
									ObjectiveProgressionData objectiveProgressionData;
									if (lootObjective != null)
									{
										int availableItems = lootObjective.GetAvailableItems(LocalPlayer.GameEntity, null);
										fromPool.AppendLine("    Loot:");
										fromPool.AppendLine(string.Format("        {0} ({1}/{2})", lootObjective.BuildObjectiveDescription(), availableItems, lootObjective.AmountRequired));
									}
									else if (keyValuePair.Value.TryGetObjective(questObjective.Id, out objectiveProgressionData))
									{
										fromPool.AppendLine(string.Format("    {0} ({1}/{2})", questObjective.Description, objectiveProgressionData.IterationsCompleted, questObjective.IterationsRequired));
									}
									else if (questObjective.IterationsRequired > 1)
									{
										fromPool.AppendLine(string.Format("    {0} (0/{1})", questObjective.Description, questObjective.IterationsRequired));
									}
									else
									{
										fromPool.AppendLine("    " + questObjective.Description);
									}
								}
							}
							fromPool.AppendLine("]");
						}
					}
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, fromPool.ToString_ReturnToPool());
				}
			}, "Displays a list of current quests and objectives", null, null);
		}

		// Token: 0x06002371 RID: 9073 RVA: 0x0012A3C4 File Offset: 0x001285C4
		private void OnLocalPlayerInitialized()
		{
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Masteries != null)
			{
				this.ValidateMasteryExistsOrDropQuestsAndTasks();
				LocalPlayer.GameEntity.CollectionController.Masteries.InstanceRemoved += this.OnMasteryUnlearned;
				foreach (ArchetypeInstance archetypeInstance in LocalPlayer.GameEntity.CollectionController.Masteries.Instances)
				{
					if (archetypeInstance.IsMastery && archetypeInstance.Mastery.HasSpecializations)
					{
						archetypeInstance.MasteryData.SpecializationUnlearned += this.OnSpecializationUnlearned;
					}
				}
			}
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Record != null && LocalPlayer.GameEntity.CollectionController.Record.Progression != null)
			{
				if (LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests != null)
				{
					foreach (KeyValuePair<UniqueId, QuestProgressionData> keyValuePair in LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests)
					{
						Quest quest;
						QuestStep questStep;
						if (InternalGameDatabase.Quests.TryGetItem(keyValuePair.Key, out quest) && quest.Enabled && quest.TryGetStep(keyValuePair.Value.CurrentNodeId, out questStep))
						{
							questStep.OnEntityInitialized(LocalPlayer.GameEntity);
						}
					}
				}
				if (LocalPlayer.GameEntity.CollectionController.Record.Progression.BBTasks != null)
				{
					foreach (KeyValuePair<UniqueId, BBTaskProgressionData> keyValuePair2 in LocalPlayer.GameEntity.CollectionController.Record.Progression.BBTasks)
					{
						BBTask bbtask;
						if (InternalGameDatabase.BBTasks.TryGetItem(keyValuePair2.Key, out bbtask) && bbtask.Enabled)
						{
							bbtask.OnEntityInitialized(LocalPlayer.GameEntity);
						}
					}
				}
			}
		}

		// Token: 0x06002372 RID: 9074 RVA: 0x0012A62C File Offset: 0x0012882C
		private void OnLocalPlayerDestroyed()
		{
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Masteries != null)
			{
				LocalPlayer.GameEntity.CollectionController.Masteries.InstanceRemoved -= this.OnMasteryUnlearned;
				foreach (ArchetypeInstance archetypeInstance in LocalPlayer.GameEntity.CollectionController.Masteries.Instances)
				{
					if (archetypeInstance.IsMastery && archetypeInstance.Mastery.HasSpecializations)
					{
						archetypeInstance.MasteryData.SpecializationUnlearned -= this.OnSpecializationUnlearned;
					}
				}
			}
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Record != null && LocalPlayer.GameEntity.CollectionController.Record.Progression != null)
			{
				if (LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests != null)
				{
					foreach (KeyValuePair<UniqueId, QuestProgressionData> keyValuePair in LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests)
					{
						Quest quest;
						QuestStep questStep;
						if (InternalGameDatabase.Quests.TryGetItem(keyValuePair.Key, out quest) && quest.Enabled && quest.TryGetStep(keyValuePair.Value.CurrentNodeId, out questStep))
						{
							questStep.OnEntityDestroyed(LocalPlayer.GameEntity);
						}
					}
				}
				if (LocalPlayer.GameEntity.CollectionController.Record.Progression.BBTasks != null)
				{
					foreach (KeyValuePair<UniqueId, BBTaskProgressionData> keyValuePair2 in LocalPlayer.GameEntity.CollectionController.Record.Progression.BBTasks)
					{
						BBTask bbtask;
						if (InternalGameDatabase.BBTasks.TryGetItem(keyValuePair2.Key, out bbtask) && bbtask.Enabled)
						{
							bbtask.OnEntityDestroyed(LocalPlayer.GameEntity);
						}
					}
				}
			}
		}

		// Token: 0x06002373 RID: 9075 RVA: 0x0012A88C File Offset: 0x00128A8C
		private void ValidateMasteryExistsOrDropQuestsAndTasks()
		{
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Record != null && LocalPlayer.GameEntity.CollectionController.Record.Progression != null && LocalPlayer.GameEntity.CollectionController.Masteries != null)
			{
				List<UniqueId> fromPool = StaticListPool<UniqueId>.GetFromPool();
				if (LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests != null)
				{
					foreach (UniqueId id in LocalPlayer.GameEntity.CollectionController.Record.Progression.Quests.Keys)
					{
						Quest quest;
						if (InternalGameDatabase.Quests.TryGetItem(id, out quest) && !(quest.AssociatedMastery == null))
						{
							bool flag = false;
							foreach (ArchetypeInstance archetypeInstance in LocalPlayer.GameEntity.CollectionController.Masteries.Instances)
							{
								if (quest.AssociatedMastery.Id == archetypeInstance.ArchetypeId || (archetypeInstance.MasteryData.Specialization != null && quest.AssociatedMastery.Id == archetypeInstance.MasteryData.Specialization))
								{
									flag = true;
								}
							}
							if (!flag && !fromPool.Contains(quest.AssociatedMastery.Id))
							{
								fromPool.Add(quest.AssociatedMastery.Id);
							}
						}
					}
				}
				if (LocalPlayer.GameEntity.CollectionController.Record.Progression.BBTasks != null)
				{
					foreach (UniqueId id2 in LocalPlayer.GameEntity.CollectionController.Record.Progression.BBTasks.Keys)
					{
						BBTask bbtask;
						if (InternalGameDatabase.BBTasks.TryGetItem(id2, out bbtask) && !(bbtask.AssociatedMastery == null))
						{
							bool flag2 = false;
							foreach (ArchetypeInstance archetypeInstance2 in LocalPlayer.GameEntity.CollectionController.Masteries.Instances)
							{
								if (bbtask.AssociatedMastery.Id == archetypeInstance2.ArchetypeId || (archetypeInstance2.MasteryData.Specialization != null && bbtask.AssociatedMastery.Id == archetypeInstance2.MasteryData.Specialization))
								{
									flag2 = true;
								}
							}
							if (!flag2 && !fromPool.Contains(bbtask.AssociatedMastery.Id))
							{
								fromPool.Add(bbtask.AssociatedMastery.Id);
							}
						}
					}
				}
				foreach (UniqueId archetypeId in fromPool)
				{
					LocalPlayer.NetworkEntity.PlayerRpcHandler.RequestDropQuestsAndTasksForMastery(archetypeId);
				}
				StaticListPool<UniqueId>.ReturnToPool(fromPool);
			}
		}

		// Token: 0x06002374 RID: 9076 RVA: 0x00059850 File Offset: 0x00057A50
		private void OnMasteryUnlearned(ArchetypeInstance instance)
		{
			if (instance.Mastery.HasSpecializations)
			{
				instance.MasteryData.SpecializationUnlearned -= this.OnSpecializationUnlearned;
			}
			LocalPlayer.NetworkEntity.PlayerRpcHandler.RequestDropQuestsAndTasksForMastery(instance.ArchetypeId);
		}

		// Token: 0x06002375 RID: 9077 RVA: 0x0005988B File Offset: 0x00057A8B
		private void OnSpecializationUnlearned(UniqueId id)
		{
			LocalPlayer.NetworkEntity.PlayerRpcHandler.RequestDropQuestsAndTasksForMastery(id);
		}

		// Token: 0x040026EC RID: 9964
		private readonly Dictionary<UniqueId, ClientQuestManager.DelayedRefreshData> m_delayedRefreshDict = new Dictionary<UniqueId, ClientQuestManager.DelayedRefreshData>(default(UniqueIdComparer));

		// Token: 0x040026ED RID: 9965
		private readonly List<UniqueId> m_delayedRefreshesToRemove = new List<UniqueId>(10);

		// Token: 0x040026EE RID: 9966
		private readonly Dictionary<string, Quest> m_questLookupByTag = new Dictionary<string, Quest>();

		// Token: 0x040026EF RID: 9967
		private readonly Dictionary<UniqueId, List<InkEntry>> m_inkLookup = new Dictionary<UniqueId, List<InkEntry>>();

		// Token: 0x020004EF RID: 1263
		private struct DelayedRefreshData
		{
			// Token: 0x040026F1 RID: 9969
			public int FrameToRefresh;

			// Token: 0x040026F2 RID: 9970
			public IDelayedRefresh ToRefresh;
		}
	}
}
