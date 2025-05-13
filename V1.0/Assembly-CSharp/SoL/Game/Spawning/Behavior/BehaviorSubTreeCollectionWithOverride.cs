using System;
using System.Collections;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Spawning.Behavior
{
	// Token: 0x020006E0 RID: 1760
	[Serializable]
	public class BehaviorSubTreeCollectionWithOverride : BehaviorSubTreeCollection
	{
		// Token: 0x17000BC4 RID: 3012
		// (get) Token: 0x06003545 RID: 13637 RVA: 0x0006471B File Offset: 0x0006291B
		protected override bool m_showSubTrees
		{
			get
			{
				return this.m_behaviorOverrides == null;
			}
		}

		// Token: 0x17000BC5 RID: 3013
		// (get) Token: 0x06003546 RID: 13638 RVA: 0x00064729 File Offset: 0x00062929
		internal override BehaviorSubTree[] BehaviorSubTrees
		{
			get
			{
				if (!(this.m_behaviorOverrides == null))
				{
					return this.m_behaviorOverrides.BehaviorSubTrees;
				}
				return base.BehaviorSubTrees;
			}
		}

		// Token: 0x06003547 RID: 13639 RVA: 0x0006474B File Offset: 0x0006294B
		private IEnumerable GetScriptables()
		{
			return SolOdinUtilities.GetDropdownItems<BehaviorSubTreeCollectionScriptable>();
		}

		// Token: 0x04003365 RID: 13157
		[SerializeField]
		private BehaviorSubTreeCollectionScriptable m_behaviorOverrides;
	}
}
