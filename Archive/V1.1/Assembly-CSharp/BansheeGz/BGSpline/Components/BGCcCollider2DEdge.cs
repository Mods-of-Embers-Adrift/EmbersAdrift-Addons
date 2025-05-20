using System;
using System.Collections.Generic;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001A4 RID: 420
	[HelpURL("http://www.bansheegz.com/BGCurve/Cc/BGCcCollider2DEdge")]
	[BGCc.CcDescriptor(Description = "Create a set of Edge 2D colliders along 2D spline.", Name = "Collider 2D Edge", Icon = "BGCcCollider2DEdge123")]
	[RequireComponent(typeof(EdgeCollider2D))]
	[AddComponentMenu("BansheeGz/BGCurve/Components/BGCcCollider2DEdge")]
	public class BGCcCollider2DEdge : BGCcColliderAbstract<EdgeCollider2D>
	{
		// Token: 0x170003F4 RID: 1012
		// (get) Token: 0x06000ED2 RID: 3794 RVA: 0x0004C9B1 File Offset: 0x0004ABB1
		public override string Error
		{
			get
			{
				return base.ChoseMessage(base.Error, delegate
				{
					if (base.Curve.Mode2D == BGCurve.Mode2DEnum.XY)
					{
						return null;
					}
					return "Curve should be in XY 2D mode";
				});
			}
		}

		// Token: 0x170003F5 RID: 1013
		// (get) Token: 0x06000ED3 RID: 3795 RVA: 0x00045BCA File Offset: 0x00043DCA
		public override bool RequireGameObjects
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x06000ED4 RID: 3796 RVA: 0x00049FFA File Offset: 0x000481FA
		protected override List<EdgeCollider2D> WorkingList
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170003F7 RID: 1015
		// (get) Token: 0x06000ED5 RID: 3797 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool LocalSpace
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000ED6 RID: 3798 RVA: 0x0004C9CB File Offset: 0x0004ABCB
		public override void UpdateUi()
		{
			if (base.Curve.Mode2D != BGCurve.Mode2DEnum.XY)
			{
				return;
			}
			base.UpdateUi();
		}

		// Token: 0x06000ED7 RID: 3799 RVA: 0x000DB48C File Offset: 0x000D968C
		protected override void FillSingleCollider(List<Vector3> positions, int count)
		{
			EdgeCollider2D component = base.GetComponent<EdgeCollider2D>();
			if (component == null)
			{
				return;
			}
			List<Vector3> positions2 = base.Positions;
			int count2 = positions2.Count;
			Vector2[] array = new Vector2[count2];
			for (int i = 0; i < count2; i++)
			{
				array[i] = positions2[i];
			}
			component.points = array;
		}
	}
}
