using System;
using SoL.Game.Interactives;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.States
{
	// Token: 0x02000664 RID: 1636
	public class StateTooltip : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x060032F3 RID: 13043 RVA: 0x00161BF4 File Offset: 0x0015FDF4
		private void OnValidate()
		{
			if (this.m_stateTooltips != null && this.m_state)
			{
				for (int i = 0; i < this.m_stateTooltips.Length; i++)
				{
					this.m_stateTooltips[i].State = Mathf.Clamp(this.m_stateTooltips[i].State, 0, this.m_state.MaxState);
				}
			}
		}

		// Token: 0x060032F4 RID: 13044 RVA: 0x00161C54 File Offset: 0x0015FE54
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_stateTooltips != null && this.m_state)
			{
				for (int i = 0; i < this.m_stateTooltips.Length; i++)
				{
					if (this.m_stateTooltips[i].State == (int)this.m_state.CurrentState && !string.IsNullOrEmpty(this.m_stateTooltips[i].Tooltip))
					{
						return new ObjectTextTooltipParameter(this, this.m_stateTooltips[i].Tooltip, false);
					}
				}
			}
			return null;
		}

		// Token: 0x17000ADF RID: 2783
		// (get) Token: 0x060032F5 RID: 13045 RVA: 0x00063128 File Offset: 0x00061328
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionSettings;
			}
		}

		// Token: 0x17000AE0 RID: 2784
		// (get) Token: 0x060032F6 RID: 13046 RVA: 0x00063130 File Offset: 0x00061330
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000AE1 RID: 2785
		// (get) Token: 0x060032F7 RID: 13047 RVA: 0x0006313E File Offset: 0x0006133E
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x060032F9 RID: 13049 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003132 RID: 12594
		[SerializeField]
		private InteractionSettings m_interactionSettings;

		// Token: 0x04003133 RID: 12595
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003134 RID: 12596
		[SerializeField]
		private BaseState m_state;

		// Token: 0x04003135 RID: 12597
		[SerializeField]
		private StateTooltip.StateTooltipSetting[] m_stateTooltips;

		// Token: 0x02000665 RID: 1637
		[Serializable]
		private class StateTooltipSetting
		{
			// Token: 0x04003136 RID: 12598
			[Range(0f, 255f)]
			public int State;

			// Token: 0x04003137 RID: 12599
			public string Tooltip;
		}
	}
}
