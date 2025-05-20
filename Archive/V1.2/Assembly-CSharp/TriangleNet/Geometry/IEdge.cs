using System;

namespace TriangleNet.Geometry
{
	// Token: 0x02000146 RID: 326
	public interface IEdge
	{
		// Token: 0x17000344 RID: 836
		// (get) Token: 0x06000B1A RID: 2842
		int P0 { get; }

		// Token: 0x17000345 RID: 837
		// (get) Token: 0x06000B1B RID: 2843
		int P1 { get; }

		// Token: 0x17000346 RID: 838
		// (get) Token: 0x06000B1C RID: 2844
		int Label { get; }
	}
}
