using System;
using TriangleNet.Meshing;

namespace TriangleNet.Smoothing
{
	// Token: 0x02000115 RID: 277
	public interface ISmoother
	{
		// Token: 0x060009EC RID: 2540
		void Smooth(IMesh mesh);

		// Token: 0x060009ED RID: 2541
		void Smooth(IMesh mesh, int limit);
	}
}
