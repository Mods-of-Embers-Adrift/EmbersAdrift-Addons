using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C69 RID: 3177
	public static class EffectResourceTypeExtensions
	{
		// Token: 0x06006147 RID: 24903 RVA: 0x0008193C File Offset: 0x0007FB3C
		public static bool HasWounds(this EffectResourceType type)
		{
			return type <= EffectResourceType.Stamina;
		}
	}
}
