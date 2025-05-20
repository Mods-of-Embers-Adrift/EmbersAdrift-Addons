using System;
using System.Collections.Generic;
using SoL.Game;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Quests;
using SoL.Game.Quests.Objectives;
using SoL.Networking;
using SoL.Networking.Database;
using SoL.Networking.Objects;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Managers
{
	// Token: 0x02000543 RID: 1347
	public class ServerQuestManager : QuestManager
	{
		// Token: 0x17000872 RID: 2162
		// (get) Token: 0x060028E8 RID: 10472 RVA: 0x0005C534 File Offset: 0x0005A734
		public List<ValueTuple<KillNpcObjective, ObjectiveIterationCache>> KillStartQuests
		{
			get
			{
				return this.m_killStartQuests;
			}
		}

		// Token: 0x060028E9 RID: 10473 RVA: 0x0005C53C File Offset: 0x0005A73C
		protected override void Start()
		{
			base.Start();
			NetworkEntity.Initialized += this.OnNetworkEntityInitialized;
			NetworkEntity.Destroyed += this.OnNetworkEntityDestroyed;
			this.InitKillStartQuestsLookup();
			this.InitBulletinBoardTaskCaches();
		}

		// Token: 0x060028EA RID: 10474 RVA: 0x0005C572 File Offset: 0x0005A772
		protected override void OnDestroy()
		{
			base.OnDestroy();
			NetworkEntity.Initialized -= this.OnNetworkEntityInitialized;
			NetworkEntity.Destroyed -= this.OnNetworkEntityDestroyed;
		}

		// Token: 0x060028EB RID: 10475 RVA: 0x0013DB10 File Offset: 0x0013BD10
		private void InitKillStartQuestsLookup()
		{
			this.m_killStartQuests.Clear();
			foreach (Quest quest in InternalGameDatabase.Quests.GetAllItems())
			{
				List<ValueTuple<QuestStep, KillNpcObjective>> list;
				if (quest.Enabled && quest.TryGetKillStartObjective(out list))
				{
					foreach (ValueTuple<QuestStep, KillNpcObjective> valueTuple in list)
					{
						this.m_killStartQuests.Add(new ValueTuple<KillNpcObjective, ObjectiveIterationCache>(valueTuple.Item2, new ObjectiveIterationCache
						{
							QuestId = quest.Id,
							ObjectiveHashes = new int[]
							{
								valueTuple.Item2.CombinedId(valueTuple.Item1.Id)
							},
							StartQuestIfNotPresent = true
						}));
					}
					StaticListPool<ValueTuple<QuestStep, KillNpcObjective>>.ReturnToPool(list);
				}
			}
		}

		// Token: 0x060028EC RID: 10476 RVA: 0x0013DC20 File Offset: 0x0013BE20
		private void InitBulletinBoardTaskCaches()
		{
			foreach (BBTask bbtask in InternalGameDatabase.BBTasks.GetAllItems())
			{
				if (bbtask != null && bbtask.BulletinBoard != null)
				{
					bbtask.BulletinBoard.Tasks.Add(bbtask);
				}
			}
		}

		// Token: 0x060028ED RID: 10477 RVA: 0x0013DC94 File Offset: 0x0013BE94
		public override void Progress(ObjectiveIterationCache cache, GameEntity sourceEntity = null, bool failQuietly = false)
		{
			if (!sourceEntity)
			{
				return;
			}
			Quest quest;
			if (InternalGameDatabase.Quests.TryGetItem(cache.QuestId, out quest) && quest.Enabled && ((quest.CanStartQuest(sourceEntity) && cache.StartQuestIfNotPresent) || quest.AreObjectivesActive(cache.ObjectiveHashes, sourceEntity)))
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
								if (text == string.Empty)
								{
									text = "Objective validation failed.";
								}
								Debug.Log(string.Format("Objective validation failed for character {0} on quest {1} and objective {2} with message: {3}", new object[]
								{
									sourceEntity.CharacterData.CharacterId,
									quest.Id,
									questObjective.Id,
									text
								}));
								if (!failQuietly)
								{
									sourceEntity.NetworkEntity.PlayerRpcHandler.NotifyObjectiveIteration(OpCodes.Error, text, cache);
								}
								return;
							}
							foreach (QuestObjective questObjective2 in questStep.Objectives)
							{
								if (questObjective2.Passive && questObjective2.ActiveParent.Id == questObjective.Id && !questObjective2.Validate(sourceEntity, cache, out text))
								{
									if (text == string.Empty)
									{
										text = "Passive objective validation failed.";
									}
									Debug.Log(string.Format("Passive objective validation failed for character {0} on task {1} and objective {2} with message: {3}", new object[]
									{
										sourceEntity.CharacterData.CharacterId,
										quest.Id,
										questObjective2.Id,
										text
									}));
									if (!failQuietly)
									{
										sourceEntity.NetworkEntity.PlayerRpcHandler.NotifyObjectiveIteration(OpCodes.Error, text, cache);
									}
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
				QuestStep questStep2 = null;
				QuestProgressionData questProgressionData;
				if (quest.TryGetProgress(sourceEntity, out questProgressionData))
				{
					quest.TryGetStep(questProgressionData.CurrentNodeId, out questStep2);
				}
				if ((cache.ObjectiveHashes == null) ? base.StartQuest(cache, sourceEntity) : base.UpdateQuest(cache, sourceEntity))
				{
					if (questStep2 != null && questStep2.Id != questProgressionData.CurrentNodeId)
					{
						questStep2.OnExit(cache, sourceEntity);
					}
					sourceEntity.CollectionController.Record.UpdateQuests(ExternalGameDatabase.Database);
					sourceEntity.NetworkEntity.PlayerRpcHandler.NotifyObjectiveIteration(OpCodes.Ok, string.Empty, cache);
					this.NotifyQuestsUpdated(cache, sourceEntity);
				}
			}
		}

		// Token: 0x060028EE RID: 10478 RVA: 0x0013DFC0 File Offset: 0x0013C1C0
		public override void DrawBBTask(BBTaskDrawCache cache, GameEntity entity = null)
		{
			if (!entity || entity.CollectionController == null || entity.CharacterData == null || cache.BulletinBoard == null)
			{
				return;
			}
			if (entity.CollectionController.Record != null && entity.CollectionController.Record.Progression != null && entity.CollectionController.Record.Progression.BBTasks != null)
			{
				foreach (UniqueId id in entity.CollectionController.Record.Progression.BBTasks.Keys)
				{
					BBTask bbtask;
					if (InternalGameDatabase.BBTasks.TryGetItem(id, out bbtask) && bbtask.Enabled && bbtask.BulletinBoard.Id == cache.BulletinBoard.Id && bbtask.Type == cache.Type)
					{
						entity.NetworkEntity.PlayerRpcHandler.NotifyDrawBBTask(OpCodes.Error, "Task already accepted for this bulletin board and task type!", cache);
						return;
					}
				}
			}
			List<BBTask> localTaskDiscard = entity.CollectionController.LocalTaskDiscard;
			int num;
			switch (cache.Type)
			{
			case BBTaskType.Adventuring:
				num = entity.CharacterData.AdventuringLevel;
				break;
			case BBTaskType.Crafting:
				num = entity.CharacterData.CraftingLevel;
				break;
			case BBTaskType.Gathering:
				num = entity.CharacterData.GatheringLevel;
				break;
			default:
				num = 1;
				break;
			}
			int playerLevel = num;
			BBTask bbtask2 = ServerQuestManager.DrawTaskForLevel(cache.BulletinBoard, cache.Type, playerLevel, localTaskDiscard, entity);
			if (bbtask2 == null)
			{
				entity.NetworkEntity.PlayerRpcHandler.NotifyDrawBBTask(OpCodes.Error, "No eligible tasks found!", cache);
				return;
			}
			ObjectiveIterationCache cache2 = new ObjectiveIterationCache
			{
				QuestId = bbtask2.Id
			};
			this.ProgressTask(cache2, entity, false);
		}

		// Token: 0x060028EF RID: 10479 RVA: 0x0013E1A8 File Offset: 0x0013C3A8
		private static BBTask DrawTaskForLevel(BulletinBoard board, BBTaskType type, int playerLevel, List<BBTask> playerDiscard, GameEntity entity)
		{
			if (board == null)
			{
				throw new ArgumentNullException("board");
			}
			if (!entity)
			{
				throw new ArgumentNullException("entity");
			}
			BBTask bbtask = null;
			List<BBTask> fromPool = StaticListPool<BBTask>.GetFromPool();
			List<BBTask> fromPool2 = StaticListPool<BBTask>.GetFromPool();
			List<BBTask> fromPool3 = StaticListPool<BBTask>.GetFromPool();
			foreach (BBTask bbtask2 in board.Tasks)
			{
				if (bbtask2 && bbtask2.Enabled && bbtask2.Type == type && (!Application.isPlaying || bbtask2.CanStart(entity)))
				{
					playerLevel = 1;
					BBTaskType type2 = bbtask2.Type;
					if (type2 != BBTaskType.Adventuring)
					{
						if (type2 - BBTaskType.Crafting <= 1)
						{
							ArchetypeInstance archetypeInstance;
							if (bbtask2.AssociatedMastery && entity.CollectionController != null && entity.CollectionController.Masteries != null && entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(bbtask2.AssociatedMastery.Id, out archetypeInstance) && archetypeInstance.MasteryData != null)
							{
								playerLevel = Mathf.FloorToInt(archetypeInstance.MasteryData.BaseLevel);
							}
						}
					}
					else if (entity.CharacterData)
					{
						playerLevel = entity.CharacterData.AdventuringLevel;
					}
					if (playerLevel >= bbtask2.LevelRange.x && playerLevel <= bbtask2.LevelRange.y)
					{
						fromPool.Add(bbtask2);
					}
					else if (playerLevel > bbtask2.LevelRange.y)
					{
						fromPool2.Add(bbtask2);
					}
					else if (playerLevel < bbtask2.LevelRange.x)
					{
						fromPool3.Add(bbtask2);
					}
				}
			}
			if (fromPool.Count > 0)
			{
				fromPool.Shuffle<BBTask>();
				foreach (BBTask bbtask3 in fromPool)
				{
					if (playerDiscard == null || !playerDiscard.Contains(bbtask3))
					{
						bbtask = bbtask3;
						break;
					}
				}
				if (bbtask == null)
				{
					if (playerDiscard != null)
					{
						playerDiscard.Clear();
					}
					bbtask = fromPool[UnityEngine.Random.Range(0, fromPool.Count)];
				}
			}
			else if (fromPool2.Count > 0)
			{
				int num = int.MinValue;
				foreach (BBTask bbtask4 in fromPool2)
				{
					if (bbtask4.LevelRange.y > num)
					{
						num = bbtask4.LevelRange.y;
					}
				}
				List<BBTask> fromPool4 = StaticListPool<BBTask>.GetFromPool();
				foreach (BBTask bbtask5 in fromPool2)
				{
					if (bbtask5.LevelRange.y >= num)
					{
						fromPool4.Add(bbtask5);
					}
				}
				fromPool4.Shuffle<BBTask>();
				foreach (BBTask bbtask6 in fromPool4)
				{
					if (playerDiscard == null || !playerDiscard.Contains(bbtask6))
					{
						bbtask = bbtask6;
						break;
					}
				}
				if (bbtask == null)
				{
					if (playerDiscard != null)
					{
						playerDiscard.Clear();
					}
					bbtask = fromPool4[UnityEngine.Random.Range(0, fromPool4.Count)];
				}
				StaticListPool<BBTask>.ReturnToPool(fromPool4);
			}
			else if (fromPool3.Count > 0)
			{
				int num2 = int.MaxValue;
				foreach (BBTask bbtask7 in fromPool3)
				{
					if (bbtask7.LevelRange.x < num2)
					{
						num2 = bbtask7.LevelRange.x;
					}
				}
				List<BBTask> fromPool5 = StaticListPool<BBTask>.GetFromPool();
				foreach (BBTask bbtask8 in fromPool3)
				{
					if (bbtask8.LevelRange.x <= num2)
					{
						fromPool5.Add(bbtask8);
					}
				}
				fromPool5.Shuffle<BBTask>();
				foreach (BBTask bbtask9 in fromPool5)
				{
					if (playerDiscard == null || !playerDiscard.Contains(bbtask9))
					{
						bbtask = bbtask9;
						break;
					}
				}
				if (bbtask == null)
				{
					if (playerDiscard != null)
					{
						playerDiscard.Clear();
					}
					bbtask = fromPool5[UnityEngine.Random.Range(0, fromPool5.Count)];
				}
				StaticListPool<BBTask>.ReturnToPool(fromPool5);
			}
			StaticListPool<BBTask>.ReturnToPool(fromPool);
			StaticListPool<BBTask>.ReturnToPool(fromPool2);
			StaticListPool<BBTask>.ReturnToPool(fromPool3);
			return bbtask;
		}

		// Token: 0x060028F0 RID: 10480 RVA: 0x0013E710 File Offset: 0x0013C910
		private BBTask DrawTask(BulletinBoard board, BBTaskType type, List<BBTask> discardedTasks, GameEntity entity, out bool earlyResetNeeded)
		{
			earlyResetNeeded = false;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			int num;
			switch (type)
			{
			case BBTaskType.Adventuring:
				num = entity.CharacterData.AdventuringLevel;
				break;
			case BBTaskType.Crafting:
				num = entity.CharacterData.CraftingLevel;
				break;
			case BBTaskType.Gathering:
				num = entity.CharacterData.GatheringLevel;
				break;
			default:
				num = 1;
				break;
			}
			int num2 = num;
			List<BBTask> fromPool = StaticListPool<BBTask>.GetFromPool();
			List<BBTask> fromPool2 = StaticListPool<BBTask>.GetFromPool();
			List<BBTask> fromPool3 = StaticListPool<BBTask>.GetFromPool();
			foreach (BBTask bbtask in board.Tasks)
			{
				bool flag5 = discardedTasks.Contains(bbtask);
				if (bbtask.Type == type && bbtask.CanStart(entity))
				{
					if (num2 >= bbtask.LevelRange.x && num2 <= bbtask.LevelRange.y)
					{
						if (flag5)
						{
							flag = true;
						}
						else
						{
							fromPool.Add(bbtask);
						}
					}
					else if (num2 > bbtask.LevelRange.y)
					{
						if (flag5)
						{
							flag2 = true;
						}
						else
						{
							fromPool3.Add(bbtask);
						}
					}
					else if (num2 < bbtask.LevelRange.x)
					{
						if (!flag5)
						{
							fromPool2.Add(bbtask);
						}
					}
					else
					{
						Debug.LogError("Unable to find pool for task, must be a problem with my level range conditionals.");
					}
				}
			}
			BBTask result = null;
			if (fromPool.Count > 0)
			{
				result = fromPool[UnityEngine.Random.Range(0, fromPool.Count)];
			}
			else if (fromPool3.Count > 0)
			{
				result = fromPool3[UnityEngine.Random.Range(0, fromPool3.Count)];
				flag3 = true;
			}
			else if (fromPool2.Count > 0)
			{
				result = fromPool2[UnityEngine.Random.Range(0, fromPool2.Count)];
				flag4 = true;
			}
			if (flag3 && flag)
			{
				earlyResetNeeded = true;
			}
			else if ((flag || flag2) && flag4)
			{
				earlyResetNeeded = true;
			}
			StaticListPool<BBTask>.ReturnToPool(fromPool);
			StaticListPool<BBTask>.ReturnToPool(fromPool2);
			StaticListPool<BBTask>.ReturnToPool(fromPool3);
			return result;
		}

		// Token: 0x060028F1 RID: 10481 RVA: 0x0013E920 File Offset: 0x0013CB20
		public override void ProgressTask(ObjectiveIterationCache cache, GameEntity sourceEntity = null, bool failQuietly = false)
		{
			if (!sourceEntity)
			{
				return;
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
								if (text == string.Empty)
								{
									text = "Objective validation failed.";
								}
								Debug.Log(string.Format("Objective validation failed for character {0} on task {1} and objective {2} with message: {3}", new object[]
								{
									sourceEntity.CharacterData.CharacterId,
									bbtask.Id,
									questObjective.Id,
									text
								}));
								if (!failQuietly)
								{
									sourceEntity.NetworkEntity.PlayerRpcHandler.NotifyObjectiveIteration(OpCodes.Error, text, cache);
								}
								return;
							}
							foreach (QuestObjective questObjective2 in bbtask.Objectives)
							{
								if (questObjective2.Passive && questObjective2.ActiveParent.Id == questObjective.Id && !questObjective2.Validate(sourceEntity, cache, out text))
								{
									if (text == string.Empty)
									{
										text = "Passive objective validation failed.";
									}
									if (!failQuietly)
									{
										sourceEntity.NetworkEntity.PlayerRpcHandler.NotifyObjectiveIteration(OpCodes.Error, text, cache);
									}
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
				bool flag;
				if ((cache.ObjectiveHashes == null) ? (bbtask.IsReadyForTurnIn(sourceEntity) ? base.TurnInBBTask(cache, sourceEntity) : base.StartBBTask(cache, sourceEntity)) : base.UpdateBBTask(cache, sourceEntity, out flag))
				{
					sourceEntity.CollectionController.Record.UpdateQuests(ExternalGameDatabase.Database);
					sourceEntity.NetworkEntity.PlayerRpcHandler.NotifyBBTaskIterated(OpCodes.Ok, string.Empty, cache);
					this.NotifyTaskUpdated(cache, sourceEntity);
				}
			}
		}

		// Token: 0x060028F2 RID: 10482 RVA: 0x0013EB98 File Offset: 0x0013CD98
		public override void Learn(NpcLearningCache cache, GameEntity sourceEntity = null)
		{
			if (!sourceEntity)
			{
				return;
			}
			bool flag = false;
			if (!cache.NpcEntity && cache.WorldId.IsEmpty)
			{
				sourceEntity.NetworkEntity.PlayerRpcHandler.NotifyNpcLearn(OpCodes.Error, "No validation entity provided!", cache);
				return;
			}
			IDialogueNpc dialogueNpc;
			if (cache.NpcEntity && cache.NpcEntity.TryGetComponent<IDialogueNpc>(out dialogueNpc))
			{
				if (!dialogueNpc.CanConverseWith(sourceEntity))
				{
					sourceEntity.NetworkEntity.PlayerRpcHandler.NotifyNpcLearn(OpCodes.Error, "Cannot converse with NPC!", cache);
					return;
				}
				if (cache.NpcEntity.GameEntity)
				{
					NpcCharacterData npcCharacterData = cache.NpcEntity.GameEntity.CharacterData as NpcCharacterData;
					if (npcCharacterData != null)
					{
						if (npcCharacterData.NpcInitData.ProfileId != cache.NpcProfile.Id)
						{
							sourceEntity.NetworkEntity.PlayerRpcHandler.NotifyNpcLearn(OpCodes.Error, "Validation entity does not match the specified profile!", cache);
							return;
						}
						flag = true;
						goto IL_FE;
					}
				}
				sourceEntity.NetworkEntity.PlayerRpcHandler.NotifyNpcLearn(OpCodes.Error, "Validation entity is not an NPC!", cache);
				return;
			}
			IL_FE:
			if (!cache.WorldId.IsEmpty)
			{
				IWorldObject worldObject;
				if (!LocalZoneManager.TryGetWorldObject(cache.WorldId, out worldObject))
				{
					sourceEntity.NetworkEntity.PlayerRpcHandler.NotifyNpcLearn(OpCodes.Error, "Invalid world object ID!", cache);
					return;
				}
				if (!worldObject.Validate(sourceEntity))
				{
					sourceEntity.NetworkEntity.PlayerRpcHandler.NotifyNpcLearn(OpCodes.Error, "Failed world object validation! Are you too far from the object?", cache);
					return;
				}
				IKnowledgeCapable knowledgeCapable = worldObject as IKnowledgeCapable;
				if (knowledgeCapable == null)
				{
					sourceEntity.NetworkEntity.PlayerRpcHandler.NotifyNpcLearn(OpCodes.Error, "The specified world object cannot be used for knowledge work.", cache);
					return;
				}
				if (knowledgeCapable != null && knowledgeCapable.KnowledgeHolder != null && knowledgeCapable.KnowledgeHolder.Id != cache.NpcProfile.Id)
				{
					sourceEntity.NetworkEntity.PlayerRpcHandler.NotifyNpcLearn(OpCodes.Error, "Validation world object does not match the specified profile!", cache);
					return;
				}
				flag = true;
			}
			if (flag)
			{
				base.UpdateNpcKnowledge(cache, sourceEntity);
				sourceEntity.CollectionController.Record.UpdateQuests(ExternalGameDatabase.Database);
				this.NotifyLearned(cache, sourceEntity);
				sourceEntity.NetworkEntity.PlayerRpcHandler.NotifyNpcLearn(OpCodes.Ok, string.Empty, cache);
				return;
			}
			sourceEntity.NetworkEntity.PlayerRpcHandler.NotifyNpcLearn(OpCodes.Error, "Unable to update knowledge, unknown error!", cache);
		}

		// Token: 0x060028F3 RID: 10483 RVA: 0x0013EDCC File Offset: 0x0013CFCC
		public override void ReissueReward(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			string text = null;
			Quest quest;
			QuestStep questStep;
			RewardChoiceObjective rewardChoiceObjective;
			List<RewardItem> rewards;
			if (InternalGameDatabase.Quests.TryGetItem(cache.QuestId, out quest) && quest.TryGetMostRecentReward(sourceEntity, out questStep, out rewardChoiceObjective, null) && rewardChoiceObjective.Reward.HasReissuableRewardsForEntity(sourceEntity) && rewardChoiceObjective.Reward.TryGetRewards(sourceEntity, cache.RewardChoiceId, out rewards, true) && rewards.EntityCanAcquire(sourceEntity, out text))
			{
				rewardChoiceObjective.Reward.GrantReward(sourceEntity, cache.RewardChoiceId, true);
				return;
			}
			if (text != null && text != string.Empty)
			{
				sourceEntity.NetworkEntity.PlayerRpcHandler.SendChatNotification("Reward could not be reissued: " + text);
			}
		}

		// Token: 0x060028F4 RID: 10484 RVA: 0x0013EE70 File Offset: 0x0013D070
		private void OnNetworkEntityInitialized(NetworkEntity netEntity)
		{
			if (netEntity && netEntity.GameEntity && netEntity.GameEntity.Type == GameEntityType.Player && netEntity.GameEntity.CollectionController != null && netEntity.GameEntity.CollectionController.Record != null && netEntity.GameEntity.CollectionController.Record.Progression != null && netEntity.GameEntity.CollectionController.Record.Progression.Quests != null)
			{
				List<Quest> fromPool = StaticListPool<Quest>.GetFromPool();
				List<UniqueId> fromPool2 = StaticListPool<UniqueId>.GetFromPool();
				foreach (KeyValuePair<UniqueId, QuestProgressionData> keyValuePair in netEntity.GameEntity.CollectionController.Record.Progression.Quests)
				{
					Quest quest;
					if (InternalGameDatabase.Quests.TryGetItem(keyValuePair.Key, out quest))
					{
						if (quest.Enabled)
						{
							QuestStep questStep;
							if (quest.TryGetStep(keyValuePair.Value.CurrentNodeId, out questStep))
							{
								questStep.OnEntityInitialized(netEntity.GameEntity);
							}
							else
							{
								fromPool.Add(quest);
							}
						}
					}
					else
					{
						fromPool2.Add(keyValuePair.Key);
					}
				}
				foreach (Quest quest2 in fromPool)
				{
					netEntity.GameEntity.CollectionController.Record.Progression.Quests.Remove(quest2.Id);
					netEntity.PlayerRpcHandler.NotifyGMQuestReset(quest2.Id);
					netEntity.PlayerRpcHandler.SendChatNotification("Your progress on the quest \"" + quest2.Title + "\" has been reset due to quest design changes by Stormhaven Studios. You may restart the quest at your discretion.");
				}
				foreach (UniqueId uniqueId in fromPool2)
				{
					netEntity.GameEntity.CollectionController.Record.Progression.Quests.Remove(uniqueId);
					netEntity.PlayerRpcHandler.NotifyGMQuestReset(uniqueId);
				}
				if (fromPool.Count > 0)
				{
					netEntity.GameEntity.CollectionController.Record.UpdateQuests(ExternalGameDatabase.Database);
				}
				StaticListPool<Quest>.ReturnToPool(fromPool);
				StaticListPool<UniqueId>.ReturnToPool(fromPool2);
			}
			if (netEntity && netEntity.GameEntity && netEntity.GameEntity.Type == GameEntityType.Player && netEntity.GameEntity.CollectionController != null && netEntity.GameEntity.CollectionController.Record != null && netEntity.GameEntity.CollectionController.Record.Progression != null && netEntity.GameEntity.CollectionController.Record.Progression.BBTasks != null)
			{
				foreach (KeyValuePair<UniqueId, BBTaskProgressionData> keyValuePair2 in netEntity.GameEntity.CollectionController.Record.Progression.BBTasks)
				{
					BBTask bbtask;
					if (InternalGameDatabase.BBTasks.TryGetItem(keyValuePair2.Key, out bbtask) && bbtask.Enabled)
					{
						bbtask.OnEntityInitialized(netEntity.GameEntity);
					}
				}
			}
		}

		// Token: 0x060028F5 RID: 10485 RVA: 0x0013F1F0 File Offset: 0x0013D3F0
		private void OnNetworkEntityDestroyed(NetworkEntity netEntity)
		{
			if (netEntity && netEntity.GameEntity && netEntity.GameEntity.Type == GameEntityType.Player && netEntity.GameEntity.CollectionController != null && netEntity.GameEntity.CollectionController.Record != null && netEntity.GameEntity.CollectionController.Record.Progression != null && netEntity.GameEntity.CollectionController.Record.Progression.Quests != null)
			{
				foreach (KeyValuePair<UniqueId, QuestProgressionData> keyValuePair in netEntity.GameEntity.CollectionController.Record.Progression.Quests)
				{
					Quest quest;
					QuestStep questStep;
					if (InternalGameDatabase.Quests.TryGetItem(keyValuePair.Key, out quest) && quest.Enabled && quest.TryGetStep(keyValuePair.Value.CurrentNodeId, out questStep))
					{
						questStep.OnEntityDestroyed(netEntity.GameEntity);
					}
				}
			}
			if (netEntity && netEntity.GameEntity && netEntity.GameEntity.Type == GameEntityType.Player && netEntity.GameEntity.CollectionController != null && netEntity.GameEntity.CollectionController.Record != null && netEntity.GameEntity.CollectionController.Record.Progression != null && netEntity.GameEntity.CollectionController.Record.Progression.BBTasks != null)
			{
				foreach (KeyValuePair<UniqueId, BBTaskProgressionData> keyValuePair2 in netEntity.GameEntity.CollectionController.Record.Progression.BBTasks)
				{
					BBTask bbtask;
					if (InternalGameDatabase.BBTasks.TryGetItem(keyValuePair2.Key, out bbtask) && bbtask.Enabled)
					{
						bbtask.OnEntityDestroyed(netEntity.GameEntity);
					}
				}
			}
		}

		// Token: 0x04002A00 RID: 10752
		private readonly List<ValueTuple<KillNpcObjective, ObjectiveIterationCache>> m_killStartQuests = new List<ValueTuple<KillNpcObjective, ObjectiveIterationCache>>();
	}
}
