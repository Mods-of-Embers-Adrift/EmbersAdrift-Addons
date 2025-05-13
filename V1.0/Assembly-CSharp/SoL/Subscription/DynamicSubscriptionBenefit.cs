using System;
using Cysharp.Text;
using TMPro;
using UnityEngine;

namespace SoL.Subscription
{
	// Token: 0x020003A9 RID: 937
	public class DynamicSubscriptionBenefit : MonoBehaviour
	{
		// Token: 0x06001999 RID: 6553 RVA: 0x0005410D File Offset: 0x0005230D
		private void Start()
		{
			if (!this.m_label)
			{
				return;
			}
			if (this.m_type == DynamicSubscriptionBenefit.BenefitType.CharacterSlots)
			{
				this.m_label.SetTextFormat("{0} additional active characters", 7);
			}
		}

		// Token: 0x04002085 RID: 8325
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x04002086 RID: 8326
		[SerializeField]
		private DynamicSubscriptionBenefit.BenefitType m_type;

		// Token: 0x020003AA RID: 938
		private enum BenefitType
		{
			// Token: 0x04002088 RID: 8328
			None,
			// Token: 0x04002089 RID: 8329
			CharacterSlots
		}
	}
}
