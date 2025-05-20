using System;
using System.Collections.Generic;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001A6 RID: 422
	public abstract class BGCcCollider3DAbstract<T> : BGCcColliderAbstract<T> where T : Collider
	{
		// Token: 0x170003F8 RID: 1016
		// (get) Token: 0x06000EDD RID: 3805 RVA: 0x0004C9F2 File Offset: 0x0004ABF2
		protected override List<T> WorkingList
		{
			get
			{
				return BGCcCollider3DAbstract<T>.TempColliders;
			}
		}

		// Token: 0x170003F9 RID: 1017
		// (get) Token: 0x06000EDE RID: 3806 RVA: 0x0004C9F9 File Offset: 0x0004ABF9
		// (set) Token: 0x06000EDF RID: 3807 RVA: 0x0004CA01 File Offset: 0x0004AC01
		public bool IsTrigger
		{
			get
			{
				return this.isTrigger;
			}
			set
			{
				base.ParamChanged<bool>(ref this.isTrigger, value);
			}
		}

		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x06000EE0 RID: 3808 RVA: 0x0004CA11 File Offset: 0x0004AC11
		// (set) Token: 0x06000EE1 RID: 3809 RVA: 0x0004CA19 File Offset: 0x0004AC19
		public PhysicMaterial Material
		{
			get
			{
				return this.material;
			}
			set
			{
				base.ParamChanged<PhysicMaterial>(ref this.material, value);
			}
		}

		// Token: 0x170003FB RID: 1019
		// (get) Token: 0x06000EE2 RID: 3810 RVA: 0x0004CA29 File Offset: 0x0004AC29
		// (set) Token: 0x06000EE3 RID: 3811 RVA: 0x0004CA31 File Offset: 0x0004AC31
		public BGCcCollider3DAbstract<T>.HeightAxisModeEnum HeightAxisMode
		{
			get
			{
				return this.heightAxisMode;
			}
			set
			{
				base.ParamChanged<BGCcCollider3DAbstract<T>.HeightAxisModeEnum>(ref this.heightAxisMode, value);
			}
		}

		// Token: 0x170003FC RID: 1020
		// (get) Token: 0x06000EE4 RID: 3812 RVA: 0x0004CA41 File Offset: 0x0004AC41
		// (set) Token: 0x06000EE5 RID: 3813 RVA: 0x0004CA49 File Offset: 0x0004AC49
		public Vector3 CustomHeightAxis
		{
			get
			{
				return this.customHeightAxis;
			}
			set
			{
				base.ParamChanged<Vector3>(ref this.customHeightAxis, value);
			}
		}

		// Token: 0x170003FD RID: 1021
		// (get) Token: 0x06000EE6 RID: 3814 RVA: 0x0004CA59 File Offset: 0x0004AC59
		// (set) Token: 0x06000EE7 RID: 3815 RVA: 0x0004CA61 File Offset: 0x0004AC61
		public float HeightAxisRotation
		{
			get
			{
				return this.heightAxisRotation;
			}
			set
			{
				base.ParamChanged<float>(ref this.heightAxisRotation, value);
			}
		}

		// Token: 0x170003FE RID: 1022
		// (get) Token: 0x06000EE8 RID: 3816 RVA: 0x0004CA71 File Offset: 0x0004AC71
		// (set) Token: 0x06000EE9 RID: 3817 RVA: 0x0004CA79 File Offset: 0x0004AC79
		public bool GenerateKinematicRigidbody
		{
			get
			{
				return this.generateKinematicRigidbody;
			}
			set
			{
				base.ParamChanged<bool>(ref this.generateKinematicRigidbody, value);
			}
		}

		// Token: 0x170003FF RID: 1023
		// (get) Token: 0x06000EEA RID: 3818 RVA: 0x0004CA89 File Offset: 0x0004AC89
		// (set) Token: 0x06000EEB RID: 3819 RVA: 0x0004CA91 File Offset: 0x0004AC91
		public Rigidbody CustomRigidbody
		{
			get
			{
				return this.Rigidbody;
			}
			set
			{
				base.ParamChanged<Rigidbody>(ref this.Rigidbody, value);
			}
		}

		// Token: 0x17000400 RID: 1024
		// (get) Token: 0x06000EEC RID: 3820 RVA: 0x000DB6E4 File Offset: 0x000D98E4
		protected Vector3 RotationUpAxis
		{
			get
			{
				Vector3 vector;
				switch (this.heightAxisMode)
				{
				case BGCcCollider3DAbstract<T>.HeightAxisModeEnum.Y:
					vector = Vector3.up;
					break;
				case BGCcCollider3DAbstract<T>.HeightAxisModeEnum.X:
					vector = Vector3.right;
					break;
				case BGCcCollider3DAbstract<T>.HeightAxisModeEnum.Z:
					vector = Vector3.forward;
					break;
				case BGCcCollider3DAbstract<T>.HeightAxisModeEnum.Custom:
					vector = this.customHeightAxis;
					if (vector == Vector3.zero)
					{
						vector = Vector3.up;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException("HeightAxisMode");
				}
				return vector;
			}
		}

		// Token: 0x06000EED RID: 3821 RVA: 0x000DB750 File Offset: 0x000D9950
		protected void SetUpRigidBody(GameObject go)
		{
			Rigidbody rigidbody = go.GetComponent<Rigidbody>();
			if (!this.generateKinematicRigidbody && !(this.Rigidbody != null))
			{
				if (rigidbody != null)
				{
					BGCurve.DestroyIt(rigidbody);
				}
				return;
			}
			if (rigidbody == null)
			{
				rigidbody = go.AddComponent<Rigidbody>();
			}
			if (this.generateKinematicRigidbody)
			{
				rigidbody.isKinematic = true;
				return;
			}
			rigidbody.mass = this.Rigidbody.mass;
			rigidbody.drag = this.Rigidbody.drag;
			rigidbody.angularDrag = this.Rigidbody.angularDrag;
			rigidbody.useGravity = this.Rigidbody.useGravity;
			rigidbody.isKinematic = this.Rigidbody.isKinematic;
			rigidbody.interpolation = this.Rigidbody.interpolation;
			rigidbody.collisionDetectionMode = this.Rigidbody.collisionDetectionMode;
			rigidbody.constraints = this.Rigidbody.constraints;
		}

		// Token: 0x04000D07 RID: 3335
		private static readonly List<T> TempColliders = new List<T>();

		// Token: 0x04000D08 RID: 3336
		[SerializeField]
		[Tooltip("Material for colliders")]
		private PhysicMaterial material;

		// Token: 0x04000D09 RID: 3337
		[SerializeField]
		[Tooltip("Height Axis direction.")]
		private BGCcCollider3DAbstract<T>.HeightAxisModeEnum heightAxisMode;

		// Token: 0x04000D0A RID: 3338
		[SerializeField]
		[Tooltip("Custom Height Axis for Custom height axis mode")]
		private Vector3 customHeightAxis = Vector3.up;

		// Token: 0x04000D0B RID: 3339
		[Range(0f, 360f)]
		[SerializeField]
		[Tooltip("Height axis rotation in degrees. Default is 0, which is Vector.up. 90 is Vector.right, 180 is Vector.down and 270 is Vector.left.")]
		private float heightAxisRotation;

		// Token: 0x04000D0C RID: 3340
		[SerializeField]
		[Tooltip("If colliders should be triggers")]
		private bool isTrigger;

		// Token: 0x04000D0D RID: 3341
		[SerializeField]
		[Tooltip("Generate kinematic rigidbody for generated colliders")]
		private bool generateKinematicRigidbody;

		// Token: 0x04000D0E RID: 3342
		[SerializeField]
		[Tooltip("Custom Rigidbody for generated colliders")]
		private Rigidbody Rigidbody;

		// Token: 0x020001A7 RID: 423
		public enum HeightAxisModeEnum
		{
			// Token: 0x04000D10 RID: 3344
			Y,
			// Token: 0x04000D11 RID: 3345
			X,
			// Token: 0x04000D12 RID: 3346
			Z,
			// Token: 0x04000D13 RID: 3347
			Custom
		}
	}
}
