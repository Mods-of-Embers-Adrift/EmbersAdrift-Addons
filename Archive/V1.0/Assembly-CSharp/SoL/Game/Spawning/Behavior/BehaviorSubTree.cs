using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Spawning.Behavior
{
	// Token: 0x020006DE RID: 1758
	[Serializable]
	public class BehaviorSubTree
	{
		// Token: 0x0600353E RID: 13630 RVA: 0x000646FF File Offset: 0x000628FF
		public void PopulateSubTrees(Dictionary<BehaviorTreeNodeName, ExternalBehaviorTree> treeDict)
		{
			treeDict.AddOrReplace(this.m_nodeName, this.m_subTree);
		}

		// Token: 0x0600353F RID: 13631 RVA: 0x00060BBC File Offset: 0x0005EDBC
		private IEnumerable GetTree()
		{
			return SolOdinUtilities.GetDropdownItems<ExternalBehaviorTree>();
		}

		// Token: 0x04003362 RID: 13154
		[SerializeField]
		private BehaviorTreeNodeName m_nodeName;

		// Token: 0x04003363 RID: 13155
		[SerializeField]
		private ExternalBehaviorTree m_subTree;
	}
}
