using System;

namespace TriangleNet.Geometry
{
	// Token: 0x0200014A RID: 330
	public class Point : IComparable<Point>, IEquatable<Point>
	{
		// Token: 0x06000B3A RID: 2874 RVA: 0x0004A1F6 File Offset: 0x000483F6
		public Point() : this(0.0, 0.0, 0)
		{
		}

		// Token: 0x06000B3B RID: 2875 RVA: 0x0004A211 File Offset: 0x00048411
		public Point(double x, double y) : this(x, y, 0)
		{
		}

		// Token: 0x06000B3C RID: 2876 RVA: 0x0004A21C File Offset: 0x0004841C
		public Point(double x, double y, int label)
		{
			this.x = x;
			this.y = y;
			this.label = label;
		}

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x06000B3D RID: 2877 RVA: 0x0004A239 File Offset: 0x00048439
		// (set) Token: 0x06000B3E RID: 2878 RVA: 0x0004A241 File Offset: 0x00048441
		public int ID
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x06000B3F RID: 2879 RVA: 0x0004A24A File Offset: 0x0004844A
		// (set) Token: 0x06000B40 RID: 2880 RVA: 0x0004A252 File Offset: 0x00048452
		public double X
		{
			get
			{
				return this.x;
			}
			set
			{
				this.x = value;
			}
		}

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x06000B41 RID: 2881 RVA: 0x0004A25B File Offset: 0x0004845B
		// (set) Token: 0x06000B42 RID: 2882 RVA: 0x0004A263 File Offset: 0x00048463
		public double Y
		{
			get
			{
				return this.y;
			}
			set
			{
				this.y = value;
			}
		}

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x06000B43 RID: 2883 RVA: 0x0004A26C File Offset: 0x0004846C
		// (set) Token: 0x06000B44 RID: 2884 RVA: 0x0004A274 File Offset: 0x00048474
		public double Z
		{
			get
			{
				return this.z;
			}
			set
			{
				this.z = value;
			}
		}

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x06000B45 RID: 2885 RVA: 0x0004A27D File Offset: 0x0004847D
		// (set) Token: 0x06000B46 RID: 2886 RVA: 0x0004A285 File Offset: 0x00048485
		public int Label
		{
			get
			{
				return this.label;
			}
			set
			{
				this.label = value;
			}
		}

		// Token: 0x06000B47 RID: 2887 RVA: 0x0004A28E File Offset: 0x0004848E
		public static bool operator ==(Point a, Point b)
		{
			return a == b || (a != null && b != null && a.Equals(b));
		}

		// Token: 0x06000B48 RID: 2888 RVA: 0x0004A2A5 File Offset: 0x000484A5
		public static bool operator !=(Point a, Point b)
		{
			return !(a == b);
		}

		// Token: 0x06000B49 RID: 2889 RVA: 0x000CC320 File Offset: 0x000CA520
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			Point point = obj as Point;
			return point != null && this.x == point.x && this.y == point.y;
		}

		// Token: 0x06000B4A RID: 2890 RVA: 0x0004A2B1 File Offset: 0x000484B1
		public bool Equals(Point p)
		{
			return p != null && this.x == p.x && this.y == p.y;
		}

		// Token: 0x06000B4B RID: 2891 RVA: 0x000CC35C File Offset: 0x000CA55C
		public int CompareTo(Point other)
		{
			if (this.x == other.x && this.y == other.y)
			{
				return 0;
			}
			if (this.x >= other.x && (this.x != other.x || this.y >= other.y))
			{
				return 1;
			}
			return -1;
		}

		// Token: 0x06000B4C RID: 2892 RVA: 0x0004A2D6 File Offset: 0x000484D6
		public override int GetHashCode()
		{
			return (19 * 31 + this.x.GetHashCode()) * 31 + this.y.GetHashCode();
		}

		// Token: 0x06000B4D RID: 2893 RVA: 0x0004A2F8 File Offset: 0x000484F8
		public override string ToString()
		{
			return string.Format("[{0},{1}]", this.x, this.y);
		}

		// Token: 0x04000B0D RID: 2829
		internal int id;

		// Token: 0x04000B0E RID: 2830
		internal int label;

		// Token: 0x04000B0F RID: 2831
		internal double x;

		// Token: 0x04000B10 RID: 2832
		internal double y;

		// Token: 0x04000B11 RID: 2833
		internal double z;
	}
}
