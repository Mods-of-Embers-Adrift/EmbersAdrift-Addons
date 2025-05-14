using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Logging;
using TriangleNet.Meshing.Iterators;
using TriangleNet.Topology;

namespace TriangleNet.Meshing
{
	// Token: 0x0200011A RID: 282
	internal class ConstraintMesher
	{
		// Token: 0x06000A0A RID: 2570 RVA: 0x000C3D04 File Offset: 0x000C1F04
		public ConstraintMesher(Mesh mesh, Configuration config)
		{
			this.mesh = mesh;
			this.predicates = config.Predicates();
			this.behavior = mesh.behavior;
			this.locator = mesh.locator;
			this.viri = new List<Triangle>();
			this.logger = Log.Instance;
		}

		// Token: 0x06000A0B RID: 2571 RVA: 0x000C3D60 File Offset: 0x000C1F60
		public void Apply(IPolygon input, ConstraintOptions options)
		{
			this.behavior.Poly = (input.Segments.Count > 0);
			if (options != null)
			{
				this.behavior.ConformingDelaunay = options.ConformingDelaunay;
				this.behavior.Convex = options.Convex;
				this.behavior.NoBisect = options.SegmentSplitting;
				if (this.behavior.ConformingDelaunay)
				{
					this.behavior.Quality = true;
				}
			}
			this.behavior.useRegions = (input.Regions.Count > 0);
			this.mesh.infvertex1 = null;
			this.mesh.infvertex2 = null;
			this.mesh.infvertex3 = null;
			if (this.behavior.useSegments)
			{
				this.mesh.checksegments = true;
				this.FormSkeleton(input);
			}
			if (this.behavior.Poly && this.mesh.triangles.Count > 0)
			{
				this.mesh.holes.AddRange(input.Holes);
				this.mesh.regions.AddRange(input.Regions);
				this.CarveHoles();
			}
		}

		// Token: 0x06000A0C RID: 2572 RVA: 0x000C3E84 File Offset: 0x000C2084
		private void CarveHoles()
		{
			Otri otri = default(Otri);
			Triangle[] array = null;
			Triangle dummytri = this.mesh.dummytri;
			if (!this.mesh.behavior.Convex)
			{
				this.InfectHull();
			}
			if (!this.mesh.behavior.NoHoles)
			{
				foreach (Point point in this.mesh.holes)
				{
					if (this.mesh.bounds.Contains(point))
					{
						otri.tri = dummytri;
						otri.orient = 0;
						otri.Sym();
						Vertex a = otri.Org();
						Vertex b = otri.Dest();
						if (this.predicates.CounterClockwise(a, b, point) > 0.0 && this.mesh.locator.Locate(point, ref otri) != LocateResult.Outside && !otri.IsInfected())
						{
							otri.Infect();
							this.viri.Add(otri.tri);
						}
					}
				}
			}
			if (this.mesh.regions.Count > 0)
			{
				int num = 0;
				array = new Triangle[this.mesh.regions.Count];
				foreach (RegionPointer regionPointer in this.mesh.regions)
				{
					array[num] = dummytri;
					if (this.mesh.bounds.Contains(regionPointer.point))
					{
						otri.tri = dummytri;
						otri.orient = 0;
						otri.Sym();
						Vertex a = otri.Org();
						Vertex b = otri.Dest();
						if (this.predicates.CounterClockwise(a, b, regionPointer.point) > 0.0 && this.mesh.locator.Locate(regionPointer.point, ref otri) != LocateResult.Outside && !otri.IsInfected())
						{
							array[num] = otri.tri;
							array[num].label = regionPointer.id;
							array[num].area = regionPointer.area;
						}
					}
					num++;
				}
			}
			if (this.viri.Count > 0)
			{
				this.Plague();
			}
			if (array != null)
			{
				RegionIterator regionIterator = new RegionIterator(this.mesh);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].id != -1 && !Otri.IsDead(array[i]))
					{
						regionIterator.Process(array[i], 0);
					}
				}
			}
			this.viri.Clear();
		}

		// Token: 0x06000A0D RID: 2573 RVA: 0x000C4148 File Offset: 0x000C2348
		private void FormSkeleton(IPolygon input)
		{
			this.mesh.insegments = 0;
			if (this.behavior.Poly)
			{
				if (this.mesh.triangles.Count == 0)
				{
					return;
				}
				if (input.Segments.Count > 0)
				{
					this.mesh.MakeVertexMap();
				}
				foreach (ISegment segment in input.Segments)
				{
					this.mesh.insegments++;
					Vertex vertex = segment.GetVertex(0);
					Vertex vertex2 = segment.GetVertex(1);
					if (vertex.x == vertex2.x && vertex.y == vertex2.y)
					{
						if (Log.Verbose)
						{
							this.logger.Warning(string.Concat(new string[]
							{
								"Endpoints of segment (IDs ",
								vertex.id.ToString(),
								"/",
								vertex2.id.ToString(),
								") are coincident."
							}), "Mesh.FormSkeleton()");
						}
					}
					else
					{
						this.InsertSegment(vertex, vertex2, segment.Label);
					}
				}
			}
			if (this.behavior.Convex || !this.behavior.Poly)
			{
				this.MarkHull();
			}
		}

		// Token: 0x06000A0E RID: 2574 RVA: 0x000C42A8 File Offset: 0x000C24A8
		private void InfectHull()
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Otri ot = default(Otri);
			Osub osub = default(Osub);
			Triangle dummytri = this.mesh.dummytri;
			otri.tri = dummytri;
			otri.orient = 0;
			otri.Sym();
			otri.Copy(ref ot);
			do
			{
				if (!otri.IsInfected())
				{
					otri.Pivot(ref osub);
					if (osub.seg.hash == -1)
					{
						if (!otri.IsInfected())
						{
							otri.Infect();
							this.viri.Add(otri.tri);
						}
					}
					else if (osub.seg.boundary == 0)
					{
						osub.seg.boundary = 1;
						Vertex vertex = otri.Org();
						Vertex vertex2 = otri.Dest();
						if (vertex.label == 0)
						{
							vertex.label = 1;
						}
						if (vertex2.label == 0)
						{
							vertex2.label = 1;
						}
					}
				}
				otri.Lnext();
				otri.Oprev(ref otri2);
				while (otri2.tri.id != -1)
				{
					otri2.Copy(ref otri);
					otri.Oprev(ref otri2);
				}
			}
			while (!otri.Equals(ot));
		}

		// Token: 0x06000A0F RID: 2575 RVA: 0x000C43D8 File Offset: 0x000C25D8
		private void Plague()
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Osub osub = default(Osub);
			SubSegment dummysub = this.mesh.dummysub;
			Triangle dummytri = this.mesh.dummytri;
			for (int i = 0; i < this.viri.Count; i++)
			{
				otri.tri = this.viri[i];
				otri.Uninfect();
				otri.orient = 0;
				while (otri.orient < 3)
				{
					otri.Sym(ref otri2);
					otri.Pivot(ref osub);
					if (otri2.tri.id == -1 || otri2.IsInfected())
					{
						if (osub.seg.hash != -1)
						{
							this.mesh.SubsegDealloc(osub.seg);
							if (otri2.tri.id != -1)
							{
								otri2.Uninfect();
								otri2.SegDissolve(dummysub);
								otri2.Infect();
							}
						}
					}
					else if (osub.seg.hash == -1)
					{
						otri2.Infect();
						this.viri.Add(otri2.tri);
					}
					else
					{
						osub.TriDissolve(dummytri);
						if (osub.seg.boundary == 0)
						{
							osub.seg.boundary = 1;
						}
						Vertex vertex = otri2.Org();
						Vertex vertex2 = otri2.Dest();
						if (vertex.label == 0)
						{
							vertex.label = 1;
						}
						if (vertex2.label == 0)
						{
							vertex2.label = 1;
						}
					}
					otri.orient++;
				}
				otri.Infect();
			}
			foreach (Triangle tri in this.viri)
			{
				otri.tri = tri;
				otri.orient = 0;
				while (otri.orient < 3)
				{
					Vertex vertex3 = otri.Org();
					if (vertex3 != null)
					{
						bool flag = true;
						otri.SetOrg(null);
						otri.Onext(ref otri2);
						while (otri2.tri.id != -1 && !otri2.Equals(otri))
						{
							if (otri2.IsInfected())
							{
								otri2.SetOrg(null);
							}
							else
							{
								flag = false;
							}
							otri2.Onext();
						}
						if (otri2.tri.id == -1)
						{
							otri.Oprev(ref otri2);
							while (otri2.tri.id != -1)
							{
								if (otri2.IsInfected())
								{
									otri2.SetOrg(null);
								}
								else
								{
									flag = false;
								}
								otri2.Oprev();
							}
						}
						if (flag)
						{
							vertex3.type = VertexType.UndeadVertex;
							this.mesh.undeads++;
						}
					}
					otri.orient++;
				}
				otri.orient = 0;
				while (otri.orient < 3)
				{
					otri.Sym(ref otri2);
					if (otri2.tri.id == -1)
					{
						this.mesh.hullsize--;
					}
					else
					{
						otri2.Dissolve(dummytri);
						this.mesh.hullsize++;
					}
					otri.orient++;
				}
				this.mesh.TriangleDealloc(otri.tri);
			}
			this.viri.Clear();
		}

		// Token: 0x06000A10 RID: 2576 RVA: 0x000C4738 File Offset: 0x000C2938
		private FindDirectionResult FindDirection(ref Otri searchtri, Vertex searchpoint)
		{
			Otri otri = default(Otri);
			Vertex vertex = searchtri.Org();
			Vertex c = searchtri.Dest();
			Vertex c2 = searchtri.Apex();
			double num = this.predicates.CounterClockwise(searchpoint, vertex, c2);
			bool flag = num > 0.0;
			double num2 = this.predicates.CounterClockwise(vertex, searchpoint, c);
			bool flag2 = num2 > 0.0;
			if (flag && flag2)
			{
				searchtri.Onext(ref otri);
				if (otri.tri.id == -1)
				{
					flag = false;
				}
				else
				{
					flag2 = false;
				}
			}
			while (flag)
			{
				searchtri.Onext();
				if (searchtri.tri.id == -1)
				{
					this.logger.Error("Unable to find a triangle on path.", "Mesh.FindDirection().1");
					throw new Exception("Unable to find a triangle on path.");
				}
				c2 = searchtri.Apex();
				num2 = num;
				num = this.predicates.CounterClockwise(searchpoint, vertex, c2);
				flag = (num > 0.0);
			}
			while (flag2)
			{
				searchtri.Oprev();
				if (searchtri.tri.id == -1)
				{
					this.logger.Error("Unable to find a triangle on path.", "Mesh.FindDirection().2");
					throw new Exception("Unable to find a triangle on path.");
				}
				c = searchtri.Dest();
				num = num2;
				num2 = this.predicates.CounterClockwise(vertex, searchpoint, c);
				flag2 = (num2 > 0.0);
			}
			if (num == 0.0)
			{
				return FindDirectionResult.Leftcollinear;
			}
			if (num2 == 0.0)
			{
				return FindDirectionResult.Rightcollinear;
			}
			return FindDirectionResult.Within;
		}

		// Token: 0x06000A11 RID: 2577 RVA: 0x000C48AC File Offset: 0x000C2AAC
		private void SegmentIntersection(ref Otri splittri, ref Osub splitsubseg, Vertex endpoint2)
		{
			Osub osub = default(Osub);
			SubSegment dummysub = this.mesh.dummysub;
			Vertex vertex = splittri.Apex();
			Vertex vertex2 = splittri.Org();
			Vertex vertex3 = splittri.Dest();
			double num = vertex3.x - vertex2.x;
			double num2 = vertex3.y - vertex2.y;
			double num3 = endpoint2.x - vertex.x;
			double num4 = endpoint2.y - vertex.y;
			double num5 = vertex2.x - endpoint2.x;
			double num6 = vertex2.y - endpoint2.y;
			double num7 = num2 * num3 - num * num4;
			if (num7 == 0.0)
			{
				this.logger.Error("Attempt to find intersection of parallel segments.", "Mesh.SegmentIntersection()");
				throw new Exception("Attempt to find intersection of parallel segments.");
			}
			double num8 = (num4 * num5 - num3 * num6) / num7;
			Vertex vertex4 = new Vertex(vertex2.x + num8 * (vertex3.x - vertex2.x), vertex2.y + num8 * (vertex3.y - vertex2.y), splitsubseg.seg.boundary);
			Vertex vertex5 = vertex4;
			Mesh mesh = this.mesh;
			int hash_vtx = mesh.hash_vtx;
			mesh.hash_vtx = hash_vtx + 1;
			vertex5.hash = hash_vtx;
			vertex4.id = vertex4.hash;
			vertex4.z = vertex2.z + num8 * (vertex3.z - vertex2.z);
			this.mesh.vertices.Add(vertex4.hash, vertex4);
			if (this.mesh.InsertVertex(vertex4, ref splittri, ref splitsubseg, false, false) != InsertVertexResult.Successful)
			{
				this.logger.Error("Failure to split a segment.", "Mesh.SegmentIntersection()");
				throw new Exception("Failure to split a segment.");
			}
			vertex4.tri = splittri;
			if (this.mesh.steinerleft > 0)
			{
				this.mesh.steinerleft--;
			}
			splitsubseg.Sym();
			splitsubseg.Pivot(ref osub);
			splitsubseg.Dissolve(dummysub);
			osub.Dissolve(dummysub);
			do
			{
				splitsubseg.SetSegOrg(vertex4);
				splitsubseg.Next();
			}
			while (splitsubseg.seg.hash != -1);
			do
			{
				osub.SetSegOrg(vertex4);
				osub.Next();
			}
			while (osub.seg.hash != -1);
			this.FindDirection(ref splittri, vertex);
			Vertex vertex6 = splittri.Dest();
			Vertex vertex7 = splittri.Apex();
			if (vertex7.x == vertex.x && vertex7.y == vertex.y)
			{
				splittri.Onext();
				return;
			}
			if (vertex6.x != vertex.x || vertex6.y != vertex.y)
			{
				this.logger.Error("Topological inconsistency after splitting a segment.", "Mesh.SegmentIntersection()");
				throw new Exception("Topological inconsistency after splitting a segment.");
			}
		}

		// Token: 0x06000A12 RID: 2578 RVA: 0x000C4B60 File Offset: 0x000C2D60
		private bool ScoutSegment(ref Otri searchtri, Vertex endpoint2, int newmark)
		{
			Otri otri = default(Otri);
			Osub osub = default(Osub);
			FindDirectionResult findDirectionResult = this.FindDirection(ref searchtri, endpoint2);
			Vertex vertex = searchtri.Dest();
			Vertex vertex2 = searchtri.Apex();
			if ((vertex2.x == endpoint2.x && vertex2.y == endpoint2.y) || (vertex.x == endpoint2.x && vertex.y == endpoint2.y))
			{
				if (vertex2.x == endpoint2.x && vertex2.y == endpoint2.y)
				{
					searchtri.Lprev();
				}
				this.mesh.InsertSubseg(ref searchtri, newmark);
				return true;
			}
			if (findDirectionResult == FindDirectionResult.Leftcollinear)
			{
				searchtri.Lprev();
				this.mesh.InsertSubseg(ref searchtri, newmark);
				return this.ScoutSegment(ref searchtri, endpoint2, newmark);
			}
			if (findDirectionResult == FindDirectionResult.Rightcollinear)
			{
				this.mesh.InsertSubseg(ref searchtri, newmark);
				searchtri.Lnext();
				return this.ScoutSegment(ref searchtri, endpoint2, newmark);
			}
			searchtri.Lnext(ref otri);
			otri.Pivot(ref osub);
			if (osub.seg.hash == -1)
			{
				return false;
			}
			this.SegmentIntersection(ref otri, ref osub, endpoint2);
			otri.Copy(ref searchtri);
			this.mesh.InsertSubseg(ref searchtri, newmark);
			return this.ScoutSegment(ref searchtri, endpoint2, newmark);
		}

		// Token: 0x06000A13 RID: 2579 RVA: 0x000C4C8C File Offset: 0x000C2E8C
		private void DelaunayFixup(ref Otri fixuptri, bool leftside)
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Osub osub = default(Osub);
			fixuptri.Lnext(ref otri);
			otri.Sym(ref otri2);
			if (otri2.tri.id == -1)
			{
				return;
			}
			otri.Pivot(ref osub);
			if (osub.seg.hash != -1)
			{
				return;
			}
			Vertex vertex = otri.Apex();
			Vertex vertex2 = otri.Org();
			Vertex vertex3 = otri.Dest();
			Vertex vertex4 = otri2.Apex();
			if (leftside)
			{
				if (this.predicates.CounterClockwise(vertex, vertex2, vertex4) <= 0.0)
				{
					return;
				}
			}
			else if (this.predicates.CounterClockwise(vertex4, vertex3, vertex) <= 0.0)
			{
				return;
			}
			if (this.predicates.CounterClockwise(vertex3, vertex2, vertex4) > 0.0 && this.predicates.InCircle(vertex2, vertex4, vertex3, vertex) <= 0.0)
			{
				return;
			}
			this.mesh.Flip(ref otri);
			fixuptri.Lprev();
			this.DelaunayFixup(ref fixuptri, leftside);
			this.DelaunayFixup(ref otri2, leftside);
		}

		// Token: 0x06000A14 RID: 2580 RVA: 0x000C4DA8 File Offset: 0x000C2FA8
		private void ConstrainedEdge(ref Otri starttri, Vertex endpoint2, int newmark)
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Osub osub = default(Osub);
			Vertex a = starttri.Org();
			starttri.Lnext(ref otri);
			this.mesh.Flip(ref otri);
			bool flag = false;
			bool flag2 = false;
			do
			{
				Vertex vertex = otri.Org();
				if (vertex.x == endpoint2.x && vertex.y == endpoint2.y)
				{
					otri.Oprev(ref otri2);
					this.DelaunayFixup(ref otri, false);
					this.DelaunayFixup(ref otri2, true);
					flag2 = true;
				}
				else
				{
					double num = this.predicates.CounterClockwise(a, endpoint2, vertex);
					if (num == 0.0)
					{
						flag = true;
						otri.Oprev(ref otri2);
						this.DelaunayFixup(ref otri, false);
						this.DelaunayFixup(ref otri2, true);
						flag2 = true;
					}
					else
					{
						if (num > 0.0)
						{
							otri.Oprev(ref otri2);
							this.DelaunayFixup(ref otri2, true);
							otri.Lprev();
						}
						else
						{
							this.DelaunayFixup(ref otri, false);
							otri.Oprev();
						}
						otri.Pivot(ref osub);
						if (osub.seg.hash == -1)
						{
							this.mesh.Flip(ref otri);
						}
						else
						{
							flag = true;
							this.SegmentIntersection(ref otri, ref osub, endpoint2);
							flag2 = true;
						}
					}
				}
			}
			while (!flag2);
			this.mesh.InsertSubseg(ref otri, newmark);
			if (flag && !this.ScoutSegment(ref otri, endpoint2, newmark))
			{
				this.ConstrainedEdge(ref otri, endpoint2, newmark);
			}
		}

		// Token: 0x06000A15 RID: 2581 RVA: 0x000C4F18 File Offset: 0x000C3118
		private void InsertSegment(Vertex endpoint1, Vertex endpoint2, int newmark)
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Vertex a = null;
			Triangle dummytri = this.mesh.dummytri;
			otri = endpoint1.tri;
			if (otri.tri != null)
			{
				a = otri.Org();
			}
			if (a != endpoint1)
			{
				otri.tri = dummytri;
				otri.orient = 0;
				otri.Sym();
				if (this.locator.Locate(endpoint1, ref otri) != LocateResult.OnVertex)
				{
					this.logger.Error("Unable to locate PSLG vertex in triangulation.", "Mesh.InsertSegment().1");
					throw new Exception("Unable to locate PSLG vertex in triangulation.");
				}
			}
			this.locator.Update(ref otri);
			if (this.ScoutSegment(ref otri, endpoint2, newmark))
			{
				return;
			}
			endpoint1 = otri.Org();
			a = null;
			otri2 = endpoint2.tri;
			if (otri2.tri != null)
			{
				a = otri2.Org();
			}
			if (a != endpoint2)
			{
				otri2.tri = dummytri;
				otri2.orient = 0;
				otri2.Sym();
				if (this.locator.Locate(endpoint2, ref otri2) != LocateResult.OnVertex)
				{
					this.logger.Error("Unable to locate PSLG vertex in triangulation.", "Mesh.InsertSegment().2");
					throw new Exception("Unable to locate PSLG vertex in triangulation.");
				}
			}
			this.locator.Update(ref otri2);
			if (this.ScoutSegment(ref otri2, endpoint1, newmark))
			{
				return;
			}
			endpoint2 = otri2.Org();
			this.ConstrainedEdge(ref otri, endpoint2, newmark);
		}

		// Token: 0x06000A16 RID: 2582 RVA: 0x000C5068 File Offset: 0x000C3268
		private void MarkHull()
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Otri ot = default(Otri);
			otri.tri = this.mesh.dummytri;
			otri.orient = 0;
			otri.Sym();
			otri.Copy(ref ot);
			do
			{
				this.mesh.InsertSubseg(ref otri, 1);
				otri.Lnext();
				otri.Oprev(ref otri2);
				while (otri2.tri.id != -1)
				{
					otri2.Copy(ref otri);
					otri.Oprev(ref otri2);
				}
			}
			while (!otri.Equals(ot));
		}

		// Token: 0x04000A95 RID: 2709
		private IPredicates predicates;

		// Token: 0x04000A96 RID: 2710
		private Mesh mesh;

		// Token: 0x04000A97 RID: 2711
		private Behavior behavior;

		// Token: 0x04000A98 RID: 2712
		private TriangleLocator locator;

		// Token: 0x04000A99 RID: 2713
		private List<Triangle> viri;

		// Token: 0x04000A9A RID: 2714
		private ILog<LogItem> logger;
	}
}
