using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Topology;
using TriangleNet.Topology.DCEL;
using TriangleNet.Voronoi;

namespace TriangleNet.Smoothing
{
	// Token: 0x02000116 RID: 278
	public class SimpleSmoother : ISmoother
	{
		// Token: 0x060009EE RID: 2542 RVA: 0x00049A39 File Offset: 0x00047C39
		public SimpleSmoother() : this(new VoronoiFactory())
		{
		}

		// Token: 0x060009EF RID: 2543 RVA: 0x000C3770 File Offset: 0x000C1970
		public SimpleSmoother(IVoronoiFactory factory)
		{
			this.factory = factory;
			this.pool = new TrianglePool();
			this.config = new Configuration(() => RobustPredicates.Default, () => this.pool.Restart());
			this.options = new ConstraintOptions
			{
				ConformingDelaunay = true
			};
		}

		// Token: 0x060009F0 RID: 2544 RVA: 0x00049A46 File Offset: 0x00047C46
		public SimpleSmoother(IVoronoiFactory factory, Configuration config)
		{
			this.factory = factory;
			this.config = config;
			this.options = new ConstraintOptions
			{
				ConformingDelaunay = true
			};
		}

		// Token: 0x060009F1 RID: 2545 RVA: 0x00049A6E File Offset: 0x00047C6E
		public void Smooth(IMesh mesh)
		{
			this.Smooth(mesh, 10);
		}

		// Token: 0x060009F2 RID: 2546 RVA: 0x000C37E0 File Offset: 0x000C19E0
		public void Smooth(IMesh mesh, int limit)
		{
			Mesh mesh2 = (Mesh)mesh;
			GenericMesher genericMesher = new GenericMesher(this.config);
			IPredicates predicates = this.config.Predicates();
			this.options.SegmentSplitting = mesh2.behavior.NoBisect;
			for (int i = 0; i < limit; i++)
			{
				this.Step(mesh2, this.factory, predicates);
				mesh2 = (Mesh)genericMesher.Triangulate(this.Rebuild(mesh2), this.options);
				this.factory.Reset();
			}
			mesh2.CopyTo((Mesh)mesh);
		}

		// Token: 0x060009F3 RID: 2547 RVA: 0x000C3874 File Offset: 0x000C1A74
		private void Step(Mesh mesh, IVoronoiFactory factory, IPredicates predicates)
		{
			foreach (Face face in new BoundedVoronoi(mesh, factory, predicates).Faces)
			{
				if (face.generator.label == 0)
				{
					double x;
					double y;
					this.Centroid(face, out x, out y);
					face.generator.x = x;
					face.generator.y = y;
				}
			}
		}

		// Token: 0x060009F4 RID: 2548 RVA: 0x000C38F8 File Offset: 0x000C1AF8
		private void Centroid(Face face, out double x, out double y)
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			HalfEdge halfEdge = face.Edge;
			int id = halfEdge.Next.ID;
			do
			{
				Point origin = halfEdge.Origin;
				Point origin2 = halfEdge.Twin.Origin;
				double num4 = origin.x * origin2.y - origin2.x * origin.y;
				num += num4;
				num2 += (origin2.x + origin.x) * num4;
				num3 += (origin2.y + origin.y) * num4;
				halfEdge = halfEdge.Next;
			}
			while (halfEdge.Next.ID != id);
			x = num2 / (3.0 * num);
			y = num3 / (3.0 * num);
		}

		// Token: 0x060009F5 RID: 2549 RVA: 0x000C39D4 File Offset: 0x000C1BD4
		private Polygon Rebuild(Mesh mesh)
		{
			Polygon polygon = new Polygon(mesh.vertices.Count);
			foreach (TriangleNet.Geometry.Vertex vertex in mesh.vertices.Values)
			{
				vertex.type = VertexType.InputVertex;
				polygon.Points.Add(vertex);
			}
			List<ISegment> collection = new List<SubSegment>(mesh.subsegs.Values).ConvertAll<ISegment>((SubSegment x) => x);
			polygon.Segments.AddRange(collection);
			polygon.Holes.AddRange(mesh.holes);
			polygon.Regions.AddRange(mesh.regions);
			return polygon;
		}

		// Token: 0x04000A88 RID: 2696
		private TrianglePool pool;

		// Token: 0x04000A89 RID: 2697
		private Configuration config;

		// Token: 0x04000A8A RID: 2698
		private IVoronoiFactory factory;

		// Token: 0x04000A8B RID: 2699
		private ConstraintOptions options;
	}
}
