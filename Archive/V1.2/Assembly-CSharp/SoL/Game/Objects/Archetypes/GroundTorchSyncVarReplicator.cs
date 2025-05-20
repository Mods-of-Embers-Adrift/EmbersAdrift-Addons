using System;
using SoL.Networking.Replication;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A68 RID: 2664
	public class GroundTorchSyncVarReplicator : SyncVarReplicator
	{
		// Token: 0x06005287 RID: 21127 RVA: 0x001D4244 File Offset: 0x001D2444
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.Data);
			this.Data.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x040049CB RID: 18891
		public readonly SynchronizedStruct<GroundTorchData> Data = new SynchronizedStruct<GroundTorchData>();
	}
}
