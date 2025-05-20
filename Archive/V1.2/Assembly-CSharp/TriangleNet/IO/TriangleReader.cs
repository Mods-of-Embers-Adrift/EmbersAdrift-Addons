using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TriangleNet.Geometry;

namespace TriangleNet.IO
{
	// Token: 0x02000140 RID: 320
	public class TriangleReader
	{
		// Token: 0x06000ADF RID: 2783 RVA: 0x0004A049 File Offset: 0x00048249
		public static bool IsNullOrWhiteSpace(string value)
		{
			return value == null || string.IsNullOrEmpty(value.Trim());
		}

		// Token: 0x06000AE0 RID: 2784 RVA: 0x000CA624 File Offset: 0x000C8824
		private bool TryReadLine(StreamReader reader, out string[] token)
		{
			token = null;
			if (reader.EndOfStream)
			{
				return false;
			}
			string text = reader.ReadLine().Trim();
			while (TriangleReader.IsNullOrWhiteSpace(text) || text.StartsWith("#"))
			{
				if (reader.EndOfStream)
				{
					return false;
				}
				text = reader.ReadLine().Trim();
			}
			token = text.Split(new char[]
			{
				' ',
				'\t'
			}, StringSplitOptions.RemoveEmptyEntries);
			return true;
		}

		// Token: 0x06000AE1 RID: 2785 RVA: 0x000CA694 File Offset: 0x000C8894
		private void ReadVertex(List<Vertex> data, int index, string[] line, int attributes, int marks)
		{
			double x = double.Parse(line[1], TriangleReader.nfi);
			double y = double.Parse(line[2], TriangleReader.nfi);
			Vertex vertex = new Vertex(x, y);
			if (marks > 0 && line.Length > 3 + attributes)
			{
				vertex.Label = int.Parse(line[3 + attributes]);
			}
			data.Add(vertex);
		}

		// Token: 0x06000AE2 RID: 2786 RVA: 0x000CA6F0 File Offset: 0x000C88F0
		public void Read(string filename, out Polygon polygon)
		{
			polygon = null;
			string text = Path.ChangeExtension(filename, ".poly");
			if (File.Exists(text))
			{
				polygon = this.ReadPolyFile(text);
				return;
			}
			text = Path.ChangeExtension(filename, ".node");
			polygon = this.ReadNodeFile(text);
		}

		// Token: 0x06000AE3 RID: 2787 RVA: 0x000CA734 File Offset: 0x000C8934
		public void Read(string filename, out Polygon geometry, out List<ITriangle> triangles)
		{
			triangles = null;
			this.Read(filename, out geometry);
			string text = Path.ChangeExtension(filename, ".ele");
			if (File.Exists(text) && geometry != null)
			{
				triangles = this.ReadEleFile(text);
			}
		}

		// Token: 0x06000AE4 RID: 2788 RVA: 0x000CA770 File Offset: 0x000C8970
		public IPolygon Read(string filename)
		{
			Polygon result = null;
			this.Read(filename, out result);
			return result;
		}

		// Token: 0x06000AE5 RID: 2789 RVA: 0x0004A05B File Offset: 0x0004825B
		public Polygon ReadNodeFile(string nodefilename)
		{
			return this.ReadNodeFile(nodefilename, false);
		}

		// Token: 0x06000AE6 RID: 2790 RVA: 0x000CA78C File Offset: 0x000C898C
		public Polygon ReadNodeFile(string nodefilename, bool readElements)
		{
			this.startIndex = 0;
			int attributes = 0;
			int marks = 0;
			Polygon polygon;
			using (StreamReader streamReader = new StreamReader(nodefilename))
			{
				string[] array;
				if (!this.TryReadLine(streamReader, out array))
				{
					throw new Exception("Can't read input file.");
				}
				int num = int.Parse(array[0]);
				if (num < 3)
				{
					throw new Exception("Input must have at least three input vertices.");
				}
				if (array.Length > 1 && int.Parse(array[1]) != 2)
				{
					throw new Exception("Triangle only works with two-dimensional meshes.");
				}
				if (array.Length > 2)
				{
					attributes = int.Parse(array[2]);
				}
				if (array.Length > 3)
				{
					marks = int.Parse(array[3]);
				}
				polygon = new Polygon(num);
				if (num > 0)
				{
					for (int i = 0; i < num; i++)
					{
						if (!this.TryReadLine(streamReader, out array))
						{
							throw new Exception("Can't read input file (vertices).");
						}
						if (array.Length < 3)
						{
							throw new Exception("Invalid vertex.");
						}
						if (i == 0)
						{
							this.startIndex = int.Parse(array[0], TriangleReader.nfi);
						}
						this.ReadVertex(polygon.Points, i, array, attributes, marks);
					}
				}
			}
			if (readElements)
			{
				string text = Path.ChangeExtension(nodefilename, ".ele");
				if (File.Exists(text))
				{
					this.ReadEleFile(text, true);
				}
			}
			return polygon;
		}

		// Token: 0x06000AE7 RID: 2791 RVA: 0x0004A065 File Offset: 0x00048265
		public Polygon ReadPolyFile(string polyfilename)
		{
			return this.ReadPolyFile(polyfilename, false, false);
		}

		// Token: 0x06000AE8 RID: 2792 RVA: 0x0004A070 File Offset: 0x00048270
		public Polygon ReadPolyFile(string polyfilename, bool readElements)
		{
			return this.ReadPolyFile(polyfilename, readElements, false);
		}

		// Token: 0x06000AE9 RID: 2793 RVA: 0x000CA8C8 File Offset: 0x000C8AC8
		public Polygon ReadPolyFile(string polyfilename, bool readElements, bool readArea)
		{
			this.startIndex = 0;
			int attributes = 0;
			int marks = 0;
			Polygon polygon;
			using (StreamReader streamReader = new StreamReader(polyfilename))
			{
				string[] array;
				if (!this.TryReadLine(streamReader, out array))
				{
					throw new Exception("Can't read input file.");
				}
				int num = int.Parse(array[0]);
				if (array.Length > 1 && int.Parse(array[1]) != 2)
				{
					throw new Exception("Triangle only works with two-dimensional meshes.");
				}
				if (array.Length > 2)
				{
					attributes = int.Parse(array[2]);
				}
				if (array.Length > 3)
				{
					marks = int.Parse(array[3]);
				}
				if (num > 0)
				{
					polygon = new Polygon(num);
					for (int i = 0; i < num; i++)
					{
						if (!this.TryReadLine(streamReader, out array))
						{
							throw new Exception("Can't read input file (vertices).");
						}
						if (array.Length < 3)
						{
							throw new Exception("Invalid vertex.");
						}
						if (i == 0)
						{
							this.startIndex = int.Parse(array[0], TriangleReader.nfi);
						}
						this.ReadVertex(polygon.Points, i, array, attributes, marks);
					}
				}
				else
				{
					polygon = this.ReadNodeFile(Path.ChangeExtension(polyfilename, ".node"));
					num = polygon.Points.Count;
				}
				List<Vertex> points = polygon.Points;
				if (points.Count == 0)
				{
					throw new Exception("No nodes available.");
				}
				if (!this.TryReadLine(streamReader, out array))
				{
					throw new Exception("Can't read input file (segments).");
				}
				int num2 = int.Parse(array[0]);
				int num3 = 0;
				if (array.Length > 1)
				{
					num3 = int.Parse(array[1]);
				}
				for (int j = 0; j < num2; j++)
				{
					if (!this.TryReadLine(streamReader, out array))
					{
						throw new Exception("Can't read input file (segments).");
					}
					if (array.Length < 3)
					{
						throw new Exception("Segment has no endpoints.");
					}
					int num4 = int.Parse(array[1]) - this.startIndex;
					int num5 = int.Parse(array[2]) - this.startIndex;
					int label = 0;
					if (num3 > 0 && array.Length > 3)
					{
						label = int.Parse(array[3]);
					}
					if (num4 < 0 || num4 >= num)
					{
						if (Log.Verbose)
						{
							Log.Instance.Warning("Invalid first endpoint of segment.", "MeshReader.ReadPolyfile()");
						}
					}
					else if (num5 < 0 || num5 >= num)
					{
						if (Log.Verbose)
						{
							Log.Instance.Warning("Invalid second endpoint of segment.", "MeshReader.ReadPolyfile()");
						}
					}
					else
					{
						polygon.Add(new Segment(points[num4], points[num5], label), false);
					}
				}
				if (!this.TryReadLine(streamReader, out array))
				{
					throw new Exception("Can't read input file (holes).");
				}
				int num6 = int.Parse(array[0]);
				if (num6 > 0)
				{
					for (int k = 0; k < num6; k++)
					{
						if (!this.TryReadLine(streamReader, out array))
						{
							throw new Exception("Can't read input file (holes).");
						}
						if (array.Length < 3)
						{
							throw new Exception("Invalid hole.");
						}
						polygon.Holes.Add(new Point(double.Parse(array[1], TriangleReader.nfi), double.Parse(array[2], TriangleReader.nfi)));
					}
				}
				if (this.TryReadLine(streamReader, out array))
				{
					int num7 = int.Parse(array[0]);
					if (num7 > 0)
					{
						for (int l = 0; l < num7; l++)
						{
							if (!this.TryReadLine(streamReader, out array))
							{
								throw new Exception("Can't read input file (region).");
							}
							if (array.Length < 4)
							{
								throw new Exception("Invalid region attributes.");
							}
							int id;
							if (!int.TryParse(array[3], out id))
							{
								id = l;
							}
							double area = 0.0;
							if (array.Length > 4)
							{
								double.TryParse(array[4], NumberStyles.Number, TriangleReader.nfi, out area);
							}
							polygon.Regions.Add(new RegionPointer(double.Parse(array[1], TriangleReader.nfi), double.Parse(array[2], TriangleReader.nfi), id, area));
						}
					}
				}
			}
			if (readElements)
			{
				string text = Path.ChangeExtension(polyfilename, ".ele");
				if (File.Exists(text))
				{
					this.ReadEleFile(text, readArea);
				}
			}
			return polygon;
		}

		// Token: 0x06000AEA RID: 2794 RVA: 0x0004A07B File Offset: 0x0004827B
		public List<ITriangle> ReadEleFile(string elefilename)
		{
			return this.ReadEleFile(elefilename, false);
		}

		// Token: 0x06000AEB RID: 2795 RVA: 0x000CACA4 File Offset: 0x000C8EA4
		private List<ITriangle> ReadEleFile(string elefilename, bool readArea)
		{
			int num = 0;
			List<ITriangle> list;
			using (StreamReader streamReader = new StreamReader(elefilename))
			{
				bool flag = false;
				string[] array;
				if (!this.TryReadLine(streamReader, out array))
				{
					throw new Exception("Can't read input file (elements).");
				}
				num = int.Parse(array[0]);
				int num2 = 0;
				if (array.Length > 2)
				{
					num2 = int.Parse(array[2]);
					flag = true;
				}
				if (num2 > 1)
				{
					Log.Instance.Warning("Triangle attributes not supported.", "FileReader.Read");
				}
				list = new List<ITriangle>(num);
				for (int i = 0; i < num; i++)
				{
					if (!this.TryReadLine(streamReader, out array))
					{
						throw new Exception("Can't read input file (elements).");
					}
					if (array.Length < 4)
					{
						throw new Exception("Triangle has no nodes.");
					}
					InputTriangle inputTriangle = new InputTriangle(int.Parse(array[1]) - this.startIndex, int.Parse(array[2]) - this.startIndex, int.Parse(array[3]) - this.startIndex);
					if (num2 > 0 && flag)
					{
						int label = 0;
						flag = int.TryParse(array[4], out label);
						inputTriangle.label = label;
					}
					list.Add(inputTriangle);
				}
			}
			if (readArea)
			{
				string text = Path.ChangeExtension(elefilename, ".area");
				if (File.Exists(text))
				{
					this.ReadAreaFile(text, num);
				}
			}
			return list;
		}

		// Token: 0x06000AEC RID: 2796 RVA: 0x000CADF4 File Offset: 0x000C8FF4
		private double[] ReadAreaFile(string areafilename, int intriangles)
		{
			double[] array = null;
			using (StreamReader streamReader = new StreamReader(areafilename))
			{
				string[] array2;
				if (!this.TryReadLine(streamReader, out array2))
				{
					throw new Exception("Can't read input file (area).");
				}
				if (int.Parse(array2[0]) != intriangles)
				{
					Log.Instance.Warning("Number of area constraints doesn't match number of triangles.", "ReadAreaFile()");
					return null;
				}
				array = new double[intriangles];
				for (int i = 0; i < intriangles; i++)
				{
					if (!this.TryReadLine(streamReader, out array2))
					{
						throw new Exception("Can't read input file (area).");
					}
					if (array2.Length != 2)
					{
						throw new Exception("Triangle has no nodes.");
					}
					array[i] = double.Parse(array2[1], TriangleReader.nfi);
				}
			}
			return array;
		}

		// Token: 0x06000AED RID: 2797 RVA: 0x000CAEB4 File Offset: 0x000C90B4
		public List<Edge> ReadEdgeFile(string edgeFile, int invertices)
		{
			List<Edge> list = null;
			this.startIndex = 0;
			using (StreamReader streamReader = new StreamReader(edgeFile))
			{
				string[] array;
				if (!this.TryReadLine(streamReader, out array))
				{
					throw new Exception("Can't read input file (segments).");
				}
				int num = int.Parse(array[0]);
				int num2 = 0;
				if (array.Length > 1)
				{
					num2 = int.Parse(array[1]);
				}
				if (num > 0)
				{
					list = new List<Edge>(num);
				}
				for (int i = 0; i < num; i++)
				{
					if (!this.TryReadLine(streamReader, out array))
					{
						throw new Exception("Can't read input file (segments).");
					}
					if (array.Length < 3)
					{
						throw new Exception("Segment has no endpoints.");
					}
					int num3 = int.Parse(array[1]) - this.startIndex;
					int num4 = int.Parse(array[2]) - this.startIndex;
					int label = 0;
					if (num2 > 0 && array.Length > 3)
					{
						label = int.Parse(array[3]);
					}
					if (num3 < 0 || num3 >= invertices)
					{
						if (Log.Verbose)
						{
							Log.Instance.Warning("Invalid first endpoint of segment.", "MeshReader.ReadPolyfile()");
						}
					}
					else if (num4 < 0 || num4 >= invertices)
					{
						if (Log.Verbose)
						{
							Log.Instance.Warning("Invalid second endpoint of segment.", "MeshReader.ReadPolyfile()");
						}
					}
					else
					{
						list.Add(new Edge(num3, num4, label));
					}
				}
			}
			return list;
		}

		// Token: 0x04000B02 RID: 2818
		private static NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;

		// Token: 0x04000B03 RID: 2819
		private int startIndex;
	}
}
