using System;

namespace SoL.Game.Objects.Archetypes.Abilities
{
	// Token: 0x02000AEB RID: 2795
	public static class AbilityCooldownFlagsExtensions
	{
		// Token: 0x0600561F RID: 22047 RVA: 0x00079717 File Offset: 0x00077917
		public static bool IncreaseCooldown(this AbilityCooldownFlags flag)
		{
			if (flag <= AbilityCooldownFlags.Global)
			{
				if (flag - AbilityCooldownFlags.Regular > 1 && flag != AbilityCooldownFlags.Global)
				{
					return true;
				}
			}
			else if (flag != AbilityCooldownFlags.Memorization && flag != AbilityCooldownFlags.WeaponRuneSwap)
			{
				return true;
			}
			return false;
		}

		// Token: 0x06005620 RID: 22048 RVA: 0x00079735 File Offset: 0x00077935
		public static bool CanUnmemorize(this AbilityCooldownFlags flags)
		{
			return !flags.HasBitFlag(AbilityCooldownFlags.Execution) && !flags.HasBitFlag(AbilityCooldownFlags.Global);
		}

		// Token: 0x06005621 RID: 22049 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this AbilityCooldownFlags a, AbilityCooldownFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x06005622 RID: 22050 RVA: 0x000578B5 File Offset: 0x00055AB5
		public static AbilityCooldownFlags SetBitFlag(this AbilityCooldownFlags a, AbilityCooldownFlags b)
		{
			return a | b;
		}

		// Token: 0x06005623 RID: 22051 RVA: 0x000578BA File Offset: 0x00055ABA
		public static AbilityCooldownFlags UnsetBitFlag(this AbilityCooldownFlags a, AbilityCooldownFlags b)
		{
			return a & ~b;
		}
	}
}
