using System;

namespace TriangleNet.Geometry
{
	// Token: 0x02000149 RID: 329
	public interface ITriangle
	{
		// Token: 0x1700034D RID: 845
		// (get) Token: 0x06000B2F RID: 2863
		// (set) Token: 0x06000B30 RID: 2864
		int ID { get; set; }

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x06000B31 RID: 2865
		// (set) Token: 0x06000B32 RID: 2866
		int Label { get; set; }

		// Token: 0x1700034F RID: 847
		// (get) Token: 0x06000B33 RID: 2867
		// (set) Token: 0x06000B34 RID: 2868
		double Area { get; set; }

		// Token: 0x06000B35 RID: 2869
		Vertex GetVertex(int index);

		// Token: 0x06000B36 RID: 2870
		int GetVertexID(int index);

		// Token: 0x06000B37 RID: 2871
		ITriangle GetNeighbor(int index);

		// Token: 0x06000B38 RID: 2872
		int GetNeighborID(int index);

		// Token: 0x06000B39 RID: 2873
		ISegment GetSegment(int index);
	}
}
