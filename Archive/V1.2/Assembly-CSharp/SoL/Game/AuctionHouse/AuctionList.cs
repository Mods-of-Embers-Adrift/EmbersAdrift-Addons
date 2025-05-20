using System;
using System.Collections.Generic;
using NetStack.Serialization;
using SoL.Networking;
using SoL.Networking.Database;
using SoL.Utilities;

namespace SoL.Game.AuctionHouse
{
	// Token: 0x02000D33 RID: 3379
	public struct AuctionList : INetworkSerializable
	{
		// Token: 0x06006586 RID: 25990 RVA: 0x0020D008 File Offset: 0x0020B208
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddInt(this.Auctions.Count);
			for (int i = 0; i < this.Auctions.Count; i++)
			{
				this.Auctions[i].PackData(buffer);
			}
			StaticListPool<AuctionRecord>.ReturnToPool(this.Auctions);
			return buffer;
		}

		// Token: 0x06006587 RID: 25991 RVA: 0x0020D05C File Offset: 0x0020B25C
		public BitBuffer ReadData(BitBuffer buffer)
		{
			int num = buffer.ReadInt();
			this.Auctions = StaticListPool<AuctionRecord>.GetFromPool();
			for (int i = 0; i < num; i++)
			{
				AuctionRecord auctionRecord = new AuctionRecord();
				auctionRecord.ReadData(buffer);
				this.Auctions.Add(auctionRecord);
			}
			return buffer;
		}

		// Token: 0x0400584F RID: 22607
		public List<AuctionRecord> Auctions;
	}
}
