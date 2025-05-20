using System;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x0200037F RID: 895
	public class SimpleArchetypeTooltipTrigger : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x170005DE RID: 1502
		// (get) Token: 0x060018A3 RID: 6307 RVA: 0x00053460 File Offset: 0x00051660
		// (set) Token: 0x060018A4 RID: 6308 RVA: 0x00053468 File Offset: 0x00051668
		public BaseArchetype Archetype { get; set; }

		// Token: 0x060018A5 RID: 6309 RVA: 0x00105204 File Offset: 0x00103404
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.Archetype == null)
			{
				return null;
			}
			return new ArchetypeTooltipParameter
			{
				Archetype = this.Archetype,
				Instance = null,
				IsDialogTooltip = this.m_isDialogTooltip
			};
		}

		// Token: 0x170005DF RID: 1503
		// (get) Token: 0x060018A6 RID: 6310 RVA: 0x00053471 File Offset: 0x00051671
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170005E0 RID: 1504
		// (get) Token: 0x060018A7 RID: 6311 RVA: 0x0005347F File Offset: 0x0005167F
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x170005E1 RID: 1505
		// (get) Token: 0x060018A8 RID: 6312 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060018AA RID: 6314 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04001FBF RID: 8127
		[SerializeField]
		private bool m_isDialogTooltip;

		// Token: 0x04001FC0 RID: 8128
		[SerializeField]
		private TooltipSettings m_tooltipSettings;
	}
}
