using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.AuctionHouse
{
	// Token: 0x02000D31 RID: 3377
	public struct AuctionRequest : INetworkSerializable
	{
		// Token: 0x06006582 RID: 25986 RVA: 0x0008458C File Offset: 0x0008278C
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.InstanceId);
			buffer.AddNullableUlong(this.BuyNowPrice);
			buffer.AddNullableUlong(this.StartingBid);
			buffer.AddByte(this.Duration);
			return buffer;
		}

		// Token: 0x06006583 RID: 25987 RVA: 0x000845C3 File Offset: 0x000827C3
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.InstanceId = buffer.ReadUniqueId();
			this.BuyNowPrice = buffer.ReadNullableUlong();
			this.StartingBid = buffer.ReadNullableUlong();
			this.Duration = buffer.ReadByte();
			return buffer;
		}

		// Token: 0x04005847 RID: 22599
		public UniqueId InstanceId;

		// Token: 0x04005848 RID: 22600
		public ulong? BuyNowPrice;

		// Token: 0x04005849 RID: 22601
		public ulong? StartingBid;

		// Token: 0x0400584A RID: 22602
		public byte Duration;
	}
}
