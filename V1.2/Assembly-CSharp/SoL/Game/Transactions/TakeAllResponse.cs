using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Transactions
{
	// Token: 0x02000637 RID: 1591
	public struct TakeAllResponse : INetworkSerializable
	{
		// Token: 0x17000A9D RID: 2717
		// (get) Token: 0x060031D6 RID: 12758 RVA: 0x000626B3 File Offset: 0x000608B3
		// (set) Token: 0x060031D7 RID: 12759 RVA: 0x000626BB File Offset: 0x000608BB
		public UniqueId TransactionId { readonly get; set; }

		// Token: 0x060031D8 RID: 12760 RVA: 0x0015DD88 File Offset: 0x0015BF88
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.TransactionId);
			buffer.AddEnum(this.Op);
			buffer.AddString(this.SourceContainerId);
			buffer.AddULong(this.Currency);
			buffer.AddInt(this.Items.Length);
			if (this.Items.Length != 0)
			{
				for (int i = 0; i < this.Items.Length; i++)
				{
					this.Items[i].PackData(buffer);
				}
			}
			return buffer;
		}

		// Token: 0x060031D9 RID: 12761 RVA: 0x0015DE08 File Offset: 0x0015C008
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.TransactionId = buffer.ReadUniqueId();
			this.Op = buffer.ReadEnum<OpCodes>();
			this.SourceContainerId = buffer.ReadString();
			this.Currency = buffer.ReadULong();
			int num = buffer.ReadInt();
			if (num > 0)
			{
				this.Items = new TakeAllItem[num];
				for (int i = 0; i < num; i++)
				{
					TakeAllItem takeAllItem = default(TakeAllItem);
					takeAllItem.ReadData(buffer);
					this.Items[i] = takeAllItem;
				}
			}
			return buffer;
		}

		// Token: 0x04003071 RID: 12401
		public OpCodes Op;

		// Token: 0x04003072 RID: 12402
		public string SourceContainerId;

		// Token: 0x04003073 RID: 12403
		public ulong Currency;

		// Token: 0x04003074 RID: 12404
		public TakeAllItem[] Items;
	}
}
