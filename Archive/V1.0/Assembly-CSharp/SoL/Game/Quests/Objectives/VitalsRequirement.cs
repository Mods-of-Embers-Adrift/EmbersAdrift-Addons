using System;
using SoL.Utilities;

namespace SoL.Game.Quests.Objectives
{
	// Token: 0x020007B3 RID: 1971
	[Serializable]
	public class VitalsRequirement
	{
		// Token: 0x04003886 RID: 14470
		public VitalType Type;

		// Token: 0x04003887 RID: 14471
		public NumericComparator Comparator;

		// Token: 0x04003888 RID: 14472
		public int Value;
	}
}
