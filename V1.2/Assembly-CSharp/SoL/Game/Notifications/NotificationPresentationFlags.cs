using System;

namespace SoL.Game.Notifications
{
	// Token: 0x02000845 RID: 2117
	[Flags]
	public enum NotificationPresentationFlags
	{
		// Token: 0x04003BEF RID: 15343
		None = 0,
		// Token: 0x04003BF0 RID: 15344
		SidePanel = 1,
		// Token: 0x04003BF1 RID: 15345
		CenterToast = 2,
		// Token: 0x04003BF2 RID: 15346
		TutorialPopup = 4,
		// Token: 0x04003BF3 RID: 15347
		UIPips = 8
	}
}
