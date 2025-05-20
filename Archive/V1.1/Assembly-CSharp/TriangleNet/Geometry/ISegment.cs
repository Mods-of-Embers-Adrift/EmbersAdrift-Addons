using System;

namespace TriangleNet.Geometry
{
	// Token: 0x02000148 RID: 328
	public interface ISegment : IEdge
	{
		// Token: 0x06000B2D RID: 2861
		Vertex GetVertex(int index);

		// Token: 0x06000B2E RID: 2862
		ITriangle GetTriangle(int index);
	}
}
