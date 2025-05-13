using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace SoL.Utilities.Splines
{
	// Token: 0x02000308 RID: 776
	[ExecuteAlways]
	public class MoveAlongSpline : MonoBehaviour
	{
		// Token: 0x060015AD RID: 5549 RVA: 0x000513D3 File Offset: 0x0004F5D3
		private int GetMaxSplineIndex()
		{
			if (!this.m_spline)
			{
				return 0;
			}
			return this.m_spline.Splines.Count - 1;
		}

		// Token: 0x1700052B RID: 1323
		// (get) Token: 0x060015AE RID: 5550 RVA: 0x000FD770 File Offset: 0x000FB970
		private float Length
		{
			get
			{
				if (this.m_length == null && this.m_spline)
				{
					this.m_length = new float?(this.m_spline.Splines[this.m_splineIndex].GetLength());
				}
				return this.m_length.Value;
			}
		}

		// Token: 0x060015AF RID: 5551 RVA: 0x000FD7C8 File Offset: 0x000FB9C8
		private void Update()
		{
			if (this.m_spline)
			{
				this.m_positionAlongSpline += this.m_speed * Time.deltaTime / this.Length;
				if (this.m_positionAlongSpline > 1f)
				{
					this.m_positionAlongSpline = 0f;
				}
				float3 v = this.m_spline.EvaluatePosition(this.m_splineIndex, this.m_positionAlongSpline);
				if (this.m_toMove)
				{
					this.m_toMove.transform.position = v;
				}
			}
		}

		// Token: 0x04001DBC RID: 7612
		[SerializeField]
		private SplineContainer m_spline;

		// Token: 0x04001DBD RID: 7613
		[SerializeField]
		private int m_splineIndex;

		// Token: 0x04001DBE RID: 7614
		[SerializeField]
		private GameObject m_toMove;

		// Token: 0x04001DBF RID: 7615
		[SerializeField]
		private float m_speed = 1f;

		// Token: 0x04001DC0 RID: 7616
		private float m_positionAlongSpline;

		// Token: 0x04001DC1 RID: 7617
		private float? m_length;
	}
}
