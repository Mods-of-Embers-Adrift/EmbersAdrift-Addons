using System;

namespace SoL.Game.Notifications
{
	// Token: 0x02000846 RID: 2118
	public static class NotificationPresentationFlagExtensions
	{
		// Token: 0x06003D1D RID: 15645 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this NotificationPresentationFlags a, NotificationPresentationFlags b)
		{
			return (a & b) == b;
		}
	}
}
