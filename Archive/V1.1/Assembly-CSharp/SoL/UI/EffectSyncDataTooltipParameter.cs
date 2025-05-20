using System;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Archetypes;

namespace SoL.UI
{
	// Token: 0x02000387 RID: 903
	public struct EffectSyncDataTooltipParameter : ITooltipParameter
	{
		// Token: 0x170005EB RID: 1515
		// (get) Token: 0x060018C2 RID: 6338 RVA: 0x00053500 File Offset: 0x00051700
		public TooltipType Type
		{
			get
			{
				return TooltipType.Archetype;
			}
		}

		// Token: 0x04001FD8 RID: 8152
		public EffectSyncData SyncData;

		// Token: 0x04001FD9 RID: 8153
		public BaseArchetype Archetype;
	}
}
