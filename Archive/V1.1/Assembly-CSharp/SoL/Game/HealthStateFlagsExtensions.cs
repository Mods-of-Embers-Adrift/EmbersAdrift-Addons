using System;

namespace SoL.Game
{
	// Token: 0x020005EC RID: 1516
	public static class HealthStateFlagsExtensions
	{
		// Token: 0x06002FEA RID: 12266 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this HealthStateFlags a, HealthStateFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x06002FEB RID: 12267 RVA: 0x00061025 File Offset: 0x0005F225
		private static HealthStateFlags GetHealthStateFlagForHealthState(this HealthState state)
		{
			switch (state)
			{
			case HealthState.Alive:
				return HealthStateFlags.Alive;
			case HealthState.Unconscious:
				return HealthStateFlags.Unconscious;
			case HealthState.WakingUp:
				return HealthStateFlags.WakingUp;
			case HealthState.Dead:
				return HealthStateFlags.Dead;
			default:
				return HealthStateFlags.None;
			}
		}

		// Token: 0x06002FEC RID: 12268 RVA: 0x0006104A File Offset: 0x0005F24A
		public static bool MeetsRequirements(this HealthStateFlags allowableFlags, HealthState currentHealthState)
		{
			return allowableFlags == HealthStateFlags.None || allowableFlags.HasBitFlag(currentHealthState.GetHealthStateFlagForHealthState());
		}
	}
}
