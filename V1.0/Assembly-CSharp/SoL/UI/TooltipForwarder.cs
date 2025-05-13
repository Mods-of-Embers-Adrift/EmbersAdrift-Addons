using System;
using SoL.Game.Interactives;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x0200038A RID: 906
	public class TooltipForwarder : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x060018CD RID: 6349 RVA: 0x00053573 File Offset: 0x00051773
		private void Awake()
		{
			if (!this.m_target)
			{
				Debug.LogError("Null TARGET for TooltipForwarder on " + base.gameObject.name);
				return;
			}
			this.m_tooltip = this.m_target.GetComponent<ITooltip>();
		}

		// Token: 0x060018CE RID: 6350 RVA: 0x000535AE File Offset: 0x000517AE
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_tooltip != null)
			{
				return this.m_tooltip.GetTooltipParameter();
			}
			return null;
		}

		// Token: 0x170005ED RID: 1517
		// (get) Token: 0x060018CF RID: 6351 RVA: 0x000535CA File Offset: 0x000517CA
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170005EE RID: 1518
		// (get) Token: 0x060018D0 RID: 6352 RVA: 0x000535D8 File Offset: 0x000517D8
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				if (this.m_tooltip != null)
				{
					return this.m_tooltip.TooltipSettings;
				}
				return this.m_dummySettings;
			}
		}

		// Token: 0x170005EF RID: 1519
		// (get) Token: 0x060018D1 RID: 6353 RVA: 0x000535F4 File Offset: 0x000517F4
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				if (this.m_tooltip != null)
				{
					return this.m_tooltip.Settings;
				}
				return null;
			}
		}

		// Token: 0x060018D3 RID: 6355 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04001FDE RID: 8158
		[SerializeField]
		private GameObject m_target;

		// Token: 0x04001FDF RID: 8159
		private ITooltip m_tooltip;

		// Token: 0x04001FE0 RID: 8160
		private TooltipSettings m_dummySettings;
	}
}
