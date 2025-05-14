using System;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001AC RID: 428
	[HelpURL("http://www.bansheegz.com/BGCurve/Cc/BGCcColliderCapsule")]
	[BGCc.CcDescriptor(Description = "Create a set of capsule colliders along 3D spline.", Name = "Collider Capsule", Icon = "BGCcColliderCapsule123")]
	[AddComponentMenu("BansheeGz/BGCurve/Components/BGCcColliderCapsule")]
	public class BGCcColliderCapsule : BGCcCollider3DAbstract<CapsuleCollider>
	{
		// Token: 0x17000417 RID: 1047
		// (get) Token: 0x06000F25 RID: 3877 RVA: 0x0004CCC8 File Offset: 0x0004AEC8
		// (set) Token: 0x06000F26 RID: 3878 RVA: 0x0004CCD0 File Offset: 0x0004AED0
		public float Radius
		{
			get
			{
				return this.radius;
			}
			set
			{
				base.ParamChanged<float>(ref this.radius, value);
			}
		}

		// Token: 0x17000418 RID: 1048
		// (get) Token: 0x06000F27 RID: 3879 RVA: 0x0004CCE0 File Offset: 0x0004AEE0
		// (set) Token: 0x06000F28 RID: 3880 RVA: 0x0004CCE8 File Offset: 0x0004AEE8
		public float LengthExtends
		{
			get
			{
				return this.lengthExtends;
			}
			set
			{
				base.ParamChanged<float>(ref this.lengthExtends, value);
			}
		}

		// Token: 0x06000F29 RID: 3881 RVA: 0x000DC1FC File Offset: 0x000DA3FC
		protected override void SetUpGoCollider(CapsuleCollider collider, Vector3 from, Vector3 to)
		{
			collider.transform.position = from;
			Vector3 forward = to - from;
			collider.transform.rotation = Quaternion.LookRotation(forward, base.RotationUpAxis);
			collider.transform.Rotate(Vector3.forward, base.HeightAxisRotation);
			collider.direction = 2;
			float magnitude = forward.magnitude;
			float height = magnitude + this.lengthExtends * 2f;
			collider.height = height;
			collider.center = new Vector3(0f, 0f, magnitude * 0.5f);
			collider.radius = this.radius;
			collider.isTrigger = base.IsTrigger;
			collider.material = base.Material;
			base.SetUpRigidBody(collider.gameObject);
		}

		// Token: 0x04000D26 RID: 3366
		[SerializeField]
		[Tooltip("Capsule radius ")]
		private float radius = 0.1f;

		// Token: 0x04000D27 RID: 3367
		[SerializeField]
		[Tooltip("Extends for colliders length")]
		private float lengthExtends;
	}
}
