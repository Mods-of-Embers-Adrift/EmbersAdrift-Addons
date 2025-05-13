using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000D0 RID: 208
	public class LightningMeshSurfaceScript : LightningBoltPrefabScriptBase
	{
		// Token: 0x0600077E RID: 1918 RVA: 0x000ADC54 File Offset: 0x000ABE54
		private void CheckMesh()
		{
			if (this.MeshFilter == null || this.MeshFilter.sharedMesh == null)
			{
				this.meshHelper = null;
				return;
			}
			if (this.MeshFilter.sharedMesh != this.previousMesh)
			{
				this.previousMesh = this.MeshFilter.sharedMesh;
				this.meshHelper = new MeshHelper(this.previousMesh);
			}
		}

		// Token: 0x0600077F RID: 1919 RVA: 0x00048192 File Offset: 0x00046392
		protected override LightningBoltParameters OnCreateParameters()
		{
			LightningBoltParameters lightningBoltParameters = base.OnCreateParameters();
			lightningBoltParameters.Generator = LightningGeneratorPath.PathGeneratorInstance;
			return lightningBoltParameters;
		}

		// Token: 0x06000780 RID: 1920 RVA: 0x000481A5 File Offset: 0x000463A5
		protected virtual void PopulateSourcePoints(List<Vector3> points)
		{
			if (this.meshHelper != null)
			{
				this.CreateRandomLightningPath(this.sourcePoints);
			}
		}

		// Token: 0x06000781 RID: 1921 RVA: 0x000ADCC4 File Offset: 0x000ABEC4
		public void CreateRandomLightningPath(List<Vector3> points)
		{
			if (this.meshHelper == null)
			{
				return;
			}
			RaycastHit raycastHit = default(RaycastHit);
			this.maximumPathDistanceSquared = this.MaximumPathDistance * this.MaximumPathDistance;
			int num;
			this.meshHelper.GenerateRandomPoint(ref raycastHit, out num);
			raycastHit.distance = UnityEngine.Random.Range(this.MeshOffsetRange.Minimum, this.MeshOffsetRange.Maximum);
			Vector3 vector = raycastHit.point + raycastHit.normal * raycastHit.distance;
			float num2 = UnityEngine.Random.Range(this.MinimumPathDistanceRange.Minimum, this.MinimumPathDistanceRange.Maximum);
			num2 *= num2;
			this.sourcePoints.Add(this.MeshFilter.transform.TransformPoint(vector));
			int num3 = (UnityEngine.Random.Range(0, 1) == 1) ? 3 : -3;
			int num4 = UnityEngine.Random.Range(this.PathLengthCount.Minimum, this.PathLengthCount.Maximum);
			while (num4 != 0)
			{
				num += num3;
				if (num >= 0 && num < this.meshHelper.Triangles.Length)
				{
					this.meshHelper.GetRaycastFromTriangleIndex(num, ref raycastHit);
					raycastHit.distance = UnityEngine.Random.Range(this.MeshOffsetRange.Minimum, this.MeshOffsetRange.Maximum);
					Vector3 vector2 = raycastHit.point + raycastHit.normal * raycastHit.distance;
					float sqrMagnitude = (vector2 - vector).sqrMagnitude;
					if (sqrMagnitude > this.maximumPathDistanceSquared)
					{
						break;
					}
					if (sqrMagnitude >= num2)
					{
						vector = vector2;
						this.sourcePoints.Add(this.MeshFilter.transform.TransformPoint(vector2));
						num4--;
						num2 = UnityEngine.Random.Range(this.MinimumPathDistanceRange.Minimum, this.MinimumPathDistanceRange.Maximum);
						num2 *= num2;
					}
				}
				else
				{
					num3 = -num3;
					num += num3;
					num4--;
				}
			}
		}

		// Token: 0x06000782 RID: 1922 RVA: 0x00048162 File Offset: 0x00046362
		protected override void Start()
		{
			base.Start();
		}

		// Token: 0x06000783 RID: 1923 RVA: 0x000481BB File Offset: 0x000463BB
		protected override void Update()
		{
			if (Time.timeScale > 0f)
			{
				this.CheckMesh();
			}
			base.Update();
		}

		// Token: 0x06000784 RID: 1924 RVA: 0x000ADEA8 File Offset: 0x000AC0A8
		public override void CreateLightningBolt(LightningBoltParameters parameters)
		{
			if (this.meshHelper == null)
			{
				return;
			}
			this.Generations = (parameters.Generations = Mathf.Clamp(this.Generations, 1, 5));
			this.sourcePoints.Clear();
			this.PopulateSourcePoints(this.sourcePoints);
			if (this.sourcePoints.Count > 1)
			{
				parameters.Points.Clear();
				if (this.Spline && this.sourcePoints.Count > 3)
				{
					LightningSplineScript.PopulateSpline(parameters.Points, this.sourcePoints, this.Generations, this.DistancePerSegmentHint, base.Camera);
					parameters.SmoothingFactor = (parameters.Points.Count - 1) / this.sourcePoints.Count;
				}
				else
				{
					parameters.Points.AddRange(this.sourcePoints);
					parameters.SmoothingFactor = 1;
				}
				base.CreateLightningBolt(parameters);
			}
		}

		// Token: 0x040008D2 RID: 2258
		[Header("Lightning Mesh Properties")]
		[Tooltip("The mesh filter. You must assign a mesh filter in order to create lightning on the mesh.")]
		public MeshFilter MeshFilter;

		// Token: 0x040008D3 RID: 2259
		[Tooltip("The mesh collider. This is used to get random points on the mesh.")]
		public Collider MeshCollider;

		// Token: 0x040008D4 RID: 2260
		[SingleLine("Random range that the point will offset from the mesh, using the normal of the chosen point to offset")]
		public RangeOfFloats MeshOffsetRange = new RangeOfFloats
		{
			Minimum = 0.5f,
			Maximum = 1f
		};

		// Token: 0x040008D5 RID: 2261
		[Header("Lightning Path Properties")]
		[SingleLine("Range for points in the lightning path")]
		public RangeOfIntegers PathLengthCount = new RangeOfIntegers
		{
			Minimum = 3,
			Maximum = 6
		};

		// Token: 0x040008D6 RID: 2262
		[SingleLine("Range for minimum distance between points in the lightning path")]
		public RangeOfFloats MinimumPathDistanceRange = new RangeOfFloats
		{
			Minimum = 0.5f,
			Maximum = 1f
		};

		// Token: 0x040008D7 RID: 2263
		[Tooltip("The maximum distance between mesh points. When walking the mesh, if a point is greater than this, the path direction is reversed. This tries to avoid paths crossing between mesh points that are not actually physically touching.")]
		public float MaximumPathDistance = 2f;

		// Token: 0x040008D8 RID: 2264
		private float maximumPathDistanceSquared;

		// Token: 0x040008D9 RID: 2265
		[Tooltip("Whether to use spline interpolation between the path points. Paths must be at least 4 points long to be splined.")]
		public bool Spline;

		// Token: 0x040008DA RID: 2266
		[Tooltip("For spline. the distance hint for each spline segment. Set to <= 0 to use the generations to determine how many spline segments to use. If > 0, it will be divided by Generations before being applied. This value is a guideline and is approximate, and not uniform on the spline.")]
		public float DistancePerSegmentHint;

		// Token: 0x040008DB RID: 2267
		private readonly List<Vector3> sourcePoints = new List<Vector3>();

		// Token: 0x040008DC RID: 2268
		private Mesh previousMesh;

		// Token: 0x040008DD RID: 2269
		private MeshHelper meshHelper;
	}
}
