using System;
using SoL.Game.Interactives;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.UI.Merchants
{
	// Token: 0x02000971 RID: 2417
	public class MerchantForSaleListItemDeathIcon : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x060047DB RID: 18395 RVA: 0x001A80BC File Offset: 0x001A62BC
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_item == null)
			{
				return null;
			}
			string txt = this.m_item.CanPurchaseButNoRoom ? "You have no room to purchase this!" : "You cannot purchase this while your bag!";
			return new ObjectTextTooltipParameter(this, txt, false);
		}

		// Token: 0x17000FED RID: 4077
		// (get) Token: 0x060047DC RID: 18396 RVA: 0x0007065A File Offset: 0x0006E85A
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000FEE RID: 4078
		// (get) Token: 0x060047DD RID: 18397 RVA: 0x00070668 File Offset: 0x0006E868
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000FEF RID: 4079
		// (get) Token: 0x060047DE RID: 18398 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060047E0 RID: 18400 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004366 RID: 17254
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04004367 RID: 17255
		[SerializeField]
		private MerchantForSaleListItem m_item;
	}
}
