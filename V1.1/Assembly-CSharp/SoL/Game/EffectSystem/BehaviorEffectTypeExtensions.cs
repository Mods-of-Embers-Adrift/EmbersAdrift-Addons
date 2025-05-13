using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C4F RID: 3151
	public static class BehaviorEffectTypeExtensions
	{
		// Token: 0x06006110 RID: 24848 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this BehaviorEffectTypeFlags a, BehaviorEffectTypeFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x06006111 RID: 24849 RVA: 0x0008161B File Offset: 0x0007F81B
		public static StatusEffectSubType GetResistChannel(this BehaviorEffectTypes channel)
		{
			switch (channel)
			{
			case BehaviorEffectTypes.Stun:
				return StatusEffectSubType.StatusEffectResist_Stun;
			case BehaviorEffectTypes.Fear:
				return StatusEffectSubType.StatusEffectResist_Fear;
			case BehaviorEffectTypes.Charm:
				return StatusEffectSubType.StatusEffectResist_Charm;
			case BehaviorEffectTypes.Daze:
				return StatusEffectSubType.StatusEffectResist_Daze;
			case BehaviorEffectTypes.Enrage:
				return StatusEffectSubType.StatusEffectResist_Enrage;
			case BehaviorEffectTypes.Confuse:
				return StatusEffectSubType.StatusEffectResist_Confuse;
			default:
				throw new ArgumentException("channel");
			}
		}

		// Token: 0x06006112 RID: 24850 RVA: 0x00081659 File Offset: 0x0007F859
		public static StatType GetBehaviorResistChannel(this BehaviorEffectTypes channel)
		{
			switch (channel)
			{
			case BehaviorEffectTypes.Stun:
				return StatType.ResistStun;
			case BehaviorEffectTypes.Fear:
				return StatType.ResistFear;
			case BehaviorEffectTypes.Daze:
				return StatType.ResistDaze;
			case BehaviorEffectTypes.Enrage:
				return StatType.ResistEnrage;
			case BehaviorEffectTypes.Confuse:
				return StatType.ResistConfuse;
			case BehaviorEffectTypes.Lull:
				return StatType.ResistLull;
			}
			return StatType.None;
		}

		// Token: 0x06006113 RID: 24851 RVA: 0x001FF0C0 File Offset: 0x001FD2C0
		public static BehaviorEffectTypeFlags GetFlagForType(this BehaviorEffectTypes type)
		{
			switch (type)
			{
			case BehaviorEffectTypes.Stun:
				return BehaviorEffectTypeFlags.Stunned;
			case BehaviorEffectTypes.Fear:
				return BehaviorEffectTypeFlags.Feared;
			case BehaviorEffectTypes.Charm:
				return BehaviorEffectTypeFlags.Charmed;
			case BehaviorEffectTypes.Daze:
				return BehaviorEffectTypeFlags.Dazed;
			case BehaviorEffectTypes.Enrage:
				return BehaviorEffectTypeFlags.Enraged;
			case BehaviorEffectTypes.Confuse:
				return BehaviorEffectTypeFlags.Confused;
			case BehaviorEffectTypes.Lull:
				return BehaviorEffectTypeFlags.Lull;
			default:
				throw new ArgumentException("type");
			}
		}

		// Token: 0x06006114 RID: 24852 RVA: 0x00081692 File Offset: 0x0007F892
		public static bool RemoveOnDamage(this BehaviorEffectTypeFlags flags)
		{
			return flags.HasBitFlag(BehaviorEffectTypeFlags.Charmed) || flags.HasBitFlag(BehaviorEffectTypeFlags.Lull);
		}

		// Token: 0x06006115 RID: 24853 RVA: 0x00056E2C File Offset: 0x0005502C
		public static bool RemoveOnSummon(this BehaviorEffectTypeFlags flags)
		{
			return flags > BehaviorEffectTypeFlags.None;
		}

		// Token: 0x06006116 RID: 24854 RVA: 0x000816A7 File Offset: 0x0007F8A7
		public static bool CancelExecutionForFlag(this BehaviorEffectTypeFlags flags)
		{
			return flags.HasBitFlag(BehaviorEffectTypeFlags.Stunned) || flags.HasBitFlag(BehaviorEffectTypeFlags.Dazed);
		}

		// Token: 0x06006117 RID: 24855 RVA: 0x000816BB File Offset: 0x0007F8BB
		public static string CancelExecutionForFlagDescription(this BehaviorEffectTypeFlags flags)
		{
			if (flags.HasBitFlag(BehaviorEffectTypeFlags.Stunned))
			{
				return "Stunned!";
			}
			if (flags.HasBitFlag(BehaviorEffectTypeFlags.Dazed))
			{
				return "Dazed!";
			}
			return string.Empty;
		}

		// Token: 0x06006118 RID: 24856 RVA: 0x000816E0 File Offset: 0x0007F8E0
		public static float GetInitialThreat(this BehaviorEffectTypes behaviorType)
		{
			if (behaviorType == BehaviorEffectTypes.Charm || behaviorType == BehaviorEffectTypes.Lull)
			{
				return 0f;
			}
			return 10f;
		}
	}
}
