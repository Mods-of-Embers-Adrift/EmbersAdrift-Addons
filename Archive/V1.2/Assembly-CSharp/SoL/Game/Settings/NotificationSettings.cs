using System;
using SoL.Game.Discovery;
using SoL.Game.Notifications;

namespace SoL.Game.Settings
{
	// Token: 0x02000732 RID: 1842
	[Serializable]
	public class NotificationSettings
	{
		// Token: 0x040035AD RID: 13741
		public NewFriendRequestNotification NewFriendRequest;

		// Token: 0x040035AE RID: 13742
		public NewGuildInviteNotification NewGuildInvite;

		// Token: 0x040035AF RID: 13743
		public SpecializationAvailableNotification SpecializationAvailable;

		// Token: 0x040035B0 RID: 13744
		public BaseNotification UnreadMail;

		// Token: 0x040035B1 RID: 13745
		public TutorialNotification[] TutorialNotifications;

		// Token: 0x040035B2 RID: 13746
		public DiscoveryProfile TutorialDiscovery1;
	}
}
