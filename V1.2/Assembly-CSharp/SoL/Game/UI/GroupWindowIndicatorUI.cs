using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.UI;
using TMPro;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x02000889 RID: 2185
	public abstract class GroupWindowIndicatorUI : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06003F9B RID: 16283
		protected abstract string GetLabelText();

		// Token: 0x06003F9C RID: 16284
		protected abstract string GetTooltipText();

		// Token: 0x17000EB6 RID: 3766
		// (get) Token: 0x06003F9D RID: 16285 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool m_alwaysShowTooltip
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000EB7 RID: 3767
		// (get) Token: 0x06003F9E RID: 16286 RVA: 0x0006B067 File Offset: 0x00069267
		// (set) Token: 0x06003F9F RID: 16287 RVA: 0x001892F8 File Offset: 0x001874F8
		public int Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
				string labelText = this.GetLabelText();
				this.m_text.ZStringSetText(labelText);
				this.m_tooltipText = this.GetTooltipText();
				this.m_text.raycastTarget = (this.m_alwaysShowTooltip || (this.m_value > 0 && !string.IsNullOrEmpty(labelText)));
			}
		}

		// Token: 0x06003FA0 RID: 16288 RVA: 0x0006B06F File Offset: 0x0006926F
		private ITooltipParameter GetTooltipParameter()
		{
			if (!this.m_alwaysShowTooltip && this.Value <= 0)
			{
				return null;
			}
			return new ObjectTextTooltipParameter(this, this.m_tooltipText, false);
		}

		// Token: 0x17000EB8 RID: 3768
		// (get) Token: 0x06003FA1 RID: 16289 RVA: 0x0006B096 File Offset: 0x00069296
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000EB9 RID: 3769
		// (get) Token: 0x06003FA2 RID: 16290 RVA: 0x0006B0A4 File Offset: 0x000692A4
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000EBA RID: 3770
		// (get) Token: 0x06003FA3 RID: 16291 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06003FA5 RID: 16293 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003D56 RID: 15702
		[SerializeField]
		private TextMeshProUGUI m_text;

		// Token: 0x04003D57 RID: 15703
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003D58 RID: 15704
		private string m_tooltipText = string.Empty;

		// Token: 0x04003D59 RID: 15705
		private int m_value;
	}
}
