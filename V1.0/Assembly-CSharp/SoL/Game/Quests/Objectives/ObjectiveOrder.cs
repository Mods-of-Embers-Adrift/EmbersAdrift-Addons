using System;

namespace SoL.Game.Quests.Objectives
{
	// Token: 0x020007AB RID: 1963
	public struct ObjectiveOrder
	{
		// Token: 0x17000D48 RID: 3400
		// (get) Token: 0x060039D0 RID: 14800 RVA: 0x00067314 File Offset: 0x00065514
		// (set) Token: 0x060039D1 RID: 14801 RVA: 0x0006731C File Offset: 0x0006551C
		public UniqueId ParentId { readonly get; set; }

		// Token: 0x17000D49 RID: 3401
		// (get) Token: 0x060039D2 RID: 14802 RVA: 0x00067325 File Offset: 0x00065525
		// (set) Token: 0x060039D3 RID: 14803 RVA: 0x0006732D File Offset: 0x0006552D
		public QuestObjective Objective { readonly get; set; }
	}
}
