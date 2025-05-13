using System;

namespace TriangleNet.Geometry
{
	// Token: 0x0200014F RID: 335
	public class Segment : ISegment, IEdge
	{
		// Token: 0x17000363 RID: 867
		// (get) Token: 0x06000B7A RID: 2938 RVA: 0x0004A64E File Offset: 0x0004884E
		// (set) Token: 0x06000B7B RID: 2939 RVA: 0x0004A656 File Offset: 0x00048856
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

		// Token: 0x17000364 RID: 868
		// (get) Token: 0x06000B7C RID: 2940 RVA: 0x0004A65F File Offset: 0x0004885F
		public int P0
		{
			get
			{
				return this.v0.id;
			}
		}

		// Token: 0x17000365 RID: 869
		// (get) Token: 0x06000B7D RID: 2941 RVA: 0x0004A66C File Offset: 0x0004886C
		public int P1
		{
			get
			{
				return this.v1.id;
			}
		}

		// Token: 0x06000B7E RID: 2942 RVA: 0x0004A679 File Offset: 0x00048879
		public Segment(Vertex v0, Vertex v1) : this(v0, v1, 0)
		{
		}

		// Token: 0x06000B7F RID: 2943 RVA: 0x0004A684 File Offset: 0x00048884
		public Segment(Vertex v0, Vertex v1, int label)
		{
			this.v0 = v0;
			this.v1 = v1;
			this.label = label;
		}

		// Token: 0x06000B80 RID: 2944 RVA: 0x0004A6A1 File Offset: 0x000488A1
		public Vertex GetVertex(int index)
		{
			if (index == 0)
			{
				return this.v0;
			}
			if (index == 1)
			{
				return this.v1;
			}
			throw new IndexOutOfRangeException();
		}

		// Token: 0x06000B81 RID: 2945 RVA: 0x00048A92 File Offset: 0x00046C92
		public ITriangle GetTriangle(int index)
		{
			throw new NotImplementedException();
		}

		// Token: 0x04000B21 RID: 2849
		private Vertex v0;

		// Token: 0x04000B22 RID: 2850
		private Vertex v1;

		// Token: 0x04000B23 RID: 2851
		private int label;
	}
}
