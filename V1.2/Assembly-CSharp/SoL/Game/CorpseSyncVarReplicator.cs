using System;
using SoL.Networking.Replication;

namespace SoL.Game
{
	// Token: 0x0200056B RID: 1387
	public class CorpseSyncVarReplicator : SyncVarReplicator
	{
		// Token: 0x06002AD4 RID: 10964 RVA: 0x00144CE8 File Offset: 0x00142EE8
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.CorpseData);
			this.CorpseData.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.CorpseLocation);
			this.CorpseLocation.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x04002B18 RID: 11032
		public readonly SynchronizedStruct<CorpseData> CorpseData = new SynchronizedStruct<CorpseData>();

		// Token: 0x04002B19 RID: 11033
		public readonly SynchronizedLocation CorpseLocation = new SynchronizedLocation();
	}
}
