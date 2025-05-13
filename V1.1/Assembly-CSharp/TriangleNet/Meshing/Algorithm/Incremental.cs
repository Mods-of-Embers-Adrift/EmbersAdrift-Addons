using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.Meshing.Algorithm
{
	// Token: 0x02000130 RID: 304
	public class Incremental : ITriangulator
	{
		// Token: 0x06000A8A RID: 2698 RVA: 0x000C89F4 File Offset: 0x000C6BF4
		public IMesh Triangulate(IList<Vertex> points, Configuration config)
		{
			this.mesh = new Mesh(config);
			this.mesh.TransferNodes(points);
			Otri otri = default(Otri);
			this.GetBoundingBox();
			foreach (Vertex vertex in this.mesh.vertices.Values)
			{
				otri.tri = this.mesh.dummytri;
				Osub osub = default(Osub);
				if (this.mesh.InsertVertex(vertex, ref otri, ref osub, false, false) == InsertVertexResult.Duplicate)
				{
					if (Log.Verbose)
					{
						Log.Instance.Warning("A duplicate vertex appeared and was ignored.", "Incremental.Triangulate()");
					}
					vertex.type = VertexType.UndeadVertex;
					this.mesh.undeads++;
				}
			}
			this.mesh.hullsize = this.RemoveBox();
			return this.mesh;
		}

		// Token: 0x06000A8B RID: 2699 RVA: 0x000C8AEC File Offset: 0x000C6CEC
		private void GetBoundingBox()
		{
			Otri otri = default(Otri);
			Rectangle bounds = this.mesh.bounds;
			double num = bounds.Width;
			if (bounds.Height > num)
			{
				num = bounds.Height;
			}
			if (num == 0.0)
			{
				num = 1.0;
			}
			this.mesh.infvertex1 = new Vertex(bounds.Left - 50.0 * num, bounds.Bottom - 40.0 * num);
			this.mesh.infvertex2 = new Vertex(bounds.Right + 50.0 * num, bounds.Bottom - 40.0 * num);
			this.mesh.infvertex3 = new Vertex(0.5 * (bounds.Left + bounds.Right), bounds.Top + 60.0 * num);
			this.mesh.MakeTriangle(ref otri);
			otri.SetOrg(this.mesh.infvertex1);
			otri.SetDest(this.mesh.infvertex2);
			otri.SetApex(this.mesh.infvertex3);
			this.mesh.dummytri.neighbors[0] = otri;
		}

		// Token: 0x06000A8C RID: 2700 RVA: 0x000C8C38 File Offset: 0x000C6E38
		private int RemoveBox()
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Otri otri3 = default(Otri);
			Otri otri4 = default(Otri);
			Otri otri5 = default(Otri);
			Otri otri6 = default(Otri);
			bool flag = !this.mesh.behavior.Poly;
			otri4.tri = this.mesh.dummytri;
			otri4.orient = 0;
			otri4.Sym();
			otri4.Lprev(ref otri5);
			otri4.Lnext();
			otri4.Sym();
			otri4.Lprev(ref otri2);
			otri2.Sym();
			otri4.Lnext(ref otri3);
			otri3.Sym();
			if (otri3.tri.id == -1)
			{
				otri2.Lprev();
				otri2.Sym();
			}
			this.mesh.dummytri.neighbors[0] = otri2;
			int num = -2;
			while (!otri4.Equals(otri5))
			{
				num++;
				otri4.Lprev(ref otri6);
				otri6.Sym();
				if (flag && otri6.tri.id != -1)
				{
					Vertex vertex = otri6.Org();
					if (vertex.label == 0)
					{
						vertex.label = 1;
					}
				}
				otri6.Dissolve(this.mesh.dummytri);
				otri4.Lnext(ref otri);
				otri.Sym(ref otri4);
				this.mesh.TriangleDealloc(otri.tri);
				if (otri4.tri.id == -1)
				{
					otri6.Copy(ref otri4);
				}
			}
			this.mesh.TriangleDealloc(otri5.tri);
			return num;
		}

		// Token: 0x04000ADD RID: 2781
		private Mesh mesh;
	}
}
