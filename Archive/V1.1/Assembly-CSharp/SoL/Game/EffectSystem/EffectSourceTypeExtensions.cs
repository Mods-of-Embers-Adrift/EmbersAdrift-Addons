using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C6B RID: 3179
	public static class EffectSourceTypeExtensions
	{
		// Token: 0x06006148 RID: 24904 RVA: 0x00081945 File Offset: 0x0007FB45
		public static bool GivesCredit(this EffectSourceType type)
		{
			return type == EffectSourceType.Ability || type == EffectSourceType.Dynamic;
		}
	}
}
