using System;
using Cysharp.Text;

namespace SoL.Game.UI
{
	// Token: 0x0200088A RID: 2186
	public class GroupWindowNearbyUI : GroupWindowIndicatorUI
	{
		// Token: 0x06003FA6 RID: 16294 RVA: 0x0006B0BF File Offset: 0x000692BF
		protected override string GetLabelText()
		{
			if (base.Value > 0)
			{
				return ZString.Format<int>("+{0}", base.Value);
			}
			return string.Empty;
		}

		// Token: 0x06003FA7 RID: 16295 RVA: 0x0006B0E0 File Offset: 0x000692E0
		protected override string GetTooltipText()
		{
			if (base.Value != 1)
			{
				return ZString.Format<int>("{0} party members nearby are providing you with an advancement bonus.", base.Value);
			}
			return ZString.Format<int>("{0} party member nearby is providing you with an advancement bonus.", base.Value);
		}
	}
}
