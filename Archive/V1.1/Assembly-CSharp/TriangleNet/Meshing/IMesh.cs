using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.Meshing
{
	// Token: 0x0200011F RID: 287
	public interface IMesh
	{
		// Token: 0x1700031F RID: 799
		// (get) Token: 0x06000A32 RID: 2610
		ICollection<Vertex> Vertices { get; }

		// Token: 0x17000320 RID: 800
		// (get) Token: 0x06000A33 RID: 2611
		IEnumerable<Edge> Edges { get; }

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x06000A34 RID: 2612
		ICollection<SubSegment> Segments { get; }

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x06000A35 RID: 2613
		ICollection<Triangle> Triangles { get; }

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x06000A36 RID: 2614
		IList<Point> Holes { get; }

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x06000A37 RID: 2615
		Rectangle Bounds { get; }

		// Token: 0x06000A38 RID: 2616
		void Renumber();

		// Token: 0x06000A39 RID: 2617
		void Refine(QualityOptions quality, bool delaunay);
	}
}
