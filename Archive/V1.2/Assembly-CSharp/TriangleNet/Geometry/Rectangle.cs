using System;
using System.Collections.Generic;

namespace TriangleNet.Geometry
{
	// Token: 0x0200014D RID: 333
	public class Rectangle
	{
		// Token: 0x06000B65 RID: 2917 RVA: 0x000CC448 File Offset: 0x000CA648
		public Rectangle()
		{
			this.xmin = (this.ymin = double.MaxValue);
			this.xmax = (this.ymax = double.MinValue);
		}

		// Token: 0x06000B66 RID: 2918 RVA: 0x0004A473 File Offset: 0x00048673
		public Rectangle(Rectangle other) : this(other.Left, other.Bottom, other.Right, other.Top)
		{
		}

		// Token: 0x06000B67 RID: 2919 RVA: 0x0004A493 File Offset: 0x00048693
		public Rectangle(double x, double y, double width, double height)
		{
			this.xmin = x;
			this.ymin = y;
			this.xmax = x + width;
			this.ymax = y + height;
		}

		// Token: 0x1700035C RID: 860
		// (get) Token: 0x06000B68 RID: 2920 RVA: 0x0004A4BC File Offset: 0x000486BC
		public double Left
		{
			get
			{
				return this.xmin;
			}
		}

		// Token: 0x1700035D RID: 861
		// (get) Token: 0x06000B69 RID: 2921 RVA: 0x0004A4C4 File Offset: 0x000486C4
		public double Right
		{
			get
			{
				return this.xmax;
			}
		}

		// Token: 0x1700035E RID: 862
		// (get) Token: 0x06000B6A RID: 2922 RVA: 0x0004A4CC File Offset: 0x000486CC
		public double Bottom
		{
			get
			{
				return this.ymin;
			}
		}

		// Token: 0x1700035F RID: 863
		// (get) Token: 0x06000B6B RID: 2923 RVA: 0x0004A4D4 File Offset: 0x000486D4
		public double Top
		{
			get
			{
				return this.ymax;
			}
		}

		// Token: 0x17000360 RID: 864
		// (get) Token: 0x06000B6C RID: 2924 RVA: 0x0004A4DC File Offset: 0x000486DC
		public double Width
		{
			get
			{
				return this.xmax - this.xmin;
			}
		}

		// Token: 0x17000361 RID: 865
		// (get) Token: 0x06000B6D RID: 2925 RVA: 0x0004A4EB File Offset: 0x000486EB
		public double Height
		{
			get
			{
				return this.ymax - this.ymin;
			}
		}

		// Token: 0x06000B6E RID: 2926 RVA: 0x0004A4FA File Offset: 0x000486FA
		public void Resize(double dx, double dy)
		{
			this.xmin -= dx;
			this.xmax += dx;
			this.ymin -= dy;
			this.ymax += dy;
		}

		// Token: 0x06000B6F RID: 2927 RVA: 0x000CC48C File Offset: 0x000CA68C
		public void Expand(Point p)
		{
			this.xmin = Math.Min(this.xmin, p.x);
			this.ymin = Math.Min(this.ymin, p.y);
			this.xmax = Math.Max(this.xmax, p.x);
			this.ymax = Math.Max(this.ymax, p.y);
		}

		// Token: 0x06000B70 RID: 2928 RVA: 0x000CC4F8 File Offset: 0x000CA6F8
		public void Expand(IEnumerable<Point> points)
		{
			foreach (Point p in points)
			{
				this.Expand(p);
			}
		}

		// Token: 0x06000B71 RID: 2929 RVA: 0x000CC540 File Offset: 0x000CA740
		public void Expand(Rectangle other)
		{
			this.xmin = Math.Min(this.xmin, other.xmin);
			this.ymin = Math.Min(this.ymin, other.ymin);
			this.xmax = Math.Max(this.xmax, other.xmax);
			this.ymax = Math.Max(this.ymax, other.ymax);
		}

		// Token: 0x06000B72 RID: 2930 RVA: 0x0004A534 File Offset: 0x00048734
		public bool Contains(double x, double y)
		{
			return x >= this.xmin && x <= this.xmax && y >= this.ymin && y <= this.ymax;
		}

		// Token: 0x06000B73 RID: 2931 RVA: 0x0004A55F File Offset: 0x0004875F
		public bool Contains(Point pt)
		{
			return this.Contains(pt.x, pt.y);
		}

		// Token: 0x06000B74 RID: 2932 RVA: 0x0004A573 File Offset: 0x00048773
		public bool Contains(Rectangle other)
		{
			return this.xmin <= other.Left && other.Right <= this.xmax && this.ymin <= other.Bottom && other.Top <= this.ymax;
		}

		// Token: 0x06000B75 RID: 2933 RVA: 0x0004A5B2 File Offset: 0x000487B2
		public bool Intersects(Rectangle other)
		{
			return other.Left < this.xmax && this.xmin < other.Right && other.Bottom < this.ymax && this.ymin < other.Top;
		}

		// Token: 0x04000B1A RID: 2842
		private double xmin;

		// Token: 0x04000B1B RID: 2843
		private double ymin;

		// Token: 0x04000B1C RID: 2844
		private double xmax;

		// Token: 0x04000B1D RID: 2845
		private double ymax;
	}
}
