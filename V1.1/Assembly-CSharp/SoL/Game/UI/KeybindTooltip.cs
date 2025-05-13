using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x020008A3 RID: 2211
	public class KeybindTooltip : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x17000EC9 RID: 3785
		// (get) Token: 0x06004076 RID: 16502 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool IncludeKeybind
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000ECA RID: 3786
		// (get) Token: 0x06004077 RID: 16503 RVA: 0x0006BA4F File Offset: 0x00069C4F
		// (set) Token: 0x06004078 RID: 16504 RVA: 0x0006BA57 File Offset: 0x00069C57
		public string AdditionalLineText { get; set; }

		// Token: 0x06004079 RID: 16505 RVA: 0x0006BA60 File Offset: 0x00069C60
		private string GetKeybindDescription()
		{
			if (this.m_actionId < 0)
			{
				return string.Empty;
			}
			return BindingLabels.GetAbbreviatedBindingLabel(SolInput.Mapper.GetPrimaryBindingNameForAction(this.m_actionId));
		}

		// Token: 0x0600407A RID: 16506 RVA: 0x0018C7CC File Offset: 0x0018A9CC
		private ITooltipParameter GetTooltipParameter()
		{
			if (string.IsNullOrEmpty(this.m_text))
			{
				return null;
			}
			string text = this.IncludeKeybind ? ZString.Format<string, string>("{0} ({1})", this.m_text, this.GetKeybindDescription()) : this.m_text;
			if (!string.IsNullOrEmpty(this.AdditionalLineText))
			{
				text = ZString.Format<string, string>("{0}\n{1}", text, this.AdditionalLineText);
			}
			return new ObjectTextTooltipParameter(this, text, false);
		}

		// Token: 0x17000ECB RID: 3787
		// (get) Token: 0x0600407B RID: 16507 RVA: 0x0006BA86 File Offset: 0x00069C86
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000ECC RID: 3788
		// (get) Token: 0x0600407C RID: 16508 RVA: 0x0006BA94 File Offset: 0x00069C94
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000ECD RID: 3789
		// (get) Token: 0x0600407D RID: 16509 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600407F RID: 16511 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003E38 RID: 15928
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003E39 RID: 15929
		[SerializeField]
		private int m_actionId = -1;

		// Token: 0x04003E3A RID: 15930
		[TextArea]
		[SerializeField]
		private string m_text;
	}
}
