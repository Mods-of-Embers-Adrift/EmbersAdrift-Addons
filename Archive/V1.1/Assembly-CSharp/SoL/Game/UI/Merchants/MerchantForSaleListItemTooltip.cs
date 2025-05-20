using System;
using SoL.Game.Interactives;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.UI.Merchants
{
	// Token: 0x02000972 RID: 2418
	public class MerchantForSaleListItemTooltip : MonoBehaviour, ITooltip, IInteractiveBase, ICursor
	{
		// Token: 0x060047E1 RID: 18401 RVA: 0x00070670 File Offset: 0x0006E870
		private ITooltipParameter GetTooltipParameter()
		{
			if (!this.m_listItem)
			{
				return null;
			}
			return this.m_listItem.GetTooltipParameter();
		}

		// Token: 0x17000FF0 RID: 4080
		// (get) Token: 0x060047E2 RID: 18402 RVA: 0x0007068C File Offset: 0x0006E88C
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000FF1 RID: 4081
		// (get) Token: 0x060047E3 RID: 18403 RVA: 0x0007069A File Offset: 0x0006E89A
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000FF2 RID: 4082
		// (get) Token: 0x060047E4 RID: 18404 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000FF3 RID: 4083
		// (get) Token: 0x060047E5 RID: 18405 RVA: 0x000706A2 File Offset: 0x0006E8A2
		CursorType ICursor.Type
		{
			get
			{
				if (!this.m_listItem)
				{
					return CursorType.MainCursor;
				}
				return this.m_listItem.CursorType;
			}
		}

		// Token: 0x060047E7 RID: 18407 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004368 RID: 17256
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04004369 RID: 17257
		[SerializeField]
		private MerchantForSaleListItem m_listItem;
	}
}
