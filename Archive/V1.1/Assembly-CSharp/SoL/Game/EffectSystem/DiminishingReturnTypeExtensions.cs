using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C0F RID: 3087
	public static class DiminishingReturnTypeExtensions
	{
		// Token: 0x17001689 RID: 5769
		// (get) Token: 0x06005EFB RID: 24315 RVA: 0x0007FEB5 File Offset: 0x0007E0B5
		public static DiminishingReturnType[] DiminishingReturnTypes
		{
			get
			{
				if (DiminishingReturnTypeExtensions.m_diminishingReturnTypes == null)
				{
					DiminishingReturnTypeExtensions.m_diminishingReturnTypes = (DiminishingReturnType[])Enum.GetValues(typeof(DiminishingReturnType));
				}
				return DiminishingReturnTypeExtensions.m_diminishingReturnTypes;
			}
		}

		// Token: 0x06005EFC RID: 24316 RVA: 0x0007FEDC File Offset: 0x0007E0DC
		public static DiminishingReturnType GetDiminishingReturnType(this StatusEffectType type)
		{
			if (type == StatusEffectType.Haste)
			{
				return DiminishingReturnType.Haste;
			}
			if (type != StatusEffectType.Movement)
			{
				return DiminishingReturnType.None;
			}
			return DiminishingReturnType.Movement;
		}

		// Token: 0x06005EFD RID: 24317 RVA: 0x0007FEEF File Offset: 0x0007E0EF
		public static DiminishingReturnType GetDiminishingReturnType(this BehaviorEffectTypes type)
		{
			switch (type)
			{
			case BehaviorEffectTypes.Stun:
				return DiminishingReturnType.Stun;
			case BehaviorEffectTypes.Fear:
				return DiminishingReturnType.Fear;
			case BehaviorEffectTypes.Daze:
				return DiminishingReturnType.Daze;
			case BehaviorEffectTypes.Enrage:
				return DiminishingReturnType.Enrage;
			case BehaviorEffectTypes.Lull:
				return DiminishingReturnType.Lull;
			}
			return DiminishingReturnType.Behavior;
		}

		// Token: 0x06005EFE RID: 24318 RVA: 0x0007FF20 File Offset: 0x0007E120
		public static float GetDiminishingTime(this DiminishingReturnType type)
		{
			if (type - DiminishingReturnType.Stun <= 1 || type == DiminishingReturnType.Lull)
			{
				return 60f;
			}
			return 30f;
		}

		// Token: 0x04005220 RID: 21024
		private static DiminishingReturnType[] m_diminishingReturnTypes;
	}
}
