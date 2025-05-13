using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C63 RID: 3171
	internal static class CombatEffectEnumExtensions
	{
		// Token: 0x0600613C RID: 24892 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this ExpirationConditionFlags a, ExpirationConditionFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x0600613D RID: 24893 RVA: 0x00081894 File Offset: 0x0007FA94
		public static bool HasDelay(this DeliveryMethodTypes deliveryType)
		{
			return deliveryType - DeliveryMethodTypes.Delayed <= 1;
		}
	}
}
