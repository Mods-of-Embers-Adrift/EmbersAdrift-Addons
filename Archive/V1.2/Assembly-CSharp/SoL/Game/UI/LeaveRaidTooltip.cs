using System;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x020008A5 RID: 2213
	public class LeaveRaidTooltip : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06004087 RID: 16519 RVA: 0x0018C958 File Offset: 0x0018AB58
		private ITooltipParameter GetParameter()
		{
			string txt = (ClientGameManager.GroupManager && ClientGameManager.GroupManager.IsGrouped && ClientGameManager.GroupManager.IsLeader && ClientGameManager.SocialManager && ClientGameManager.SocialManager.IsInRaid) ? "Leave Raid" : "Only group leaders can leave the raid.";
			return new ObjectTextTooltipParameter(this, txt, false);
		}

		// Token: 0x17000ED1 RID: 3793
		// (get) Token: 0x06004088 RID: 16520 RVA: 0x0006BAC1 File Offset: 0x00069CC1
		public BaseTooltip.GetTooltipParameter GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetParameter);
			}
		}

		// Token: 0x17000ED2 RID: 3794
		// (get) Token: 0x06004089 RID: 16521 RVA: 0x0006BACF File Offset: 0x00069CCF
		public TooltipSettings TooltipSettings
		{
			get
			{
				return this.m_settings;
			}
		}

		// Token: 0x17000ED3 RID: 3795
		// (get) Token: 0x0600408A RID: 16522 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600408C RID: 16524 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003E41 RID: 15937
		[SerializeField]
		private TooltipSettings m_settings;
	}
}
