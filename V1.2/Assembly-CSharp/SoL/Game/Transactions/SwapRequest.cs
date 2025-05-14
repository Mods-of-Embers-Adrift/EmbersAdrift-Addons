using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Transactions
{
	// Token: 0x02000633 RID: 1587
	public struct SwapRequest : INetworkSerializable, ITransaction
	{
		// Token: 0x17000A9B RID: 2715
		// (get) Token: 0x060031CC RID: 12748 RVA: 0x0004479C File Offset: 0x0004299C
		public TransactionType Type
		{
			get
			{
				return TransactionType.Swap;
			}
		}

		// Token: 0x060031CD RID: 12749 RVA: 0x0015DD34 File Offset: 0x0015BF34
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.TransactionId);
			buffer.AddUniqueId(this.InstanceIdA);
			buffer.AddString(this.InstanceIdB);
			buffer.AddString(this.SourceContainerA);
			buffer.AddString(this.SourceContainerB);
			return buffer;
		}

		// Token: 0x060031CE RID: 12750 RVA: 0x000625B3 File Offset: 0x000607B3
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.TransactionId = buffer.ReadUniqueId();
			this.InstanceIdA = buffer.ReadUniqueId();
			this.InstanceIdB = buffer.ReadUniqueId();
			this.SourceContainerA = buffer.ReadString();
			this.SourceContainerB = buffer.ReadString();
			return buffer;
		}

		// Token: 0x04003064 RID: 12388
		public UniqueId TransactionId;

		// Token: 0x04003065 RID: 12389
		public UniqueId InstanceIdA;

		// Token: 0x04003066 RID: 12390
		public UniqueId InstanceIdB;

		// Token: 0x04003067 RID: 12391
		public string SourceContainerA;

		// Token: 0x04003068 RID: 12392
		public string SourceContainerB;
	}
}
