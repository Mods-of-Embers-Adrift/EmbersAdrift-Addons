using System;
using System.Collections.Generic;

namespace SoL.Game.Spawning.Behavior
{
	// Token: 0x020006E3 RID: 1763
	public struct BehaviorTreeNodeNameComparer : IEqualityComparer<BehaviorTreeNodeName>
	{
		// Token: 0x0600354B RID: 13643 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(BehaviorTreeNodeName x, BehaviorTreeNodeName y)
		{
			return x == y;
		}

		// Token: 0x0600354C RID: 13644 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(BehaviorTreeNodeName obj)
		{
			return (int)obj;
		}
	}
}
