using System;
using System.Collections.Generic;
using System.Linq;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.Tools
{
	// Token: 0x02000112 RID: 274
	public class TriangleQuadTree
	{
		// Token: 0x060009D6 RID: 2518 RVA: 0x000C2A18 File Offset: 0x000C0C18
		public TriangleQuadTree(Mesh mesh, int maxDepth = 10, int sizeBound = 10)
		{
			this.maxDepth = maxDepth;
			this.sizeBound = sizeBound;
			ITriangle[] array = mesh.Triangles.ToArray<Triangle>();
			this.triangles = array;
			int num = 0;
			this.root = new TriangleQuadTree.QuadNode(mesh.Bounds, this, true);
			this.root.CreateSubRegion(num + 1);
		}

		// Token: 0x060009D7 RID: 2519 RVA: 0x000C2A74 File Offset: 0x000C0C74
		public ITriangle Query(double x, double y)
		{
			Point point = new Point(x, y);
			foreach (int num in this.root.FindTriangles(point))
			{
				ITriangle triangle = this.triangles[num];
				if (TriangleQuadTree.IsPointInTriangle(point, triangle.GetVertex(0), triangle.GetVertex(1), triangle.GetVertex(2)))
				{
					return triangle;
				}
			}
			return null;
		}

		// Token: 0x060009D8 RID: 2520 RVA: 0x000C2B00 File Offset: 0x000C0D00
		internal static bool IsPointInTriangle(Point p, Point t0, Point t1, Point t2)
		{
			Point point = new Point(t1.x - t0.x, t1.y - t0.y);
			Point point2 = new Point(t2.x - t0.x, t2.y - t0.y);
			Point p2 = new Point(p.x - t0.x, p.y - t0.y);
			Point q = new Point(-point.y, point.x);
			Point q2 = new Point(-point2.y, point2.x);
			double num = TriangleQuadTree.DotProduct(p2, q2) / TriangleQuadTree.DotProduct(point, q2);
			double num2 = TriangleQuadTree.DotProduct(p2, q) / TriangleQuadTree.DotProduct(point2, q);
			return num >= 0.0 && num2 >= 0.0 && num + num2 <= 1.0;
		}

		// Token: 0x060009D9 RID: 2521 RVA: 0x00049990 File Offset: 0x00047B90
		internal static double DotProduct(Point p, Point q)
		{
			return p.x * q.x + p.y * q.y;
		}

		// Token: 0x04000A75 RID: 2677
		private TriangleQuadTree.QuadNode root;

		// Token: 0x04000A76 RID: 2678
		internal ITriangle[] triangles;

		// Token: 0x04000A77 RID: 2679
		internal int sizeBound;

		// Token: 0x04000A78 RID: 2680
		internal int maxDepth;

		// Token: 0x02000113 RID: 275
		private class QuadNode
		{
			// Token: 0x060009DA RID: 2522 RVA: 0x000499AD File Offset: 0x00047BAD
			public QuadNode(Rectangle box, TriangleQuadTree tree) : this(box, tree, false)
			{
			}

			// Token: 0x060009DB RID: 2523 RVA: 0x000C2BE0 File Offset: 0x000C0DE0
			public QuadNode(Rectangle box, TriangleQuadTree tree, bool init)
			{
				this.tree = tree;
				this.bounds = new Rectangle(box.Left, box.Bottom, box.Width, box.Height);
				this.pivot = new Point((box.Left + box.Right) / 2.0, (box.Bottom + box.Top) / 2.0);
				this.bitRegions = 0;
				this.regions = new TriangleQuadTree.QuadNode[4];
				this.triangles = new List<int>();
				if (init)
				{
					int num = tree.triangles.Length;
					this.triangles.Capacity = num;
					for (int i = 0; i < num; i++)
					{
						this.triangles.Add(i);
					}
				}
			}

			// Token: 0x060009DC RID: 2524 RVA: 0x000C2CA4 File Offset: 0x000C0EA4
			public List<int> FindTriangles(Point searchPoint)
			{
				int num = this.FindRegion(searchPoint);
				if (this.regions[num] == null)
				{
					return this.triangles;
				}
				return this.regions[num].FindTriangles(searchPoint);
			}

			// Token: 0x060009DD RID: 2525 RVA: 0x000C2CD8 File Offset: 0x000C0ED8
			public void CreateSubRegion(int currentDepth)
			{
				double width = this.bounds.Right - this.pivot.x;
				double height = this.bounds.Top - this.pivot.y;
				Rectangle box = new Rectangle(this.bounds.Left, this.bounds.Bottom, width, height);
				this.regions[0] = new TriangleQuadTree.QuadNode(box, this.tree);
				box = new Rectangle(this.pivot.x, this.bounds.Bottom, width, height);
				this.regions[1] = new TriangleQuadTree.QuadNode(box, this.tree);
				box = new Rectangle(this.bounds.Left, this.pivot.y, width, height);
				this.regions[2] = new TriangleQuadTree.QuadNode(box, this.tree);
				box = new Rectangle(this.pivot.x, this.pivot.y, width, height);
				this.regions[3] = new TriangleQuadTree.QuadNode(box, this.tree);
				Point[] array = new Point[3];
				foreach (int num in this.triangles)
				{
					ITriangle triangle = this.tree.triangles[num];
					array[0] = triangle.GetVertex(0);
					array[1] = triangle.GetVertex(1);
					array[2] = triangle.GetVertex(2);
					this.AddTriangleToRegion(array, num);
				}
				for (int i = 0; i < 4; i++)
				{
					if (this.regions[i].triangles.Count > this.tree.sizeBound && currentDepth < this.tree.maxDepth)
					{
						this.regions[i].CreateSubRegion(currentDepth + 1);
					}
				}
			}

			// Token: 0x060009DE RID: 2526 RVA: 0x000C2EB0 File Offset: 0x000C10B0
			private void AddTriangleToRegion(Point[] triangle, int index)
			{
				this.bitRegions = 0;
				if (TriangleQuadTree.IsPointInTriangle(this.pivot, triangle[0], triangle[1], triangle[2]))
				{
					this.AddToRegion(index, 0);
					this.AddToRegion(index, 1);
					this.AddToRegion(index, 2);
					this.AddToRegion(index, 3);
					return;
				}
				this.FindTriangleIntersections(triangle, index);
				if (this.bitRegions == 0)
				{
					int num = this.FindRegion(triangle[0]);
					this.regions[num].triangles.Add(index);
				}
			}

			// Token: 0x060009DF RID: 2527 RVA: 0x000C2F28 File Offset: 0x000C1128
			private void FindTriangleIntersections(Point[] triangle, int index)
			{
				int num = 2;
				int i = 0;
				while (i < 3)
				{
					double num2 = triangle[i].x - triangle[num].x;
					double num3 = triangle[i].y - triangle[num].y;
					if (num2 != 0.0)
					{
						this.FindIntersectionsWithX(num2, num3, triangle, index, num);
					}
					if (num3 != 0.0)
					{
						this.FindIntersectionsWithY(num2, num3, triangle, index, num);
					}
					num = i++;
				}
			}

			// Token: 0x060009E0 RID: 2528 RVA: 0x000C2F98 File Offset: 0x000C1198
			private void FindIntersectionsWithX(double dx, double dy, Point[] triangle, int index, int k)
			{
				double num = (this.pivot.x - triangle[k].x) / dx;
				if (num < 1.000001 && num > -1E-06)
				{
					double num2 = triangle[k].y + num * dy;
					if (num2 < this.pivot.y && num2 >= this.bounds.Bottom)
					{
						this.AddToRegion(index, 0);
						this.AddToRegion(index, 1);
					}
					else if (num2 <= this.bounds.Top)
					{
						this.AddToRegion(index, 2);
						this.AddToRegion(index, 3);
					}
				}
				num = (this.bounds.Left - triangle[k].x) / dx;
				if (num < 1.000001 && num > -1E-06)
				{
					double num3 = triangle[k].y + num * dy;
					if (num3 < this.pivot.y && num3 >= this.bounds.Bottom)
					{
						this.AddToRegion(index, 0);
					}
					else if (num3 <= this.bounds.Top)
					{
						this.AddToRegion(index, 2);
					}
				}
				num = (this.bounds.Right - triangle[k].x) / dx;
				if (num < 1.000001 && num > -1E-06)
				{
					double num4 = triangle[k].y + num * dy;
					if (num4 < this.pivot.y && num4 >= this.bounds.Bottom)
					{
						this.AddToRegion(index, 1);
						return;
					}
					if (num4 <= this.bounds.Top)
					{
						this.AddToRegion(index, 3);
					}
				}
			}

			// Token: 0x060009E1 RID: 2529 RVA: 0x000C312C File Offset: 0x000C132C
			private void FindIntersectionsWithY(double dx, double dy, Point[] triangle, int index, int k)
			{
				double num = (this.pivot.y - triangle[k].y) / dy;
				if (num < 1.000001 && num > -1E-06)
				{
					double num2 = triangle[k].x + num * dx;
					if (num2 > this.pivot.x && num2 <= this.bounds.Right)
					{
						this.AddToRegion(index, 1);
						this.AddToRegion(index, 3);
					}
					else if (num2 >= this.bounds.Left)
					{
						this.AddToRegion(index, 0);
						this.AddToRegion(index, 2);
					}
				}
				num = (this.bounds.Bottom - triangle[k].y) / dy;
				if (num < 1.000001 && num > -1E-06)
				{
					double num2 = triangle[k].x + num * dx;
					if (num2 > this.pivot.x && num2 <= this.bounds.Right)
					{
						this.AddToRegion(index, 1);
					}
					else if (num2 >= this.bounds.Left)
					{
						this.AddToRegion(index, 0);
					}
				}
				num = (this.bounds.Top - triangle[k].y) / dy;
				if (num < 1.000001 && num > -1E-06)
				{
					double num2 = triangle[k].x + num * dx;
					if (num2 > this.pivot.x && num2 <= this.bounds.Right)
					{
						this.AddToRegion(index, 3);
						return;
					}
					if (num2 >= this.bounds.Left)
					{
						this.AddToRegion(index, 2);
					}
				}
			}

			// Token: 0x060009E2 RID: 2530 RVA: 0x000C32C0 File Offset: 0x000C14C0
			private int FindRegion(Point point)
			{
				int num = 2;
				if (point.y < this.pivot.y)
				{
					num = 0;
				}
				if (point.x > this.pivot.x)
				{
					num++;
				}
				return num;
			}

			// Token: 0x060009E3 RID: 2531 RVA: 0x000499B8 File Offset: 0x00047BB8
			private void AddToRegion(int index, int region)
			{
				if ((this.bitRegions & TriangleQuadTree.QuadNode.BITVECTOR[region]) == 0)
				{
					this.regions[region].triangles.Add(index);
					this.bitRegions |= TriangleQuadTree.QuadNode.BITVECTOR[region];
				}
			}

			// Token: 0x04000A79 RID: 2681
			private const int SW = 0;

			// Token: 0x04000A7A RID: 2682
			private const int SE = 1;

			// Token: 0x04000A7B RID: 2683
			private const int NW = 2;

			// Token: 0x04000A7C RID: 2684
			private const int NE = 3;

			// Token: 0x04000A7D RID: 2685
			private const double EPS = 1E-06;

			// Token: 0x04000A7E RID: 2686
			private static readonly byte[] BITVECTOR = new byte[]
			{
				1,
				2,
				4,
				8
			};

			// Token: 0x04000A7F RID: 2687
			private Rectangle bounds;

			// Token: 0x04000A80 RID: 2688
			private Point pivot;

			// Token: 0x04000A81 RID: 2689
			private TriangleQuadTree tree;

			// Token: 0x04000A82 RID: 2690
			private TriangleQuadTree.QuadNode[] regions;

			// Token: 0x04000A83 RID: 2691
			private List<int> triangles;

			// Token: 0x04000A84 RID: 2692
			private byte bitRegions;
		}
	}
}
