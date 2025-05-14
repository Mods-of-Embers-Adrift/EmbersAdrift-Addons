using System;
using SoL.Managers;
using SoL.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x020008B8 RID: 2232
	public class MoveUiWindow : MonoBehaviour
	{
		// Token: 0x06004162 RID: 16738 RVA: 0x0018F304 File Offset: 0x0018D504
		private void Awake()
		{
			if (!this.m_dragImage || !this.m_window)
			{
				base.enabled = false;
				return;
			}
			this.m_window.PreventReparentOnDrag = true;
			InputManager.MoveUIHeldChanged += this.InputManagerOnMoveUIHeldChanged;
			if (this.m_resetButton)
			{
				this.m_resetButton.onClick.AddListener(new UnityAction(this.ResetButtonClicked));
			}
			this.ToggleMovability(false);
		}

		// Token: 0x06004163 RID: 16739 RVA: 0x0006C27B File Offset: 0x0006A47B
		private void OnDestroy()
		{
			InputManager.MoveUIHeldChanged -= this.InputManagerOnMoveUIHeldChanged;
			if (this.m_resetButton)
			{
				this.m_resetButton.onClick.RemoveListener(new UnityAction(this.ResetButtonClicked));
			}
		}

		// Token: 0x06004164 RID: 16740 RVA: 0x0006C2B7 File Offset: 0x0006A4B7
		private void ResetButtonClicked()
		{
			if (this.m_window)
			{
				this.m_window.ResetPosition();
			}
		}

		// Token: 0x06004165 RID: 16741 RVA: 0x0006C2D1 File Offset: 0x0006A4D1
		private void InputManagerOnMoveUIHeldChanged(bool obj)
		{
			this.ToggleMovability(obj);
		}

		// Token: 0x06004166 RID: 16742 RVA: 0x0018F380 File Offset: 0x0018D580
		private void ToggleMovability(bool canMove)
		{
			if (this.m_dragImage)
			{
				this.m_dragImage.enabled = canMove;
			}
			if (this.m_visuals)
			{
				this.m_visuals.SetActive(canMove);
			}
			if (this.m_window)
			{
				this.m_window.PreventDragging = !canMove;
			}
		}

		// Token: 0x04003EBF RID: 16063
		[SerializeField]
		private Image m_dragImage;

		// Token: 0x04003EC0 RID: 16064
		[SerializeField]
		private DraggableUIWindow m_window;

		// Token: 0x04003EC1 RID: 16065
		[SerializeField]
		private GameObject m_visuals;

		// Token: 0x04003EC2 RID: 16066
		[SerializeField]
		private SolButton m_resetButton;
	}
}
