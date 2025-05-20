using System;
using System.Collections.Generic;
using TriangleNet.Geometry;

namespace TriangleNet.Voronoi.Legacy
{
	// Token: 0x020000FD RID: 253
	public class VoronoiRegion
	{
		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x060008F4 RID: 2292 RVA: 0x00048CB1 File Offset: 0x00046EB1
		public int ID
		{
			get
			{
				return this.id;
			}
		}

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x060008F5 RID: 2293 RVA: 0x00048CB9 File Offset: 0x00046EB9
		public Point Generator
		{
			get
			{
				return this.generator;
			}
		}

		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x060008F6 RID: 2294 RVA: 0x00048CC1 File Offset: 0x00046EC1
		public ICollection<Point> Vertices
		{
			get
			{
				return this.vertices;
			}
		}

		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x060008F7 RID: 2295 RVA: 0x00048CC9 File Offset: 0x00046EC9
		// (set) Token: 0x060008F8 RID: 2296 RVA: 0x00048CD1 File Offset: 0x00046ED1
		public bool Bounded
		{
			get
			{
				return this.bounded;
			}
			set
			{
				this.bounded = value;
			}
		}

		// Token: 0x060008F9 RID: 2297 RVA: 0x00048CDA File Offset: 0x00046EDA
		public VoronoiRegion(Vertex generator)
		{
			this.id = generator.id;
			this.generator = generator;
			this.vertices = new List<Point>();
			this.bounded = true;
			this.neighbors = new Dictionary<int, VoronoiRegion>();
		}

		// Token: 0x060008FA RID: 2298 RVA: 0x00048D12 File Offset: 0x00046F12
		public void Add(Point point)
		{
			this.vertices.Add(point);
		}

		// Token: 0x060008FB RID: 2299 RVA: 0x00048D20 File Offset: 0x00046F20
		public void Add(List<Point> points)
		{
			this.vertices.AddRange(points);
		}

		// Token: 0x060008FC RID: 2300 RVA: 0x000BFA4C File Offset: 0x000BDC4C
		public VoronoiRegion GetNeighbor(Point p)
		{
			VoronoiRegion result;
			if (this.neighbors.TryGetValue(p.id, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x060008FD RID: 2301 RVA: 0x00048D2E File Offset: 0x00046F2E
		internal void AddNeighbor(int id, VoronoiRegion neighbor)
		{
			this.neighbors.Add(id, neighbor);
		}

		// Token: 0x060008FE RID: 2302 RVA: 0x00048D3D File Offset: 0x00046F3D
		public override string ToString()
		{
			return string.Format("R-ID {0}", this.id);
		}

		// Token: 0x04000A17 RID: 2583
		private int id;

		// Token: 0x04000A18 RID: 2584
		private Point generator;

		// Token: 0x04000A19 RID: 2585
		private List<Point> vertices;

		// Token: 0x04000A1A RID: 2586
		private bool bounded;

		// Token: 0x04000A1B RID: 2587
		private Dictionary<int, VoronoiRegion> neighbors;
	}
}
