using System;
using UnityEngine;

namespace SoL.Game.States
{
	// Token: 0x02000666 RID: 1638
	public class StateTrigger : MonoBehaviour
	{
		// Token: 0x04003138 RID: 12600
		[SerializeField]
		private StateTrigger.TriggerType m_triggerOn;

		// Token: 0x04003139 RID: 12601
		[SerializeField]
		private StateSetting[] m_requirements;

		// Token: 0x0400313A RID: 12602
		[SerializeField]
		private StateSetting[] m_toTrigger;

		// Token: 0x0400313B RID: 12603
		[SerializeField]
		private DelayedStateSet[] m_delayedToTrigger;

		// Token: 0x02000667 RID: 1639
		private enum TriggerType
		{
			// Token: 0x0400313D RID: 12605
			AnyChange,
			// Token: 0x0400313E RID: 12606
			ClientChange
		}
	}
}
