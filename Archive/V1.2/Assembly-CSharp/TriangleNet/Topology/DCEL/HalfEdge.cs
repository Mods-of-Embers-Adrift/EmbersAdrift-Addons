using System;

namespace TriangleNet.Topology.DCEL
{
	// Token: 0x02000105 RID: 261
	public class HalfEdge
	{
		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x0600096F RID: 2415 RVA: 0x00049560 File Offset: 0x00047760
		// (set) Token: 0x06000970 RID: 2416 RVA: 0x00049568 File Offset: 0x00047768
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

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x06000971 RID: 2417 RVA: 0x00049571 File Offset: 0x00047771
		// (set) Token: 0x06000972 RID: 2418 RVA: 0x00049579 File Offset: 0x00047779
		public int Boundary
		{
			get
			{
				return this.mark;
			}
			set
			{
				this.mark = value;
			}
		}

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x06000973 RID: 2419 RVA: 0x00049582 File Offset: 0x00047782
		// (set) Token: 0x06000974 RID: 2420 RVA: 0x0004958A File Offset: 0x0004778A
		public Vertex Origin
		{
			get
			{
				return this.origin;
			}
			set
			{
				this.origin = value;
			}
		}

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x06000975 RID: 2421 RVA: 0x00049593 File Offset: 0x00047793
		// (set) Token: 0x06000976 RID: 2422 RVA: 0x0004959B File Offset: 0x0004779B
		public Face Face
		{
			get
			{
				return this.face;
			}
			set
			{
				this.face = value;
			}
		}

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x06000977 RID: 2423 RVA: 0x000495A4 File Offset: 0x000477A4
		// (set) Token: 0x06000978 RID: 2424 RVA: 0x000495AC File Offset: 0x000477AC
		public HalfEdge Twin
		{
			get
			{
				return this.twin;
			}
			set
			{
				this.twin = value;
			}
		}

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x06000979 RID: 2425 RVA: 0x000495B5 File Offset: 0x000477B5
		// (set) Token: 0x0600097A RID: 2426 RVA: 0x000495BD File Offset: 0x000477BD
		public HalfEdge Next
		{
			get
			{
				return this.next;
			}
			set
			{
				this.next = value;
			}
		}

		// Token: 0x0600097B RID: 2427 RVA: 0x000495C6 File Offset: 0x000477C6
		public HalfEdge(Vertex origin)
		{
			this.origin = origin;
		}

		// Token: 0x0600097C RID: 2428 RVA: 0x000495D5 File Offset: 0x000477D5
		public HalfEdge(Vertex origin, Face face)
		{
			this.origin = origin;
			this.face = face;
			if (face != null && face.edge == null)
			{
				face.edge = this;
			}
		}

		// Token: 0x0600097D RID: 2429 RVA: 0x000495FD File Offset: 0x000477FD
		public override string ToString()
		{
			return string.Format("HE-ID {0} (Origin = VID-{1})", this.id, this.origin.id);
		}

		// Token: 0x04000A3D RID: 2621
		internal int id;

		// Token: 0x04000A3E RID: 2622
		internal int mark;

		// Token: 0x04000A3F RID: 2623
		internal Vertex origin;

		// Token: 0x04000A40 RID: 2624
		internal Face face;

		// Token: 0x04000A41 RID: 2625
		internal HalfEdge twin;

		// Token: 0x04000A42 RID: 2626
		internal HalfEdge next;
	}
}
