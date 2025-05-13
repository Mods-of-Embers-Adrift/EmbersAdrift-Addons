using System;

namespace TriangleNet
{
	// Token: 0x020000E2 RID: 226
	public class Configuration
	{
		// Token: 0x06000828 RID: 2088 RVA: 0x000B08B0 File Offset: 0x000AEAB0
		public Configuration() : this(() => RobustPredicates.Default, () => new TrianglePool())
		{
		}

		// Token: 0x06000829 RID: 2089 RVA: 0x0004877C File Offset: 0x0004697C
		public Configuration(Func<IPredicates> predicates) : this(predicates, () => new TrianglePool())
		{
		}

		// Token: 0x0600082A RID: 2090 RVA: 0x000487A4 File Offset: 0x000469A4
		public Configuration(Func<IPredicates> predicates, Func<TrianglePool> trianglePool)
		{
			this.Predicates = predicates;
			this.TrianglePool = trianglePool;
		}

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x0600082B RID: 2091 RVA: 0x000487BA File Offset: 0x000469BA
		// (set) Token: 0x0600082C RID: 2092 RVA: 0x000487C2 File Offset: 0x000469C2
		public Func<IPredicates> Predicates { get; set; }

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x0600082D RID: 2093 RVA: 0x000487CB File Offset: 0x000469CB
		// (set) Token: 0x0600082E RID: 2094 RVA: 0x000487D3 File Offset: 0x000469D3
		public Func<TrianglePool> TrianglePool { get; set; }
	}
}
