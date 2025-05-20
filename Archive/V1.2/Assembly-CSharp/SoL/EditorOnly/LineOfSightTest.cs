using System;
using SoL.Utilities;
using UnityEngine;

namespace SoL.EditorOnly
{
	// Token: 0x02000DCA RID: 3530
	public class LineOfSightTest : MonoBehaviour
	{
		// Token: 0x06006952 RID: 26962 RVA: 0x00086A01 File Offset: 0x00084C01
		private void OnDrawGizmos()
		{
			if (this.m_alwaysDraw)
			{
				this.DoTheGizmoDraw();
			}
		}

		// Token: 0x06006953 RID: 26963 RVA: 0x00086A11 File Offset: 0x00084C11
		private void OnDrawGizmosSelected()
		{
			if (!this.m_alwaysDraw)
			{
				this.DoTheGizmoDraw();
			}
		}

		// Token: 0x06006954 RID: 26964 RVA: 0x0021764C File Offset: 0x0021584C
		private void DoTheGizmoDraw()
		{
			if (this.m_target != null)
			{
				Gizmos.color = (LineOfSight.HasLineOfSight(base.gameObject.transform.position, this.m_target.transform.position) ? Color.blue : Color.red);
				Gizmos.DrawLine(base.gameObject.transform.position, this.m_target.transform.position);
			}
		}

		// Token: 0x04005BB1 RID: 23473
		[SerializeField]
		private bool m_alwaysDraw;

		// Token: 0x04005BB2 RID: 23474
		[SerializeField]
		private GameObject m_target;
	}
}
