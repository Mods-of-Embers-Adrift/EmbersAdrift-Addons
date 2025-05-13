using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.SkyDome;
using SoL.UI;
using TMPro;
using UnityEngine;

namespace SoL.Game.UI.Chat
{
	// Token: 0x020009BB RID: 2491
	public class TimeStatusUI : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06004B4F RID: 19279 RVA: 0x00072F4F File Offset: 0x0007114F
		private void Start()
		{
			this.SkyDomeManagerOnSkydomeControllerChanged();
			SkyDomeManager.SkydomeControllerChanged += this.SkyDomeManagerOnSkydomeControllerChanged;
		}

		// Token: 0x06004B50 RID: 19280 RVA: 0x00072F68 File Offset: 0x00071168
		private void OnDestroy()
		{
			SkyDomeManager.SkydomeControllerChanged -= this.SkyDomeManagerOnSkydomeControllerChanged;
		}

		// Token: 0x06004B51 RID: 19281 RVA: 0x00072F7B File Offset: 0x0007117B
		private void SkyDomeManagerOnSkydomeControllerChanged()
		{
			if (SkyDomeManager.SkyDomeController != null)
			{
				this.RefreshDayNightIcon();
				SkyDomeManager.SkyDomeController.DayNightChanged += this.RefreshDayNightIcon;
			}
		}

		// Token: 0x06004B52 RID: 19282 RVA: 0x00072FA0 File Offset: 0x000711A0
		private void RefreshDayNightIcon()
		{
			this.m_iconObj.ZStringSetText(SkyDomeManager.GetSunMoonUnicode());
		}

		// Token: 0x06004B53 RID: 19283 RVA: 0x00072FB2 File Offset: 0x000711B2
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_showTooltip)
			{
				return new ObjectTextTooltipParameter(this, SkyDomeManager.GetFullTimeDisplay(), this.m_isOptionsMenu);
			}
			return null;
		}

		// Token: 0x1700108B RID: 4235
		// (get) Token: 0x06004B54 RID: 19284 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700108C RID: 4236
		// (get) Token: 0x06004B55 RID: 19285 RVA: 0x00072FD4 File Offset: 0x000711D4
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x1700108D RID: 4237
		// (get) Token: 0x06004B56 RID: 19286 RVA: 0x00072FE2 File Offset: 0x000711E2
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06004B58 RID: 19288 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040045DD RID: 17885
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x040045DE RID: 17886
		[SerializeField]
		private TextMeshProUGUI m_iconObj;

		// Token: 0x040045DF RID: 17887
		[SerializeField]
		private bool m_showTooltip;

		// Token: 0x040045E0 RID: 17888
		[SerializeField]
		private bool m_isOptionsMenu;
	}
}
