using System;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.UI
{
	// Token: 0x0200036B RID: 875
	public class ResizableWindowTrigger : MonoBehaviour, IPointerUpHandler, IEventSystemHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, ICursor, IInteractiveBase
	{
		// Token: 0x060017EF RID: 6127 RVA: 0x00052BF2 File Offset: 0x00050DF2
		private void Start()
		{
			if (this.m_resizableWindow == null)
			{
				Debug.LogErrorFormat("No resizable window for {0}", new object[]
				{
					base.gameObject.name
				});
			}
		}

		// Token: 0x060017F0 RID: 6128 RVA: 0x00052C20 File Offset: 0x00050E20
		public void Init(ResizableWindow rw)
		{
			this.m_resizableWindow = rw;
			this.m_sign = this.m_triggerType.GetSign();
			base.gameObject.GetComponent<Image>() != null;
		}

		// Token: 0x060017F1 RID: 6129 RVA: 0x00052C4C File Offset: 0x00050E4C
		private void SetPivotForResize()
		{
			this.m_resizableWindow.SetPivotPointExternal(this.m_triggerType.GetPivot());
		}

		// Token: 0x060017F2 RID: 6130 RVA: 0x00052C64 File Offset: 0x00050E64
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			if (this.m_resizableWindow == null)
			{
				return;
			}
			this.m_resizableWindow.FinishResize();
			CursorManager.UnlockCursorImage();
		}

		// Token: 0x060017F3 RID: 6131 RVA: 0x001032C8 File Offset: 0x001014C8
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			if (this.m_resizableWindow == null || !this.m_resizableWindow.CanModify)
			{
				return;
			}
			this.SetPivotForResize();
			this.m_startCursorPosition = eventData.position;
			this.m_startSizeDelta = this.m_resizableWindow.CurrentSizeDelta;
			CursorManager.SetCursorImage(this.Type, new Vector2?(ResizableWindowTrigger.kCursorHotspot));
			CursorManager.LockCursorImage();
		}

		// Token: 0x060017F4 RID: 6132 RVA: 0x0004475B File Offset: 0x0004295B
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
		}

		// Token: 0x060017F5 RID: 6133 RVA: 0x0004475B File Offset: 0x0004295B
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
		}

		// Token: 0x060017F6 RID: 6134 RVA: 0x00103330 File Offset: 0x00101530
		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			if (this.m_resizableWindow == null || !this.m_resizableWindow.CanModify)
			{
				return;
			}
			ClientGameManager.UIManager.RegisterDrag(null);
			Vector2 b = (eventData.position.Clamp(new Vector2(4f, 4f), new Vector2((float)Screen.width - 4f, (float)Screen.height - 4f)) - this.m_startCursorPosition) * this.m_sign;
			ResizableWindowTrigger.ResizableWindowTriggerType triggerType = this.m_triggerType;
			if (triggerType > ResizableWindowTrigger.ResizableWindowTriggerType.Right)
			{
				if (triggerType - ResizableWindowTrigger.ResizableWindowTriggerType.Top <= 1)
				{
					b.x = 0f;
				}
			}
			else
			{
				b.y = 0f;
			}
			this.m_resizableWindow.SetWindowSizeFromResizeTrigger(this.m_startSizeDelta + b);
		}

		// Token: 0x060017F7 RID: 6135 RVA: 0x00052C85 File Offset: 0x00050E85
		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
			if (this.m_resizableWindow == null)
			{
				return;
			}
			this.m_resizableWindow.FinishResize();
			ClientGameManager.UIManager.DeregisterDrag(false);
		}

		// Token: 0x060017F8 RID: 6136 RVA: 0x00052CAC File Offset: 0x00050EAC
		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			if (this.m_resizableWindow)
			{
				this.m_resizableWindow.InvokeResizeBegin();
			}
		}

		// Token: 0x170005BF RID: 1471
		// (get) Token: 0x060017F9 RID: 6137 RVA: 0x001033F8 File Offset: 0x001015F8
		public CursorType Type
		{
			get
			{
				bool flag = this.m_resizableWindow && !this.m_resizableWindow.CanModify;
				switch (this.m_triggerType)
				{
				case ResizableWindowTrigger.ResizableWindowTriggerType.Left:
				case ResizableWindowTrigger.ResizableWindowTriggerType.Right:
					if (!flag)
					{
						return CursorType.ResizeHorizontal;
					}
					return CursorType.ResizeHorizontal_Locked;
				case ResizableWindowTrigger.ResizableWindowTriggerType.Top:
				case ResizableWindowTrigger.ResizableWindowTriggerType.Bottom:
					if (!flag)
					{
						return CursorType.ResizeVertical;
					}
					return CursorType.ResizeVertical_Locked;
				case ResizableWindowTrigger.ResizableWindowTriggerType.LowerLeft:
				case ResizableWindowTrigger.ResizableWindowTriggerType.UpperRight:
					if (!flag)
					{
						return CursorType.ResizeForward;
					}
					return CursorType.ResizeForward_Locked;
				case ResizableWindowTrigger.ResizableWindowTriggerType.LowerRight:
				case ResizableWindowTrigger.ResizableWindowTriggerType.UpperLeft:
					if (!flag)
					{
						return CursorType.ResizeBackward;
					}
					return CursorType.ResizeBackward_Locked;
				default:
					return CursorType.MainCursor;
				}
			}
		}

		// Token: 0x170005C0 RID: 1472
		// (get) Token: 0x060017FA RID: 6138 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060017FD RID: 6141 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04001F70 RID: 8048
		private const float kScreenEdgeBuffer = 4f;

		// Token: 0x04001F71 RID: 8049
		public static Vector2 kCursorHotspot = new Vector2(16f, 16f);

		// Token: 0x04001F72 RID: 8050
		[SerializeField]
		private ResizableWindowTrigger.ResizableWindowTriggerType m_triggerType = ResizableWindowTrigger.ResizableWindowTriggerType.Top;

		// Token: 0x04001F73 RID: 8051
		private ResizableWindow m_resizableWindow;

		// Token: 0x04001F74 RID: 8052
		private Vector2 m_sign;

		// Token: 0x04001F75 RID: 8053
		private Vector2 m_startCursorPosition;

		// Token: 0x04001F76 RID: 8054
		private Vector2 m_startSizeDelta;

		// Token: 0x0200036C RID: 876
		public enum ResizableWindowTriggerType
		{
			// Token: 0x04001F78 RID: 8056
			Left,
			// Token: 0x04001F79 RID: 8057
			Right,
			// Token: 0x04001F7A RID: 8058
			Top,
			// Token: 0x04001F7B RID: 8059
			Bottom,
			// Token: 0x04001F7C RID: 8060
			LowerLeft,
			// Token: 0x04001F7D RID: 8061
			LowerRight,
			// Token: 0x04001F7E RID: 8062
			UpperLeft,
			// Token: 0x04001F7F RID: 8063
			UpperRight
		}
	}
}
