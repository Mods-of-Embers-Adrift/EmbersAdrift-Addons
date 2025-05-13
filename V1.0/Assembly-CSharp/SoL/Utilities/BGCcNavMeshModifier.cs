using System;
using System.Collections.Generic;
using BansheeGz.BGSpline.Components;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x0200024A RID: 586
	[BGCc.CcDescriptor(Description = "Create a set of NavMeshModifier volumes along 3D spline.", Name = "NavMeshModifier Volume", Icon = "BGCcCollider3DBox123")]
	public class BGCcNavMeshModifier : BGCcCollider3DBox
	{
		// Token: 0x06001334 RID: 4916 RVA: 0x0004F907 File Offset: 0x0004DB07
		private void Awake()
		{
			if (Application.isPlaying)
			{
				base.enabled = false;
			}
		}

		// Token: 0x06001335 RID: 4917 RVA: 0x0004F917 File Offset: 0x0004DB17
		protected override void SetUpGoCollider(BoxCollider col, Vector3 from, Vector3 to)
		{
			base.SetUpGoCollider(col, from, to);
			if (!col)
			{
				return;
			}
			col.enabled = false;
		}

		// Token: 0x040010F1 RID: 4337
		[SerializeField]
		private int m_area;

		// Token: 0x040010F2 RID: 4338
		[SerializeField]
		private List<int> m_affectedAgents = new List<int>(new int[]
		{
			-1
		});
	}
}
