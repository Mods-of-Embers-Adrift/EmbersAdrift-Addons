using System;
using System.Collections.Generic;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001A9 RID: 425
	[HelpURL("http://www.bansheegz.com/BGCurve/Cc/BGCcCollider3DMesh")]
	[BGCc.CcDescriptor(Description = "Create a mesh collider inside 2D spline.", Name = "Collider 3D Mesh", Icon = "BGCcCollider3DMesh123")]
	[RequireComponent(typeof(MeshCollider))]
	[AddComponentMenu("BansheeGz/BGCurve/Components/BGCcCollider3DMesh")]
	public class BGCcCollider3DMesh : BGCcColliderMeshAbstract<MeshCollider>
	{
		// Token: 0x17000407 RID: 1031
		// (get) Token: 0x06000EFF RID: 3839 RVA: 0x0004CB6E File Offset: 0x0004AD6E
		// (set) Token: 0x06000F00 RID: 3840 RVA: 0x0004CB76 File Offset: 0x0004AD76
		public bool Flip
		{
			get
			{
				return this.flip;
			}
			set
			{
				if (this.flip == value)
				{
					return;
				}
				base.ParamChanged<bool>(ref this.flip, value);
			}
		}

		// Token: 0x17000408 RID: 1032
		// (get) Token: 0x06000F01 RID: 3841 RVA: 0x0004CB90 File Offset: 0x0004AD90
		// (set) Token: 0x06000F02 RID: 3842 RVA: 0x0004CB98 File Offset: 0x0004AD98
		public bool DoubleSided
		{
			get
			{
				return this.doubleSided;
			}
			set
			{
				base.ParamChanged<bool>(ref this.doubleSided, value);
			}
		}

		// Token: 0x17000409 RID: 1033
		// (get) Token: 0x06000F03 RID: 3843 RVA: 0x0004CBA8 File Offset: 0x0004ADA8
		// (set) Token: 0x06000F04 RID: 3844 RVA: 0x0004CBB0 File Offset: 0x0004ADB0
		public bool MeshDoubleSided
		{
			get
			{
				return this.meshDoubleSided;
			}
			set
			{
				base.ParamChanged<bool>(ref this.meshDoubleSided, value);
			}
		}

		// Token: 0x1700040A RID: 1034
		// (get) Token: 0x06000F05 RID: 3845 RVA: 0x0004CBC0 File Offset: 0x0004ADC0
		// (set) Token: 0x06000F06 RID: 3846 RVA: 0x0004CBC8 File Offset: 0x0004ADC8
		public bool MeshFlip
		{
			get
			{
				return this.meshFlip;
			}
			set
			{
				base.ParamChanged<bool>(ref this.meshFlip, value);
			}
		}

		// Token: 0x06000F07 RID: 3847 RVA: 0x000DBD34 File Offset: 0x000D9F34
		protected override void Build(MeshCollider collider, List<Vector3> positions)
		{
			Mesh mesh = collider.sharedMesh;
			if (mesh == null)
			{
				mesh = new Mesh();
			}
			if (this.triangulator == null)
			{
				this.triangulator = new BGTriangulator2D();
			}
			this.triangulator.Bind(mesh, positions, new BGTriangulator2D.Config
			{
				Closed = base.Curve.Closed,
				Mode2D = base.Curve.Mode2D,
				Flip = this.flip,
				ScaleUV = Vector2.one,
				OffsetUV = Vector2.zero,
				DoubleSided = this.doubleSided,
				ScaleBackUV = Vector2.one,
				OffsetBackUV = Vector2.zero
			});
			collider.sharedMesh = mesh;
			if (base.IsMeshGenerationOn)
			{
				base.Get<MeshRenderer>();
				MeshFilter meshFilter = base.Get<MeshFilter>();
				Mesh mesh2 = meshFilter.sharedMesh;
				if (mesh2 == null)
				{
					mesh2 = new Mesh();
				}
				this.triangulator.Bind(mesh2, positions, new BGTriangulator2D.Config
				{
					Closed = base.Curve.Closed,
					Mode2D = base.Curve.Mode2D,
					Flip = this.meshFlip,
					ScaleUV = Vector2.one,
					OffsetUV = Vector2.zero,
					DoubleSided = this.meshDoubleSided,
					ScaleBackUV = Vector2.one,
					OffsetBackUV = Vector2.zero
				});
				meshFilter.sharedMesh = mesh2;
			}
		}

		// Token: 0x04000D1A RID: 3354
		[SerializeField]
		[Tooltip("Double sided")]
		private bool doubleSided;

		// Token: 0x04000D1B RID: 3355
		[SerializeField]
		[Tooltip("Flip triangles")]
		private bool flip;

		// Token: 0x04000D1C RID: 3356
		[SerializeField]
		[Tooltip("Generated mesh double sided")]
		private bool meshDoubleSided;

		// Token: 0x04000D1D RID: 3357
		[SerializeField]
		[Tooltip("Generated mesh flipped")]
		private bool meshFlip;

		// Token: 0x04000D1E RID: 3358
		private BGTriangulator2D triangulator;
	}
}
