using System;
using SoL.Game.Interactives;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x02000381 RID: 897
	public class SteamLoginButtonTooltip : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x060018B3 RID: 6323 RVA: 0x000534AE File Offset: 0x000516AE
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_button && !this.m_button.interactable)
			{
				return new ObjectTextTooltipParameter(this, "Steam could not be initialized!\nPlease restart the game through Steam to use this option.", true);
			}
			return null;
		}

		// Token: 0x170005E6 RID: 1510
		// (get) Token: 0x060018B4 RID: 6324 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170005E7 RID: 1511
		// (get) Token: 0x060018B5 RID: 6325 RVA: 0x000534DD File Offset: 0x000516DD
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170005E8 RID: 1512
		// (get) Token: 0x060018B6 RID: 6326 RVA: 0x000534EB File Offset: 0x000516EB
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x060018B8 RID: 6328 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04001FC5 RID: 8133
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04001FC6 RID: 8134
		[SerializeField]
		private SolButton m_button;
	}
}
