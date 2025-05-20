using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.Voronoi.Legacy
{
	// Token: 0x020000FC RID: 252
	[Obsolete("Use TriangleNet.Voronoi.StandardVoronoi class instead.")]
	public class SimpleVoronoi : IVoronoi
	{
		// Token: 0x060008EB RID: 2283 RVA: 0x00048C74 File Offset: 0x00046E74
		public SimpleVoronoi(Mesh mesh)
		{
			this.mesh = mesh;
			this.Generate();
		}

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x060008EC RID: 2284 RVA: 0x00048C94 File Offset: 0x00046E94
		public Point[] Points
		{
			get
			{
				return this.points;
			}
		}

		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x060008ED RID: 2285 RVA: 0x00048C9C File Offset: 0x00046E9C
		public ICollection<VoronoiRegion> Regions
		{
			get
			{
				return this.regions.Values;
			}
		}

		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x060008EE RID: 2286 RVA: 0x00048CA9 File Offset: 0x00046EA9
		public IEnumerable<IEdge> Edges
		{
			get
			{
				return this.EnumerateEdges();
			}
		}

		// Token: 0x060008EF RID: 2287 RVA: 0x000BF258 File Offset: 0x000BD458
		private void Generate()
		{
			this.mesh.Renumber();
			this.mesh.MakeVertexMap();
			this.points = new Point[this.mesh.triangles.Count + this.mesh.hullsize];
			this.regions = new Dictionary<int, VoronoiRegion>(this.mesh.vertices.Count);
			this.rayPoints = new Dictionary<int, Point>();
			this.rayIndex = 0;
			this.bounds = new Rectangle();
			this.ComputeCircumCenters();
			foreach (Vertex vertex in this.mesh.vertices.Values)
			{
				this.regions.Add(vertex.id, new VoronoiRegion(vertex));
			}
			foreach (VoronoiRegion region in this.regions.Values)
			{
				this.ConstructCell(region);
			}
		}

		// Token: 0x060008F0 RID: 2288 RVA: 0x000BF388 File Offset: 0x000BD588
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
				this.bounds.Expand(point);
			}
			double num3 = Math.Max(this.bounds.Width, this.bounds.Height);
			this.bounds.Resize(num3, num3);
		}

		// Token: 0x060008F1 RID: 2289 RVA: 0x000BF478 File Offset: 0x000BD678
		private void ConstructCell(VoronoiRegion region)
		{
			Vertex vertex = region.Generator as Vertex;
			List<Point> list = new List<Point>();
			Otri otri = default(Otri);
			Otri ot = default(Otri);
			Otri otri2 = default(Otri);
			Otri otri3 = default(Otri);
			Osub osub = default(Osub);
			vertex.tri.Copy(ref ot);
			ot.Copy(ref otri);
			ot.Onext(ref otri2);
			if (otri2.tri.id == -1)
			{
				ot.Oprev(ref otri3);
				if (otri3.tri.id != -1)
				{
					ot.Copy(ref otri2);
					ot.Oprev();
					ot.Copy(ref otri);
				}
			}
			while (otri2.tri.id != -1)
			{
				list.Add(this.points[otri.tri.id]);
				region.AddNeighbor(otri.tri.id, this.regions[otri.Apex().id]);
				if (otri2.Equals(ot))
				{
					region.Add(list);
					return;
				}
				otri2.Copy(ref otri);
				otri2.Onext();
			}
			region.Bounded = false;
			int count = this.mesh.triangles.Count;
			otri.Lprev(ref otri2);
			otri2.Pivot(ref osub);
			int hash = osub.seg.hash;
			list.Add(this.points[otri.tri.id]);
			region.AddNeighbor(otri.tri.id, this.regions[otri.Apex().id]);
			Point point;
			if (!this.rayPoints.TryGetValue(hash, out point))
			{
				Vertex vertex2 = otri.Org();
				Vertex vertex3 = otri.Apex();
				this.BoxRayIntersection(this.points[otri.tri.id], vertex2.y - vertex3.y, vertex3.x - vertex2.x, out point);
				point.id = count + this.rayIndex;
				this.points[count + this.rayIndex] = point;
				this.rayIndex++;
				this.rayPoints.Add(hash, point);
			}
			list.Add(point);
			list.Reverse();
			ot.Copy(ref otri);
			otri.Oprev(ref otri3);
			while (otri3.tri.id != -1)
			{
				list.Add(this.points[otri3.tri.id]);
				region.AddNeighbor(otri3.tri.id, this.regions[otri3.Apex().id]);
				otri3.Copy(ref otri);
				otri3.Oprev();
			}
			otri.Pivot(ref osub);
			hash = osub.seg.hash;
			if (!this.rayPoints.TryGetValue(hash, out point))
			{
				Vertex vertex2 = otri.Org();
				Vertex vertex4 = otri.Dest();
				this.BoxRayIntersection(this.points[otri.tri.id], vertex4.y - vertex2.y, vertex2.x - vertex4.x, out point);
				point.id = count + this.rayIndex;
				this.rayPoints.Add(hash, point);
				this.points[count + this.rayIndex] = point;
				this.rayIndex++;
			}
			list.Add(point);
			region.AddNeighbor(point.id, this.regions[otri.Dest().id]);
			list.Reverse();
			region.Add(list);
		}

		// Token: 0x060008F2 RID: 2290 RVA: 0x000BF818 File Offset: 0x000BDA18
		private bool BoxRayIntersection(Point pt, double dx, double dy, out Point intersect)
		{
			double x = pt.x;
			double y = pt.y;
			double left = this.bounds.Left;
			double right = this.bounds.Right;
			double bottom = this.bounds.Bottom;
			double top = this.bounds.Top;
			if (x < left || x > right || y < bottom || y > top)
			{
				intersect = null;
				return false;
			}
			double num;
			double x2;
			double y2;
			if (dx < 0.0)
			{
				num = (left - x) / dx;
				x2 = left;
				y2 = y + num * dy;
			}
			else if (dx > 0.0)
			{
				num = (right - x) / dx;
				x2 = right;
				y2 = y + num * dy;
			}
			else
			{
				num = double.MaxValue;
				y2 = (x2 = 0.0);
			}
			double num2;
			double x3;
			double y3;
			if (dy < 0.0)
			{
				num2 = (bottom - y) / dy;
				x3 = x + num2 * dx;
				y3 = bottom;
			}
			else if (dy > 0.0)
			{
				num2 = (top - y) / dy;
				x3 = x + num2 * dx;
				y3 = top;
			}
			else
			{
				num2 = double.MaxValue;
				y3 = (x3 = 0.0);
			}
			if (num < num2)
			{
				intersect = new Point(x2, y2);
			}
			else
			{
				intersect = new Point(x3, y3);
			}
			return true;
		}

		// Token: 0x060008F3 RID: 2291 RVA: 0x000BF954 File Offset: 0x000BDB54
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

		// Token: 0x04000A10 RID: 2576
		private IPredicates predicates = RobustPredicates.Default;

		// Token: 0x04000A11 RID: 2577
		private Mesh mesh;

		// Token: 0x04000A12 RID: 2578
		private Point[] points;

		// Token: 0x04000A13 RID: 2579
		private Dictionary<int, VoronoiRegion> regions;

		// Token: 0x04000A14 RID: 2580
		private Dictionary<int, Point> rayPoints;

		// Token: 0x04000A15 RID: 2581
		private int rayIndex;

		// Token: 0x04000A16 RID: 2582
		private Rectangle bounds;
	}
}
