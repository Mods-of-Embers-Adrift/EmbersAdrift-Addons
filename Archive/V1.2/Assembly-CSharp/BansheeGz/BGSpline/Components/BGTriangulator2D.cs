using System;
using System.Collections.Generic;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001D4 RID: 468
	public class BGTriangulator2D
	{
		// Token: 0x060010AB RID: 4267 RVA: 0x000E01D0 File Offset: 0x000DE3D0
		public void Bind(Mesh mesh, List<Vector3> positions, BGTriangulator2D.Config config)
		{
			int num = positions.Count;
			if (config.Closed)
			{
				num--;
			}
			this.Clear();
			if (num > 2)
			{
				Vector4 vector = new Vector4(float.MaxValue, float.MaxValue, float.MinValue, float.MinValue);
				for (int i = 0; i < num; i++)
				{
					Vector3 vector2 = positions[i];
					BGCurve.Mode2DEnum mode2D = config.Mode2D;
					Vector3 item;
					Vector2 vector3;
					if (mode2D != BGCurve.Mode2DEnum.XY)
					{
						if (mode2D != BGCurve.Mode2DEnum.XZ)
						{
							item = new Vector3(0f, vector2.y, vector2.z);
							vector3 = new Vector2(vector2.y, vector2.z);
						}
						else
						{
							item = new Vector3(vector2.x, 0f, vector2.z);
							vector3 = new Vector2(vector2.x, vector2.z);
						}
					}
					else
					{
						item = new Vector3(vector2.x, vector2.y);
						vector3 = new Vector2(vector2.x, vector2.y);
					}
					BGTriangulator2D.Vertices.Add(item);
					BGTriangulator2D.Points.Add(vector3);
					if (vector3.x < vector.x)
					{
						vector.x = vector3.x;
					}
					if (vector3.y < vector.y)
					{
						vector.y = vector3.y;
					}
					if (vector3.x > vector.z)
					{
						vector.z = vector3.x;
					}
					if (vector3.y > vector.w)
					{
						vector.w = vector3.y;
					}
				}
				BGTriangulator2D.Triangulate(BGTriangulator2D.Points, BGTriangulator2D.Triangles);
				if (config.AutoFlip)
				{
					Vector3 lhs = BGTriangulator2D.Points[BGTriangulator2D.Triangles[1]] - BGTriangulator2D.Points[BGTriangulator2D.Triangles[0]];
					Vector3 rhs = BGTriangulator2D.Points[BGTriangulator2D.Triangles[2]] - BGTriangulator2D.Points[BGTriangulator2D.Triangles[0]];
					if (Vector3.Dot(Vector3.Cross(lhs, rhs), Camera.main.transform.forward) > 0f)
					{
						BGTriangulator2D.Triangles.Reverse();
					}
				}
				else if (!config.Flip)
				{
					BGTriangulator2D.Triangles.Reverse();
				}
				if (config.UvMode == BGTriangulator2D.Config.UvModeEnum.Scale)
				{
					BGTriangulator2D.Bind(vector, config.ScaleUV, config.OffsetUV);
				}
				else
				{
					this.Bind(vector, config.PixelsPerUnit, config.TextureSize);
				}
				if (config.DoubleSided)
				{
					int count = BGTriangulator2D.Vertices.Count;
					for (int j = 0; j < count; j++)
					{
						BGTriangulator2D.Vertices.Add(BGTriangulator2D.Vertices[j]);
					}
					for (int k = BGTriangulator2D.Triangles.Count - 1; k >= 0; k--)
					{
						BGTriangulator2D.Triangles.Add(BGTriangulator2D.Triangles[k] + count);
					}
					if (config.UvMode == BGTriangulator2D.Config.UvModeEnum.Scale)
					{
						BGTriangulator2D.Bind(vector, config.ScaleBackUV, config.OffsetBackUV);
					}
					else
					{
						this.Bind(vector, config.PixelsPerUnitBack, config.TextureSize);
					}
				}
			}
			mesh.Clear();
			mesh.SetVertices(BGTriangulator2D.Vertices);
			mesh.SetTriangles(BGTriangulator2D.Triangles, 0);
			mesh.SetUVs(0, BGTriangulator2D.Uvs);
			mesh.RecalculateNormals();
			this.Clear();
		}

		// Token: 0x060010AC RID: 4268 RVA: 0x0004DE23 File Offset: 0x0004C023
		private void Clear()
		{
			BGTriangulator2D.Vertices.Clear();
			BGTriangulator2D.Triangles.Clear();
			BGTriangulator2D.Uvs.Clear();
			BGTriangulator2D.Points.Clear();
		}

		// Token: 0x060010AD RID: 4269 RVA: 0x000E051C File Offset: 0x000DE71C
		private static void Bind(Vector4 minMax, Vector2 scale, Vector2 offset)
		{
			float num = minMax.z - minMax.x;
			float num2 = minMax.w - minMax.y;
			int count = BGTriangulator2D.Points.Count;
			float num3 = Mathf.Clamp(scale.x, 1E-06f, 1000000f);
			float num4 = Mathf.Clamp(scale.y, 1E-06f, 1000000f);
			for (int i = 0; i < count; i++)
			{
				Vector2 vector = BGTriangulator2D.Points[i];
				BGTriangulator2D.Uvs.Add(new Vector2(offset.x + (vector.x - minMax.x) / num * num3, offset.y + (vector.y - minMax.y) / num2 * num4));
			}
		}

		// Token: 0x060010AE RID: 4270 RVA: 0x000E05E0 File Offset: 0x000DE7E0
		private void Bind(Vector4 minMax, BGPpu pixelsPerUnit, BGPpu textureSize)
		{
			float num = (float)pixelsPerUnit.X / (float)textureSize.X;
			float num2 = (float)pixelsPerUnit.Y / (float)textureSize.Y;
			int count = BGTriangulator2D.Points.Count;
			for (int i = 0; i < count; i++)
			{
				Vector2 vector = BGTriangulator2D.Points[i];
				float x = (vector.x - minMax.x) * num;
				float y = (vector.y - minMax.y) * num2;
				BGTriangulator2D.Uvs.Add(new Vector2(x, y));
			}
		}

		// Token: 0x060010AF RID: 4271 RVA: 0x000E0668 File Offset: 0x000DE868
		private static void Triangulate(List<Vector2> points, List<int> tris)
		{
			tris.Clear();
			int count = points.Count;
			if (count < 3)
			{
				return;
			}
			BGTriangulator2D.V.Clear();
			if (BGTriangulator2D.Area(points) > 0f)
			{
				for (int i = 0; i < count; i++)
				{
					BGTriangulator2D.V.Add(i);
				}
			}
			else
			{
				for (int j = 0; j < count; j++)
				{
					BGTriangulator2D.V.Add(count - 1 - j);
				}
			}
			int k = count;
			int num = 2 * k;
			int num2 = 0;
			int num3 = k - 1;
			while (k > 2)
			{
				if (num-- <= 0)
				{
					return;
				}
				int num4 = num3;
				if (k <= num4)
				{
					num4 = 0;
				}
				num3 = num4 + 1;
				if (k <= num3)
				{
					num3 = 0;
				}
				int num5 = num3 + 1;
				if (k <= num5)
				{
					num5 = 0;
				}
				if (BGTriangulator2D.Snip(points, num4, num3, num5, k, BGTriangulator2D.V))
				{
					int item = BGTriangulator2D.V[num4];
					int item2 = BGTriangulator2D.V[num3];
					int item3 = BGTriangulator2D.V[num5];
					tris.Add(item);
					tris.Add(item2);
					tris.Add(item3);
					num2++;
					int num6 = num3;
					for (int l = num3 + 1; l < k; l++)
					{
						BGTriangulator2D.V[num6] = BGTriangulator2D.V[l];
						num6++;
					}
					k--;
					num = 2 * k;
				}
			}
		}

		// Token: 0x060010B0 RID: 4272 RVA: 0x000E07C0 File Offset: 0x000DE9C0
		private static float Area(List<Vector2> points)
		{
			int count = points.Count;
			float num = 0f;
			int index = count - 1;
			int i = 0;
			while (i < count)
			{
				Vector2 vector = points[index];
				Vector2 vector2 = points[i];
				num += vector.x * vector2.y - vector2.x * vector.y;
				index = i++;
			}
			return num * 0.5f;
		}

		// Token: 0x060010B1 RID: 4273 RVA: 0x000E0828 File Offset: 0x000DEA28
		private static bool Snip(List<Vector2> points, int u, int v, int w, int n, List<int> V)
		{
			Vector2 vector = points[V[u]];
			Vector2 vector2 = points[V[v]];
			Vector2 vector3 = points[V[w]];
			if (Mathf.Epsilon > (vector2.x - vector.x) * (vector3.y - vector.y) - (vector2.y - vector.y) * (vector3.x - vector.x))
			{
				return false;
			}
			for (int i = 0; i < n; i++)
			{
				if (i != u && i != v && i != w)
				{
					Vector2 p = points[V[i]];
					if (BGTriangulator2D.InsideTriangle(vector, vector2, vector3, p))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060010B2 RID: 4274 RVA: 0x000E08DC File Offset: 0x000DEADC
		private static bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
		{
			float num = C.x - B.x;
			float num2 = C.y - B.y;
			float num3 = A.x - C.x;
			float num4 = A.y - C.y;
			float num5 = B.x - A.x;
			float num6 = B.y - A.y;
			float num7 = P.x - A.x;
			float num8 = P.y - A.y;
			float num9 = P.x - B.x;
			float num10 = P.y - B.y;
			float num11 = P.x - C.x;
			float num12 = P.y - C.y;
			float num13 = num * num10 - num2 * num9;
			float num14 = num5 * num8 - num6 * num7;
			float num15 = num3 * num12 - num4 * num11;
			return num13 >= 0f && num15 >= 0f && num14 >= 0f;
		}

		// Token: 0x04000DE8 RID: 3560
		private const float MinUvScale = 1E-06f;

		// Token: 0x04000DE9 RID: 3561
		private const float MaxUvScale = 1000000f;

		// Token: 0x04000DEA RID: 3562
		private static readonly List<int> V = new List<int>();

		// Token: 0x04000DEB RID: 3563
		private static readonly List<Vector3> Vertices = new List<Vector3>();

		// Token: 0x04000DEC RID: 3564
		private static readonly List<Vector2> Points = new List<Vector2>();

		// Token: 0x04000DED RID: 3565
		private static readonly List<Vector2> Uvs = new List<Vector2>();

		// Token: 0x04000DEE RID: 3566
		private static readonly List<int> Triangles = new List<int>();

		// Token: 0x020001D5 RID: 469
		public class Config
		{
			// Token: 0x04000DEF RID: 3567
			public BGTriangulator2D.Config.UvModeEnum UvMode;

			// Token: 0x04000DF0 RID: 3568
			public bool Closed;

			// Token: 0x04000DF1 RID: 3569
			public bool Flip;

			// Token: 0x04000DF2 RID: 3570
			public bool AutoFlip;

			// Token: 0x04000DF3 RID: 3571
			public BGCurve.Mode2DEnum Mode2D;

			// Token: 0x04000DF4 RID: 3572
			public bool DoubleSided;

			// Token: 0x04000DF5 RID: 3573
			public Vector2 ScaleUV = Vector2.one;

			// Token: 0x04000DF6 RID: 3574
			public Vector2 OffsetUV = Vector2.zero;

			// Token: 0x04000DF7 RID: 3575
			public Vector2 ScaleBackUV = Vector2.one;

			// Token: 0x04000DF8 RID: 3576
			public Vector2 OffsetBackUV = Vector2.zero;

			// Token: 0x04000DF9 RID: 3577
			public BGPpu TextureSize;

			// Token: 0x04000DFA RID: 3578
			public BGPpu PixelsPerUnit;

			// Token: 0x04000DFB RID: 3579
			public BGPpu PixelsPerUnitBack;

			// Token: 0x020001D6 RID: 470
			public enum UvModeEnum
			{
				// Token: 0x04000DFD RID: 3581
				Scale,
				// Token: 0x04000DFE RID: 3582
				PPU
			}
		}
	}
}
