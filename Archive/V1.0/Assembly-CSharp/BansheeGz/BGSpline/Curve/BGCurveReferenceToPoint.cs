using System;
using UnityEngine;

namespace BansheeGz.BGSpline.Curve
{
	// Token: 0x020001A1 RID: 417
	public class BGCurveReferenceToPoint : MonoBehaviour
	{
		// Token: 0x170003E7 RID: 999
		// (get) Token: 0x06000EAB RID: 3755 RVA: 0x000DB080 File Offset: 0x000D9280
		// (set) Token: 0x06000EAC RID: 3756 RVA: 0x000DB0AC File Offset: 0x000D92AC
		public BGCurvePointI Point
		{
			get
			{
				if (!(this.pointGo != null))
				{
					return this.pointComponent;
				}
				return this.pointGo;
			}
			set
			{
				if (value == null)
				{
					this.pointGo = null;
					this.pointComponent = null;
					return;
				}
				if (value is BGCurvePointGO)
				{
					this.pointGo = (BGCurvePointGO)value;
					this.pointComponent = null;
					return;
				}
				if (value is BGCurvePointComponent)
				{
					this.pointComponent = (BGCurvePointComponent)value;
					this.pointGo = null;
					return;
				}
				this.pointGo = null;
				this.pointComponent = null;
			}
		}

		// Token: 0x06000EAD RID: 3757 RVA: 0x000DB114 File Offset: 0x000D9314
		public static BGCurveReferenceToPoint GetReferenceToPoint(BGCurvePointI point)
		{
			if (point.PointTransform == null)
			{
				return null;
			}
			BGCurveReferenceToPoint[] components = point.PointTransform.GetComponents<BGCurveReferenceToPoint>();
			if (components.Length == 0)
			{
				return null;
			}
			int num = components.Length;
			for (int i = 0; i < num; i++)
			{
				BGCurveReferenceToPoint bgcurveReferenceToPoint = components[i];
				if (bgcurveReferenceToPoint.Point == point)
				{
					return bgcurveReferenceToPoint;
				}
			}
			return null;
		}

		// Token: 0x04000CF8 RID: 3320
		[SerializeField]
		private BGCurvePointComponent pointComponent;

		// Token: 0x04000CF9 RID: 3321
		[SerializeField]
		private BGCurvePointGO pointGo;
	}
}
