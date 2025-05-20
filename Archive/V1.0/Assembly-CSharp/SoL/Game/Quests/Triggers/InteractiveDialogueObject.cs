using System;
using SoL.Game.Interactives;
using SoL.Game.NPCs;
using SoL.Game.Objects.Archetypes;
using SoL.Game.UI.Dialog;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Quests.Triggers
{
	// Token: 0x02000798 RID: 1944
	public class InteractiveDialogueObject : WorldObject, IInteractive, IInteractiveBase, ICursor, IKnowledgeCapable
	{
		// Token: 0x17000D28 RID: 3368
		// (get) Token: 0x0600395E RID: 14686 RVA: 0x00066D0B File Offset: 0x00064F0B
		public NpcProfile KnowledgeHolder
		{
			get
			{
				return this.m_knowledgeHolder;
			}
		}

		// Token: 0x0600395F RID: 14687 RVA: 0x00172F18 File Offset: 0x00171118
		private void Start()
		{
			if (!GameManager.IsServer && GameManager.QuestManager)
			{
				((ClientQuestManager)GameManager.QuestManager).InkLookupUpdated += this.OnInkLookupUpdated;
			}
			base.gameObject.layer = LayerMap.Interaction.Layer;
		}

		// Token: 0x06003960 RID: 14688 RVA: 0x00066D13 File Offset: 0x00064F13
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (!GameManager.IsServer && GameManager.QuestManager)
			{
				((ClientQuestManager)GameManager.QuestManager).InkLookupUpdated -= this.OnInkLookupUpdated;
			}
		}

		// Token: 0x06003961 RID: 14689 RVA: 0x00066D49 File Offset: 0x00064F49
		private void OnInkLookupUpdated()
		{
			if (this.m_dialogueSource != null)
			{
				this.m_hasDialogue = this.m_dialogueSource.HasAnyDialogue;
				return;
			}
			Debug.LogError("InteractiveDialogueObject is missing its DialogueSource!");
		}

		// Token: 0x17000D29 RID: 3369
		// (get) Token: 0x06003962 RID: 14690 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IInteractive.RequiresLos
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06003963 RID: 14691 RVA: 0x00066D75 File Offset: 0x00064F75
		public bool ClientInteraction()
		{
			if (this.m_dialogueSource != null && LocalPlayer.GameEntity && this.CanInteract(LocalPlayer.GameEntity))
			{
				DialogueManager.InitiateDialogue(this.m_dialogueSource, null, this, DialogSourceType.NPC);
				return true;
			}
			return false;
		}

		// Token: 0x06003964 RID: 14692 RVA: 0x00066DAF File Offset: 0x00064FAF
		public bool CanInteract(GameEntity entity)
		{
			return (GameManager.IsServer || this.m_hasDialogue) && entity != null && this.m_interactionSettings.IsWithinRange(base.gameObject, entity);
		}

		// Token: 0x06003965 RID: 14693 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void BeginInteraction(GameEntity interactionSource)
		{
		}

		// Token: 0x06003966 RID: 14694 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void EndInteraction(GameEntity interactionSource, bool clientIsEnding)
		{
		}

		// Token: 0x06003967 RID: 14695 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void EndAllInteractions()
		{
		}

		// Token: 0x17000D2A RID: 3370
		// (get) Token: 0x06003968 RID: 14696 RVA: 0x00066DDF File Offset: 0x00064FDF
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionSettings;
			}
		}

		// Token: 0x17000D2B RID: 3371
		// (get) Token: 0x06003969 RID: 14697 RVA: 0x00066DE7 File Offset: 0x00064FE7
		protected virtual CursorType ActiveCursorType
		{
			get
			{
				return CursorType.TextCursor;
			}
		}

		// Token: 0x17000D2C RID: 3372
		// (get) Token: 0x0600396A RID: 14698 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual CursorType InactiveCursorType
		{
			get
			{
				return CursorType.MainCursor;
			}
		}

		// Token: 0x17000D2D RID: 3373
		// (get) Token: 0x0600396B RID: 14699 RVA: 0x00066DEB File Offset: 0x00064FEB
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

		// Token: 0x0600396C RID: 14700 RVA: 0x00066E07 File Offset: 0x00065007
		protected override bool Validate(GameEntity entity)
		{
			return base.Validate(entity) && this.CanInteract(entity);
		}

		// Token: 0x0600396E RID: 14702 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003819 RID: 14361
		[SerializeField]
		private InteractionSettings m_interactionSettings;

		// Token: 0x0400381A RID: 14362
		[SerializeField]
		private DialogueSource m_dialogueSource;

		// Token: 0x0400381B RID: 14363
		[SerializeField]
		private NpcProfile m_knowledgeHolder;

		// Token: 0x0400381C RID: 14364
		private bool m_hasDialogue;
	}
}
