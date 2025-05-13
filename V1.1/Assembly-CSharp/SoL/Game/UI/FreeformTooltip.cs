using System;
using SoL.Game.Interactives;
using SoL.UI;
using TMPro;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x02000887 RID: 2183
	public class FreeformTooltip : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06003F90 RID: 16272 RVA: 0x0006AFDA File Offset: 0x000691DA
		private ITooltipParameter GetTooltipParameter()
		{
			return new ObjectTextTooltipParameter(this, this.m_text.text, false);
		}

		// Token: 0x17000EB2 RID: 3762
		// (get) Token: 0x06003F91 RID: 16273 RVA: 0x0006AFF3 File Offset: 0x000691F3
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000EB3 RID: 3763
		// (get) Token: 0x06003F92 RID: 16274 RVA: 0x0006B001 File Offset: 0x00069201
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000EB4 RID: 3764
		// (get) Token: 0x06003F93 RID: 16275 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06003F95 RID: 16277 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003D52 RID: 15698
		[SerializeField]
		private TextMeshProUGUI m_text;

		// Token: 0x04003D53 RID: 15699
		[SerializeField]
		private TooltipSettings m_tooltipSettings;
	}
}
