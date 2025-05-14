using System;
using SoL.Game.EffectSystem;
using SoL.UI;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AD0 RID: 2768
	public static class IStatModifierExtensions
	{
		// Token: 0x06005574 RID: 21876 RVA: 0x001DEB4C File Offset: 0x001DCD4C
		public static void FillTooltipBlock(TooltipTextBlock block, int? level, VitalScalingValue[] scalingValues)
		{
			if (scalingValues == null || scalingValues.Length == 0)
			{
				return;
			}
			for (int i = 0; i < scalingValues.Length; i++)
			{
				scalingValues[i].AddToTooltipBlock(block, level);
			}
		}

		// Token: 0x06005575 RID: 21877 RVA: 0x001DEB7C File Offset: 0x001DCD7C
		public static void FillTooltipBlock(TooltipTextBlock block, int? level, StatModifierScaling[] scalingValues)
		{
			if (scalingValues == null || scalingValues.Length == 0)
			{
				return;
			}
			for (int i = 0; i < scalingValues.Length; i++)
			{
				scalingValues[i].AddToTooltipBlock(block, level);
			}
		}
	}
}
