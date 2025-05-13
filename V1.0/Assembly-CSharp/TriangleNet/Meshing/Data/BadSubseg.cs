using System;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.Meshing.Data
{
	// Token: 0x0200012C RID: 300
	internal class BadSubseg
	{
		// Token: 0x06000A7B RID: 2683 RVA: 0x00049DEF File Offset: 0x00047FEF
		public override int GetHashCode()
		{
			return this.subseg.seg.hash;
		}

		// Token: 0x06000A7C RID: 2684 RVA: 0x00049E01 File Offset: 0x00048001
		public override string ToString()
		{
			return string.Format("B-SID {0}", this.subseg.seg.hash);
		}

		// Token: 0x04000ACA RID: 2762
		public Osub subseg;

		// Token: 0x04000ACB RID: 2763
		public Vertex org;

		// Token: 0x04000ACC RID: 2764
		public Vertex dest;
	}
}
