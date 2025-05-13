using System;
using System.Collections.Generic;
using TriangleNet.Geometry;

namespace TriangleNet.Voronoi.Legacy
{
	// Token: 0x020000FB RID: 251
	public interface IVoronoi
	{
		// Token: 0x170002DD RID: 733
		// (get) Token: 0x060008E8 RID: 2280
		Point[] Points { get; }

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x060008E9 RID: 2281
		ICollection<VoronoiRegion> Regions { get; }

		// Token: 0x170002DF RID: 735
		// (get) Token: 0x060008EA RID: 2282
		IEnumerable<IEdge> Edges { get; }
	}
}
