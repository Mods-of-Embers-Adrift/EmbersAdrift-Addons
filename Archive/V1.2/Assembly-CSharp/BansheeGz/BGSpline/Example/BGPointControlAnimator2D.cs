using System;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Example
{
	// Token: 0x020001D7 RID: 471
	[RequireComponent(typeof(BGCurve))]
	public class BGPointControlAnimator2D : MonoBehaviour
	{
		// Token: 0x060010B6 RID: 4278 RVA: 0x000E09D4 File Offset: 0x000DEBD4
		private void Start()
		{
			this.curve = base.GetComponent<BGCurve>();
			this.curve[this.PointIndex].ControlSecondLocal = this.From;
			this.halfCyclePeriod = this.CyclePeriod * 0.5f;
		}

		// Token: 0x060010B7 RID: 4279 RVA: 0x000E0A20 File Offset: 0x000DEC20
		private void FixedUpdate()
		{
			float time = Time.time;
			if (this.cycleStarted < 0f || this.cycleStarted + this.CyclePeriod < time)
			{
				this.cycleStarted = time;
			}
			float num = time - this.cycleStarted;
			Vector2 v = Vector2.Lerp(this.From, this.To, (num < this.halfCyclePeriod) ? (num / this.halfCyclePeriod) : ((this.halfCyclePeriod * 2f - num) / this.halfCyclePeriod));
			BGCurvePointI bgcurvePointI = this.curve[this.PointIndex];
			if (this.AnimateSecondControl)
			{
				bgcurvePointI.ControlSecondLocal = v;
				return;
			}
			bgcurvePointI.ControlFirstLocal = v;
		}

		// Token: 0x04000DFF RID: 3583
		public Vector2 From = new Vector2(8f, 4f);

		// Token: 0x04000E00 RID: 3584
		public Vector2 To = new Vector2(12f, 2f);

		// Token: 0x04000E01 RID: 3585
		public float CyclePeriod = 1f;

		// Token: 0x04000E02 RID: 3586
		public int PointIndex;

		// Token: 0x04000E03 RID: 3587
		public bool AnimateSecondControl;

		// Token: 0x04000E04 RID: 3588
		private BGCurve curve;

		// Token: 0x04000E05 RID: 3589
		private float cycleStarted = -1f;

		// Token: 0x04000E06 RID: 3590
		private float halfCyclePeriod;
	}
}
