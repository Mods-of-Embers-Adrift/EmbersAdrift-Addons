using System;
using SoL.Game.Discovery;
using SoL.Game.Interactives;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x0200038B RID: 907
	public class TooltipForwarderDiscovery : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x060018D4 RID: 6356 RVA: 0x00105750 File Offset: 0x00103950
		private void Awake()
		{
			MapDiscovery componentInParent = base.gameObject.GetComponentInParent<MapDiscovery>();
			if (componentInParent == null)
			{
				Debug.LogError("Null TARGET for TooltipForwarderDiscovery on " + base.gameObject.name);
				return;
			}
			this.m_tooltip = componentInParent;
		}

		// Token: 0x060018D5 RID: 6357 RVA: 0x0005360B File Offset: 0x0005180B
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_tooltip != null)
			{
				return this.m_tooltip.GetTooltipParameter();
			}
			return null;
		}

		// Token: 0x170005F0 RID: 1520
		// (get) Token: 0x060018D6 RID: 6358 RVA: 0x00053627 File Offset: 0x00051827
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170005F1 RID: 1521
		// (get) Token: 0x060018D7 RID: 6359 RVA: 0x00053635 File Offset: 0x00051835
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

		// Token: 0x170005F2 RID: 1522
		// (get) Token: 0x060018D8 RID: 6360 RVA: 0x00053651 File Offset: 0x00051851
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

		// Token: 0x060018DA RID: 6362 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04001FE1 RID: 8161
		private ITooltip m_tooltip;

		// Token: 0x04001FE2 RID: 8162
		private TooltipSettings m_dummySettings;
	}
}
