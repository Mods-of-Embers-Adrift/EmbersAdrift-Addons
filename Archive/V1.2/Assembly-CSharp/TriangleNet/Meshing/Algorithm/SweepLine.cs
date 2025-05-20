using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Tools;
using TriangleNet.Topology;

namespace TriangleNet.Meshing.Algorithm
{
	// Token: 0x02000131 RID: 305
	public class SweepLine : ITriangulator
	{
		// Token: 0x06000A8E RID: 2702 RVA: 0x00049E5A File Offset: 0x0004805A
		private static int randomnation(int choices)
		{
			SweepLine.randomseed = (SweepLine.randomseed * 1366 + 150889) % 714025;
			return SweepLine.randomseed / (714025 / choices + 1);
		}

		// Token: 0x06000A8F RID: 2703 RVA: 0x000C8DF0 File Offset: 0x000C6FF0
		public IMesh Triangulate(IList<Vertex> points, Configuration config)
		{
			this.predicates = config.Predicates();
			this.mesh = new Mesh(config);
			this.mesh.TransferNodes(points);
			this.xminextreme = 10.0 * this.mesh.bounds.Left - 9.0 * this.mesh.bounds.Right;
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Otri otri3 = default(Otri);
			Otri otri4 = default(Otri);
			Otri otri5 = default(Otri);
			Otri otriEvent = default(Otri);
			Otri newkey = default(Otri);
			bool flag = false;
			this.splaynodes = new List<SweepLine.SplayNode>();
			SweepLine.SplayNode splayroot = null;
			int i = points.Count;
			SweepLine.SweepEvent[] array;
			this.CreateHeap(out array, i);
			this.mesh.MakeTriangle(ref otri3);
			this.mesh.MakeTriangle(ref otri4);
			otri3.Bond(ref otri4);
			otri3.Lnext();
			otri4.Lprev();
			otri3.Bond(ref otri4);
			otri3.Lnext();
			otri4.Lprev();
			otri3.Bond(ref otri4);
			Vertex vertexEvent = array[0].vertexEvent;
			this.HeapDelete(array, i, 0);
			i--;
			while (i != 0)
			{
				Vertex vertexEvent2 = array[0].vertexEvent;
				this.HeapDelete(array, i, 0);
				i--;
				if (vertexEvent.x == vertexEvent2.x && vertexEvent.y == vertexEvent2.y)
				{
					if (Log.Verbose)
					{
						Log.Instance.Warning("A duplicate vertex appeared and was ignored (ID " + vertexEvent2.id.ToString() + ").", "SweepLine.Triangulate().1");
					}
					vertexEvent2.type = VertexType.UndeadVertex;
					this.mesh.undeads++;
				}
				if (vertexEvent.x != vertexEvent2.x || vertexEvent.y != vertexEvent2.y)
				{
					otri3.SetOrg(vertexEvent);
					otri3.SetDest(vertexEvent2);
					otri4.SetOrg(vertexEvent2);
					otri4.SetDest(vertexEvent);
					otri3.Lprev(ref otri);
					Vertex vertex = vertexEvent2;
					while (i > 0)
					{
						SweepLine.SweepEvent sweepEvent = array[0];
						this.HeapDelete(array, i, 0);
						i--;
						bool flag2 = true;
						if (sweepEvent.xkey < this.mesh.bounds.Left)
						{
							Otri otriEvent2 = sweepEvent.otriEvent;
							otriEvent2.Oprev(ref otri5);
							this.Check4DeadEvent(ref otri5, array, ref i);
							otriEvent2.Onext(ref otriEvent);
							this.Check4DeadEvent(ref otriEvent, array, ref i);
							if (otri5.Equals(otri))
							{
								otriEvent2.Lprev(ref otri);
							}
							this.mesh.Flip(ref otriEvent2);
							otriEvent2.SetApex(null);
							otriEvent2.Lprev(ref otri3);
							otriEvent2.Lnext(ref otri4);
							otri3.Sym(ref otri5);
							if (SweepLine.randomnation(SweepLine.SAMPLERATE) == 0)
							{
								otriEvent2.Sym();
								Vertex vertex2 = otriEvent2.Dest();
								Vertex vertex3 = otriEvent2.Apex();
								Vertex vertex4 = otriEvent2.Org();
								splayroot = this.CircleTopInsert(splayroot, otri3, vertex2, vertex3, vertex4, sweepEvent.ykey);
							}
						}
						else
						{
							Vertex vertexEvent3 = sweepEvent.vertexEvent;
							if (vertexEvent3.x == vertex.x && vertexEvent3.y == vertex.y)
							{
								if (Log.Verbose)
								{
									Log.Instance.Warning("A duplicate vertex appeared and was ignored (ID " + vertexEvent3.id.ToString() + ").", "SweepLine.Triangulate().2");
								}
								vertexEvent3.type = VertexType.UndeadVertex;
								this.mesh.undeads++;
								flag2 = false;
							}
							else
							{
								vertex = vertexEvent3;
								splayroot = this.FrontLocate(splayroot, otri, vertexEvent3, ref otri2, ref flag);
								this.Check4DeadEvent(ref otri2, array, ref i);
								otri2.Copy(ref otriEvent);
								otri2.Sym(ref otri5);
								this.mesh.MakeTriangle(ref otri3);
								this.mesh.MakeTriangle(ref otri4);
								Vertex vertex5 = otriEvent.Dest();
								otri3.SetOrg(vertex5);
								otri3.SetDest(vertexEvent3);
								otri4.SetOrg(vertexEvent3);
								otri4.SetDest(vertex5);
								otri3.Bond(ref otri4);
								otri3.Lnext();
								otri4.Lprev();
								otri3.Bond(ref otri4);
								otri3.Lnext();
								otri4.Lprev();
								otri3.Bond(ref otri5);
								otri4.Bond(ref otriEvent);
								if (!flag && otriEvent.Equals(otri))
								{
									otri3.Copy(ref otri);
								}
								if (SweepLine.randomnation(SweepLine.SAMPLERATE) == 0)
								{
									splayroot = this.SplayInsert(splayroot, otri3, vertexEvent3);
								}
								else if (SweepLine.randomnation(SweepLine.SAMPLERATE) == 0)
								{
									otri4.Lnext(ref newkey);
									splayroot = this.SplayInsert(splayroot, newkey, vertexEvent3);
								}
							}
						}
						if (flag2)
						{
							Vertex vertex2 = otri5.Apex();
							Vertex vertex3 = otri3.Dest();
							Vertex vertex4 = otri3.Apex();
							double num = this.predicates.CounterClockwise(vertex2, vertex3, vertex4);
							if (num > 0.0)
							{
								SweepLine.SweepEvent sweepEvent2 = new SweepLine.SweepEvent();
								sweepEvent2.xkey = this.xminextreme;
								sweepEvent2.ykey = this.CircleTop(vertex2, vertex3, vertex4, num);
								sweepEvent2.otriEvent = otri3;
								this.HeapInsert(array, i, sweepEvent2);
								i++;
								otri3.SetOrg(new SweepLine.SweepEventVertex(sweepEvent2));
							}
							vertex2 = otri4.Apex();
							vertex3 = otri4.Org();
							vertex4 = otriEvent.Apex();
							double num2 = this.predicates.CounterClockwise(vertex2, vertex3, vertex4);
							if (num2 > 0.0)
							{
								SweepLine.SweepEvent sweepEvent2 = new SweepLine.SweepEvent();
								sweepEvent2.xkey = this.xminextreme;
								sweepEvent2.ykey = this.CircleTop(vertex2, vertex3, vertex4, num2);
								sweepEvent2.otriEvent = otriEvent;
								this.HeapInsert(array, i, sweepEvent2);
								i++;
								otriEvent.SetOrg(new SweepLine.SweepEventVertex(sweepEvent2));
							}
						}
					}
					this.splaynodes.Clear();
					otri.Lprev();
					this.mesh.hullsize = this.RemoveGhosts(ref otri);
					return this.mesh;
				}
			}
			Log.Instance.Error("Input vertices are all identical.", "SweepLine.Triangulate()");
			throw new Exception("Input vertices are all identical.");
		}

		// Token: 0x06000A90 RID: 2704 RVA: 0x000C93E8 File Offset: 0x000C75E8
		private void HeapInsert(SweepLine.SweepEvent[] heap, int heapsize, SweepLine.SweepEvent newevent)
		{
			double xkey = newevent.xkey;
			double ykey = newevent.ykey;
			int num = heapsize;
			bool flag = num > 0;
			while (flag)
			{
				int num2 = num - 1 >> 1;
				if (heap[num2].ykey < ykey || (heap[num2].ykey == ykey && heap[num2].xkey <= xkey))
				{
					flag = false;
				}
				else
				{
					heap[num] = heap[num2];
					heap[num].heapposition = num;
					num = num2;
					flag = (num > 0);
				}
			}
			heap[num] = newevent;
			newevent.heapposition = num;
		}

		// Token: 0x06000A91 RID: 2705 RVA: 0x000C9460 File Offset: 0x000C7660
		private void Heapify(SweepLine.SweepEvent[] heap, int heapsize, int eventnum)
		{
			SweepLine.SweepEvent sweepEvent = heap[eventnum];
			double xkey = sweepEvent.xkey;
			double ykey = sweepEvent.ykey;
			int num = 2 * eventnum + 1;
			bool flag = num < heapsize;
			while (flag)
			{
				int num2;
				if (heap[num].ykey < ykey || (heap[num].ykey == ykey && heap[num].xkey < xkey))
				{
					num2 = num;
				}
				else
				{
					num2 = eventnum;
				}
				int num3 = num + 1;
				if (num3 < heapsize && (heap[num3].ykey < heap[num2].ykey || (heap[num3].ykey == heap[num2].ykey && heap[num3].xkey < heap[num2].xkey)))
				{
					num2 = num3;
				}
				if (num2 == eventnum)
				{
					flag = false;
				}
				else
				{
					heap[eventnum] = heap[num2];
					heap[eventnum].heapposition = eventnum;
					heap[num2] = sweepEvent;
					sweepEvent.heapposition = num2;
					eventnum = num2;
					num = 2 * eventnum + 1;
					flag = (num < heapsize);
				}
			}
		}

		// Token: 0x06000A92 RID: 2706 RVA: 0x000C9544 File Offset: 0x000C7744
		private void HeapDelete(SweepLine.SweepEvent[] heap, int heapsize, int eventnum)
		{
			SweepLine.SweepEvent sweepEvent = heap[heapsize - 1];
			if (eventnum > 0)
			{
				double xkey = sweepEvent.xkey;
				double ykey = sweepEvent.ykey;
				bool flag;
				do
				{
					int num = eventnum - 1 >> 1;
					if (heap[num].ykey < ykey || (heap[num].ykey == ykey && heap[num].xkey <= xkey))
					{
						flag = false;
					}
					else
					{
						heap[eventnum] = heap[num];
						heap[eventnum].heapposition = eventnum;
						eventnum = num;
						flag = (eventnum > 0);
					}
				}
				while (flag);
			}
			heap[eventnum] = sweepEvent;
			sweepEvent.heapposition = eventnum;
			this.Heapify(heap, heapsize - 1, eventnum);
		}

		// Token: 0x06000A93 RID: 2707 RVA: 0x000C95C8 File Offset: 0x000C77C8
		private void CreateHeap(out SweepLine.SweepEvent[] eventheap, int size)
		{
			int num = 3 * size / 2;
			eventheap = new SweepLine.SweepEvent[num];
			int num2 = 0;
			foreach (Vertex vertex in this.mesh.vertices.Values)
			{
				SweepLine.SweepEvent sweepEvent = new SweepLine.SweepEvent();
				sweepEvent.vertexEvent = vertex;
				sweepEvent.xkey = vertex.x;
				sweepEvent.ykey = vertex.y;
				this.HeapInsert(eventheap, num2++, sweepEvent);
			}
		}

		// Token: 0x06000A94 RID: 2708 RVA: 0x000C9664 File Offset: 0x000C7864
		private SweepLine.SplayNode Splay(SweepLine.SplayNode splaytree, Point searchpoint, ref Otri searchtri)
		{
			if (splaytree == null)
			{
				return null;
			}
			if (splaytree.keyedge.Dest() == splaytree.keydest)
			{
				bool flag = this.RightOfHyperbola(ref splaytree.keyedge, searchpoint);
				SweepLine.SplayNode splayNode;
				if (flag)
				{
					splaytree.keyedge.Copy(ref searchtri);
					splayNode = splaytree.rchild;
				}
				else
				{
					splayNode = splaytree.lchild;
				}
				if (splayNode == null)
				{
					return splaytree;
				}
				if (splayNode.keyedge.Dest() != splayNode.keydest)
				{
					splayNode = this.Splay(splayNode, searchpoint, ref searchtri);
					if (splayNode == null)
					{
						if (flag)
						{
							splaytree.rchild = null;
						}
						else
						{
							splaytree.lchild = null;
						}
						return splaytree;
					}
				}
				bool flag2 = this.RightOfHyperbola(ref splayNode.keyedge, searchpoint);
				SweepLine.SplayNode splayNode2;
				if (flag2)
				{
					splayNode.keyedge.Copy(ref searchtri);
					splayNode2 = this.Splay(splayNode.rchild, searchpoint, ref searchtri);
					splayNode.rchild = splayNode2;
				}
				else
				{
					splayNode2 = this.Splay(splayNode.lchild, searchpoint, ref searchtri);
					splayNode.lchild = splayNode2;
				}
				if (splayNode2 == null)
				{
					if (flag)
					{
						splaytree.rchild = splayNode.lchild;
						splayNode.lchild = splaytree;
					}
					else
					{
						splaytree.lchild = splayNode.rchild;
						splayNode.rchild = splaytree;
					}
					return splayNode;
				}
				if (flag2)
				{
					if (flag)
					{
						splaytree.rchild = splayNode.lchild;
						splayNode.lchild = splaytree;
					}
					else
					{
						splaytree.lchild = splayNode2.rchild;
						splayNode2.rchild = splaytree;
					}
					splayNode.rchild = splayNode2.lchild;
					splayNode2.lchild = splayNode;
				}
				else
				{
					if (flag)
					{
						splaytree.rchild = splayNode2.lchild;
						splayNode2.lchild = splaytree;
					}
					else
					{
						splaytree.lchild = splayNode.rchild;
						splayNode.rchild = splaytree;
					}
					splayNode.lchild = splayNode2.rchild;
					splayNode2.rchild = splayNode;
				}
				return splayNode2;
			}
			else
			{
				SweepLine.SplayNode splayNode3 = this.Splay(splaytree.lchild, searchpoint, ref searchtri);
				SweepLine.SplayNode splayNode4 = this.Splay(splaytree.rchild, searchpoint, ref searchtri);
				this.splaynodes.Remove(splaytree);
				if (splayNode3 == null)
				{
					return splayNode4;
				}
				if (splayNode4 == null)
				{
					return splayNode3;
				}
				if (splayNode3.rchild == null)
				{
					splayNode3.rchild = splayNode4.lchild;
					splayNode4.lchild = splayNode3;
					return splayNode4;
				}
				if (splayNode4.lchild == null)
				{
					splayNode4.lchild = splayNode3.rchild;
					splayNode3.rchild = splayNode4;
					return splayNode3;
				}
				SweepLine.SplayNode rchild = splayNode3.rchild;
				while (rchild.rchild != null)
				{
					rchild = rchild.rchild;
				}
				rchild.rchild = splayNode4;
				return splayNode3;
			}
		}

		// Token: 0x06000A95 RID: 2709 RVA: 0x000C9898 File Offset: 0x000C7A98
		private SweepLine.SplayNode SplayInsert(SweepLine.SplayNode splayroot, Otri newkey, Point searchpoint)
		{
			SweepLine.SplayNode splayNode = new SweepLine.SplayNode();
			this.splaynodes.Add(splayNode);
			newkey.Copy(ref splayNode.keyedge);
			splayNode.keydest = newkey.Dest();
			if (splayroot == null)
			{
				splayNode.lchild = null;
				splayNode.rchild = null;
			}
			else if (this.RightOfHyperbola(ref splayroot.keyedge, searchpoint))
			{
				splayNode.lchild = splayroot;
				splayNode.rchild = splayroot.rchild;
				splayroot.rchild = null;
			}
			else
			{
				splayNode.lchild = splayroot.lchild;
				splayNode.rchild = splayroot;
				splayroot.lchild = null;
			}
			return splayNode;
		}

		// Token: 0x06000A96 RID: 2710 RVA: 0x000C992C File Offset: 0x000C7B2C
		private SweepLine.SplayNode FrontLocate(SweepLine.SplayNode splayroot, Otri bottommost, Vertex searchvertex, ref Otri searchtri, ref bool farright)
		{
			bottommost.Copy(ref searchtri);
			splayroot = this.Splay(splayroot, searchvertex, ref searchtri);
			bool flag = false;
			while (!flag && this.RightOfHyperbola(ref searchtri, searchvertex))
			{
				searchtri.Onext();
				flag = searchtri.Equals(bottommost);
			}
			farright = flag;
			return splayroot;
		}

		// Token: 0x06000A97 RID: 2711 RVA: 0x000C9978 File Offset: 0x000C7B78
		private SweepLine.SplayNode CircleTopInsert(SweepLine.SplayNode splayroot, Otri newkey, Vertex pa, Vertex pb, Vertex pc, double topy)
		{
			Point point = new Point();
			Otri otri = default(Otri);
			double num = this.predicates.CounterClockwise(pa, pb, pc);
			double num2 = pa.x - pc.x;
			double num3 = pa.y - pc.y;
			double num4 = pb.x - pc.x;
			double num5 = pb.y - pc.y;
			double num6 = num2 * num2 + num3 * num3;
			double num7 = num4 * num4 + num5 * num5;
			point.x = pc.x - (num3 * num7 - num5 * num6) / (2.0 * num);
			point.y = topy;
			return this.SplayInsert(this.Splay(splayroot, point, ref otri), newkey, point);
		}

		// Token: 0x06000A98 RID: 2712 RVA: 0x000C9A34 File Offset: 0x000C7C34
		private bool RightOfHyperbola(ref Otri fronttri, Point newsite)
		{
			Statistic.HyperbolaCount += 1L;
			Vertex vertex = fronttri.Dest();
			Vertex vertex2 = fronttri.Apex();
			if (vertex.y < vertex2.y || (vertex.y == vertex2.y && vertex.x < vertex2.x))
			{
				if (newsite.x >= vertex2.x)
				{
					return true;
				}
			}
			else if (newsite.x <= vertex.x)
			{
				return false;
			}
			double num = vertex.x - newsite.x;
			double num2 = vertex.y - newsite.y;
			double num3 = vertex2.x - newsite.x;
			double num4 = vertex2.y - newsite.y;
			return num2 * (num3 * num3 + num4 * num4) > num4 * (num * num + num2 * num2);
		}

		// Token: 0x06000A99 RID: 2713 RVA: 0x000C9AFC File Offset: 0x000C7CFC
		private double CircleTop(Vertex pa, Vertex pb, Vertex pc, double ccwabc)
		{
			Statistic.CircleTopCount += 1L;
			double num = pa.x - pc.x;
			double num2 = pa.y - pc.y;
			double num3 = pb.x - pc.x;
			double num4 = pb.y - pc.y;
			double num5 = pa.x - pb.x;
			double num6 = pa.y - pb.y;
			double num7 = num * num + num2 * num2;
			double num8 = num3 * num3 + num4 * num4;
			double num9 = num5 * num5 + num6 * num6;
			return pc.y + (num * num8 - num3 * num7 + Math.Sqrt(num7 * num8 * num9)) / (2.0 * ccwabc);
		}

		// Token: 0x06000A9A RID: 2714 RVA: 0x000C9BB4 File Offset: 0x000C7DB4
		private void Check4DeadEvent(ref Otri checktri, SweepLine.SweepEvent[] eventheap, ref int heapsize)
		{
			SweepLine.SweepEventVertex sweepEventVertex = checktri.Org() as SweepLine.SweepEventVertex;
			if (sweepEventVertex != null)
			{
				int heapposition = sweepEventVertex.evt.heapposition;
				this.HeapDelete(eventheap, heapsize, heapposition);
				heapsize--;
				checktri.SetOrg(null);
			}
		}

		// Token: 0x06000A9B RID: 2715 RVA: 0x000C9BFC File Offset: 0x000C7DFC
		private int RemoveGhosts(ref Otri startghost)
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Otri otri3 = default(Otri);
			bool flag = !this.mesh.behavior.Poly;
			Triangle dummytri = this.mesh.dummytri;
			startghost.Lprev(ref otri);
			otri.Sym();
			dummytri.neighbors[0] = otri;
			startghost.Copy(ref otri2);
			int num = 0;
			do
			{
				num++;
				otri2.Lnext(ref otri3);
				otri2.Lprev();
				otri2.Sym();
				if (flag && otri2.tri.id != -1)
				{
					Vertex vertex = otri2.Org();
					if (vertex.label == 0)
					{
						vertex.label = 1;
					}
				}
				otri2.Dissolve(dummytri);
				otri3.Sym(ref otri2);
				this.mesh.TriangleDealloc(otri3.tri);
			}
			while (!otri2.Equals(startghost));
			return num;
		}

		// Token: 0x04000ADE RID: 2782
		private static int randomseed = 1;

		// Token: 0x04000ADF RID: 2783
		private static int SAMPLERATE = 10;

		// Token: 0x04000AE0 RID: 2784
		private IPredicates predicates;

		// Token: 0x04000AE1 RID: 2785
		private Mesh mesh;

		// Token: 0x04000AE2 RID: 2786
		private double xminextreme;

		// Token: 0x04000AE3 RID: 2787
		private List<SweepLine.SplayNode> splaynodes;

		// Token: 0x02000132 RID: 306
		private class SweepEvent
		{
			// Token: 0x04000AE4 RID: 2788
			public double xkey;

			// Token: 0x04000AE5 RID: 2789
			public double ykey;

			// Token: 0x04000AE6 RID: 2790
			public Vertex vertexEvent;

			// Token: 0x04000AE7 RID: 2791
			public Otri otriEvent;

			// Token: 0x04000AE8 RID: 2792
			public int heapposition;
		}

		// Token: 0x02000133 RID: 307
		private class SweepEventVertex : Vertex
		{
			// Token: 0x06000A9F RID: 2719 RVA: 0x00049E96 File Offset: 0x00048096
			public SweepEventVertex(SweepLine.SweepEvent e)
			{
				this.evt = e;
			}

			// Token: 0x04000AE9 RID: 2793
			public SweepLine.SweepEvent evt;
		}

		// Token: 0x02000134 RID: 308
		private class SplayNode
		{
			// Token: 0x04000AEA RID: 2794
			public Otri keyedge;

			// Token: 0x04000AEB RID: 2795
			public Vertex keydest;

			// Token: 0x04000AEC RID: 2796
			public SweepLine.SplayNode lchild;

			// Token: 0x04000AED RID: 2797
			public SweepLine.SplayNode rchild;
		}
	}
}
