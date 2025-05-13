using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Tools;
using TriangleNet.Topology;

namespace TriangleNet.Meshing.Algorithm
{
	// Token: 0x0200012F RID: 303
	public class Dwyer : ITriangulator
	{
		// Token: 0x06000A85 RID: 2693 RVA: 0x000C7C68 File Offset: 0x000C5E68
		public IMesh Triangulate(IList<Vertex> points, Configuration config)
		{
			this.predicates = config.Predicates();
			this.mesh = new Mesh(config);
			this.mesh.TransferNodes(points);
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			int count = points.Count;
			this.sortarray = new Vertex[count];
			int num = 0;
			foreach (Vertex vertex in points)
			{
				this.sortarray[num++] = vertex;
			}
			VertexSorter.Sort(this.sortarray, 57113);
			num = 0;
			for (int i = 1; i < count; i++)
			{
				if (this.sortarray[num].x == this.sortarray[i].x && this.sortarray[num].y == this.sortarray[i].y)
				{
					if (Log.Verbose)
					{
						Log.Instance.Warning(string.Format("A duplicate vertex appeared and was ignored (ID {0}).", this.sortarray[i].id), "Dwyer.Triangulate()");
					}
					this.sortarray[i].type = VertexType.UndeadVertex;
					this.mesh.undeads++;
				}
				else
				{
					num++;
					this.sortarray[num] = this.sortarray[i];
				}
			}
			num++;
			if (this.UseDwyer)
			{
				VertexSorter.Alternate(this.sortarray, num, 57113);
			}
			this.DivconqRecurse(0, num - 1, 0, ref otri, ref otri2);
			this.mesh.hullsize = this.RemoveGhosts(ref otri);
			return this.mesh;
		}

		// Token: 0x06000A86 RID: 2694 RVA: 0x000C7E18 File Offset: 0x000C6018
		private void MergeHulls(ref Otri farleft, ref Otri innerleft, ref Otri innerright, ref Otri farright, int axis)
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Otri otri3 = default(Otri);
			Otri otri4 = default(Otri);
			Otri otri5 = default(Otri);
			Otri otri6 = default(Otri);
			Otri otri7 = default(Otri);
			Otri otri8 = default(Otri);
			Vertex vertex = innerleft.Dest();
			Vertex vertex2 = innerleft.Apex();
			Vertex vertex3 = innerright.Org();
			Vertex vertex4 = innerright.Apex();
			Vertex vertex5;
			Vertex vertex7;
			if (this.UseDwyer && axis == 1)
			{
				vertex5 = farleft.Org();
				Vertex vertex6 = farleft.Apex();
				vertex7 = farright.Dest();
				Vertex vertex8 = farright.Apex();
				while (vertex6.y < vertex5.y)
				{
					farleft.Lnext();
					farleft.Sym();
					vertex5 = vertex6;
					vertex6 = farleft.Apex();
				}
				innerleft.Sym(ref otri7);
				Vertex vertex9 = otri7.Apex();
				while (vertex9.y > vertex.y)
				{
					otri7.Lnext(ref innerleft);
					vertex2 = vertex;
					vertex = vertex9;
					innerleft.Sym(ref otri7);
					vertex9 = otri7.Apex();
				}
				while (vertex4.y < vertex3.y)
				{
					innerright.Lnext();
					innerright.Sym();
					vertex3 = vertex4;
					vertex4 = innerright.Apex();
				}
				farright.Sym(ref otri7);
				vertex9 = otri7.Apex();
				while (vertex9.y > vertex7.y)
				{
					otri7.Lnext(ref farright);
					vertex7 = vertex9;
					farright.Sym(ref otri7);
					vertex9 = otri7.Apex();
				}
			}
			bool flag;
			do
			{
				flag = false;
				if (this.predicates.CounterClockwise(vertex, vertex2, vertex3) > 0.0)
				{
					innerleft.Lprev();
					innerleft.Sym();
					vertex = vertex2;
					vertex2 = innerleft.Apex();
					flag = true;
				}
				if (this.predicates.CounterClockwise(vertex4, vertex3, vertex) > 0.0)
				{
					innerright.Lnext();
					innerright.Sym();
					vertex3 = vertex4;
					vertex4 = innerright.Apex();
					flag = true;
				}
			}
			while (flag);
			innerleft.Sym(ref otri);
			innerright.Sym(ref otri2);
			this.mesh.MakeTriangle(ref otri8);
			otri8.Bond(ref innerleft);
			otri8.Lnext();
			otri8.Bond(ref innerright);
			otri8.Lnext();
			otri8.SetOrg(vertex3);
			otri8.SetDest(vertex);
			vertex5 = farleft.Org();
			if (vertex == vertex5)
			{
				otri8.Lnext(ref farleft);
			}
			vertex7 = farright.Dest();
			if (vertex3 == vertex7)
			{
				otri8.Lprev(ref farright);
			}
			Vertex vertex10 = vertex;
			Vertex vertex11 = vertex3;
			Vertex vertex12 = otri.Apex();
			Vertex vertex13 = otri2.Apex();
			for (;;)
			{
				bool flag2 = this.predicates.CounterClockwise(vertex12, vertex10, vertex11) <= 0.0;
				bool flag3 = this.predicates.CounterClockwise(vertex13, vertex10, vertex11) <= 0.0;
				if (flag2 && flag3)
				{
					break;
				}
				if (!flag2)
				{
					otri.Lprev(ref otri3);
					otri3.Sym();
					Vertex vertex14 = otri3.Apex();
					if (vertex14 != null)
					{
						bool flag4 = this.predicates.InCircle(vertex10, vertex11, vertex12, vertex14) > 0.0;
						while (flag4)
						{
							otri3.Lnext();
							otri3.Sym(ref otri5);
							otri3.Lnext();
							otri3.Sym(ref otri4);
							otri3.Bond(ref otri5);
							otri.Bond(ref otri4);
							otri.Lnext();
							otri.Sym(ref otri6);
							otri3.Lprev();
							otri3.Bond(ref otri6);
							otri.SetOrg(vertex10);
							otri.SetDest(null);
							otri.SetApex(vertex14);
							otri3.SetOrg(null);
							otri3.SetDest(vertex12);
							otri3.SetApex(vertex14);
							vertex12 = vertex14;
							otri4.Copy(ref otri3);
							vertex14 = otri3.Apex();
							flag4 = (vertex14 != null && this.predicates.InCircle(vertex10, vertex11, vertex12, vertex14) > 0.0);
						}
					}
				}
				if (!flag3)
				{
					otri2.Lnext(ref otri3);
					otri3.Sym();
					Vertex vertex14 = otri3.Apex();
					if (vertex14 != null)
					{
						bool flag4 = this.predicates.InCircle(vertex10, vertex11, vertex13, vertex14) > 0.0;
						while (flag4)
						{
							otri3.Lprev();
							otri3.Sym(ref otri5);
							otri3.Lprev();
							otri3.Sym(ref otri4);
							otri3.Bond(ref otri5);
							otri2.Bond(ref otri4);
							otri2.Lprev();
							otri2.Sym(ref otri6);
							otri3.Lnext();
							otri3.Bond(ref otri6);
							otri2.SetOrg(null);
							otri2.SetDest(vertex11);
							otri2.SetApex(vertex14);
							otri3.SetOrg(vertex13);
							otri3.SetDest(null);
							otri3.SetApex(vertex14);
							vertex13 = vertex14;
							otri4.Copy(ref otri3);
							vertex14 = otri3.Apex();
							flag4 = (vertex14 != null && this.predicates.InCircle(vertex10, vertex11, vertex13, vertex14) > 0.0);
						}
					}
				}
				if (flag2 || (!flag3 && this.predicates.InCircle(vertex12, vertex10, vertex11, vertex13) > 0.0))
				{
					otri8.Bond(ref otri2);
					otri2.Lprev(ref otri8);
					otri8.SetDest(vertex10);
					vertex11 = vertex13;
					otri8.Sym(ref otri2);
					vertex13 = otri2.Apex();
				}
				else
				{
					otri8.Bond(ref otri);
					otri.Lnext(ref otri8);
					otri8.SetOrg(vertex11);
					vertex10 = vertex12;
					otri8.Sym(ref otri);
					vertex12 = otri.Apex();
				}
			}
			this.mesh.MakeTriangle(ref otri3);
			otri3.SetOrg(vertex10);
			otri3.SetDest(vertex11);
			otri3.Bond(ref otri8);
			otri3.Lnext();
			otri3.Bond(ref otri2);
			otri3.Lnext();
			otri3.Bond(ref otri);
			if (this.UseDwyer && axis == 1)
			{
				vertex5 = farleft.Org();
				Vertex vertex6 = farleft.Apex();
				vertex7 = farright.Dest();
				Vertex vertex8 = farright.Apex();
				farleft.Sym(ref otri7);
				Vertex vertex9 = otri7.Apex();
				while (vertex9.x < vertex5.x)
				{
					otri7.Lprev(ref farleft);
					vertex5 = vertex9;
					farleft.Sym(ref otri7);
					vertex9 = otri7.Apex();
				}
				while (vertex8.x > vertex7.x)
				{
					farright.Lprev();
					farright.Sym();
					vertex7 = vertex8;
					vertex8 = farright.Apex();
				}
			}
		}

		// Token: 0x06000A87 RID: 2695 RVA: 0x000C84C4 File Offset: 0x000C66C4
		private void DivconqRecurse(int left, int right, int axis, ref Otri farleft, ref Otri farright)
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Otri otri3 = default(Otri);
			Otri otri4 = default(Otri);
			Otri otri5 = default(Otri);
			Otri otri6 = default(Otri);
			int num = right - left + 1;
			if (num == 2)
			{
				this.mesh.MakeTriangle(ref farleft);
				farleft.SetOrg(this.sortarray[left]);
				farleft.SetDest(this.sortarray[left + 1]);
				this.mesh.MakeTriangle(ref farright);
				farright.SetOrg(this.sortarray[left + 1]);
				farright.SetDest(this.sortarray[left]);
				farleft.Bond(ref farright);
				farleft.Lprev();
				farright.Lnext();
				farleft.Bond(ref farright);
				farleft.Lprev();
				farright.Lnext();
				farleft.Bond(ref farright);
				farright.Lprev(ref farleft);
				return;
			}
			if (num != 3)
			{
				int num2 = num >> 1;
				this.DivconqRecurse(left, left + num2 - 1, 1 - axis, ref farleft, ref otri5);
				this.DivconqRecurse(left + num2, right, 1 - axis, ref otri6, ref farright);
				this.MergeHulls(ref farleft, ref otri5, ref otri6, ref farright, axis);
				return;
			}
			this.mesh.MakeTriangle(ref otri);
			this.mesh.MakeTriangle(ref otri2);
			this.mesh.MakeTriangle(ref otri3);
			this.mesh.MakeTriangle(ref otri4);
			double num3 = this.predicates.CounterClockwise(this.sortarray[left], this.sortarray[left + 1], this.sortarray[left + 2]);
			if (num3 == 0.0)
			{
				otri.SetOrg(this.sortarray[left]);
				otri.SetDest(this.sortarray[left + 1]);
				otri2.SetOrg(this.sortarray[left + 1]);
				otri2.SetDest(this.sortarray[left]);
				otri3.SetOrg(this.sortarray[left + 2]);
				otri3.SetDest(this.sortarray[left + 1]);
				otri4.SetOrg(this.sortarray[left + 1]);
				otri4.SetDest(this.sortarray[left + 2]);
				otri.Bond(ref otri2);
				otri3.Bond(ref otri4);
				otri.Lnext();
				otri2.Lprev();
				otri3.Lnext();
				otri4.Lprev();
				otri.Bond(ref otri4);
				otri2.Bond(ref otri3);
				otri.Lnext();
				otri2.Lprev();
				otri3.Lnext();
				otri4.Lprev();
				otri.Bond(ref otri2);
				otri3.Bond(ref otri4);
				otri2.Copy(ref farleft);
				otri3.Copy(ref farright);
				return;
			}
			otri.SetOrg(this.sortarray[left]);
			otri2.SetDest(this.sortarray[left]);
			otri4.SetOrg(this.sortarray[left]);
			if (num3 > 0.0)
			{
				otri.SetDest(this.sortarray[left + 1]);
				otri2.SetOrg(this.sortarray[left + 1]);
				otri3.SetDest(this.sortarray[left + 1]);
				otri.SetApex(this.sortarray[left + 2]);
				otri3.SetOrg(this.sortarray[left + 2]);
				otri4.SetDest(this.sortarray[left + 2]);
			}
			else
			{
				otri.SetDest(this.sortarray[left + 2]);
				otri2.SetOrg(this.sortarray[left + 2]);
				otri3.SetDest(this.sortarray[left + 2]);
				otri.SetApex(this.sortarray[left + 1]);
				otri3.SetOrg(this.sortarray[left + 1]);
				otri4.SetDest(this.sortarray[left + 1]);
			}
			otri.Bond(ref otri2);
			otri.Lnext();
			otri.Bond(ref otri3);
			otri.Lnext();
			otri.Bond(ref otri4);
			otri2.Lprev();
			otri3.Lnext();
			otri2.Bond(ref otri3);
			otri2.Lprev();
			otri4.Lprev();
			otri2.Bond(ref otri4);
			otri3.Lnext();
			otri4.Lprev();
			otri3.Bond(ref otri4);
			otri2.Copy(ref farleft);
			if (num3 > 0.0)
			{
				otri3.Copy(ref farright);
				return;
			}
			farleft.Lnext(ref farright);
		}

		// Token: 0x06000A88 RID: 2696 RVA: 0x000C8900 File Offset: 0x000C6B00
		private int RemoveGhosts(ref Otri startghost)
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Otri otri3 = default(Otri);
			bool flag = !this.mesh.behavior.Poly;
			startghost.Lprev(ref otri);
			otri.Sym();
			this.mesh.dummytri.neighbors[0] = otri;
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
				otri2.Dissolve(this.mesh.dummytri);
				otri3.Sym(ref otri2);
				this.mesh.TriangleDealloc(otri3.tri);
			}
			while (!otri2.Equals(startghost));
			return num;
		}

		// Token: 0x04000AD9 RID: 2777
		private IPredicates predicates;

		// Token: 0x04000ADA RID: 2778
		public bool UseDwyer = true;

		// Token: 0x04000ADB RID: 2779
		private Vertex[] sortarray;

		// Token: 0x04000ADC RID: 2780
		private Mesh mesh;
	}
}
