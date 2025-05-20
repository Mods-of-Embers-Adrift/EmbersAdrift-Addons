using System;

namespace SoL.Game.UI
{
	// Token: 0x0200089A RID: 2202
	[Flags]
	public enum InputPreventionFlags
	{
		// Token: 0x04003E13 RID: 15891
		None = 0,
		// Token: 0x04003E14 RID: 15892
		HealthState = 1,
		// Token: 0x04003E15 RID: 15893
		InputField = 2,
		// Token: 0x04003E16 RID: 15894
		Looting = 4,
		// Token: 0x04003E17 RID: 15895
		QuestDialog = 8,
		// Token: 0x04003E18 RID: 15896
		GMConsole = 16,
		// Token: 0x04003E19 RID: 15897
		Options = 32
	}
}
