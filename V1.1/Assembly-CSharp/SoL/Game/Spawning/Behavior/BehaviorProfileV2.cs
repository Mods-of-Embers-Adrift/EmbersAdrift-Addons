using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Spawning.Behavior
{
	// Token: 0x020006DB RID: 1755
	[Serializable]
	public class BehaviorProfileV2
	{
		// Token: 0x17000BC0 RID: 3008
		// (get) Token: 0x06003534 RID: 13620 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool m_hasOverride
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06003535 RID: 13621 RVA: 0x00064675 File Offset: 0x00062875
		public virtual void PopulateExternalBehaviorTrees(Dictionary<BehaviorTreeNodeName, ExternalBehaviorTree> treeDict)
		{
			treeDict.AddOrReplace(BehaviorTreeNodeName.Idle, this.m_idle);
			treeDict.AddOrReplace(BehaviorTreeNodeName.Search, this.m_search);
			treeDict.AddOrReplace(BehaviorTreeNodeName.Neutral, this.m_neutral);
			treeDict.AddOrReplace(BehaviorTreeNodeName.Combat, this.m_combat);
		}

		// Token: 0x06003536 RID: 13622 RVA: 0x00060BBC File Offset: 0x0005EDBC
		private IEnumerable GetExternalTrees()
		{
			return SolOdinUtilities.GetDropdownItems<ExternalBehaviorTree>();
		}

		// Token: 0x0400335B RID: 13147
		protected const string kGroupName = "Behavior Profile";

		// Token: 0x0400335C RID: 13148
		[SerializeField]
		private ExternalBehaviorTree m_idle;

		// Token: 0x0400335D RID: 13149
		[SerializeField]
		private ExternalBehaviorTree m_search;

		// Token: 0x0400335E RID: 13150
		[SerializeField]
		private ExternalBehaviorTree m_neutral;

		// Token: 0x0400335F RID: 13151
		[SerializeField]
		private ExternalBehaviorTree m_combat;
	}
}
