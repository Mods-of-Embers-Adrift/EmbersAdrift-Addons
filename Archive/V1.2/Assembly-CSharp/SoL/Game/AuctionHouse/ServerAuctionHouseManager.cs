using System;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Networking;
using SoL.Networking.Database;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.AuctionHouse
{
	// Token: 0x02000D42 RID: 3394
	public class ServerAuctionHouseManager : MonoBehaviour
	{
		// Token: 0x0600663C RID: 26172 RVA: 0x00084DBF File Offset: 0x00082FBF
		internal static int GetMaxListingsPerUser(GameEntity entity)
		{
			if (!entity || !entity.Subscriber)
			{
				return 10;
			}
			return 20;
		}

		// Token: 0x0600663D RID: 26173 RVA: 0x00210484 File Offset: 0x0020E684
		internal static ulong GetMinimumBid(ulong? currentBid, int bidCount)
		{
			ulong num = (currentBid != null) ? currentBid.Value : 0UL;
			if (num > 0UL && bidCount > 0)
			{
				int num2 = Mathf.FloorToInt(num * 0.1f);
				num2 = Mathf.Max(1, num2);
				num += (ulong)((long)num2);
			}
			return num;
		}

		// Token: 0x0600663E RID: 26174 RVA: 0x002104CC File Offset: 0x0020E6CC
		internal static ulong GetDepositCost(AuctionRecord auction)
		{
			if (auction != null)
			{
				int duration = (int)(auction.Expiration - auction.Created).TotalDays;
				return ServerAuctionHouseManager.GetDepositCost(auction.Instance, duration);
			}
			return ServerAuctionHouseManager.GetDepositCost(null, 0);
		}

		// Token: 0x0600663F RID: 26175 RVA: 0x0021050C File Offset: 0x0020E70C
		internal static ulong GetDepositCost(ArchetypeInstance instance, int duration)
		{
			ulong num = Mail.GetPostageForInstance(instance);
			if (num < 100UL)
			{
				num = 100UL;
			}
			if (duration > 1)
			{
				float num2 = num * 0.1f;
				duration--;
				num += (ulong)((long)Mathf.CeilToInt(num2 * (float)duration));
			}
			return num;
		}

		// Token: 0x06006640 RID: 26176 RVA: 0x0021054C File Offset: 0x0020E74C
		private static string GetCurrencyString(ulong value)
		{
			return new CurrencyConverter(value).ToString();
		}

		// Token: 0x06006641 RID: 26177 RVA: 0x00210570 File Offset: 0x0020E770
		private static string GetFullCurrencyString(ulong value)
		{
			return new CurrencyConverter(value).GetFullValueString();
		}

		// Token: 0x06006642 RID: 26178 RVA: 0x0004475B File Offset: 0x0004295B
		public void AddListener(GameEntity listener)
		{
		}

		// Token: 0x06006643 RID: 26179 RVA: 0x0004475B File Offset: 0x0004295B
		public void RemoveListener(GameEntity listener)
		{
		}

		// Token: 0x06006644 RID: 26180 RVA: 0x0021058C File Offset: 0x0020E78C
		public AuctionResponse NewAuction(GameEntity source, ContainerInstance auctionOutgoing, InteractiveAuctionHouse interactiveAuctionHouse, ref AuctionRequest auctionRequest)
		{
			return new AuctionResponse
			{
				OpCode = OpCodes.Error,
				DestroyContents = false,
				Message = string.Empty
			};
		}

		// Token: 0x06006645 RID: 26181 RVA: 0x0021058C File Offset: 0x0020E78C
		public AuctionResponse PlaceBid(GameEntity source, InteractiveAuctionHouse interactiveAuctionHouse, string auctionId, ulong newBid)
		{
			return new AuctionResponse
			{
				OpCode = OpCodes.Error,
				DestroyContents = false,
				Message = string.Empty
			};
		}

		// Token: 0x06006646 RID: 26182 RVA: 0x0021058C File Offset: 0x0020E78C
		public AuctionResponse BuyItNow(GameEntity source, InteractiveAuctionHouse interactiveAuctionHouse, string auctionId)
		{
			return new AuctionResponse
			{
				OpCode = OpCodes.Error,
				DestroyContents = false,
				Message = string.Empty
			};
		}

		// Token: 0x06006647 RID: 26183 RVA: 0x0021058C File Offset: 0x0020E78C
		public AuctionResponse CancelAuction(GameEntity source, string auctionId)
		{
			return new AuctionResponse
			{
				OpCode = OpCodes.Error,
				DestroyContents = false,
				Message = string.Empty
			};
		}

		// Token: 0x06006648 RID: 26184 RVA: 0x0021058C File Offset: 0x0020E78C
		public AuctionResponse RefundAuction(GameEntity source, string auctionId)
		{
			return new AuctionResponse
			{
				OpCode = OpCodes.Error,
				DestroyContents = false,
				Message = string.Empty
			};
		}

		// Token: 0x06006649 RID: 26185 RVA: 0x00045BCA File Offset: 0x00043DCA
		private bool CancelAuctionInternal(AuctionRecord auction, bool isRefund)
		{
			return false;
		}

		// Token: 0x040058D5 RID: 22741
		internal const int kNormalDuration = 2;

		// Token: 0x040058D6 RID: 22742
		internal const int kExtendedDuration = 5;

		// Token: 0x040058D7 RID: 22743
		internal const int kMinimumDuration = 1;

		// Token: 0x040058D8 RID: 22744
		internal const int kMaximumDuration = 5;

		// Token: 0x040058D9 RID: 22745
		private const float kDepositIncreasePerDay = 0.1f;

		// Token: 0x040058DA RID: 22746
		private const float kMinBidIncrease = 0.1f;

		// Token: 0x040058DB RID: 22747
		private const float kCadence = 30f;

		// Token: 0x040058DC RID: 22748
		private const int kInitialSize = 512;

		// Token: 0x040058DD RID: 22749
		internal const int kTaxValueInt = 10;

		// Token: 0x040058DE RID: 22750
		private const float kTaxValueFloat = 0.1f;

		// Token: 0x040058DF RID: 22751
		internal const int kSubscriberTaxValueInt = 5;

		// Token: 0x040058E0 RID: 22752
		private const float kSubscriberTaxValueFloat = 0.05f;

		// Token: 0x040058E1 RID: 22753
		private const int kMaxListingsPerUser = 10;

		// Token: 0x040058E2 RID: 22754
		private const int kMaxListingsPerSusbcriber = 20;

		// Token: 0x040058E3 RID: 22755
		private int m_zoneId;

		// Token: 0x040058E4 RID: 22756
		private Dictionary<string, int> m_userAuctionCount;

		// Token: 0x040058E5 RID: 22757
		private Dictionary<string, AuctionRecord> m_auctionsDict;

		// Token: 0x040058E6 RID: 22758
		private List<AuctionRecord> m_auctions;

		// Token: 0x040058E7 RID: 22759
		private bool m_isExpiring;

		// Token: 0x040058E8 RID: 22760
		private List<string> m_expiredAuctions;

		// Token: 0x040058E9 RID: 22761
		private List<GameEntity> m_listeners;

		// Token: 0x040058EA RID: 22762
		private AuctionRecord m_newAuction;

		// Token: 0x040058EB RID: 22763
		private UpdateAuction m_updateAuction;
	}
}
