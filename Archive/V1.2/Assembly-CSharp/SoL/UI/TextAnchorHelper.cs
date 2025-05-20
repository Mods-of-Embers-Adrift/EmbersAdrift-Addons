using System;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x02000365 RID: 869
	public static class TextAnchorHelper
	{
		// Token: 0x060017C9 RID: 6089 RVA: 0x00102ED4 File Offset: 0x001010D4
		public static Vector2 GetPivot(this TextAnchor anchor)
		{
			switch (anchor)
			{
			case TextAnchor.UpperLeft:
				return new Vector2(0f, 1f);
			case TextAnchor.UpperCenter:
				return new Vector2(0.5f, 1f);
			case TextAnchor.UpperRight:
				return Vector2.one;
			case TextAnchor.MiddleLeft:
				return new Vector2(0f, 0.5f);
			case TextAnchor.MiddleCenter:
				return new Vector2(0.5f, 0.5f);
			case TextAnchor.MiddleRight:
				return new Vector2(1f, 0.5f);
			case TextAnchor.LowerLeft:
				return Vector2.zero;
			case TextAnchor.LowerCenter:
				return new Vector2(0.5f, 0f);
			case TextAnchor.LowerRight:
				return new Vector2(1f, 0f);
			default:
				return new Vector2(0f, 1f);
			}
		}
	}
}
