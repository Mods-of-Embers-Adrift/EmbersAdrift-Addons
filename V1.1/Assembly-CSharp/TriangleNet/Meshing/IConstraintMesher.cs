using System;
using TriangleNet.Geometry;

namespace TriangleNet.Meshing
{
	// Token: 0x0200011E RID: 286
	public interface IConstraintMesher
	{
		// Token: 0x06000A30 RID: 2608
		IMesh Triangulate(IPolygon polygon);

		// Token: 0x06000A31 RID: 2609
		IMesh Triangulate(IPolygon polygon, ConstraintOptions options);
	}
}
