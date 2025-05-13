using System;
using SoL.Game.Interactives;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.UI.Skills
{
	// Token: 0x02000925 RID: 2341
	public class AlchemyAbilityTooltip : AlchemyUI, ITooltip, IInteractiveBase
	{
		// Token: 0x060044F0 RID: 17648 RVA: 0x0006E8C1 File Offset: 0x0006CAC1
		private ITooltipParameter GetTooltipParameters()
		{
			if (this.m_abilitySlot)
			{
				return this.m_abilitySlot.GetAlchemyTooltipParameter(this.m_alchemyPowerLevel);
			}
			return null;
		}

		// Token: 0x17000F75 RID: 3957
		// (get) Token: 0x060044F1 RID: 17649 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000F76 RID: 3958
		// (get) Token: 0x060044F2 RID: 17650 RVA: 0x0006E8E3 File Offset: 0x0006CAE3
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameters);
			}
		}

		// Token: 0x17000F77 RID: 3959
		// (get) Token: 0x060044F3 RID: 17651 RVA: 0x0006E8F1 File Offset: 0x0006CAF1
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x060044F5 RID: 17653 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004185 RID: 16773
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04004186 RID: 16774
		[SerializeField]
		private AbilitySlot m_abilitySlot;
	}
}
