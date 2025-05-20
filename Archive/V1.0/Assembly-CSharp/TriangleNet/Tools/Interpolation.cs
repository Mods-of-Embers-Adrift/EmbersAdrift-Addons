using System;
using TriangleNet.Geometry;

namespace TriangleNet.Tools
{
	// Token: 0x0200010A RID: 266
	public static class Interpolation
	{
		// Token: 0x060009A0 RID: 2464 RVA: 0x000C1294 File Offset: 0x000BF494
		public static void InterpolateZ(Point p, ITriangle triangle)
		{
			Vertex vertex = triangle.GetVertex(0);
			Vertex vertex2 = triangle.GetVertex(1);
			Vertex vertex3 = triangle.GetVertex(2);
			double num = vertex2.x - vertex.x;
			double num2 = vertex2.y - vertex.y;
			double num3 = vertex3.x - vertex.x;
			double num4 = vertex3.y - vertex.y;
			double num5 = 0.5 / (num * num4 - num3 * num2);
			double num6 = p.x - vertex.x;
			double num7 = p.y - vertex.y;
			double num8 = (num4 * num6 - num3 * num7) * (2.0 * num5);
			double num9 = (num * num7 - num2 * num6) * (2.0 * num5);
			p.z = vertex.z + num8 * (vertex2.z - vertex.z) + num9 * (vertex3.z - vertex.z);
		}
	}
}
