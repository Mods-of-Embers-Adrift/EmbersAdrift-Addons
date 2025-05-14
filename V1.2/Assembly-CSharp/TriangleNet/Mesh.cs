using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Logging;
using TriangleNet.Meshing;
using TriangleNet.Meshing.Data;
using TriangleNet.Meshing.Iterators;
using TriangleNet.Tools;
using TriangleNet.Topology;

namespace TriangleNet
{
	// Token: 0x020000EB RID: 235
	public class Mesh : IMesh
	{
		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x06000844 RID: 2116 RVA: 0x00048894 File Offset: 0x00046A94
		public Rectangle Bounds
		{
			get
			{
				return this.bounds;
			}
		}

		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x06000845 RID: 2117 RVA: 0x0004889C File Offset: 0x00046A9C
		public ICollection<Vertex> Vertices
		{
			get
			{
				return this.vertices.Values;
			}
		}

		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x06000846 RID: 2118 RVA: 0x000488A9 File Offset: 0x00046AA9
		public IList<Point> Holes
		{
			get
			{
				return this.holes;
			}
		}

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x06000847 RID: 2119 RVA: 0x000488B1 File Offset: 0x00046AB1
		public ICollection<Triangle> Triangles
		{
			get
			{
				return this.triangles;
			}
		}

		// Token: 0x170002CB RID: 715
		// (get) Token: 0x06000848 RID: 2120 RVA: 0x000488B9 File Offset: 0x00046AB9
		public ICollection<SubSegment> Segments
		{
			get
			{
				return this.subsegs.Values;
			}
		}

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x06000849 RID: 2121 RVA: 0x000488C6 File Offset: 0x00046AC6
		public IEnumerable<Edge> Edges
		{
			get
			{
				EdgeIterator e = new EdgeIterator(this);
				while (e.MoveNext())
				{
					Edge edge = e.Current;
					yield return edge;
				}
				yield break;
			}
		}

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x0600084A RID: 2122 RVA: 0x000488D6 File Offset: 0x00046AD6
		public int NumberOfInputPoints
		{
			get
			{
				return this.invertices;
			}
		}

		// Token: 0x170002CE RID: 718
		// (get) Token: 0x0600084B RID: 2123 RVA: 0x000488DE File Offset: 0x00046ADE
		public int NumberOfEdges
		{
			get
			{
				return (3 * this.triangles.Count + this.hullsize) / 2;
			}
		}

		// Token: 0x170002CF RID: 719
		// (get) Token: 0x0600084C RID: 2124 RVA: 0x000488F6 File Offset: 0x00046AF6
		public bool IsPolygon
		{
			get
			{
				return this.insegments > 0;
			}
		}

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x0600084D RID: 2125 RVA: 0x00048901 File Offset: 0x00046B01
		public NodeNumbering CurrentNumbering
		{
			get
			{
				return this.numbering;
			}
		}

		// Token: 0x0600084E RID: 2126 RVA: 0x000B0924 File Offset: 0x000AEB24
		private void Initialize()
		{
			this.dummysub = new SubSegment();
			this.dummysub.hash = -1;
			this.dummysub.subsegs[0].seg = this.dummysub;
			this.dummysub.subsegs[1].seg = this.dummysub;
			this.dummytri = new Triangle();
			this.dummytri.hash = (this.dummytri.id = -1);
			this.dummytri.neighbors[0].tri = this.dummytri;
			this.dummytri.neighbors[1].tri = this.dummytri;
			this.dummytri.neighbors[2].tri = this.dummytri;
			this.dummytri.subsegs[0].seg = this.dummysub;
			this.dummytri.subsegs[1].seg = this.dummysub;
			this.dummytri.subsegs[2].seg = this.dummysub;
		}

		// Token: 0x0600084F RID: 2127 RVA: 0x000B0A50 File Offset: 0x000AEC50
		public Mesh(Configuration config)
		{
			this.Initialize();
			this.logger = Log.Instance;
			this.behavior = new Behavior(false, 20.0);
			this.vertices = new Dictionary<int, Vertex>();
			this.subsegs = new Dictionary<int, SubSegment>();
			this.triangles = config.TrianglePool();
			this.flipstack = new Stack<Otri>();
			this.holes = new List<Point>();
			this.regions = new List<RegionPointer>();
			this.steinerleft = -1;
			this.predicates = config.Predicates();
			this.locator = new TriangleLocator(this, this.predicates);
		}

		// Token: 0x06000850 RID: 2128 RVA: 0x000B0AFC File Offset: 0x000AECFC
		public void Refine(QualityOptions quality, bool delaunay = false)
		{
			this.invertices = this.vertices.Count;
			if (this.behavior.Poly)
			{
				this.insegments = (this.behavior.useSegments ? this.subsegs.Count : this.hullsize);
			}
			this.Reset();
			if (this.qualityMesher == null)
			{
				this.qualityMesher = new QualityMesher(this, new Configuration());
			}
			this.qualityMesher.Apply(quality, delaunay);
		}

		// Token: 0x06000851 RID: 2129 RVA: 0x00048909 File Offset: 0x00046B09
		public void Renumber()
		{
			this.Renumber(NodeNumbering.Linear);
		}

		// Token: 0x06000852 RID: 2130 RVA: 0x000B0B7C File Offset: 0x000AED7C
		public void Renumber(NodeNumbering num)
		{
			if (num == this.numbering)
			{
				return;
			}
			int num2;
			if (num == NodeNumbering.Linear)
			{
				num2 = 0;
				using (Dictionary<int, Vertex>.ValueCollection.Enumerator enumerator = this.vertices.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Vertex vertex = enumerator.Current;
						vertex.id = num2++;
					}
					goto IL_9F;
				}
			}
			if (num == NodeNumbering.CuthillMcKee)
			{
				int[] array = new CuthillMcKee().Renumber(this);
				foreach (Vertex vertex2 in this.vertices.Values)
				{
					vertex2.id = array[vertex2.id];
				}
			}
			IL_9F:
			this.numbering = num;
			num2 = 0;
			foreach (Triangle triangle in this.triangles)
			{
				triangle.id = num2++;
			}
		}

		// Token: 0x06000853 RID: 2131 RVA: 0x00048912 File Offset: 0x00046B12
		internal void SetQualityMesher(QualityMesher qmesher)
		{
			this.qualityMesher = qmesher;
		}

		// Token: 0x06000854 RID: 2132 RVA: 0x000B0C90 File Offset: 0x000AEE90
		internal void CopyTo(Mesh target)
		{
			target.vertices = this.vertices;
			target.triangles = this.triangles;
			target.subsegs = this.subsegs;
			target.holes = this.holes;
			target.regions = this.regions;
			target.hash_vtx = this.hash_vtx;
			target.hash_seg = this.hash_seg;
			target.hash_tri = this.hash_tri;
			target.numbering = this.numbering;
			target.hullsize = this.hullsize;
		}

		// Token: 0x06000855 RID: 2133 RVA: 0x000B0D18 File Offset: 0x000AEF18
		private void ResetData()
		{
			this.vertices.Clear();
			this.triangles.Restart();
			this.subsegs.Clear();
			this.holes.Clear();
			this.regions.Clear();
			this.hash_vtx = 0;
			this.hash_seg = 0;
			this.hash_tri = 0;
			this.flipstack.Clear();
			this.hullsize = 0;
			this.Reset();
			this.locator.Reset();
		}

		// Token: 0x06000856 RID: 2134 RVA: 0x000B0D98 File Offset: 0x000AEF98
		private void Reset()
		{
			this.numbering = NodeNumbering.None;
			this.undeads = 0;
			this.checksegments = false;
			this.checkquality = false;
			Statistic.InCircleCount = 0L;
			Statistic.CounterClockwiseCount = 0L;
			Statistic.InCircleAdaptCount = 0L;
			Statistic.CounterClockwiseAdaptCount = 0L;
			Statistic.Orient3dCount = 0L;
			Statistic.HyperbolaCount = 0L;
			Statistic.CircleTopCount = 0L;
			Statistic.CircumcenterCount = 0L;
		}

		// Token: 0x06000857 RID: 2135 RVA: 0x000B0DFC File Offset: 0x000AEFFC
		internal void TransferNodes(IList<Vertex> points)
		{
			this.invertices = points.Count;
			this.mesh_dim = 2;
			this.bounds = new Rectangle();
			if (this.invertices < 3)
			{
				this.logger.Error("Input must have at least three input vertices.", "Mesh.TransferNodes()");
				throw new Exception("Input must have at least three input vertices.");
			}
			Point point = points[0];
			int num = this.nextras;
			this.nextras = num;
			bool flag = point.id != points[1].id;
			foreach (Vertex vertex in points)
			{
				if (flag)
				{
					vertex.hash = vertex.id;
					this.hash_vtx = Math.Max(vertex.hash + 1, this.hash_vtx);
				}
				else
				{
					Vertex vertex2 = vertex;
					Point point2 = vertex;
					int num2 = this.hash_vtx;
					this.hash_vtx = num2 + 1;
					vertex2.hash = (point2.id = num2);
				}
				this.vertices.Add(vertex.hash, vertex);
				this.bounds.Expand(vertex);
			}
		}

		// Token: 0x06000858 RID: 2136 RVA: 0x000B0F20 File Offset: 0x000AF120
		internal void MakeVertexMap()
		{
			Otri otri = default(Otri);
			foreach (Triangle tri in this.triangles)
			{
				otri.tri = tri;
				otri.orient = 0;
				while (otri.orient < 3)
				{
					otri.Org().tri = otri;
					otri.orient++;
				}
			}
		}

		// Token: 0x06000859 RID: 2137 RVA: 0x000B0FA4 File Offset: 0x000AF1A4
		internal void MakeTriangle(ref Otri newotri)
		{
			Triangle triangle = this.triangles.Get();
			triangle.subsegs[0].seg = this.dummysub;
			triangle.subsegs[1].seg = this.dummysub;
			triangle.subsegs[2].seg = this.dummysub;
			triangle.neighbors[0].tri = this.dummytri;
			triangle.neighbors[1].tri = this.dummytri;
			triangle.neighbors[2].tri = this.dummytri;
			newotri.tri = triangle;
			newotri.orient = 0;
		}

		// Token: 0x0600085A RID: 2138 RVA: 0x000B1058 File Offset: 0x000AF258
		internal void MakeSegment(ref Osub newsubseg)
		{
			SubSegment subSegment = new SubSegment();
			SubSegment subSegment2 = subSegment;
			int num = this.hash_seg;
			this.hash_seg = num + 1;
			subSegment2.hash = num;
			subSegment.subsegs[0].seg = this.dummysub;
			subSegment.subsegs[1].seg = this.dummysub;
			subSegment.triangles[0].tri = this.dummytri;
			subSegment.triangles[1].tri = this.dummytri;
			newsubseg.seg = subSegment;
			newsubseg.orient = 0;
			this.subsegs.Add(subSegment.hash, subSegment);
		}

		// Token: 0x0600085B RID: 2139 RVA: 0x000B1100 File Offset: 0x000AF300
		internal InsertVertexResult InsertVertex(Vertex newvertex, ref Otri searchtri, ref Osub splitseg, bool segmentflaws, bool triflaws)
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Otri otri3 = default(Otri);
			Otri otri4 = default(Otri);
			Otri otri5 = default(Otri);
			Otri otri6 = default(Otri);
			Otri otri7 = default(Otri);
			Otri otri8 = default(Otri);
			Otri otri9 = default(Otri);
			Otri otri10 = default(Otri);
			Otri otri11 = default(Otri);
			Otri otri12 = default(Otri);
			Otri otri13 = default(Otri);
			Otri otri14 = default(Otri);
			Osub osub = default(Osub);
			Osub osub2 = default(Osub);
			Osub osub3 = default(Osub);
			Osub osub4 = default(Osub);
			Osub osub5 = default(Osub);
			Osub osub6 = default(Osub);
			Osub osub7 = default(Osub);
			Osub osub8 = default(Osub);
			LocateResult locateResult;
			if (splitseg.seg == null)
			{
				if (searchtri.tri.id == -1)
				{
					otri.tri = this.dummytri;
					otri.orient = 0;
					otri.Sym();
					locateResult = this.locator.Locate(newvertex, ref otri);
				}
				else
				{
					searchtri.Copy(ref otri);
					locateResult = this.locator.PreciseLocate(newvertex, ref otri, true);
				}
			}
			else
			{
				searchtri.Copy(ref otri);
				locateResult = LocateResult.OnEdge;
			}
			if (locateResult == LocateResult.OnVertex)
			{
				otri.Copy(ref searchtri);
				this.locator.Update(ref otri);
				return InsertVertexResult.Duplicate;
			}
			Vertex vertex;
			Vertex vertex2;
			Vertex vertex3;
			if (locateResult == LocateResult.OnEdge || locateResult == LocateResult.Outside)
			{
				if (this.checksegments && splitseg.seg == null)
				{
					otri.Pivot(ref osub5);
					if (osub5.seg.hash != -1)
					{
						if (segmentflaws)
						{
							bool flag = this.behavior.NoBisect != 2;
							if (flag && this.behavior.NoBisect == 1)
							{
								otri.Sym(ref otri14);
								flag = (otri14.tri.id != -1);
							}
							if (flag)
							{
								BadSubseg badSubseg = new BadSubseg();
								badSubseg.subseg = osub5;
								badSubseg.org = osub5.Org();
								badSubseg.dest = osub5.Dest();
								this.qualityMesher.AddBadSubseg(badSubseg);
							}
						}
						otri.Copy(ref searchtri);
						this.locator.Update(ref otri);
						return InsertVertexResult.Violating;
					}
				}
				otri.Lprev(ref otri4);
				otri4.Sym(ref otri11);
				otri.Sym(ref otri6);
				bool flag2 = otri6.tri.id != -1;
				if (flag2)
				{
					otri6.Lnext();
					otri6.Sym(ref otri13);
					this.MakeTriangle(ref otri9);
				}
				else
				{
					this.hullsize++;
				}
				this.MakeTriangle(ref otri8);
				vertex = otri.Org();
				vertex2 = otri.Dest();
				vertex3 = otri.Apex();
				otri8.SetOrg(vertex3);
				otri8.SetDest(vertex);
				otri8.SetApex(newvertex);
				otri.SetOrg(newvertex);
				otri8.tri.label = otri4.tri.label;
				if (this.behavior.VarArea)
				{
					otri8.tri.area = otri4.tri.area;
				}
				if (flag2)
				{
					Vertex dest = otri6.Dest();
					otri9.SetOrg(vertex);
					otri9.SetDest(dest);
					otri9.SetApex(newvertex);
					otri6.SetOrg(newvertex);
					otri9.tri.label = otri6.tri.label;
					if (this.behavior.VarArea)
					{
						otri9.tri.area = otri6.tri.area;
					}
				}
				if (this.checksegments)
				{
					otri4.Pivot(ref osub2);
					if (osub2.seg.hash != -1)
					{
						otri4.SegDissolve(this.dummysub);
						otri8.SegBond(ref osub2);
					}
					if (flag2)
					{
						otri6.Pivot(ref osub4);
						if (osub4.seg.hash != -1)
						{
							otri6.SegDissolve(this.dummysub);
							otri9.SegBond(ref osub4);
						}
					}
				}
				otri8.Bond(ref otri11);
				otri8.Lprev();
				otri8.Bond(ref otri4);
				otri8.Lprev();
				if (flag2)
				{
					otri9.Bond(ref otri13);
					otri9.Lnext();
					otri9.Bond(ref otri6);
					otri9.Lnext();
					otri9.Bond(ref otri8);
				}
				if (splitseg.seg != null)
				{
					splitseg.SetDest(newvertex);
					Vertex segOrg = splitseg.SegOrg();
					Vertex segDest = splitseg.SegDest();
					splitseg.Sym();
					splitseg.Pivot(ref osub7);
					this.InsertSubseg(ref otri8, splitseg.seg.boundary);
					otri8.Pivot(ref osub8);
					osub8.SetSegOrg(segOrg);
					osub8.SetSegDest(segDest);
					splitseg.Bond(ref osub8);
					osub8.Sym();
					osub8.Bond(ref osub7);
					splitseg.Sym();
					if (newvertex.label == 0)
					{
						newvertex.label = splitseg.seg.boundary;
					}
				}
				if (this.checkquality)
				{
					this.flipstack.Clear();
					this.flipstack.Push(default(Otri));
					this.flipstack.Push(otri);
				}
				otri.Lnext();
			}
			else
			{
				otri.Lnext(ref otri3);
				otri.Lprev(ref otri4);
				otri3.Sym(ref otri10);
				otri4.Sym(ref otri11);
				this.MakeTriangle(ref otri7);
				this.MakeTriangle(ref otri8);
				vertex = otri.Org();
				vertex2 = otri.Dest();
				vertex3 = otri.Apex();
				otri7.SetOrg(vertex2);
				otri7.SetDest(vertex3);
				otri7.SetApex(newvertex);
				otri8.SetOrg(vertex3);
				otri8.SetDest(vertex);
				otri8.SetApex(newvertex);
				otri.SetApex(newvertex);
				otri7.tri.label = otri.tri.label;
				otri8.tri.label = otri.tri.label;
				if (this.behavior.VarArea)
				{
					double area = otri.tri.area;
					otri7.tri.area = area;
					otri8.tri.area = area;
				}
				if (this.checksegments)
				{
					otri3.Pivot(ref osub);
					if (osub.seg.hash != -1)
					{
						otri3.SegDissolve(this.dummysub);
						otri7.SegBond(ref osub);
					}
					otri4.Pivot(ref osub2);
					if (osub2.seg.hash != -1)
					{
						otri4.SegDissolve(this.dummysub);
						otri8.SegBond(ref osub2);
					}
				}
				otri7.Bond(ref otri10);
				otri8.Bond(ref otri11);
				otri7.Lnext();
				otri8.Lprev();
				otri7.Bond(ref otri8);
				otri7.Lnext();
				otri3.Bond(ref otri7);
				otri8.Lprev();
				otri4.Bond(ref otri8);
				if (this.checkquality)
				{
					this.flipstack.Clear();
					this.flipstack.Push(otri);
				}
			}
			InsertVertexResult result = InsertVertexResult.Successful;
			if (newvertex.tri.tri != null)
			{
				newvertex.tri.SetOrg(vertex);
				newvertex.tri.SetDest(vertex2);
				newvertex.tri.SetApex(vertex3);
			}
			Vertex vertex4 = otri.Org();
			vertex = vertex4;
			vertex2 = otri.Dest();
			for (;;)
			{
				bool flag3 = true;
				if (this.checksegments)
				{
					otri.Pivot(ref osub6);
					if (osub6.seg.hash != -1)
					{
						flag3 = false;
						if (segmentflaws && this.qualityMesher.CheckSeg4Encroach(ref osub6) > 0)
						{
							result = InsertVertexResult.Encroaching;
						}
					}
				}
				if (flag3)
				{
					otri.Sym(ref otri2);
					if (otri2.tri.id == -1)
					{
						flag3 = false;
					}
					else
					{
						Vertex vertex5 = otri2.Apex();
						if (vertex2 == this.infvertex1 || vertex2 == this.infvertex2 || vertex2 == this.infvertex3)
						{
							flag3 = (this.predicates.CounterClockwise(newvertex, vertex, vertex5) > 0.0);
						}
						else if (vertex == this.infvertex1 || vertex == this.infvertex2 || vertex == this.infvertex3)
						{
							flag3 = (this.predicates.CounterClockwise(vertex5, vertex2, newvertex) > 0.0);
						}
						else
						{
							flag3 = (!(vertex5 == this.infvertex1) && !(vertex5 == this.infvertex2) && !(vertex5 == this.infvertex3) && this.predicates.InCircle(vertex2, newvertex, vertex, vertex5) > 0.0);
						}
						if (flag3)
						{
							otri2.Lprev(ref otri5);
							otri5.Sym(ref otri12);
							otri2.Lnext(ref otri6);
							otri6.Sym(ref otri13);
							otri.Lnext(ref otri3);
							otri3.Sym(ref otri10);
							otri.Lprev(ref otri4);
							otri4.Sym(ref otri11);
							otri5.Bond(ref otri10);
							otri3.Bond(ref otri11);
							otri4.Bond(ref otri13);
							otri6.Bond(ref otri12);
							if (this.checksegments)
							{
								otri5.Pivot(ref osub3);
								otri3.Pivot(ref osub);
								otri4.Pivot(ref osub2);
								otri6.Pivot(ref osub4);
								if (osub3.seg.hash == -1)
								{
									otri6.SegDissolve(this.dummysub);
								}
								else
								{
									otri6.SegBond(ref osub3);
								}
								if (osub.seg.hash == -1)
								{
									otri5.SegDissolve(this.dummysub);
								}
								else
								{
									otri5.SegBond(ref osub);
								}
								if (osub2.seg.hash == -1)
								{
									otri3.SegDissolve(this.dummysub);
								}
								else
								{
									otri3.SegBond(ref osub2);
								}
								if (osub4.seg.hash == -1)
								{
									otri4.SegDissolve(this.dummysub);
								}
								else
								{
									otri4.SegBond(ref osub4);
								}
							}
							otri.SetOrg(vertex5);
							otri.SetDest(newvertex);
							otri.SetApex(vertex);
							otri2.SetOrg(newvertex);
							otri2.SetDest(vertex5);
							otri2.SetApex(vertex2);
							int label = Math.Min(otri2.tri.label, otri.tri.label);
							otri2.tri.label = label;
							otri.tri.label = label;
							if (this.behavior.VarArea)
							{
								double area;
								if (otri2.tri.area <= 0.0 || otri.tri.area <= 0.0)
								{
									area = -1.0;
								}
								else
								{
									area = 0.5 * (otri2.tri.area + otri.tri.area);
								}
								otri2.tri.area = area;
								otri.tri.area = area;
							}
							if (this.checkquality)
							{
								this.flipstack.Push(otri);
							}
							otri.Lprev();
							vertex2 = vertex5;
						}
					}
				}
				if (!flag3)
				{
					if (triflaws)
					{
						this.qualityMesher.TestTriangle(ref otri);
					}
					otri.Lnext();
					otri.Sym(ref otri14);
					if (vertex2 == vertex4 || otri14.tri.id == -1)
					{
						break;
					}
					otri14.Lnext(ref otri);
					vertex = vertex2;
					vertex2 = otri.Dest();
				}
			}
			otri.Lnext(ref searchtri);
			Otri otri15 = default(Otri);
			otri.Lnext(ref otri15);
			this.locator.Update(ref otri15);
			return result;
		}

		// Token: 0x0600085C RID: 2140 RVA: 0x000B1C1C File Offset: 0x000AFE1C
		internal void InsertSubseg(ref Otri tri, int subsegmark)
		{
			Otri otri = default(Otri);
			Osub osub = default(Osub);
			Vertex vertex = tri.Org();
			Vertex vertex2 = tri.Dest();
			if (vertex.label == 0)
			{
				vertex.label = subsegmark;
			}
			if (vertex2.label == 0)
			{
				vertex2.label = subsegmark;
			}
			tri.Pivot(ref osub);
			if (osub.seg.hash == -1)
			{
				this.MakeSegment(ref osub);
				osub.SetOrg(vertex2);
				osub.SetDest(vertex);
				osub.SetSegOrg(vertex2);
				osub.SetSegDest(vertex);
				tri.SegBond(ref osub);
				tri.Sym(ref otri);
				osub.Sym();
				otri.SegBond(ref osub);
				osub.seg.boundary = subsegmark;
				return;
			}
			if (osub.seg.boundary == 0)
			{
				osub.seg.boundary = subsegmark;
			}
		}

		// Token: 0x0600085D RID: 2141 RVA: 0x000B1CEC File Offset: 0x000AFEEC
		internal void Flip(ref Otri flipedge)
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Otri otri3 = default(Otri);
			Otri otri4 = default(Otri);
			Otri otri5 = default(Otri);
			Otri otri6 = default(Otri);
			Otri otri7 = default(Otri);
			Otri otri8 = default(Otri);
			Otri otri9 = default(Otri);
			Osub osub = default(Osub);
			Osub osub2 = default(Osub);
			Osub osub3 = default(Osub);
			Osub osub4 = default(Osub);
			Vertex apex = flipedge.Org();
			Vertex apex2 = flipedge.Dest();
			Vertex vertex = flipedge.Apex();
			flipedge.Sym(ref otri5);
			Vertex vertex2 = otri5.Apex();
			otri5.Lprev(ref otri3);
			otri3.Sym(ref otri8);
			otri5.Lnext(ref otri4);
			otri4.Sym(ref otri9);
			flipedge.Lnext(ref otri);
			otri.Sym(ref otri6);
			flipedge.Lprev(ref otri2);
			otri2.Sym(ref otri7);
			otri3.Bond(ref otri6);
			otri.Bond(ref otri7);
			otri2.Bond(ref otri9);
			otri4.Bond(ref otri8);
			if (this.checksegments)
			{
				otri3.Pivot(ref osub3);
				otri.Pivot(ref osub);
				otri2.Pivot(ref osub2);
				otri4.Pivot(ref osub4);
				if (osub3.seg.hash == -1)
				{
					otri4.SegDissolve(this.dummysub);
				}
				else
				{
					otri4.SegBond(ref osub3);
				}
				if (osub.seg.hash == -1)
				{
					otri3.SegDissolve(this.dummysub);
				}
				else
				{
					otri3.SegBond(ref osub);
				}
				if (osub2.seg.hash == -1)
				{
					otri.SegDissolve(this.dummysub);
				}
				else
				{
					otri.SegBond(ref osub2);
				}
				if (osub4.seg.hash == -1)
				{
					otri2.SegDissolve(this.dummysub);
				}
				else
				{
					otri2.SegBond(ref osub4);
				}
			}
			flipedge.SetOrg(vertex2);
			flipedge.SetDest(vertex);
			flipedge.SetApex(apex);
			otri5.SetOrg(vertex);
			otri5.SetDest(vertex2);
			otri5.SetApex(apex2);
		}

		// Token: 0x0600085E RID: 2142 RVA: 0x000B1EF4 File Offset: 0x000B00F4
		internal void Unflip(ref Otri flipedge)
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Otri otri3 = default(Otri);
			Otri otri4 = default(Otri);
			Otri otri5 = default(Otri);
			Otri otri6 = default(Otri);
			Otri otri7 = default(Otri);
			Otri otri8 = default(Otri);
			Otri otri9 = default(Otri);
			Osub osub = default(Osub);
			Osub osub2 = default(Osub);
			Osub osub3 = default(Osub);
			Osub osub4 = default(Osub);
			Vertex apex = flipedge.Org();
			Vertex apex2 = flipedge.Dest();
			Vertex vertex = flipedge.Apex();
			flipedge.Sym(ref otri5);
			Vertex vertex2 = otri5.Apex();
			otri5.Lprev(ref otri3);
			otri3.Sym(ref otri8);
			otri5.Lnext(ref otri4);
			otri4.Sym(ref otri9);
			flipedge.Lnext(ref otri);
			otri.Sym(ref otri6);
			flipedge.Lprev(ref otri2);
			otri2.Sym(ref otri7);
			otri3.Bond(ref otri9);
			otri.Bond(ref otri8);
			otri2.Bond(ref otri6);
			otri4.Bond(ref otri7);
			if (this.checksegments)
			{
				otri3.Pivot(ref osub3);
				otri.Pivot(ref osub);
				otri2.Pivot(ref osub2);
				otri4.Pivot(ref osub4);
				if (osub3.seg.hash == -1)
				{
					otri.SegDissolve(this.dummysub);
				}
				else
				{
					otri.SegBond(ref osub3);
				}
				if (osub.seg.hash == -1)
				{
					otri2.SegDissolve(this.dummysub);
				}
				else
				{
					otri2.SegBond(ref osub);
				}
				if (osub2.seg.hash == -1)
				{
					otri4.SegDissolve(this.dummysub);
				}
				else
				{
					otri4.SegBond(ref osub2);
				}
				if (osub4.seg.hash == -1)
				{
					otri3.SegDissolve(this.dummysub);
				}
				else
				{
					otri3.SegBond(ref osub4);
				}
			}
			flipedge.SetOrg(vertex);
			flipedge.SetDest(vertex2);
			flipedge.SetApex(apex2);
			otri5.SetOrg(vertex2);
			otri5.SetDest(vertex);
			otri5.SetApex(apex);
		}

		// Token: 0x0600085F RID: 2143 RVA: 0x000B20FC File Offset: 0x000B02FC
		private void TriangulatePolygon(Otri firstedge, Otri lastedge, int edgecount, bool doflip, bool triflaws)
		{
			Otri otri = default(Otri);
			Otri firstedge2 = default(Otri);
			Otri lastedge2 = default(Otri);
			int num = 1;
			Vertex a = lastedge.Apex();
			Vertex b = firstedge.Dest();
			firstedge.Onext(ref firstedge2);
			Vertex c = firstedge2.Dest();
			firstedge2.Copy(ref otri);
			for (int i = 2; i <= edgecount - 2; i++)
			{
				otri.Onext();
				Vertex vertex = otri.Dest();
				if (this.predicates.InCircle(a, b, c, vertex) > 0.0)
				{
					otri.Copy(ref firstedge2);
					c = vertex;
					num = i;
				}
			}
			if (num > 1)
			{
				firstedge2.Oprev(ref lastedge2);
				this.TriangulatePolygon(firstedge, lastedge2, num + 1, true, triflaws);
			}
			if (num < edgecount - 2)
			{
				firstedge2.Sym(ref lastedge2);
				this.TriangulatePolygon(firstedge2, lastedge, edgecount - num, true, triflaws);
				lastedge2.Sym(ref firstedge2);
			}
			if (doflip)
			{
				this.Flip(ref firstedge2);
				if (triflaws)
				{
					firstedge2.Sym(ref otri);
					this.qualityMesher.TestTriangle(ref otri);
				}
			}
			firstedge2.Copy(ref lastedge);
		}

		// Token: 0x06000860 RID: 2144 RVA: 0x000B2214 File Offset: 0x000B0414
		internal void DeleteVertex(ref Otri deltri)
		{
			Otri ot = default(Otri);
			Otri firstedge = default(Otri);
			Otri lastedge = default(Otri);
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Otri otri3 = default(Otri);
			Otri otri4 = default(Otri);
			Otri otri5 = default(Otri);
			Osub osub = default(Osub);
			Osub osub2 = default(Osub);
			Vertex dyingvertex = deltri.Org();
			this.VertexDealloc(dyingvertex);
			deltri.Onext(ref ot);
			int num = 1;
			while (!deltri.Equals(ot))
			{
				num++;
				ot.Onext();
			}
			if (num > 3)
			{
				deltri.Onext(ref firstedge);
				deltri.Oprev(ref lastedge);
				this.TriangulatePolygon(firstedge, lastedge, num, false, this.behavior.NoBisect == 0);
			}
			deltri.Lprev(ref otri);
			deltri.Dnext(ref otri2);
			otri2.Sym(ref otri4);
			otri.Oprev(ref otri3);
			otri3.Sym(ref otri5);
			deltri.Bond(ref otri4);
			otri.Bond(ref otri5);
			otri2.Pivot(ref osub);
			if (osub.seg.hash != -1)
			{
				deltri.SegBond(ref osub);
			}
			otri3.Pivot(ref osub2);
			if (osub2.seg.hash != -1)
			{
				otri.SegBond(ref osub2);
			}
			Vertex org = otri2.Org();
			deltri.SetOrg(org);
			if (this.behavior.NoBisect == 0)
			{
				this.qualityMesher.TestTriangle(ref deltri);
			}
			this.TriangleDealloc(otri2.tri);
			this.TriangleDealloc(otri3.tri);
		}

		// Token: 0x06000861 RID: 2145 RVA: 0x000B2394 File Offset: 0x000B0594
		internal void UndoVertex()
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Otri otri3 = default(Otri);
			Otri otri4 = default(Otri);
			Otri otri5 = default(Otri);
			Otri otri6 = default(Otri);
			Otri otri7 = default(Otri);
			Osub osub = default(Osub);
			Osub osub2 = default(Osub);
			Osub osub3 = default(Osub);
			while (this.flipstack.Count > 0)
			{
				Otri otri8 = this.flipstack.Pop();
				if (this.flipstack.Count == 0)
				{
					otri8.Dprev(ref otri);
					otri.Lnext();
					otri8.Onext(ref otri2);
					otri2.Lprev();
					otri.Sym(ref otri4);
					otri2.Sym(ref otri5);
					Vertex apex = otri.Dest();
					otri8.SetApex(apex);
					otri8.Lnext();
					otri8.Bond(ref otri4);
					otri.Pivot(ref osub);
					otri8.SegBond(ref osub);
					otri8.Lnext();
					otri8.Bond(ref otri5);
					otri2.Pivot(ref osub2);
					otri8.SegBond(ref osub2);
					this.TriangleDealloc(otri.tri);
					this.TriangleDealloc(otri2.tri);
				}
				else if (this.flipstack.Peek().tri == null)
				{
					otri8.Lprev(ref otri7);
					otri7.Sym(ref otri2);
					otri2.Lnext();
					otri2.Sym(ref otri5);
					Vertex org = otri2.Dest();
					otri8.SetOrg(org);
					otri7.Bond(ref otri5);
					otri2.Pivot(ref osub2);
					otri7.SegBond(ref osub2);
					this.TriangleDealloc(otri2.tri);
					otri8.Sym(ref otri7);
					if (otri7.tri.id != -1)
					{
						otri7.Lnext();
						otri7.Dnext(ref otri3);
						otri3.Sym(ref otri6);
						otri7.SetOrg(org);
						otri7.Bond(ref otri6);
						otri3.Pivot(ref osub3);
						otri7.SegBond(ref osub3);
						this.TriangleDealloc(otri3.tri);
					}
					this.flipstack.Clear();
				}
				else
				{
					this.Unflip(ref otri8);
				}
			}
		}

		// Token: 0x06000862 RID: 2146 RVA: 0x0004891B File Offset: 0x00046B1B
		internal void TriangleDealloc(Triangle dyingtriangle)
		{
			Otri.Kill(dyingtriangle);
			this.triangles.Release(dyingtriangle);
		}

		// Token: 0x06000863 RID: 2147 RVA: 0x0004892F File Offset: 0x00046B2F
		internal void VertexDealloc(Vertex dyingvertex)
		{
			dyingvertex.type = VertexType.DeadVertex;
			this.vertices.Remove(dyingvertex.hash);
		}

		// Token: 0x06000864 RID: 2148 RVA: 0x0004894A File Offset: 0x00046B4A
		internal void SubsegDealloc(SubSegment dyingsubseg)
		{
			Osub.Kill(dyingsubseg);
			this.subsegs.Remove(dyingsubseg.hash);
		}

		// Token: 0x0400098E RID: 2446
		private IPredicates predicates;

		// Token: 0x0400098F RID: 2447
		private ILog<LogItem> logger;

		// Token: 0x04000990 RID: 2448
		private QualityMesher qualityMesher;

		// Token: 0x04000991 RID: 2449
		private Stack<Otri> flipstack;

		// Token: 0x04000992 RID: 2450
		internal TrianglePool triangles;

		// Token: 0x04000993 RID: 2451
		internal Dictionary<int, SubSegment> subsegs;

		// Token: 0x04000994 RID: 2452
		internal Dictionary<int, Vertex> vertices;

		// Token: 0x04000995 RID: 2453
		internal int hash_vtx;

		// Token: 0x04000996 RID: 2454
		internal int hash_seg;

		// Token: 0x04000997 RID: 2455
		internal int hash_tri;

		// Token: 0x04000998 RID: 2456
		internal List<Point> holes;

		// Token: 0x04000999 RID: 2457
		internal List<RegionPointer> regions;

		// Token: 0x0400099A RID: 2458
		internal Rectangle bounds;

		// Token: 0x0400099B RID: 2459
		internal int invertices;

		// Token: 0x0400099C RID: 2460
		internal int insegments;

		// Token: 0x0400099D RID: 2461
		internal int undeads;

		// Token: 0x0400099E RID: 2462
		internal int mesh_dim;

		// Token: 0x0400099F RID: 2463
		internal int nextras;

		// Token: 0x040009A0 RID: 2464
		internal int hullsize;

		// Token: 0x040009A1 RID: 2465
		internal int steinerleft;

		// Token: 0x040009A2 RID: 2466
		internal bool checksegments;

		// Token: 0x040009A3 RID: 2467
		internal bool checkquality;

		// Token: 0x040009A4 RID: 2468
		internal Vertex infvertex1;

		// Token: 0x040009A5 RID: 2469
		internal Vertex infvertex2;

		// Token: 0x040009A6 RID: 2470
		internal Vertex infvertex3;

		// Token: 0x040009A7 RID: 2471
		internal TriangleLocator locator;

		// Token: 0x040009A8 RID: 2472
		internal Behavior behavior;

		// Token: 0x040009A9 RID: 2473
		internal NodeNumbering numbering;

		// Token: 0x040009AA RID: 2474
		internal const int DUMMY = -1;

		// Token: 0x040009AB RID: 2475
		internal Triangle dummytri;

		// Token: 0x040009AC RID: 2476
		internal SubSegment dummysub;
	}
}
