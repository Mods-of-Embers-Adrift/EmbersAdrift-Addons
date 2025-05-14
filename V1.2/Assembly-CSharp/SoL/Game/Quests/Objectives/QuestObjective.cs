using System;
using SoL.Game.Objects;
using UnityEngine;

namespace SoL.Game.Quests.Objectives
{
	// Token: 0x020007B1 RID: 1969
	public class QuestObjective : Identifiable
	{
		// Token: 0x17000D4B RID: 3403
		// (get) Token: 0x060039E3 RID: 14819 RVA: 0x000673EB File Offset: 0x000655EB
		public string Description
		{
			get
			{
				return this.m_description;
			}
		}

		// Token: 0x17000D4C RID: 3404
		// (get) Token: 0x060039E4 RID: 14820 RVA: 0x000673F3 File Offset: 0x000655F3
		public int IterationsRequired
		{
			get
			{
				return this.m_iterationsRequired;
			}
		}

		// Token: 0x17000D4D RID: 3405
		// (get) Token: 0x060039E5 RID: 14821 RVA: 0x000673FB File Offset: 0x000655FB
		public string DialogueTag
		{
			get
			{
				return this.m_dialogueTag;
			}
		}

		// Token: 0x17000D4E RID: 3406
		// (get) Token: 0x060039E6 RID: 14822 RVA: 0x00067403 File Offset: 0x00065603
		public ObjectiveBehaviorHint Hints
		{
			get
			{
				return this.m_hints;
			}
		}

		// Token: 0x17000D4F RID: 3407
		// (get) Token: 0x060039E7 RID: 14823 RVA: 0x0006740B File Offset: 0x0006560B
		public bool Passive
		{
			get
			{
				if (!this.CanBeActive || !this.CanBePassive)
				{
					return this.CanBePassive;
				}
				return this.m_passive;
			}
		}

		// Token: 0x17000D50 RID: 3408
		// (get) Token: 0x060039E8 RID: 14824 RVA: 0x0006742A File Offset: 0x0006562A
		public QuestObjective ActiveParent
		{
			get
			{
				return this.m_activeParent;
			}
		}

		// Token: 0x17000D51 RID: 3409
		// (get) Token: 0x060039E9 RID: 14825 RVA: 0x00067432 File Offset: 0x00065632
		public bool IsQuiet
		{
			get
			{
				return (this.Hints & ObjectiveBehaviorHint.Quiet) == ObjectiveBehaviorHint.Quiet;
			}
		}

		// Token: 0x060039EA RID: 14826 RVA: 0x0006743F File Offset: 0x0006563F
		public bool IsVisible(GameEntity entity)
		{
			return true && (this.m_hints & ObjectiveBehaviorHint.Hidden) == ObjectiveBehaviorHint.None && (this.m_hiddenBy == null || this.m_hiddenBy.IsComplete(entity));
		}

		// Token: 0x060039EB RID: 14827 RVA: 0x00174D0C File Offset: 0x00172F0C
		public bool IsComplete(GameEntity entity)
		{
			if (entity && entity.CollectionController != null && entity.CollectionController.Record != null && entity.CollectionController.Record.Progression != null && entity.CollectionController.Record.Progression.Quests != null)
			{
				foreach (QuestProgressionData questProgressionData in entity.CollectionController.Record.Progression.Quests.Values)
				{
					if (questProgressionData.Objectives != null)
					{
						foreach (ObjectiveProgressionData objectiveProgressionData in questProgressionData.Objectives)
						{
							if (objectiveProgressionData.ObjectiveId == base.Id && objectiveProgressionData.IterationsCompleted == this.IterationsRequired)
							{
								return true;
							}
						}
					}
				}
			}
			if (entity && entity.CollectionController != null && entity.CollectionController.Record != null && entity.CollectionController.Record.Progression != null && entity.CollectionController.Record.Progression.BBTasks != null)
			{
				foreach (BBTaskProgressionData bbtaskProgressionData in entity.CollectionController.Record.Progression.BBTasks.Values)
				{
					if (bbtaskProgressionData.Objectives != null)
					{
						foreach (ObjectiveProgressionData objectiveProgressionData2 in bbtaskProgressionData.Objectives)
						{
							if (objectiveProgressionData2.ObjectiveId == base.Id && objectiveProgressionData2.IterationsCompleted == this.IterationsRequired)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x060039EC RID: 14828 RVA: 0x00067472 File Offset: 0x00065672
		public int CombinedId(UniqueId parentId)
		{
			return this.m_id.GetHashCode() * 397 ^ parentId.GetHashCode();
		}

		// Token: 0x17000D52 RID: 3410
		// (get) Token: 0x060039ED RID: 14829 RVA: 0x0004479C File Offset: 0x0004299C
		public virtual bool CanBeActive
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000D53 RID: 3411
		// (get) Token: 0x060039EE RID: 14830 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool CanBePassive
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060039EF RID: 14831 RVA: 0x00067499 File Offset: 0x00065699
		public virtual bool Validate(GameEntity sourceEntity, ObjectiveIterationCache cache, out string message)
		{
			message = string.Empty;
			return true;
		}

		// Token: 0x060039F0 RID: 14832 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void OnComplete(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
		}

		// Token: 0x060039F1 RID: 14833 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void OnEnterStep(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
		}

		// Token: 0x060039F2 RID: 14834 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void OnEntityInitializedWhenActive(GameEntity sourceEntity, UniqueId questOrTaskId)
		{
		}

		// Token: 0x060039F3 RID: 14835 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void OnEntityDestroyedWhenActive(GameEntity sourceEntity, UniqueId questOrTaskId)
		{
		}

		// Token: 0x060039F4 RID: 14836 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void OnActivate(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
		}

		// Token: 0x060039F5 RID: 14837 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void OnDeactivate(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
		}

		// Token: 0x060039F6 RID: 14838 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void OnMuteChanged(GameEntity sourceEntity, bool mute)
		{
		}

		// Token: 0x0400387A RID: 14458
		[SerializeField]
		private string m_description;

		// Token: 0x0400387B RID: 14459
		[SerializeField]
		private int m_iterationsRequired = 1;

		// Token: 0x0400387C RID: 14460
		[SerializeField]
		private string m_dialogueTag;

		// Token: 0x0400387D RID: 14461
		[SerializeField]
		private ObjectiveBehaviorHint m_hints;

		// Token: 0x0400387E RID: 14462
		[SerializeField]
		private QuestObjective m_hiddenBy;

		// Token: 0x0400387F RID: 14463
		[SerializeField]
		private bool m_passive;

		// Token: 0x04003880 RID: 14464
		[SerializeField]
		private QuestObjective m_activeParent;
	}
}
