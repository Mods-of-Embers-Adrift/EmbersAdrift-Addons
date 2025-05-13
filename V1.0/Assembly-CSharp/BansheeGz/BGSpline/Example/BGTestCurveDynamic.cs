using System;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Example
{
	// Token: 0x020001DF RID: 479
	[RequireComponent(typeof(BGCurve))]
	[RequireComponent(typeof(LineRenderer))]
	public class BGTestCurveDynamic : MonoBehaviour
	{
		// Token: 0x060010FF RID: 4351 RVA: 0x000E1D58 File Offset: 0x000DFF58
		private void Start()
		{
			this.curve = base.GetComponent<BGCurve>();
			this.lineRenderer = base.GetComponent<LineRenderer>();
			this.curveBaseMath = new BGCurveBaseMath(this.curve);
			this.started = Time.time;
			this.ResetLineRenderer();
			this.curve.Changed += delegate(object sender, BGCurveChangedArgs args)
			{
				this.ResetLineRenderer();
			};
		}

		// Token: 0x06001100 RID: 4352 RVA: 0x000E1DB8 File Offset: 0x000DFFB8
		private void ResetLineRenderer()
		{
			Vector3[] array = new Vector3[50];
			for (int i = 0; i < 50; i++)
			{
				array[i] = this.curveBaseMath.CalcPositionByDistanceRatio((float)i / 49f, false);
			}
			this.lineRenderer.positionCount = 50;
			this.lineRenderer.SetPositions(array);
		}

		// Token: 0x06001101 RID: 4353 RVA: 0x000E1E10 File Offset: 0x000E0010
		private void Update()
		{
			base.transform.RotateAround(Vector3.zero, Vector3.up, 40f * Time.deltaTime);
			this.ratio = (Time.time - this.started) / 3f;
			if (this.ratio >= 1f)
			{
				this.started = Time.time;
				this.ratio = 0f;
				return;
			}
			this.ObjectToMove.transform.position = this.curveBaseMath.CalcPositionByDistanceRatio(this.ratio, false);
		}

		// Token: 0x04000E2F RID: 3631
		private const int TimeToMoveUp = 3;

		// Token: 0x04000E30 RID: 3632
		public GameObject ObjectToMove;

		// Token: 0x04000E31 RID: 3633
		private BGCurve curve;

		// Token: 0x04000E32 RID: 3634
		private BGCurveBaseMath curveBaseMath;

		// Token: 0x04000E33 RID: 3635
		private float started;

		// Token: 0x04000E34 RID: 3636
		private float ratio;

		// Token: 0x04000E35 RID: 3637
		private LineRenderer lineRenderer;
	}
}
