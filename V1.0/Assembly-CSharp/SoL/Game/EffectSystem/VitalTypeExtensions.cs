using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C80 RID: 3200
	public static class VitalTypeExtensions
	{
		// Token: 0x0600617D RID: 24957 RVA: 0x00081BB5 File Offset: 0x0007FDB5
		public static string GetDisplayName(this VitalType type)
		{
			if (type == VitalType.Health)
			{
				return "Health";
			}
			if (type != VitalType.ArmorWeightCapacity)
			{
				return "None";
			}
			return "Armor Weight Capacity";
		}
	}
}
