using System;
using TriangleNet.Geometry;

namespace TriangleNet.IO
{
	// Token: 0x0200013D RID: 317
	public class InputTriangle : ITriangle
	{
		// Token: 0x06000AC8 RID: 2760 RVA: 0x00049FB8 File Offset: 0x000481B8
		public InputTriangle(int p0, int p1, int p2)
		{
			this.vertices = new int[]
			{
				p0,
				p1,
				p2
			};
		}

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x06000AC9 RID: 2761 RVA: 0x00045BCA File Offset: 0x00043DCA
		// (set) Token: 0x06000ACA RID: 2762 RVA: 0x0004475B File Offset: 0x0004295B
		public int ID
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		// Token: 0x1700033E RID: 830
		// (get) Token: 0x06000ACB RID: 2763 RVA: 0x00049FD8 File Offset: 0x000481D8
		// (set) Token: 0x06000ACC RID: 2764 RVA: 0x00049FE0 File Offset: 0x000481E0
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

		// Token: 0x1700033F RID: 831
		// (get) Token: 0x06000ACD RID: 2765 RVA: 0x00049FE9 File Offset: 0x000481E9
		// (set) Token: 0x06000ACE RID: 2766 RVA: 0x00049FF1 File Offset: 0x000481F1
		public double Area
		{
			get
			{
				return this.area;
			}
			set
			{
				this.area = value;
			}
		}

		// Token: 0x06000ACF RID: 2767 RVA: 0x00049FFA File Offset: 0x000481FA
		public Vertex GetVertex(int index)
		{
			return null;
		}

		// Token: 0x06000AD0 RID: 2768 RVA: 0x00049FFD File Offset: 0x000481FD
		public int GetVertexID(int index)
		{
			return this.vertices[index];
		}

		// Token: 0x06000AD1 RID: 2769 RVA: 0x00049FFA File Offset: 0x000481FA
		public ITriangle GetNeighbor(int index)
		{
			return null;
		}

		// Token: 0x06000AD2 RID: 2770 RVA: 0x00045BE1 File Offset: 0x00043DE1
		public int GetNeighborID(int index)
		{
			return -1;
		}

		// Token: 0x06000AD3 RID: 2771 RVA: 0x00049FFA File Offset: 0x000481FA
		public ISegment GetSegment(int index)
		{
			return null;
		}

		// Token: 0x04000AFF RID: 2815
		internal int[] vertices;

		// Token: 0x04000B00 RID: 2816
		internal int label;

		// Token: 0x04000B01 RID: 2817
		internal double area;
	}
}
