using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x020000A5 RID: 165
	[Serializable]
	public class RTMeshData
	{
		// Token: 0x0600066D RID: 1645 RVA: 0x000A8604 File Offset: 0x000A6804
		public RTMeshData(int numVertices, int numSubmeshes, List<int> numTrianglesPerSubmesh)
		{
			Vector3[] array = new Vector3[numVertices];
			Vector3[] array2 = new Vector3[numVertices];
			Vector2[] array3 = new Vector2[numVertices];
			Color[] array4 = new Color[numVertices];
			int[][] array5 = new int[numSubmeshes][];
			for (int i = 0; i < array5.Length; i++)
			{
				array5[i] = new int[numTrianglesPerSubmesh[i]];
			}
			this.SetValues(array, array2, array3, array4, array5);
		}

		// Token: 0x0600066E RID: 1646 RVA: 0x000A866C File Offset: 0x000A686C
		public RTMeshData(Mesh mesh)
		{
			int subMeshCount = mesh.subMeshCount;
			Vector3[] array = mesh.vertices;
			Vector3[] array2 = mesh.normals;
			Vector2[] array3 = mesh.uv;
			Vector2[] array4 = mesh.uv2;
			Color[] array5 = mesh.colors;
			int[][] array6 = new int[subMeshCount][];
			for (int i = 0; i < array6.Length; i++)
			{
				array6[i] = mesh.GetTriangles(i);
			}
			this.SetValues(array, array2, array3, array5, array6);
		}

		// Token: 0x0600066F RID: 1647 RVA: 0x0004769D File Offset: 0x0004589D
		private void SetValues(Vector3[] vertices, Vector3[] normals, Vector2[] uv, Color[] colors, int[][] triangles)
		{
			this.vertices = vertices;
			this.normals = normals;
			this.uv = uv;
			this.colors = colors;
			this.triangles = triangles;
			this.triangleIndices = new int[triangles.Length];
			this.vertexIndex = 0;
		}

		// Token: 0x06000670 RID: 1648 RVA: 0x000A86DC File Offset: 0x000A68DC
		public void CopyDataFromIndex(int index, int lastTriCount, int numTris, RTMeshData copyFrom)
		{
			this.vertices[index] = copyFrom.vertices[index];
			this.normals[index] = copyFrom.normals[index];
			this.uv[index] = copyFrom.uv[index];
		}

		// Token: 0x06000671 RID: 1649 RVA: 0x000A8734 File Offset: 0x000A6934
		public void AddTriangle(int sumbesh, int value)
		{
			if (this.triangleIndices[sumbesh] >= this.triangles[sumbesh].Length)
			{
				int newSize = this.triangles[sumbesh].Length * 2;
				Array.Resize<int>(ref this.triangles[sumbesh], newSize);
			}
			this.triangles[sumbesh][this.triangleIndices[sumbesh]] = value;
			this.triangleIndices[sumbesh]++;
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x000A8798 File Offset: 0x000A6998
		public void AddVertex(Vector3 vertexValue, Vector3 normalValue, Vector2 uvValue, Color color)
		{
			if (this.vertCount >= this.vertices.Length)
			{
				this.Resize();
			}
			this.vertices[this.vertexIndex] = vertexValue;
			this.normals[this.vertexIndex] = normalValue;
			this.uv[this.vertexIndex] = uvValue;
			this.colors[this.vertexIndex] = color;
			this.vertexIndex++;
			this.vertCount++;
		}

		// Token: 0x06000673 RID: 1651 RVA: 0x000A8820 File Offset: 0x000A6A20
		private void Resize()
		{
			int newSize = this.vertices.Length * 2;
			Array.Resize<Vector3>(ref this.vertices, newSize);
			Array.Resize<Vector3>(ref this.normals, newSize);
			Array.Resize<Vector2>(ref this.uv, newSize);
			Array.Resize<Color>(ref this.colors, newSize);
		}

		// Token: 0x06000674 RID: 1652 RVA: 0x000476DA File Offset: 0x000458DA
		public int VertexCount()
		{
			return this.vertCount;
		}

		// Token: 0x06000675 RID: 1653 RVA: 0x000A8868 File Offset: 0x000A6A68
		public void Clear()
		{
			this.vertCount = 0;
			this.vertexIndex = 0;
			for (int i = 0; i < this.triangleIndices.Length; i++)
			{
				this.triangleIndices[i] = 0;
			}
		}

		// Token: 0x04000763 RID: 1891
		private int vertCount;

		// Token: 0x04000764 RID: 1892
		private int vertexIndex;

		// Token: 0x04000765 RID: 1893
		public Vector3[] vertices;

		// Token: 0x04000766 RID: 1894
		public Vector3[] normals;

		// Token: 0x04000767 RID: 1895
		public Vector2[] uv;

		// Token: 0x04000768 RID: 1896
		public Vector2[] uv2;

		// Token: 0x04000769 RID: 1897
		public Color[] colors;

		// Token: 0x0400076A RID: 1898
		public int[] triangleIndices;

		// Token: 0x0400076B RID: 1899
		public int[][] triangles;
	}
}
