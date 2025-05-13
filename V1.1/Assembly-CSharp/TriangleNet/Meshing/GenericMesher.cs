using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.IO;
using TriangleNet.Meshing.Algorithm;

namespace TriangleNet.Meshing
{
	// Token: 0x0200011D RID: 285
	public class GenericMesher
	{
		// Token: 0x06000A25 RID: 2597 RVA: 0x00049BB0 File Offset: 0x00047DB0
		public GenericMesher() : this(new Dwyer(), new Configuration())
		{
		}

		// Token: 0x06000A26 RID: 2598 RVA: 0x00049BC2 File Offset: 0x00047DC2
		public GenericMesher(ITriangulator triangulator) : this(triangulator, new Configuration())
		{
		}

		// Token: 0x06000A27 RID: 2599 RVA: 0x00049BD0 File Offset: 0x00047DD0
		public GenericMesher(Configuration config) : this(new Dwyer(), config)
		{
		}

		// Token: 0x06000A28 RID: 2600 RVA: 0x00049BDE File Offset: 0x00047DDE
		public GenericMesher(ITriangulator triangulator, Configuration config)
		{
			this.config = config;
			this.triangulator = triangulator;
		}

		// Token: 0x06000A29 RID: 2601 RVA: 0x00049BF4 File Offset: 0x00047DF4
		public IMesh Triangulate(IList<Vertex> points)
		{
			return this.triangulator.Triangulate(points, this.config);
		}

		// Token: 0x06000A2A RID: 2602 RVA: 0x00049C08 File Offset: 0x00047E08
		public IMesh Triangulate(IPolygon polygon)
		{
			return this.Triangulate(polygon, null, null);
		}

		// Token: 0x06000A2B RID: 2603 RVA: 0x00049C13 File Offset: 0x00047E13
		public IMesh Triangulate(IPolygon polygon, ConstraintOptions options)
		{
			return this.Triangulate(polygon, options, null);
		}

		// Token: 0x06000A2C RID: 2604 RVA: 0x00049C1E File Offset: 0x00047E1E
		public IMesh Triangulate(IPolygon polygon, QualityOptions quality)
		{
			return this.Triangulate(polygon, null, quality);
		}

		// Token: 0x06000A2D RID: 2605 RVA: 0x000C5BC4 File Offset: 0x000C3DC4
		public IMesh Triangulate(IPolygon polygon, ConstraintOptions options, QualityOptions quality)
		{
			Mesh mesh = (Mesh)this.triangulator.Triangulate(polygon.Points, this.config);
			ConstraintMesher constraintMesher = new ConstraintMesher(mesh, this.config);
			QualityMesher qualityMesher = new QualityMesher(mesh, this.config);
			mesh.SetQualityMesher(qualityMesher);
			constraintMesher.Apply(polygon, options);
			qualityMesher.Apply(quality, false);
			return mesh;
		}

		// Token: 0x06000A2E RID: 2606 RVA: 0x000C5C20 File Offset: 0x000C3E20
		public static IMesh StructuredMesh(double width, double height, int nx, int ny)
		{
			if (width <= 0.0)
			{
				throw new ArgumentException("width");
			}
			if (height <= 0.0)
			{
				throw new ArgumentException("height");
			}
			return GenericMesher.StructuredMesh(new Rectangle(0.0, 0.0, width, height), nx, ny);
		}

		// Token: 0x06000A2F RID: 2607 RVA: 0x000C5C7C File Offset: 0x000C3E7C
		public static IMesh StructuredMesh(Rectangle bounds, int nx, int ny)
		{
			Polygon polygon = new Polygon((nx + 1) * (ny + 1));
			double num = bounds.Width / (double)nx;
			double num2 = bounds.Height / (double)ny;
			double left = bounds.Left;
			double bottom = bounds.Bottom;
			int num3 = 0;
			Vertex[] array = new Vertex[(nx + 1) * (ny + 1)];
			for (int i = 0; i <= nx; i++)
			{
				double x = left + (double)i * num;
				for (int j = 0; j <= ny; j++)
				{
					double y = bottom + (double)j * num2;
					array[num3++] = new Vertex(x, y);
				}
			}
			polygon.Points.AddRange(array);
			num3 = 0;
			foreach (Vertex vertex in array)
			{
				vertex.hash = (vertex.id = num3++);
			}
			List<ISegment> segments = polygon.Segments;
			segments.Capacity = 2 * (nx + ny);
			for (int j = 0; j < ny; j++)
			{
				Vertex vertex2 = array[j];
				Vertex vertex3 = array[j + 1];
				segments.Add(new Segment(vertex2, vertex3, 1));
				vertex2.Label = (vertex3.Label = 1);
				vertex2 = array[nx * (ny + 1) + j];
				vertex3 = array[nx * (ny + 1) + (j + 1)];
				segments.Add(new Segment(vertex2, vertex3, 1));
				vertex2.Label = (vertex3.Label = 1);
			}
			for (int i = 0; i < nx; i++)
			{
				Vertex vertex2 = array[(ny + 1) * i];
				Vertex vertex3 = array[(ny + 1) * (i + 1)];
				segments.Add(new Segment(vertex2, vertex3, 1));
				vertex2.Label = (vertex3.Label = 1);
				vertex2 = array[ny + (ny + 1) * i];
				vertex3 = array[ny + (ny + 1) * (i + 1)];
				segments.Add(new Segment(vertex2, vertex3, 1));
				vertex2.Label = (vertex3.Label = 1);
			}
			InputTriangle[] array3 = new InputTriangle[2 * nx * ny];
			num3 = 0;
			for (int i = 0; i < nx; i++)
			{
				for (int j = 0; j < ny; j++)
				{
					int num4 = j + (ny + 1) * i;
					int num5 = j + (ny + 1) * (i + 1);
					if ((i + j) % 2 == 0)
					{
						array3[num3++] = new InputTriangle(num4, num5, num5 + 1);
						array3[num3++] = new InputTriangle(num4, num5 + 1, num4 + 1);
					}
					else
					{
						array3[num3++] = new InputTriangle(num4, num5, num4 + 1);
						array3[num3++] = new InputTriangle(num5, num5 + 1, num4 + 1);
					}
				}
			}
			Polygon polygon2 = polygon;
			ITriangle[] triangles = array3;
			return Converter.ToMesh(polygon2, triangles);
		}

		// Token: 0x04000A9F RID: 2719
		private Configuration config;

		// Token: 0x04000AA0 RID: 2720
		private ITriangulator triangulator;
	}
}
