using System;
using SoL.Game.Interactives;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.UI.Merchants
{
	// Token: 0x0200096C RID: 2412
	public class MerchantBuybackListItemTooltip : MonoBehaviour, ITooltip, IInteractiveBase, ICursor
	{
		// Token: 0x060047A6 RID: 18342 RVA: 0x000703BB File Offset: 0x0006E5BB
		private ITooltipParameter GetTooltipParameter()
		{
			if (!this.m_listItem)
			{
				return null;
			}
			return this.m_listItem.GetTooltipParameter();
		}

		// Token: 0x17000FE3 RID: 4067
		// (get) Token: 0x060047A7 RID: 18343 RVA: 0x000703D7 File Offset: 0x0006E5D7
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000FE4 RID: 4068
		// (get) Token: 0x060047A8 RID: 18344 RVA: 0x000703E5 File Offset: 0x0006E5E5
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000FE5 RID: 4069
		// (get) Token: 0x060047A9 RID: 18345 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000FE6 RID: 4070
		// (get) Token: 0x060047AA RID: 18346 RVA: 0x000703ED File Offset: 0x0006E5ED
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

		// Token: 0x060047AC RID: 18348 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004351 RID: 17233
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04004352 RID: 17234
		[SerializeField]
		private MerchantBuybackListItem m_listItem;
	}
}
