using System;
using SoL.Game.Interactives;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x020008F0 RID: 2288
	public class WorldObjectTooltipTrigger : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x17000F3A RID: 3898
		// (get) Token: 0x0600431C RID: 17180 RVA: 0x0006D3E7 File Offset: 0x0006B5E7
		// (set) Token: 0x0600431D RID: 17181 RVA: 0x0006D3EF File Offset: 0x0006B5EF
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

		// Token: 0x0600431E RID: 17182 RVA: 0x0006D3F8 File Offset: 0x0006B5F8
		private ITooltipParameter GetParameter()
		{
			if (string.IsNullOrEmpty(this.m_text))
			{
				return null;
			}
			return new ObjectTextTooltipParameter(this, this.m_text, this.m_isOptionsMenu);
		}

		// Token: 0x17000F3B RID: 3899
		// (get) Token: 0x0600431F RID: 17183 RVA: 0x0006D420 File Offset: 0x0006B620
		public BaseTooltip.GetTooltipParameter GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetParameter);
			}
		}

		// Token: 0x17000F3C RID: 3900
		// (get) Token: 0x06004320 RID: 17184 RVA: 0x0006D42E File Offset: 0x0006B62E
		public TooltipSettings TooltipSettings
		{
			get
			{
				return this.m_settings;
			}
		}

		// Token: 0x17000F3D RID: 3901
		// (get) Token: 0x06004321 RID: 17185 RVA: 0x0006D436 File Offset: 0x0006B636
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionSettings;
			}
		}

		// Token: 0x06004323 RID: 17187 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003FD5 RID: 16341
		[SerializeField]
		private InteractionSettings m_interactionSettings;

		// Token: 0x04003FD6 RID: 16342
		[SerializeField]
		private TooltipSettings m_settings;

		// Token: 0x04003FD7 RID: 16343
		[SerializeField]
		private bool m_isOptionsMenu;

		// Token: 0x04003FD8 RID: 16344
		[TextArea]
		[SerializeField]
		private string m_text;
	}
}
