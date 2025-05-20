using System;
using Cysharp.Text;
using SoL.Game;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Subscription
{
	// Token: 0x020003B1 RID: 945
	public static class SubscriptionExtensions
	{
		// Token: 0x060019AA RID: 6570 RVA: 0x000541C0 File Offset: 0x000523C0
		private static bool AutoCancel()
		{
			return !SteamManager.SteamIsAvailable;
		}

		// Token: 0x060019AB RID: 6571 RVA: 0x000541CA File Offset: 0x000523CA
		public static void ExecuteActivateSubscription()
		{
			SubscriptionExtensions.ExecuteActivateSubscription(ElementMode.ActivateSub);
		}

		// Token: 0x060019AC RID: 6572 RVA: 0x00106FC4 File Offset: 0x001051C4
		internal static void ExecuteActivateSubscription(ElementMode mode)
		{
			if (ClientGameManager.UIManager)
			{
				if (ClientGameManager.UIManager.InGameUiOptions)
				{
					ClientGameManager.UIManager.InGameUiOptions.EscapePressedFirstPass();
				}
				if (ClientGameManager.UIManager.InGameUiMenu)
				{
					ClientGameManager.UIManager.InGameUiMenu.EscapePressedFirstPass();
				}
			}
			switch (mode)
			{
			case ElementMode.Donate:
				Application.OpenURL("https://www.embersadrift.com/Donate");
				return;
			case ElementMode.Purchase:
				Application.OpenURL("https://store.steampowered.com/app/3336530/Embers_Adrift/");
				return;
			case ElementMode.ActivateSub:
				if (ClientGameManager.UIManager)
				{
					if (SteamManager.SteamIsAvailable)
					{
						SteamSubscriptionDialogOptions steamSubscriptionDialogOptions = default(SteamSubscriptionDialogOptions);
						steamSubscriptionDialogOptions.Title = "Activate Subscription";
						steamSubscriptionDialogOptions.ConfirmationText = "YES";
						steamSubscriptionDialogOptions.CancelText = "Cancel";
						steamSubscriptionDialogOptions.BlockInteractions = true;
						steamSubscriptionDialogOptions.BackgroundBlockerColor = Color.black.GetColorWithAlpha(0.8f);
						steamSubscriptionDialogOptions.ShowCloseButton = false;
						steamSubscriptionDialogOptions.Callback = delegate(bool answer, object obj)
						{
							if (answer && ClientGameManager.SteamManager)
							{
								ClientGameManager.SteamManager.PurchaseSubscriptionRequest();
							}
						};
						steamSubscriptionDialogOptions.AutoCancel = new Func<bool>(SubscriptionExtensions.AutoCancel);
						SteamSubscriptionDialogOptions opts = steamSubscriptionDialogOptions;
						ClientGameManager.UIManager.SteamSubscriptionDialog.Init(opts);
						return;
					}
					DialogOptions opts2 = new DialogOptions
					{
						Title = "Activate Subscription",
						Text = "Please start the game through Steam to activate a subscription.",
						ConfirmationText = "OK",
						HideCancelButton = true,
						BlockInteractions = true,
						BackgroundBlockerColor = Color.black.GetColorWithAlpha(0.8f),
						ShowCloseButton = false
					};
					ClientGameManager.UIManager.InformationDialog.Init(opts2);
				}
				return;
			default:
				return;
			}
		}

		// Token: 0x060019AD RID: 6573 RVA: 0x00107170 File Offset: 0x00105370
		internal static string GetLabel(TimeSpan? timeDelta, bool includeIcon = false)
		{
			string result = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				if (includeIcon)
				{
					utf16ValueStringBuilder.AppendFormat<string>("{0} ", SubscriptionExtensions.GetFormattedIcon());
				}
				if (SessionData.IsSubscriber)
				{
					string subscriptionStatus = SessionData.SubscriptionStatus;
					if (!(subscriptionStatus == "newpurchase"))
					{
						if (!(subscriptionStatus == "cancelled"))
						{
							utf16ValueStringBuilder.AppendLine("Subscription: Active");
						}
						else
						{
							utf16ValueStringBuilder.AppendLine("Subscription: Cancelled");
						}
					}
					else
					{
						utf16ValueStringBuilder.AppendLine("Optional Sub: New Purchase");
					}
					if (timeDelta != null)
					{
						if (timeDelta.Value.TotalSeconds <= 0.0)
						{
							utf16ValueStringBuilder.Append("Expired!");
						}
						else if (timeDelta.Value.Days > 0)
						{
							utf16ValueStringBuilder.AppendFormat<int, int>("Time Remaining: {0}d {1}hrs", timeDelta.Value.Days, timeDelta.Value.Hours);
						}
						else
						{
							utf16ValueStringBuilder.AppendFormat<int>("Time Remaining: {0}hrs", timeDelta.Value.Hours);
						}
					}
				}
				else
				{
					utf16ValueStringBuilder.AppendLine("Subscription: Inactive");
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x060019AE RID: 6574 RVA: 0x001072C8 File Offset: 0x001054C8
		internal static string GetIconUnicode()
		{
			string result = "";
			if (SessionData.IsSubscriber)
			{
				if (SessionData.SubscriptionExpires != null)
				{
					DateTime serverCorrectedDateTimeUtc = GameTimeReplicator.GetServerCorrectedDateTimeUtc();
					TimeSpan timeSpan = (serverCorrectedDateTimeUtc > SessionData.SubscriptionExpires.Value) ? new TimeSpan(0L) : (SessionData.SubscriptionExpires.Value - serverCorrectedDateTimeUtc);
					if (timeSpan.TotalDays <= 1.0)
					{
						result = "";
					}
					else if (timeSpan.TotalDays <= 7.0)
					{
						result = "";
					}
					else
					{
						result = "";
					}
				}
				else
				{
					result = "";
				}
			}
			return result;
		}

		// Token: 0x060019AF RID: 6575 RVA: 0x000541D2 File Offset: 0x000523D2
		internal static string GetFormattedIcon()
		{
			return ZString.Format<string, string, string>("{0}{1}{2}", "<font=\"Font Awesome 5 Free-Solid-900 SDF\">", SubscriptionExtensions.GetIconUnicode(), "</font>");
		}

		// Token: 0x060019B0 RID: 6576 RVA: 0x00107370 File Offset: 0x00105570
		internal static ITooltipParameter GetTooltipParameter(MonoBehaviour behavior, bool isOptionsMenu = false)
		{
			TimeSpan? timeDelta = null;
			if (!string.IsNullOrEmpty(SessionData.SubscriptionStatus) && SessionData.SubscriptionExpires != null)
			{
				DateTime serverCorrectedDateTimeUtc = GameTimeReplicator.GetServerCorrectedDateTimeUtc();
				timeDelta = new TimeSpan?((serverCorrectedDateTimeUtc > SessionData.SubscriptionExpires.Value) ? new TimeSpan(0L) : (SessionData.SubscriptionExpires.Value - serverCorrectedDateTimeUtc));
			}
			string text = SubscriptionExtensions.GetLabel(timeDelta, true);
			if (SessionData.IsSubscriber)
			{
				string subscriptionStatus = SessionData.SubscriptionStatus;
				if (!(subscriptionStatus == "newpurchase"))
				{
					if (!(subscriptionStatus == "cancelled"))
					{
						text = ZString.Format<string, string>("{0}\n<size=80%>{1}</size>", text, "<i>Visit</i> https://store.steampowered.com/account/<i>, where you can edit your payment method on record or cancel a Subscription at any time. Your cancelled Subscription will remain active until the paid plan expires.</i>");
					}
					else
					{
						text = ZString.Format<string, string, string>("{0}\n<size=80%>({1})\n\n{2}</size>", text, "before your subscription expires", "<i>Due to Steam limitations you must wait for this subscription to expire before you can purchase another.</i>");
					}
				}
				else
				{
					text = ZString.Format<string, string, string>("{0}\n<size=80%>({1})\n\n{2}</size>", text, "included in your initial purchase", "<i>Due to Steam limitations you must wait for this subscription to expire before you can purchase another.</i>");
				}
			}
			return new ObjectTextTooltipParameter(behavior, text, isOptionsMenu);
		}

		// Token: 0x0400209E RID: 8350
		public const string kPurchaseGameButtonText = "Purchase Game";

		// Token: 0x0400209F RID: 8351
		public const string kPurchaseRequiredText = "(Purchase Required)";

		// Token: 0x040020A0 RID: 8352
		internal const string kActivateSubscriptionButtonText = "Activate Subscription";

		// Token: 0x040020A1 RID: 8353
		private const string kSubscriptionActive = "Subscription: Active";

		// Token: 0x040020A2 RID: 8354
		private const string kSubscriptionInactive = "Subscription: Inactive";

		// Token: 0x040020A3 RID: 8355
		private const float kBackgroundAlpha = 0.8f;

		// Token: 0x040020A4 RID: 8356
		private const string kStatusNewPurchase = "newpurchase";

		// Token: 0x040020A5 RID: 8357
		private const string kNewPurchaseTooltip = "included in your initial purchase";

		// Token: 0x040020A6 RID: 8358
		private const string kStatusCancelled = "cancelled";

		// Token: 0x040020A7 RID: 8359
		private const string kCancelledTooltip = "before your subscription expires";

		// Token: 0x040020A8 RID: 8360
		private const string kWaitToBuyText = "<i>Due to Steam limitations you must wait for this subscription to expire before you can purchase another.</i>";

		// Token: 0x040020A9 RID: 8361
		private const string kManageSubscriptionText = "<i>Visit</i> https://store.steampowered.com/account/<i>, where you can edit your payment method on record or cancel a Subscription at any time. Your cancelled Subscription will remain active until the paid plan expires.</i>";
	}
}
