using System;
using BansheeGz.BGSpline.Components;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x0200024B RID: 587
	[BGCc.CcDescriptor(Description = "Create a set of Trigger colliders along 3D spline.", Name = "Player Road Collider", Icon = "BGCcCollider3DBox123")]
	public class BGCcRoadCollider : BGCcCollider3DBox
	{
		// Token: 0x06001337 RID: 4919 RVA: 0x0004F907 File Offset: 0x0004DB07
		private void Awake()
		{
			if (Application.isPlaying)
			{
				base.enabled = false;
			}
		}

		// Token: 0x06001338 RID: 4920 RVA: 0x0004F94F File Offset: 0x0004DB4F
		protected override void SetUpGoCollider(BoxCollider col, Vector3 from, Vector3 to)
		{
			base.SetUpGoCollider(col, from, to);
			col;
		}
	}
}
