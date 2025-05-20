using System;
using SoL.Networking.Replication;

namespace SoL.Game
{
	// Token: 0x020005FC RID: 1532
	public class VitalsReplicatorNpc : VitalsReplicator
	{
		// Token: 0x060030FD RID: 12541 RVA: 0x0015BEA4 File Offset: 0x0015A0A4
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.ArmorClassPercent);
			this.ArmorClassPercent.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.HealthPercent);
			this.HealthPercent.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x04002F42 RID: 12098
		public readonly SynchronizedByte HealthPercent = new SynchronizedByte(100);

		// Token: 0x04002F43 RID: 12099
		public readonly SynchronizedByte ArmorClassPercent = new SynchronizedByte(100);
	}
}
