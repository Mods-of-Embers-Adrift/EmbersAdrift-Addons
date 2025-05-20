using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.AuctionHouse
{
	// Token: 0x02000D32 RID: 3378
	public struct UpdateAuction : INetworkSerializable
	{
		// Token: 0x06006584 RID: 25988 RVA: 0x000845F6 File Offset: 0x000827F6
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddString(this.Id);
			buffer.AddNullableString(this.BuyerCharacterId);
			buffer.AddNullableUlong(this.CurrentBid);
			buffer.AddInt(this.BidCount);
			return buffer;
		}

		// Token: 0x06006585 RID: 25989 RVA: 0x0008462D File Offset: 0x0008282D
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Id = buffer.ReadString();
			this.BuyerCharacterId = buffer.ReadNullableString();
			this.CurrentBid = buffer.ReadNullableUlong();
			this.BidCount = buffer.ReadInt();
			return buffer;
		}

		// Token: 0x0400584B RID: 22603
		public string Id;

		// Token: 0x0400584C RID: 22604
		public string BuyerCharacterId;

		// Token: 0x0400584D RID: 22605
		public ulong? CurrentBid;

		// Token: 0x0400584E RID: 22606
		public int BidCount;
	}
}
