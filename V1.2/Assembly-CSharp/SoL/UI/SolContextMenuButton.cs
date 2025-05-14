using System;
using UnityEngine.EventSystems;

namespace SoL.UI
{
	// Token: 0x02000371 RID: 881
	public class SolContextMenuButton : SolButton
	{
		// Token: 0x170005C4 RID: 1476
		// (get) Token: 0x06001822 RID: 6178 RVA: 0x00052EA7 File Offset: 0x000510A7
		// (set) Token: 0x06001823 RID: 6179 RVA: 0x00052EAF File Offset: 0x000510AF
		public bool CursorInside { get; private set; }

		// Token: 0x170005C5 RID: 1477
		// (get) Token: 0x06001824 RID: 6180 RVA: 0x00052EB8 File Offset: 0x000510B8
		private bool HasNestedActions
		{
			get
			{
				return this.m_contextMenu != null && this.m_currentAction != null && this.m_currentAction.NestedActions != null && this.m_currentAction.NestedActions.Count > 0;
			}
		}

		// Token: 0x06001825 RID: 6181 RVA: 0x00052EF2 File Offset: 0x000510F2
		public void SetContextMenu(ContextMenuUI menu)
		{
			this.m_contextMenu = menu;
		}

		// Token: 0x06001826 RID: 6182 RVA: 0x00052EFB File Offset: 0x000510FB
		public void SetCurrentAction(ContextMenuAction action)
		{
			this.m_currentAction = action;
			base.text = action.Text;
			base.interactable = action.Enabled;
		}

		// Token: 0x06001827 RID: 6183 RVA: 0x00052F1C File Offset: 0x0005111C
		internal void UpdateInteractive()
		{
			if (this.m_currentAction != null && this.m_currentAction.InteractiveCheck != null)
			{
				base.interactable = this.m_currentAction.InteractiveCheck();
			}
		}

		// Token: 0x06001828 RID: 6184 RVA: 0x001038F8 File Offset: 0x00101AF8
		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);
			if (eventData.button != PointerEventData.InputButton.Left || this.HasNestedActions || !base.interactable)
			{
				return;
			}
			Action callback = this.m_currentAction.Callback;
			if (callback != null)
			{
				callback();
			}
			this.m_contextMenu.OptionSelected();
		}

		// Token: 0x06001829 RID: 6185 RVA: 0x00103948 File Offset: 0x00101B48
		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			this.CursorInside = true;
			base.UnderlineText();
			base.BoldText();
			if (this.HasNestedActions && this.m_currentAction.Enabled)
			{
				this.m_contextMenu.InitNested(this, this.m_currentAction.NestedActions);
			}
		}

		// Token: 0x0600182A RID: 6186 RVA: 0x00052F49 File Offset: 0x00051149
		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			this.CursorInside = false;
			base.RemoveUnderlineText();
			base.RemoveBoldText();
		}

		// Token: 0x04001F8A RID: 8074
		private ContextMenuUI m_contextMenu;

		// Token: 0x04001F8B RID: 8075
		private ContextMenuAction m_currentAction;
	}
}
