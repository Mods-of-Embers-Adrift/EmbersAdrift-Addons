using System;
using System.Collections.Generic;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001C0 RID: 448
	[HelpURL("http://www.bansheegz.com/BGCurve/Cc/BGCcSweep2D")]
	[DisallowMultipleComponent]
	[BGCc.CcDescriptor(Description = "Sweep a line or 2d spline along another 2d spline.", Name = "Sweep 2D", Icon = "BGCcSweep2d123")]
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	[AddComponentMenu("BansheeGz/BGCurve/Components/BGCcSweep2D")]
	[ExecuteInEditMode]
	public class BGCcSweep2D : BGCcSplitterPolyline
	{
		// Token: 0x1700045D RID: 1117
		// (get) Token: 0x0600100D RID: 4109 RVA: 0x0004D751 File Offset: 0x0004B951
		// (set) Token: 0x0600100E RID: 4110 RVA: 0x0004D759 File Offset: 0x0004B959
		public BGCcSweep2D.ProfileModeEnum ProfileMode
		{
			get
			{
				return this.profileMode;
			}
			set
			{
				base.ParamChanged<BGCcSweep2D.ProfileModeEnum>(ref this.profileMode, value);
			}
		}

		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x0600100F RID: 4111 RVA: 0x0004D769 File Offset: 0x0004B969
		// (set) Token: 0x06001010 RID: 4112 RVA: 0x0004D771 File Offset: 0x0004B971
		public float LineWidth
		{
			get
			{
				return this.lineWidth;
			}
			set
			{
				base.ParamChanged<float>(ref this.lineWidth, value);
			}
		}

		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x06001011 RID: 4113 RVA: 0x0004D781 File Offset: 0x0004B981
		// (set) Token: 0x06001012 RID: 4114 RVA: 0x0004D789 File Offset: 0x0004B989
		public float UCoordinateStart
		{
			get
			{
				return this.uCoordinateStart;
			}
			set
			{
				base.ParamChanged<float>(ref this.uCoordinateStart, value);
			}
		}

		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x06001013 RID: 4115 RVA: 0x0004D799 File Offset: 0x0004B999
		// (set) Token: 0x06001014 RID: 4116 RVA: 0x0004D7A1 File Offset: 0x0004B9A1
		public float UCoordinateEnd
		{
			get
			{
				return this.uCoordinateEnd;
			}
			set
			{
				base.ParamChanged<float>(ref this.uCoordinateEnd, value);
			}
		}

		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x06001015 RID: 4117 RVA: 0x0004D7B1 File Offset: 0x0004B9B1
		// (set) Token: 0x06001016 RID: 4118 RVA: 0x0004D7B9 File Offset: 0x0004B9B9
		public BGCcSplitterPolyline ProfileSpline
		{
			get
			{
				return this.profileSpline;
			}
			set
			{
				base.ParamChanged<BGCcSplitterPolyline>(ref this.profileSpline, value);
			}
		}

		// Token: 0x17000462 RID: 1122
		// (get) Token: 0x06001017 RID: 4119 RVA: 0x0004D7C9 File Offset: 0x0004B9C9
		// (set) Token: 0x06001018 RID: 4120 RVA: 0x0004D7D1 File Offset: 0x0004B9D1
		public bool SwapUv
		{
			get
			{
				return this.swapUV;
			}
			set
			{
				base.ParamChanged<bool>(ref this.swapUV, value);
			}
		}

		// Token: 0x17000463 RID: 1123
		// (get) Token: 0x06001019 RID: 4121 RVA: 0x0004D7E1 File Offset: 0x0004B9E1
		// (set) Token: 0x0600101A RID: 4122 RVA: 0x0004D7E9 File Offset: 0x0004B9E9
		public bool SwapNormals
		{
			get
			{
				return this.swapNormals;
			}
			set
			{
				base.ParamChanged<bool>(ref this.swapNormals, value);
			}
		}

		// Token: 0x17000464 RID: 1124
		// (get) Token: 0x0600101B RID: 4123 RVA: 0x0004D7F9 File Offset: 0x0004B9F9
		// (set) Token: 0x0600101C RID: 4124 RVA: 0x0004D801 File Offset: 0x0004BA01
		public float VCoordinateScale
		{
			get
			{
				return this.vCoordinateScale;
			}
			set
			{
				base.ParamChanged<float>(ref this.vCoordinateScale, value);
			}
		}

		// Token: 0x17000465 RID: 1125
		// (get) Token: 0x0600101D RID: 4125 RVA: 0x0004D811 File Offset: 0x0004BA11
		public override string Error
		{
			get
			{
				return base.ChoseMessage(base.Error, delegate
				{
					if (!base.Curve.Mode2DOn)
					{
						return "Curve should be in 2D mode";
					}
					if (this.profileMode == BGCcSweep2D.ProfileModeEnum.Spline)
					{
						if (this.profileSpline == null)
						{
							return "Profile spline is not set.";
						}
						if (this.profileSpline.Curve.Mode2D != BGCurve.Mode2DEnum.XY)
						{
							return "Profile spline should be in XY 2D mode.";
						}
						this.profileSpline.InvalidateData();
						if (this.profileSpline.PointsCount < 2)
						{
							return "Profile spline should have at least 2 points.";
						}
					}
					int num = (this.profileMode == BGCcSweep2D.ProfileModeEnum.Line) ? 2 : this.profileSpline.PointsCount;
					if (base.PointsCount * num > 65534)
					{
						return "Vertex count per mesh limit is exceeded ( > 65534)";
					}
					return null;
				});
			}
		}

		// Token: 0x17000466 RID: 1126
		// (get) Token: 0x0600101E RID: 4126 RVA: 0x0004D82B File Offset: 0x0004BA2B
		public MeshFilter MeshFilter
		{
			get
			{
				if (this.meshFilter == null)
				{
					this.meshFilter = base.GetComponent<MeshFilter>();
				}
				return this.meshFilter;
			}
		}

		// Token: 0x0600101F RID: 4127 RVA: 0x0004D84D File Offset: 0x0004BA4D
		public override void Start()
		{
			this.useLocal = true;
			base.Start();
			if (this.MeshFilter.sharedMesh == null)
			{
				this.UpdateUI();
			}
		}

		// Token: 0x06001020 RID: 4128 RVA: 0x000DE400 File Offset: 0x000DC600
		public void UpdateUI()
		{
			if (this.Error != null)
			{
				return;
			}
			if (!this.UseLocal)
			{
				this.useLocal = true;
				this.dataValid = false;
			}
			List<Vector3> positions = base.Positions;
			if (positions.Count < 2)
			{
				return;
			}
			MeshFilter meshFilter;
			try
			{
				meshFilter = this.MeshFilter;
			}
			catch (MissingReferenceException)
			{
				base.RemoveListeners();
				return;
			}
			Mesh mesh = meshFilter.sharedMesh;
			if (mesh == null)
			{
				mesh = new Mesh();
				meshFilter.mesh = mesh;
			}
			BGCcSweep2D.crossSectionList.Clear();
			this.triangles.Clear();
			this.uvs.Clear();
			this.vertices.Clear();
			if (this.profileMode == BGCcSweep2D.ProfileModeEnum.Line)
			{
				BGCcSweep2D.crossSectionList.Add(new BGCcSweep2D.PositionWithU
				{
					Position = Vector3.left * this.lineWidth * 0.5f,
					U = this.uCoordinateStart
				});
				BGCcSweep2D.crossSectionList.Add(new BGCcSweep2D.PositionWithU
				{
					Position = Vector3.right * this.lineWidth * 0.5f,
					U = this.uCoordinateEnd
				});
			}
			else
			{
				List<Vector3> positions2 = this.profileSpline.Positions;
				for (int i = 0; i < positions2.Count; i++)
				{
					BGCcSweep2D.crossSectionList.Add(new BGCcSweep2D.PositionWithU
					{
						Position = positions2[i]
					});
				}
			}
			int count = BGCcSweep2D.crossSectionList.Count;
			float num = 0f;
			for (int j = 0; j < count - 1; j++)
			{
				num += Vector3.Distance(BGCcSweep2D.crossSectionList[j].Position, BGCcSweep2D.crossSectionList[j + 1].Position);
			}
			if (this.profileMode == BGCcSweep2D.ProfileModeEnum.Spline)
			{
				float num2 = 0f;
				for (int k = 0; k < count - 1; k++)
				{
					BGCcSweep2D.crossSectionList[k] = new BGCcSweep2D.PositionWithU
					{
						Position = BGCcSweep2D.crossSectionList[k].Position,
						U = this.uCoordinateStart + num2 / num * (this.uCoordinateEnd - this.uCoordinateStart)
					};
					num2 += Vector3.Distance(BGCcSweep2D.crossSectionList[k].Position, BGCcSweep2D.crossSectionList[k + 1].Position);
				}
				BGCcSweep2D.crossSectionList[BGCcSweep2D.crossSectionList.Count - 1] = new BGCcSweep2D.PositionWithU
				{
					Position = BGCcSweep2D.crossSectionList[BGCcSweep2D.crossSectionList.Count - 1].Position,
					U = this.uCoordinateEnd
				};
			}
			Vector3 upwards;
			switch (base.Curve.Mode2D)
			{
			case BGCurve.Mode2DEnum.XY:
				upwards = (this.swapNormals ? Vector3.back : Vector3.forward);
				break;
			case BGCurve.Mode2DEnum.XZ:
				upwards = (this.swapNormals ? Vector3.down : Vector3.up);
				break;
			case BGCurve.Mode2DEnum.YZ:
				upwards = (this.swapNormals ? Vector3.left : Vector3.right);
				break;
			default:
				throw new ArgumentOutOfRangeException("Curve.Mode2D");
			}
			bool closed = base.Curve.Closed;
			Vector3 vector3;
			if (closed)
			{
				Vector3 vector = positions[1] - positions[0];
				float magnitude = vector.magnitude;
				Vector3 vector2 = positions[positions.Count - 1] - positions[positions.Count - 2];
				float magnitude2 = vector2.magnitude;
				float d = magnitude / magnitude2;
				vector3 = vector.normalized + vector2.normalized * d;
			}
			else
			{
				vector3 = positions[1] - positions[0];
			}
			Vector3 vector4 = vector3;
			Vector3 a = vector4.normalized;
			float num3 = (positions[1] - positions[0]).magnitude;
			Matrix4x4 matrix4x = Matrix4x4.TRS(positions[0], Quaternion.LookRotation(vector4, upwards), Vector3.one);
			for (int l = 0; l < count; l++)
			{
				BGCcSweep2D.PositionWithU positionWithU = BGCcSweep2D.crossSectionList[l];
				this.vertices.Add(matrix4x.MultiplyPoint(positionWithU.Position));
				this.uvs.Add(this.swapUV ? new Vector2(0f, positionWithU.U) : new Vector2(positionWithU.U, 0f));
			}
			float num4 = num3;
			int count2 = positions.Count;
			for (int m = 1; m < count2; m++)
			{
				Vector3 vector5 = positions[m];
				bool flag = m == count2 - 1;
				Vector3 vector6 = flag ? vector4 : (positions[m + 1] - vector5);
				Vector3 normalized = vector6.normalized;
				float magnitude3 = vector6.magnitude;
				float d2 = magnitude3 / num3;
				Vector3 forward = normalized + a * d2;
				if (flag && closed)
				{
					forward = vector3;
				}
				matrix4x = Matrix4x4.TRS(vector5, Quaternion.LookRotation(forward, upwards), Vector3.one);
				float num5 = num4 / num * this.vCoordinateScale;
				for (int n = 0; n < count; n++)
				{
					BGCcSweep2D.PositionWithU positionWithU2 = BGCcSweep2D.crossSectionList[n];
					this.vertices.Add(matrix4x.MultiplyPoint(positionWithU2.Position));
					this.uvs.Add(this.swapUV ? new Vector2(num5, positionWithU2.U) : new Vector2(positionWithU2.U, num5));
				}
				int num6 = this.vertices.Count - count * 2;
				int num7 = this.vertices.Count - count;
				for (int num8 = 0; num8 < count - 1; num8++)
				{
					this.triangles.Add(num6 + num8);
					this.triangles.Add(num7 + num8);
					this.triangles.Add(num6 + num8 + 1);
					this.triangles.Add(num6 + num8 + 1);
					this.triangles.Add(num7 + num8);
					this.triangles.Add(num7 + num8 + 1);
				}
				num4 += magnitude3;
				vector4 = vector6;
				a = normalized;
				num3 = magnitude3;
			}
			mesh.Clear();
			mesh.SetVertices(this.vertices);
			mesh.SetUVs(0, this.uvs);
			mesh.SetTriangles(this.triangles, 0);
			mesh.RecalculateNormals();
		}

		// Token: 0x06001021 RID: 4129 RVA: 0x0004D875 File Offset: 0x0004BA75
		protected override void UpdateRequested(object sender, EventArgs e)
		{
			base.UpdateRequested(sender, e);
			this.UpdateUI();
		}

		// Token: 0x04000D8B RID: 3467
		[SerializeField]
		[Tooltip("Profile mode.\r\n StraightLine -use straight line as cross section;\r\n Spline - use 2d spline as cross section;")]
		private BGCcSweep2D.ProfileModeEnum profileMode;

		// Token: 0x04000D8C RID: 3468
		[SerializeField]
		[Tooltip("Line width for StraightLine profile mode")]
		private float lineWidth = 1f;

		// Token: 0x04000D8D RID: 3469
		[SerializeField]
		[Tooltip("U coordinate for line start")]
		private float uCoordinateStart;

		// Token: 0x04000D8E RID: 3470
		[SerializeField]
		[Tooltip("U coordinate for line end")]
		private float uCoordinateEnd = 1f;

		// Token: 0x04000D8F RID: 3471
		[SerializeField]
		[Tooltip("Profile spline for Spline profile mode")]
		private BGCcSplitterPolyline profileSpline;

		// Token: 0x04000D90 RID: 3472
		[SerializeField]
		[Tooltip("V coordinate multiplier")]
		private float vCoordinateScale = 1f;

		// Token: 0x04000D91 RID: 3473
		[SerializeField]
		[Tooltip("Swap U with V coordinate")]
		private bool swapUV;

		// Token: 0x04000D92 RID: 3474
		[SerializeField]
		[Tooltip("Swap mesh normals direction")]
		private bool swapNormals;

		// Token: 0x04000D93 RID: 3475
		private static readonly List<BGCcSweep2D.PositionWithU> crossSectionList = new List<BGCcSweep2D.PositionWithU>();

		// Token: 0x04000D94 RID: 3476
		[NonSerialized]
		private MeshFilter meshFilter;

		// Token: 0x04000D95 RID: 3477
		private readonly List<Vector3> vertices = new List<Vector3>();

		// Token: 0x04000D96 RID: 3478
		private readonly List<Vector2> uvs = new List<Vector2>();

		// Token: 0x04000D97 RID: 3479
		private readonly List<int> triangles = new List<int>();

		// Token: 0x020001C1 RID: 449
		public enum ProfileModeEnum
		{
			// Token: 0x04000D99 RID: 3481
			Line,
			// Token: 0x04000D9A RID: 3482
			Spline
		}

		// Token: 0x020001C2 RID: 450
		private struct PositionWithU
		{
			// Token: 0x04000D9B RID: 3483
			public Vector3 Position;

			// Token: 0x04000D9C RID: 3484
			public float U;
		}
	}
}
