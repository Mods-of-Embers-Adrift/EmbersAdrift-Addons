using System;

namespace TriangleNet.Geometry
{
	// Token: 0x02000144 RID: 324
	public class Edge : IEdge
	{
		// Token: 0x17000341 RID: 833
		// (get) Token: 0x06000B09 RID: 2825 RVA: 0x0004A11E File Offset: 0x0004831E
		// (set) Token: 0x06000B0A RID: 2826 RVA: 0x0004A126 File Offset: 0x00048326
		public int P0 { get; private set; }

		// Token: 0x17000342 RID: 834
		// (get) Token: 0x06000B0B RID: 2827 RVA: 0x0004A12F File Offset: 0x0004832F
		// (set) Token: 0x06000B0C RID: 2828 RVA: 0x0004A137 File Offset: 0x00048337
		public int P1 { get; private set; }

		// Token: 0x17000343 RID: 835
		// (get) Token: 0x06000B0D RID: 2829 RVA: 0x0004A140 File Offset: 0x00048340
		// (set) Token: 0x06000B0E RID: 2830 RVA: 0x0004A148 File Offset: 0x00048348
		public int Label { get; private set; }

		// Token: 0x06000B0F RID: 2831 RVA: 0x0004A151 File Offset: 0x00048351
		public Edge(int p0, int p1) : this(p0, p1, 0)
		{
		}

		// Token: 0x06000B10 RID: 2832 RVA: 0x0004A15C File Offset: 0x0004835C
		public Edge(int p0, int p1, int label)
		{
			this.P0 = p0;
			this.P1 = p1;
			this.Label = label;
		}
	}
}
