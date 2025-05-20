using System;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Example
{
	// Token: 0x020001F2 RID: 498
	[RequireComponent(typeof(BGCurve))]
	[RequireComponent(typeof(LineRenderer))]
	public class BGTestCurveStatic : MonoBehaviour
	{
		// Token: 0x06001147 RID: 4423 RVA: 0x0004E59B File Offset: 0x0004C79B
		private void Start()
		{
			this.curve = base.GetComponent<BGCurve>();
			this.lineRenderer = base.GetComponent<LineRenderer>();
			this.curveBaseMath = new BGCurveBaseMath(this.curve);
			this.started = Time.time;
			this.ResetLineRenderer();
		}

		// Token: 0x06001148 RID: 4424 RVA: 0x000E3354 File Offset: 0x000E1554
		private void ResetLineRenderer()
		{
			Vector3[] array = new Vector3[100];
			for (int i = 0; i < 100; i++)
			{
				array[i] = this.curveBaseMath.CalcPositionByDistanceRatio((float)i / 99f, false);
			}
			this.lineRenderer.positionCount = 100;
			this.lineRenderer.SetPositions(array);
		}

		// Token: 0x06001149 RID: 4425 RVA: 0x000E33AC File Offset: 0x000E15AC
		private void Update()
		{
			this.ratio = (Time.time - this.started) / 3f;
			if (this.ratio >= 1f)
			{
				this.started = Time.time;
				this.ratio = 0f;
				return;
			}
			this.ObjectToMove.transform.position = this.curveBaseMath.CalcPositionByDistanceRatio(this.ratio, false);
		}

		// Token: 0x04000EA8 RID: 3752
		private const int TimeToMoveUp = 3;

		// Token: 0x04000EA9 RID: 3753
		public GameObject ObjectToMove;

		// Token: 0x04000EAA RID: 3754
		private BGCurve curve;

		// Token: 0x04000EAB RID: 3755
		private BGCurveBaseMath curveBaseMath;

		// Token: 0x04000EAC RID: 3756
		private float started;

		// Token: 0x04000EAD RID: 3757
		private float ratio;

		// Token: 0x04000EAE RID: 3758
		private LineRenderer lineRenderer;
	}
}
