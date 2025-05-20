using System;
using System.Collections.Generic;

namespace TriangleNet.Geometry
{
	// Token: 0x02000142 RID: 322
	public class Contour
	{
		// Token: 0x17000340 RID: 832
		// (get) Token: 0x06000AFC RID: 2812 RVA: 0x0004A0CE File Offset: 0x000482CE
		// (set) Token: 0x06000AFD RID: 2813 RVA: 0x0004A0D6 File Offset: 0x000482D6
		public List<Vertex> Points { get; set; }

		// Token: 0x06000AFE RID: 2814 RVA: 0x0004A0DF File Offset: 0x000482DF
		public Contour(IEnumerable<Vertex> points) : this(points, 0, false)
		{
		}

		// Token: 0x06000AFF RID: 2815 RVA: 0x0004A0EA File Offset: 0x000482EA
		public Contour(IEnumerable<Vertex> points, int marker) : this(points, marker, false)
		{
		}

		// Token: 0x06000B00 RID: 2816 RVA: 0x0004A0F5 File Offset: 0x000482F5
		public Contour(IEnumerable<Vertex> points, int marker, bool convex)
		{
			this.AddPoints(points);
			this.marker = marker;
			this.convex = convex;
		}

		// Token: 0x06000B01 RID: 2817 RVA: 0x000CBDBC File Offset: 0x000C9FBC
		public List<ISegment> GetSegments()
		{
			List<ISegment> list = new List<ISegment>();
			List<Vertex> points = this.Points;
			int num = points.Count - 1;
			for (int i = 0; i < num; i++)
			{
				list.Add(new Segment(points[i], points[i + 1], this.marker));
			}
			list.Add(new Segment(points[num], points[0], this.marker));
			return list;
		}

		// Token: 0x06000B02 RID: 2818 RVA: 0x000CBE2C File Offset: 0x000CA02C
		public Point FindInteriorPoint(int limit = 5, double eps = 2E-05)
		{
			if (this.convex)
			{
				int count = this.Points.Count;
				Point point = new Point(0.0, 0.0);
				for (int i = 0; i < count; i++)
				{
					point.x += this.Points[i].x;
					point.y += this.Points[i].y;
				}
				point.x /= (double)count;
				point.y /= (double)count;
				return point;
			}
			return Contour.FindPointInPolygon(this.Points, limit, eps);
		}

		// Token: 0x06000B03 RID: 2819 RVA: 0x000CBEE0 File Offset: 0x000CA0E0
		private void AddPoints(IEnumerable<Vertex> points)
		{
			this.Points = new List<Vertex>(points);
			int index = this.Points.Count - 1;
			if (this.Points[0] == this.Points[index])
			{
				this.Points.RemoveAt(index);
			}
		}

		// Token: 0x06000B04 RID: 2820 RVA: 0x000CBF34 File Offset: 0x000CA134
		private static Point FindPointInPolygon(List<Vertex> contour, int limit, double eps)
		{
			List<Point> points = contour.ConvertAll<Point>((Vertex x) => x);
			Rectangle rectangle = new Rectangle();
			rectangle.Expand(points);
			int count = contour.Count;
			Point point = new Point();
			RobustPredicates robustPredicates = new RobustPredicates();
			Point point2 = contour[0];
			Point point3 = contour[1];
			for (int i = 0; i < count; i++)
			{
				Point point4 = contour[(i + 2) % count];
				double x2 = point3.x;
				double y = point3.y;
				double num = robustPredicates.CounterClockwise(point2, point3, point4);
				double num2;
				double num3;
				if (Math.Abs(num) < eps)
				{
					num2 = (point4.y - point2.y) / 2.0;
					num3 = (point2.x - point4.x) / 2.0;
				}
				else
				{
					num2 = (point2.x + point4.x) / 2.0 - x2;
					num3 = (point2.y + point4.y) / 2.0 - y;
				}
				point2 = point3;
				point3 = point4;
				num = 1.0;
				for (int j = 0; j < limit; j++)
				{
					point.x = x2 + num2 * num;
					point.y = y + num3 * num;
					if (rectangle.Contains(point) && Contour.IsPointInPolygon(point, contour))
					{
						return point;
					}
					point.x = x2 - num2 * num;
					point.y = y - num3 * num;
					if (rectangle.Contains(point) && Contour.IsPointInPolygon(point, contour))
					{
						return point;
					}
					num /= 2.0;
				}
			}
			throw new Exception();
		}

		// Token: 0x06000B05 RID: 2821 RVA: 0x000CC0F4 File Offset: 0x000CA2F4
		private static bool IsPointInPolygon(Point point, List<Vertex> poly)
		{
			bool flag = false;
			double x = point.x;
			double y = point.y;
			int count = poly.Count;
			int i = 0;
			int index = count - 1;
			while (i < count)
			{
				if (((poly[i].y < y && poly[index].y >= y) || (poly[index].y < y && poly[i].y >= y)) && (poly[i].x <= x || poly[index].x <= x))
				{
					flag ^= (poly[i].x + (y - poly[i].y) / (poly[index].y - poly[i].y) * (poly[index].x - poly[i].x) < x);
				}
				index = i;
				i++;
			}
			return flag;
		}

		// Token: 0x04000B05 RID: 2821
		private int marker;

		// Token: 0x04000B06 RID: 2822
		private bool convex;
	}
}
