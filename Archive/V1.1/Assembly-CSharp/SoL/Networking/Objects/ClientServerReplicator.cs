using System;
using SoL.Networking.Replication;

namespace SoL.Networking.Objects
{
	// Token: 0x020004BB RID: 1211
	[Serializable]
	public struct ClientServerReplicator
	{
		// Token: 0x0400262C RID: 9772
		public ClientServerReplicator.SlotType Type;

		// Token: 0x0400262D RID: 9773
		public Replicator Universal;

		// Token: 0x0400262E RID: 9774
		public Replicator Server;

		// Token: 0x0400262F RID: 9775
		public Replicator Client;

		// Token: 0x04002630 RID: 9776
		public Replicator LocalClient;

		// Token: 0x020004BC RID: 1212
		public enum SlotType
		{
			// Token: 0x04002632 RID: 9778
			Universal,
			// Token: 0x04002633 RID: 9779
			Individual
		}
	}
}
