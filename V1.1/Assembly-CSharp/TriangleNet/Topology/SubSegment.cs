using System;
using TriangleNet.Geometry;

namespace TriangleNet.Topology
{
	// Token: 0x02000100 RID: 256
	public class SubSegment : ISegment, IEdge
	{
		// Token: 0x0600093D RID: 2365 RVA: 0x0004925D File Offset: 0x0004745D
		public SubSegment()
		{
			this.vertices = new Vertex[4];
			this.boundary = 0;
			this.subsegs = new Osub[2];
			this.triangles = new Otri[2];
		}

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x0600093E RID: 2366 RVA: 0x00049290 File Offset: 0x00047490
		public int P0
		{
			get
			{
				return this.vertices[0].id;
			}
		}

		// Token: 0x170002EA RID: 746
		// (get) Token: 0x0600093F RID: 2367 RVA: 0x0004929F File Offset: 0x0004749F
		public int P1
		{
			get
			{
				return this.vertices[1].id;
			}
		}

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x06000940 RID: 2368 RVA: 0x000492AE File Offset: 0x000474AE
		public int Label
		{
			get
			{
				return this.boundary;
			}
		}

		// Token: 0x06000941 RID: 2369 RVA: 0x000492B6 File Offset: 0x000474B6
		public Vertex GetVertex(int index)
		{
			return this.vertices[index];
		}

		// Token: 0x06000942 RID: 2370 RVA: 0x000492C0 File Offset: 0x000474C0
		public ITriangle GetTriangle(int index)
		{
			if (this.triangles[index].tri.hash != -1)
			{
				return this.triangles[index].tri;
			}
			return null;
		}

		// Token: 0x06000943 RID: 2371 RVA: 0x000492EE File Offset: 0x000474EE
		public override int GetHashCode()
		{
			return this.hash;
		}

		// Token: 0x06000944 RID: 2372 RVA: 0x000492F6 File Offset: 0x000474F6
		public override string ToString()
		{
			return string.Format("SID {0}", this.hash);
		}

		// Token: 0x04000A22 RID: 2594
		internal int hash;

		// Token: 0x04000A23 RID: 2595
		internal Osub[] subsegs;

		// Token: 0x04000A24 RID: 2596
		internal Vertex[] vertices;

		// Token: 0x04000A25 RID: 2597
		internal Otri[] triangles;

		// Token: 0x04000A26 RID: 2598
		internal int boundary;
	}
}
