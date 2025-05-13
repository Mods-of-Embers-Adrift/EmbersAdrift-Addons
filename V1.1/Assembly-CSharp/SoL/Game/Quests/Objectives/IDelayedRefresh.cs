using System;

namespace SoL.Game.Quests.Objectives
{
	// Token: 0x020007A3 RID: 1955
	public interface IDelayedRefresh
	{
		// Token: 0x17000D3E RID: 3390
		// (get) Token: 0x060039A7 RID: 14759
		UniqueId Id { get; }

		// Token: 0x060039A8 RID: 14760
		void ExecuteRefresh();
	}
}
