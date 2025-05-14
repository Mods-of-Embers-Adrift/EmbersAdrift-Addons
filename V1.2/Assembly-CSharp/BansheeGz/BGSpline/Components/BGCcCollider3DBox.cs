using System;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001A8 RID: 424
	[HelpURL("http://www.bansheegz.com/BGCurve/Cc/BGCcCollider3DBox")]
	[BGCc.CcDescriptor(Description = "Create a set of box colliders along 3D spline.", Name = "Collider 3D Box", Icon = "BGCcCollider3DBox123")]
	[AddComponentMenu("BansheeGz/BGCurve/Components/BGCcCollider3DBox")]
	public class BGCcCollider3DBox : BGCcCollider3DAbstract<BoxCollider>
	{
		// Token: 0x17000401 RID: 1025
		// (get) Token: 0x06000EF0 RID: 3824 RVA: 0x0004CAC0 File Offset: 0x0004ACC0
		// (set) Token: 0x06000EF1 RID: 3825 RVA: 0x0004CAC8 File Offset: 0x0004ACC8
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

		// Token: 0x17000402 RID: 1026
		// (get) Token: 0x06000EF2 RID: 3826 RVA: 0x0004CAD8 File Offset: 0x0004ACD8
		// (set) Token: 0x06000EF3 RID: 3827 RVA: 0x0004CAE0 File Offset: 0x0004ACE0
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

		// Token: 0x17000403 RID: 1027
		// (get) Token: 0x06000EF4 RID: 3828 RVA: 0x0004CAF0 File Offset: 0x0004ACF0
		// (set) Token: 0x06000EF5 RID: 3829 RVA: 0x0004CAF8 File Offset: 0x0004ACF8
		public float Width
		{
			get
			{
				return this.width;
			}
			set
			{
				base.ParamChanged<float>(ref this.width, value);
			}
		}

		// Token: 0x17000404 RID: 1028
		// (get) Token: 0x06000EF6 RID: 3830 RVA: 0x0004CB08 File Offset: 0x0004AD08
		// (set) Token: 0x06000EF7 RID: 3831 RVA: 0x0004CB10 File Offset: 0x0004AD10
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

		// Token: 0x17000405 RID: 1029
		// (get) Token: 0x06000EF8 RID: 3832 RVA: 0x0004CB20 File Offset: 0x0004AD20
		// (set) Token: 0x06000EF9 RID: 3833 RVA: 0x0004CB28 File Offset: 0x0004AD28
		public bool IsMeshGenerationOn
		{
			get
			{
				return this.isMeshGenerationOn;
			}
			set
			{
				base.ParamChanged<bool>(ref this.isMeshGenerationOn, value);
			}
		}

		// Token: 0x17000406 RID: 1030
		// (get) Token: 0x06000EFA RID: 3834 RVA: 0x0004CB38 File Offset: 0x0004AD38
		// (set) Token: 0x06000EFB RID: 3835 RVA: 0x0004CB40 File Offset: 0x0004AD40
		public Material MeshMaterialToGenerate
		{
			get
			{
				return this.MeshMaterial;
			}
			set
			{
				base.ParamChanged<Material>(ref this.MeshMaterial, value);
			}
		}

		// Token: 0x06000EFC RID: 3836 RVA: 0x000DB838 File Offset: 0x000D9A38
		protected override void SetUpGoCollider(BoxCollider collider, Vector3 from, Vector3 to)
		{
			collider.transform.position = from;
			Vector3 vector = to - from;
			collider.transform.rotation = Quaternion.LookRotation(vector, base.RotationUpAxis);
			collider.transform.Rotate(Vector3.forward, base.HeightAxisRotation);
			float num = vector.magnitude + this.LengthExtends;
			collider.center = new Vector3(0f, this.Height * 0.5f + this.heightOffset, num * 0.5f - this.LengthExtends * 0.5f);
			collider.size = new Vector3(this.width, this.Height, num);
			collider.isTrigger = base.IsTrigger;
			collider.material = base.Material;
			if (this.isMeshGenerationOn)
			{
				Vector3 offset = collider.transform.InverseTransformDirection(vector) * 0.5f + Vector3.up * (this.heightOffset + this.Height * 0.5f);
				this.GenerateMesh(collider, offset, this.width, this.Height, num);
			}
			base.SetUpRigidBody(collider.gameObject);
		}

		// Token: 0x06000EFD RID: 3837 RVA: 0x000DB960 File Offset: 0x000D9B60
		private void GenerateMesh(BoxCollider collider, Vector3 offset, float boxLength, float boxWidth, float boxHeight)
		{
			MeshRenderer meshRenderer = collider.GetComponent<MeshRenderer>();
			if (meshRenderer == null)
			{
				meshRenderer = collider.gameObject.AddComponent<MeshRenderer>();
			}
			meshRenderer.sharedMaterial = this.MeshMaterial;
			MeshFilter meshFilter = collider.GetComponent<MeshFilter>();
			if (meshFilter == null)
			{
				meshFilter = collider.gameObject.AddComponent<MeshFilter>();
			}
			Mesh mesh = meshFilter.sharedMesh;
			if (mesh == null)
			{
				mesh = new Mesh();
				meshFilter.sharedMesh = mesh;
			}
			Vector3 vector = new Vector3(-boxLength * 0.5f, -boxWidth * 0.5f, boxHeight * 0.5f) + offset;
			Vector3 vector2 = new Vector3(boxLength * 0.5f, -boxWidth * 0.5f, boxHeight * 0.5f) + offset;
			Vector3 vector3 = new Vector3(boxLength * 0.5f, -boxWidth * 0.5f, -boxHeight * 0.5f) + offset;
			Vector3 vector4 = new Vector3(-boxLength * 0.5f, -boxWidth * 0.5f, -boxHeight * 0.5f) + offset;
			Vector3 vector5 = new Vector3(-boxLength * 0.5f, boxWidth * 0.5f, boxHeight * 0.5f) + offset;
			Vector3 vector6 = new Vector3(boxLength * 0.5f, boxWidth * 0.5f, boxHeight * 0.5f) + offset;
			Vector3 vector7 = new Vector3(boxLength * 0.5f, boxWidth * 0.5f, -boxHeight * 0.5f) + offset;
			Vector3 vector8 = new Vector3(-boxLength * 0.5f, boxWidth * 0.5f, -boxHeight * 0.5f) + offset;
			Vector3[] vertices = new Vector3[]
			{
				vector,
				vector2,
				vector3,
				vector4,
				vector8,
				vector5,
				vector,
				vector4,
				vector5,
				vector6,
				vector2,
				vector,
				vector7,
				vector8,
				vector4,
				vector3,
				vector6,
				vector7,
				vector3,
				vector2,
				vector8,
				vector7,
				vector6,
				vector5
			};
			Vector2 vector9 = new Vector2(0f, 0f);
			Vector2 vector10 = new Vector2(1f, 0f);
			Vector2 vector11 = new Vector2(0f, 1f);
			Vector2 vector12 = new Vector2(1f, 1f);
			Vector2[] uv = new Vector2[]
			{
				vector12,
				vector11,
				vector9,
				vector10,
				vector12,
				vector11,
				vector9,
				vector10,
				vector12,
				vector11,
				vector9,
				vector10,
				vector12,
				vector11,
				vector9,
				vector10,
				vector12,
				vector11,
				vector9,
				vector10,
				vector12,
				vector11,
				vector9,
				vector10
			};
			int[] triangles = new int[]
			{
				3,
				1,
				0,
				3,
				2,
				1,
				7,
				5,
				4,
				7,
				6,
				5,
				11,
				9,
				8,
				11,
				10,
				9,
				15,
				13,
				12,
				15,
				14,
				13,
				19,
				17,
				16,
				19,
				18,
				17,
				23,
				21,
				20,
				23,
				22,
				21
			};
			mesh.vertices = vertices;
			mesh.uv = uv;
			mesh.triangles = triangles;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
		}

		// Token: 0x04000D14 RID: 3348
		[SerializeField]
		[Tooltip("Height of the colliders")]
		private float height = 1f;

		// Token: 0x04000D15 RID: 3349
		[SerializeField]
		[Tooltip("Width of the colliders")]
		private float width = 0.2f;

		// Token: 0x04000D16 RID: 3350
		[SerializeField]
		[Tooltip("Extends for colliders length")]
		private float lengthExtends;

		// Token: 0x04000D17 RID: 3351
		[SerializeField]
		[Tooltip("Offset by height")]
		private float heightOffset;

		// Token: 0x04000D18 RID: 3352
		[SerializeField]
		[Tooltip("If mesh should be generated along with colliders")]
		private bool isMeshGenerationOn;

		// Token: 0x04000D19 RID: 3353
		[SerializeField]
		[Tooltip("Generated mesh material. Note, UVs are not scaled, so only material with single-color texture will work fine")]
		private Material MeshMaterial;
	}
}
