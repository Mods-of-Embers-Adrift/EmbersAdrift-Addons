using System;
using TriangleNet.Geometry;

namespace TriangleNet.Topology
{
	// Token: 0x020000FF RID: 255
	public struct Otri
	{
		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x06000915 RID: 2325 RVA: 0x00048FB0 File Offset: 0x000471B0
		// (set) Token: 0x06000916 RID: 2326 RVA: 0x00048FB8 File Offset: 0x000471B8
		public Triangle Triangle
		{
			get
			{
				return this.tri;
			}
			set
			{
				this.tri = value;
			}
		}

		// Token: 0x06000917 RID: 2327 RVA: 0x00048FC1 File Offset: 0x000471C1
		public override string ToString()
		{
			if (this.tri == null)
			{
				return "O-TID [null]";
			}
			return string.Format("O-TID {0}", this.tri.hash);
		}

		// Token: 0x06000918 RID: 2328 RVA: 0x000BFA74 File Offset: 0x000BDC74
		public void Sym(ref Otri ot)
		{
			ot.tri = this.tri.neighbors[this.orient].tri;
			ot.orient = this.tri.neighbors[this.orient].orient;
		}

		// Token: 0x06000919 RID: 2329 RVA: 0x000BFAC4 File Offset: 0x000BDCC4
		public void Sym()
		{
			int num = this.orient;
			this.orient = this.tri.neighbors[num].orient;
			this.tri = this.tri.neighbors[num].tri;
		}

		// Token: 0x0600091A RID: 2330 RVA: 0x00048FEB File Offset: 0x000471EB
		public void Lnext(ref Otri ot)
		{
			ot.tri = this.tri;
			ot.orient = Otri.plus1Mod3[this.orient];
		}

		// Token: 0x0600091B RID: 2331 RVA: 0x0004900B File Offset: 0x0004720B
		public void Lnext()
		{
			this.orient = Otri.plus1Mod3[this.orient];
		}

		// Token: 0x0600091C RID: 2332 RVA: 0x0004901F File Offset: 0x0004721F
		public void Lprev(ref Otri ot)
		{
			ot.tri = this.tri;
			ot.orient = Otri.minus1Mod3[this.orient];
		}

		// Token: 0x0600091D RID: 2333 RVA: 0x0004903F File Offset: 0x0004723F
		public void Lprev()
		{
			this.orient = Otri.minus1Mod3[this.orient];
		}

		// Token: 0x0600091E RID: 2334 RVA: 0x000BFB10 File Offset: 0x000BDD10
		public void Onext(ref Otri ot)
		{
			ot.tri = this.tri;
			ot.orient = Otri.minus1Mod3[this.orient];
			int num = ot.orient;
			ot.orient = ot.tri.neighbors[num].orient;
			ot.tri = ot.tri.neighbors[num].tri;
		}

		// Token: 0x0600091F RID: 2335 RVA: 0x000BFB7C File Offset: 0x000BDD7C
		public void Onext()
		{
			this.orient = Otri.minus1Mod3[this.orient];
			int num = this.orient;
			this.orient = this.tri.neighbors[num].orient;
			this.tri = this.tri.neighbors[num].tri;
		}

		// Token: 0x06000920 RID: 2336 RVA: 0x000BFBDC File Offset: 0x000BDDDC
		public void Oprev(ref Otri ot)
		{
			ot.tri = this.tri.neighbors[this.orient].tri;
			ot.orient = this.tri.neighbors[this.orient].orient;
			ot.orient = Otri.plus1Mod3[ot.orient];
		}

		// Token: 0x06000921 RID: 2337 RVA: 0x000BFC40 File Offset: 0x000BDE40
		public void Oprev()
		{
			int num = this.orient;
			this.orient = this.tri.neighbors[num].orient;
			this.tri = this.tri.neighbors[num].tri;
			this.orient = Otri.plus1Mod3[this.orient];
		}

		// Token: 0x06000922 RID: 2338 RVA: 0x000BFCA0 File Offset: 0x000BDEA0
		public void Dnext(ref Otri ot)
		{
			ot.tri = this.tri.neighbors[this.orient].tri;
			ot.orient = this.tri.neighbors[this.orient].orient;
			ot.orient = Otri.minus1Mod3[ot.orient];
		}

		// Token: 0x06000923 RID: 2339 RVA: 0x000BFD04 File Offset: 0x000BDF04
		public void Dnext()
		{
			int num = this.orient;
			this.orient = this.tri.neighbors[num].orient;
			this.tri = this.tri.neighbors[num].tri;
			this.orient = Otri.minus1Mod3[this.orient];
		}

		// Token: 0x06000924 RID: 2340 RVA: 0x000BFD64 File Offset: 0x000BDF64
		public void Dprev(ref Otri ot)
		{
			ot.tri = this.tri;
			ot.orient = Otri.plus1Mod3[this.orient];
			int num = ot.orient;
			ot.orient = ot.tri.neighbors[num].orient;
			ot.tri = ot.tri.neighbors[num].tri;
		}

		// Token: 0x06000925 RID: 2341 RVA: 0x000BFDD0 File Offset: 0x000BDFD0
		public void Dprev()
		{
			this.orient = Otri.plus1Mod3[this.orient];
			int num = this.orient;
			this.orient = this.tri.neighbors[num].orient;
			this.tri = this.tri.neighbors[num].tri;
		}

		// Token: 0x06000926 RID: 2342 RVA: 0x000BFE30 File Offset: 0x000BE030
		public void Rnext(ref Otri ot)
		{
			ot.tri = this.tri.neighbors[this.orient].tri;
			ot.orient = this.tri.neighbors[this.orient].orient;
			ot.orient = Otri.plus1Mod3[ot.orient];
			int num = ot.orient;
			ot.orient = ot.tri.neighbors[num].orient;
			ot.tri = ot.tri.neighbors[num].tri;
		}

		// Token: 0x06000927 RID: 2343 RVA: 0x000BFED0 File Offset: 0x000BE0D0
		public void Rnext()
		{
			int num = this.orient;
			this.orient = this.tri.neighbors[num].orient;
			this.tri = this.tri.neighbors[num].tri;
			this.orient = Otri.plus1Mod3[this.orient];
			num = this.orient;
			this.orient = this.tri.neighbors[num].orient;
			this.tri = this.tri.neighbors[num].tri;
		}

		// Token: 0x06000928 RID: 2344 RVA: 0x000BFF70 File Offset: 0x000BE170
		public void Rprev(ref Otri ot)
		{
			ot.tri = this.tri.neighbors[this.orient].tri;
			ot.orient = this.tri.neighbors[this.orient].orient;
			ot.orient = Otri.minus1Mod3[ot.orient];
			int num = ot.orient;
			ot.orient = ot.tri.neighbors[num].orient;
			ot.tri = ot.tri.neighbors[num].tri;
		}

		// Token: 0x06000929 RID: 2345 RVA: 0x000C0010 File Offset: 0x000BE210
		public void Rprev()
		{
			int num = this.orient;
			this.orient = this.tri.neighbors[num].orient;
			this.tri = this.tri.neighbors[num].tri;
			this.orient = Otri.minus1Mod3[this.orient];
			num = this.orient;
			this.orient = this.tri.neighbors[num].orient;
			this.tri = this.tri.neighbors[num].tri;
		}

		// Token: 0x0600092A RID: 2346 RVA: 0x00049053 File Offset: 0x00047253
		public Vertex Org()
		{
			return this.tri.vertices[Otri.plus1Mod3[this.orient]];
		}

		// Token: 0x0600092B RID: 2347 RVA: 0x0004906D File Offset: 0x0004726D
		public Vertex Dest()
		{
			return this.tri.vertices[Otri.minus1Mod3[this.orient]];
		}

		// Token: 0x0600092C RID: 2348 RVA: 0x00049087 File Offset: 0x00047287
		public Vertex Apex()
		{
			return this.tri.vertices[this.orient];
		}

		// Token: 0x0600092D RID: 2349 RVA: 0x0004909B File Offset: 0x0004729B
		public void Copy(ref Otri ot)
		{
			ot.tri = this.tri;
			ot.orient = this.orient;
		}

		// Token: 0x0600092E RID: 2350 RVA: 0x000490B5 File Offset: 0x000472B5
		public bool Equals(Otri ot)
		{
			return this.tri == ot.tri && this.orient == ot.orient;
		}

		// Token: 0x0600092F RID: 2351 RVA: 0x000490D5 File Offset: 0x000472D5
		internal void SetOrg(Vertex v)
		{
			this.tri.vertices[Otri.plus1Mod3[this.orient]] = v;
		}

		// Token: 0x06000930 RID: 2352 RVA: 0x000490F0 File Offset: 0x000472F0
		internal void SetDest(Vertex v)
		{
			this.tri.vertices[Otri.minus1Mod3[this.orient]] = v;
		}

		// Token: 0x06000931 RID: 2353 RVA: 0x0004910B File Offset: 0x0004730B
		internal void SetApex(Vertex v)
		{
			this.tri.vertices[this.orient] = v;
		}

		// Token: 0x06000932 RID: 2354 RVA: 0x000C00B0 File Offset: 0x000BE2B0
		internal void Bond(ref Otri ot)
		{
			this.tri.neighbors[this.orient].tri = ot.tri;
			this.tri.neighbors[this.orient].orient = ot.orient;
			ot.tri.neighbors[ot.orient].tri = this.tri;
			ot.tri.neighbors[ot.orient].orient = this.orient;
		}

		// Token: 0x06000933 RID: 2355 RVA: 0x00049120 File Offset: 0x00047320
		internal void Dissolve(Triangle dummy)
		{
			this.tri.neighbors[this.orient].tri = dummy;
			this.tri.neighbors[this.orient].orient = 0;
		}

		// Token: 0x06000934 RID: 2356 RVA: 0x0004915A File Offset: 0x0004735A
		internal void Infect()
		{
			this.tri.infected = true;
		}

		// Token: 0x06000935 RID: 2357 RVA: 0x00049168 File Offset: 0x00047368
		internal void Uninfect()
		{
			this.tri.infected = false;
		}

		// Token: 0x06000936 RID: 2358 RVA: 0x00049176 File Offset: 0x00047376
		internal bool IsInfected()
		{
			return this.tri.infected;
		}

		// Token: 0x06000937 RID: 2359 RVA: 0x00049183 File Offset: 0x00047383
		internal void Pivot(ref Osub os)
		{
			os = this.tri.subsegs[this.orient];
		}

		// Token: 0x06000938 RID: 2360 RVA: 0x000491A1 File Offset: 0x000473A1
		internal void SegBond(ref Osub os)
		{
			this.tri.subsegs[this.orient] = os;
			os.seg.triangles[os.orient] = this;
		}

		// Token: 0x06000939 RID: 2361 RVA: 0x000491DB File Offset: 0x000473DB
		internal void SegDissolve(SubSegment dummy)
		{
			this.tri.subsegs[this.orient].seg = dummy;
		}

		// Token: 0x0600093A RID: 2362 RVA: 0x000491F9 File Offset: 0x000473F9
		internal static bool IsDead(Triangle tria)
		{
			return tria.neighbors[0].tri == null;
		}

		// Token: 0x0600093B RID: 2363 RVA: 0x0004920F File Offset: 0x0004740F
		internal static void Kill(Triangle tri)
		{
			tri.neighbors[0].tri = null;
			tri.neighbors[2].tri = null;
		}

		// Token: 0x0600093C RID: 2364 RVA: 0x00049235 File Offset: 0x00047435
		// Note: this type is marked as 'beforefieldinit'.
		static Otri()
		{
			int[] array = new int[3];
			array[0] = 1;
			array[1] = 2;
			Otri.plus1Mod3 = array;
			Otri.minus1Mod3 = new int[]
			{
				2,
				0,
				1
			};
		}

		// Token: 0x04000A1E RID: 2590
		internal Triangle tri;

		// Token: 0x04000A1F RID: 2591
		internal int orient;

		// Token: 0x04000A20 RID: 2592
		private static readonly int[] plus1Mod3;

		// Token: 0x04000A21 RID: 2593
		private static readonly int[] minus1Mod3;
	}
}
