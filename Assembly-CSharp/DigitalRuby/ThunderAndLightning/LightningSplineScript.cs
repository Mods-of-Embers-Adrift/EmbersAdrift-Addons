using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000D1 RID: 209
	public class LightningSplineScript : LightningBoltPathScriptBase
	{
		// Token: 0x06000786 RID: 1926 RVA: 0x000AE020 File Offset: 0x000AC220
		private bool SourceChanged()
		{
			if (this.sourcePoints.Count != this.prevSourcePoints.Count)
			{
				return true;
			}
			for (int i = 0; i < this.sourcePoints.Count; i++)
			{
				if (this.sourcePoints[i] != this.prevSourcePoints[i])
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000787 RID: 1927 RVA: 0x00048162 File Offset: 0x00046362
		protected override void Start()
		{
			base.Start();
		}

		// Token: 0x06000788 RID: 1928 RVA: 0x000481D5 File Offset: 0x000463D5
		protected override void Update()
		{
			base.Update();
		}

		// Token: 0x06000789 RID: 1929 RVA: 0x000AE080 File Offset: 0x000AC280
		public override void CreateLightningBolt(LightningBoltParameters parameters)
		{
			if (this.LightningPath == null)
			{
				return;
			}
			this.sourcePoints.Clear();
			try
			{
				foreach (GameObject gameObject in this.LightningPath)
				{
					if (gameObject != null)
					{
						this.sourcePoints.Add(gameObject.transform.position);
					}
				}
			}
			catch (NullReferenceException)
			{
				return;
			}
			if (this.sourcePoints.Count < 4)
			{
				Debug.LogError("To create spline lightning, you need a lightning path with at least " + 4.ToString() + " points.");
				return;
			}
			this.Generations = (parameters.Generations = Mathf.Clamp(this.Generations, 1, 5));
			parameters.Points.Clear();
			if (this.previousGenerations != this.Generations || this.previousDistancePerSegment != this.DistancePerSegmentHint || this.SourceChanged())
			{
				this.previousGenerations = this.Generations;
				this.previousDistancePerSegment = this.DistancePerSegmentHint;
				LightningSplineScript.PopulateSpline(parameters.Points, this.sourcePoints, this.Generations, this.DistancePerSegmentHint, base.Camera);
				this.prevSourcePoints.Clear();
				this.prevSourcePoints.AddRange(this.sourcePoints);
				this.savedSplinePoints.Clear();
				this.savedSplinePoints.AddRange(parameters.Points);
			}
			else
			{
				parameters.Points.AddRange(this.savedSplinePoints);
			}
			parameters.SmoothingFactor = (parameters.Points.Count - 1) / this.sourcePoints.Count;
			base.CreateLightningBolt(parameters);
		}

		// Token: 0x0600078A RID: 1930 RVA: 0x000481DD File Offset: 0x000463DD
		protected override LightningBoltParameters OnCreateParameters()
		{
			LightningBoltParameters orCreateParameters = LightningBoltParameters.GetOrCreateParameters();
			orCreateParameters.Generator = LightningGeneratorPath.PathGeneratorInstance;
			return orCreateParameters;
		}

		// Token: 0x0600078B RID: 1931 RVA: 0x000AE238 File Offset: 0x000AC438
		public void Trigger(List<Vector3> points, bool spline)
		{
			if (points.Count < 2)
			{
				return;
			}
			this.Generations = Mathf.Clamp(this.Generations, 1, 5);
			LightningBoltParameters lightningBoltParameters = base.CreateParameters();
			lightningBoltParameters.Points.Clear();
			if (spline && points.Count > 3)
			{
				LightningSplineScript.PopulateSpline(lightningBoltParameters.Points, points, this.Generations, this.DistancePerSegmentHint, base.Camera);
				lightningBoltParameters.SmoothingFactor = (lightningBoltParameters.Points.Count - 1) / points.Count;
			}
			else
			{
				lightningBoltParameters.Points.AddRange(points);
				lightningBoltParameters.SmoothingFactor = 1;
			}
			base.CreateLightningBolt(lightningBoltParameters);
			base.CreateLightningBoltsNow();
		}

		// Token: 0x0600078C RID: 1932 RVA: 0x000AE2DC File Offset: 0x000AC4DC
		public static void PopulateSpline(List<Vector3> splinePoints, List<Vector3> sourcePoints, int generations, float distancePerSegmentHit, Camera camera)
		{
			splinePoints.Clear();
			PathGenerator.Is2D = (camera != null && camera.orthographic);
			if (distancePerSegmentHit > 0f)
			{
				PathGenerator.CreateSplineWithSegmentDistance(splinePoints, sourcePoints, distancePerSegmentHit / (float)generations, false);
				return;
			}
			PathGenerator.CreateSpline(splinePoints, sourcePoints, sourcePoints.Count * generations * generations, false);
		}

		// Token: 0x040008DE RID: 2270
		public const int MaxSplineGenerations = 5;

		// Token: 0x040008DF RID: 2271
		[Header("Lightning Spline Properties")]
		[Tooltip("The distance hint for each spline segment. Set to <= 0 to use the generations to determine how many spline segments to use. If > 0, it will be divided by Generations before being applied. This value is a guideline and is approximate, and not uniform on the spline.")]
		public float DistancePerSegmentHint;

		// Token: 0x040008E0 RID: 2272
		private readonly List<Vector3> prevSourcePoints = new List<Vector3>(new Vector3[]
		{
			Vector3.zero
		});

		// Token: 0x040008E1 RID: 2273
		private readonly List<Vector3> sourcePoints = new List<Vector3>();

		// Token: 0x040008E2 RID: 2274
		private List<Vector3> savedSplinePoints = new List<Vector3>();

		// Token: 0x040008E3 RID: 2275
		private int previousGenerations = -1;

		// Token: 0x040008E4 RID: 2276
		private float previousDistancePerSegment = -1f;
	}
}
