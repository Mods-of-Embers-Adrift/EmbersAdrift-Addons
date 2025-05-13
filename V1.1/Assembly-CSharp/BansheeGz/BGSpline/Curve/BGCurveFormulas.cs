using System;
using UnityEngine;

namespace BansheeGz.BGSpline.Curve
{
	// Token: 0x02000194 RID: 404
	public static class BGCurveFormulas
	{
		// Token: 0x06000DA6 RID: 3494 RVA: 0x000D8A4C File Offset: 0x000D6C4C
		public static Vector3 BezierCubic(float t, Vector3 from, Vector3 fromControl, Vector3 toControl, Vector3 to)
		{
			float num = 1f - t;
			float num2 = num * num;
			float num3 = t * t;
			return num * num2 * from + 3f * num2 * t * fromControl + 3f * num * num3 * toControl + t * num3 * to;
		}

		// Token: 0x06000DA7 RID: 3495 RVA: 0x000D8AAC File Offset: 0x000D6CAC
		public static Vector3 BezierQuadratic(float t, Vector3 from, Vector3 control, Vector3 to)
		{
			float num = 1f - t;
			float d = num * num;
			float d2 = t * t;
			return d * from + 2f * num * t * control + d2 * to;
		}

		// Token: 0x06000DA8 RID: 3496 RVA: 0x000D8AF0 File Offset: 0x000D6CF0
		public static Vector3 BezierCubicDerivative(float t, Vector3 from, Vector3 fromControl, Vector3 toControl, Vector3 to)
		{
			float num = 1f - t;
			return 3f * (num * num) * (fromControl - from) + 6f * num * t * (toControl - fromControl) + 3f * (t * t) * (to - toControl);
		}

		// Token: 0x06000DA9 RID: 3497 RVA: 0x000D8B50 File Offset: 0x000D6D50
		public static Vector3 BezierQuadraticDerivative(float t, Vector3 from, Vector3 control, Vector3 to)
		{
			float num = 1f - t;
			return 2f * num * (control - from) + 2f * t * (to - control);
		}
	}
}
