using System;
using System.Collections.Generic;
using System.Diagnostics;
using DataStructures.ViliWonka.KDTree;
using UnityEngine;

namespace DataStructures.ViliWonka.Tests
{
	// Token: 0x02000152 RID: 338
	public class KDTreeBenchmark : MonoBehaviour
	{
		// Token: 0x06000B8A RID: 2954 RVA: 0x0004A726 File Offset: 0x00048926
		private void Awake()
		{
			this.points10k = new Vector3[10000];
			this.points100k = new Vector3[100000];
			this.points1m = new Vector3[1000000];
			this.stopwatch = new Stopwatch();
		}

		// Token: 0x06000B8B RID: 2955 RVA: 0x000CC620 File Offset: 0x000CA820
		private void Start()
		{
			this.testingArray = this.points10k;
			UnityEngine.Debug.Log(" -- 10K THOUSAND POINTS --");
			this.TestSet();
			this.testingArray = this.points100k;
			UnityEngine.Debug.Log(" -- 100K THOUSAND POINTS --");
			this.TestSet();
			this.testingArray = this.points1m;
			UnityEngine.Debug.Log(" -- 1 MILLION POINTS --");
			this.TestSet();
		}

		// Token: 0x06000B8C RID: 2956 RVA: 0x000CC684 File Offset: 0x000CA884
		private void TestSet()
		{
			this.TestConstruction(5, "Uniform", new Action(this.RandomizeUniform));
			this.TestConstruction(5, "Triangular", new Action(this.RandomizeUniform));
			this.TestConstruction(5, "2D planar", new Action(this.Randomize2DPlane));
			this.TestConstruction(5, "2D planar, sorted", new Action(this.SortedRandomize2DPlane));
			this.TestQuery(5, "Uniform", new Action(this.RandomizeUniform));
			this.TestQuery(5, "Triangular", new Action(this.RandomizeUniform));
			this.TestQuery(5, "2D planar", new Action(this.Randomize2DPlane));
			this.TestQuery(5, "2D planar, sorted", new Action(this.SortedRandomize2DPlane));
		}

		// Token: 0x06000B8D RID: 2957 RVA: 0x000CC754 File Offset: 0x000CA954
		private void TestConstruction(int tests, string distributionName, Action randomize)
		{
			long num = 0L;
			for (int i = 0; i < tests; i++)
			{
				randomize();
				long num2 = this.Construct();
				num += num2;
			}
			UnityEngine.Debug.Log(string.Concat(new string[]
			{
				"Average ",
				distributionName,
				" distribution construction time: ",
				((long)((float)num / (float)tests)).ToString(),
				" ms"
			}));
		}

		// Token: 0x06000B8E RID: 2958 RVA: 0x000CC7C0 File Offset: 0x000CA9C0
		private void TestQuery(int tests, string distributionName, Action randomize)
		{
			randomize();
			this.Construct();
			long num = 0L;
			for (int i = 0; i < tests; i++)
			{
				num += this.QueryRadius();
			}
			UnityEngine.Debug.Log(distributionName + " distribution average query time: " + ((long)((float)num / (float)tests)).ToString() + " ms");
		}

		// Token: 0x06000B8F RID: 2959 RVA: 0x000CC818 File Offset: 0x000CAA18
		private void RandomizeUniform()
		{
			for (int i = 0; i < this.testingArray.Length; i++)
			{
				this.testingArray[i] = new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
			}
		}

		// Token: 0x06000B90 RID: 2960 RVA: 0x000CC858 File Offset: 0x000CAA58
		private void RandomizeTriangle()
		{
			for (int i = 0; i < this.testingArray.Length; i++)
			{
				this.testingArray[i] = new Vector3((UnityEngine.Random.value + UnityEngine.Random.value) / 2f, (UnityEngine.Random.value + UnityEngine.Random.value) / 2f, (UnityEngine.Random.value + UnityEngine.Random.value) / 2f);
			}
		}

		// Token: 0x06000B91 RID: 2961 RVA: 0x000CC8BC File Offset: 0x000CAABC
		private void Randomize2DPlane()
		{
			Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
			Vector3 onUnitSphere2 = UnityEngine.Random.onUnitSphere;
			for (int i = 0; i < this.testingArray.Length; i++)
			{
				this.testingArray[i] = UnityEngine.Random.value * onUnitSphere + UnityEngine.Random.value * onUnitSphere2 + UnityEngine.Random.insideUnitSphere * 0.1f;
			}
		}

		// Token: 0x06000B92 RID: 2962 RVA: 0x000CC924 File Offset: 0x000CAB24
		private void SortedRandomize2DPlane()
		{
			this.Randomize2DPlane();
			Array.Sort<Vector3>(this.testingArray, (Vector3 v1, Vector3 v2) => v1.x.CompareTo(v2.x));
			Array.Sort<Vector3>(this.testingArray, (Vector3 v1, Vector3 v2) => v1.y.CompareTo(v2.y));
			Array.Sort<Vector3>(this.testingArray, (Vector3 v1, Vector3 v2) => v1.z.CompareTo(v2.z));
		}

		// Token: 0x06000B93 RID: 2963 RVA: 0x000CC9B8 File Offset: 0x000CABB8
		private long Construct()
		{
			this.stopwatch.Reset();
			this.stopwatch.Start();
			this.tree = new KDTree(16);
			this.tree.Build(this.testingArray, -1);
			this.stopwatch.Stop();
			return this.stopwatch.ElapsedMilliseconds;
		}

		// Token: 0x06000B94 RID: 2964 RVA: 0x000CCA10 File Offset: 0x000CAC10
		private long QueryRadius()
		{
			this.stopwatch.Reset();
			this.stopwatch.Start();
			Vector3 queryPosition = Vector3.one * 0.5f + UnityEngine.Random.insideUnitSphere;
			float queryRadius = 0.25f;
			this.results.Clear();
			this.query.Radius(this.tree, queryPosition, queryRadius, this.results);
			this.stopwatch.Stop();
			return this.stopwatch.ElapsedMilliseconds;
		}

		// Token: 0x06000B95 RID: 2965 RVA: 0x000CCA90 File Offset: 0x000CAC90
		private long QueryClosest()
		{
			this.stopwatch.Reset();
			this.stopwatch.Start();
			Vector3 queryPosition = Vector3.one * 0.5f + UnityEngine.Random.insideUnitSphere;
			this.results.Clear();
			this.query.ClosestPoint(this.tree, queryPosition, this.results, null);
			this.stopwatch.Stop();
			return this.stopwatch.ElapsedMilliseconds;
		}

		// Token: 0x06000B96 RID: 2966 RVA: 0x000CCB08 File Offset: 0x000CAD08
		private long QueryKNearest()
		{
			this.stopwatch.Reset();
			this.stopwatch.Start();
			Vector3 queryPosition = Vector3.one * 0.5f + UnityEngine.Random.insideUnitSphere;
			int k = 13;
			this.results.Clear();
			this.query.KNearest(this.tree, queryPosition, k, this.results, null);
			this.stopwatch.Stop();
			return this.stopwatch.ElapsedMilliseconds;
		}

		// Token: 0x06000B97 RID: 2967 RVA: 0x000CCB84 File Offset: 0x000CAD84
		private long QueryInterval()
		{
			this.stopwatch.Reset();
			this.stopwatch.Start();
			Vector3 b = UnityEngine.Random.insideUnitSphere * 0.25f;
			Vector3 min = Vector3.one * 0.25f + UnityEngine.Random.insideUnitSphere * 0.25f + b;
			Vector3 max = Vector3.one * 0.75f + UnityEngine.Random.insideUnitSphere * 0.25f + b;
			this.results.Clear();
			this.query.Interval(this.tree, min, max, this.results);
			this.stopwatch.Stop();
			return this.stopwatch.ElapsedMilliseconds;
		}

		// Token: 0x04000B27 RID: 2855
		private Vector3[] points10k;

		// Token: 0x04000B28 RID: 2856
		private Vector3[] points100k;

		// Token: 0x04000B29 RID: 2857
		private Vector3[] points1m;

		// Token: 0x04000B2A RID: 2858
		private Vector3[] testingArray;

		// Token: 0x04000B2B RID: 2859
		private Stopwatch stopwatch;

		// Token: 0x04000B2C RID: 2860
		private KDTree tree;

		// Token: 0x04000B2D RID: 2861
		private KDQuery query = new KDQuery(2048);

		// Token: 0x04000B2E RID: 2862
		private List<int> results = new List<int>();
	}
}
