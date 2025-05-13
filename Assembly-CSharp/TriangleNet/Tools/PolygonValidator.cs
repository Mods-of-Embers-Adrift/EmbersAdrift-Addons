using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Logging;

namespace TriangleNet.Tools
{
	// Token: 0x0200010C RID: 268
	public static class PolygonValidator
	{
		// Token: 0x060009A4 RID: 2468 RVA: 0x000C16F0 File Offset: 0x000BF8F0
		public static bool IsConsistent(IPolygon poly)
		{
			ILog<LogItem> instance = Log.Instance;
			List<Vertex> points = poly.Points;
			int num = 0;
			int num2 = 0;
			int count = points.Count;
			if (count < 3)
			{
				instance.Warning("Polygon must have at least 3 vertices.", "PolygonValidator.IsConsistent()");
				return false;
			}
			foreach (Vertex vertex in points)
			{
				if (vertex == null)
				{
					num++;
					instance.Warning(string.Format("Point {0} is null.", num2), "PolygonValidator.IsConsistent()");
				}
				else if (double.IsNaN(vertex.x) || double.IsNaN(vertex.y))
				{
					num++;
					instance.Warning(string.Format("Point {0} has invalid coordinates.", num2), "PolygonValidator.IsConsistent()");
				}
				else if (double.IsInfinity(vertex.x) || double.IsInfinity(vertex.y))
				{
					num++;
					instance.Warning(string.Format("Point {0} has invalid coordinates.", num2), "PolygonValidator.IsConsistent()");
				}
				num2++;
			}
			num2 = 0;
			foreach (ISegment segment in poly.Segments)
			{
				if (segment == null)
				{
					num++;
					instance.Warning(string.Format("Segment {0} is null.", num2), "PolygonValidator.IsConsistent()");
					return false;
				}
				Vertex vertex2 = segment.GetVertex(0);
				Vertex vertex3 = segment.GetVertex(1);
				if (vertex2.x == vertex3.x && vertex2.y == vertex3.y)
				{
					num++;
					instance.Warning(string.Format("Endpoints of segment {0} are coincident (IDs {1} / {2}).", num2, vertex2.id, vertex3.id), "PolygonValidator.IsConsistent()");
				}
				num2++;
			}
			if (points[0].id == points[1].id)
			{
				num += PolygonValidator.CheckVertexIDs(poly, count);
			}
			else
			{
				num += PolygonValidator.CheckDuplicateIDs(poly);
			}
			return num == 0;
		}

		// Token: 0x060009A5 RID: 2469 RVA: 0x000C192C File Offset: 0x000BFB2C
		public static bool HasDuplicateVertices(IPolygon poly)
		{
			ILog<LogItem> instance = Log.Instance;
			int num = 0;
			Vertex[] array = poly.Points.ToArray();
			VertexSorter.Sort(array, 57113);
			for (int i = 1; i < array.Length; i++)
			{
				if (array[i - 1] == array[i])
				{
					num++;
					instance.Warning(string.Format("Found duplicate point {0}.", array[i]), "PolygonValidator.HasDuplicateVertices()");
				}
			}
			return num > 0;
		}

		// Token: 0x060009A6 RID: 2470 RVA: 0x000C1998 File Offset: 0x000BFB98
		public static bool HasBadAngles(IPolygon poly, double threshold = 2E-12)
		{
			ILog<LogItem> instance = Log.Instance;
			int num = 0;
			int num2 = 0;
			Point point = null;
			Point point2 = null;
			int count = poly.Points.Count;
			foreach (ISegment segment in poly.Segments)
			{
				Point a = point;
				Point point3 = point2;
				point = segment.GetVertex(0);
				point2 = segment.GetVertex(1);
				if (!(point == point2) && !(a == point3))
				{
					if (a != null && point3 != null && point == point3 && point2 != null && PolygonValidator.IsBadAngle(a, point, point2, threshold))
					{
						num++;
						instance.Warning(string.Format("Bad segment angle found at index {0}.", num2), "PolygonValidator.HasBadAngles()");
					}
					num2++;
				}
			}
			return num > 0;
		}

		// Token: 0x060009A7 RID: 2471 RVA: 0x000C1A90 File Offset: 0x000BFC90
		private static bool IsBadAngle(Point a, Point b, Point c, double threshold = 0.0)
		{
			double x = PolygonValidator.DotProduct(a, b, c);
			return Math.Abs(Math.Atan2(PolygonValidator.CrossProductLength(a, b, c), x)) <= threshold;
		}

		// Token: 0x060009A8 RID: 2472 RVA: 0x000496C6 File Offset: 0x000478C6
		private static double DotProduct(Point a, Point b, Point c)
		{
			return (a.x - b.x) * (c.x - b.x) + (a.y - b.y) * (c.y - b.y);
		}

		// Token: 0x060009A9 RID: 2473 RVA: 0x000496FF File Offset: 0x000478FF
		private static double CrossProductLength(Point a, Point b, Point c)
		{
			return (a.x - b.x) * (c.y - b.y) - (a.y - b.y) * (c.x - b.x);
		}

		// Token: 0x060009AA RID: 2474 RVA: 0x000C1AC0 File Offset: 0x000BFCC0
		private static int CheckVertexIDs(IPolygon poly, int count)
		{
			ILog<LogItem> instance = Log.Instance;
			int num = 0;
			int num2 = 0;
			foreach (ISegment segment in poly.Segments)
			{
				Vertex vertex = segment.GetVertex(0);
				Vertex vertex2 = segment.GetVertex(1);
				if (vertex.id < 0 || vertex.id >= count)
				{
					num++;
					instance.Warning(string.Format("Segment {0} has invalid startpoint.", num2), "PolygonValidator.IsConsistent()");
				}
				if (vertex2.id < 0 || vertex2.id >= count)
				{
					num++;
					instance.Warning(string.Format("Segment {0} has invalid endpoint.", num2), "PolygonValidator.IsConsistent()");
				}
				num2++;
			}
			return num;
		}

		// Token: 0x060009AB RID: 2475 RVA: 0x000C1B94 File Offset: 0x000BFD94
		private static int CheckDuplicateIDs(IPolygon poly)
		{
			HashSet<int> hashSet = new HashSet<int>();
			foreach (Vertex vertex in poly.Points)
			{
				if (!hashSet.Add(vertex.id))
				{
					Log.Instance.Warning("Found duplicate vertex ids.", "PolygonValidator.IsConsistent()");
					return 1;
				}
			}
			return 0;
		}
	}
}
