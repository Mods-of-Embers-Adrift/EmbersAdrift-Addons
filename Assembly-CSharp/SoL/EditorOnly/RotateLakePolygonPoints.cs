using System;
using UnityEngine;

namespace SoL.EditorOnly
{
	// Token: 0x02000DD3 RID: 3539
	public class RotateLakePolygonPoints : MonoBehaviour
	{
		// Token: 0x06006962 RID: 26978 RVA: 0x00217664 File Offset: 0x00215864
		private void RotatePoints()
		{
			if (this.m_angle == 0f || this.m_lakePolygon == null || this.m_lakePolygon.points == null || this.m_lakePolygon.points.Count <= 0)
			{
				return;
			}
			Quaternion rotation = Quaternion.Euler(0f, this.m_angle, 0f);
			for (int i = 0; i < this.m_lakePolygon.points.Count; i++)
			{
				Vector3 point = this.m_lakePolygon.points[i];
				this.m_lakePolygon.points[i] = rotation * point;
			}
		}

		// Token: 0x04005BD3 RID: 23507
		[SerializeField]
		private LakePolygon m_lakePolygon;

		// Token: 0x04005BD4 RID: 23508
		[SerializeField]
		private float m_angle;
	}
}
