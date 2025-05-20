using System;
using System.Collections.Generic;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Quests.Objectives;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Utilities;
using SoL.Utilities.NodeGraph;
using UnityEngine;

namespace SoL.Game.Quests
{
	// Token: 0x0200078A RID: 1930
	[CreateAssetMenu(menuName = "SoL/Quests/Quest")]
	public class Quest : Identifiable
	{
		// Token: 0x17000D00 RID: 3328
		// (get) Token: 0x060038E4 RID: 14564 RVA: 0x00066938 File Offset: 0x00064B38
		public string Title
		{
			get
			{
				return this.m_title;
			}
		}

		// Token: 0x17000D01 RID: 3329
		// (get) Token: 0x060038E5 RID: 14565 RVA: 0x00066940 File Offset: 0x00064B40
		public string DialogueTag
		{
			get
			{
				return this.m_dialogueTag;
			}
		}

		// Token: 0x17000D02 RID: 3330
		// (get) Token: 0x060038E6 RID: 14566 RVA: 0x00066948 File Offset: 0x00064B48
		public string DialogueChoicePrompt
		{
			get
			{
				return this.m_dialogueChoicePrompt;
			}
		}

		// Token: 0x17000D03 RID: 3331
		// (get) Token: 0x060038E7 RID: 14567 RVA: 0x00066950 File Offset: 0x00064B50
		public bool Enabled
		{
			get
			{
				return this.m_enabled.HasBitFlag(DeploymentBranchFlagsExtensions.GetBranchFlags()) && !this.m_gmOnly;
			}
		}

		// Token: 0x17000D04 RID: 3332
		// (get) Token: 0x060038E8 RID: 14568 RVA: 0x0006696F File Offset: 0x00064B6F
		public ObjectiveBehaviorHint StartHints
		{
			get
			{
				return this.m_startHints;
			}
		}

		// Token: 0x17000D05 RID: 3333
		// (get) Token: 0x060038E9 RID: 14569 RVA: 0x00066977 File Offset: 0x00064B77
		public ProgressionRequirement Requirements
		{
			get
			{
				return this.m_progressionRequirements;
			}
		}

		// Token: 0x17000D06 RID: 3334
		// (get) Token: 0x060038EA RID: 14570 RVA: 0x0006697F File Offset: 0x00064B7F
		public BaseArchetype AssociatedMastery
		{
			get
			{
				return this.m_associatedMastery;
			}
		}

		// Token: 0x17000D07 RID: 3335
		// (get) Token: 0x060038EB RID: 14571 RVA: 0x00066987 File Offset: 0x00064B87
		public bool HideCompletedObjectives
		{
			get
			{
				return this.m_hideCompletedObjectives;
			}
		}

		// Token: 0x17000D08 RID: 3336
		// (get) Token: 0x060038EC RID: 14572 RVA: 0x0006698F File Offset: 0x00064B8F
		public QuestStep Start
		{
			get
			{
				return this.m_start;
			}
		}

		// Token: 0x17000D09 RID: 3337
		// (get) Token: 0x060038ED RID: 14573 RVA: 0x00066997 File Offset: 0x00064B97
		public List<QuestStep> Steps
		{
			get
			{
				return this.m_steps;
			}
		}

		// Token: 0x060038EE RID: 14574 RVA: 0x00170E5C File Offset: 0x0016F05C
		public bool TryGetStep(UniqueId id, out QuestStep value)
		{
			if (id.IsEmpty)
			{
				value = null;
				return false;
			}
			if (this.m_stepsLookup == null)
			{
				this.m_stepsLookup = new Dictionary<UniqueId, QuestStep>();
				foreach (QuestStep questStep in this.m_steps)
				{
					this.m_stepsLookup.Add(questStep.Id, questStep);
				}
			}
			return this.m_stepsLookup.TryGetValue(id, out value);
		}

		// Token: 0x060038EF RID: 14575 RVA: 0x00170EE8 File Offset: 0x0016F0E8
		public bool TryGetObjectiveUsage(int hash, out QuestStep step, out QuestObjective objective)
		{
			if (this.m_objectivesLookup == null)
			{
				this.m_objectivesLookup = new Dictionary<int, ValueTuple<QuestStep, QuestObjective>>();
				foreach (QuestStep questStep in this.m_steps)
				{
					foreach (QuestObjective questObjective in questStep.Objectives)
					{
						this.m_objectivesLookup.Add(questObjective.CombinedId(questStep.Id), new ValueTuple<QuestStep, QuestObjective>(questStep, questObjective));
					}
				}
			}
			ValueTuple<QuestStep, QuestObjective> valueTuple;
			if (this.m_objectivesLookup.TryGetValue(hash, out valueTuple))
			{
				step = valueTuple.Item1;
				objective = valueTuple.Item2;
				return true;
			}
			step = null;
			objective = null;
			return false;
		}

		// Token: 0x17000D0A RID: 3338
		// (get) Token: 0x060038F0 RID: 14576 RVA: 0x00170FD0 File Offset: 0x0016F1D0
		public List<DialogueSource> AffectedSources
		{
			get
			{
				if (this.m_affectedSources == null)
				{
					this.m_affectedSources = new List<DialogueSource>();
					foreach (QuestStep questStep in this.m_steps)
					{
						foreach (InkEntry inkEntry in questStep.InkEntries)
						{
							if (!this.m_affectedSources.Contains(inkEntry.Source))
							{
								this.m_affectedSources.Add(inkEntry.Source);
							}
						}
					}
				}
				return this.m_affectedSources;
			}
		}

		// Token: 0x060038F1 RID: 14577 RVA: 0x00171098 File Offset: 0x0016F298
		public bool TryGetStartByObjectiveId(UniqueId objectiveId, out QuestStep objectiveStep, out QuestObjective objective)
		{
			foreach (QuestStep questStep in this.m_steps)
			{
				if (questStep.AllowStartHere)
				{
					foreach (QuestObjective questObjective in questStep.Objectives)
					{
						if (questObjective.Id == objectiveId)
						{
							objectiveStep = questStep;
							objective = questObjective;
							return true;
						}
					}
				}
			}
			objectiveStep = null;
			objective = null;
			return false;
		}

		// Token: 0x060038F2 RID: 14578 RVA: 0x0017114C File Offset: 0x0016F34C
		public bool TryGetStartByObjectiveTag(string tag, out QuestStep objectiveStep, out QuestObjective objective)
		{
			foreach (QuestStep questStep in this.m_steps)
			{
				if (questStep.Id == this.Start.Id || questStep.AllowStartHere)
				{
					foreach (QuestObjective questObjective in questStep.Objectives)
					{
						if (questObjective.DialogueTag == tag)
						{
							objectiveStep = questStep;
							objective = questObjective;
							return true;
						}
					}
				}
			}
			objectiveStep = null;
			objective = null;
			return false;
		}

		// Token: 0x060038F3 RID: 14579 RVA: 0x00171218 File Offset: 0x0016F418
		public List<QuestStep> GetPrecedingSteps(UniqueId stepId)
		{
			this.m_tempPrecedingSteps.Clear();
			if (stepId.IsEmpty)
			{
				return this.m_tempPrecedingSteps;
			}
			foreach (QuestStep questStep in this.m_steps)
			{
				if (questStep.Next.Contains(stepId))
				{
					this.m_tempPrecedingSteps.Add(questStep);
				}
			}
			return this.m_tempPrecedingSteps;
		}

		// Token: 0x060038F4 RID: 14580 RVA: 0x0006699F File Offset: 0x00064B9F
		public bool CanStartQuest(GameEntity entity = null)
		{
			if (!this.Enabled)
			{
				return false;
			}
			if (!GameManager.IsServer)
			{
				entity = LocalPlayer.GameEntity;
			}
			if (!this.IsOnQuest(entity))
			{
				ProgressionRequirement progressionRequirements = this.m_progressionRequirements;
				return progressionRequirements == null || progressionRequirements.MeetsAllRequirements(entity);
			}
			return false;
		}

		// Token: 0x060038F5 RID: 14581 RVA: 0x001712A0 File Offset: 0x0016F4A0
		public bool IsOnQuest(GameEntity entity = null)
		{
			if (!this.Enabled)
			{
				return false;
			}
			if (!GameManager.IsServer)
			{
				entity = LocalPlayer.GameEntity;
			}
			if (entity)
			{
				ICollectionController collectionController = entity.CollectionController;
				bool? flag;
				if (collectionController == null)
				{
					flag = null;
				}
				else
				{
					CharacterRecord record = collectionController.Record;
					if (record == null)
					{
						flag = null;
					}
					else
					{
						PlayerProgressionData progression = record.Progression;
						if (progression == null)
						{
							flag = null;
						}
						else
						{
							Dictionary<UniqueId, QuestProgressionData> quests = progression.Quests;
							flag = ((quests != null) ? new bool?(quests.ContainsKey(base.Id)) : null);
						}
					}
				}
				bool? flag2 = flag;
				return flag2.GetValueOrDefault();
			}
			return false;
		}

		// Token: 0x060038F6 RID: 14582 RVA: 0x000669D6 File Offset: 0x00064BD6
		public bool TryGetProgress(out QuestProgressionData progress)
		{
			if (GameManager.IsServer)
			{
				throw new InvalidOperationException();
			}
			return this.TryGetProgress(LocalPlayer.GameEntity, out progress);
		}

		// Token: 0x060038F7 RID: 14583 RVA: 0x00171338 File Offset: 0x0016F538
		public bool TryGetProgress(GameEntity entity, out QuestProgressionData progress)
		{
			if (!GameManager.IsServer)
			{
				entity = LocalPlayer.GameEntity;
			}
			progress = null;
			return entity && entity.CollectionController != null && entity.CollectionController.Record != null && entity.CollectionController.Record.Progression != null && entity.CollectionController.Record.Progression.Quests != null && entity.CollectionController.Record.Progression.Quests.TryGetValue(base.Id, out progress);
		}

		// Token: 0x060038F8 RID: 14584 RVA: 0x000669F1 File Offset: 0x00064BF1
		public bool TryGetObjectiveHashForActiveObjective(UniqueId objectiveId, out int objectiveHash)
		{
			if (GameManager.IsServer)
			{
				throw new InvalidOperationException();
			}
			return this.TryGetObjectiveHashForActiveObjective(objectiveId, LocalPlayer.GameEntity, out objectiveHash);
		}

		// Token: 0x060038F9 RID: 14585 RVA: 0x001713C0 File Offset: 0x0016F5C0
		public bool TryGetObjectiveHashForActiveObjective(UniqueId objectiveId, GameEntity entity, out int objectiveHash)
		{
			objectiveHash = 0;
			if (objectiveId.IsEmpty)
			{
				return false;
			}
			if (!GameManager.IsServer)
			{
				entity = LocalPlayer.GameEntity;
			}
			QuestProgressionData questProgressionData;
			QuestStep questStep;
			QuestStep questStep2;
			QuestObjective questObjective;
			if (this.TryGetProgress(entity, out questProgressionData) && this.TryGetStep(questProgressionData.CurrentNodeId, out questStep) && questStep.TryGetNextWithObjective(objectiveId, out questStep2, out questObjective))
			{
				objectiveHash = questObjective.CombinedId(questStep2.Id);
				return true;
			}
			return false;
		}

		// Token: 0x060038FA RID: 14586 RVA: 0x00171424 File Offset: 0x0016F624
		public bool TryGetKillStartObjective(out List<ValueTuple<QuestStep, KillNpcObjective>> killStarts)
		{
			killStarts = null;
			foreach (QuestStep questStep in this.m_steps)
			{
				KillNpcObjective item;
				if ((questStep == this.Start || questStep.AllowStartHere) && questStep.TryGetObjectiveByType<KillNpcObjective>(out item))
				{
					if (killStarts == null)
					{
						killStarts = StaticListPool<ValueTuple<QuestStep, KillNpcObjective>>.GetFromPool();
					}
					killStarts.Add(new ValueTuple<QuestStep, KillNpcObjective>(questStep, item));
				}
			}
			return killStarts != null;
		}

		// Token: 0x060038FB RID: 14587 RVA: 0x001714B4 File Offset: 0x0016F6B4
		public bool TryGetMostRecentReward(GameEntity entity, out QuestStep step, out RewardChoiceObjective objective, QuestStep searchStep = null)
		{
			if (!GameManager.IsServer)
			{
				entity = LocalPlayer.GameEntity;
			}
			if (this.m_loopProtectionScratch == null)
			{
				this.m_loopProtectionScratch = new HashSet<UniqueId>(this.m_steps.Count);
			}
			else if (searchStep == null)
			{
				this.m_loopProtectionScratch.Clear();
			}
			step = null;
			objective = null;
			QuestProgressionData questProgressionData;
			if (searchStep == null && (!this.TryGetProgress(entity, out questProgressionData) || !this.TryGetStep(questProgressionData.CurrentNodeId, out searchStep)))
			{
				return false;
			}
			RewardChoiceObjective rewardChoiceObjective;
			if (searchStep.TryGetObjectiveByType<RewardChoiceObjective>(out rewardChoiceObjective))
			{
				step = searchStep;
				objective = rewardChoiceObjective;
				return true;
			}
			if (searchStep.PreviousSteps.Count == 1 && !this.m_loopProtectionScratch.Contains(searchStep.PreviousSteps[0].Id))
			{
				this.m_loopProtectionScratch.Add(searchStep.Id);
				return this.TryGetMostRecentReward(entity, out step, out objective, searchStep.PreviousSteps[0]);
			}
			return false;
		}

		// Token: 0x060038FC RID: 14588 RVA: 0x001715A0 File Offset: 0x0016F7A0
		public bool IsObjectiveActive(int objectiveHash, GameEntity entity = null)
		{
			if (objectiveHash == 0)
			{
				return false;
			}
			if (!GameManager.IsServer)
			{
				entity = LocalPlayer.GameEntity;
			}
			QuestProgressionData questProgressionData;
			QuestStep questStep;
			QuestStep questStep2;
			QuestObjective questObjective;
			return this.TryGetProgress(entity, out questProgressionData) && this.TryGetStep(questProgressionData.CurrentNodeId, out questStep) && questStep.TryGetNextWithObjective(objectiveHash, out questStep2, out questObjective);
		}

		// Token: 0x060038FD RID: 14589 RVA: 0x001715EC File Offset: 0x0016F7EC
		public bool IsObjectiveActive(UniqueId objectiveId, GameEntity entity = null)
		{
			if (objectiveId.IsEmpty)
			{
				return false;
			}
			if (!GameManager.IsServer)
			{
				entity = LocalPlayer.GameEntity;
			}
			QuestProgressionData questProgressionData;
			QuestStep questStep;
			QuestStep questStep2;
			QuestObjective questObjective;
			return this.TryGetProgress(entity, out questProgressionData) && this.TryGetStep(questProgressionData.CurrentNodeId, out questStep) && questStep.TryGetNextWithObjective(objectiveId, out questStep2, out questObjective);
		}

		// Token: 0x060038FE RID: 14590 RVA: 0x00171640 File Offset: 0x0016F840
		public unsafe bool AreObjectivesActive(ReadOnlySpan<int> objectiveHashes, GameEntity entity = null)
		{
			if (objectiveHashes == null || objectiveHashes.Length == 0)
			{
				return false;
			}
			if (!GameManager.IsServer)
			{
				entity = LocalPlayer.GameEntity;
			}
			bool flag = true;
			ReadOnlySpan<int> readOnlySpan = objectiveHashes;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				int objectiveHash = *readOnlySpan[i];
				QuestProgressionData questProgressionData;
				QuestStep questStep;
				QuestStep questStep2;
				QuestObjective questObjective;
				flag = (flag && this.TryGetProgress(entity, out questProgressionData) && this.TryGetStep(questProgressionData.CurrentNodeId, out questStep) && questStep.TryGetNextWithObjective(objectiveHash, out questStep2, out questObjective));
			}
			return flag;
		}

		// Token: 0x060038FF RID: 14591 RVA: 0x001716C4 File Offset: 0x0016F8C4
		public bool IsComplete(GameEntity entity = null)
		{
			if (!GameManager.IsServer)
			{
				entity = LocalPlayer.GameEntity;
			}
			QuestProgressionData questProgressionData;
			QuestStep questStep;
			return this.TryGetProgress(entity, out questProgressionData) && this.TryGetStep(questProgressionData.CurrentNodeId, out questStep) && questStep.Next.Count == 0;
		}

		// Token: 0x06003900 RID: 14592 RVA: 0x0017170C File Offset: 0x0016F90C
		public void Mute(bool mute, GameEntity entity = null)
		{
			if (!GameManager.IsServer)
			{
				entity = LocalPlayer.GameEntity;
			}
			QuestProgressionData questProgressionData;
			QuestStep questStep;
			if (this.TryGetProgress(entity, out questProgressionData) && this.TryGetStep(questProgressionData.CurrentNodeId, out questStep))
			{
				bool muted = questProgressionData.Muted;
				questProgressionData.Muted = mute;
				if (muted != mute)
				{
					questStep.OnMuteChanged(entity, mute);
				}
			}
		}

		// Token: 0x06003901 RID: 14593 RVA: 0x0017175C File Offset: 0x0016F95C
		public bool IsMuted(GameEntity entity = null)
		{
			if (!GameManager.IsServer)
			{
				entity = LocalPlayer.GameEntity;
			}
			QuestProgressionData questProgressionData;
			return this.TryGetProgress(entity, out questProgressionData) && questProgressionData.Muted;
		}

		// Token: 0x06003902 RID: 14594 RVA: 0x00066719 File Offset: 0x00064919
		private bool ValidateMastery(BaseArchetype archetype, ref string errorMessage)
		{
			if (archetype != null && !(archetype is MasteryArchetype) && !(archetype is SpecializedRole))
			{
				errorMessage = "Associated Mastery must be a valid MasteryArchetype or SpecializedRole!";
				return false;
			}
			return true;
		}

		// Token: 0x040037C9 RID: 14281
		[SerializeField]
		private string m_title;

		// Token: 0x040037CA RID: 14282
		[SerializeField]
		private string m_dialogueTag;

		// Token: 0x040037CB RID: 14283
		[SerializeField]
		private string m_dialogueChoicePrompt;

		// Token: 0x040037CC RID: 14284
		[SerializeField]
		private DeploymentBranchFlags m_enabled = DeploymentBranchFlags.DEV | DeploymentBranchFlags.QA | DeploymentBranchFlags.LIVE;

		// Token: 0x040037CD RID: 14285
		[SerializeField]
		private bool m_gmOnly;

		// Token: 0x040037CE RID: 14286
		[SerializeField]
		private ObjectiveBehaviorHint m_startHints;

		// Token: 0x040037CF RID: 14287
		[SerializeField]
		private ProgressionRequirement m_progressionRequirements;

		// Token: 0x040037D0 RID: 14288
		[SerializeField]
		private BaseArchetype m_associatedMastery;

		// Token: 0x040037D1 RID: 14289
		[SerializeField]
		private bool m_hideCompletedObjectives;

		// Token: 0x040037D2 RID: 14290
		[SerializeField]
		private QuestStep m_start;

		// Token: 0x040037D3 RID: 14291
		[SerializeField]
		private List<QuestStep> m_steps;

		// Token: 0x040037D4 RID: 14292
		[SerializeField]
		private List<CommentBlockData> m_commentBlocks;

		// Token: 0x040037D5 RID: 14293
		private Dictionary<UniqueId, QuestStep> m_stepsLookup;

		// Token: 0x040037D6 RID: 14294
		private Dictionary<int, ValueTuple<QuestStep, QuestObjective>> m_objectivesLookup;

		// Token: 0x040037D7 RID: 14295
		private List<DialogueSource> m_affectedSources;

		// Token: 0x040037D8 RID: 14296
		private List<QuestStep> m_tempPrecedingSteps = new List<QuestStep>();

		// Token: 0x040037D9 RID: 14297
		private HashSet<UniqueId> m_loopProtectionScratch;
	}
}
