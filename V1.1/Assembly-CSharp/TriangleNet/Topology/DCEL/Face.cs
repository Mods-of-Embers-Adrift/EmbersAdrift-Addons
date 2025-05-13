using System;
using System.Collections.Generic;
using TriangleNet.Geometry;

namespace TriangleNet.Topology.DCEL
{
	// Token: 0x02000103 RID: 259
	public class Face
	{
		// Token: 0x0600095C RID: 2396 RVA: 0x00049488 File Offset: 0x00047688
		static Face()
		{
			Face.Empty.id = -1;
		}

		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x0600095D RID: 2397 RVA: 0x000494A0 File Offset: 0x000476A0
		// (set) Token: 0x0600095E RID: 2398 RVA: 0x000494A8 File Offset: 0x000476A8
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

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x0600095F RID: 2399 RVA: 0x000494B1 File Offset: 0x000476B1
		// (set) Token: 0x06000960 RID: 2400 RVA: 0x000494B9 File Offset: 0x000476B9
		public HalfEdge Edge
		{
			get
			{
				return this.edge;
			}
			set
			{
				this.edge = value;
			}
		}

		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x06000961 RID: 2401 RVA: 0x000494C2 File Offset: 0x000476C2
		// (set) Token: 0x06000962 RID: 2402 RVA: 0x000494CA File Offset: 0x000476CA
		public bool Bounded
		{
			get
			{
				return this.bounded;
			}
			set
			{
				this.bounded = value;
			}
		}

		// Token: 0x06000963 RID: 2403 RVA: 0x000494D3 File Offset: 0x000476D3
		public Face(Point generator) : this(generator, null)
		{
		}

		// Token: 0x06000964 RID: 2404 RVA: 0x000494DD File Offset: 0x000476DD
		public Face(Point generator, HalfEdge edge)
		{
			this.generator = generator;
			this.edge = edge;
			this.bounded = true;
			if (generator != null)
			{
				this.id = generator.ID;
			}
		}

		// Token: 0x06000965 RID: 2405 RVA: 0x0004950F File Offset: 0x0004770F
		public IEnumerable<HalfEdge> EnumerateEdges()
		{
			HalfEdge edge = this.Edge;
			int first = edge.ID;
			do
			{
				yield return edge;
				edge = edge.Next;
			}
			while (edge.ID != first);
			yield break;
		}

		// Token: 0x06000966 RID: 2406 RVA: 0x0004951F File Offset: 0x0004771F
		public override string ToString()
		{
			return string.Format("F-ID {0}", this.id);
		}

		// Token: 0x04000A32 RID: 2610
		public static readonly Face Empty = new Face(null);

		// Token: 0x04000A33 RID: 2611
		internal int id;

		// Token: 0x04000A34 RID: 2612
		internal Point generator;

		// Token: 0x04000A35 RID: 2613
		internal HalfEdge edge;

		// Token: 0x04000A36 RID: 2614
		internal bool bounded;
	}
}
