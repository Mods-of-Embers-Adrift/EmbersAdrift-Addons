using System;

namespace SoL.Game
{
	// Token: 0x02000586 RID: 1414
	public static class MountPositionExtension
	{
		// Token: 0x06002C3A RID: 11322 RVA: 0x0005EB69 File Offset: 0x0005CD69
		public static bool IsDynamic(this MountPosition position)
		{
			return position == MountPosition.DynamicHand || position == MountPosition.DynamicHip || position == MountPosition.DynamicShoulder;
		}
	}
}
