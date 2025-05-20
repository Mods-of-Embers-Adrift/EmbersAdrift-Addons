using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C2A RID: 3114
	[Flags]
	public enum DamageCategoriesForTriggerType
	{
		// Token: 0x040052D6 RID: 21206
		None = 0,
		// Token: 0x040052D7 RID: 21207
		Melee = 1,
		// Token: 0x040052D8 RID: 21208
		Ranged = 2,
		// Token: 0x040052D9 RID: 21209
		Ember = 4,
		// Token: 0x040052DA RID: 21210
		Chemical = 8,
		// Token: 0x040052DB RID: 21211
		Mental = 16,
		// Token: 0x040052DC RID: 21212
		All = 31
	}
}
