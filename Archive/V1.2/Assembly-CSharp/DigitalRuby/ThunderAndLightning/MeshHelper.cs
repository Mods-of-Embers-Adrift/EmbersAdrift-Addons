using System;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000D4 RID: 212
	public class MeshHelper
	{
		// Token: 0x06000798 RID: 1944 RVA: 0x00048224 File Offset: 0x00046424
		public MeshHelper(Mesh mesh)
		{
			this.mesh = mesh;
			this.triangles = mesh.triangles;
			this.vertices = mesh.vertices;
			this.normals = mesh.normals;
			this.CalculateNormalizedAreaWeights();
		}

		// Token: 0x06000799 RID: 1945 RVA: 0x0004825D File Offset: 0x0004645D
		public void GenerateRandomPoint(ref RaycastHit hit, out int triangleIndex)
		{
			triangleIndex = this.SelectRandomTriangle();
			this.GetRaycastFromTriangleIndex(triangleIndex, ref hit);
		}

		// Token: 0x0600079A RID: 1946 RVA: 0x000AE728 File Offset: 0x000AC928
		public void GetRaycastFromTriangleIndex(int triangleIndex, ref RaycastHit hit)
		{
			Vector3 vector = this.GenerateRandomBarycentricCoordinates();
			Vector3 a = this.vertices[this.triangles[triangleIndex]];
			Vector3 vector2 = this.vertices[this.triangles[triangleIndex + 1]];
			Vector3 a2 = this.vertices[this.triangles[triangleIndex + 2]];
			hit.barycentricCoordinate = vector;
			hit.point = a * vector.x + vector2 * vector.y + a2 * vector.z;
			if (this.normals == null)
			{
				hit.normal = Vector3.Cross(a2 - vector2, a - vector2).normalized;
				return;
			}
			a = this.normals[this.triangles[triangleIndex]];
			vector2 = this.normals[this.triangles[triangleIndex + 1]];
			a2 = this.normals[this.triangles[triangleIndex + 2]];
			hit.normal = a * vector.x + vector2 * vector.y + a2 * vector.z;
		}

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x0600079B RID: 1947 RVA: 0x00048270 File Offset: 0x00046470
		public Mesh Mesh
		{
			get
			{
				return this.mesh;
			}
		}

		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x0600079C RID: 1948 RVA: 0x00048278 File Offset: 0x00046478
		public int[] Triangles
		{
			get
			{
				return this.triangles;
			}
		}

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x0600079D RID: 1949 RVA: 0x00048280 File Offset: 0x00046480
		public Vector3[] Vertices
		{
			get
			{
				return this.vertices;
			}
		}

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x0600079E RID: 1950 RVA: 0x00048288 File Offset: 0x00046488
		public Vector3[] Normals
		{
			get
			{
				return this.normals;
			}
		}

		// Token: 0x0600079F RID: 1951 RVA: 0x000AE858 File Offset: 0x000ACA58
		private float[] CalculateSurfaceAreas(out float totalSurfaceArea)
		{
			int num = 0;
			totalSurfaceArea = 0f;
			float[] array = new float[this.triangles.Length / 3];
			for (int i = 0; i < this.triangles.Length; i += 3)
			{
				Vector3 a = this.vertices[this.triangles[i]];
				Vector3 vector = this.vertices[this.triangles[i + 1]];
				Vector3 b = this.vertices[this.triangles[i + 2]];
				float sqrMagnitude = (a - vector).sqrMagnitude;
				float sqrMagnitude2 = (a - b).sqrMagnitude;
				float sqrMagnitude3 = (vector - b).sqrMagnitude;
				float num2 = PathGenerator.SquareRoot((2f * sqrMagnitude * sqrMagnitude2 + 2f * sqrMagnitude2 * sqrMagnitude3 + 2f * sqrMagnitude3 * sqrMagnitude - sqrMagnitude * sqrMagnitude - sqrMagnitude2 * sqrMagnitude2 - sqrMagnitude3 * sqrMagnitude3) / 16f);
				array[num++] = num2;
				totalSurfaceArea += num2;
			}
			return array;
		}

		// Token: 0x060007A0 RID: 1952 RVA: 0x000AE964 File Offset: 0x000ACB64
		private void CalculateNormalizedAreaWeights()
		{
			float num;
			this.normalizedAreaWeights = this.CalculateSurfaceAreas(out num);
			if (this.normalizedAreaWeights.Length == 0)
			{
				return;
			}
			float num2 = 0f;
			for (int i = 0; i < this.normalizedAreaWeights.Length; i++)
			{
				float num3 = this.normalizedAreaWeights[i] / num;
				this.normalizedAreaWeights[i] = num2;
				num2 += num3;
			}
		}

		// Token: 0x060007A1 RID: 1953 RVA: 0x000AE9BC File Offset: 0x000ACBBC
		private int SelectRandomTriangle()
		{
			float value = UnityEngine.Random.value;
			int i = 0;
			int num = this.normalizedAreaWeights.Length - 1;
			while (i < num)
			{
				int num2 = (i + num) / 2;
				if (this.normalizedAreaWeights[num2] < value)
				{
					i = num2 + 1;
				}
				else
				{
					num = num2;
				}
			}
			return i * 3;
		}

		// Token: 0x060007A2 RID: 1954 RVA: 0x000AEA00 File Offset: 0x000ACC00
		private Vector3 GenerateRandomBarycentricCoordinates()
		{
			Vector3 vector = new Vector3(UnityEngine.Random.Range(Mathf.Epsilon, 1f), UnityEngine.Random.Range(Mathf.Epsilon, 1f), UnityEngine.Random.Range(Mathf.Epsilon, 1f));
			return vector / (vector.x + vector.y + vector.z);
		}

		// Token: 0x040008F2 RID: 2290
		private Mesh mesh;

		// Token: 0x040008F3 RID: 2291
		private int[] triangles;

		// Token: 0x040008F4 RID: 2292
		private Vector3[] vertices;

		// Token: 0x040008F5 RID: 2293
		private Vector3[] normals;

		// Token: 0x040008F6 RID: 2294
		private float[] normalizedAreaWeights;
	}
}
