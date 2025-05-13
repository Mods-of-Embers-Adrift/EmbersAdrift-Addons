using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.Voronoi.Legacy
{
	// Token: 0x020000FA RID: 250
	[Obsolete("Use TriangleNet.Voronoi.BoundedVoronoi class instead.")]
	public class BoundedVoronoiLegacy : IVoronoi
	{
		// Token: 0x060008DB RID: 2267 RVA: 0x00048C24 File Offset: 0x00046E24
		public BoundedVoronoiLegacy(Mesh mesh) : this(mesh, true)
		{
		}

		// Token: 0x060008DC RID: 2268 RVA: 0x00048C2E File Offset: 0x00046E2E
		public BoundedVoronoiLegacy(Mesh mesh, bool includeBoundary)
		{
			this.mesh = mesh;
			this.includeBoundary = includeBoundary;
			this.Generate();
		}

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x060008DD RID: 2269 RVA: 0x00048C5C File Offset: 0x00046E5C
		public Point[] Points
		{
			get
			{
				return this.points;
			}
		}

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x060008DE RID: 2270 RVA: 0x00048C64 File Offset: 0x00046E64
		public ICollection<VoronoiRegion> Regions
		{
			get
			{
				return this.regions;
			}
		}

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x060008DF RID: 2271 RVA: 0x00048C6C File Offset: 0x00046E6C
		public IEnumerable<IEdge> Edges
		{
			get
			{
				return this.EnumerateEdges();
			}
		}

		// Token: 0x060008E0 RID: 2272 RVA: 0x000BE24C File Offset: 0x000BC44C
		private void Generate()
		{
			this.mesh.Renumber();
			this.mesh.MakeVertexMap();
			this.regions = new List<VoronoiRegion>(this.mesh.vertices.Count);
			this.points = new Point[this.mesh.triangles.Count];
			this.segPoints = new List<Point>(this.mesh.subsegs.Count * 4);
			this.ComputeCircumCenters();
			this.TagBlindTriangles();
			foreach (Vertex vertex in this.mesh.vertices.Values)
			{
				if (vertex.type == VertexType.FreeVertex || vertex.label == 0)
				{
					this.ConstructCell(vertex);
				}
				else if (this.includeBoundary)
				{
					this.ConstructBoundaryCell(vertex);
				}
			}
			int num = this.points.Length;
			Array.Resize<Point>(ref this.points, num + this.segPoints.Count);
			for (int i = 0; i < this.segPoints.Count; i++)
			{
				this.points[num + i] = this.segPoints[i];
			}
			this.segPoints.Clear();
			this.segPoints = null;
		}

		// Token: 0x060008E1 RID: 2273 RVA: 0x000BE3A0 File Offset: 0x000BC5A0
		private void ComputeCircumCenters()
		{
			Otri otri = default(Otri);
			double num = 0.0;
			double num2 = 0.0;
			foreach (Triangle triangle in this.mesh.triangles)
			{
				otri.tri = triangle;
				Point point = this.predicates.FindCircumcenter(otri.Org(), otri.Dest(), otri.Apex(), ref num, ref num2);
				point.id = triangle.id;
				this.points[triangle.id] = point;
			}
		}

		// Token: 0x060008E2 RID: 2274 RVA: 0x000BE458 File Offset: 0x000BC658
		private void TagBlindTriangles()
		{
			int num = 0;
			this.subsegMap = new Dictionary<int, SubSegment>();
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Osub osub = default(Osub);
			Osub osub2 = default(Osub);
			foreach (Triangle triangle in this.mesh.triangles)
			{
				triangle.infected = false;
			}
			foreach (SubSegment seg in this.mesh.subsegs.Values)
			{
				Stack<Triangle> stack = new Stack<Triangle>();
				osub.seg = seg;
				osub.orient = 0;
				osub.Pivot(ref otri);
				if (otri.tri.id != -1 && !otri.tri.infected)
				{
					stack.Push(otri.tri);
				}
				osub.Sym();
				osub.Pivot(ref otri);
				if (otri.tri.id != -1 && !otri.tri.infected)
				{
					stack.Push(otri.tri);
				}
				while (stack.Count > 0)
				{
					otri.tri = stack.Pop();
					otri.orient = 0;
					if (this.TriangleIsBlinded(ref otri, ref osub))
					{
						otri.tri.infected = true;
						num++;
						this.subsegMap.Add(otri.tri.hash, osub.seg);
						otri.orient = 0;
						while (otri.orient < 3)
						{
							otri.Sym(ref otri2);
							otri2.Pivot(ref osub2);
							if (otri2.tri.id != -1 && !otri2.tri.infected && osub2.seg.hash == -1)
							{
								stack.Push(otri2.tri);
							}
							otri.orient++;
						}
					}
				}
			}
			num = 0;
		}

		// Token: 0x060008E3 RID: 2275 RVA: 0x000BE690 File Offset: 0x000BC890
		private bool TriangleIsBlinded(ref Otri tri, ref Osub seg)
		{
			Vertex p = tri.Org();
			Vertex p2 = tri.Dest();
			Vertex p3 = tri.Apex();
			Vertex p4 = seg.Org();
			Vertex p5 = seg.Dest();
			Point p6 = this.points[tri.tri.id];
			Point point;
			return this.SegmentsIntersect(p4, p5, p6, p, out point, true) || this.SegmentsIntersect(p4, p5, p6, p2, out point, true) || this.SegmentsIntersect(p4, p5, p6, p3, out point, true);
		}

		// Token: 0x060008E4 RID: 2276 RVA: 0x000BE714 File Offset: 0x000BC914
		private void ConstructCell(Vertex vertex)
		{
			VoronoiRegion voronoiRegion = new VoronoiRegion(vertex);
			this.regions.Add(voronoiRegion);
			Otri otri = default(Otri);
			Otri ot = default(Otri);
			Otri otri2 = default(Otri);
			Osub osub = default(Osub);
			Osub os = default(Osub);
			int count = this.mesh.triangles.Count;
			List<Point> list = new List<Point>();
			vertex.tri.Copy(ref ot);
			if (ot.Org() != vertex)
			{
				throw new Exception("ConstructCell: inconsistent topology.");
			}
			ot.Copy(ref otri);
			ot.Onext(ref otri2);
			do
			{
				Point point = this.points[otri.tri.id];
				Point p = this.points[otri2.tri.id];
				if (!otri.tri.infected)
				{
					list.Add(point);
					if (otri2.tri.infected)
					{
						os.seg = this.subsegMap[otri2.tri.hash];
						Point point2;
						if (this.SegmentsIntersect(os.Org(), os.Dest(), point, p, out point2, true))
						{
							Point point3 = point2;
							int num = count;
							int num2 = this.segIndex;
							this.segIndex = num2 + 1;
							point3.id = num + num2;
							this.segPoints.Add(point2);
							list.Add(point2);
						}
					}
				}
				else
				{
					osub.seg = this.subsegMap[otri.tri.hash];
					if (!otri2.tri.infected)
					{
						Point point2;
						if (this.SegmentsIntersect(osub.Org(), osub.Dest(), point, p, out point2, true))
						{
							Point point4 = point2;
							int num3 = count;
							int num2 = this.segIndex;
							this.segIndex = num2 + 1;
							point4.id = num3 + num2;
							this.segPoints.Add(point2);
							list.Add(point2);
						}
					}
					else
					{
						os.seg = this.subsegMap[otri2.tri.hash];
						if (!osub.Equal(os))
						{
							Point point2;
							if (this.SegmentsIntersect(osub.Org(), osub.Dest(), point, p, out point2, true))
							{
								Point point5 = point2;
								int num4 = count;
								int num2 = this.segIndex;
								this.segIndex = num2 + 1;
								point5.id = num4 + num2;
								this.segPoints.Add(point2);
								list.Add(point2);
							}
							if (this.SegmentsIntersect(os.Org(), os.Dest(), point, p, out point2, true))
							{
								Point point6 = point2;
								int num5 = count;
								int num2 = this.segIndex;
								this.segIndex = num2 + 1;
								point6.id = num5 + num2;
								this.segPoints.Add(point2);
								list.Add(point2);
							}
						}
					}
				}
				otri2.Copy(ref otri);
				otri2.Onext();
			}
			while (!otri.Equals(ot));
			voronoiRegion.Add(list);
		}

		// Token: 0x060008E5 RID: 2277 RVA: 0x000BE9EC File Offset: 0x000BCBEC
		private void ConstructBoundaryCell(Vertex vertex)
		{
			VoronoiRegion voronoiRegion = new VoronoiRegion(vertex);
			this.regions.Add(voronoiRegion);
			Otri otri = default(Otri);
			Otri ot = default(Otri);
			Otri otri2 = default(Otri);
			Otri otri3 = default(Otri);
			Osub osub = default(Osub);
			Osub os = default(Osub);
			int count = this.mesh.triangles.Count;
			List<Point> list = new List<Point>();
			vertex.tri.Copy(ref ot);
			if (ot.Org() != vertex)
			{
				throw new Exception("ConstructBoundaryCell: inconsistent topology.");
			}
			ot.Copy(ref otri);
			ot.Onext(ref otri2);
			ot.Oprev(ref otri3);
			if (otri3.tri.id != -1)
			{
				while (otri3.tri.id != -1 && !otri3.Equals(ot))
				{
					otri3.Copy(ref otri);
					otri3.Oprev();
				}
				otri.Copy(ref ot);
				otri.Onext(ref otri2);
			}
			Point point;
			int num2;
			if (otri3.tri.id == -1)
			{
				point = new Point(vertex.x, vertex.y);
				Point point2 = point;
				int num = count;
				num2 = this.segIndex;
				this.segIndex = num2 + 1;
				point2.id = num + num2;
				this.segPoints.Add(point);
				list.Add(point);
			}
			Vertex vertex2 = otri.Org();
			Vertex vertex3 = otri.Dest();
			point = new Point((vertex2.x + vertex3.x) / 2.0, (vertex2.y + vertex3.y) / 2.0);
			Point point3 = point;
			int num3 = count;
			num2 = this.segIndex;
			this.segIndex = num2 + 1;
			point3.id = num3 + num2;
			this.segPoints.Add(point);
			list.Add(point);
			Point point4;
			Vertex vertex4;
			for (;;)
			{
				point4 = this.points[otri.tri.id];
				if (otri2.tri.id == -1)
				{
					break;
				}
				Point p = this.points[otri2.tri.id];
				if (!otri.tri.infected)
				{
					list.Add(point4);
					if (otri2.tri.infected)
					{
						os.seg = this.subsegMap[otri2.tri.hash];
						if (this.SegmentsIntersect(os.Org(), os.Dest(), point4, p, out point, true))
						{
							Point point5 = point;
							int num4 = count;
							num2 = this.segIndex;
							this.segIndex = num2 + 1;
							point5.id = num4 + num2;
							this.segPoints.Add(point);
							list.Add(point);
						}
					}
				}
				else
				{
					osub.seg = this.subsegMap[otri.tri.hash];
					Vertex p2 = osub.Org();
					Vertex p3 = osub.Dest();
					if (!otri2.tri.infected)
					{
						vertex3 = otri.Dest();
						vertex4 = otri.Apex();
						Point p4 = new Point((vertex3.x + vertex4.x) / 2.0, (vertex3.y + vertex4.y) / 2.0);
						if (this.SegmentsIntersect(p2, p3, p4, point4, out point, false))
						{
							Point point6 = point;
							int num5 = count;
							num2 = this.segIndex;
							this.segIndex = num2 + 1;
							point6.id = num5 + num2;
							this.segPoints.Add(point);
							list.Add(point);
						}
						if (this.SegmentsIntersect(p2, p3, point4, p, out point, true))
						{
							Point point7 = point;
							int num6 = count;
							num2 = this.segIndex;
							this.segIndex = num2 + 1;
							point7.id = num6 + num2;
							this.segPoints.Add(point);
							list.Add(point);
						}
					}
					else
					{
						os.seg = this.subsegMap[otri2.tri.hash];
						if (!osub.Equal(os))
						{
							if (this.SegmentsIntersect(p2, p3, point4, p, out point, true))
							{
								Point point8 = point;
								int num7 = count;
								num2 = this.segIndex;
								this.segIndex = num2 + 1;
								point8.id = num7 + num2;
								this.segPoints.Add(point);
								list.Add(point);
							}
							if (this.SegmentsIntersect(os.Org(), os.Dest(), point4, p, out point, true))
							{
								Point point9 = point;
								int num8 = count;
								num2 = this.segIndex;
								this.segIndex = num2 + 1;
								point9.id = num8 + num2;
								this.segPoints.Add(point);
								list.Add(point);
							}
						}
						else
						{
							Point p5 = new Point((vertex2.x + vertex3.x) / 2.0, (vertex2.y + vertex3.y) / 2.0);
							if (this.SegmentsIntersect(p2, p3, p5, p, out point, false))
							{
								Point point10 = point;
								int num9 = count;
								num2 = this.segIndex;
								this.segIndex = num2 + 1;
								point10.id = num9 + num2;
								this.segPoints.Add(point);
								list.Add(point);
							}
						}
					}
				}
				otri2.Copy(ref otri);
				otri2.Onext();
				if (otri.Equals(ot))
				{
					goto IL_5B9;
				}
			}
			if (!otri.tri.infected)
			{
				list.Add(point4);
			}
			vertex2 = otri.Org();
			vertex4 = otri.Apex();
			point = new Point((vertex2.x + vertex4.x) / 2.0, (vertex2.y + vertex4.y) / 2.0);
			Point point11 = point;
			int num10 = count;
			num2 = this.segIndex;
			this.segIndex = num2 + 1;
			point11.id = num10 + num2;
			this.segPoints.Add(point);
			list.Add(point);
			IL_5B9:
			voronoiRegion.Add(list);
		}

		// Token: 0x060008E6 RID: 2278 RVA: 0x000BEFBC File Offset: 0x000BD1BC
		private bool SegmentsIntersect(Point p1, Point p2, Point p3, Point p4, out Point p, bool strictIntersect)
		{
			p = null;
			double x = p1.x;
			double y = p1.y;
			double num = p2.x;
			double num2 = p2.y;
			double num3 = p3.x;
			double num4 = p3.y;
			double num5 = p4.x;
			double num6 = p4.y;
			if ((x == num && y == num2) || (num3 == num5 && num4 == num6))
			{
				return false;
			}
			if ((x == num3 && y == num4) || (num == num3 && num2 == num4) || (x == num5 && y == num6) || (num == num5 && num2 == num6))
			{
				return false;
			}
			num -= x;
			num2 -= y;
			num3 -= x;
			num4 -= y;
			num5 -= x;
			num6 -= y;
			double num7 = Math.Sqrt(num * num + num2 * num2);
			double num8 = num / num7;
			double num9 = num2 / num7;
			double num10 = num3 * num8 + num4 * num9;
			num4 = num4 * num8 - num3 * num9;
			num3 = num10;
			double num11 = num5 * num8 + num6 * num9;
			num6 = num6 * num8 - num5 * num9;
			num5 = num11;
			if ((num4 < 0.0 && num6 < 0.0) || (num4 >= 0.0 && num6 >= 0.0 && strictIntersect))
			{
				return false;
			}
			double num12 = num5 + (num3 - num5) * num6 / (num6 - num4);
			if (num12 < 0.0 || (num12 > num7 && strictIntersect))
			{
				return false;
			}
			p = new Point(x + num12 * num8, y + num12 * num9);
			return true;
		}

		// Token: 0x060008E7 RID: 2279 RVA: 0x000BF140 File Offset: 0x000BD340
		private IEnumerable<IEdge> EnumerateEdges()
		{
			List<IEdge> list = new List<IEdge>(this.Regions.Count * 2);
			foreach (VoronoiRegion voronoiRegion in this.Regions)
			{
				Point point = null;
				Point point2 = null;
				foreach (Point point3 in voronoiRegion.Vertices)
				{
					if (point == null)
					{
						point = point3;
						point2 = point3;
					}
					else
					{
						list.Add(new Edge(point2.id, point3.id));
						point2 = point3;
					}
				}
				if (voronoiRegion.Bounded && point != null)
				{
					list.Add(new Edge(point2.id, point.id));
				}
			}
			return list;
		}

		// Token: 0x04000A08 RID: 2568
		private IPredicates predicates = RobustPredicates.Default;

		// Token: 0x04000A09 RID: 2569
		private Mesh mesh;

		// Token: 0x04000A0A RID: 2570
		private Point[] points;

		// Token: 0x04000A0B RID: 2571
		private List<VoronoiRegion> regions;

		// Token: 0x04000A0C RID: 2572
		private List<Point> segPoints;

		// Token: 0x04000A0D RID: 2573
		private int segIndex;

		// Token: 0x04000A0E RID: 2574
		private Dictionary<int, SubSegment> subsegMap;

		// Token: 0x04000A0F RID: 2575
		private bool includeBoundary = true;
	}
}
