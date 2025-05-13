using System;
using TriangleNet.Geometry;

namespace TriangleNet.Topology
{
	// Token: 0x02000101 RID: 257
	public class Triangle : ITriangle
	{
		// Token: 0x06000945 RID: 2373 RVA: 0x0004930D File Offset: 0x0004750D
		public Triangle()
		{
			this.vertices = new Vertex[3];
			this.subsegs = new Osub[3];
			this.neighbors = new Otri[3];
		}

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x06000946 RID: 2374 RVA: 0x00049339 File Offset: 0x00047539
		// (set) Token: 0x06000947 RID: 2375 RVA: 0x00049341 File Offset: 0x00047541
		public int ID
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		// Token: 0x170002ED RID: 749
		// (get) Token: 0x06000948 RID: 2376 RVA: 0x0004934A File Offset: 0x0004754A
		// (set) Token: 0x06000949 RID: 2377 RVA: 0x00049352 File Offset: 0x00047552
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

		// Token: 0x170002EE RID: 750
		// (get) Token: 0x0600094A RID: 2378 RVA: 0x0004935B File Offset: 0x0004755B
		// (set) Token: 0x0600094B RID: 2379 RVA: 0x00049363 File Offset: 0x00047563
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

		// Token: 0x0600094C RID: 2380 RVA: 0x0004936C File Offset: 0x0004756C
		public Vertex GetVertex(int index)
		{
			return this.vertices[index];
		}

		// Token: 0x0600094D RID: 2381 RVA: 0x00049376 File Offset: 0x00047576
		public int GetVertexID(int index)
		{
			return this.vertices[index].id;
		}

		// Token: 0x0600094E RID: 2382 RVA: 0x00049385 File Offset: 0x00047585
		public ITriangle GetNeighbor(int index)
		{
			if (this.neighbors[index].tri.hash != -1)
			{
				return this.neighbors[index].tri;
			}
			return null;
		}

		// Token: 0x0600094F RID: 2383 RVA: 0x000493B3 File Offset: 0x000475B3
		public int GetNeighborID(int index)
		{
			if (this.neighbors[index].tri.hash != -1)
			{
				return this.neighbors[index].tri.id;
			}
			return -1;
		}

		// Token: 0x06000950 RID: 2384 RVA: 0x000493E6 File Offset: 0x000475E6
		public ISegment GetSegment(int index)
		{
			if (this.subsegs[index].seg.hash != -1)
			{
				return this.subsegs[index].seg;
			}
			return null;
		}

		// Token: 0x06000951 RID: 2385 RVA: 0x00049414 File Offset: 0x00047614
		public override int GetHashCode()
		{
			return this.hash;
		}

		// Token: 0x06000952 RID: 2386 RVA: 0x0004941C File Offset: 0x0004761C
		public override string ToString()
		{
			return string.Format("TID {0}", this.hash);
		}

		// Token: 0x04000A27 RID: 2599
		internal int hash;

		// Token: 0x04000A28 RID: 2600
		internal int id;

		// Token: 0x04000A29 RID: 2601
		internal Otri[] neighbors;

		// Token: 0x04000A2A RID: 2602
		internal Vertex[] vertices;

		// Token: 0x04000A2B RID: 2603
		internal Osub[] subsegs;

		// Token: 0x04000A2C RID: 2604
		internal int label;

		// Token: 0x04000A2D RID: 2605
		internal double area;

		// Token: 0x04000A2E RID: 2606
		internal bool infected;
	}
}
