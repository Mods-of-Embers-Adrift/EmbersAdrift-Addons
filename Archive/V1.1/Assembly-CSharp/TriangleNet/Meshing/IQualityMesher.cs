using System;
using TriangleNet.Geometry;

namespace TriangleNet.Meshing
{
	// Token: 0x02000120 RID: 288
	public interface IQualityMesher
	{
		// Token: 0x06000A3A RID: 2618
		IMesh Triangulate(IPolygon polygon, QualityOptions quality);

		// Token: 0x06000A3B RID: 2619
		IMesh Triangulate(IPolygon polygon, ConstraintOptions options, QualityOptions quality);
	}
}
