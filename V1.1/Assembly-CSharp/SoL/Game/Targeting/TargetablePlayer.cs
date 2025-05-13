using System;

namespace SoL.Game.Targeting
{
	// Token: 0x0200065A RID: 1626
	public class TargetablePlayer : BaseTargetable
	{
		// Token: 0x17000ACA RID: 2762
		// (get) Token: 0x0600329E RID: 12958 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool IsPlayer
		{
			get
			{
				return true;
			}
		}
	}
}
