using System;
using SoL.Game.Interactives;
using SoL.Game.NPCs;
using SoL.Game.Quests.Objectives;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Quests.Triggers
{
	// Token: 0x02000799 RID: 1945
	public class InteractiveProgressionTriggerClick : WorldObject, IInteractive, IInteractiveBase, ICursor, IKnowledgeCapable
	{
		// Token: 0x17000D2E RID: 3374
		// (get) Token: 0x0600396F RID: 14703 RVA: 0x00066E1B File Offset: 0x0006501B
		public NpcProfile KnowledgeHolder
		{
			get
			{
				return this.m_knowledgeHolder;
			}
		}

		// Token: 0x17000D2F RID: 3375
		// (get) Token: 0x06003970 RID: 14704 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IInteractive.RequiresLos
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06003971 RID: 14705 RVA: 0x00066E23 File Offset: 0x00065023
		public bool CanInteract(GameEntity entity)
		{
			return entity != null && this.m_interactionSettings.IsWithinRange(base.gameObject, entity) && (this.m_progressionRequirement == null || this.m_progressionRequirement.MeetsAllRequirements(entity));
		}

		// Token: 0x06003972 RID: 14706 RVA: 0x00172F70 File Offset: 0x00171170
		public bool ClientInteraction()
		{
			bool result = false;
			int hash;
			if (this.m_quest.TryGetObjectiveHashForActiveObjective(this.m_objective.Id, out hash) || this.m_triggerIfQuestNotPresent)
			{
				GameManager.QuestManager.Progress(new ObjectiveIterationCache
				{
					QuestId = this.m_quest.Id,
					ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(hash),
					WorldId = base.WorldId,
					StartQuestIfNotPresent = this.m_triggerIfQuestNotPresent
				}, null, false);
				result = true;
			}
			int knowledgeIndex;
			if (this.m_knowledgeHolder != null && this.m_knowledgeHolder.TryGetKnowledgeIndexByLabel(this.m_knowledgeLabel, out knowledgeIndex))
			{
				GameManager.QuestManager.Learn(new NpcLearningCache
				{
					NpcProfile = this.m_knowledgeHolder,
					KnowledgeIndex = knowledgeIndex,
					WorldId = base.WorldId
				}, null);
				result = true;
			}
			return result;
		}

		// Token: 0x06003973 RID: 14707 RVA: 0x0004475B File Offset: 0x0004295B
		public void BeginInteraction(GameEntity interactionSource)
		{
		}

		// Token: 0x06003974 RID: 14708 RVA: 0x0004475B File Offset: 0x0004295B
		public void EndInteraction(GameEntity interactionSource, bool clientIsEnding)
		{
		}

		// Token: 0x06003975 RID: 14709 RVA: 0x0004475B File Offset: 0x0004295B
		public void EndAllInteractions()
		{
		}

		// Token: 0x17000D30 RID: 3376
		// (get) Token: 0x06003976 RID: 14710 RVA: 0x00066E5A File Offset: 0x0006505A
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionSettings;
			}
		}

		// Token: 0x17000D31 RID: 3377
		// (get) Token: 0x06003977 RID: 14711 RVA: 0x00066DE7 File Offset: 0x00064FE7
		protected virtual CursorType ActiveCursorType
		{
			get
			{
				return CursorType.TextCursor;
			}
		}

		// Token: 0x17000D32 RID: 3378
		// (get) Token: 0x06003978 RID: 14712 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual CursorType InactiveCursorType
		{
			get
			{
				return CursorType.MainCursor;
			}
		}

		// Token: 0x17000D33 RID: 3379
		// (get) Token: 0x06003979 RID: 14713 RVA: 0x00066E62 File Offset: 0x00065062
		CursorType ICursor.Type
		{
			get
			{
				if (!this.CanInteract(LocalPlayer.GameEntity))
				{
					return this.InactiveCursorType;
				}
				return this.ActiveCursorType;
			}
		}

		// Token: 0x0600397A RID: 14714 RVA: 0x00066E7E File Offset: 0x0006507E
		protected override bool Validate(GameEntity entity)
		{
			return base.Validate(entity) && this.CanInteract(entity);
		}

		// Token: 0x0600397C RID: 14716 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x0400381D RID: 14365
		private const string kGroupQuest = "Quest Progression";

		// Token: 0x0400381E RID: 14366
		private const string kGroupKnowledge = "Knowledge Progression";

		// Token: 0x0400381F RID: 14367
		[SerializeField]
		private Quest m_quest;

		// Token: 0x04003820 RID: 14368
		[SerializeField]
		private WorldObjectQuestObjective m_objective;

		// Token: 0x04003821 RID: 14369
		[SerializeField]
		private bool m_triggerIfQuestNotPresent;

		// Token: 0x04003822 RID: 14370
		[SerializeField]
		private string m_knowledgeLabel;

		// Token: 0x04003823 RID: 14371
		[SerializeField]
		private NpcProfile m_knowledgeHolder;

		// Token: 0x04003824 RID: 14372
		[SerializeField]
		private ProgressionRequirement m_progressionRequirement;

		// Token: 0x04003825 RID: 14373
		[SerializeField]
		private InteractionSettings m_interactionSettings;
	}
}
