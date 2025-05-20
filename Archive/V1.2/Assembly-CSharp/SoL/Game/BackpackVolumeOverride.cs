using System;
using SoL.Managers;

namespace SoL.Game
{
	// Token: 0x02000601 RID: 1537
	public class BackpackVolumeOverride : BaseVolumeOverride
	{
		// Token: 0x06003111 RID: 12561 RVA: 0x00061C9F File Offset: 0x0005FE9F
		protected override void Register()
		{
			base.Register();
			LocalZoneManager.RegisterBackpackVolumeOverride(this);
		}

		// Token: 0x06003112 RID: 12562 RVA: 0x00061CAD File Offset: 0x0005FEAD
		protected override void Deregister()
		{
			base.Deregister();
			LocalZoneManager.DeregisterBackpackVolume(this);
		}
	}
}
