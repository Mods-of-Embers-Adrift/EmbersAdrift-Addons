using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x020008B7 RID: 2231
	public class MoveUIReset : MonoBehaviour
	{
		// Token: 0x0600415E RID: 16734 RVA: 0x0006C222 File Offset: 0x0006A422
		private void Awake()
		{
			InputManager.MoveUIHeldChanged += this.InputManagerOnMoveUIHeldChanged;
		}

		// Token: 0x0600415F RID: 16735 RVA: 0x0006C235 File Offset: 0x0006A435
		private void OnDestroy()
		{
			InputManager.MoveUIHeldChanged -= this.InputManagerOnMoveUIHeldChanged;
		}

		// Token: 0x06004160 RID: 16736 RVA: 0x0006C248 File Offset: 0x0006A448
		private void InputManagerOnMoveUIHeldChanged(bool obj)
		{
			if (this.m_rect)
			{
				this.m_rect.anchoredPosition = this.m_anchoredPos;
			}
		}

		// Token: 0x04003EBD RID: 16061
		[SerializeField]
		private Vector2 m_anchoredPos = Vector2.zero;

		// Token: 0x04003EBE RID: 16062
		[SerializeField]
		private RectTransform m_rect;
	}
}
