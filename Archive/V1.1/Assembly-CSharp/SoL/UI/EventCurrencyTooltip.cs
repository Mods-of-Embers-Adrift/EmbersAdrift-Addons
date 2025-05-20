using System;
using SoL.Game.Interactives;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x0200037D RID: 893
	public class EventCurrencyTooltip : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x0600189A RID: 6298 RVA: 0x0005342F File Offset: 0x0005162F
		private ITooltipParameter GetTooltipParameter()
		{
			return new ObjectTextTooltipParameter(this, "Bloops", false);
		}

		// Token: 0x170005DB RID: 1499
		// (get) Token: 0x0600189B RID: 6299 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170005DC RID: 1500
		// (get) Token: 0x0600189C RID: 6300 RVA: 0x00053442 File Offset: 0x00051642
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170005DD RID: 1501
		// (get) Token: 0x0600189D RID: 6301 RVA: 0x00053450 File Offset: 0x00051650
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x0600189F RID: 6303 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04001FB9 RID: 8121
		[SerializeField]
		private TooltipSettings m_tooltipSettings;
	}
}
