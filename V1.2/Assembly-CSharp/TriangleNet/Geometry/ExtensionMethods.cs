using System;
using TriangleNet.Meshing;

namespace TriangleNet.Geometry
{
	// Token: 0x02000145 RID: 325
	public static class ExtensionMethods
	{
		// Token: 0x06000B11 RID: 2833 RVA: 0x0004A179 File Offset: 0x00048379
		public static IMesh Triangulate(this IPolygon polygon)
		{
			return new GenericMesher().Triangulate(polygon, null, null);
		}

		// Token: 0x06000B12 RID: 2834 RVA: 0x0004A188 File Offset: 0x00048388
		public static IMesh Triangulate(this IPolygon polygon, ConstraintOptions options)
		{
			return new GenericMesher().Triangulate(polygon, options, null);
		}

		// Token: 0x06000B13 RID: 2835 RVA: 0x0004A197 File Offset: 0x00048397
		public static IMesh Triangulate(this IPolygon polygon, QualityOptions quality)
		{
			return new GenericMesher().Triangulate(polygon, null, quality);
		}

		// Token: 0x06000B14 RID: 2836 RVA: 0x0004A1A6 File Offset: 0x000483A6
		public static IMesh Triangulate(this IPolygon polygon, ConstraintOptions options, QualityOptions quality)
		{
			return new GenericMesher().Triangulate(polygon, options, quality);
		}

		// Token: 0x06000B15 RID: 2837 RVA: 0x0004A1B5 File Offset: 0x000483B5
		public static IMesh Triangulate(this IPolygon polygon, ConstraintOptions options, QualityOptions quality, ITriangulator triangulator)
		{
			return new GenericMesher(triangulator).Triangulate(polygon, options, quality);
		}

		// Token: 0x06000B16 RID: 2838 RVA: 0x0004A1C5 File Offset: 0x000483C5
		public static bool Contains(this ITriangle triangle, Point p)
		{
			return triangle.Contains(p.X, p.Y);
		}

		// Token: 0x06000B17 RID: 2839 RVA: 0x000CC1F8 File Offset: 0x000CA3F8
		public static bool Contains(this ITriangle triangle, double x, double y)
		{
			Vertex vertex = triangle.GetVertex(0);
			Vertex vertex2 = triangle.GetVertex(1);
			Vertex vertex3 = triangle.GetVertex(2);
			Point point = new Point(vertex2.X - vertex.X, vertex2.Y - vertex.Y);
			Point point2 = new Point(vertex3.X - vertex.X, vertex3.Y - vertex.Y);
			Point p = new Point(x - vertex.X, y - vertex.Y);
			Point q = new Point(-point.Y, point.X);
			Point q2 = new Point(-point2.Y, point2.X);
			double num = ExtensionMethods.DotProduct(p, q2) / ExtensionMethods.DotProduct(point, q2);
			double num2 = ExtensionMethods.DotProduct(p, q) / ExtensionMethods.DotProduct(point2, q);
			return num >= 0.0 && num2 >= 0.0 && num + num2 <= 1.0;
		}

		// Token: 0x06000B18 RID: 2840 RVA: 0x000CC2F0 File Offset: 0x000CA4F0
		public static Rectangle Bounds(this ITriangle triangle)
		{
			Rectangle rectangle = new Rectangle();
			for (int i = 0; i < 3; i++)
			{
				rectangle.Expand(triangle.GetVertex(i));
			}
			return rectangle;
		}

		// Token: 0x06000B19 RID: 2841 RVA: 0x0004A1D9 File Offset: 0x000483D9
		internal static double DotProduct(Point p, Point q)
		{
			return p.X * q.X + p.Y * q.Y;
		}
	}
}
