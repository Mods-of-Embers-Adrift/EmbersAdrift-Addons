using System;

namespace SoL.Game.UI
{
	// Token: 0x0200089B RID: 2203
	public static class InputPreventionFlagExtensions
	{
		// Token: 0x0600402D RID: 16429 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this InputPreventionFlags a, InputPreventionFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x0600402E RID: 16430 RVA: 0x000578B5 File Offset: 0x00055AB5
		public static InputPreventionFlags SetBitFlag(this InputPreventionFlags a, InputPreventionFlags b)
		{
			return a | b;
		}

		// Token: 0x0600402F RID: 16431 RVA: 0x000578BA File Offset: 0x00055ABA
		public static InputPreventionFlags UnsetBitFlag(this InputPreventionFlags a, InputPreventionFlags b)
		{
			return a & ~b;
		}

		// Token: 0x06004030 RID: 16432 RVA: 0x0006B74C File Offset: 0x0006994C
		public static bool PreventForUI(this InputPreventionFlags flags)
		{
			return flags.HasBitFlag(InputPreventionFlags.InputField) || flags.HasBitFlag(InputPreventionFlags.GMConsole);
		}

		// Token: 0x06004031 RID: 16433 RVA: 0x0006B761 File Offset: 0x00069961
		public static bool PreventForAutoRun(this InputPreventionFlags flags)
		{
			return flags.HasBitFlag(InputPreventionFlags.HealthState) || flags.HasBitFlag(InputPreventionFlags.Looting);
		}

		// Token: 0x06004032 RID: 16434 RVA: 0x0006B775 File Offset: 0x00069975
		public static bool AllowAutoRun(this InputPreventionFlags flags)
		{
			return !flags.PreventForAutoRun();
		}

		// Token: 0x06004033 RID: 16435 RVA: 0x0006B780 File Offset: 0x00069980
		public static bool PreventForLook(this InputPreventionFlags flags)
		{
			return flags != InputPreventionFlags.HealthState && flags > InputPreventionFlags.None;
		}
	}
}
