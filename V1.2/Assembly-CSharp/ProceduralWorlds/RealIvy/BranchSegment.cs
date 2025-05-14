using System;
using System.Collections.Generic;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x02000089 RID: 137
	[Serializable]
	public class BranchSegment
	{
		// Token: 0x0400061E RID: 1566
		public List<LeafPoint> leaves;

		// Token: 0x0400061F RID: 1567
		public BranchPoint initSegment;

		// Token: 0x04000620 RID: 1568
		public BranchPoint endSegment;
	}
}
