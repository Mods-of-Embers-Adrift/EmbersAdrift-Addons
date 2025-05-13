using System;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x0200039E RID: 926
	public class UIWindowAnchorAdjuster : MonoBehaviour
	{
		// Token: 0x0600196D RID: 6509 RVA: 0x001068A8 File Offset: 0x00104AA8
		private void Start()
		{
			this.m_rect = (base.gameObject.transform as RectTransform);
			this.m_draggableUIWindow = base.gameObject.GetComponent<DraggableUIWindow>();
			if (!this.m_rect || !this.m_draggableUIWindow)
			{
				base.enabled = false;
				return;
			}
			this.m_rect.SetPivot(Vector2.one * 0.5f);
			this.m_currentPivotCorner = this.GetPivotCorner(this.m_rect.position);
			this.SetPivot(this.m_currentPivotCorner);
			this.m_draggableUIWindow.WindowChanged += this.WindowChanged;
			Options.GameOptions.GameUIScale.Changed += this.WindowChanged;
		}

		// Token: 0x0600196E RID: 6510 RVA: 0x00053E58 File Offset: 0x00052058
		private void OnDestroy()
		{
			if (this.m_draggableUIWindow)
			{
				this.m_draggableUIWindow.WindowChanged -= this.WindowChanged;
			}
			Options.GameOptions.GameUIScale.Changed -= this.WindowChanged;
		}

		// Token: 0x0600196F RID: 6511 RVA: 0x00106970 File Offset: 0x00104B70
		private void WindowChanged()
		{
			Vector3 v = this.m_rect.TransformPoint(this.m_rect.rect.center);
			this.m_currentPivotCorner = this.GetPivotCorner(v);
			this.SetPivot(this.m_currentPivotCorner);
			this.SetAnchors(this.m_currentPivotCorner);
		}

		// Token: 0x06001970 RID: 6512 RVA: 0x001069CC File Offset: 0x00104BCC
		private UIWindowAnchorAdjuster.PivotCorner GetPivotCorner(Vector2 pos)
		{
			float num = (float)Screen.width * 0.5f;
			float num2 = (float)Screen.height * 0.5f;
			if (pos.x < num && pos.y < num2)
			{
				return UIWindowAnchorAdjuster.PivotCorner.LowerLeft;
			}
			if (pos.x < num && pos.y > num2)
			{
				return UIWindowAnchorAdjuster.PivotCorner.UpperLeft;
			}
			if (pos.x > num && pos.y < num2)
			{
				return UIWindowAnchorAdjuster.PivotCorner.LowerRight;
			}
			if (pos.x > num && pos.y > num2)
			{
				return UIWindowAnchorAdjuster.PivotCorner.UpperRight;
			}
			return UIWindowAnchorAdjuster.PivotCorner.LowerLeft;
		}

		// Token: 0x06001971 RID: 6513 RVA: 0x00106A44 File Offset: 0x00104C44
		private void SetPivot(UIWindowAnchorAdjuster.PivotCorner pivotCorner)
		{
			Vector2 pivot = this.m_rect.pivot;
			Vector2 vector = pivot;
			switch (pivotCorner)
			{
			case UIWindowAnchorAdjuster.PivotCorner.LowerLeft:
				vector = Vector2.zero;
				break;
			case UIWindowAnchorAdjuster.PivotCorner.UpperLeft:
				vector = Vector2.up;
				break;
			case UIWindowAnchorAdjuster.PivotCorner.LowerRight:
				vector = Vector2.right;
				break;
			case UIWindowAnchorAdjuster.PivotCorner.UpperRight:
				vector = Vector2.one;
				break;
			}
			if (pivot != vector)
			{
				this.m_rect.SetPivot(vector);
			}
		}

		// Token: 0x06001972 RID: 6514 RVA: 0x00106AAC File Offset: 0x00104CAC
		private void SetAnchors(UIWindowAnchorAdjuster.PivotCorner pivotCorner)
		{
			Vector2 anchorMin = this.m_rect.anchorMin;
			Vector2 anchorMax = this.m_rect.anchorMax;
			Vector2 vector = anchorMin;
			Vector2 vector2 = anchorMax;
			switch (pivotCorner)
			{
			case UIWindowAnchorAdjuster.PivotCorner.LowerLeft:
				vector = Vector2.zero;
				vector2 = Vector2.zero;
				break;
			case UIWindowAnchorAdjuster.PivotCorner.UpperLeft:
				vector = Vector2.up;
				vector2 = Vector2.up;
				break;
			case UIWindowAnchorAdjuster.PivotCorner.LowerRight:
				vector = Vector2.right;
				vector2 = Vector2.right;
				break;
			case UIWindowAnchorAdjuster.PivotCorner.UpperRight:
				vector = Vector2.one;
				vector2 = Vector2.one;
				break;
			}
			if (anchorMin != vector || anchorMax != vector2)
			{
				this.m_rect.SetAnchors(vector, vector2);
			}
		}

		// Token: 0x0400205C RID: 8284
		private RectTransform m_rect;

		// Token: 0x0400205D RID: 8285
		private DraggableUIWindow m_draggableUIWindow;

		// Token: 0x0400205E RID: 8286
		private UIWindowAnchorAdjuster.PivotCorner m_currentPivotCorner;

		// Token: 0x0200039F RID: 927
		private enum PivotCorner
		{
			// Token: 0x04002060 RID: 8288
			LowerLeft,
			// Token: 0x04002061 RID: 8289
			UpperLeft,
			// Token: 0x04002062 RID: 8290
			LowerRight,
			// Token: 0x04002063 RID: 8291
			UpperRight
		}
	}
}
