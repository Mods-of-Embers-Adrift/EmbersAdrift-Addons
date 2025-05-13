using System;
using System.Collections.Generic;
using TriangleNet.Topology;

namespace TriangleNet.Meshing.Iterators
{
	// Token: 0x02000125 RID: 293
	public class RegionIterator
	{
		// Token: 0x06000A5A RID: 2650 RVA: 0x00049CBF File Offset: 0x00047EBF
		public RegionIterator(Mesh mesh)
		{
			this.region = new List<Triangle>();
		}

		// Token: 0x06000A5B RID: 2651 RVA: 0x000C74D8 File Offset: 0x000C56D8
		public void Process(Triangle triangle, int boundary = 0)
		{
			this.Process(triangle, delegate(Triangle tri)
			{
				tri.label = triangle.label;
				tri.area = triangle.area;
			}, boundary);
		}

		// Token: 0x06000A5C RID: 2652 RVA: 0x000C750C File Offset: 0x000C570C
		public void Process(Triangle triangle, Action<Triangle> action, int boundary = 0)
		{
			if (triangle.id == -1 || Otri.IsDead(triangle))
			{
				return;
			}
			this.region.Add(triangle);
			triangle.infected = true;
			if (boundary == 0)
			{
				this.ProcessRegion(action, (SubSegment seg) => seg.hash == -1);
			}
			else
			{
				this.ProcessRegion(action, (SubSegment seg) => seg.boundary != boundary);
			}
			this.region.Clear();
		}

		// Token: 0x06000A5D RID: 2653 RVA: 0x000C759C File Offset: 0x000C579C
		private void ProcessRegion(Action<Triangle> action, Func<SubSegment, bool> protector)
		{
			Otri otri = default(Otri);
			Otri otri2 = default(Otri);
			Osub osub = default(Osub);
			for (int i = 0; i < this.region.Count; i++)
			{
				otri.tri = this.region[i];
				action(otri.tri);
				otri.orient = 0;
				while (otri.orient < 3)
				{
					otri.Sym(ref otri2);
					otri.Pivot(ref osub);
					if (otri2.tri.id != -1 && !otri2.IsInfected() && protector(osub.seg))
					{
						otri2.Infect();
						this.region.Add(otri2.tri);
					}
					otri.orient++;
				}
			}
			foreach (Triangle triangle in this.region)
			{
				triangle.infected = false;
			}
			this.region.Clear();
		}

		// Token: 0x04000AB6 RID: 2742
		private List<Triangle> region;
	}
}
