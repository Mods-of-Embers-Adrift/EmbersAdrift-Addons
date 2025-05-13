using System;
using BansheeGz.BGSpline.Components;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Example
{
	// Token: 0x020001F1 RID: 497
	public class BGTestCurveSnapping : MonoBehaviour
	{
		// Token: 0x06001144 RID: 4420 RVA: 0x000E3170 File Offset: 0x000E1370
		private void Start()
		{
			this.from = new Vector3(-this.XChange, base.transform.position.y, base.transform.position.z);
			this.to = new Vector3(this.XChange, base.transform.position.y, base.transform.position.z);
			this.initialY = base.transform.position.y;
		}

		// Token: 0x06001145 RID: 4421 RVA: 0x000E31F8 File Offset: 0x000E13F8
		private void Update()
		{
			Vector3 vector = base.transform.position;
			Vector3 b = Vector3.up * Time.deltaTime * 2f;
			if (this.goingUp)
			{
				vector += b;
			}
			else
			{
				vector -= b;
			}
			if (vector.y > this.initialY + this.YChange)
			{
				this.goingUp = false;
			}
			else if (vector.y < this.initialY - this.YChange)
			{
				this.goingUp = true;
			}
			Vector3 vector2 = Vector3.MoveTowards(vector, this.to, Time.deltaTime * 2f);
			if ((double)Mathf.Abs(vector2.x - this.to.x) < 0.1 && (double)Mathf.Abs(vector2.z - this.to.z) < 0.1)
			{
				Vector3 vector3 = this.to;
				this.to = this.from;
				this.from = vector3;
			}
			base.transform.position = vector2;
			if (!this.Curve.SnapMonitoring)
			{
				BGCurve.SnapTypeEnum snapType = this.Curve.SnapType;
				if (snapType == BGCurve.SnapTypeEnum.Points)
				{
					this.Curve.ApplySnapping();
					return;
				}
				if (snapType != BGCurve.SnapTypeEnum.Curve)
				{
					return;
				}
				if (!this.Curve.ApplySnapping())
				{
					this.Curve.GetComponent<BGCcMath>().Recalculate(false);
				}
			}
		}

		// Token: 0x04000EA1 RID: 3745
		public BGCurve Curve;

		// Token: 0x04000EA2 RID: 3746
		public float XChange = 5f;

		// Token: 0x04000EA3 RID: 3747
		public float YChange = 0.5f;

		// Token: 0x04000EA4 RID: 3748
		private bool goingUp = true;

		// Token: 0x04000EA5 RID: 3749
		private Vector3 from;

		// Token: 0x04000EA6 RID: 3750
		private Vector3 to;

		// Token: 0x04000EA7 RID: 3751
		private float initialY;
	}
}
