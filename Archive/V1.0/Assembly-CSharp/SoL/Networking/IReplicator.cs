using System;
using NetStack.Serialization;
using SoL.Networking.Managers;
using SoL.Networking.Objects;
using SoL.Networking.Replication;

namespace SoL.Networking
{
	// Token: 0x020003D0 RID: 976
	public interface IReplicator
	{
		// Token: 0x1700062D RID: 1581
		// (get) Token: 0x06001A4B RID: 6731
		bool Dirty { get; }

		// Token: 0x1700062E RID: 1582
		// (get) Token: 0x06001A4C RID: 6732
		ReplicatorTypes Type { get; }

		// Token: 0x06001A4D RID: 6733
		void Init(int index, INetworkManager network, NetworkEntity netEntity);

		// Token: 0x06001A4E RID: 6734
		void SetDirtyFlags(DateTime timestamp);

		// Token: 0x06001A4F RID: 6735
		BitBuffer PackData(BitBuffer outBuffer);

		// Token: 0x06001A50 RID: 6736
		BitBuffer ReadData(BitBuffer inBuffer);

		// Token: 0x06001A51 RID: 6737
		BitBuffer PackInitialData(BitBuffer outBuffer);

		// Token: 0x06001A52 RID: 6738
		BitBuffer ReadInitialData(BitBuffer inBuffer);
	}
}
