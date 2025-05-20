using System;

namespace BansheeGz.BGSpline.Curve
{
	// Token: 0x02000190 RID: 400
	public class BGCurveChangedArgs : EventArgs, ICloneable
	{
		// Token: 0x170003AC RID: 940
		// (get) Token: 0x06000D92 RID: 3474 RVA: 0x0004BC56 File Offset: 0x00049E56
		public BGCurveChangedArgs.ChangeTypeEnum ChangeType
		{
			get
			{
				return this.changeType;
			}
		}

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x06000D93 RID: 3475 RVA: 0x0004BC5E File Offset: 0x00049E5E
		public BGCurve Curve
		{
			get
			{
				return this.curve;
			}
		}

		// Token: 0x170003AE RID: 942
		// (get) Token: 0x06000D94 RID: 3476 RVA: 0x0004BC66 File Offset: 0x00049E66
		public string Message
		{
			get
			{
				return this.message;
			}
		}

		// Token: 0x170003AF RID: 943
		// (get) Token: 0x06000D95 RID: 3477 RVA: 0x0004BC6E File Offset: 0x00049E6E
		public BGCurveChangedArgs[] MultipleChanges
		{
			get
			{
				return this.multipleChanges;
			}
		}

		// Token: 0x06000D96 RID: 3478 RVA: 0x0004BC76 File Offset: 0x00049E76
		private BGCurveChangedArgs()
		{
		}

		// Token: 0x06000D97 RID: 3479 RVA: 0x0004BC7E File Offset: 0x00049E7E
		public static BGCurveChangedArgs GetInstance(BGCurve curve, BGCurveChangedArgs.ChangeTypeEnum type, string message)
		{
			BGCurveChangedArgs.Instance.curve = curve;
			BGCurveChangedArgs.Instance.changeType = type;
			BGCurveChangedArgs.Instance.message = message;
			BGCurveChangedArgs.Instance.multipleChanges = null;
			BGCurveChangedArgs.Instance.point = null;
			return BGCurveChangedArgs.Instance;
		}

		// Token: 0x06000D98 RID: 3480 RVA: 0x000D8170 File Offset: 0x000D6370
		public static BGCurveChangedArgs GetInstance(BGCurve curve, BGCurveChangedArgs[] changes, string changesInTransaction)
		{
			BGCurveChangedArgs.Instance.curve = curve;
			BGCurveChangedArgs.Instance.changeType = BGCurveChangedArgs.ChangeTypeEnum.Multiple;
			BGCurveChangedArgs.Instance.message = "changes in transaction";
			BGCurveChangedArgs.Instance.multipleChanges = changes;
			BGCurveChangedArgs.Instance.point = null;
			return BGCurveChangedArgs.Instance;
		}

		// Token: 0x06000D99 RID: 3481 RVA: 0x0004BCBC File Offset: 0x00049EBC
		public static BGCurveChangedArgs GetInstance(BGCurve curve, BGCurvePointI point, string changesInTransaction)
		{
			BGCurveChangedArgs.Instance.curve = curve;
			BGCurveChangedArgs.Instance.changeType = BGCurveChangedArgs.ChangeTypeEnum.Point;
			BGCurveChangedArgs.Instance.message = "changes in transaction";
			BGCurveChangedArgs.Instance.point = point;
			return BGCurveChangedArgs.Instance;
		}

		// Token: 0x06000D9A RID: 3482 RVA: 0x000D81C0 File Offset: 0x000D63C0
		public object Clone()
		{
			return new BGCurveChangedArgs
			{
				changeType = this.changeType,
				curve = this.curve,
				multipleChanges = this.multipleChanges,
				message = this.message,
				point = this.point
			};
		}

		// Token: 0x06000D9B RID: 3483 RVA: 0x0004BCF3 File Offset: 0x00049EF3
		protected bool Equals(BGCurveChangedArgs other)
		{
			return this.changeType == other.changeType && object.Equals(this.curve, other.curve);
		}

		// Token: 0x06000D9C RID: 3484 RVA: 0x0004BD16 File Offset: 0x00049F16
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((BGCurveChangedArgs)obj)));
		}

		// Token: 0x06000D9D RID: 3485 RVA: 0x0004BD44 File Offset: 0x00049F44
		public override int GetHashCode()
		{
			return (int)(this.changeType * (BGCurveChangedArgs.ChangeTypeEnum)397 ^ (BGCurveChangedArgs.ChangeTypeEnum)((this.curve != null) ? this.curve.GetHashCode() : 0));
		}

		// Token: 0x04000C9D RID: 3229
		private static readonly BGCurveChangedArgs Instance = new BGCurveChangedArgs();

		// Token: 0x04000C9E RID: 3230
		private BGCurveChangedArgs.ChangeTypeEnum changeType;

		// Token: 0x04000C9F RID: 3231
		private BGCurve curve;

		// Token: 0x04000CA0 RID: 3232
		private BGCurvePointI point;

		// Token: 0x04000CA1 RID: 3233
		private string message;

		// Token: 0x04000CA2 RID: 3234
		private BGCurveChangedArgs[] multipleChanges;

		// Token: 0x02000191 RID: 401
		public enum ChangeTypeEnum
		{
			// Token: 0x04000CA4 RID: 3236
			Multiple,
			// Token: 0x04000CA5 RID: 3237
			CurveTransform,
			// Token: 0x04000CA6 RID: 3238
			Points,
			// Token: 0x04000CA7 RID: 3239
			Point,
			// Token: 0x04000CA8 RID: 3240
			Fields,
			// Token: 0x04000CA9 RID: 3241
			Snap,
			// Token: 0x04000CAA RID: 3242
			Curve
		}

		// Token: 0x02000192 RID: 402
		public class BeforeChange : EventArgs
		{
			// Token: 0x06000D9F RID: 3487 RVA: 0x0004BC76 File Offset: 0x00049E76
			private BeforeChange()
			{
			}

			// Token: 0x06000DA0 RID: 3488 RVA: 0x0004BD7B File Offset: 0x00049F7B
			public static BGCurveChangedArgs.BeforeChange GetInstance(string operation)
			{
				BGCurveChangedArgs.BeforeChange.BeforeChangeInstance.Operation = operation;
				return BGCurveChangedArgs.BeforeChange.BeforeChangeInstance;
			}

			// Token: 0x04000CAB RID: 3243
			public static readonly BGCurveChangedArgs.BeforeChange BeforeChangeInstance = new BGCurveChangedArgs.BeforeChange();

			// Token: 0x04000CAC RID: 3244
			public string Operation;
		}
	}
}
