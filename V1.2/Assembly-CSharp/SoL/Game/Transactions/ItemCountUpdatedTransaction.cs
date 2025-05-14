using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Transactions
{
	// Token: 0x02000641 RID: 1601
	public struct ItemCountUpdatedTransaction : INetworkSerializable
	{
		// Token: 0x060031EC RID: 12780 RVA: 0x000627D0 File Offset: 0x000609D0
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.InstanceId);
			buffer.AddString(this.Container);
			buffer.AddInt(this.NewCount);
			return buffer;
		}

		// Token: 0x060031ED RID: 12781 RVA: 0x000627FA File Offset: 0x000609FA
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.InstanceId = buffer.ReadUniqueId();
			this.Container = buffer.ReadString();
			this.NewCount = buffer.ReadInt();
			return buffer;
		}

		// Token: 0x04003094 RID: 12436
		public UniqueId InstanceId;

		// Token: 0x04003095 RID: 12437
		public string Container;

		// Token: 0x04003096 RID: 12438
		public int NewCount;
	}
}
