using System;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.Meshing.Data
{
	// Token: 0x0200012D RID: 301
	internal class BadTriangle
	{
		// Token: 0x06000A7E RID: 2686 RVA: 0x00049E22 File Offset: 0x00048022
		public override string ToString()
		{
			return string.Format("B-TID {0}", this.poortri.tri.hash);
		}

		// Token: 0x04000ACD RID: 2765
		public Otri poortri;

		// Token: 0x04000ACE RID: 2766
		public double key;

		// Token: 0x04000ACF RID: 2767
		public Vertex org;

		// Token: 0x04000AD0 RID: 2768
		public Vertex dest;

		// Token: 0x04000AD1 RID: 2769
		public Vertex apex;

		// Token: 0x04000AD2 RID: 2770
		public BadTriangle next;
	}
}
