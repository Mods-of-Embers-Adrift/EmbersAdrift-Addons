using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace SoL.Utilities.Splines
{
	// Token: 0x02000309 RID: 777
	public class SplineCopy : MonoBehaviour
	{
		// Token: 0x060015B1 RID: 5553 RVA: 0x000FD878 File Offset: 0x000FBA78
		private void CopyKnots()
		{
			if (!this.m_from || !this.m_to)
			{
				return;
			}
			List<Spline> list = new List<Spline>();
			foreach (Spline spline in this.m_from.Splines)
			{
				list.Add(new Spline(spline));
			}
			this.m_to.Splines = list;
			int num = 0;
			foreach (Spline spline2 in this.m_from.Splines)
			{
				Spline spline3 = this.m_to.Splines[num];
				int num2 = 0;
				foreach (BezierKnot bezierKnot in spline2)
				{
					Vector3 position = this.m_from.gameObject.transform.TransformPoint(bezierKnot.Position);
					Vector3 v = this.m_to.gameObject.transform.InverseTransformPoint(position);
					bezierKnot.Position = v;
					spline3.SetKnot(num2, bezierKnot, BezierTangent.Out);
					spline3.SetTangentMode(num2, spline2.GetTangentMode(num2), BezierTangent.Out);
					num2++;
				}
				num++;
			}
		}

		// Token: 0x04001DC2 RID: 7618
		[SerializeField]
		private SplineContainer m_from;

		// Token: 0x04001DC3 RID: 7619
		[SerializeField]
		private SplineContainer m_to;
	}
}
