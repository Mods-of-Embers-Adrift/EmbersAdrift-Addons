using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.IO
{
	// Token: 0x02000141 RID: 321
	public class TriangleWriter
	{
		// Token: 0x06000AF0 RID: 2800 RVA: 0x0004A091 File Offset: 0x00048291
		public void Write(Mesh mesh, string filename)
		{
			this.WritePoly(mesh, Path.ChangeExtension(filename, ".poly"));
			this.WriteElements(mesh, Path.ChangeExtension(filename, ".ele"));
		}

		// Token: 0x06000AF1 RID: 2801 RVA: 0x000CB010 File Offset: 0x000C9210
		public void WriteNodes(Mesh mesh, string filename)
		{
			using (StreamWriter streamWriter = new StreamWriter(filename))
			{
				this.WriteNodes(streamWriter, mesh);
			}
		}

		// Token: 0x06000AF2 RID: 2802 RVA: 0x000CB048 File Offset: 0x000C9248
		private void WriteNodes(StreamWriter writer, Mesh mesh)
		{
			int num = mesh.vertices.Count;
			int nextras = mesh.nextras;
			Behavior behavior = mesh.behavior;
			if (behavior.Jettison)
			{
				num = mesh.vertices.Count - mesh.undeads;
			}
			if (writer != null)
			{
				writer.WriteLine("{0} {1} {2} {3}", new object[]
				{
					num,
					mesh.mesh_dim,
					nextras,
					behavior.UseBoundaryMarkers ? "1" : "0"
				});
				if (mesh.numbering == NodeNumbering.None)
				{
					mesh.Renumber();
				}
				if (mesh.numbering == NodeNumbering.Linear)
				{
					this.WriteNodes(writer, mesh.vertices.Values, behavior.UseBoundaryMarkers, nextras, behavior.Jettison);
					return;
				}
				Vertex[] array = new Vertex[mesh.vertices.Count];
				foreach (Vertex vertex in mesh.vertices.Values)
				{
					array[vertex.id] = vertex;
				}
				this.WriteNodes(writer, array, behavior.UseBoundaryMarkers, nextras, behavior.Jettison);
			}
		}

		// Token: 0x06000AF3 RID: 2803 RVA: 0x000CB188 File Offset: 0x000C9388
		private void WriteNodes(StreamWriter writer, IEnumerable<Vertex> nodes, bool markers, int attribs, bool jettison)
		{
			int num = 0;
			foreach (Vertex vertex in nodes)
			{
				if (!jettison || vertex.type != VertexType.UndeadVertex)
				{
					writer.Write("{0} {1} {2}", num, vertex.x.ToString(TriangleWriter.nfi), vertex.y.ToString(TriangleWriter.nfi));
					if (markers)
					{
						writer.Write(" {0}", vertex.label);
					}
					writer.WriteLine();
					num++;
				}
			}
		}

		// Token: 0x06000AF4 RID: 2804 RVA: 0x000CB22C File Offset: 0x000C942C
		public void WriteElements(Mesh mesh, string filename)
		{
			Otri otri = default(Otri);
			bool useRegions = mesh.behavior.useRegions;
			int num = 0;
			otri.orient = 0;
			using (StreamWriter streamWriter = new StreamWriter(filename))
			{
				streamWriter.WriteLine("{0} 3 {1}", mesh.triangles.Count, useRegions ? 1 : 0);
				foreach (Triangle triangle in mesh.triangles)
				{
					otri.tri = triangle;
					Vertex vertex = otri.Org();
					Vertex vertex2 = otri.Dest();
					Vertex vertex3 = otri.Apex();
					streamWriter.Write("{0} {1} {2} {3}", new object[]
					{
						num,
						vertex.id,
						vertex2.id,
						vertex3.id
					});
					if (useRegions)
					{
						streamWriter.Write(" {0}", otri.tri.label);
					}
					streamWriter.WriteLine();
					triangle.id = num++;
				}
			}
		}

		// Token: 0x06000AF5 RID: 2805 RVA: 0x000CB384 File Offset: 0x000C9584
		public void WritePoly(IPolygon polygon, string filename)
		{
			bool hasSegmentMarkers = polygon.HasSegmentMarkers;
			using (StreamWriter streamWriter = new StreamWriter(filename))
			{
				streamWriter.WriteLine("{0} 2 0 {1}", polygon.Points.Count, polygon.HasPointMarkers ? "1" : "0");
				this.WriteNodes(streamWriter, polygon.Points, polygon.HasPointMarkers, 0, false);
				streamWriter.WriteLine("{0} {1}", polygon.Segments.Count, hasSegmentMarkers ? "1" : "0");
				int num = 0;
				foreach (ISegment segment in polygon.Segments)
				{
					Vertex vertex = segment.GetVertex(0);
					Vertex vertex2 = segment.GetVertex(1);
					if (hasSegmentMarkers)
					{
						streamWriter.WriteLine("{0} {1} {2} {3}", new object[]
						{
							num,
							vertex.ID,
							vertex2.ID,
							segment.Label
						});
					}
					else
					{
						streamWriter.WriteLine("{0} {1} {2}", num, vertex.ID, vertex2.ID);
					}
					num++;
				}
				num = 0;
				streamWriter.WriteLine("{0}", polygon.Holes.Count);
				foreach (Point point in polygon.Holes)
				{
					streamWriter.WriteLine("{0} {1} {2}", num++, point.X.ToString(TriangleWriter.nfi), point.Y.ToString(TriangleWriter.nfi));
				}
				if (polygon.Regions.Count > 0)
				{
					num = 0;
					streamWriter.WriteLine("{0}", polygon.Regions.Count);
					foreach (RegionPointer regionPointer in polygon.Regions)
					{
						streamWriter.WriteLine("{0} {1} {2} {3}", new object[]
						{
							num,
							regionPointer.point.X.ToString(TriangleWriter.nfi),
							regionPointer.point.Y.ToString(TriangleWriter.nfi),
							regionPointer.id
						});
						num++;
					}
				}
			}
		}

		// Token: 0x06000AF6 RID: 2806 RVA: 0x0004A0B7 File Offset: 0x000482B7
		public void WritePoly(Mesh mesh, string filename)
		{
			this.WritePoly(mesh, filename, true);
		}

		// Token: 0x06000AF7 RID: 2807 RVA: 0x000CB69C File Offset: 0x000C989C
		public void WritePoly(Mesh mesh, string filename, bool writeNodes)
		{
			Osub osub = default(Osub);
			bool useBoundaryMarkers = mesh.behavior.UseBoundaryMarkers;
			using (StreamWriter streamWriter = new StreamWriter(filename))
			{
				if (writeNodes)
				{
					this.WriteNodes(streamWriter, mesh);
				}
				else
				{
					streamWriter.WriteLine("0 {0} {1} {2}", mesh.mesh_dim, mesh.nextras, useBoundaryMarkers ? "1" : "0");
				}
				streamWriter.WriteLine("{0} {1}", mesh.subsegs.Count, useBoundaryMarkers ? "1" : "0");
				osub.orient = 0;
				int num = 0;
				foreach (SubSegment seg in mesh.subsegs.Values)
				{
					osub.seg = seg;
					Vertex vertex = osub.Org();
					Vertex vertex2 = osub.Dest();
					if (useBoundaryMarkers)
					{
						streamWriter.WriteLine("{0} {1} {2} {3}", new object[]
						{
							num,
							vertex.id,
							vertex2.id,
							osub.seg.boundary
						});
					}
					else
					{
						streamWriter.WriteLine("{0} {1} {2}", num, vertex.id, vertex2.id);
					}
					num++;
				}
				num = 0;
				streamWriter.WriteLine("{0}", mesh.holes.Count);
				foreach (Point point in mesh.holes)
				{
					streamWriter.WriteLine("{0} {1} {2}", num++, point.X.ToString(TriangleWriter.nfi), point.Y.ToString(TriangleWriter.nfi));
				}
				if (mesh.regions.Count > 0)
				{
					num = 0;
					streamWriter.WriteLine("{0}", mesh.regions.Count);
					foreach (RegionPointer regionPointer in mesh.regions)
					{
						streamWriter.WriteLine("{0} {1} {2} {3}", new object[]
						{
							num,
							regionPointer.point.X.ToString(TriangleWriter.nfi),
							regionPointer.point.Y.ToString(TriangleWriter.nfi),
							regionPointer.id
						});
						num++;
					}
				}
			}
		}

		// Token: 0x06000AF8 RID: 2808 RVA: 0x000CB9E0 File Offset: 0x000C9BE0
		public void WriteEdges(Mesh mesh, string filename)
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Osub osub = default(Osub);
			Behavior behavior = mesh.behavior;
			using (StreamWriter streamWriter = new StreamWriter(filename))
			{
				streamWriter.WriteLine("{0} {1}", mesh.NumberOfEdges, behavior.UseBoundaryMarkers ? "1" : "0");
				long num = 0L;
				foreach (Triangle tri in mesh.triangles)
				{
					otri.tri = tri;
					otri.orient = 0;
					while (otri.orient < 3)
					{
						otri.Sym(ref otri2);
						if (otri.tri.id < otri2.tri.id || otri2.tri.id == -1)
						{
							Vertex vertex = otri.Org();
							Vertex vertex2 = otri.Dest();
							if (behavior.UseBoundaryMarkers)
							{
								if (behavior.useSegments)
								{
									otri.Pivot(ref osub);
									if (osub.seg.hash == -1)
									{
										streamWriter.WriteLine("{0} {1} {2} {3}", new object[]
										{
											num,
											vertex.id,
											vertex2.id,
											0
										});
									}
									else
									{
										streamWriter.WriteLine("{0} {1} {2} {3}", new object[]
										{
											num,
											vertex.id,
											vertex2.id,
											osub.seg.boundary
										});
									}
								}
								else
								{
									streamWriter.WriteLine("{0} {1} {2} {3}", new object[]
									{
										num,
										vertex.id,
										vertex2.id,
										(otri2.tri.id == -1) ? "1" : "0"
									});
								}
							}
							else
							{
								streamWriter.WriteLine("{0} {1} {2}", num, vertex.id, vertex2.id);
							}
							num += 1L;
						}
						otri.orient++;
					}
				}
			}
		}

		// Token: 0x06000AF9 RID: 2809 RVA: 0x000CBC7C File Offset: 0x000C9E7C
		public void WriteNeighbors(Mesh mesh, string filename)
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			int num = 0;
			using (StreamWriter streamWriter = new StreamWriter(filename))
			{
				streamWriter.WriteLine("{0} 3", mesh.triangles.Count);
				foreach (Triangle tri in mesh.triangles)
				{
					otri.tri = tri;
					otri.orient = 1;
					otri.Sym(ref otri2);
					int id = otri2.tri.id;
					otri.orient = 2;
					otri.Sym(ref otri2);
					int id2 = otri2.tri.id;
					otri.orient = 0;
					otri.Sym(ref otri2);
					int id3 = otri2.tri.id;
					streamWriter.WriteLine("{0} {1} {2} {3}", new object[]
					{
						num++,
						id,
						id2,
						id3
					});
				}
			}
		}

		// Token: 0x04000B04 RID: 2820
		private static NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;
	}
}
