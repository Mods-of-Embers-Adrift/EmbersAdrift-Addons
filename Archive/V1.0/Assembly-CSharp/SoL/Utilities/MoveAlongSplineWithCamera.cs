using System;
using BansheeGz.BGSpline.Components;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x0200029F RID: 671
	public class MoveAlongSplineWithCamera : MonoBehaviour
	{
		// Token: 0x06001433 RID: 5171 RVA: 0x000F9598 File Offset: 0x000F7798
		private void Start()
		{
			if (Camera.main != null && this.m_cursor != null && this.m_math != null)
			{
				this.m_cameraTransform = Camera.main.transform;
				return;
			}
			base.enabled = false;
		}

		// Token: 0x06001434 RID: 5172 RVA: 0x000F95E8 File Offset: 0x000F77E8
		private void Update()
		{
			if (this.m_cameraTransform)
			{
				float distance;
				this.m_math.CalcPositionByClosestPoint(this.m_cameraTransform.position, out distance, false, false);
				this.m_cursor.Distance = distance;
			}
		}

		// Token: 0x04001C8D RID: 7309
		[SerializeField]
		private BGCcMath m_math;

		// Token: 0x04001C8E RID: 7310
		[SerializeField]
		private BGCcCursor m_cursor;

		// Token: 0x04001C8F RID: 7311
		private Transform m_cameraTransform;
	}
}
