using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Settings;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x02000849 RID: 2121
	public class DifficultyIndicatorGelUI : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06003D21 RID: 15649 RVA: 0x00181C74 File Offset: 0x0017FE74
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_difficultyIndicator && this.m_difficultyIndicator.GelIconActive)
			{
				string txt = ZString.Format<string>("G.E.L Difficulty:\n{0}", GlobalSettings.Values.Npcs.GetDifficultyChallengeText(this.m_difficultyIndicator.GelDifficulty, this.m_difficultyIndicator.ChallengeRating));
				return new ObjectTextTooltipParameter(this, txt, false);
			}
			return null;
		}

		// Token: 0x17000E1F RID: 3615
		// (get) Token: 0x06003D22 RID: 15650 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E20 RID: 3616
		// (get) Token: 0x06003D23 RID: 15651 RVA: 0x0006966D File Offset: 0x0006786D
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000E21 RID: 3617
		// (get) Token: 0x06003D24 RID: 15652 RVA: 0x0006967B File Offset: 0x0006787B
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06003D26 RID: 15654 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003BF9 RID: 15353
		[SerializeField]
		private DifficultyIndicatorUI m_difficultyIndicator;

		// Token: 0x04003BFA RID: 15354
		[SerializeField]
		private TooltipSettings m_tooltipSettings;
	}
}
