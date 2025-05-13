using System;
using SoL.UI;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B92 RID: 2962
	public interface ITooltip : IInteractiveBase
	{
		// Token: 0x1700155C RID: 5468
		// (get) Token: 0x06005B40 RID: 23360
		BaseTooltip.GetTooltipParameter GetTooltipParameter { get; }

		// Token: 0x1700155D RID: 5469
		// (get) Token: 0x06005B41 RID: 23361
		TooltipSettings TooltipSettings { get; }
	}
}
