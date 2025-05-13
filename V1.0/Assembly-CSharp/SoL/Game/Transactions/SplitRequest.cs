using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Transactions
{
	// Token: 0x0200062F RID: 1583
	public struct SplitRequest : INetworkSerializable, ITransaction
	{
		// Token: 0x17000A99 RID: 2713
		// (get) Token: 0x060031C2 RID: 12738 RVA: 0x00062532 File Offset: 0x00060732
		public TransactionType Type
		{
			get
			{
				return TransactionType.Split;
			}
		}

		// Token: 0x060031C3 RID: 12739 RVA: 0x0015DB1C File Offset: 0x0015BD1C
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.TransactionId);
			buffer.AddUniqueId(this.InstanceId);
			buffer.AddString(this.SourceContainer);
			buffer.AddString(this.TargetContainer);
			buffer.AddInt(this.SplitCount);
			return buffer;
		}

		// Token: 0x060031C4 RID: 12740 RVA: 0x00062535 File Offset: 0x00060735
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.TransactionId = buffer.ReadUniqueId();
			this.InstanceId = buffer.ReadUniqueId();
			this.SourceContainer = buffer.ReadString();
			this.TargetContainer = buffer.ReadString();
			this.SplitCount = buffer.ReadInt();
			return buffer;
		}

		// Token: 0x04003051 RID: 12369
		public UniqueId TransactionId;

		// Token: 0x04003052 RID: 12370
		public UniqueId InstanceId;

		// Token: 0x04003053 RID: 12371
		public string SourceContainer;

		// Token: 0x04003054 RID: 12372
		public string TargetContainer;

		// Token: 0x04003055 RID: 12373
		public int SplitCount;
	}
}
