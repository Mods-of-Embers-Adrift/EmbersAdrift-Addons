using System;
using System.IO;
using TriangleNet.Geometry;

namespace TriangleNet.IO
{
	// Token: 0x0200013E RID: 318
	public interface IPolygonFormat : IFileFormat
	{
		// Token: 0x06000AD4 RID: 2772
		IPolygon Read(string filename);

		// Token: 0x06000AD5 RID: 2773
		void Write(IPolygon polygon, string filename);

		// Token: 0x06000AD6 RID: 2774
		void Write(IPolygon polygon, Stream stream);
	}
}
