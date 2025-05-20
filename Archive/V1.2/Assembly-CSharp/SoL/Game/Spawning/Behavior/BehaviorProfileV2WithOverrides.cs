using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Spawning.Behavior
{
	// Token: 0x020006DC RID: 1756
	[Serializable]
	public class BehaviorProfileV2WithOverrides : BehaviorProfileV2
	{
		// Token: 0x17000BC1 RID: 3009
		// (get) Token: 0x06003538 RID: 13624 RVA: 0x000646AB File Offset: 0x000628AB
		protected override bool m_hasOverride
		{
			get
			{
				return this.m_override != null;
			}
		}

		// Token: 0x06003539 RID: 13625 RVA: 0x000646B9 File Offset: 0x000628B9
		public override void PopulateExternalBehaviorTrees(Dictionary<BehaviorTreeNodeName, ExternalBehaviorTree> treeDict)
		{
			if (this.m_override != null)
			{
				this.m_override.PopulateExternalBehaviorTrees(treeDict);
				return;
			}
			base.PopulateExternalBehaviorTrees(treeDict);
		}

		// Token: 0x0600353A RID: 13626 RVA: 0x000646DD File Offset: 0x000628DD
		private IEnumerable GetScriptable()
		{
			return SolOdinUtilities.GetDropdownItems<BehaviorProfileV2Scriptable>();
		}

		// Token: 0x04003360 RID: 13152
		[SerializeField]
		private BehaviorProfileV2Scriptable m_override;
	}
}
