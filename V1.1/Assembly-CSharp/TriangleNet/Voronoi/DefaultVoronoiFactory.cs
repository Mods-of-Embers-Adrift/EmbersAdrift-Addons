using System;
using TriangleNet.Geometry;
using TriangleNet.Topology.DCEL;

namespace TriangleNet.Voronoi
{
	// Token: 0x020000F6 RID: 246
	public class DefaultVoronoiFactory : IVoronoiFactory
	{
		// Token: 0x060008C6 RID: 2246 RVA: 0x0004475B File Offset: 0x0004295B
		public void Initialize(int vertexCount, int edgeCount, int faceCount)
		{
		}

		// Token: 0x060008C7 RID: 2247 RVA: 0x0004475B File Offset: 0x0004295B
		public void Reset()
		{
		}

		// Token: 0x060008C8 RID: 2248 RVA: 0x00048B9B File Offset: 0x00046D9B
		public TriangleNet.Topology.DCEL.Vertex CreateVertex(double x, double y)
		{
			return new TriangleNet.Topology.DCEL.Vertex(x, y);
		}

		// Token: 0x060008C9 RID: 2249 RVA: 0x00048BA4 File Offset: 0x00046DA4
		public HalfEdge CreateHalfEdge(TriangleNet.Topology.DCEL.Vertex origin, Face face)
		{
			return new HalfEdge(origin, face);
		}

		// Token: 0x060008CA RID: 2250 RVA: 0x00048BAD File Offset: 0x00046DAD
		public Face CreateFace(TriangleNet.Geometry.Vertex vertex)
		{
			return new Face(vertex);
		}
	}
}
