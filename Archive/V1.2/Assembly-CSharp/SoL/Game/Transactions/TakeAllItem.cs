using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Transactions
{
	// Token: 0x02000636 RID: 1590
	public struct TakeAllItem : INetworkSerializable
	{
		// Token: 0x060031D4 RID: 12756 RVA: 0x00062662 File Offset: 0x00060862
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.InstanceId);
			buffer.AddString(this.ContainerId);
			buffer.AddInt(this.Index);
			return buffer;
		}

		// Token: 0x060031D5 RID: 12757 RVA: 0x0006268C File Offset: 0x0006088C
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.InstanceId = buffer.ReadUniqueId();
			this.ContainerId = buffer.ReadString();
			this.Index = buffer.ReadInt();
			return buffer;
		}

		// Token: 0x0400306D RID: 12397
		public UniqueId InstanceId;

		// Token: 0x0400306E RID: 12398
		public string ContainerId;

		// Token: 0x0400306F RID: 12399
		public int Index;
	}
}
