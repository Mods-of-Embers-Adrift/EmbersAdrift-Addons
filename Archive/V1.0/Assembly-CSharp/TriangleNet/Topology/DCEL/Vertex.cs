using System;
using System.Collections.Generic;
using TriangleNet.Geometry;

namespace TriangleNet.Topology.DCEL
{
	// Token: 0x02000106 RID: 262
	public class Vertex : Point
	{
		// Token: 0x170002FE RID: 766
		// (get) Token: 0x0600097E RID: 2430 RVA: 0x00049624 File Offset: 0x00047824
		// (set) Token: 0x0600097F RID: 2431 RVA: 0x0004962C File Offset: 0x0004782C
		public HalfEdge Leaving
		{
			get
			{
				return this.leaving;
			}
			set
			{
				this.leaving = value;
			}
		}

		// Token: 0x06000980 RID: 2432 RVA: 0x00049635 File Offset: 0x00047835
		public Vertex(double x, double y) : base(x, y)
		{
		}

		// Token: 0x06000981 RID: 2433 RVA: 0x0004963F File Offset: 0x0004783F
		public Vertex(double x, double y, HalfEdge leaving) : base(x, y)
		{
			this.leaving = leaving;
		}

		// Token: 0x06000982 RID: 2434 RVA: 0x00049650 File Offset: 0x00047850
		public IEnumerable<HalfEdge> EnumerateEdges()
		{
			HalfEdge edge = this.Leaving;
			int first = edge.ID;
			do
			{
				yield return edge;
				edge = edge.Twin.Next;
			}
			while (edge.ID != first);
			yield break;
		}

		// Token: 0x06000983 RID: 2435 RVA: 0x00049660 File Offset: 0x00047860
		public override string ToString()
		{
			return string.Format("V-ID {0}", this.id);
		}

		// Token: 0x04000A43 RID: 2627
		internal HalfEdge leaving;
	}
}
