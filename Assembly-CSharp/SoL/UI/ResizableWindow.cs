using System;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x0200036A RID: 874
	public class ResizableWindow : DraggableUIWindow
	{
		// Token: 0x14000020 RID: 32
		// (add) Token: 0x060017DD RID: 6109 RVA: 0x00103020 File Offset: 0x00101220
		// (remove) Token: 0x060017DE RID: 6110 RVA: 0x00103058 File Offset: 0x00101258
		public event Action ResizeBegin;

		// Token: 0x14000021 RID: 33
		// (add) Token: 0x060017DF RID: 6111 RVA: 0x00103090 File Offset: 0x00101290
		// (remove) Token: 0x060017E0 RID: 6112 RVA: 0x001030C8 File Offset: 0x001012C8
		public event Action ResizeDrag;

		// Token: 0x14000022 RID: 34
		// (add) Token: 0x060017E1 RID: 6113 RVA: 0x00103100 File Offset: 0x00101300
		// (remove) Token: 0x060017E2 RID: 6114 RVA: 0x00103138 File Offset: 0x00101338
		public event Action ResizeFinish;

		// Token: 0x170005BE RID: 1470
		// (get) Token: 0x060017E3 RID: 6115 RVA: 0x00052B0D File Offset: 0x00050D0D
		public Vector2 CurrentSizeDelta
		{
			get
			{
				return base.RectTransform.sizeDelta;
			}
		}

		// Token: 0x060017E4 RID: 6116 RVA: 0x00052B1A File Offset: 0x00050D1A
		protected override void Awake()
		{
			base.Awake();
			this.InitResizeTriggers();
		}

		// Token: 0x060017E5 RID: 6117 RVA: 0x00103170 File Offset: 0x00101370
		private void InitResizeTriggers()
		{
			if (this.m_resizeTriggers == null)
			{
				return;
			}
			for (int i = 0; i < this.m_resizeTriggers.Length; i++)
			{
				this.m_resizeTriggers[i].Init(this);
			}
		}

		// Token: 0x060017E6 RID: 6118 RVA: 0x00052B28 File Offset: 0x00050D28
		public void SetPivotPointExternal(Vector2 newPivot)
		{
			base.SetPivot(newPivot);
		}

		// Token: 0x060017E7 RID: 6119 RVA: 0x00052B31 File Offset: 0x00050D31
		public void SetWindowSizeFromResizeTrigger(Vector2 newDelta)
		{
			base.RectTransform.sizeDelta = newDelta.Clamp(this.m_minSize, this.m_maxSize);
			Action resizeDrag = this.ResizeDrag;
			if (resizeDrag == null)
			{
				return;
			}
			resizeDrag();
		}

		// Token: 0x060017E8 RID: 6120 RVA: 0x00052B60 File Offset: 0x00050D60
		public virtual void FinishResize()
		{
			this.ResetPivot();
			if (this.m_saveWindowPositionSize)
			{
				base.SaveWindowPositionSize();
			}
			base.TriggerWindowChanged();
			Action resizeFinish = this.ResizeFinish;
			if (resizeFinish == null)
			{
				return;
			}
			resizeFinish();
		}

		// Token: 0x060017E9 RID: 6121 RVA: 0x00052B8C File Offset: 0x00050D8C
		protected override void ResetButtonPressed()
		{
			base.ResetButtonPressed();
			base.RectTransform.sizeDelta = this.m_defaultSize.Clamp(this.m_minSize, this.m_maxSize);
			base.RectTransform.ClampToScreen();
		}

		// Token: 0x060017EA RID: 6122 RVA: 0x001031A8 File Offset: 0x001013A8
		protected override void RestoreSize(DraggableUIWindow.UIWindowData windowData)
		{
			base.RestoreSize(windowData);
			float x = Mathf.Clamp(windowData.Xsize, this.m_minSize.x, this.m_maxSize.x);
			float y = Mathf.Clamp(windowData.Ysize, this.m_minSize.y, this.m_maxSize.y);
			base.RectTransform.sizeDelta = new Vector2(x, y);
		}

		// Token: 0x060017EB RID: 6123 RVA: 0x00103214 File Offset: 0x00101414
		public override string FillActionsGetTitle()
		{
			string text = base.FillActionsGetTitle();
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			ContextMenuUI.AddContextAction("Reset Size", this.CanModify, delegate()
			{
				this.RestoreSize(this.m_originalRect);
				base.RectTransform.ClampToScreen();
				base.SaveWindowPositionSize();
			}, null, null);
			return text;
		}

		// Token: 0x060017EC RID: 6124 RVA: 0x00052BC1 File Offset: 0x00050DC1
		public void InvokeResizeBegin()
		{
			Action resizeBegin = this.ResizeBegin;
			if (resizeBegin == null)
			{
				return;
			}
			resizeBegin();
		}

		// Token: 0x04001F6C RID: 8044
		[SerializeField]
		private Vector2 m_defaultSize = new Vector2(200f, 200f);

		// Token: 0x04001F6D RID: 8045
		[SerializeField]
		private Vector2 m_minSize = new Vector2(100f, 100f);

		// Token: 0x04001F6E RID: 8046
		[SerializeField]
		private Vector2 m_maxSize = new Vector2(400f, 400f);

		// Token: 0x04001F6F RID: 8047
		[SerializeField]
		private ResizableWindowTrigger[] m_resizeTriggers;
	}
}
