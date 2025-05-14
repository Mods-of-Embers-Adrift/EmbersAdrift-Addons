using System;
using System.Collections.Generic;
using System.Linq;
using TriangleNet.Geometry;
using TriangleNet.Topology;
using TriangleNet.Topology.DCEL;

namespace TriangleNet.Meshing
{
	// Token: 0x0200011C RID: 284
	public static class Converter
	{
		// Token: 0x06000A20 RID: 2592 RVA: 0x00049BA2 File Offset: 0x00047DA2
		public static Mesh ToMesh(Polygon polygon, IList<ITriangle> triangles)
		{
			return Converter.ToMesh(polygon, triangles.ToArray<ITriangle>());
		}

		// Token: 0x06000A21 RID: 2593 RVA: 0x000C5104 File Offset: 0x000C3304
		public static Mesh ToMesh(Polygon polygon, ITriangle[] triangles)
		{
			Otri otri = default(Otri);
			Osub osub = default(Osub);
			int num = (triangles == null) ? 0 : triangles.Length;
			int count = polygon.Segments.Count;
			Mesh mesh = new Mesh(new Configuration());
			mesh.TransferNodes(polygon.Points);
			mesh.regions.AddRange(polygon.Regions);
			mesh.behavior.useRegions = (polygon.Regions.Count > 0);
			if (polygon.Segments.Count > 0)
			{
				mesh.behavior.Poly = true;
				mesh.holes.AddRange(polygon.Holes);
			}
			for (int i = 0; i < num; i++)
			{
				mesh.MakeTriangle(ref otri);
			}
			if (mesh.behavior.Poly)
			{
				mesh.insegments = count;
				for (int i = 0; i < count; i++)
				{
					mesh.MakeSegment(ref osub);
				}
			}
			List<Otri>[] vertexarray = Converter.SetNeighbors(mesh, triangles);
			Converter.SetSegments(mesh, polygon, vertexarray);
			return mesh;
		}

		// Token: 0x06000A22 RID: 2594 RVA: 0x000C5204 File Offset: 0x000C3404
		private static List<Otri>[] SetNeighbors(Mesh mesh, ITriangle[] triangles)
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Otri otri3 = default(Otri);
			Otri otri4 = default(Otri);
			int[] array = new int[3];
			List<Otri>[] array2 = new List<Otri>[mesh.vertices.Count];
			int i;
			for (i = 0; i < mesh.vertices.Count; i++)
			{
				Otri item = default(Otri);
				item.tri = mesh.dummytri;
				array2[i] = new List<Otri>(3);
				array2[i].Add(item);
			}
			i = 0;
			foreach (Triangle tri in mesh.triangles)
			{
				otri.tri = tri;
				for (int j = 0; j < 3; j++)
				{
					array[j] = triangles[i].GetVertexID(j);
					if (array[j] < 0 || array[j] >= mesh.invertices)
					{
						Log.Instance.Error("Triangle has an invalid vertex index.", "MeshReader.Reconstruct()");
						throw new Exception("Triangle has an invalid vertex index.");
					}
				}
				otri.tri.label = triangles[i].Label;
				if (mesh.behavior.VarArea)
				{
					otri.tri.area = triangles[i].Area;
				}
				otri.orient = 0;
				otri.SetOrg(mesh.vertices[array[0]]);
				otri.SetDest(mesh.vertices[array[1]]);
				otri.SetApex(mesh.vertices[array[2]]);
				otri.orient = 0;
				while (otri.orient < 3)
				{
					int num = array[otri.orient];
					int num2 = array2[num].Count - 1;
					Otri otri5 = array2[num][num2];
					array2[num].Add(otri);
					otri3 = otri5;
					if (otri3.tri.id != -1)
					{
						TriangleNet.Geometry.Vertex a = otri.Dest();
						TriangleNet.Geometry.Vertex a2 = otri.Apex();
						do
						{
							TriangleNet.Geometry.Vertex b = otri3.Dest();
							TriangleNet.Geometry.Vertex b2 = otri3.Apex();
							if (a2 == b)
							{
								otri.Lprev(ref otri2);
								otri2.Bond(ref otri3);
							}
							if (a == b2)
							{
								otri3.Lprev(ref otri4);
								otri.Bond(ref otri4);
							}
							num2--;
							otri5 = array2[num][num2];
							otri3 = otri5;
						}
						while (otri3.tri.id != -1);
					}
					otri.orient++;
				}
				i++;
			}
			return array2;
		}

		// Token: 0x06000A23 RID: 2595 RVA: 0x000C54B8 File Offset: 0x000C36B8
		private static void SetSegments(Mesh mesh, Polygon polygon, List<Otri>[] vertexarray)
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Osub osub = default(Osub);
			int num = 0;
			if (mesh.behavior.Poly)
			{
				int i = 0;
				foreach (SubSegment seg in mesh.subsegs.Values)
				{
					osub.seg = seg;
					TriangleNet.Geometry.Vertex vertex = polygon.Segments[i].GetVertex(0);
					TriangleNet.Geometry.Vertex vertex2 = polygon.Segments[i].GetVertex(1);
					int label = polygon.Segments[i].Label;
					if (vertex.id < 0 || vertex.id >= mesh.invertices || vertex2.id < 0 || vertex2.id >= mesh.invertices)
					{
						Log.Instance.Error("Segment has an invalid vertex index.", "MeshReader.Reconstruct()");
						throw new Exception("Segment has an invalid vertex index.");
					}
					osub.orient = 0;
					osub.SetOrg(vertex);
					osub.SetDest(vertex2);
					osub.SetSegOrg(vertex);
					osub.SetSegDest(vertex2);
					osub.seg.boundary = label;
					osub.orient = 0;
					while (osub.orient < 2)
					{
						int num2 = (osub.orient == 1) ? vertex.id : vertex2.id;
						int num3 = vertexarray[num2].Count - 1;
						Otri item = vertexarray[num2][num3];
						otri = vertexarray[num2][num3];
						TriangleNet.Geometry.Vertex a = osub.Org();
						bool flag = true;
						while (flag && otri.tri.id != -1)
						{
							TriangleNet.Geometry.Vertex b = otri.Dest();
							if (a == b)
							{
								vertexarray[num2].Remove(item);
								otri.SegBond(ref osub);
								otri.Sym(ref otri2);
								if (otri2.tri.id == -1)
								{
									mesh.InsertSubseg(ref otri, 1);
									num++;
								}
								flag = false;
							}
							num3--;
							item = vertexarray[num2][num3];
							otri = vertexarray[num2][num3];
						}
						osub.orient++;
					}
					i++;
				}
			}
			for (int i = 0; i < mesh.vertices.Count; i++)
			{
				int num4 = vertexarray[i].Count - 1;
				otri = vertexarray[i][num4];
				while (otri.tri.id != -1)
				{
					num4--;
					Otri otri3 = vertexarray[i][num4];
					otri.SegDissolve(mesh.dummysub);
					otri.Sym(ref otri2);
					if (otri2.tri.id == -1)
					{
						mesh.InsertSubseg(ref otri, 1);
						num++;
					}
					otri = otri3;
				}
			}
			mesh.hullsize = num;
		}

		// Token: 0x06000A24 RID: 2596 RVA: 0x000C57B0 File Offset: 0x000C39B0
		public static DcelMesh ToDCEL(Mesh mesh)
		{
			DcelMesh dcelMesh = new DcelMesh();
			TriangleNet.Topology.DCEL.Vertex[] array = new TriangleNet.Topology.DCEL.Vertex[mesh.vertices.Count];
			Face[] array2 = new Face[mesh.triangles.Count];
			dcelMesh.HalfEdges.Capacity = 2 * mesh.NumberOfEdges;
			mesh.Renumber();
			foreach (TriangleNet.Geometry.Vertex vertex in mesh.vertices.Values)
			{
				TriangleNet.Topology.DCEL.Vertex vertex2 = new TriangleNet.Topology.DCEL.Vertex(vertex.x, vertex.y);
				vertex2.id = vertex.id;
				vertex2.label = vertex.label;
				array[vertex.id] = vertex2;
			}
			List<HalfEdge>[] array3 = new List<HalfEdge>[mesh.triangles.Count];
			foreach (Triangle triangle in mesh.triangles)
			{
				Face face = new Face(null);
				face.id = triangle.id;
				array2[triangle.id] = face;
				array3[triangle.id] = new List<HalfEdge>(3);
			}
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			int count = mesh.triangles.Count;
			List<HalfEdge> halfEdges = dcelMesh.HalfEdges;
			int num = 0;
			Dictionary<int, HalfEdge> dictionary = new Dictionary<int, HalfEdge>();
			foreach (Triangle triangle2 in mesh.triangles)
			{
				int id = triangle2.id;
				otri.tri = triangle2;
				for (int i = 0; i < 3; i++)
				{
					otri.orient = i;
					otri.Sym(ref otri2);
					int id2 = otri2.tri.id;
					if (id < id2 || id2 < 0)
					{
						Face face = array2[id];
						TriangleNet.Geometry.Vertex vertex3 = otri.Org();
						TriangleNet.Geometry.Vertex vertex4 = otri.Dest();
						HalfEdge halfEdge = new HalfEdge(array[vertex3.id], face);
						HalfEdge halfEdge2 = new HalfEdge(array[vertex4.id], (id2 < 0) ? Face.Empty : array2[id2]);
						array3[id].Add(halfEdge);
						if (id2 >= 0)
						{
							array3[id2].Add(halfEdge2);
						}
						else
						{
							dictionary.Add(vertex4.id, halfEdge2);
						}
						halfEdge.origin.leaving = halfEdge;
						halfEdge2.origin.leaving = halfEdge2;
						halfEdge.twin = halfEdge2;
						halfEdge2.twin = halfEdge;
						halfEdge.id = num++;
						halfEdge2.id = num++;
						halfEdges.Add(halfEdge);
						halfEdges.Add(halfEdge2);
					}
				}
			}
			foreach (List<HalfEdge> list in array3)
			{
				HalfEdge halfEdge = list[0];
				HalfEdge halfEdge3 = list[1];
				if (halfEdge.twin.origin.id == halfEdge3.origin.id)
				{
					halfEdge.next = halfEdge3;
					halfEdge3.next = list[2];
					list[2].next = halfEdge;
				}
				else
				{
					halfEdge.next = list[2];
					halfEdge3.next = halfEdge;
					list[2].next = halfEdge3;
				}
			}
			foreach (HalfEdge halfEdge4 in dictionary.Values)
			{
				halfEdge4.next = dictionary[halfEdge4.twin.origin.id];
			}
			dcelMesh.Vertices.AddRange(array);
			dcelMesh.Faces.AddRange(array2);
			return dcelMesh;
		}
	}
}
