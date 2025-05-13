using System;

namespace BansheeGz.BGSpline.Curve
{
	// Token: 0x0200018D RID: 397
	public static class FieldExtensions
	{
		// Token: 0x06000D8B RID: 3467 RVA: 0x0004BC1E File Offset: 0x00049E1E
		public static bool In(this BGCurveBaseMath.Field field, int mask)
		{
			return (field.Val() & mask) != 0;
		}

		// Token: 0x06000D8C RID: 3468 RVA: 0x0004BC2B File Offset: 0x00049E2B
		public static int Val(this BGCurveBaseMath.Field field)
		{
			return (int)field;
		}
	}
}
