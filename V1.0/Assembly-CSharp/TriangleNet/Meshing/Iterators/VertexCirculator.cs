using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.Meshing.Iterators
{
	// Token: 0x02000129 RID: 297
	public class VertexCirculator
	{
		// Token: 0x06000A65 RID: 2661 RVA: 0x00049D20 File Offset: 0x00047F20
		public VertexCirculator(Mesh mesh)
		{
			mesh.MakeVertexMap();
		}

		// Token: 0x06000A66 RID: 2662 RVA: 0x00049D39 File Offset: 0x00047F39
		public IEnumerable<Vertex> EnumerateVertices(Vertex vertex)
		{
			this.BuildCache(vertex, true);
			foreach (Otri otri in this.cache)
			{
				yield return otri.Dest();
			}
			List<Otri>.Enumerator enumerator = default(List<Otri>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06000A67 RID: 2663 RVA: 0x00049D50 File Offset: 0x00047F50
		public IEnumerable<ITriangle> EnumerateTriangles(Vertex vertex)
		{
			this.BuildCache(vertex, false);
			foreach (Otri otri in this.cache)
			{
				yield return otri.tri;
			}
			List<Otri>.Enumerator enumerator = default(List<Otri>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06000A68 RID: 2664 RVA: 0x000C76BC File Offset: 0x000C58BC
		private void BuildCache(Vertex vertex, bool vertices)
		{
			this.cache.Clear();
			Otri tri = vertex.tri;
			Otri otri = default(Otri);
			Otri item = default(Otri);
			tri.Copy(ref otri);
			while (otri.tri.id != -1)
			{
				this.cache.Add(otri);
				otri.Copy(ref item);
				otri.Onext();
				if (otri.Equals(tri))
				{
					break;
				}
			}
			if (otri.tri.id == -1)
			{
				tri.Copy(ref otri);
				if (vertices)
				{
					item.Lnext();
					this.cache.Add(item);
				}
				otri.Oprev();
				while (otri.tri.id != -1)
				{
					this.cache.Insert(0, otri);
					otri.Oprev();
					if (otri.Equals(tri))
					{
						break;
					}
				}
			}
		}

		// Token: 0x04000ABB RID: 2747
		private List<Otri> cache = new List<Otri>();
	}
}
