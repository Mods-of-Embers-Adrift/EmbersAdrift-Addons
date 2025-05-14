using System;
using MongoDB.Bson.Serialization.Attributes;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Subscription;

namespace SoL.Networking
{
	// Token: 0x020003BC RID: 956
	[Serializable]
	public class SteamConfig : ConfigRecordBase
	{
		// Token: 0x060019E2 RID: 6626 RVA: 0x000543CA File Offset: 0x000525CA
		public static string GetPurchaseSubscriptionDescription()
		{
			if (SteamManager.Config == null || string.IsNullOrEmpty(SteamManager.Config.SubscriptionDescription))
			{
				return "Stormhaven Studios is dedicated to keeping Embers Adrift cash shop free! Support our small indie team and help us in the fight against P2W and Microtransactions by subscribing to get these great perks.";
			}
			return SteamManager.Config.SubscriptionDescription;
		}

		// Token: 0x060019E3 RID: 6627 RVA: 0x000543F4 File Offset: 0x000525F4
		public static string GetSubscriptionPerksHeader()
		{
			if (SteamManager.Config == null || string.IsNullOrEmpty(SteamManager.Config.SubscriptionPerkHeader))
			{
				return "<indent=16pt><i>PERKS</i></indent>";
			}
			return SteamManager.Config.SubscriptionPerkHeader;
		}

		// Token: 0x060019E4 RID: 6628 RVA: 0x0005441E File Offset: 0x0005261E
		public static string GetSubscriptionCostDisplay()
		{
			if (SteamManager.Config == null || string.IsNullOrEmpty(SteamManager.Config.SubscriptionCostDisplay))
			{
				return "$9.99 USD / month";
			}
			return SteamManager.Config.SubscriptionCostDisplay;
		}

		// Token: 0x060019E5 RID: 6629 RVA: 0x00054448 File Offset: 0x00052648
		public static string GetAcceptText()
		{
			if (SteamManager.Config == null || string.IsNullOrEmpty(SteamManager.Config.AcceptText))
			{
				return "SUBSCRIBE";
			}
			return SteamManager.Config.AcceptText;
		}

		// Token: 0x060019E6 RID: 6630 RVA: 0x00054472 File Offset: 0x00052672
		public static string GetCancelText()
		{
			if (SteamManager.Config == null || string.IsNullOrEmpty(SteamManager.Config.CancelText))
			{
				return "Cancel";
			}
			return SteamManager.Config.CancelText;
		}

		// Token: 0x060019E7 RID: 6631 RVA: 0x00107B4C File Offset: 0x00105D4C
		public static SubscriptionPerk[] GetSubscriberPerks()
		{
			if (SteamManager.Config == null || SteamManager.Config.SubscriptionPerks == null || SteamManager.Config.SubscriptionPerks.Length == 0)
			{
				return GlobalSettings.Values.Subscribers.DefaultSubscriberPerks.Perks;
			}
			return SteamManager.Config.SubscriptionPerks;
		}

		// Token: 0x040020E2 RID: 8418
		[BsonIgnore]
		private const string kKey = "steam";

		// Token: 0x040020E3 RID: 8419
		[BsonIgnore]
		private const string kDefaultPurchaseSubscriptionDescription = "Stormhaven Studios is dedicated to keeping Embers Adrift cash shop free! Support our small indie team and help us in the fight against P2W and Microtransactions by subscribing to get these great perks.";

		// Token: 0x040020E4 RID: 8420
		[BsonIgnore]
		private const string kDefaultSubscriptionPerksHeader = "<indent=16pt><i>PERKS</i></indent>";

		// Token: 0x040020E5 RID: 8421
		[BsonIgnore]
		private const string kDefaultSubscriptionCostDisplay = "$9.99 USD / month";

		// Token: 0x040020E6 RID: 8422
		[BsonIgnore]
		private const string kDefaultAcceptText = "SUBSCRIBE";

		// Token: 0x040020E7 RID: 8423
		[BsonIgnore]
		private const string kDefaultCancelText = "Cancel";

		// Token: 0x040020E8 RID: 8424
		public string SubscriptionDescription = "Stormhaven Studios is dedicated to keeping Embers Adrift cash shop free! Support our small indie team and help us in the fight against P2W and Microtransactions by subscribing to get these great perks.";

		// Token: 0x040020E9 RID: 8425
		public string SubscriptionPerkHeader = "<indent=16pt><i>PERKS</i></indent>";

		// Token: 0x040020EA RID: 8426
		public string SubscriptionCostDisplay = "$9.99 USD / month";

		// Token: 0x040020EB RID: 8427
		public string AcceptText = "SUBSCRIBE";

		// Token: 0x040020EC RID: 8428
		public string CancelText = "Cancel";

		// Token: 0x040020ED RID: 8429
		public SubscriptionPerk[] SubscriptionPerks;
	}
}
