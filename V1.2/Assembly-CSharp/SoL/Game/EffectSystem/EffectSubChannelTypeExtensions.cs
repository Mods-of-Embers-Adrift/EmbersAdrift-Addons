using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C6E RID: 3182
	public static class EffectSubChannelTypeExtensions
	{
		// Token: 0x0400548A RID: 21642
		public static readonly EffectSubChannelType[] DamageSubChannels = new EffectSubChannelType[]
		{
			EffectSubChannelType.Melee,
			EffectSubChannelType.Ranged,
			EffectSubChannelType.Natural,
			EffectSubChannelType.Elemental
		};

		// Token: 0x0400548B RID: 21643
		public static readonly EffectSubChannelType[] RegenSubChannels = new EffectSubChannelType[]
		{
			EffectSubChannelType.Health,
			EffectSubChannelType.Stamina
		};
	}
}
