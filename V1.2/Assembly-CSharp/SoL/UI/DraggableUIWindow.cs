using System;
using Newtonsoft.Json;
using SoL.Managers;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoL.UI
{
	// Token: 0x02000361 RID: 865
	public class DraggableUIWindow : UIWindow, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDraggable
	{
		// Token: 0x170005B1 RID: 1457
		// (get) Token: 0x0600179A RID: 6042 RVA: 0x00052800 File Offset: 0x00050A00
		// (set) Token: 0x0600179B RID: 6043 RVA: 0x00052808 File Offset: 0x00050A08
		internal bool SaveWindowPositionSizeValue
		{
			get
			{
				return this.m_saveWindowPositionSize;
			}
			set
			{
				this.m_saveWindowPositionSize = value;
			}
		}

		// Token: 0x170005B2 RID: 1458
		// (get) Token: 0x0600179C RID: 6044 RVA: 0x00052811 File Offset: 0x00050A11
		// (set) Token: 0x0600179D RID: 6045 RVA: 0x00052823 File Offset: 0x00050A23
		public string PlayerPrefsKey
		{
			get
			{
				return "UIWindow_" + this.m_playerPrefsKey;
			}
			set
			{
				this.m_playerPrefsKey = value;
				this.m_saveWindowPositionSize = !string.IsNullOrEmpty(this.m_playerPrefsKey);
			}
		}

		// Token: 0x170005B3 RID: 1459
		// (get) Token: 0x0600179E RID: 6046 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool m_reparentOnDrag
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170005B4 RID: 1460
		// (get) Token: 0x0600179F RID: 6047 RVA: 0x00052840 File Offset: 0x00050A40
		public override bool CanModify
		{
			get
			{
				return !this.PreventDragging && base.CanModify;
			}
		}

		// Token: 0x170005B5 RID: 1461
		// (get) Token: 0x060017A0 RID: 6048 RVA: 0x00052852 File Offset: 0x00050A52
		// (set) Token: 0x060017A1 RID: 6049 RVA: 0x0005285A File Offset: 0x00050A5A
		public bool PreventReparentOnDrag { get; set; }

		// Token: 0x170005B6 RID: 1462
		// (get) Token: 0x060017A2 RID: 6050 RVA: 0x00052863 File Offset: 0x00050A63
		// (set) Token: 0x060017A3 RID: 6051 RVA: 0x0005286B File Offset: 0x00050A6B
		public bool PreventDragging { get; set; }

		// Token: 0x1400001F RID: 31
		// (add) Token: 0x060017A4 RID: 6052 RVA: 0x001027A8 File Offset: 0x001009A8
		// (remove) Token: 0x060017A5 RID: 6053 RVA: 0x001027E0 File Offset: 0x001009E0
		public event Action WindowChanged;

		// Token: 0x060017A6 RID: 6054 RVA: 0x00052874 File Offset: 0x00050A74
		protected void TriggerWindowChanged()
		{
			Action windowChanged = this.WindowChanged;
			if (windowChanged == null)
			{
				return;
			}
			windowChanged();
		}

		// Token: 0x060017A7 RID: 6055 RVA: 0x00052886 File Offset: 0x00050A86
		protected override void Awake()
		{
			base.Awake();
			this.m_internalStartingPivot = base.RectTransform.pivot;
			this.m_originalRect = new DraggableUIWindow.UIWindowData(base.RectTransform, base.Locked);
		}

		// Token: 0x060017A8 RID: 6056 RVA: 0x000528B6 File Offset: 0x00050AB6
		protected override void Start()
		{
			base.Start();
			if (this.m_saveWindowPositionSize && !string.IsNullOrEmpty(this.m_playerPrefsKey))
			{
				this.RestoreWindowPositionSize();
			}
		}

		// Token: 0x060017A9 RID: 6057 RVA: 0x00102818 File Offset: 0x00100A18
		private void SetPivot(PointerEventData eventData)
		{
			this.m_previousPivot = new Vector2?(base.RectTransform.pivot);
			Vector2 vector;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(base.RectTransform, eventData.position, eventData.pressEventCamera, out vector);
			vector.x = vector.x / base.RectTransform.sizeDelta.x + base.RectTransform.pivot.x;
			vector.y = vector.y / base.RectTransform.sizeDelta.y + base.RectTransform.pivot.y;
			base.RectTransform.SetPivot(vector);
		}

		// Token: 0x060017AA RID: 6058 RVA: 0x000528D9 File Offset: 0x00050AD9
		protected void SetPivot(Vector2 newPivot)
		{
			this.m_previousPivot = new Vector2?(base.RectTransform.pivot);
			base.RectTransform.SetPivot(newPivot);
		}

		// Token: 0x060017AB RID: 6059 RVA: 0x001028C0 File Offset: 0x00100AC0
		protected virtual void ResetPivot()
		{
			if (this.m_previousPivot != null)
			{
				base.RectTransform.SetPivot(this.m_previousPivot.Value);
				this.m_previousPivot = null;
				return;
			}
			base.RectTransform.SetPivot(this.m_internalStartingPivot);
		}

		// Token: 0x060017AC RID: 6060 RVA: 0x000528FD File Offset: 0x00050AFD
		public void ClampToScreen(bool savePosition)
		{
			base.RectTransform.ClampToScreen();
			if (savePosition)
			{
				this.SaveWindowPositionSize();
			}
		}

		// Token: 0x060017AD RID: 6061 RVA: 0x00052913 File Offset: 0x00050B13
		public override void ResolutionChanged()
		{
			base.ResolutionChanged();
			this.ClampToScreen(false);
		}

		// Token: 0x060017AE RID: 6062 RVA: 0x00102910 File Offset: 0x00100B10
		public override void Hide(bool skipTransition = false)
		{
			if (ClientGameManager.UIManager != null && ClientGameManager.UIManager.Dragged != null && ClientGameManager.UIManager.Dragged is DraggableUIWindow && (DraggableUIWindow)ClientGameManager.UIManager.Dragged == this)
			{
				ClientGameManager.UIManager.DeregisterDrag(false);
			}
			base.Hide(skipTransition);
		}

		// Token: 0x060017AF RID: 6063 RVA: 0x00052922 File Offset: 0x00050B22
		protected override void LockButtonPressed()
		{
			base.LockButtonPressed();
			this.SaveWindowPositionSize();
		}

		// Token: 0x060017B0 RID: 6064 RVA: 0x00052930 File Offset: 0x00050B30
		public override void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerDown(eventData);
			if (this.CanModify && eventData.button == PointerEventData.InputButton.Left)
			{
				this.SetPivot(eventData);
			}
		}

		// Token: 0x060017B1 RID: 6065 RVA: 0x00052950 File Offset: 0x00050B50
		public virtual void OnPointerUp(PointerEventData eventData)
		{
			if (this.CanModify)
			{
				this.ResetPivot();
			}
		}

		// Token: 0x060017B2 RID: 6066 RVA: 0x00102970 File Offset: 0x00100B70
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (this.CanModify && eventData.button == PointerEventData.InputButton.Left)
			{
				base.gameObject.transform.position = eventData.position;
				if (ClientGameManager.UIManager != null)
				{
					DraggableUIWindow draggable = (this.m_reparentOnDrag && !this.PreventReparentOnDrag) ? this : null;
					ClientGameManager.UIManager.RegisterDrag(draggable);
				}
				this.m_isDragging = true;
			}
		}

		// Token: 0x060017B3 RID: 6067 RVA: 0x001029DC File Offset: 0x00100BDC
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (this.m_isDragging && this.CanModify)
			{
				this.ResetPivot();
				if (ClientGameManager.UIManager != null)
				{
					ClientGameManager.UIManager.DeregisterDrag(false);
				}
				this.m_isDragging = false;
				this.SaveWindowPositionSize();
				Action windowChanged = this.WindowChanged;
				if (windowChanged == null)
				{
					return;
				}
				windowChanged();
			}
		}

		// Token: 0x060017B4 RID: 6068 RVA: 0x00052960 File Offset: 0x00050B60
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (this.m_isDragging && this.CanModify)
			{
				base.gameObject.transform.position = eventData.position;
				base.RectTransform.ClampToScreen();
			}
		}

		// Token: 0x170005B7 RID: 1463
		// (get) Token: 0x060017B5 RID: 6069 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IDraggable.ExternallyHandlePositionUpdate
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060017B6 RID: 6070 RVA: 0x0004475B File Offset: 0x0004295B
		void IDraggable.CompleteDrag(bool canceled)
		{
		}

		// Token: 0x060017B7 RID: 6071 RVA: 0x00102A34 File Offset: 0x00100C34
		protected void SaveWindowPositionSize()
		{
			if (!this.m_saveWindowPositionSize || string.IsNullOrEmpty(this.m_playerPrefsKey))
			{
				return;
			}
			DraggableUIWindow.UIWindowData uiwindowData = new DraggableUIWindow.UIWindowData(base.RectTransform, base.Locked);
			PlayerPrefs.SetString(this.PlayerPrefsKey, JsonConvert.SerializeObject(uiwindowData));
		}

		// Token: 0x060017B8 RID: 6072 RVA: 0x00102A80 File Offset: 0x00100C80
		internal void RestoreWindowPositionSize()
		{
			if (!this.m_saveWindowPositionSize || string.IsNullOrEmpty(this.m_playerPrefsKey))
			{
				return;
			}
			string @string = PlayerPrefs.GetString(this.PlayerPrefsKey, string.Empty);
			if (string.IsNullOrEmpty(@string))
			{
				return;
			}
			DraggableUIWindow.UIWindowData windowData = JsonConvert.DeserializeObject<DraggableUIWindow.UIWindowData>(@string);
			this.RestorePosition(windowData);
			this.RestoreSize(windowData);
			this.RestoreLockState(windowData);
			base.RectTransform.ClampToScreen();
		}

		// Token: 0x060017B9 RID: 6073 RVA: 0x00052998 File Offset: 0x00050B98
		private void RestorePosition(DraggableUIWindow.UIWindowData windowData)
		{
			base.RectTransform.anchoredPosition = new Vector2(windowData.Xpos, windowData.Ypos);
		}

		// Token: 0x060017BA RID: 6074 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void RestoreSize(DraggableUIWindow.UIWindowData windowData)
		{
		}

		// Token: 0x060017BB RID: 6075 RVA: 0x000529B6 File Offset: 0x00050BB6
		protected void RestoreLockState(DraggableUIWindow.UIWindowData windowData)
		{
			base.SetLockState(windowData.Locked);
		}

		// Token: 0x060017BC RID: 6076 RVA: 0x000529C4 File Offset: 0x00050BC4
		public virtual string FillActionsGetTitle()
		{
			if (!this.CanModify)
			{
				return null;
			}
			ContextMenuUI.AddContextAction("Reset Position", this.CanModify, delegate()
			{
				this.RestorePosition(this.m_originalRect);
				base.RectTransform.ClampToScreen();
				this.SaveWindowPositionSize();
			}, null, null);
			return "Reset Window";
		}

		// Token: 0x060017BD RID: 6077 RVA: 0x000529F3 File Offset: 0x00050BF3
		public void ResetPosition()
		{
			this.RestorePosition(this.m_originalRect);
			base.RectTransform.ClampToScreen();
			this.SaveWindowPositionSize();
		}

		// Token: 0x04001F41 RID: 8001
		[SerializeField]
		protected bool m_saveWindowPositionSize;

		// Token: 0x04001F42 RID: 8002
		[SerializeField]
		private string m_playerPrefsKey;

		// Token: 0x04001F43 RID: 8003
		protected Vector2? m_previousPivot;

		// Token: 0x04001F44 RID: 8004
		protected bool m_isDragging;

		// Token: 0x04001F45 RID: 8005
		private Vector2 m_internalStartingPivot = Vector2.zero;

		// Token: 0x04001F46 RID: 8006
		protected DraggableUIWindow.UIWindowData m_originalRect;

		// Token: 0x02000362 RID: 866
		[Serializable]
		protected struct UIWindowData
		{
			// Token: 0x060017C0 RID: 6080 RVA: 0x00052A25 File Offset: 0x00050C25
			public UIWindowData(float xpos, float ypos, float xsize, float ysize, bool locked)
			{
				this.Xpos = xpos;
				this.Ypos = ypos;
				this.Xsize = xsize;
				this.Ysize = ysize;
				this.Locked = locked;
			}

			// Token: 0x060017C1 RID: 6081 RVA: 0x00102AE4 File Offset: 0x00100CE4
			public UIWindowData(RectTransform rect, bool locked)
			{
				this.Xpos = rect.anchoredPosition.x;
				this.Ypos = rect.anchoredPosition.y;
				this.Xsize = rect.sizeDelta.x;
				this.Ysize = rect.sizeDelta.y;
				this.Locked = locked;
			}

			// Token: 0x04001F4A RID: 8010
			public float Xpos;

			// Token: 0x04001F4B RID: 8011
			public float Ypos;

			// Token: 0x04001F4C RID: 8012
			public float Xsize;

			// Token: 0x04001F4D RID: 8013
			public float Ysize;

			// Token: 0x04001F4E RID: 8014
			public bool Locked;
		}
	}
}
