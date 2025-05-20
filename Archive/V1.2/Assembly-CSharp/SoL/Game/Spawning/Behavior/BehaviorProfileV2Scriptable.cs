using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine;

namespace SoL.Game.Spawning.Behavior
{
	// Token: 0x020006DD RID: 1757
	[CreateAssetMenu(menuName = "SoL/Profiles/Behavior V2")]
	public class BehaviorProfileV2Scriptable : ScriptableObject
	{
		// Token: 0x0600353C RID: 13628 RVA: 0x000646EC File Offset: 0x000628EC
		public void PopulateExternalBehaviorTrees(Dictionary<BehaviorTreeNodeName, ExternalBehaviorTree> treeDict)
		{
			BehaviorProfileV2 profile = this.m_profile;
			if (profile == null)
			{
				return;
			}
			profile.PopulateExternalBehaviorTrees(treeDict);
		}

		// Token: 0x04003361 RID: 13153
		[SerializeField]
		private BehaviorProfileV2 m_profile;
	}
}
