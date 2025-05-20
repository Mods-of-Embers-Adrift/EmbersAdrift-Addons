using System;

namespace TriangleNet.Meshing
{
	// Token: 0x0200011B RID: 283
	public class ConstraintOptions
	{
		// Token: 0x1700031B RID: 795
		// (get) Token: 0x06000A17 RID: 2583 RVA: 0x00049B5E File Offset: 0x00047D5E
		// (set) Token: 0x06000A18 RID: 2584 RVA: 0x00049B66 File Offset: 0x00047D66
		[Obsolete("Not used anywhere, will be removed in beta 4.")]
		public bool UseRegions { get; set; }

		// Token: 0x1700031C RID: 796
		// (get) Token: 0x06000A19 RID: 2585 RVA: 0x00049B6F File Offset: 0x00047D6F
		// (set) Token: 0x06000A1A RID: 2586 RVA: 0x00049B77 File Offset: 0x00047D77
		public bool ConformingDelaunay { get; set; }

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x06000A1B RID: 2587 RVA: 0x00049B80 File Offset: 0x00047D80
		// (set) Token: 0x06000A1C RID: 2588 RVA: 0x00049B88 File Offset: 0x00047D88
		public bool Convex { get; set; }

		// Token: 0x1700031E RID: 798
		// (get) Token: 0x06000A1D RID: 2589 RVA: 0x00049B91 File Offset: 0x00047D91
		// (set) Token: 0x06000A1E RID: 2590 RVA: 0x00049B99 File Offset: 0x00047D99
		public int SegmentSplitting { get; set; }
	}
}
