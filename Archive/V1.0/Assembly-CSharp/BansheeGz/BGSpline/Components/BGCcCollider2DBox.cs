using System;
using System.Collections.Generic;
using System.Reflection;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001A3 RID: 419
	[HelpURL("http://www.bansheegz.com/BGCurve/Cc/BGCcCollider2DBox")]
	[BGCc.CcDescriptor(Description = "Create a set of Box 2D colliders along 2D spline.", Name = "Collider 2D Box", Icon = "BGCcCollider2DBox123")]
	[AddComponentMenu("BansheeGz/BGCurve/Components/BGCcCollider2DBox")]
	public class BGCcCollider2DBox : BGCcColliderAbstract<BoxCollider2D>
	{
		// Token: 0x170003E8 RID: 1000
		// (get) Token: 0x06000EB4 RID: 3764 RVA: 0x0004C839 File Offset: 0x0004AA39
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

		// Token: 0x170003E9 RID: 1001
		// (get) Token: 0x06000EB5 RID: 3765 RVA: 0x0004C853 File Offset: 0x0004AA53
		// (set) Token: 0x06000EB6 RID: 3766 RVA: 0x0004C85B File Offset: 0x0004AA5B
		public bool UsedByComposite
		{
			get
			{
				return this.usedByComposite;
			}
			set
			{
				base.ParamChanged<bool>(ref this.usedByComposite, value);
			}
		}

		// Token: 0x170003EA RID: 1002
		// (get) Token: 0x06000EB7 RID: 3767 RVA: 0x0004C86B File Offset: 0x0004AA6B
		// (set) Token: 0x06000EB8 RID: 3768 RVA: 0x0004C873 File Offset: 0x0004AA73
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

		// Token: 0x170003EB RID: 1003
		// (get) Token: 0x06000EB9 RID: 3769 RVA: 0x0004C883 File Offset: 0x0004AA83
		// (set) Token: 0x06000EBA RID: 3770 RVA: 0x0004C88B File Offset: 0x0004AA8B
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

		// Token: 0x170003EC RID: 1004
		// (get) Token: 0x06000EBB RID: 3771 RVA: 0x0004C89B File Offset: 0x0004AA9B
		// (set) Token: 0x06000EBC RID: 3772 RVA: 0x0004C8A3 File Offset: 0x0004AAA3
		public float Height
		{
			get
			{
				return this.height;
			}
			set
			{
				base.ParamChanged<float>(ref this.height, value);
			}
		}

		// Token: 0x170003ED RID: 1005
		// (get) Token: 0x06000EBD RID: 3773 RVA: 0x0004C8B3 File Offset: 0x0004AAB3
		// (set) Token: 0x06000EBE RID: 3774 RVA: 0x0004C8BB File Offset: 0x0004AABB
		public float HeightOffset
		{
			get
			{
				return this.heightOffset;
			}
			set
			{
				base.ParamChanged<float>(ref this.heightOffset, value);
			}
		}

		// Token: 0x170003EE RID: 1006
		// (get) Token: 0x06000EBF RID: 3775 RVA: 0x0004C8CB File Offset: 0x0004AACB
		// (set) Token: 0x06000EC0 RID: 3776 RVA: 0x0004C8D3 File Offset: 0x0004AAD3
		public bool UsedByEffector
		{
			get
			{
				return this.usedByEffector;
			}
			set
			{
				base.ParamChanged<bool>(ref this.usedByEffector, value);
			}
		}

		// Token: 0x170003EF RID: 1007
		// (get) Token: 0x06000EC1 RID: 3777 RVA: 0x0004C8E3 File Offset: 0x0004AAE3
		// (set) Token: 0x06000EC2 RID: 3778 RVA: 0x0004C8EB File Offset: 0x0004AAEB
		public PhysicsMaterial2D Material
		{
			get
			{
				return this.material;
			}
			set
			{
				base.ParamChanged<PhysicsMaterial2D>(ref this.material, value);
			}
		}

		// Token: 0x170003F0 RID: 1008
		// (get) Token: 0x06000EC3 RID: 3779 RVA: 0x0004C8FB File Offset: 0x0004AAFB
		// (set) Token: 0x06000EC4 RID: 3780 RVA: 0x0004C903 File Offset: 0x0004AB03
		public Effector2D Effector
		{
			get
			{
				return this.effector;
			}
			set
			{
				base.ParamChanged<Effector2D>(ref this.effector, value);
			}
		}

		// Token: 0x170003F1 RID: 1009
		// (get) Token: 0x06000EC5 RID: 3781 RVA: 0x0004C913 File Offset: 0x0004AB13
		// (set) Token: 0x06000EC6 RID: 3782 RVA: 0x0004C91B File Offset: 0x0004AB1B
		public Rigidbody2D CustomRigidbody
		{
			get
			{
				return this.Rigidbody;
			}
			set
			{
				base.ParamChanged<Rigidbody2D>(ref this.Rigidbody, value);
			}
		}

		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x06000EC7 RID: 3783 RVA: 0x0004C92B File Offset: 0x0004AB2B
		// (set) Token: 0x06000EC8 RID: 3784 RVA: 0x0004C933 File Offset: 0x0004AB33
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

		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x06000EC9 RID: 3785 RVA: 0x0004C943 File Offset: 0x0004AB43
		protected override List<BoxCollider2D> WorkingList
		{
			get
			{
				return BGCcCollider2DBox.TempColliders;
			}
		}

		// Token: 0x06000ECA RID: 3786 RVA: 0x0004C94A File Offset: 0x0004AB4A
		public override void UpdateUi()
		{
			if (base.Curve.Mode2D != BGCurve.Mode2DEnum.XY)
			{
				return;
			}
			base.UpdateUi();
		}

		// Token: 0x06000ECB RID: 3787 RVA: 0x000DB164 File Offset: 0x000D9364
		protected override void CheckCollider(Component component)
		{
			Type type = (this.effector != null) ? this.effector.GetType() : null;
			BGCcCollider2DBox.TempEffectors.Clear();
			component.GetComponents<Effector2D>(BGCcCollider2DBox.TempEffectors);
			Effector2D effector2D = null;
			for (int i = BGCcCollider2DBox.TempEffectors.Count - 1; i >= 0; i--)
			{
				Effector2D effector2D2 = BGCcCollider2DBox.TempEffectors[i];
				if (type == effector2D2.GetType())
				{
					effector2D = effector2D2;
				}
				else
				{
					BGCurve.DestroyIt(effector2D2);
				}
			}
			BGCcCollider2DBox.TempEffectors.Clear();
			if (this.effector == null)
			{
				return;
			}
			if (effector2D == null)
			{
				effector2D = (Effector2D)component.gameObject.AddComponent(type);
			}
			foreach (FieldInfo fieldInfo in this.GetFields(type))
			{
				if (!fieldInfo.IsStatic)
				{
					fieldInfo.SetValue(effector2D, fieldInfo.GetValue(this.effector));
				}
			}
			foreach (PropertyInfo propertyInfo in this.GetProperties(type))
			{
				if (propertyInfo.CanWrite && propertyInfo.CanWrite && !(propertyInfo.Name == "name"))
				{
					propertyInfo.SetValue(effector2D, propertyInfo.GetValue(this.effector, null), null);
				}
			}
		}

		// Token: 0x06000ECC RID: 3788 RVA: 0x000DB2B4 File Offset: 0x000D94B4
		protected override void SetUpGoCollider(BoxCollider2D collider, Vector3 from, Vector3 to)
		{
			Vector3 vector = to - from;
			float num = Vector3.Angle(Vector3.right, vector);
			num = ((vector.y < 0f) ? (360f - num) : num);
			collider.transform.rotation = Quaternion.Euler(0f, 0f, num);
			collider.transform.position = from;
			float num2 = vector.magnitude + this.LengthExtends;
			collider.offset = new Vector3(num2 * 0.5f - this.LengthExtends * 0.5f, this.heightOffset);
			collider.size = new Vector2(num2, this.height);
			collider.isTrigger = this.IsTrigger;
			collider.sharedMaterial = this.Material;
			collider.usedByEffector = this.usedByEffector;
			Rigidbody2D rigidbody2D = collider.gameObject.GetComponent<Rigidbody2D>();
			if (this.generateKinematicRigidbody || this.Rigidbody != null)
			{
				if (rigidbody2D == null)
				{
					rigidbody2D = collider.gameObject.AddComponent<Rigidbody2D>();
				}
				if (this.generateKinematicRigidbody)
				{
					rigidbody2D.isKinematic = true;
				}
				else
				{
					rigidbody2D.mass = this.Rigidbody.mass;
					rigidbody2D.drag = this.Rigidbody.drag;
					rigidbody2D.angularDrag = this.Rigidbody.angularDrag;
					rigidbody2D.gravityScale = this.Rigidbody.gravityScale;
					rigidbody2D.isKinematic = this.Rigidbody.isKinematic;
					rigidbody2D.interpolation = this.Rigidbody.interpolation;
					rigidbody2D.sleepMode = this.Rigidbody.sleepMode;
					rigidbody2D.collisionDetectionMode = this.Rigidbody.collisionDetectionMode;
					rigidbody2D.constraints = this.Rigidbody.constraints;
				}
			}
			else if (rigidbody2D != null)
			{
				BGCurve.DestroyIt(rigidbody2D);
			}
			if (this.usedByComposite)
			{
				collider.usedByComposite = true;
			}
		}

		// Token: 0x06000ECD RID: 3789 RVA: 0x0004C961 File Offset: 0x0004AB61
		private FieldInfo[] GetFields(Type type)
		{
			return type.GetFields();
		}

		// Token: 0x06000ECE RID: 3790 RVA: 0x0004C969 File Offset: 0x0004AB69
		private PropertyInfo[] GetProperties(Type type)
		{
			return type.GetProperties();
		}

		// Token: 0x04000CFA RID: 3322
		private static readonly List<BoxCollider2D> TempColliders = new List<BoxCollider2D>();

		// Token: 0x04000CFB RID: 3323
		private static readonly List<Effector2D> TempEffectors = new List<Effector2D>();

		// Token: 0x04000CFC RID: 3324
		[SerializeField]
		[Tooltip("Height of the colliders")]
		private float height = 1f;

		// Token: 0x04000CFD RID: 3325
		[SerializeField]
		[Tooltip("Height offset for colliders")]
		private float heightOffset;

		// Token: 0x04000CFE RID: 3326
		[SerializeField]
		[Tooltip("Set BoxCollider2D usedByEffector flag")]
		private bool usedByEffector;

		// Token: 0x04000CFF RID: 3327
		[SerializeField]
		[Tooltip("Material for colliders")]
		private PhysicsMaterial2D material;

		// Token: 0x04000D00 RID: 3328
		[SerializeField]
		[Tooltip("Extends for colliders length")]
		private float lengthExtends;

		// Token: 0x04000D01 RID: 3329
		[SerializeField]
		[Tooltip("Effector for colliders")]
		private Effector2D effector;

		// Token: 0x04000D02 RID: 3330
		[SerializeField]
		[Tooltip("If colliders should be triggers")]
		private bool isTrigger;

		// Token: 0x04000D03 RID: 3331
		[SerializeField]
		[Tooltip("Generate kinematic rigidbody for generated colliders. Rigidbody is a must If you plan to move/change colliders at runtime, otherwise do not use it")]
		private bool generateKinematicRigidbody;

		// Token: 0x04000D04 RID: 3332
		[SerializeField]
		[Tooltip("Rigidbody for generated colliders. Rigidbody is a must if you plan to move/change colliders at runtime, otherwise do not use it")]
		private Rigidbody2D Rigidbody;

		// Token: 0x04000D05 RID: 3333
		[SerializeField]
		[Tooltip("If colliders are used by composite collider")]
		private bool usedByComposite;
	}
}
