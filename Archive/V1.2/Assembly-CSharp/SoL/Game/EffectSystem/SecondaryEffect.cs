using System;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C47 RID: 3143
	[Serializable]
	public class SecondaryEffect
	{
		// Token: 0x17001749 RID: 5961
		// (get) Token: 0x060060EA RID: 24810 RVA: 0x00081411 File Offset: 0x0007F611
		public TargetingParams Targeting
		{
			get
			{
				return this.m_targetParams;
			}
		}

		// Token: 0x1700174A RID: 5962
		// (get) Token: 0x060060EB RID: 24811 RVA: 0x00081419 File Offset: 0x0007F619
		public CombatEffect Effect
		{
			get
			{
				return this.m_effect;
			}
		}

		// Token: 0x04005386 RID: 21382
		[SerializeField]
		private TargetingParams m_targetParams;

		// Token: 0x04005387 RID: 21383
		[SerializeField]
		private CombatEffect m_effect;
	}
}
