using System;
using TriangleNet.Geometry;
using TriangleNet.Logging;
using TriangleNet.Topology;

namespace TriangleNet
{
	// Token: 0x020000ED RID: 237
	public static class MeshValidator
	{
		// Token: 0x0600086D RID: 2157 RVA: 0x000B263C File Offset: 0x000B083C
		public static bool IsConsistent(Mesh mesh)
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Otri otri3 = default(Otri);
			ILog<LogItem> instance = Log.Instance;
			bool noExact = Behavior.NoExact;
			Behavior.NoExact = false;
			int num = 0;
			foreach (Triangle triangle in mesh.triangles)
			{
				otri.tri = triangle;
				otri.orient = 0;
				while (otri.orient < 3)
				{
					Vertex vertex = otri.Org();
					Vertex vertex2 = otri.Dest();
					if (otri.orient == 0)
					{
						Vertex pc = otri.Apex();
						if (MeshValidator.predicates.CounterClockwise(vertex, vertex2, pc) <= 0.0)
						{
							if (Log.Verbose)
							{
								instance.Warning(string.Format("Triangle is flat or inverted (ID {0}).", triangle.id), "MeshValidator.IsConsistent()");
							}
							num++;
						}
					}
					otri.Sym(ref otri2);
					if (otri2.tri.id != -1)
					{
						otri2.Sym(ref otri3);
						if (otri.tri != otri3.tri || otri.orient != otri3.orient)
						{
							if (otri.tri == otri3.tri && Log.Verbose)
							{
								instance.Warning("Asymmetric triangle-triangle bond: (Right triangle, wrong orientation)", "MeshValidator.IsConsistent()");
							}
							num++;
						}
						Vertex b = otri2.Org();
						Vertex b2 = otri2.Dest();
						if (vertex != b2 || vertex2 != b)
						{
							if (Log.Verbose)
							{
								instance.Warning("Mismatched edge coordinates between two triangles.", "MeshValidator.IsConsistent()");
							}
							num++;
						}
					}
					otri.orient++;
				}
			}
			mesh.MakeVertexMap();
			foreach (Vertex vertex3 in mesh.vertices.Values)
			{
				if (vertex3.tri.tri == null && Log.Verbose)
				{
					instance.Warning("Vertex (ID " + vertex3.id.ToString() + ") not connected to mesh (duplicate input vertex?)", "MeshValidator.IsConsistent()");
				}
			}
			Behavior.NoExact = noExact;
			return num == 0;
		}

		// Token: 0x0600086E RID: 2158 RVA: 0x0004898E File Offset: 0x00046B8E
		public static bool IsDelaunay(Mesh mesh)
		{
			return MeshValidator.IsDelaunay(mesh, false);
		}

		// Token: 0x0600086F RID: 2159 RVA: 0x00048997 File Offset: 0x00046B97
		public static bool IsConstrainedDelaunay(Mesh mesh)
		{
			return MeshValidator.IsDelaunay(mesh, true);
		}

		// Token: 0x06000870 RID: 2160 RVA: 0x000B28B0 File Offset: 0x000B0AB0
		private static bool IsDelaunay(Mesh mesh, bool constrained)
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Osub osub = default(Osub);
			ILog<LogItem> instance = Log.Instance;
			bool noExact = Behavior.NoExact;
			Behavior.NoExact = false;
			int num = 0;
			Vertex infvertex = mesh.infvertex1;
			Vertex infvertex2 = mesh.infvertex2;
			Vertex infvertex3 = mesh.infvertex3;
			foreach (Triangle tri in mesh.triangles)
			{
				otri.tri = tri;
				otri.orient = 0;
				while (otri.orient < 3)
				{
					Vertex vertex = otri.Org();
					Vertex vertex2 = otri.Dest();
					Vertex vertex3 = otri.Apex();
					otri.Sym(ref otri2);
					Vertex vertex4 = otri2.Apex();
					bool flag = otri.tri.id < otri2.tri.id && !Otri.IsDead(otri2.tri) && otri2.tri.id != -1 && vertex != infvertex && vertex != infvertex2 && vertex != infvertex3 && vertex2 != infvertex && vertex2 != infvertex2 && vertex2 != infvertex3 && vertex3 != infvertex && vertex3 != infvertex2 && vertex3 != infvertex3 && vertex4 != infvertex && vertex4 != infvertex2 && vertex4 != infvertex3;
					if (constrained && mesh.checksegments && flag)
					{
						otri.Pivot(ref osub);
						if (osub.seg.hash != -1)
						{
							flag = false;
						}
					}
					if (flag && MeshValidator.predicates.NonRegular(vertex, vertex2, vertex3, vertex4) > 0.0)
					{
						if (Log.Verbose)
						{
							instance.Warning(string.Format("Non-regular pair of triangles found (IDs {0}/{1}).", otri.tri.id, otri2.tri.id), "MeshValidator.IsDelaunay()");
						}
						num++;
					}
					otri.orient++;
				}
			}
			Behavior.NoExact = noExact;
			return num == 0;
		}

		// Token: 0x040009B2 RID: 2482
		private static RobustPredicates predicates = RobustPredicates.Default;
	}
}
