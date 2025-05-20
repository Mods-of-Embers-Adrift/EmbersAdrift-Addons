using System;
using NetStack.Serialization;
using SoL.Game.Objects.Archetypes;
using SoL.Networking;

namespace SoL.Game.Transactions
{
	// Token: 0x02000631 RID: 1585
	public struct TransferRequest : INetworkSerializable, ITransaction
	{
		// Token: 0x17000A9A RID: 2714
		// (get) Token: 0x060031C7 RID: 12743 RVA: 0x00045BCA File Offset: 0x00043DCA
		public TransactionType Type
		{
			get
			{
				return TransactionType.Transfer;
			}
		}

		// Token: 0x060031C8 RID: 12744 RVA: 0x0015DC18 File Offset: 0x0015BE18
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.TransactionId);
			buffer.AddUniqueId(this.InstanceId);
			buffer.AddString(this.SourceContainer);
			buffer.AddString(this.TargetContainer);
			buffer.AddInt(this.TargetIndex);
			return buffer;
		}

		// Token: 0x060031C9 RID: 12745 RVA: 0x00062574 File Offset: 0x00060774
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.TransactionId = buffer.ReadUniqueId();
			this.InstanceId = buffer.ReadUniqueId();
			this.SourceContainer = buffer.ReadString();
			this.TargetContainer = buffer.ReadString();
			this.TargetIndex = buffer.ReadInt();
			return buffer;
		}

		// Token: 0x0400305A RID: 12378
		public UniqueId TransactionId;

		// Token: 0x0400305B RID: 12379
		public UniqueId InstanceId;

		// Token: 0x0400305C RID: 12380
		public string SourceContainer;

		// Token: 0x0400305D RID: 12381
		public string TargetContainer;

		// Token: 0x0400305E RID: 12382
		public int TargetIndex;

		// Token: 0x0400305F RID: 12383
		public ArchetypeInstance Instance;
	}
}
