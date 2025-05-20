using System;
using SoL.Game.Interactives;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.AuctionHouse
{
	// Token: 0x02000D3B RID: 3387
	public class AuctionHouseForSaleListItemTooltip : MonoBehaviour, ITooltip, IInteractiveBase, ICursor
	{
		// Token: 0x060065E3 RID: 26083 RVA: 0x00084A6F File Offset: 0x00082C6F
		private ITooltipParameter GetTooltipParameter()
		{
			if (!this.m_listItem)
			{
				return null;
			}
			return this.m_listItem.GetInstanceTooltipParameter();
		}

		// Token: 0x17001866 RID: 6246
		// (get) Token: 0x060065E4 RID: 26084 RVA: 0x00084A8B File Offset: 0x00082C8B
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17001867 RID: 6247
		// (get) Token: 0x060065E5 RID: 26085 RVA: 0x00084A99 File Offset: 0x00082C99
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17001868 RID: 6248
		// (get) Token: 0x060065E6 RID: 26086 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001869 RID: 6249
		// (get) Token: 0x060065E7 RID: 26087 RVA: 0x00084AA1 File Offset: 0x00082CA1
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

		// Token: 0x060065E9 RID: 26089 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040058A0 RID: 22688
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x040058A1 RID: 22689
		[SerializeField]
		private AuctionHouseForSaleListItem m_listItem;
	}
}
