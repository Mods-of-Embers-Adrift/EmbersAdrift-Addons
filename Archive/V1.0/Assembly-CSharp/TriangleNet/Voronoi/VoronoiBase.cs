using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Topology;
using TriangleNet.Topology.DCEL;

namespace TriangleNet.Voronoi
{
	// Token: 0x020000F9 RID: 249
	public abstract class VoronoiBase : DcelMesh
	{
		// Token: 0x060008D5 RID: 2261 RVA: 0x00048C02 File Offset: 0x00046E02
		protected VoronoiBase(Mesh mesh, IVoronoiFactory factory, IPredicates predicates, bool generate) : base(false)
		{
			this.factory = factory;
			this.predicates = predicates;
			if (generate)
			{
				this.Generate(mesh);
			}
		}

		// Token: 0x060008D6 RID: 2262 RVA: 0x000BDC7C File Offset: 0x000BBE7C
		protected void Generate(Mesh mesh)
		{
			mesh.Renumber();
			this.edges = new List<HalfEdge>();
			this.rays = new List<HalfEdge>();
			TriangleNet.Topology.DCEL.Vertex[] array = new TriangleNet.Topology.DCEL.Vertex[mesh.triangles.Count + mesh.hullsize];
			Face[] array2 = new Face[mesh.vertices.Count];
			if (this.factory == null)
			{
				this.factory = new DefaultVoronoiFactory();
			}
			this.factory.Initialize(array.Length, 2 * mesh.NumberOfEdges, array2.Length);
			List<HalfEdge>[] map = this.ComputeVertices(mesh, array);
			foreach (TriangleNet.Geometry.Vertex vertex in mesh.vertices.Values)
			{
				array2[vertex.id] = this.factory.CreateFace(vertex);
			}
			this.ComputeEdges(mesh, array, array2, map);
			this.ConnectEdges(map);
			this.vertices = new List<TriangleNet.Topology.DCEL.Vertex>(array);
			this.faces = new List<Face>(array2);
		}

		// Token: 0x060008D7 RID: 2263 RVA: 0x000BDD88 File Offset: 0x000BBF88
		protected List<HalfEdge>[] ComputeVertices(Mesh mesh, TriangleNet.Topology.DCEL.Vertex[] vertices)
		{
			Otri otri = default(Otri);
			double num = 0.0;
			double num2 = 0.0;
			List<HalfEdge>[] array = new List<HalfEdge>[mesh.triangles.Count];
			foreach (Triangle triangle in mesh.triangles)
			{
				int id = triangle.id;
				otri.tri = triangle;
				Point point = this.predicates.FindCircumcenter(otri.Org(), otri.Dest(), otri.Apex(), ref num, ref num2);
				TriangleNet.Topology.DCEL.Vertex vertex = this.factory.CreateVertex(point.x, point.y);
				vertex.id = id;
				vertices[id] = vertex;
				array[id] = new List<HalfEdge>();
			}
			return array;
		}

		// Token: 0x060008D8 RID: 2264 RVA: 0x000BDE70 File Offset: 0x000BC070
		protected void ComputeEdges(Mesh mesh, TriangleNet.Topology.DCEL.Vertex[] vertices, Face[] faces, List<HalfEdge>[] map)
		{
			Otri otri = default(Otri);
			int count = mesh.triangles.Count;
			int num = 0;
			int num2 = 0;
			foreach (Triangle triangle in mesh.triangles)
			{
				int id = triangle.id;
				Otri otri2;
				otri2.tri = triangle;
				for (int i = 0; i < 3; i++)
				{
					otri2.orient = i;
					otri2.Sym(ref otri);
					int id2 = otri.tri.id;
					if (id < id2 || id2 < 0)
					{
						TriangleNet.Geometry.Vertex vertex = otri2.Org();
						TriangleNet.Geometry.Vertex vertex2 = otri2.Dest();
						Face face = faces[vertex.id];
						Face face2 = faces[vertex2.id];
						TriangleNet.Topology.DCEL.Vertex vertex3 = vertices[id];
						TriangleNet.Topology.DCEL.Vertex vertex4;
						HalfEdge halfEdge;
						HalfEdge halfEdge2;
						if (id2 < 0)
						{
							double num3 = vertex2.y - vertex.y;
							double num4 = vertex.x - vertex2.x;
							vertex4 = this.factory.CreateVertex(vertex3.x + num3, vertex3.y + num4);
							vertex4.id = count + num++;
							vertices[vertex4.id] = vertex4;
							halfEdge = this.factory.CreateHalfEdge(vertex4, face);
							halfEdge2 = this.factory.CreateHalfEdge(vertex3, face2);
							face.edge = halfEdge;
							face.bounded = false;
							map[id].Add(halfEdge2);
							this.rays.Add(halfEdge2);
						}
						else
						{
							vertex4 = vertices[id2];
							halfEdge = this.factory.CreateHalfEdge(vertex4, face);
							halfEdge2 = this.factory.CreateHalfEdge(vertex3, face2);
							map[id2].Add(halfEdge);
							map[id].Add(halfEdge2);
						}
						vertex3.leaving = halfEdge2;
						vertex4.leaving = halfEdge;
						halfEdge.twin = halfEdge2;
						halfEdge2.twin = halfEdge;
						halfEdge.id = num2++;
						halfEdge2.id = num2++;
						this.edges.Add(halfEdge);
						this.edges.Add(halfEdge2);
					}
				}
			}
		}

		// Token: 0x060008D9 RID: 2265 RVA: 0x000BE0B4 File Offset: 0x000BC2B4
		protected virtual void ConnectEdges(List<HalfEdge>[] map)
		{
			int num = map.Length;
			foreach (HalfEdge halfEdge in this.edges)
			{
				int id = halfEdge.face.generator.id;
				int id2 = halfEdge.twin.origin.id;
				if (id2 < num)
				{
					foreach (HalfEdge halfEdge2 in map[id2])
					{
						if (halfEdge2.face.generator.id == id)
						{
							halfEdge.next = halfEdge2;
							break;
						}
					}
				}
			}
		}

		// Token: 0x060008DA RID: 2266 RVA: 0x000BE188 File Offset: 0x000BC388
		protected override IEnumerable<IEdge> EnumerateEdges()
		{
			List<IEdge> list = new List<IEdge>(this.edges.Count / 2);
			foreach (HalfEdge halfEdge in this.edges)
			{
				HalfEdge twin = halfEdge.twin;
				if (twin == null)
				{
					list.Add(new Edge(halfEdge.origin.id, halfEdge.next.origin.id));
				}
				else if (halfEdge.id < twin.id)
				{
					list.Add(new Edge(halfEdge.origin.id, twin.origin.id));
				}
			}
			return list;
		}

		// Token: 0x04000A05 RID: 2565
		protected IPredicates predicates;

		// Token: 0x04000A06 RID: 2566
		protected IVoronoiFactory factory;

		// Token: 0x04000A07 RID: 2567
		protected List<HalfEdge> rays;
	}
}
