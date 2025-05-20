using System;
using System.Collections.Generic;
using SoL.Game.Objects;
using SoL.Game.Quests.Actions;
using SoL.Game.Quests.Objectives;
using UnityEngine;

namespace SoL.Game.Quests
{
	// Token: 0x0200078F RID: 1935
	public class QuestStep : ScriptableObject
	{
		// Token: 0x17000D0E RID: 3342
		// (get) Token: 0x06003914 RID: 14612 RVA: 0x00066A54 File Offset: 0x00064C54
		private bool m_isStartStep
		{
			get
			{
				Quest quest = this.m_quest;
				return ((quest != null) ? quest.Start : null) != null && this.m_quest.Start.Id == this.Id;
			}
		}

		// Token: 0x17000D0F RID: 3343
		// (get) Token: 0x06003915 RID: 14613 RVA: 0x00066A8D File Offset: 0x00064C8D
		public Quest Quest
		{
			get
			{
				if (this.m_quest == null)
				{
					InternalGameDatabase.Quests.TryGetItem(this.m_questId, out this.m_quest);
				}
				return this.m_quest;
			}
		}

		// Token: 0x17000D10 RID: 3344
		// (get) Token: 0x06003916 RID: 14614 RVA: 0x00066ABA File Offset: 0x00064CBA
		public UniqueId Id
		{
			get
			{
				return this.m_id;
			}
		}

		// Token: 0x17000D11 RID: 3345
		// (get) Token: 0x06003917 RID: 14615 RVA: 0x00066AC2 File Offset: 0x00064CC2
		public string LogEntry
		{
			get
			{
				return this.m_logEntry;
			}
		}

		// Token: 0x17000D12 RID: 3346
		// (get) Token: 0x06003918 RID: 14616 RVA: 0x00066ACA File Offset: 0x00064CCA
		public bool AllowStartHere
		{
			get
			{
				return this.m_allowStartHere;
			}
		}

		// Token: 0x17000D13 RID: 3347
		// (get) Token: 0x06003919 RID: 14617 RVA: 0x00066AD2 File Offset: 0x00064CD2
		public bool HideCompletedObjectives
		{
			get
			{
				return this.m_hideCompletedObjectives;
			}
		}

		// Token: 0x17000D14 RID: 3348
		// (get) Token: 0x0600391A RID: 14618 RVA: 0x00066ADA File Offset: 0x00064CDA
		public bool CombineBranchObjectiveLists
		{
			get
			{
				return this.m_combineBranchObjectiveLists;
			}
		}

		// Token: 0x17000D15 RID: 3349
		// (get) Token: 0x0600391B RID: 14619 RVA: 0x00066AE2 File Offset: 0x00064CE2
		public bool CombineLikeObjectives
		{
			get
			{
				return this.m_combineLikeObjectives;
			}
		}

		// Token: 0x17000D16 RID: 3350
		// (get) Token: 0x0600391C RID: 14620 RVA: 0x00066AEA File Offset: 0x00064CEA
		public List<UniqueId> Next
		{
			get
			{
				return this.m_next;
			}
		}

		// Token: 0x17000D17 RID: 3351
		// (get) Token: 0x0600391D RID: 14621 RVA: 0x00171B1C File Offset: 0x0016FD1C
		public List<QuestStep> NextSteps
		{
			get
			{
				if (this.m_nextSteps == null && this.m_next != null)
				{
					this.m_nextSteps = new List<QuestStep>(this.m_next.Count);
					foreach (UniqueId id in this.m_next)
					{
						QuestStep item;
						if (this.Quest.TryGetStep(id, out item))
						{
							this.m_nextSteps.Add(item);
						}
					}
				}
				return this.m_nextSteps;
			}
		}

		// Token: 0x17000D18 RID: 3352
		// (get) Token: 0x0600391E RID: 14622 RVA: 0x00066AF2 File Offset: 0x00064CF2
		public List<QuestStep> PreviousSteps
		{
			get
			{
				if (this.m_previousSteps == null && this.Quest.GetPrecedingSteps(this.Id).Count > 0)
				{
					this.m_previousSteps = new List<QuestStep>(this.m_previousSteps);
				}
				return this.m_previousSteps;
			}
		}

		// Token: 0x17000D19 RID: 3353
		// (get) Token: 0x0600391F RID: 14623 RVA: 0x00066B2C File Offset: 0x00064D2C
		public List<QuestObjective> Objectives
		{
			get
			{
				return this.m_objectives;
			}
		}

		// Token: 0x17000D1A RID: 3354
		// (get) Token: 0x06003920 RID: 14624 RVA: 0x00066B34 File Offset: 0x00064D34
		public List<InkEntry> InkEntries
		{
			get
			{
				if (!this.m_inkEntriesSet && InternalGameDatabase.Quests.TryGetItem(this.m_questId, out this.m_quest))
				{
					this.OnInkEntriesChanged();
					this.m_inkEntriesSet = true;
				}
				return this.m_inkEntries;
			}
		}

		// Token: 0x17000D1B RID: 3355
		// (get) Token: 0x06003921 RID: 14625 RVA: 0x00066B69 File Offset: 0x00064D69
		public List<QuestAction> ActionsOnEntry
		{
			get
			{
				return this.m_actionsOnEntry;
			}
		}

		// Token: 0x06003922 RID: 14626 RVA: 0x00066B71 File Offset: 0x00064D71
		public QuestStep()
		{
			if (this.m_id == UniqueId.Empty)
			{
				this.m_id = UniqueId.GenerateFromGuid();
			}
		}

		// Token: 0x06003923 RID: 14627 RVA: 0x00171BB0 File Offset: 0x0016FDB0
		public void OnEntityInitialized(GameEntity sourceEntity)
		{
			if (this.NextSteps != null)
			{
				foreach (QuestStep questStep in this.NextSteps)
				{
					if (questStep.Objectives != null)
					{
						foreach (QuestObjective questObjective in questStep.Objectives)
						{
							questObjective.OnEntityInitializedWhenActive(sourceEntity, this.Quest.Id);
						}
					}
				}
			}
		}

		// Token: 0x06003924 RID: 14628 RVA: 0x00171C58 File Offset: 0x0016FE58
		public void OnEntityDestroyed(GameEntity sourceEntity)
		{
			if (this.NextSteps != null)
			{
				foreach (QuestStep questStep in this.NextSteps)
				{
					if (questStep.Objectives != null)
					{
						foreach (QuestObjective questObjective in questStep.Objectives)
						{
							questObjective.OnEntityDestroyedWhenActive(sourceEntity, this.Quest.Id);
						}
					}
				}
			}
		}

		// Token: 0x06003925 RID: 14629 RVA: 0x00171D00 File Offset: 0x0016FF00
		public void OnEntry(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			if (this.m_objectives != null)
			{
				foreach (QuestObjective questObjective in this.m_objectives)
				{
					questObjective.OnEnterStep(cache, sourceEntity);
				}
			}
			if (this.NextSteps != null)
			{
				foreach (QuestStep questStep in this.NextSteps)
				{
					if (questStep.Objectives != null)
					{
						foreach (QuestObjective questObjective2 in questStep.Objectives)
						{
							questObjective2.OnActivate(cache, sourceEntity);
						}
					}
				}
			}
			if (this.m_actionsOnEntry != null)
			{
				foreach (QuestAction questAction in this.m_actionsOnEntry)
				{
					questAction.Execute(cache, sourceEntity);
				}
			}
		}

		// Token: 0x06003926 RID: 14630 RVA: 0x00171E30 File Offset: 0x00170030
		public void OnExit(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			if (this.NextSteps != null)
			{
				foreach (QuestStep questStep in this.NextSteps)
				{
					if (questStep.Objectives != null)
					{
						foreach (QuestObjective questObjective in questStep.Objectives)
						{
							questObjective.OnDeactivate(cache, sourceEntity);
						}
					}
				}
			}
		}

		// Token: 0x06003927 RID: 14631 RVA: 0x00171ED0 File Offset: 0x001700D0
		public void OnMuteChanged(GameEntity sourceEntity, bool mute)
		{
			if (this.NextSteps != null)
			{
				foreach (QuestStep questStep in this.NextSteps)
				{
					if (questStep.Objectives != null)
					{
						foreach (QuestObjective questObjective in questStep.Objectives)
						{
							questObjective.OnMuteChanged(sourceEntity, mute);
						}
					}
				}
			}
		}

		// Token: 0x06003928 RID: 14632 RVA: 0x00171F70 File Offset: 0x00170170
		public bool HasVisibleObjective(GameEntity entity)
		{
			if (this.m_objectives != null)
			{
				using (List<QuestObjective>.Enumerator enumerator = this.m_objectives.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.IsVisible(entity))
						{
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06003929 RID: 14633 RVA: 0x00171FD4 File Offset: 0x001701D4
		public bool ContainsObjective(UniqueId objectiveId)
		{
			if (this.m_objectives != null)
			{
				using (List<QuestObjective>.Enumerator enumerator = this.m_objectives.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Id == objectiveId)
						{
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x0600392A RID: 14634 RVA: 0x0017203C File Offset: 0x0017023C
		public bool TryGetNextById(UniqueId stepId, out QuestStep step)
		{
			if (this.NextSteps != null)
			{
				foreach (QuestStep questStep in this.NextSteps)
				{
					if (questStep.Id == stepId)
					{
						step = questStep;
						return true;
					}
				}
			}
			step = null;
			return false;
		}

		// Token: 0x0600392B RID: 14635 RVA: 0x001720AC File Offset: 0x001702AC
		public bool TryGetNextWithObjective(int objectiveHash, out QuestStep objectiveStep, out QuestObjective objective)
		{
			if (this.NextSteps != null)
			{
				foreach (QuestStep questStep in this.NextSteps)
				{
					if (questStep.Objectives != null)
					{
						foreach (QuestObjective questObjective in questStep.Objectives)
						{
							if (questObjective.CombinedId(questStep.Id) == objectiveHash)
							{
								objectiveStep = questStep;
								objective = questObjective;
								return true;
							}
						}
					}
				}
			}
			objectiveStep = null;
			objective = null;
			return false;
		}

		// Token: 0x0600392C RID: 14636 RVA: 0x0017216C File Offset: 0x0017036C
		public bool TryGetNextWithObjective(UniqueId objectiveId, out QuestStep step, out QuestObjective objective)
		{
			if (this.NextSteps != null)
			{
				foreach (QuestStep questStep in this.NextSteps)
				{
					if (questStep.Objectives != null)
					{
						foreach (QuestObjective questObjective in questStep.Objectives)
						{
							if (questObjective.Id == objectiveId)
							{
								step = questStep;
								objective = questObjective;
								return true;
							}
						}
					}
				}
			}
			step = null;
			objective = null;
			return false;
		}

		// Token: 0x0600392D RID: 14637 RVA: 0x00172228 File Offset: 0x00170428
		public bool TryGetNextObjectiveByDialogueTag<T>(string tag, out QuestStep objectiveStep, out T objectiveByTag) where T : QuestObjective
		{
			if (this.NextSteps != null)
			{
				foreach (QuestStep questStep in this.NextSteps)
				{
					if (questStep.Objectives != null)
					{
						foreach (QuestObjective questObjective in questStep.Objectives)
						{
							if (questObjective.DialogueTag == tag)
							{
								objectiveStep = questStep;
								objectiveByTag = (questObjective as T);
								return true;
							}
						}
					}
				}
			}
			objectiveStep = null;
			objectiveByTag = default(T);
			return false;
		}

		// Token: 0x0600392E RID: 14638 RVA: 0x001722F8 File Offset: 0x001704F8
		public bool TryGetObjectiveByType<T>(out T objectiveByType) where T : QuestObjective
		{
			if (this.m_objectives != null)
			{
				foreach (QuestObjective questObjective in this.m_objectives)
				{
					if (questObjective is T)
					{
						objectiveByType = (questObjective as T);
						return true;
					}
				}
			}
			objectiveByType = default(T);
			return false;
		}

		// Token: 0x0600392F RID: 14639 RVA: 0x00172374 File Offset: 0x00170574
		public bool TryGetNextObjectiveByType<T>(out QuestStep objectiveStep, out T objectiveByType) where T : QuestObjective
		{
			if (this.NextSteps != null)
			{
				foreach (QuestStep questStep in this.NextSteps)
				{
					if (questStep.Objectives != null)
					{
						foreach (QuestObjective questObjective in questStep.Objectives)
						{
							if (questObjective is T)
							{
								objectiveStep = questStep;
								objectiveByType = (questObjective as T);
								return true;
							}
						}
					}
				}
			}
			objectiveStep = null;
			objectiveByType = default(T);
			return false;
		}

		// Token: 0x06003930 RID: 14640 RVA: 0x00172440 File Offset: 0x00170640
		private void OnInkEntriesChanged()
		{
			if (this.m_inkEntries != null)
			{
				foreach (InkEntry inkEntry in this.m_inkEntries)
				{
					inkEntry.Quest = this.Quest;
				}
			}
		}

		// Token: 0x040037E7 RID: 14311
		[SerializeField]
		protected UniqueId m_id = UniqueId.Empty;

		// Token: 0x040037E8 RID: 14312
		[TextArea]
		[SerializeField]
		private string m_logEntry;

		// Token: 0x040037E9 RID: 14313
		[SerializeField]
		private bool m_allowStartHere;

		// Token: 0x040037EA RID: 14314
		[SerializeField]
		private bool m_hideCompletedObjectives;

		// Token: 0x040037EB RID: 14315
		[SerializeField]
		private bool m_combineBranchObjectiveLists;

		// Token: 0x040037EC RID: 14316
		[SerializeField]
		private bool m_combineLikeObjectives;

		// Token: 0x040037ED RID: 14317
		[SerializeField]
		private List<UniqueId> m_next;

		// Token: 0x040037EE RID: 14318
		[SerializeField]
		private List<QuestObjective> m_objectives;

		// Token: 0x040037EF RID: 14319
		[SerializeField]
		private List<InkEntry> m_inkEntries;

		// Token: 0x040037F0 RID: 14320
		[SerializeField]
		private List<QuestAction> m_actionsOnEntry;

		// Token: 0x040037F1 RID: 14321
		[SerializeField]
		private UniqueId m_questId;

		// Token: 0x040037F2 RID: 14322
		[SerializeField]
		private Vector2 m_editorPosition;

		// Token: 0x040037F3 RID: 14323
		private Quest m_quest;

		// Token: 0x040037F4 RID: 14324
		private List<QuestStep> m_nextSteps;

		// Token: 0x040037F5 RID: 14325
		private List<QuestStep> m_previousSteps;

		// Token: 0x040037F6 RID: 14326
		private bool m_inkEntriesSet;
	}
}
