using System;
using SoL.Game.Interactives;
using SoL.Game.SkyDome;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x0200085C RID: 2140
	public class ClockDisplay : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06003DC8 RID: 15816 RVA: 0x00069D82 File Offset: 0x00067F82
		private void Update()
		{
			if (LocalPlayer.GameEntity != null)
			{
				this.UpdateClock();
			}
		}

		// Token: 0x06003DC9 RID: 15817 RVA: 0x001838B8 File Offset: 0x00181AB8
		private void UpdateClock()
		{
			this.m_currentDateTime = SkyDomeManager.GetCorrectedGameDateTime();
			float z = (float)this.m_currentDateTime.Hour / 24f * 2f * -360f;
			float z2 = (float)this.m_currentDateTime.Minute / 60f * -360f;
			this.m_hourHand.rotation = Quaternion.Euler(new Vector3(0f, 0f, z));
			this.m_minuteHand.rotation = Quaternion.Euler(new Vector3(0f, 0f, z2));
		}

		// Token: 0x06003DCA RID: 15818 RVA: 0x00069D97 File Offset: 0x00067F97
		private ITooltipParameter GetTooltipParameter()
		{
			return new ObjectTextTooltipParameter(this, SkyDomeManager.GetGameTimeChatCommand(), false);
		}

		// Token: 0x17000E4D RID: 3661
		// (get) Token: 0x06003DCB RID: 15819 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E4E RID: 3662
		// (get) Token: 0x06003DCC RID: 15820 RVA: 0x00069DAA File Offset: 0x00067FAA
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000E4F RID: 3663
		// (get) Token: 0x06003DCD RID: 15821 RVA: 0x00069DB8 File Offset: 0x00067FB8
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06003DCF RID: 15823 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003C4B RID: 15435
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003C4C RID: 15436
		[SerializeField]
		private RectTransform m_hourHand;

		// Token: 0x04003C4D RID: 15437
		[SerializeField]
		private RectTransform m_minuteHand;

		// Token: 0x04003C4E RID: 15438
		private DateTime m_currentDateTime;
	}
}
