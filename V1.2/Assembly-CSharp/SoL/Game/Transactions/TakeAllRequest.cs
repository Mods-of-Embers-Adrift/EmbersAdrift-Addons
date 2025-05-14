using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Transactions
{
	// Token: 0x02000635 RID: 1589
	public struct TakeAllRequest : INetworkSerializable, ITransaction
	{
		// Token: 0x17000A9C RID: 2716
		// (get) Token: 0x060031D1 RID: 12753 RVA: 0x00053500 File Offset: 0x00051700
		public TransactionType Type
		{
			get
			{
				return TransactionType.TakeAll;
			}
		}

		// Token: 0x060031D2 RID: 12754 RVA: 0x0006262A File Offset: 0x0006082A
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.TransactionId);
			buffer.AddString(this.Source);
			return buffer;
		}

		// Token: 0x060031D3 RID: 12755 RVA: 0x00062647 File Offset: 0x00060847
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.TransactionId = buffer.ReadUniqueId();
			this.Source = buffer.ReadString();
			return buffer;
		}

		// Token: 0x0400306B RID: 12395
		public UniqueId TransactionId;

		// Token: 0x0400306C RID: 12396
		public string Source;
	}
}
