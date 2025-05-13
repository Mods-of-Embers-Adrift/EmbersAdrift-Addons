using System;
using SoL.Game.UI.Social;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Notifications
{
	// Token: 0x02000842 RID: 2114
	[CreateAssetMenu(menuName = "SoL/Notifications/New Guild Invite")]
	public class NewGuildInviteNotification : BaseNotification
	{
		// Token: 0x17000E1B RID: 3611
		// (get) Token: 0x06003D13 RID: 15635 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool CanOpen
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003D14 RID: 15636 RVA: 0x0006963A File Offset: 0x0006783A
		public override void Open(object data = null)
		{
			UIManager uimanager = ClientGameManager.UIManager;
			if (uimanager == null)
			{
				return;
			}
			SocialUI socialUI = uimanager.SocialUI;
			if (socialUI == null)
			{
				return;
			}
			socialUI.ShowTab(SocialTab.Guild);
		}
	}
}
