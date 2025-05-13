using System;

namespace SoL.Game
{
	// Token: 0x020005CC RID: 1484
	public interface IDayNightToggle
	{
		// Token: 0x17000A0B RID: 2571
		// (get) Token: 0x06002F44 RID: 12100
		DayNightEnableCondition DayNightEnableCondition { get; }

		// Token: 0x06002F45 RID: 12101
		void Toggle(bool isEnabled);
	}
}
