using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SoL.Game.Login.Client;
using SoL.Game.Messages;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003DC RID: 988
	public static class ClientHandler
	{
		// Token: 0x06001A7C RID: 6780 RVA: 0x00108BC4 File Offset: 0x00106DC4
		public static void Handle(SolServerCommand cmd)
		{
			if (!cmd.State)
			{
				bool flag = false;
				object obj;
				if (cmd.Args.TryGetValue("critical", out obj))
				{
					flag = (bool)obj;
				}
				string text = cmd.Args[SolServerCommand.kErrorKey].ToString();
				object obj2;
				if (cmd.Command == CommandType.auth && flag && cmd.Args.TryGetValue("timestamp", out obj2))
				{
					long num = (long)obj2;
					long lastServerApiUpdate = GlobalSettings.Values.Configs.Data.LastServerApiUpdate;
					if (num < lastServerApiUpdate)
					{
						text = "Server update pending. Try again later.";
					}
				}
				object arg;
				if (cmd.Command == CommandType.sessionkeyauth && LoginController.Instance != null)
				{
					LoginController.Instance.LoginFailed();
				}
				else if (cmd.Command == CommandType.steampurchase && cmd.Args.TryGetValue("state", out arg))
				{
					Debug.LogWarning(string.Format("Steam Failure? {0}", arg));
				}
				ClientHandler.RaiseError(text, flag);
				Debug.LogWarning(string.Format("State=false, {0} {1}", text, cmd.Command));
				return;
			}
			Debug.Log(string.Format("Client Handler Command: {0}", cmd.Command));
			CommandType command = cmd.Command;
			if (command <= CommandType.zonecheck)
			{
				switch (command)
				{
				case CommandType.auth:
				case CommandType.sessionkeyauth:
				{
					UserRecord userRecord = cmd.DeserializeKey("user");
					IEnumerable<WorldRecord> source = cmd.DeserializeKey("worlds");
					string sessionKey = cmd.Args["session_key"].ToString();
					WorldRecord worldRecord = source.FirstOrDefault((WorldRecord a) => a.WorldId == 1);
					if (worldRecord == null)
					{
						SolServerConnectionManager.ClearSession();
						ClientHandler.RaiseError("Cannot find world", false);
						return;
					}
					if (!userRecord.Flags.HasBitFlag(AccessFlags.Active))
					{
						SolServerConnectionManager.ClearSession();
						ClientHandler.RaiseError("Account is not active", false);
						return;
					}
					if (!worldRecord.IsOnline)
					{
						SolServerConnectionManager.ClearSession();
						ClientHandler.RaiseError("World is offline", false);
						return;
					}
					if (!AccessFlagsExtensions.HasAccess(worldRecord.Flags, userRecord.Flags))
					{
						SolServerConnectionManager.ClearSession();
						ClientHandler.RaiseError("World is Locked", false);
						return;
					}
					ZoneRecord[] zones = cmd.DeserializeKey("zonelist");
					EnetConfig config = cmd.DeserializeKey("enetconfig");
					SteamConfig config2 = cmd.DeserializeKey("steamconfig");
					CharacterRecord[] array = cmd.DeserializeKey("characters");
					if (cmd.Args.ContainsKey("torename"))
					{
						foreach (string b in cmd.DeserializeKey("torename"))
						{
							foreach (CharacterRecord characterRecord in array)
							{
								if (characterRecord.Id == b)
								{
									characterRecord.RequiresRenaming = new bool?(true);
									break;
								}
							}
						}
					}
					NetworkManager.Config = config;
					SteamManager.Config = config2;
					object obj3;
					SessionData.SubscriptionStatus = (cmd.Args.TryGetValue("substatus", out obj3) ? ((string)obj3) : null);
					object obj4;
					long ticks;
					SessionData.SubscriptionExpires = ((cmd.Args.TryGetValue("subexpires", out obj4) && long.TryParse((string)obj4, out ticks)) ? new DateTime?(new DateTime(ticks)) : null);
					SessionData.SetZones(zones);
					SessionData.SessionKey = sessionKey;
					SessionData.World = worldRecord;
					SessionData.User = userRecord;
					SessionData.Characters = array;
					TutorialPopupOptions loginNotificationOverride;
					if (cmd.Command != CommandType.sessionkeyauth && cmd.TryDeserializeKey("loginMessage", out loginNotificationOverride))
					{
						GlobalSettings.Values.General.SetLoginNotificationOverride(loginNotificationOverride);
					}
					if (LoginController.Instance != null)
					{
						LoginController.Instance.SetStatusText("Login Successful.  Retrieving data...");
						LoginController.Instance.LoginComplete();
						return;
					}
					break;
				}
				case CommandType.reauth:
				case CommandType.clearsession:
				case CommandType.getallofit:
				case CommandType.disconnect:
				case CommandType.init_login:
				case CommandType.versioncheck:
				case CommandType.notification:
				case CommandType.renamed:
					break;
				case CommandType.playcharacter:
				{
					LoginController.Instance.SetStatusText("Entering world...");
					int port = cmd.DeserializeKey("port");
					LoginStage loginStage;
					if (LoginController.Instance.TryGetLoginStage(LoginStageType.CharacterSelection, out loginStage))
					{
						((LoginStageCharacterSelection)loginStage).PlayCharacterSuccess(port);
						return;
					}
					break;
				}
				case CommandType.initworld:
					CommandClass.client.NewCommand(CommandType.getallofit).Send();
					return;
				case CommandType.createcharacter:
				case CommandType.updatecharactervisuals:
				{
					string value;
					if (cmd.TryGetArgValue("user", out value))
					{
						SessionData.User = JsonConvert.DeserializeObject<UserRecord>(value);
					}
					SessionData.Characters = cmd.DeserializeKey("characters");
					LoginController.Instance.SetStage(LoginStageType.CharacterSelection);
					return;
				}
				case CommandType.deletecharacter:
				{
					string value2;
					if (cmd.TryGetArgValue("user", out value2))
					{
						SessionData.User = JsonConvert.DeserializeObject<UserRecord>(value2);
					}
					SessionData.Characters = cmd.DeserializeKey("characters");
					SessionData.SelectLastCharacter = true;
					LoginController.Instance.RefreshStage();
					return;
				}
				case CommandType.setactivecharacters:
					SessionData.User = cmd.DeserializeKey("user");
					SessionData.SortCharacters();
					LoginController.Instance.RefreshStage();
					return;
				default:
					return;
				}
			}
			else if (command != CommandType.namecheck)
			{
				if (command != CommandType.steampurchase)
				{
					return;
				}
				if (cmd.Args.ContainsKey("user"))
				{
					SessionData.User = cmd.DeserializeKey("user");
					Debug.Log("Subscription finalized.");
					return;
				}
				if (cmd.Args.ContainsKey("status"))
				{
					Debug.Log(string.Format("Transaction submitted successfully. {0}", cmd.Args["status"]));
				}
			}
			else if (ClientGameManager.SocialManager)
			{
				CharacterIdentification ident = null;
				string text2 = null;
				if (cmd.Args.ContainsKey("data") && cmd.Args["data"] != null)
				{
					cmd.TryDeserializeKey("data", out ident);
				}
				if (cmd.Args.ContainsKey("Message") && cmd.Args["Message"] != null)
				{
					text2 = (string)cmd.Args["Message"];
				}
				if (text2 != null)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, text2);
				}
				ClientGameManager.SocialManager.NameCheckResponse(ident, 1);
				return;
			}
		}

		// Token: 0x06001A7D RID: 6781 RVA: 0x00109194 File Offset: 0x00107394
		private static void RaiseError(string err, bool critical = false)
		{
			if (LoginController.Instance != null)
			{
				Debug.LogWarning(err);
				if (critical)
				{
					LoginController.Instance.RaiseErrorCritical(err.Color(Color.red));
					return;
				}
				LoginController.Instance.RaiseError(err.Color(Color.red));
			}
		}
	}
}
