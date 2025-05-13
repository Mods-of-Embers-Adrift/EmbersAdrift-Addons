using System;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x02000380 RID: 896
	public class SimpleInstanceTooltipTrigger : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x170005E2 RID: 1506
		// (get) Token: 0x060018AB RID: 6315 RVA: 0x00053487 File Offset: 0x00051687
		// (set) Token: 0x060018AC RID: 6316 RVA: 0x0005348F File Offset: 0x0005168F
		public ArchetypeInstance Instance { get; set; }

		// Token: 0x060018AD RID: 6317 RVA: 0x00105234 File Offset: 0x00103434
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.Instance == null || this.Instance.Archetype == null)
			{
				return null;
			}
			return new ArchetypeTooltipParameter
			{
				Archetype = this.Instance.Archetype,
				Instance = this.Instance,
				IsDialogTooltip = this.m_isDialogTooltip
			};
		}

		// Token: 0x170005E3 RID: 1507
		// (get) Token: 0x060018AE RID: 6318 RVA: 0x00053498 File Offset: 0x00051698
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x170005E4 RID: 1508
		// (get) Token: 0x060018AF RID: 6319 RVA: 0x000534A6 File Offset: 0x000516A6
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x170005E5 RID: 1509
		// (get) Token: 0x060018B0 RID: 6320 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060018B2 RID: 6322 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04001FC2 RID: 8130
		[SerializeField]
		private bool m_isDialogTooltip;

		// Token: 0x04001FC3 RID: 8131
		[SerializeField]
		private TooltipSettings m_tooltipSettings;
	}
}
