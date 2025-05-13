using System;
using SoL.Game.Interactives;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x020008E4 RID: 2276
	public class TextTooltipTrigger : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x17000F21 RID: 3873
		// (get) Token: 0x0600429E RID: 17054 RVA: 0x0006CFD4 File Offset: 0x0006B1D4
		// (set) Token: 0x0600429F RID: 17055 RVA: 0x0006CFDC File Offset: 0x0006B1DC
		public string Text
		{
			get
			{
				return this.m_text;
			}
			set
			{
				this.m_text = value;
			}
		}

		// Token: 0x060042A0 RID: 17056 RVA: 0x0006CFE5 File Offset: 0x0006B1E5
		private ITooltipParameter GetParameter()
		{
			if (string.IsNullOrEmpty(this.m_text))
			{
				return null;
			}
			return new ObjectTextTooltipParameter(this, this.m_text, this.m_isOptionsMenu);
		}

		// Token: 0x17000F22 RID: 3874
		// (get) Token: 0x060042A1 RID: 17057 RVA: 0x0006D00D File Offset: 0x0006B20D
		public BaseTooltip.GetTooltipParameter GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetParameter);
			}
		}

		// Token: 0x17000F23 RID: 3875
		// (get) Token: 0x060042A2 RID: 17058 RVA: 0x0006D01B File Offset: 0x0006B21B
		public TooltipSettings TooltipSettings
		{
			get
			{
				return this.m_settings;
			}
		}

		// Token: 0x17000F24 RID: 3876
		// (get) Token: 0x060042A3 RID: 17059 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060042A5 RID: 17061 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003F90 RID: 16272
		[SerializeField]
		private TooltipSettings m_settings;

		// Token: 0x04003F91 RID: 16273
		[SerializeField]
		private bool m_isOptionsMenu;

		// Token: 0x04003F92 RID: 16274
		[TextArea]
		[SerializeField]
		private string m_text;
	}
}
