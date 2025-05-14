using System;
using BansheeGz.BGSpline.Components;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Example
{
	// Token: 0x020001E5 RID: 485
	[RequireComponent(typeof(BGCcMath))]
	public class BGTestCurveRuntime : MonoBehaviour
	{
		// Token: 0x0600111B RID: 4379 RVA: 0x0004E30B File Offset: 0x0004C50B
		private void Start()
		{
			this.curve = base.GetComponent<BGCurve>();
			this.math = base.GetComponent<BGCcMath>();
			this.Reset();
		}

		// Token: 0x0600111C RID: 4380 RVA: 0x000E2684 File Offset: 0x000E0884
		private void Reset()
		{
			this.curve.Clear();
			this.curve.AddPoint(new BGCurvePoint(this.curve, Vector3.zero, false)
			{
				ControlType = BGCurvePoint.ControlTypeEnum.BezierIndependant
			});
			this.started = Time.time;
			this.counter = 0;
		}

		// Token: 0x0600111D RID: 4381 RVA: 0x000E26D4 File Offset: 0x000E08D4
		private void Update()
		{
			float num = Time.time - this.started;
			if (num >= 1.5f || this.curve.PointsCount < 2)
			{
				if (this.counter == 8)
				{
					this.Reset();
					return;
				}
				Vector3 positionLocal = this.curve[this.curve.PointsCount - 1].PositionLocal;
				this.curve.AddPoint(new BGCurvePoint(this.curve, positionLocal, false)
				{
					ControlType = BGCurvePoint.ControlTypeEnum.BezierIndependant,
					ControlFirstLocal = Vector3.zero,
					ControlSecondLocal = Vector3.zero
				});
				bool flag = this.counter < 4;
				this.nextPosition = positionLocal + new Vector3(0f, flag ? 2f : -2f, 0f);
				Vector3 b = new Vector3((float)UnityEngine.Random.Range(-4, 4), (flag ? 2f : -2f) * 0.5f, (float)UnityEngine.Random.Range(-4, 4));
				this.nextControl1 = Vector3.Lerp(positionLocal - this.nextPosition, b, 0.8f);
				this.nextControl2 = Vector3.Lerp(this.nextPosition - positionLocal, b, 0.8f);
				this.started = Time.time;
				this.counter++;
				if (this.curve.PointsCount > 2)
				{
					this.curve.Delete(0);
					return;
				}
			}
			else
			{
				float num2 = num / 1.5f;
				BGCurvePointI bgcurvePointI = this.curve[this.curve.PointsCount - 1];
				bgcurvePointI.PositionLocal = Vector3.Lerp(bgcurvePointI.PositionLocal, this.nextPosition, num2);
				bgcurvePointI.ControlFirstLocal = Vector3.Lerp(bgcurvePointI.PositionLocal, this.nextControl1, num2);
				bgcurvePointI.ControlFirstLocal = Vector3.Lerp(bgcurvePointI.PositionLocal, this.nextControl2, num2);
				BGCurveBaseMath.SectionInfo sectionInfo = this.math.Math[0];
				this.ObjectToMove.transform.position = this.math.Math.CalcByDistance(BGCurveBaseMath.Field.Position, sectionInfo.DistanceFromStartToOrigin + sectionInfo.Distance * num2, false);
			}
		}

		// Token: 0x04000E53 RID: 3667
		private const float OneTierHeight = 2f;

		// Token: 0x04000E54 RID: 3668
		private const int PointsCountToAdd = 8;

		// Token: 0x04000E55 RID: 3669
		private const float PointMoveTime = 1.5f;

		// Token: 0x04000E56 RID: 3670
		private const int MaximumControlSpread = 4;

		// Token: 0x04000E57 RID: 3671
		public GameObject ObjectToMove;

		// Token: 0x04000E58 RID: 3672
		private BGCurve curve;

		// Token: 0x04000E59 RID: 3673
		private BGCcMath math;

		// Token: 0x04000E5A RID: 3674
		private float started;

		// Token: 0x04000E5B RID: 3675
		private int counter;

		// Token: 0x04000E5C RID: 3676
		private Vector3 nextPosition;

		// Token: 0x04000E5D RID: 3677
		private Vector3 nextControl1;

		// Token: 0x04000E5E RID: 3678
		private Vector3 nextControl2;
	}
}
