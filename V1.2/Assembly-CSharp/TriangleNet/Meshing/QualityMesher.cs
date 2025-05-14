using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Logging;
using TriangleNet.Meshing.Data;
using TriangleNet.Tools;
using TriangleNet.Topology;

namespace TriangleNet.Meshing
{
	// Token: 0x02000122 RID: 290
	internal class QualityMesher
	{
		// Token: 0x06000A3D RID: 2621 RVA: 0x000C5F70 File Offset: 0x000C4170
		public QualityMesher(Mesh mesh, Configuration config)
		{
			this.logger = Log.Instance;
			this.badsubsegs = new Queue<BadSubseg>();
			this.queue = new BadTriQueue();
			this.mesh = mesh;
			this.predicates = config.Predicates();
			this.behavior = mesh.behavior;
			this.newLocation = new NewLocation(mesh, this.predicates);
			this.newvertex_tri = new Triangle();
		}

		// Token: 0x06000A3E RID: 2622 RVA: 0x000C5FE8 File Offset: 0x000C41E8
		public void Apply(QualityOptions quality, bool delaunay = false)
		{
			if (quality != null)
			{
				this.behavior.Quality = true;
				this.behavior.MinAngle = quality.MinimumAngle;
				this.behavior.MaxAngle = quality.MaximumAngle;
				this.behavior.MaxArea = quality.MaximumArea;
				this.behavior.UserTest = quality.UserTest;
				this.behavior.VarArea = quality.VariableArea;
				this.behavior.ConformingDelaunay = (this.behavior.ConformingDelaunay || delaunay);
				this.mesh.steinerleft = ((quality.SteinerPoints == 0) ? -1 : quality.SteinerPoints);
			}
			if (!this.behavior.Poly)
			{
				this.behavior.VarArea = false;
			}
			this.mesh.infvertex1 = null;
			this.mesh.infvertex2 = null;
			this.mesh.infvertex3 = null;
			if (this.behavior.useSegments)
			{
				this.mesh.checksegments = true;
			}
			if (this.behavior.Quality && this.mesh.triangles.Count > 0)
			{
				this.EnforceQuality();
			}
		}

		// Token: 0x06000A3F RID: 2623 RVA: 0x00049C29 File Offset: 0x00047E29
		public void AddBadSubseg(BadSubseg badseg)
		{
			this.badsubsegs.Enqueue(badseg);
		}

		// Token: 0x06000A40 RID: 2624 RVA: 0x000C610C File Offset: 0x000C430C
		public int CheckSeg4Encroach(ref Osub testsubseg)
		{
			Otri otri = default(Otri);
			Osub subseg = default(Osub);
			int num = 0;
			int num2 = 0;
			Vertex vertex = testsubseg.Org();
			Vertex vertex2 = testsubseg.Dest();
			testsubseg.Pivot(ref otri);
			if (otri.tri.id != -1)
			{
				num2++;
				Vertex vertex3 = otri.Apex();
				double num3 = (vertex.x - vertex3.x) * (vertex2.x - vertex3.x) + (vertex.y - vertex3.y) * (vertex2.y - vertex3.y);
				if (num3 < 0.0 && (this.behavior.ConformingDelaunay || num3 * num3 >= (2.0 * this.behavior.goodAngle - 1.0) * (2.0 * this.behavior.goodAngle - 1.0) * ((vertex.x - vertex3.x) * (vertex.x - vertex3.x) + (vertex.y - vertex3.y) * (vertex.y - vertex3.y)) * ((vertex2.x - vertex3.x) * (vertex2.x - vertex3.x) + (vertex2.y - vertex3.y) * (vertex2.y - vertex3.y))))
				{
					num = 1;
				}
			}
			testsubseg.Sym(ref subseg);
			subseg.Pivot(ref otri);
			if (otri.tri.id != -1)
			{
				num2++;
				Vertex vertex3 = otri.Apex();
				double num3 = (vertex.x - vertex3.x) * (vertex2.x - vertex3.x) + (vertex.y - vertex3.y) * (vertex2.y - vertex3.y);
				if (num3 < 0.0 && (this.behavior.ConformingDelaunay || num3 * num3 >= (2.0 * this.behavior.goodAngle - 1.0) * (2.0 * this.behavior.goodAngle - 1.0) * ((vertex.x - vertex3.x) * (vertex.x - vertex3.x) + (vertex.y - vertex3.y) * (vertex.y - vertex3.y)) * ((vertex2.x - vertex3.x) * (vertex2.x - vertex3.x) + (vertex2.y - vertex3.y) * (vertex2.y - vertex3.y))))
				{
					num += 2;
				}
			}
			if (num > 0 && (this.behavior.NoBisect == 0 || (this.behavior.NoBisect == 1 && num2 == 2)))
			{
				BadSubseg badSubseg = new BadSubseg();
				if (num == 1)
				{
					badSubseg.subseg = testsubseg;
					badSubseg.org = vertex;
					badSubseg.dest = vertex2;
				}
				else
				{
					badSubseg.subseg = subseg;
					badSubseg.org = vertex2;
					badSubseg.dest = vertex;
				}
				this.badsubsegs.Enqueue(badSubseg);
			}
			return num;
		}

		// Token: 0x06000A41 RID: 2625 RVA: 0x000C645C File Offset: 0x000C465C
		public void TestTriangle(ref Otri testtri)
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Osub osub = default(Osub);
			Vertex vertex = testtri.Org();
			Vertex vertex2 = testtri.Dest();
			Vertex vertex3 = testtri.Apex();
			double num = vertex.x - vertex2.x;
			double num2 = vertex.y - vertex2.y;
			double num3 = vertex2.x - vertex3.x;
			double num4 = vertex2.y - vertex3.y;
			double num5 = vertex3.x - vertex.x;
			double num6 = vertex3.y - vertex.y;
			double num7 = num * num;
			double num8 = num2 * num2;
			double num9 = num3 * num3;
			double num10 = num4 * num4;
			double num11 = num5 * num5;
			double num12 = num6 * num6;
			double num13 = num7 + num8;
			double num14 = num9 + num10;
			double num15 = num11 + num12;
			double minedge;
			double num16;
			Vertex vertex4;
			Vertex vertex5;
			if (num13 < num14 && num13 < num15)
			{
				minedge = num13;
				num16 = num3 * num5 + num4 * num6;
				num16 = num16 * num16 / (num14 * num15);
				vertex4 = vertex;
				vertex5 = vertex2;
				testtri.Copy(ref otri);
			}
			else if (num14 < num15)
			{
				minedge = num14;
				num16 = num * num5 + num2 * num6;
				num16 = num16 * num16 / (num13 * num15);
				vertex4 = vertex2;
				vertex5 = vertex3;
				testtri.Lnext(ref otri);
			}
			else
			{
				minedge = num15;
				num16 = num * num3 + num2 * num4;
				num16 = num16 * num16 / (num13 * num14);
				vertex4 = vertex3;
				vertex5 = vertex;
				testtri.Lprev(ref otri);
			}
			if (this.behavior.VarArea || this.behavior.fixedArea || this.behavior.UserTest != null)
			{
				double num17 = 0.5 * (num * num4 - num2 * num3);
				if (this.behavior.fixedArea && num17 > this.behavior.MaxArea)
				{
					this.queue.Enqueue(ref testtri, minedge, vertex3, vertex, vertex2);
					return;
				}
				if (this.behavior.VarArea && num17 > testtri.tri.area && testtri.tri.area > 0.0)
				{
					this.queue.Enqueue(ref testtri, minedge, vertex3, vertex, vertex2);
					return;
				}
				if (this.behavior.UserTest != null && this.behavior.UserTest(testtri.tri, num17))
				{
					this.queue.Enqueue(ref testtri, minedge, vertex3, vertex, vertex2);
					return;
				}
			}
			double num18;
			if (num13 > num14 && num13 > num15)
			{
				num18 = (num14 + num15 - num13) / (2.0 * Math.Sqrt(num14 * num15));
			}
			else if (num14 > num15)
			{
				num18 = (num13 + num15 - num14) / (2.0 * Math.Sqrt(num13 * num15));
			}
			else
			{
				num18 = (num13 + num14 - num15) / (2.0 * Math.Sqrt(num13 * num14));
			}
			if (num16 > this.behavior.goodAngle || (num18 < this.behavior.maxGoodAngle && this.behavior.MaxAngle != 0.0))
			{
				if (vertex4.type == VertexType.SegmentVertex && vertex5.type == VertexType.SegmentVertex)
				{
					otri.Pivot(ref osub);
					if (osub.seg.hash == -1)
					{
						otri.Copy(ref otri2);
						do
						{
							otri.Oprev();
							otri.Pivot(ref osub);
						}
						while (osub.seg.hash == -1);
						Vertex vertex6 = osub.SegOrg();
						Vertex vertex7 = osub.SegDest();
						do
						{
							otri2.Dnext();
							otri2.Pivot(ref osub);
						}
						while (osub.seg.hash == -1);
						Vertex vertex8 = osub.SegOrg();
						Vertex vertex9 = osub.SegDest();
						Vertex vertex10 = null;
						if (vertex7.x == vertex8.x && vertex7.y == vertex8.y)
						{
							vertex10 = vertex7;
						}
						else if (vertex6.x == vertex9.x && vertex6.y == vertex9.y)
						{
							vertex10 = vertex6;
						}
						if (vertex10 != null)
						{
							double num19 = (vertex4.x - vertex10.x) * (vertex4.x - vertex10.x) + (vertex4.y - vertex10.y) * (vertex4.y - vertex10.y);
							double num20 = (vertex5.x - vertex10.x) * (vertex5.x - vertex10.x) + (vertex5.y - vertex10.y) * (vertex5.y - vertex10.y);
							if (num19 < 1.001 * num20 && num19 > 0.999 * num20)
							{
								return;
							}
						}
					}
				}
				this.queue.Enqueue(ref testtri, minedge, vertex3, vertex, vertex2);
			}
		}

		// Token: 0x06000A42 RID: 2626 RVA: 0x000C6920 File Offset: 0x000C4B20
		private void TallyEncs()
		{
			Osub osub = default(Osub);
			osub.orient = 0;
			foreach (SubSegment seg in this.mesh.subsegs.Values)
			{
				osub.seg = seg;
				this.CheckSeg4Encroach(ref osub);
			}
		}

		// Token: 0x06000A43 RID: 2627 RVA: 0x000C6998 File Offset: 0x000C4B98
		private void SplitEncSegs(bool triflaws)
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Osub osub = default(Osub);
			Osub osub2 = default(Osub);
			while (this.badsubsegs.Count > 0 && this.mesh.steinerleft != 0)
			{
				BadSubseg badSubseg = this.badsubsegs.Dequeue();
				osub2 = badSubseg.subseg;
				Vertex vertex = osub2.Org();
				Vertex vertex2 = osub2.Dest();
				if (!Osub.IsDead(osub2.seg) && vertex == badSubseg.org && vertex2 == badSubseg.dest)
				{
					osub2.Pivot(ref otri);
					otri.Lnext(ref otri2);
					otri2.Pivot(ref osub);
					bool flag = osub.seg.hash != -1;
					otri2.Lnext();
					otri2.Pivot(ref osub);
					bool flag2 = osub.seg.hash != -1;
					if (!this.behavior.ConformingDelaunay && !flag && !flag2)
					{
						Vertex vertex3 = otri.Apex();
						while (vertex3.type == VertexType.FreeVertex && (vertex.x - vertex3.x) * (vertex2.x - vertex3.x) + (vertex.y - vertex3.y) * (vertex2.y - vertex3.y) < 0.0)
						{
							this.mesh.DeleteVertex(ref otri2);
							osub2.Pivot(ref otri);
							vertex3 = otri.Apex();
							otri.Lprev(ref otri2);
						}
					}
					otri.Sym(ref otri2);
					if (otri2.tri.id != -1)
					{
						otri2.Lnext();
						otri2.Pivot(ref osub);
						bool flag3 = osub.seg.hash != -1;
						flag2 = (flag2 || flag3);
						otri2.Lnext();
						otri2.Pivot(ref osub);
						bool flag4 = osub.seg.hash != -1;
						flag = (flag || flag4);
						if (!this.behavior.ConformingDelaunay && !flag4 && !flag3)
						{
							Vertex vertex3 = otri2.Org();
							while (vertex3.type == VertexType.FreeVertex && (vertex.x - vertex3.x) * (vertex2.x - vertex3.x) + (vertex.y - vertex3.y) * (vertex2.y - vertex3.y) < 0.0)
							{
								this.mesh.DeleteVertex(ref otri2);
								otri.Sym(ref otri2);
								vertex3 = otri2.Apex();
								otri2.Lprev();
							}
						}
					}
					double num3;
					if (flag || flag2)
					{
						double num = Math.Sqrt((vertex2.x - vertex.x) * (vertex2.x - vertex.x) + (vertex2.y - vertex.y) * (vertex2.y - vertex.y));
						double num2 = 1.0;
						while (num > 3.0 * num2)
						{
							num2 *= 2.0;
						}
						while (num < 1.5 * num2)
						{
							num2 *= 0.5;
						}
						num3 = num2 / num;
						if (flag2)
						{
							num3 = 1.0 - num3;
						}
					}
					else
					{
						num3 = 0.5;
					}
					Vertex vertex4 = new Vertex(vertex.x + num3 * (vertex2.x - vertex.x), vertex.y + num3 * (vertex2.y - vertex.y), osub2.seg.boundary);
					vertex4.type = VertexType.SegmentVertex;
					Vertex vertex5 = vertex4;
					Mesh mesh = this.mesh;
					int hash_vtx = mesh.hash_vtx;
					mesh.hash_vtx = hash_vtx + 1;
					vertex5.hash = hash_vtx;
					vertex4.id = vertex4.hash;
					this.mesh.vertices.Add(vertex4.hash, vertex4);
					vertex4.z = vertex.z + num3 * (vertex2.z - vertex.z);
					if (!Behavior.NoExact)
					{
						double num4 = this.predicates.CounterClockwise(vertex, vertex2, vertex4);
						double num5 = (vertex.x - vertex2.x) * (vertex.x - vertex2.x) + (vertex.y - vertex2.y) * (vertex.y - vertex2.y);
						if (num4 != 0.0 && num5 != 0.0)
						{
							num4 /= num5;
							if (!double.IsNaN(num4))
							{
								vertex4.x += num4 * (vertex2.y - vertex.y);
								vertex4.y += num4 * (vertex.x - vertex2.x);
							}
						}
					}
					if ((vertex4.x == vertex.x && vertex4.y == vertex.y) || (vertex4.x == vertex2.x && vertex4.y == vertex2.y))
					{
						this.logger.Error("Ran out of precision: I attempted to split a segment to a smaller size than can be accommodated by the finite precision of floating point arithmetic.", "Quality.SplitEncSegs()");
						throw new Exception("Ran out of precision");
					}
					InsertVertexResult insertVertexResult = this.mesh.InsertVertex(vertex4, ref otri, ref osub2, true, triflaws);
					if (insertVertexResult != InsertVertexResult.Successful && insertVertexResult != InsertVertexResult.Encroaching)
					{
						this.logger.Error("Failure to split a segment.", "Quality.SplitEncSegs()");
						throw new Exception("Failure to split a segment.");
					}
					if (this.mesh.steinerleft > 0)
					{
						this.mesh.steinerleft--;
					}
					this.CheckSeg4Encroach(ref osub2);
					osub2.Next();
					this.CheckSeg4Encroach(ref osub2);
				}
				badSubseg.org = null;
			}
		}

		// Token: 0x06000A44 RID: 2628 RVA: 0x000C6F70 File Offset: 0x000C5170
		private void TallyFaces()
		{
			Otri otri = default(Otri);
			otri.orient = 0;
			foreach (Triangle tri in this.mesh.triangles)
			{
				otri.tri = tri;
				this.TestTriangle(ref otri);
			}
		}

		// Token: 0x06000A45 RID: 2629 RVA: 0x000C6FDC File Offset: 0x000C51DC
		private void SplitTriangle(BadTriangle badtri)
		{
			Otri otri = default(Otri);
			double num = 0.0;
			double num2 = 0.0;
			otri = badtri.poortri;
			Vertex vertex = otri.Org();
			Vertex vertex2 = otri.Dest();
			Vertex vertex3 = otri.Apex();
			if (!Otri.IsDead(otri.tri) && vertex == badtri.org && vertex2 == badtri.dest && vertex3 == badtri.apex)
			{
				bool flag = false;
				Point point;
				if (this.behavior.fixedArea || this.behavior.VarArea)
				{
					point = this.predicates.FindCircumcenter(vertex, vertex2, vertex3, ref num, ref num2, this.behavior.offconstant);
				}
				else
				{
					point = this.newLocation.FindLocation(vertex, vertex2, vertex3, ref num, ref num2, true, otri);
				}
				if ((point.x == vertex.x && point.y == vertex.y) || (point.x == vertex2.x && point.y == vertex2.y) || (point.x == vertex3.x && point.y == vertex3.y))
				{
					if (Log.Verbose)
					{
						this.logger.Warning("New vertex falls on existing vertex.", "Quality.SplitTriangle()");
						flag = true;
					}
				}
				else
				{
					Vertex vertex4 = new Vertex(point.x, point.y, 0);
					vertex4.type = VertexType.FreeVertex;
					if (num2 < num)
					{
						otri.Lprev();
					}
					vertex4.tri.tri = this.newvertex_tri;
					Osub osub = default(Osub);
					InsertVertexResult insertVertexResult = this.mesh.InsertVertex(vertex4, ref otri, ref osub, true, true);
					if (insertVertexResult == InsertVertexResult.Successful)
					{
						Vertex vertex5 = vertex4;
						Mesh mesh = this.mesh;
						int hash_vtx = mesh.hash_vtx;
						mesh.hash_vtx = hash_vtx + 1;
						vertex5.hash = hash_vtx;
						vertex4.id = vertex4.hash;
						Interpolation.InterpolateZ(vertex4, vertex4.tri.tri);
						this.mesh.vertices.Add(vertex4.hash, vertex4);
						if (this.mesh.steinerleft > 0)
						{
							this.mesh.steinerleft--;
						}
					}
					else if (insertVertexResult == InsertVertexResult.Encroaching)
					{
						this.mesh.UndoVertex();
					}
					else if (insertVertexResult != InsertVertexResult.Violating && Log.Verbose)
					{
						this.logger.Warning("New vertex falls on existing vertex.", "Quality.SplitTriangle()");
						flag = true;
					}
				}
				if (flag)
				{
					this.logger.Error("The new vertex is at the circumcenter of triangle: This probably means that I am trying to refine triangles to a smaller size than can be accommodated by the finite precision of floating point arithmetic.", "Quality.SplitTriangle()");
					throw new Exception("The new vertex is at the circumcenter of triangle.");
				}
			}
		}

		// Token: 0x06000A46 RID: 2630 RVA: 0x000C7274 File Offset: 0x000C5474
		private void EnforceQuality()
		{
			this.TallyEncs();
			this.SplitEncSegs(false);
			if (this.behavior.MinAngle > 0.0 || this.behavior.VarArea || this.behavior.fixedArea || this.behavior.UserTest != null)
			{
				this.TallyFaces();
				this.mesh.checkquality = true;
				while (this.queue.Count > 0 && this.mesh.steinerleft != 0)
				{
					BadTriangle badtri = this.queue.Dequeue();
					this.SplitTriangle(badtri);
					if (this.badsubsegs.Count > 0)
					{
						this.queue.Enqueue(badtri);
						this.SplitEncSegs(true);
					}
				}
			}
			if (Log.Verbose && this.behavior.ConformingDelaunay && this.badsubsegs.Count > 0 && this.mesh.steinerleft == 0)
			{
				this.logger.Warning("I ran out of Steiner points, but the mesh has encroached subsegments, and therefore might not be truly Delaunay. If the Delaunay property is important to you, try increasing the number of Steiner points.", "Quality.EnforceQuality()");
			}
		}

		// Token: 0x04000AA1 RID: 2721
		private IPredicates predicates;

		// Token: 0x04000AA2 RID: 2722
		private Queue<BadSubseg> badsubsegs;

		// Token: 0x04000AA3 RID: 2723
		private BadTriQueue queue;

		// Token: 0x04000AA4 RID: 2724
		private Mesh mesh;

		// Token: 0x04000AA5 RID: 2725
		private Behavior behavior;

		// Token: 0x04000AA6 RID: 2726
		private NewLocation newLocation;

		// Token: 0x04000AA7 RID: 2727
		private ILog<LogItem> logger;

		// Token: 0x04000AA8 RID: 2728
		private Triangle newvertex_tri;
	}
}
