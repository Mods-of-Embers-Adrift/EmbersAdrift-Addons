using System;
using SoL.Game.Interactives;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.GM
{
	// Token: 0x02000BED RID: 3053
	public class DpsMeterListEntry : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06005E54 RID: 24148 RVA: 0x0004475B File Offset: 0x0004295B
		internal void SetData(DpsMeterList controller, DpsMeterInfo info)
		{
		}

		// Token: 0x06005E55 RID: 24149 RVA: 0x00049FFA File Offset: 0x000481FA
		private ITooltipParameter GetTooltipParameter()
		{
			return null;
		}

		// Token: 0x1700164C RID: 5708
		// (get) Token: 0x06005E56 RID: 24150 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700164D RID: 5709
		// (get) Token: 0x06005E57 RID: 24151 RVA: 0x0007F6C7 File Offset: 0x0007D8C7
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x1700164E RID: 5710
		// (get) Token: 0x06005E58 RID: 24152 RVA: 0x0007F6D5 File Offset: 0x0007D8D5
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06005E5A RID: 24154 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x0400519A RID: 20890
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x0400519B RID: 20891
		[SerializeField]
		private TextMeshProUGUI m_title;

		// Token: 0x0400519C RID: 20892
		[SerializeField]
		private TextMeshProUGUI m_dpsLabel;

		// Token: 0x0400519D RID: 20893
		[SerializeField]
		private TextMeshProUGUI m_fractionLabel;

		// Token: 0x0400519E RID: 20894
		[SerializeField]
		private Image m_fraction;

		// Token: 0x0400519F RID: 20895
		private DpsMeterInfo m_info;
	}
}
