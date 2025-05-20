using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine;

namespace SoL.Game.Spawning.Behavior
{
	// Token: 0x020006DF RID: 1759
	[Serializable]
	public class BehaviorSubTreeCollection
	{
		// Token: 0x17000BC2 RID: 3010
		// (get) Token: 0x06003541 RID: 13633 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool m_showSubTrees
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000BC3 RID: 3011
		// (get) Token: 0x06003542 RID: 13634 RVA: 0x00064713 File Offset: 0x00062913
		internal virtual BehaviorSubTree[] BehaviorSubTrees
		{
			get
			{
				return this.m_behaviorSubTrees;
			}
		}

		// Token: 0x06003543 RID: 13635 RVA: 0x00166C28 File Offset: 0x00164E28
		public virtual void PopulateSubTrees(Dictionary<BehaviorTreeNodeName, ExternalBehaviorTree> treeDict)
		{
			BehaviorSubTree[] behaviorSubTrees = this.BehaviorSubTrees;
			if (behaviorSubTrees == null || behaviorSubTrees.Length == 0)
			{
				return;
			}
			for (int i = 0; i < behaviorSubTrees.Length; i++)
			{
				if (behaviorSubTrees[i] != null)
				{
					behaviorSubTrees[i].PopulateSubTrees(treeDict);
				}
			}
		}

		// Token: 0x04003364 RID: 13156
		[SerializeField]
		private BehaviorSubTree[] m_behaviorSubTrees;
	}
}
