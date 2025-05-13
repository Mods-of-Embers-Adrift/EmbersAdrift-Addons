using System;
using TriangleNet.Geometry;

namespace TriangleNet
{
	// Token: 0x020000E9 RID: 233
	public interface IPredicates
	{
		// Token: 0x06000834 RID: 2100
		double CounterClockwise(Point a, Point b, Point c);

		// Token: 0x06000835 RID: 2101
		double InCircle(Point a, Point b, Point c, Point p);

		// Token: 0x06000836 RID: 2102
		Point FindCircumcenter(Point org, Point dest, Point apex, ref double xi, ref double eta);

		// Token: 0x06000837 RID: 2103
		Point FindCircumcenter(Point org, Point dest, Point apex, ref double xi, ref double eta, double offconstant);
	}
}
