using System;

namespace SoL.Game.NPCs.Senses
{
	// Token: 0x0200082E RID: 2094
	[Flags]
	public enum SensorTypeFlags
	{
		// Token: 0x04003B8E RID: 15246
		None = 0,
		// Token: 0x04003B8F RID: 15247
		VisualImmediate = 1,
		// Token: 0x04003B90 RID: 15248
		VisualPeripheral = 2,
		// Token: 0x04003B91 RID: 15249
		Auditory = 4,
		// Token: 0x04003B92 RID: 15250
		Olfactory = 8,
		// Token: 0x04003B93 RID: 15251
		All = 15
	}
}
