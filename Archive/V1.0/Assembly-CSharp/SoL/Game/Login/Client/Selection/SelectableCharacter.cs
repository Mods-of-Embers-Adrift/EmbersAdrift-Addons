using System;
using SoL.Game.Interactives;
using SoL.Networking.Database;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Login.Client.Selection
{
	// Token: 0x02000B43 RID: 2883
	public class SelectableCharacter : MonoBehaviour, IInteractive, IInteractiveBase, ICursor, IHighlight, IContextMenu
	{
		// Token: 0x060058A5 RID: 22693 RVA: 0x0007B484 File Offset: 0x00079684
		public void Init(CharacterRecord record, SelectionDirector director)
		{
			this.m_record = record;
			this.m_director = director;
		}

		// Token: 0x170014C1 RID: 5313
		// (get) Token: 0x060058A6 RID: 22694 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IInteractive.RequiresLos
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170014C2 RID: 5314
		// (get) Token: 0x060058A7 RID: 22695 RVA: 0x0007B494 File Offset: 0x00079694
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionDistance;
			}
		}

		// Token: 0x060058A8 RID: 22696 RVA: 0x0007B49C File Offset: 0x0007969C
		bool IInteractive.ClientInteraction()
		{
			if (this.m_record != null && this.m_director != null)
			{
				this.m_director.SelectCharacterByCharacterRecord(this.m_record);
				return true;
			}
			return false;
		}

		// Token: 0x060058A9 RID: 22697 RVA: 0x0007B4C9 File Offset: 0x000796C9
		bool IInteractive.CanInteract(GameEntity entity)
		{
			return this.m_record != null;
		}

		// Token: 0x060058AA RID: 22698 RVA: 0x0004475B File Offset: 0x0004295B
		void IInteractive.BeginInteraction(GameEntity interactionSource)
		{
		}

		// Token: 0x060058AB RID: 22699 RVA: 0x0004475B File Offset: 0x0004295B
		void IInteractive.EndInteraction(GameEntity interactionSource, bool clientIsEnding)
		{
		}

		// Token: 0x060058AC RID: 22700 RVA: 0x0004475B File Offset: 0x0004295B
		void IInteractive.EndAllInteractions()
		{
		}

		// Token: 0x170014C3 RID: 5315
		// (get) Token: 0x060058AD RID: 22701 RVA: 0x00070E66 File Offset: 0x0006F066
		CursorType ICursor.Type
		{
			get
			{
				return CursorType.GloveCursor;
			}
		}

		// Token: 0x060058AE RID: 22702 RVA: 0x0007B4D4 File Offset: 0x000796D4
		string IContextMenu.FillActionsGetTitle()
		{
			if (this.m_record == null || this.m_director == null)
			{
				return null;
			}
			return this.m_director.ContextMenuForCharacter(this.m_record, true);
		}

		// Token: 0x170014C4 RID: 5316
		// (get) Token: 0x060058AF RID: 22703 RVA: 0x0007B500 File Offset: 0x00079700
		// (set) Token: 0x060058B0 RID: 22704 RVA: 0x0007B51D File Offset: 0x0007971D
		bool IHighlight.HighlightEnabled
		{
			get
			{
				return this.m_highlightController != null && this.m_highlightController.HighlightEnabled;
			}
			set
			{
				if (this.m_highlightController != null)
				{
					this.m_highlightController.HighlightEnabled = value;
				}
			}
		}

		// Token: 0x060058B2 RID: 22706 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004E0F RID: 19983
		[SerializeField]
		private InteractionSettings m_interactionDistance;

		// Token: 0x04004E10 RID: 19984
		[SerializeField]
		private HighlightController m_highlightController;

		// Token: 0x04004E11 RID: 19985
		private CharacterRecord m_record;

		// Token: 0x04004E12 RID: 19986
		private SelectionDirector m_director;
	}
}
