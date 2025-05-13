using System;

namespace SoL.Game.Targeting
{
	// Token: 0x02000659 RID: 1625
	public class TargetableNpc : BaseTargetable
	{
		// Token: 0x17000AC9 RID: 2761
		// (get) Token: 0x0600329C RID: 12956 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool IsNpc
		{
			get
			{
				return true;
			}
		}
	}
}
