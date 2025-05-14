using System;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x020008A4 RID: 2212
	public class LeaderIcon : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06004080 RID: 16512 RVA: 0x0018C898 File Offset: 0x0018AA98
		public void Refresh(bool isLeader, bool isRaidLeader)
		{
			this.m_isLeader = isLeader;
			this.m_isRaidLeader = isRaidLeader;
			if (this.m_icon)
			{
				if (this.m_defaultColor == null)
				{
					this.m_defaultColor = new Color?(this.m_icon.color);
				}
				this.m_icon.color = ((this.m_isLeader && this.m_isRaidLeader) ? UIManager.RaidColor : this.m_defaultColor.Value);
			}
			base.gameObject.SetActive(isLeader);
		}

		// Token: 0x06004081 RID: 16513 RVA: 0x0018C91C File Offset: 0x0018AB1C
		private ITooltipParameter GetParameter()
		{
			if (this.m_isLeader)
			{
				string txt = this.m_isRaidLeader ? "Raid Leader" : "Party Leader";
				return new ObjectTextTooltipParameter(this, txt, false);
			}
			return null;
		}

		// Token: 0x17000ECE RID: 3790
		// (get) Token: 0x06004082 RID: 16514 RVA: 0x0006BAAB File Offset: 0x00069CAB
		public BaseTooltip.GetTooltipParameter GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetParameter);
			}
		}

		// Token: 0x17000ECF RID: 3791
		// (get) Token: 0x06004083 RID: 16515 RVA: 0x0006BAB9 File Offset: 0x00069CB9
		public TooltipSettings TooltipSettings
		{
			get
			{
				return this.m_settings;
			}
		}

		// Token: 0x17000ED0 RID: 3792
		// (get) Token: 0x06004084 RID: 16516 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06004086 RID: 16518 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003E3C RID: 15932
		[SerializeField]
		private TooltipSettings m_settings;

		// Token: 0x04003E3D RID: 15933
		[SerializeField]
		private Image m_icon;

		// Token: 0x04003E3E RID: 15934
		private Color? m_defaultColor;

		// Token: 0x04003E3F RID: 15935
		private bool m_isLeader;

		// Token: 0x04003E40 RID: 15936
		private bool m_isRaidLeader;
	}
}
