using System;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet
{
	// Token: 0x020000F0 RID: 240
	public class TriangleLocator
	{
		// Token: 0x06000898 RID: 2200 RVA: 0x00048A00 File Offset: 0x00046C00
		public TriangleLocator(Mesh mesh) : this(mesh, RobustPredicates.Default)
		{
		}

		// Token: 0x06000899 RID: 2201 RVA: 0x00048A0E File Offset: 0x00046C0E
		public TriangleLocator(Mesh mesh, IPredicates predicates)
		{
			this.mesh = mesh;
			this.predicates = predicates;
			this.sampler = new TriangleSampler(mesh);
		}

		// Token: 0x0600089A RID: 2202 RVA: 0x00048A30 File Offset: 0x00046C30
		public void Update(ref Otri otri)
		{
			otri.Copy(ref this.recenttri);
		}

		// Token: 0x0600089B RID: 2203 RVA: 0x00048A3E File Offset: 0x00046C3E
		public void Reset()
		{
			this.sampler.Reset();
			this.recenttri.tri = null;
		}

		// Token: 0x0600089C RID: 2204 RVA: 0x000BCF5C File Offset: 0x000BB15C
		public LocateResult PreciseLocate(Point searchpoint, ref Otri searchtri, bool stopatsubsegment)
		{
			Otri otri = default(Otri);
			Osub osub = default(Osub);
			Vertex vertex = searchtri.Org();
			Vertex vertex2 = searchtri.Dest();
			Vertex vertex3 = searchtri.Apex();
			while (vertex3.x != searchpoint.x || vertex3.y != searchpoint.y)
			{
				double num = this.predicates.CounterClockwise(vertex, vertex3, searchpoint);
				double num2 = this.predicates.CounterClockwise(vertex3, vertex2, searchpoint);
				bool flag;
				if (num > 0.0)
				{
					flag = (num2 <= 0.0 || (vertex3.x - searchpoint.x) * (vertex2.x - vertex.x) + (vertex3.y - searchpoint.y) * (vertex2.y - vertex.y) > 0.0);
				}
				else if (num2 > 0.0)
				{
					flag = false;
				}
				else
				{
					if (num == 0.0)
					{
						searchtri.Lprev();
						return LocateResult.OnEdge;
					}
					if (num2 == 0.0)
					{
						searchtri.Lnext();
						return LocateResult.OnEdge;
					}
					return LocateResult.InTriangle;
				}
				if (flag)
				{
					searchtri.Lprev(ref otri);
					vertex2 = vertex3;
				}
				else
				{
					searchtri.Lnext(ref otri);
					vertex = vertex3;
				}
				otri.Sym(ref searchtri);
				if (this.mesh.checksegments && stopatsubsegment)
				{
					otri.Pivot(ref osub);
					if (osub.seg.hash != -1)
					{
						otri.Copy(ref searchtri);
						return LocateResult.Outside;
					}
				}
				if (searchtri.tri.id == -1)
				{
					otri.Copy(ref searchtri);
					return LocateResult.Outside;
				}
				vertex3 = searchtri.Apex();
			}
			searchtri.Lprev();
			return LocateResult.OnVertex;
		}

		// Token: 0x0600089D RID: 2205 RVA: 0x000BD0F4 File Offset: 0x000BB2F4
		public LocateResult Locate(Point searchpoint, ref Otri searchtri)
		{
			Otri otri = default(Otri);
			Vertex vertex = searchtri.Org();
			double num = (searchpoint.x - vertex.x) * (searchpoint.x - vertex.x) + (searchpoint.y - vertex.y) * (searchpoint.y - vertex.y);
			if (this.recenttri.tri != null && !Otri.IsDead(this.recenttri.tri))
			{
				vertex = this.recenttri.Org();
				if (vertex.x == searchpoint.x && vertex.y == searchpoint.y)
				{
					this.recenttri.Copy(ref searchtri);
					return LocateResult.OnVertex;
				}
				double num2 = (searchpoint.x - vertex.x) * (searchpoint.x - vertex.x) + (searchpoint.y - vertex.y) * (searchpoint.y - vertex.y);
				if (num2 < num)
				{
					this.recenttri.Copy(ref searchtri);
					num = num2;
				}
			}
			this.sampler.Update();
			foreach (Triangle tri in this.sampler)
			{
				otri.tri = tri;
				if (!Otri.IsDead(otri.tri))
				{
					vertex = otri.Org();
					double num2 = (searchpoint.x - vertex.x) * (searchpoint.x - vertex.x) + (searchpoint.y - vertex.y) * (searchpoint.y - vertex.y);
					if (num2 < num)
					{
						otri.Copy(ref searchtri);
						num = num2;
					}
				}
			}
			vertex = searchtri.Org();
			Vertex vertex2 = searchtri.Dest();
			if (vertex.x == searchpoint.x && vertex.y == searchpoint.y)
			{
				return LocateResult.OnVertex;
			}
			if (vertex2.x == searchpoint.x && vertex2.y == searchpoint.y)
			{
				searchtri.Lnext();
				return LocateResult.OnVertex;
			}
			double num3 = this.predicates.CounterClockwise(vertex, vertex2, searchpoint);
			if (num3 < 0.0)
			{
				searchtri.Sym();
			}
			else if (num3 == 0.0 && vertex.x < searchpoint.x == searchpoint.x < vertex2.x && vertex.y < searchpoint.y == searchpoint.y < vertex2.y)
			{
				return LocateResult.OnEdge;
			}
			return this.PreciseLocate(searchpoint, ref searchtri, false);
		}

		// Token: 0x040009E7 RID: 2535
		private TriangleSampler sampler;

		// Token: 0x040009E8 RID: 2536
		private Mesh mesh;

		// Token: 0x040009E9 RID: 2537
		private IPredicates predicates;

		// Token: 0x040009EA RID: 2538
		internal Otri recenttri;
	}
}
