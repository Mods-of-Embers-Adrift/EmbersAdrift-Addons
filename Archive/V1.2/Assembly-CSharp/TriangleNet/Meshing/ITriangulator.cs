using System;
using System.Collections.Generic;
using TriangleNet.Geometry;

namespace TriangleNet.Meshing
{
	// Token: 0x02000121 RID: 289
	public interface ITriangulator
	{
		// Token: 0x06000A3C RID: 2620
		IMesh Triangulate(IList<Vertex> points, Configuration config);
	}
}
