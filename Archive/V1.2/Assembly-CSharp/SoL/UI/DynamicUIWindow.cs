using System;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.UI
{
	// Token: 0x02000363 RID: 867
	public class DynamicUIWindow : UIWindow
	{
		// Token: 0x170005B8 RID: 1464
		// (get) Token: 0x060017C2 RID: 6082 RVA: 0x00052A4C File Offset: 0x00050C4C
		// (set) Token: 0x060017C3 RID: 6083 RVA: 0x00052A54 File Offset: 0x00050C54
		protected virtual DynamicUIWindow.PivotCorner CurrentPivotCorner { get; set; }

		// Token: 0x060017C4 RID: 6084 RVA: 0x00052A5D File Offset: 0x00050C5D
		private void Update()
		{
			if (base.Visible || this.m_state == UIWindow.UIWindowState.FadingIn)
			{
				this.LateUpdateInternal();
			}
		}

		// Token: 0x060017C5 RID: 6085 RVA: 0x00102B3C File Offset: 0x00100D3C
		protected virtual void LateUpdateInternal()
		{
			this.m_currentPivot = DynamicUIWindow.m_upperLeft;
			if (this.m_followMouse)
			{
				base.gameObject.transform.position = Input.mousePosition;
			}
			else if (this.m_anchor != null)
			{
				this.m_currentPivot = this.m_overridePivot.GetPivot();
				base.gameObject.transform.position = this.m_anchor.transform.position;
			}
			this.SetPivot();
		}

		// Token: 0x060017C6 RID: 6086 RVA: 0x00102BB8 File Offset: 0x00100DB8
		private void SetPivot()
		{
			Vector2 currentPivot = this.m_currentPivot;
			if (base.gameObject.transform.position.x > (float)Screen.width * 0.5f)
			{
				currentPivot.x = 1f;
			}
			if (base.gameObject.transform.position.y < (float)Screen.height * 0.5f)
			{
				currentPivot.y = 0f;
			}
			Quaternion rotation = Quaternion.identity;
			TextAnchor childAlignment = TextAnchor.LowerLeft;
			if (currentPivot == DynamicUIWindow.m_lowerLeft)
			{
				if (this.m_followMouse)
				{
					base.gameObject.transform.position += DynamicUIWindow.m_lowerLeftOffset;
				}
				this.CurrentPivotCorner = DynamicUIWindow.PivotCorner.LowerLeft;
			}
			else if (currentPivot == DynamicUIWindow.m_upperLeft)
			{
				rotation = Quaternion.Euler(new Vector3(180f, 0f, 0f));
				childAlignment = TextAnchor.UpperLeft;
				if (this.m_followMouse)
				{
					base.gameObject.transform.position += DynamicUIWindow.m_upperLeftOffset;
				}
				this.CurrentPivotCorner = DynamicUIWindow.PivotCorner.UpperLeft;
			}
			else if (currentPivot == DynamicUIWindow.m_upperRight)
			{
				rotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
				childAlignment = TextAnchor.UpperRight;
				if (this.m_followMouse)
				{
					base.gameObject.transform.position += DynamicUIWindow.m_upperRightOffset;
				}
				this.CurrentPivotCorner = DynamicUIWindow.PivotCorner.UpperRight;
			}
			else if (currentPivot == DynamicUIWindow.m_lowerRight)
			{
				rotation = Quaternion.Euler(new Vector3(180f, 0f, 180f));
				childAlignment = TextAnchor.LowerRight;
				if (this.m_followMouse)
				{
					base.gameObject.transform.position += DynamicUIWindow.m_lowerRightOffset;
				}
				this.CurrentPivotCorner = DynamicUIWindow.PivotCorner.LowerRight;
			}
			if (this.m_contentLayoutGroup != null)
			{
				this.m_contentLayoutGroup.childAlignment = childAlignment;
			}
			if (this.m_backgroundImage != null)
			{
				this.m_backgroundImage.rectTransform.rotation = rotation;
			}
			if (this.m_animationAnchorTransition && this.m_state == UIWindow.UIWindowState.Shown)
			{
				Vector2 pivot = Vector2.Lerp(base.RectTransform.pivot, currentPivot, Time.deltaTime * 10f);
				base.RectTransform.SetPivot(pivot);
			}
			else
			{
				base.RectTransform.SetPivot(currentPivot);
			}
			if (this.m_content != null)
			{
				this.m_content.rotation = Quaternion.identity;
			}
		}

		// Token: 0x04001F4F RID: 8015
		private static readonly Vector2 m_lowerLeft = Vector2.zero;

		// Token: 0x04001F50 RID: 8016
		private static readonly Vector2 m_upperLeft = new Vector2(0f, 1f);

		// Token: 0x04001F51 RID: 8017
		private static readonly Vector2 m_upperRight = Vector2.one;

		// Token: 0x04001F52 RID: 8018
		private static readonly Vector2 m_lowerRight = new Vector2(1f, 0f);

		// Token: 0x04001F53 RID: 8019
		private const float kLeftOffset = 30f;

		// Token: 0x04001F54 RID: 8020
		private static readonly Vector3 m_lowerLeftOffset = new Vector3(30f, -30f, 0f);

		// Token: 0x04001F55 RID: 8021
		private static readonly Vector3 m_upperLeftOffset = new Vector3(30f, 0f, 0f);

		// Token: 0x04001F56 RID: 8022
		private const float kRightOffset = 10f;

		// Token: 0x04001F57 RID: 8023
		private static readonly Vector3 m_lowerRightOffset = new Vector3(-10f, -10f, 0f);

		// Token: 0x04001F58 RID: 8024
		private static readonly Vector3 m_upperRightOffset = new Vector3(-10f, 0f, 0f);

		// Token: 0x04001F59 RID: 8025
		[SerializeField]
		private Image m_backgroundImage;

		// Token: 0x04001F5A RID: 8026
		[SerializeField]
		private LayoutGroup m_contentLayoutGroup;

		// Token: 0x04001F5B RID: 8027
		[SerializeField]
		protected RectTransform m_content;

		// Token: 0x04001F5C RID: 8028
		[SerializeField]
		protected bool m_followMouse;

		// Token: 0x04001F5D RID: 8029
		[SerializeField]
		private bool m_animationAnchorTransition;

		// Token: 0x04001F5E RID: 8030
		protected Transform m_anchor;

		// Token: 0x04001F5F RID: 8031
		protected Vector2 m_currentPivot = DynamicUIWindow.m_upperLeft;

		// Token: 0x04001F60 RID: 8032
		protected TextAnchor m_overridePivot;

		// Token: 0x02000364 RID: 868
		protected enum PivotCorner
		{
			// Token: 0x04001F63 RID: 8035
			LowerLeft,
			// Token: 0x04001F64 RID: 8036
			UpperLeft,
			// Token: 0x04001F65 RID: 8037
			LowerRight,
			// Token: 0x04001F66 RID: 8038
			UpperRight
		}
	}
}
