using System;
using SoL.Game.UI.Social;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Notifications
{
	// Token: 0x02000841 RID: 2113
	[CreateAssetMenu(menuName = "SoL/Notifications/New Friend Request")]
	public class NewFriendRequestNotification : BaseNotification
	{
		// Token: 0x17000E1A RID: 3610
		// (get) Token: 0x06003D10 RID: 15632 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool CanOpen
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003D11 RID: 15633 RVA: 0x00069616 File Offset: 0x00067816
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
			socialUI.ShowTab(FriendsTab.Pending);
		}
	}
}
