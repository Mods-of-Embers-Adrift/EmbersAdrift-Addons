using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.IO
{
	// Token: 0x02000139 RID: 313
	internal class DebugWriter
	{
		// Token: 0x06000AB3 RID: 2739 RVA: 0x00044765 File Offset: 0x00042965
		private DebugWriter()
		{
		}

		// Token: 0x1700033C RID: 828
		// (get) Token: 0x06000AB4 RID: 2740 RVA: 0x00049F17 File Offset: 0x00048117
		public static DebugWriter Session
		{
			get
			{
				return DebugWriter.instance;
			}
		}

		// Token: 0x06000AB5 RID: 2741 RVA: 0x00049F1E File Offset: 0x0004811E
		public void Start(string session)
		{
			this.iteration = 0;
			this.session = session;
			if (this.stream != null)
			{
				throw new Exception("A session is active. Finish before starting a new.");
			}
			this.tmpFile = Path.GetTempFileName();
			this.stream = new StreamWriter(this.tmpFile);
		}

		// Token: 0x06000AB6 RID: 2742 RVA: 0x00049F5D File Offset: 0x0004815D
		public void Write(Mesh mesh, bool skip = false)
		{
			this.WriteMesh(mesh, skip);
			this.triangles = mesh.Triangles.Count;
		}

		// Token: 0x06000AB7 RID: 2743 RVA: 0x00049F78 File Offset: 0x00048178
		public void Finish()
		{
			this.Finish(this.session + ".mshx");
		}

		// Token: 0x06000AB8 RID: 2744 RVA: 0x000C9CC8 File Offset: 0x000C7EC8
		private void Finish(string path)
		{
			if (this.stream != null)
			{
				this.stream.Flush();
				this.stream.Dispose();
				this.stream = null;
				string s = "#!N" + this.iteration.ToString() + Environment.NewLine;
				using (FileStream fileStream = new FileStream(path, FileMode.Create))
				{
					using (GZipStream gzipStream = new GZipStream(fileStream, CompressionMode.Compress, false))
					{
						byte[] array = Encoding.UTF8.GetBytes(s);
						gzipStream.Write(array, 0, array.Length);
						array = File.ReadAllBytes(this.tmpFile);
						gzipStream.Write(array, 0, array.Length);
					}
				}
				File.Delete(this.tmpFile);
			}
		}

		// Token: 0x06000AB9 RID: 2745 RVA: 0x000C9D94 File Offset: 0x000C7F94
		private void WriteGeometry(IPolygon geometry)
		{
			TextWriter textWriter = this.stream;
			string format = "#!G{0}";
			int num = this.iteration;
			this.iteration = num + 1;
			textWriter.WriteLine(format, num);
		}

		// Token: 0x06000ABA RID: 2746 RVA: 0x000C9DC8 File Offset: 0x000C7FC8
		private void WriteMesh(Mesh mesh, bool skip)
		{
			if (this.triangles == mesh.triangles.Count && skip)
			{
				return;
			}
			TextWriter textWriter = this.stream;
			string format = "#!M{0}";
			int num = this.iteration;
			this.iteration = num + 1;
			textWriter.WriteLine(format, num);
			if (this.VerticesChanged(mesh))
			{
				this.HashVertices(mesh);
				this.stream.WriteLine("{0}", mesh.vertices.Count);
				using (Dictionary<int, Vertex>.ValueCollection.Enumerator enumerator = mesh.vertices.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Vertex vertex = enumerator.Current;
						this.stream.WriteLine("{0} {1} {2} {3}", new object[]
						{
							vertex.id,
							vertex.x.ToString(DebugWriter.nfi),
							vertex.y.ToString(DebugWriter.nfi),
							vertex.label
						});
					}
					goto IL_116;
				}
			}
			this.stream.WriteLine("0");
			IL_116:
			this.stream.WriteLine("{0}", mesh.subsegs.Count);
			Osub osub = default(Osub);
			osub.orient = 0;
			foreach (SubSegment subSegment in mesh.subsegs.Values)
			{
				if (subSegment.hash > 0)
				{
					osub.seg = subSegment;
					Vertex vertex2 = osub.Org();
					Vertex vertex3 = osub.Dest();
					this.stream.WriteLine("{0} {1} {2} {3}", new object[]
					{
						osub.seg.hash,
						vertex2.id,
						vertex3.id,
						osub.seg.boundary
					});
				}
			}
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			otri.orient = 0;
			this.stream.WriteLine("{0}", mesh.triangles.Count);
			foreach (Triangle tri in mesh.triangles)
			{
				otri.tri = tri;
				Vertex vertex2 = otri.Org();
				Vertex vertex3 = otri.Dest();
				Vertex vertex4 = otri.Apex();
				int num2 = (vertex2 == null) ? -1 : vertex2.id;
				int num3 = (vertex3 == null) ? -1 : vertex3.id;
				int num4 = (vertex4 == null) ? -1 : vertex4.id;
				this.stream.Write("{0} {1} {2} {3}", new object[]
				{
					otri.tri.hash,
					num2,
					num3,
					num4
				});
				otri.orient = 1;
				otri.Sym(ref otri2);
				int hash = otri2.tri.hash;
				otri.orient = 2;
				otri.Sym(ref otri2);
				int hash2 = otri2.tri.hash;
				otri.orient = 0;
				otri.Sym(ref otri2);
				int hash3 = otri2.tri.hash;
				this.stream.WriteLine(" {0} {1} {2}", hash, hash2, hash3);
			}
		}

		// Token: 0x06000ABB RID: 2747 RVA: 0x000CA1B4 File Offset: 0x000C83B4
		private bool VerticesChanged(Mesh mesh)
		{
			if (this.vertices == null || mesh.Vertices.Count != this.vertices.Length)
			{
				return true;
			}
			int num = 0;
			using (IEnumerator<Vertex> enumerator = mesh.Vertices.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.id != this.vertices[num++])
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000ABC RID: 2748 RVA: 0x000CA234 File Offset: 0x000C8434
		private void HashVertices(Mesh mesh)
		{
			if (this.vertices == null || mesh.Vertices.Count != this.vertices.Length)
			{
				this.vertices = new int[mesh.Vertices.Count];
			}
			int num = 0;
			foreach (Vertex vertex in mesh.Vertices)
			{
				this.vertices[num++] = vertex.id;
			}
		}

		// Token: 0x04000AF6 RID: 2806
		private static NumberFormatInfo nfi = CultureInfo.InvariantCulture.NumberFormat;

		// Token: 0x04000AF7 RID: 2807
		private int iteration;

		// Token: 0x04000AF8 RID: 2808
		private string session;

		// Token: 0x04000AF9 RID: 2809
		private StreamWriter stream;

		// Token: 0x04000AFA RID: 2810
		private string tmpFile;

		// Token: 0x04000AFB RID: 2811
		private int[] vertices;

		// Token: 0x04000AFC RID: 2812
		private int triangles;

		// Token: 0x04000AFD RID: 2813
		private static readonly DebugWriter instance = new DebugWriter();
	}
}
