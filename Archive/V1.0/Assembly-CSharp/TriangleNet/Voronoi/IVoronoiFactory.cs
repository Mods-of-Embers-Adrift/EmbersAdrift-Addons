using System;
using TriangleNet.Geometry;
using TriangleNet.Topology.DCEL;

namespace TriangleNet.Voronoi
{
	// Token: 0x020000F7 RID: 247
	public interface IVoronoiFactory
	{
		// Token: 0x060008CC RID: 2252
		void Initialize(int vertexCount, int edgeCount, int faceCount);

		// Token: 0x060008CD RID: 2253
		void Reset();

		// Token: 0x060008CE RID: 2254
		TriangleNet.Topology.DCEL.Vertex CreateVertex(double x, double y);

		// Token: 0x060008CF RID: 2255
		HalfEdge CreateHalfEdge(TriangleNet.Topology.DCEL.Vertex origin, Face face);

		// Token: 0x060008D0 RID: 2256
		Face CreateFace(TriangleNet.Geometry.Vertex vertex);
	}
}
