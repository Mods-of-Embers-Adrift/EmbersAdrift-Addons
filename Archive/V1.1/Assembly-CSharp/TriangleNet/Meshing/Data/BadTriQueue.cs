using System;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.Meshing.Data
{
	// Token: 0x0200012E RID: 302
	internal class BadTriQueue
	{
		// Token: 0x17000331 RID: 817
		// (get) Token: 0x06000A80 RID: 2688 RVA: 0x00049E43 File Offset: 0x00048043
		public int Count
		{
			get
			{
				return this.count;
			}
		}

		// Token: 0x06000A81 RID: 2689 RVA: 0x000C7A1C File Offset: 0x000C5C1C
		public BadTriQueue()
		{
			this.queuefront = new BadTriangle[4096];
			this.queuetail = new BadTriangle[4096];
			this.nextnonemptyq = new int[4096];
			this.firstnonemptyq = -1;
			this.count = 0;
		}

		// Token: 0x06000A82 RID: 2690 RVA: 0x000C7A70 File Offset: 0x000C5C70
		public void Enqueue(BadTriangle badtri)
		{
			this.count++;
			double num;
			int num2;
			if (badtri.key >= 1.0)
			{
				num = badtri.key;
				num2 = 1;
			}
			else
			{
				num = 1.0 / badtri.key;
				num2 = 0;
			}
			int num3 = 0;
			while (num > 2.0)
			{
				int num4 = 1;
				double num5 = 0.5;
				while (num * num5 * num5 > 1.0)
				{
					num4 *= 2;
					num5 *= num5;
				}
				num3 += num4;
				num *= num5;
			}
			num3 = 2 * num3 + ((num > 1.4142135623730951) ? 1 : 0);
			int num6;
			if (num2 > 0)
			{
				num6 = 2047 - num3;
			}
			else
			{
				num6 = 2048 + num3;
			}
			if (this.queuefront[num6] == null)
			{
				if (num6 > this.firstnonemptyq)
				{
					this.nextnonemptyq[num6] = this.firstnonemptyq;
					this.firstnonemptyq = num6;
				}
				else
				{
					int num7 = num6 + 1;
					while (this.queuefront[num7] == null)
					{
						num7++;
					}
					this.nextnonemptyq[num6] = this.nextnonemptyq[num7];
					this.nextnonemptyq[num7] = num6;
				}
				this.queuefront[num6] = badtri;
			}
			else
			{
				this.queuetail[num6].next = badtri;
			}
			this.queuetail[num6] = badtri;
			badtri.next = null;
		}

		// Token: 0x06000A83 RID: 2691 RVA: 0x000C7BB8 File Offset: 0x000C5DB8
		public void Enqueue(ref Otri enqtri, double minedge, Vertex apex, Vertex org, Vertex dest)
		{
			this.Enqueue(new BadTriangle
			{
				poortri = enqtri,
				key = minedge,
				apex = apex,
				org = org,
				dest = dest
			});
		}

		// Token: 0x06000A84 RID: 2692 RVA: 0x000C7BFC File Offset: 0x000C5DFC
		public BadTriangle Dequeue()
		{
			if (this.firstnonemptyq < 0)
			{
				return null;
			}
			this.count--;
			BadTriangle badTriangle = this.queuefront[this.firstnonemptyq];
			this.queuefront[this.firstnonemptyq] = badTriangle.next;
			if (badTriangle == this.queuetail[this.firstnonemptyq])
			{
				this.firstnonemptyq = this.nextnonemptyq[this.firstnonemptyq];
			}
			return badTriangle;
		}

		// Token: 0x04000AD3 RID: 2771
		private const double SQRT2 = 1.4142135623730951;

		// Token: 0x04000AD4 RID: 2772
		private BadTriangle[] queuefront;

		// Token: 0x04000AD5 RID: 2773
		private BadTriangle[] queuetail;

		// Token: 0x04000AD6 RID: 2774
		private int[] nextnonemptyq;

		// Token: 0x04000AD7 RID: 2775
		private int firstnonemptyq;

		// Token: 0x04000AD8 RID: 2776
		private int count;
	}
}
