using System;
using UnityEngine;

namespace BansheeGz.BGSpline.Example
{
	// Token: 0x020001DD RID: 477
	public class BGTestCurveChange : MonoBehaviour
	{
		// Token: 0x060010F0 RID: 4336 RVA: 0x000E1430 File Offset: 0x000DF630
		private void Update()
		{
			base.transform.RotateAround(base.transform.position, Vector3.up, 40f * Time.deltaTime);
			Vector3 vector = base.transform.localScale;
			bool flag = vector.x > 1.25f;
			bool flag2 = vector.x < 0.5f;
			if (flag || flag2)
			{
				this.scaleSpeed = -this.scaleSpeed;
				vector = (flag ? new Vector3(1.25f, 1.25f, 1.25f) : new Vector3(0.5f, 0.5f, 0.5f));
			}
			base.transform.localScale = vector + this.scaleSpeed * Time.deltaTime;
		}

		// Token: 0x04000E18 RID: 3608
		private const float RotationSpeed = 40f;

		// Token: 0x04000E19 RID: 3609
		private const float ScaleUpperLimit = 1.25f;

		// Token: 0x04000E1A RID: 3610
		private const float ScaleLowerLimit = 0.5f;

		// Token: 0x04000E1B RID: 3611
		private Vector3 scaleSpeed = Vector3.one * 0.1f;
	}
}
