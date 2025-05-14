using System;
using System.Collections.Generic;
using TriangleNet.Geometry;

namespace TriangleNet.Topology.DCEL
{
	// Token: 0x02000102 RID: 258
	public class DcelMesh
	{
		// Token: 0x06000953 RID: 2387 RVA: 0x00049433 File Offset: 0x00047633
		public DcelMesh() : this(true)
		{
		}

		// Token: 0x06000954 RID: 2388 RVA: 0x0004943C File Offset: 0x0004763C
		protected DcelMesh(bool initialize)
		{
			if (initialize)
			{
				this.vertices = new List<Vertex>();
				this.edges = new List<HalfEdge>();
				this.faces = new List<Face>();
			}
		}

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x06000955 RID: 2389 RVA: 0x00049468 File Offset: 0x00047668
		public List<Vertex> Vertices
		{
			get
			{
				return this.vertices;
			}
		}

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x06000956 RID: 2390 RVA: 0x00049470 File Offset: 0x00047670
		public List<HalfEdge> HalfEdges
		{
			get
			{
				return this.edges;
			}
		}

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x06000957 RID: 2391 RVA: 0x00049478 File Offset: 0x00047678
		public List<Face> Faces
		{
			get
			{
				return this.faces;
			}
		}

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x06000958 RID: 2392 RVA: 0x00049480 File Offset: 0x00047680
		public IEnumerable<IEdge> Edges
		{
			get
			{
				return this.EnumerateEdges();
			}
		}

		// Token: 0x06000959 RID: 2393 RVA: 0x000C0144 File Offset: 0x000BE344
		public virtual bool IsConsistent(bool closed = true, int depth = 0)
		{
			foreach (Vertex vertex in this.vertices)
			{
				if (vertex.id >= 0)
				{
					if (vertex.leaving == null)
					{
						return false;
					}
					if (vertex.Leaving.Origin.id != vertex.id)
					{
						return false;
					}
				}
			}
			foreach (Face face in this.faces)
			{
				if (face.ID >= 0)
				{
					if (face.edge == null)
					{
						return false;
					}
					if (face.id != face.edge.face.id)
					{
						return false;
					}
				}
			}
			foreach (HalfEdge halfEdge in this.edges)
			{
				if (halfEdge.id >= 0)
				{
					if (halfEdge.twin == null)
					{
						return false;
					}
					if (halfEdge.origin == null)
					{
						return false;
					}
					if (halfEdge.face == null)
					{
						return false;
					}
					if (closed && halfEdge.next == null)
					{
						return false;
					}
				}
			}
			foreach (HalfEdge halfEdge2 in this.edges)
			{
				if (halfEdge2.id >= 0)
				{
					HalfEdge twin = halfEdge2.twin;
					HalfEdge next = halfEdge2.next;
					if (halfEdge2.id != twin.twin.id)
					{
						return false;
					}
					if (closed)
					{
						if (next.origin.id != twin.origin.id)
						{
							return false;
						}
						if (next.twin.next.origin.id != halfEdge2.twin.origin.id)
						{
							return false;
						}
					}
				}
			}
			if (closed && depth > 0)
			{
				foreach (Face face2 in this.faces)
				{
					if (face2.id >= 0)
					{
						HalfEdge edge = face2.edge;
						HalfEdge next2 = edge.next;
						int id = edge.id;
						int num = 0;
						while (next2.id != id && num < depth)
						{
							next2 = next2.next;
							num++;
						}
						if (next2.id != id)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		// Token: 0x0600095A RID: 2394 RVA: 0x000C0440 File Offset: 0x000BE640
		public void ResolveBoundaryEdges()
		{
			Dictionary<int, HalfEdge> dictionary = new Dictionary<int, HalfEdge>();
			foreach (HalfEdge halfEdge in this.edges)
			{
				if (halfEdge.twin == null)
				{
					HalfEdge halfEdge2 = halfEdge.twin = new HalfEdge(halfEdge.next.origin, Face.Empty);
					halfEdge2.twin = halfEdge;
					dictionary.Add(halfEdge2.origin.id, halfEdge2);
				}
			}
			int count = this.edges.Count;
			foreach (HalfEdge halfEdge3 in dictionary.Values)
			{
				halfEdge3.id = count++;
				halfEdge3.next = dictionary[halfEdge3.twin.origin.id];
				this.edges.Add(halfEdge3);
			}
		}

		// Token: 0x0600095B RID: 2395 RVA: 0x000C0558 File Offset: 0x000BE758
		protected virtual IEnumerable<IEdge> EnumerateEdges()
		{
			List<IEdge> list = new List<IEdge>(this.edges.Count / 2);
			foreach (HalfEdge halfEdge in this.edges)
			{
				HalfEdge twin = halfEdge.twin;
				if (halfEdge.id < twin.id)
				{
					list.Add(new Edge(halfEdge.origin.id, twin.origin.id));
				}
			}
			return list;
		}

		// Token: 0x04000A2F RID: 2607
		protected List<Vertex> vertices;

		// Token: 0x04000A30 RID: 2608
		protected List<HalfEdge> edges;

		// Token: 0x04000A31 RID: 2609
		protected List<Face> faces;
	}
}
