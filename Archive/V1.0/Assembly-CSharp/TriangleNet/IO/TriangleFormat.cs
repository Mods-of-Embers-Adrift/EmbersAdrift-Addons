using System;
using System.Collections.Generic;
using System.IO;
using TriangleNet.Geometry;
using TriangleNet.Meshing;

namespace TriangleNet.IO
{
	// Token: 0x0200013F RID: 319
	public class TriangleFormat : IPolygonFormat, IFileFormat, IMeshFormat
	{
		// Token: 0x06000AD7 RID: 2775 RVA: 0x000CA4F0 File Offset: 0x000C86F0
		public bool IsSupported(string file)
		{
			string a = Path.GetExtension(file).ToLower();
			return a == ".node" || a == ".poly" || a == ".ele";
		}

		// Token: 0x06000AD8 RID: 2776 RVA: 0x000CA534 File Offset: 0x000C8734
		public IMesh Import(string filename)
		{
			string extension = Path.GetExtension(filename);
			if (extension == ".node" || extension == ".poly" || extension == ".ele")
			{
				Polygon polygon;
				List<ITriangle> list;
				new TriangleReader().Read(filename, out polygon, out list);
				if (polygon != null && list != null)
				{
					return Converter.ToMesh(polygon, list.ToArray());
				}
			}
			throw new NotSupportedException("Could not load '" + filename + "' file.");
		}

		// Token: 0x06000AD9 RID: 2777 RVA: 0x0004A007 File Offset: 0x00048207
		public void Write(IMesh mesh, string filename)
		{
			TriangleWriter triangleWriter = new TriangleWriter();
			triangleWriter.WritePoly((Mesh)mesh, Path.ChangeExtension(filename, ".poly"));
			triangleWriter.WriteElements((Mesh)mesh, Path.ChangeExtension(filename, ".ele"));
		}

		// Token: 0x06000ADA RID: 2778 RVA: 0x00048A92 File Offset: 0x00046C92
		public void Write(IMesh mesh, Stream stream)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000ADB RID: 2779 RVA: 0x000CA5A8 File Offset: 0x000C87A8
		public IPolygon Read(string filename)
		{
			string extension = Path.GetExtension(filename);
			if (extension == ".node")
			{
				return new TriangleReader().ReadNodeFile(filename);
			}
			if (extension == ".poly")
			{
				return new TriangleReader().ReadPolyFile(filename);
			}
			throw new NotSupportedException("File format '" + extension + "' not supported.");
		}

		// Token: 0x06000ADC RID: 2780 RVA: 0x0004A03B File Offset: 0x0004823B
		public void Write(IPolygon polygon, string filename)
		{
			new TriangleWriter().WritePoly(polygon, filename);
		}

		// Token: 0x06000ADD RID: 2781 RVA: 0x00048A92 File Offset: 0x00046C92
		public void Write(IPolygon polygon, Stream stream)
		{
			throw new NotImplementedException();
		}
	}
}
