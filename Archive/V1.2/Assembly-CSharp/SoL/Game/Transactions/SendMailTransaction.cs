using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Transactions
{
	// Token: 0x02000645 RID: 1605
	public struct SendMailTransaction : INetworkSerializable
	{
		// Token: 0x060031F4 RID: 12788 RVA: 0x0015E544 File Offset: 0x0015C744
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.Recipient);
			buffer.AddString(this.Subject);
			buffer.AddString(this.Message);
			buffer.AddNullableUlong(this.CurrencyAttachment);
			buffer.AddNullableUlong(this.CashOnDelivery);
			return buffer;
		}

		// Token: 0x060031F5 RID: 12789 RVA: 0x000628B8 File Offset: 0x00060AB8
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Recipient = buffer.ReadUniqueId();
			this.Subject = buffer.ReadString();
			this.Message = buffer.ReadString();
			this.CurrencyAttachment = buffer.ReadNullableUlong();
			this.CashOnDelivery = buffer.ReadNullableUlong();
			return buffer;
		}

		// Token: 0x040030A0 RID: 12448
		public UniqueId Recipient;

		// Token: 0x040030A1 RID: 12449
		public string Subject;

		// Token: 0x040030A2 RID: 12450
		public string Message;

		// Token: 0x040030A3 RID: 12451
		public ulong? CurrencyAttachment;

		// Token: 0x040030A4 RID: 12452
		public ulong? CashOnDelivery;
	}
}
