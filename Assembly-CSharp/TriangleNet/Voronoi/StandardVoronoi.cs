using System;
using TriangleNet.Geometry;
using TriangleNet.Tools;
using TriangleNet.Topology.DCEL;

namespace TriangleNet.Voronoi
{
	// Token: 0x020000F8 RID: 248
	public class StandardVoronoi : VoronoiBase
	{
		// Token: 0x060008D1 RID: 2257 RVA: 0x00048BB5 File Offset: 0x00046DB5
		public StandardVoronoi(Mesh mesh) : this(mesh, mesh.bounds, new DefaultVoronoiFactory(), RobustPredicates.Default)
		{
		}

		// Token: 0x060008D2 RID: 2258 RVA: 0x00048BCE File Offset: 0x00046DCE
		public StandardVoronoi(Mesh mesh, Rectangle box) : this(mesh, box, new DefaultVoronoiFactory(), RobustPredicates.Default)
		{
		}

		// Token: 0x060008D3 RID: 2259 RVA: 0x00048BE2 File Offset: 0x00046DE2
		public StandardVoronoi(Mesh mesh, Rectangle box, IVoronoiFactory factory, IPredicates predicates) : base(mesh, factory, predicates, true)
		{
			box.Expand(mesh.bounds);
			this.PostProcess(box);
		}

		// Token: 0x060008D4 RID: 2260 RVA: 0x000BDC00 File Offset: 0x000BBE00
		private void PostProcess(Rectangle box)
		{
			foreach (HalfEdge halfEdge in this.rays)
			{
				Point origin = halfEdge.origin;
				Point origin2 = halfEdge.twin.origin;
				if (box.Contains(origin) || box.Contains(origin2))
				{
					IntersectionHelper.BoxRayIntersection(box, origin, origin2, ref origin2);
				}
			}
		}
	}
}
