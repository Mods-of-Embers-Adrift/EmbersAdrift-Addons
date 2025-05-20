using System;
using System.Collections;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.Meshing.Iterators
{
	// Token: 0x02000124 RID: 292
	public class EdgeIterator : IEnumerator<Edge>, IEnumerator, IDisposable
	{
		// Token: 0x06000A54 RID: 2644 RVA: 0x000C7374 File Offset: 0x000C5574
		public EdgeIterator(Mesh mesh)
		{
			this.triangles = mesh.triangles.GetEnumerator();
			this.triangles.MoveNext();
			this.tri.tri = this.triangles.Current;
			this.tri.orient = 0;
		}

		// Token: 0x1700032B RID: 811
		// (get) Token: 0x06000A55 RID: 2645 RVA: 0x00049C9D File Offset: 0x00047E9D
		public Edge Current
		{
			get
			{
				return this.current;
			}
		}

		// Token: 0x06000A56 RID: 2646 RVA: 0x00049CA5 File Offset: 0x00047EA5
		public void Dispose()
		{
			this.triangles.Dispose();
		}

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x06000A57 RID: 2647 RVA: 0x00049C9D File Offset: 0x00047E9D
		object IEnumerator.Current
		{
			get
			{
				return this.current;
			}
		}

		// Token: 0x06000A58 RID: 2648 RVA: 0x000C73C8 File Offset: 0x000C55C8
		public bool MoveNext()
		{
			if (this.tri.tri == null)
			{
				return false;
			}
			this.current = null;
			while (this.current == null)
			{
				if (this.tri.orient == 3)
				{
					if (!this.triangles.MoveNext())
					{
						return false;
					}
					this.tri.tri = this.triangles.Current;
					this.tri.orient = 0;
				}
				this.tri.Sym(ref this.neighbor);
				if (this.tri.tri.id < this.neighbor.tri.id || this.neighbor.tri.id == -1)
				{
					this.p1 = this.tri.Org();
					this.p2 = this.tri.Dest();
					this.tri.Pivot(ref this.sub);
					this.current = new Edge(this.p1.id, this.p2.id, this.sub.seg.boundary);
				}
				this.tri.orient = this.tri.orient + 1;
			}
			return true;
		}

		// Token: 0x06000A59 RID: 2649 RVA: 0x00049CB2 File Offset: 0x00047EB2
		public void Reset()
		{
			this.triangles.Reset();
		}

		// Token: 0x04000AAF RID: 2735
		private IEnumerator<Triangle> triangles;

		// Token: 0x04000AB0 RID: 2736
		private Otri tri;

		// Token: 0x04000AB1 RID: 2737
		private Otri neighbor;

		// Token: 0x04000AB2 RID: 2738
		private Osub sub;

		// Token: 0x04000AB3 RID: 2739
		private Edge current;

		// Token: 0x04000AB4 RID: 2740
		private Vertex p1;

		// Token: 0x04000AB5 RID: 2741
		private Vertex p2;
	}
}
