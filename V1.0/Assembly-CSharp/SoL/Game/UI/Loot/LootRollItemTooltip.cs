using System;
using SoL.Game.Interactives;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.UI.Loot
{
	// Token: 0x02000980 RID: 2432
	public class LootRollItemTooltip : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06004876 RID: 18550 RVA: 0x00070BCA File Offset: 0x0006EDCA
		private ITooltipParameter GetTooltipParameter()
		{
			if (!this.m_lootRollItem)
			{
				return null;
			}
			return this.m_lootRollItem.GetTooltipParameter();
		}

		// Token: 0x1700101A RID: 4122
		// (get) Token: 0x06004877 RID: 18551 RVA: 0x00070BE6 File Offset: 0x0006EDE6
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x1700101B RID: 4123
		// (get) Token: 0x06004878 RID: 18552 RVA: 0x00070BF4 File Offset: 0x0006EDF4
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x1700101C RID: 4124
		// (get) Token: 0x06004879 RID: 18553 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600487B RID: 18555 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040043BC RID: 17340
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x040043BD RID: 17341
		[SerializeField]
		private LootRollItemUI m_lootRollItem;
	}
}
