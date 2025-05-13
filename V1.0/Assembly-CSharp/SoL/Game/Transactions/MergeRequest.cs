using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Transactions
{
	// Token: 0x0200062D RID: 1581
	public struct MergeRequest : INetworkSerializable, ITransaction
	{
		// Token: 0x17000A98 RID: 2712
		// (get) Token: 0x060031BD RID: 12733 RVA: 0x000580DD File Offset: 0x000562DD
		public TransactionType Type
		{
			get
			{
				return TransactionType.Merge;
			}
		}

		// Token: 0x060031BE RID: 12734 RVA: 0x0015DACC File Offset: 0x0015BCCC
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.TransactionId);
			buffer.AddUniqueId(this.SourceInstanceId);
			buffer.AddUniqueId(this.TargetInstanceId);
			buffer.AddString(this.SourceContainer);
			buffer.AddString(this.TargetContainer);
			return buffer;
		}

		// Token: 0x060031BF RID: 12735 RVA: 0x000624A2 File Offset: 0x000606A2
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.TransactionId = buffer.ReadUniqueId();
			this.SourceInstanceId = buffer.ReadUniqueId();
			this.TargetInstanceId = buffer.ReadUniqueId();
			this.SourceContainer = buffer.ReadString();
			this.TargetContainer = buffer.ReadString();
			return buffer;
		}

		// Token: 0x04003047 RID: 12359
		public UniqueId TransactionId;

		// Token: 0x04003048 RID: 12360
		public UniqueId SourceInstanceId;

		// Token: 0x04003049 RID: 12361
		public UniqueId TargetInstanceId;

		// Token: 0x0400304A RID: 12362
		public string SourceContainer;

		// Token: 0x0400304B RID: 12363
		public string TargetContainer;

		// Token: 0x0400304C RID: 12364
		public string LocalSourceDisplayName;

		// Token: 0x0400304D RID: 12365
		public int? LocalSourceQuantity;
	}
}
