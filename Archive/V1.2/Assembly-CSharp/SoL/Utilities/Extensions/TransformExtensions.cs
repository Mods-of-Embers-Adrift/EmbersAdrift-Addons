using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Utilities.Extensions
{
	// Token: 0x0200033E RID: 830
	public static class TransformExtensions
	{
		// Token: 0x060016B7 RID: 5815 RVA: 0x001010A8 File Offset: 0x000FF2A8
		public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
		{
			if (!rectTransform)
			{
				return;
			}
			Vector2 size = rectTransform.rect.size;
			Vector2 vector = rectTransform.pivot - pivot;
			Vector3 b = new Vector3(vector.x * size.x, vector.y * size.y);
			rectTransform.pivot = pivot;
			rectTransform.localPosition -= b;
		}

		// Token: 0x060016B8 RID: 5816 RVA: 0x00101114 File Offset: 0x000FF314
		public static void SetAnchors(this RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax)
		{
			if (!rectTransform)
			{
				return;
			}
			Vector3 position = rectTransform.position;
			rectTransform.anchorMin = anchorMin;
			rectTransform.anchorMax = anchorMax;
			rectTransform.position = position;
		}

		// Token: 0x060016B9 RID: 5817 RVA: 0x00101148 File Offset: 0x000FF348
		public static void ClampToScreen(this RectTransform rt)
		{
			if (!rt)
			{
				return;
			}
			Rect rect = rt.rect;
			Vector3 position = rt.position;
			Vector2 pivot = rt.pivot;
			float gameUIScalePercentage = Options.GameOptions.GameUIScalePercentage;
			float num = rect.width * gameUIScalePercentage;
			float num2 = rect.height * gameUIScalePercentage;
			Vector2 vector = new Vector2(num * pivot.x, num2 * pivot.y);
			Vector2 vector2 = new Vector2((float)Screen.width - (num - vector.x), (float)Screen.height - (num2 - vector.y));
			Vector2 v = new Vector2(Mathf.Clamp(position.x, vector.x, vector2.x), Mathf.Clamp(position.y, vector.y, vector2.y));
			rt.position = v;
		}

		// Token: 0x060016BA RID: 5818 RVA: 0x00051E43 File Offset: 0x00050043
		public static Vector2 Clamp(this Vector2 sizeDelta, Vector2 min, Vector2 max)
		{
			return new Vector2(Mathf.Clamp(sizeDelta.x, min.x, max.x), Mathf.Clamp(sizeDelta.y, min.y, max.y));
		}

		// Token: 0x060016BB RID: 5819 RVA: 0x00051E78 File Offset: 0x00050078
		public static void MoveToForeground(this Transform rt)
		{
			rt.SetAsLastSibling();
		}

		// Token: 0x060016BC RID: 5820 RVA: 0x00051E80 File Offset: 0x00050080
		public static void MoveToBackground(this Transform rt)
		{
			rt.SetAsFirstSibling();
		}

		// Token: 0x060016BD RID: 5821 RVA: 0x00051E88 File Offset: 0x00050088
		public static void MoveBack(this Transform rt)
		{
			rt.SetSiblingIndex(rt.GetSiblingIndex() - 1);
		}

		// Token: 0x060016BE RID: 5822 RVA: 0x00051E98 File Offset: 0x00050098
		public static void MoveForward(this Transform rt)
		{
			rt.SetSiblingIndex(rt.GetSiblingIndex() + 1);
		}

		// Token: 0x060016BF RID: 5823 RVA: 0x00101218 File Offset: 0x000FF418
		public static float GetExpandedHeight(this RectTransform transform, LayoutElement element)
		{
			float preferredHeight = element.preferredHeight;
			element.preferredHeight = -1f;
			float preferredHeight2 = LayoutUtility.GetPreferredHeight(transform);
			element.preferredHeight = preferredHeight;
			return preferredHeight2;
		}

		// Token: 0x060016C0 RID: 5824 RVA: 0x00101244 File Offset: 0x000FF444
		public static float GetExpandedWidth(this RectTransform transform, LayoutElement element)
		{
			float preferredWidth = element.preferredWidth;
			element.preferredWidth = -1f;
			float preferredWidth2 = LayoutUtility.GetPreferredWidth(transform);
			element.preferredWidth = preferredWidth;
			return preferredWidth2;
		}

		// Token: 0x060016C1 RID: 5825 RVA: 0x00051EA8 File Offset: 0x000500A8
		public static void SetParentResetScale(this Transform transform, Transform parent)
		{
			transform.SetParent(null);
			transform.localScale = Vector3.one;
			transform.SetParent(parent);
		}

		// Token: 0x060016C2 RID: 5826 RVA: 0x00051EC3 File Offset: 0x000500C3
		public static void SetParentResetScale(this Transform transform, Transform parent, bool worldPositionStays)
		{
			transform.SetParent(null);
			transform.localScale = Vector3.one;
			transform.SetParent(parent, worldPositionStays);
		}

		// Token: 0x060016C3 RID: 5827 RVA: 0x00101270 File Offset: 0x000FF470
		public static void Copy(this Transform extends, Transform other)
		{
			int siblingIndex = other.GetSiblingIndex();
			extends.parent = other.parent;
			extends.SetSiblingIndex(siblingIndex);
			extends.position = other.position;
			extends.rotation = other.rotation;
			extends.localScale = other.localScale;
		}

		// Token: 0x060016C4 RID: 5828 RVA: 0x00051EDF File Offset: 0x000500DF
		public static void Copy(this Transform extends, GameObject other)
		{
			extends.Copy(other.transform);
		}

		// Token: 0x060016C5 RID: 5829 RVA: 0x00051EED File Offset: 0x000500ED
		public static void Copy(this Transform extends, Component other)
		{
			extends.Copy(other.transform);
		}

		// Token: 0x060016C6 RID: 5830 RVA: 0x00051EFB File Offset: 0x000500FB
		public static string GetPath(this Transform current)
		{
			if (!(current.parent == null))
			{
				return current.parent.GetPath() + "/" + current.name;
			}
			return "/" + current.name;
		}

		// Token: 0x060016C7 RID: 5831 RVA: 0x001012BC File Offset: 0x000FF4BC
		public static void GatherAllTransforms(this Transform parent, List<Transform> transformList)
		{
			transformList.Add(parent);
			for (int i = 0; i < parent.childCount; i++)
			{
				parent.GetChild(i).GatherAllTransforms(transformList);
			}
		}

		// Token: 0x060016C8 RID: 5832 RVA: 0x001012F0 File Offset: 0x000FF4F0
		public static Vector3 GetWorldCorner(this RectTransform rectTransform, RectTransformCorner corner)
		{
			if (rectTransform)
			{
				rectTransform.GetWorldCorners(TransformExtensions.m_4corners);
				switch (corner)
				{
				case RectTransformCorner.LowerLeft:
					return TransformExtensions.m_4corners[0];
				case RectTransformCorner.UpperLeft:
					return TransformExtensions.m_4corners[1];
				case RectTransformCorner.UpperRight:
					return TransformExtensions.m_4corners[2];
				case RectTransformCorner.LowerRight:
					return TransformExtensions.m_4corners[3];
				}
			}
			return Vector3.zero;
		}

		// Token: 0x04001E9F RID: 7839
		private static Vector3[] m_4corners = new Vector3[4];
	}
}
