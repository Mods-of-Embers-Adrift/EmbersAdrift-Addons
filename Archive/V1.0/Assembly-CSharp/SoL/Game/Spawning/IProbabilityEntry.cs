using System;

namespace SoL.Game.Spawning
{
	// Token: 0x0200069D RID: 1693
	public interface IProbabilityEntry
	{
		// Token: 0x17000B2E RID: 2862
		// (get) Token: 0x060033C7 RID: 13255
		float Probability { get; }

		// Token: 0x17000B2F RID: 2863
		// (get) Token: 0x060033C8 RID: 13256
		float NormalizedProbability { get; }

		// Token: 0x17000B30 RID: 2864
		// (get) Token: 0x060033C9 RID: 13257
		// (set) Token: 0x060033CA RID: 13258
		float Threshold { get; set; }

		// Token: 0x060033CB RID: 13259
		void SetTotalProbability(float total);
	}
}
