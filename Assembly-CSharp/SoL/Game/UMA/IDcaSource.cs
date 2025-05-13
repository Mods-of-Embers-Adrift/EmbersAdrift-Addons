using System;

namespace SoL.Game.UMA
{
	// Token: 0x02000620 RID: 1568
	public interface IDcaSource
	{
		// Token: 0x17000A95 RID: 2709
		// (get) Token: 0x060031A4 RID: 12708
		int? Resolution { get; }

		// Token: 0x060031A5 RID: 12709
		void SetResolution(int? resolution, bool update);
	}
}
