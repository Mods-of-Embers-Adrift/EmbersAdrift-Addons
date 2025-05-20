using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Meshing;

namespace TriangleNet.IO
{
	// Token: 0x0200013A RID: 314
	public static class FileProcessor
	{
		// Token: 0x06000ABD RID: 2749 RVA: 0x00049F90 File Offset: 0x00048190
		static FileProcessor()
		{
			FileProcessor.formats.Add(new TriangleFormat());
		}

		// Token: 0x06000ABE RID: 2750 RVA: 0x00049FAB File Offset: 0x000481AB
		public static void Add(IFileFormat format)
		{
			FileProcessor.formats.Add(format);
		}

		// Token: 0x06000ABF RID: 2751 RVA: 0x000CA2E4 File Offset: 0x000C84E4
		public static bool IsSupported(string file)
		{
			using (List<IFileFormat>.Enumerator enumerator = FileProcessor.formats.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsSupported(file))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000AC0 RID: 2752 RVA: 0x000CA340 File Offset: 0x000C8540
		public static IPolygon Read(string filename)
		{
			foreach (IFileFormat fileFormat in FileProcessor.formats)
			{
				IPolygonFormat polygonFormat = (IPolygonFormat)fileFormat;
				if (polygonFormat != null && polygonFormat.IsSupported(filename))
				{
					return polygonFormat.Read(filename);
				}
			}
			throw new Exception("File format not supported.");
		}

		// Token: 0x06000AC1 RID: 2753 RVA: 0x000CA3B4 File Offset: 0x000C85B4
		public static void Write(IPolygon polygon, string filename)
		{
			foreach (IFileFormat fileFormat in FileProcessor.formats)
			{
				IPolygonFormat polygonFormat = (IPolygonFormat)fileFormat;
				if (polygonFormat != null && polygonFormat.IsSupported(filename))
				{
					polygonFormat.Write(polygon, filename);
					return;
				}
			}
			throw new Exception("File format not supported.");
		}

		// Token: 0x06000AC2 RID: 2754 RVA: 0x000CA428 File Offset: 0x000C8628
		public static IMesh Import(string filename)
		{
			foreach (IFileFormat fileFormat in FileProcessor.formats)
			{
				IMeshFormat meshFormat = (IMeshFormat)fileFormat;
				if (meshFormat != null && meshFormat.IsSupported(filename))
				{
					return meshFormat.Import(filename);
				}
			}
			throw new Exception("File format not supported.");
		}

		// Token: 0x06000AC3 RID: 2755 RVA: 0x000CA49C File Offset: 0x000C869C
		public static void Write(IMesh mesh, string filename)
		{
			foreach (IFileFormat fileFormat in FileProcessor.formats)
			{
				IMeshFormat meshFormat = (IMeshFormat)fileFormat;
				if (meshFormat != null && meshFormat.IsSupported(filename))
				{
					meshFormat.Write(mesh, filename);
					return;
				}
			}
			throw new Exception("File format not supported.");
		}

		// Token: 0x04000AFE RID: 2814
		private static List<IFileFormat> formats = new List<IFileFormat>();
	}
}
