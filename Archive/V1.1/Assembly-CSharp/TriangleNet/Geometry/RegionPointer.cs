using System;

namespace TriangleNet.Geometry
{
	// Token: 0x0200014E RID: 334
	public class RegionPointer
	{
		// Token: 0x17000362 RID: 866
		// (get) Token: 0x06000B76 RID: 2934 RVA: 0x0004A5EE File Offset: 0x000487EE
		// (set) Token: 0x06000B77 RID: 2935 RVA: 0x0004A5F6 File Offset: 0x000487F6
		public double Area
		{
			get
			{
				return this.area;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException("Area constraints must not be negative.");
				}
				this.area = value;
			}
		}

		// Token: 0x06000B78 RID: 2936 RVA: 0x0004A616 File Offset: 0x00048816
		public RegionPointer(double x, double y, int id) : this(x, y, id, 0.0)
		{
		}

		// Token: 0x06000B79 RID: 2937 RVA: 0x0004A62A File Offset: 0x0004882A
		public RegionPointer(double x, double y, int id, double area)
		{
			this.point = new Point(x, y);
			this.id = id;
			this.area = area;
		}

		// Token: 0x04000B1E RID: 2846
		internal Point point;

		// Token: 0x04000B1F RID: 2847
		internal int id;

		// Token: 0x04000B20 RID: 2848
		internal double area;
	}
}
