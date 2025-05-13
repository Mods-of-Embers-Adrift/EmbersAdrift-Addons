using System;

namespace SoL.Game.SkyDome
{
	// Token: 0x020006FC RID: 1788
	public class IndoorSkyController : ClientSkyController
	{
		// Token: 0x17000BEB RID: 3051
		// (get) Token: 0x060035E2 RID: 13794 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool AutoInitialize
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000BEC RID: 3052
		// (get) Token: 0x060035E3 RID: 13795 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool IsIndoors
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060035E4 RID: 13796 RVA: 0x00064FAA File Offset: 0x000631AA
		protected override void UpdateIsDayInternal()
		{
			base.UpdateIsDayInternal();
			base.IsDay = (base.SunAltitude > 0f);
		}
	}
}
