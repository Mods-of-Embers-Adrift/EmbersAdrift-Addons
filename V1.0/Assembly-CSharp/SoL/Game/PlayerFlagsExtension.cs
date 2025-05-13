using System;
using SoL.Game.Messages;
using SoL.Managers;

namespace SoL.Game
{
	// Token: 0x020005B0 RID: 1456
	public static class PlayerFlagsExtension
	{
		// Token: 0x06002DE3 RID: 11747 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this PlayerFlags a, PlayerFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x06002DE4 RID: 11748 RVA: 0x000578B5 File Offset: 0x00055AB5
		public static PlayerFlags SetBitFlag(this PlayerFlags a, PlayerFlags b)
		{
			return a | b;
		}

		// Token: 0x06002DE5 RID: 11749 RVA: 0x000578BA File Offset: 0x00055ABA
		public static PlayerFlags UnsetBitFlag(this PlayerFlags a, PlayerFlags b)
		{
			return a & ~b;
		}

		// Token: 0x06002DE6 RID: 11750 RVA: 0x0005FD0B File Offset: 0x0005DF0B
		public static bool BlockRemoteContainerInteractions(this PlayerFlags flags, bool considerDeath, bool notify = false)
		{
			return (considerDeath && PlayerFlagsExtension.HasBlockingFlag(PlayerFlags.MissingBag, flags, notify, "You cannot interact while missing your bag.")) || PlayerFlagsExtension.HasBlockingFlag(PlayerFlags.InTrade, flags, notify, "You cannot interact while trading.") || PlayerFlagsExtension.HasBlockingFlag(PlayerFlags.RemoteContainer, flags, notify, "You cannot interact while busy.");
		}

		// Token: 0x06002DE7 RID: 11751 RVA: 0x0005FD3F File Offset: 0x0005DF3F
		private static bool HasBlockingFlag(PlayerFlags flagQuery, PlayerFlags playerFlags, bool notify, string notificationText)
		{
			if (playerFlags.HasBitFlag(flagQuery))
			{
				if (notify && GameManager.IsServer)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, notificationText);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06002DE8 RID: 11752 RVA: 0x0005FD64 File Offset: 0x0005DF64
		public static bool CanTrade(this PlayerFlags flags)
		{
			return !flags.HasBitFlag(PlayerFlags.InTrade) && !flags.HasBitFlag(PlayerFlags.MissingBag) && !flags.HasBitFlag(PlayerFlags.RemoteContainer);
		}

		// Token: 0x06002DE9 RID: 11753 RVA: 0x0005FD86 File Offset: 0x0005DF86
		public static bool ExcludedFromLootRoll(this PlayerFlags flags)
		{
			return flags.HasBitFlag(PlayerFlags.MissingBag);
		}

		// Token: 0x04002D51 RID: 11601
		public const string kYouDoNotHaveYourBag = "You do not have your bag!";

		// Token: 0x04002D52 RID: 11602
		public const string kYouArePartOfTrade = "You are currently part of a trade!";

		// Token: 0x04002D53 RID: 11603
		private const string kIsMissingBagNotification = "You cannot interact while missing your bag.";

		// Token: 0x04002D54 RID: 11604
		private const string kInTradeNotification = "You cannot interact while trading.";

		// Token: 0x04002D55 RID: 11605
		private const string kRemoteContainerNotification = "You cannot interact while busy.";
	}
}
