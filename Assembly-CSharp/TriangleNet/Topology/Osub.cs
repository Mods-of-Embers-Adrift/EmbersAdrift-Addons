using System;
using TriangleNet.Geometry;

namespace TriangleNet.Topology
{
	// Token: 0x020000FE RID: 254
	public struct Osub
	{
		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x060008FF RID: 2303 RVA: 0x00048D54 File Offset: 0x00046F54
		public SubSegment Segment
		{
			get
			{
				return this.seg;
			}
		}

		// Token: 0x06000900 RID: 2304 RVA: 0x00048D5C File Offset: 0x00046F5C
		public override string ToString()
		{
			if (this.seg == null)
			{
				return "O-TID [null]";
			}
			return string.Format("O-SID {0}", this.seg.hash);
		}

		// Token: 0x06000901 RID: 2305 RVA: 0x00048D86 File Offset: 0x00046F86
		public void Sym(ref Osub os)
		{
			os.seg = this.seg;
			os.orient = 1 - this.orient;
		}

		// Token: 0x06000902 RID: 2306 RVA: 0x00048DA2 File Offset: 0x00046FA2
		public void Sym()
		{
			this.orient = 1 - this.orient;
		}

		// Token: 0x06000903 RID: 2307 RVA: 0x00048DB2 File Offset: 0x00046FB2
		public void Pivot(ref Osub os)
		{
			os = this.seg.subsegs[this.orient];
		}

		// Token: 0x06000904 RID: 2308 RVA: 0x00048DD0 File Offset: 0x00046FD0
		internal void Pivot(ref Otri ot)
		{
			ot = this.seg.triangles[this.orient];
		}

		// Token: 0x06000905 RID: 2309 RVA: 0x00048DEE File Offset: 0x00046FEE
		public void Next(ref Osub ot)
		{
			ot = this.seg.subsegs[1 - this.orient];
		}

		// Token: 0x06000906 RID: 2310 RVA: 0x00048E0E File Offset: 0x0004700E
		public void Next()
		{
			this = this.seg.subsegs[1 - this.orient];
		}

		// Token: 0x06000907 RID: 2311 RVA: 0x00048E2E File Offset: 0x0004702E
		public Vertex Org()
		{
			return this.seg.vertices[this.orient];
		}

		// Token: 0x06000908 RID: 2312 RVA: 0x00048E42 File Offset: 0x00047042
		public Vertex Dest()
		{
			return this.seg.vertices[1 - this.orient];
		}

		// Token: 0x06000909 RID: 2313 RVA: 0x00048E58 File Offset: 0x00047058
		internal void SetOrg(Vertex vertex)
		{
			this.seg.vertices[this.orient] = vertex;
		}

		// Token: 0x0600090A RID: 2314 RVA: 0x00048E6D File Offset: 0x0004706D
		internal void SetDest(Vertex vertex)
		{
			this.seg.vertices[1 - this.orient] = vertex;
		}

		// Token: 0x0600090B RID: 2315 RVA: 0x00048E84 File Offset: 0x00047084
		internal Vertex SegOrg()
		{
			return this.seg.vertices[2 + this.orient];
		}

		// Token: 0x0600090C RID: 2316 RVA: 0x00048E9A File Offset: 0x0004709A
		internal Vertex SegDest()
		{
			return this.seg.vertices[3 - this.orient];
		}

		// Token: 0x0600090D RID: 2317 RVA: 0x00048EB0 File Offset: 0x000470B0
		internal void SetSegOrg(Vertex vertex)
		{
			this.seg.vertices[2 + this.orient] = vertex;
		}

		// Token: 0x0600090E RID: 2318 RVA: 0x00048EC7 File Offset: 0x000470C7
		internal void SetSegDest(Vertex vertex)
		{
			this.seg.vertices[3 - this.orient] = vertex;
		}

		// Token: 0x0600090F RID: 2319 RVA: 0x00048EDE File Offset: 0x000470DE
		internal void Bond(ref Osub os)
		{
			this.seg.subsegs[this.orient] = os;
			os.seg.subsegs[os.orient] = this;
		}

		// Token: 0x06000910 RID: 2320 RVA: 0x00048F18 File Offset: 0x00047118
		internal void Dissolve(SubSegment dummy)
		{
			this.seg.subsegs[this.orient].seg = dummy;
		}

		// Token: 0x06000911 RID: 2321 RVA: 0x00048F36 File Offset: 0x00047136
		internal bool Equal(Osub os)
		{
			return this.seg == os.seg && this.orient == os.orient;
		}

		// Token: 0x06000912 RID: 2322 RVA: 0x00048F56 File Offset: 0x00047156
		internal void TriDissolve(Triangle dummy)
		{
			this.seg.triangles[this.orient].tri = dummy;
		}

		// Token: 0x06000913 RID: 2323 RVA: 0x00048F74 File Offset: 0x00047174
		internal static bool IsDead(SubSegment sub)
		{
			return sub.subsegs[0].seg == null;
		}

		// Token: 0x06000914 RID: 2324 RVA: 0x00048F8A File Offset: 0x0004718A
		internal static void Kill(SubSegment sub)
		{
			sub.subsegs[0].seg = null;
			sub.subsegs[1].seg = null;
		}

		// Token: 0x04000A1C RID: 2588
		internal SubSegment seg;

		// Token: 0x04000A1D RID: 2589
		internal int orient;
	}
}
