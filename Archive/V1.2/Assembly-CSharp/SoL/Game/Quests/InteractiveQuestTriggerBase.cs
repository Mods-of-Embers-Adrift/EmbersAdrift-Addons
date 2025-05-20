using System;
using SoL.Game.Interactives;
using SoL.Game.Quests.Objectives;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Quests
{
	// Token: 0x02000794 RID: 1940
	public abstract class InteractiveQuestTriggerBase : WorldObject, IInteractive, IInteractiveBase, ICursor
	{
		// Token: 0x0600393E RID: 14654 RVA: 0x00172850 File Offset: 0x00170A50
		protected override void Awake()
		{
			base.Awake();
			if (!GameManager.IsServer && GameManager.QuestManager != null)
			{
				GameManager.QuestManager.QuestsUpdated += this.OnQuestsUpdated;
				LocalPlayer.LocalPlayerInitialized += this.OnLocalPlayerInitialized;
			}
		}

		// Token: 0x0600393F RID: 14655 RVA: 0x00066C11 File Offset: 0x00064E11
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (!GameManager.IsServer && GameManager.QuestManager != null)
			{
				GameManager.QuestManager.QuestsUpdated -= this.OnQuestsUpdated;
			}
		}

		// Token: 0x06003940 RID: 14656 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void DoInternalInteraction()
		{
		}

		// Token: 0x06003941 RID: 14657 RVA: 0x001728A0 File Offset: 0x00170AA0
		private void OnLocalPlayerInitialized()
		{
			LocalPlayer.LocalPlayerInitialized -= this.OnLocalPlayerInitialized;
			this.m_isObjectiveActive = this.m_quest.IsObjectiveActive(this.m_objective.Id, null);
			QuestProgressionData questProgressionData;
			ObjectiveProgressionData objectiveProgressionData;
			if (this.m_quest.TryGetProgress(LocalPlayer.GameEntity, out questProgressionData) && questProgressionData.TryGetObjective(this.m_objective.Id, out objectiveProgressionData))
			{
				this.m_isObjectiveComplete = (objectiveProgressionData.IterationsCompleted >= this.m_objective.IterationsRequired);
			}
		}

		// Token: 0x06003942 RID: 14658 RVA: 0x00172920 File Offset: 0x00170B20
		private void OnQuestsUpdated()
		{
			this.m_isObjectiveActive = this.m_quest.IsObjectiveActive(this.m_objective.Id, null);
			QuestProgressionData questProgressionData;
			ObjectiveProgressionData objectiveProgressionData;
			if (this.m_quest.TryGetProgress(LocalPlayer.GameEntity, out questProgressionData) && questProgressionData.TryGetObjective(this.m_objective.Id, out objectiveProgressionData))
			{
				this.m_isObjectiveComplete = (objectiveProgressionData.IterationsCompleted >= this.m_objective.IterationsRequired);
			}
		}

		// Token: 0x17000D21 RID: 3361
		// (get) Token: 0x06003943 RID: 14659 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IInteractive.RequiresLos
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06003944 RID: 14660 RVA: 0x00066C43 File Offset: 0x00064E43
		public bool ClientInteraction()
		{
			if (this.CanInteract(LocalPlayer.GameEntity) && this.m_isObjectiveActive && !this.m_isObjectiveComplete)
			{
				this.DoInternalInteraction();
				return true;
			}
			return false;
		}

		// Token: 0x06003945 RID: 14661 RVA: 0x00066C6B File Offset: 0x00064E6B
		public virtual bool CanInteract(GameEntity entity)
		{
			return entity != null && this.m_interactionDistance.IsWithinRange(base.gameObject, entity);
		}

		// Token: 0x06003946 RID: 14662 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void BeginInteraction(GameEntity interactionSource)
		{
		}

		// Token: 0x06003947 RID: 14663 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void EndInteraction(GameEntity interactionSource, bool clientIsEnding)
		{
		}

		// Token: 0x06003948 RID: 14664 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void EndAllInteractions()
		{
		}

		// Token: 0x17000D22 RID: 3362
		// (get) Token: 0x06003949 RID: 14665 RVA: 0x00061BE2 File Offset: 0x0005FDE2
		protected virtual CursorType ActiveCursorType
		{
			get
			{
				return CursorType.IdentifyingGlassCursor;
			}
		}

		// Token: 0x17000D23 RID: 3363
		// (get) Token: 0x0600394A RID: 14666 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual CursorType InactiveCursorType
		{
			get
			{
				return CursorType.MainCursor;
			}
		}

		// Token: 0x17000D24 RID: 3364
		// (get) Token: 0x0600394B RID: 14667 RVA: 0x00066C8A File Offset: 0x00064E8A
		CursorType ICursor.Type
		{
			get
			{
				if (!this.CanInteract(LocalPlayer.GameEntity) || !this.m_isObjectiveActive || this.m_isObjectiveComplete)
				{
					return this.InactiveCursorType;
				}
				return this.ActiveCursorType;
			}
		}

		// Token: 0x17000D25 RID: 3365
		// (get) Token: 0x0600394C RID: 14668 RVA: 0x00066CB6 File Offset: 0x00064EB6
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionDistance;
			}
		}

		// Token: 0x0600394D RID: 14669 RVA: 0x00066CBE File Offset: 0x00064EBE
		protected override bool Validate(GameEntity entity)
		{
			return base.Validate(entity) && this.CanInteract(entity);
		}

		// Token: 0x0600394F RID: 14671 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x0400380A RID: 14346
		[SerializeField]
		private InteractionSettings m_interactionDistance;

		// Token: 0x0400380B RID: 14347
		[SerializeField]
		protected Quest m_quest;

		// Token: 0x0400380C RID: 14348
		[SerializeField]
		protected WorldObjectQuestObjective m_objective;

		// Token: 0x0400380D RID: 14349
		private bool m_isObjectiveActive;

		// Token: 0x0400380E RID: 14350
		private bool m_isObjectiveComplete;
	}
}
