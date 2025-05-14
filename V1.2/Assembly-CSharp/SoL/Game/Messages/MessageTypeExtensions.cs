using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.SolServer;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Messages
{
	// Token: 0x020009EA RID: 2538
	public static class MessageTypeExtensions
	{
		// Token: 0x06004D2F RID: 19759 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this MessageType a, MessageType b)
		{
			return (a & b) == b;
		}

		// Token: 0x06004D30 RID: 19760 RVA: 0x000578B5 File Offset: 0x00055AB5
		public static MessageType SetBitFlag(this MessageType a, MessageType b)
		{
			return a | b;
		}

		// Token: 0x06004D31 RID: 19761 RVA: 0x000578BA File Offset: 0x00055ABA
		public static MessageType UnsetBitFlag(this MessageType a, MessageType b)
		{
			return a & ~b;
		}

		// Token: 0x06004D32 RID: 19762 RVA: 0x00074255 File Offset: 0x00072455
		public static bool IsChannel(this MessageType type)
		{
			return (type & MessageType.ChatChannel) > MessageType.None;
		}

		// Token: 0x06004D33 RID: 19763 RVA: 0x00074261 File Offset: 0x00072461
		public static bool IsChat(this MessageType type)
		{
			return (type & MessageType.Chat) > MessageType.None;
		}

		// Token: 0x06004D34 RID: 19764 RVA: 0x0007426D File Offset: 0x0007246D
		public static bool IsChatOrNotification(this MessageType type)
		{
			return (type & (MessageType.Notification | MessageType.System | MessageType.Motd | MessageType.Skills | MessageType.Say | MessageType.Yell | MessageType.Tell | MessageType.Party | MessageType.Guild | MessageType.Officer | MessageType.Zone | MessageType.World | MessageType.Trade | MessageType.Subscriber | MessageType.Help | MessageType.Raid | MessageType.Quest | MessageType.Loot | MessageType.Emote | MessageType.Discovery | MessageType.Social)) > MessageType.None;
		}

		// Token: 0x06004D35 RID: 19765 RVA: 0x00074279 File Offset: 0x00072479
		public static bool IsCombat(this MessageType type)
		{
			return (type & MessageType.Combat) > MessageType.None;
		}

		// Token: 0x06004D36 RID: 19766 RVA: 0x00074285 File Offset: 0x00072485
		public static bool CanBeDefaultChannel(this MessageType type)
		{
			if (type <= MessageType.Party)
			{
				if (type != MessageType.Say && type != MessageType.Party)
				{
					return false;
				}
			}
			else if (type != MessageType.Guild && type != MessageType.Officer && type != MessageType.Raid)
			{
				return false;
			}
			return true;
		}

		// Token: 0x06004D37 RID: 19767 RVA: 0x001BF400 File Offset: 0x001BD600
		public static CommandType GetCommandType(this MessageType type)
		{
			if (type <= MessageType.Zone)
			{
				if (type <= MessageType.Tell)
				{
					if (type == MessageType.Say)
					{
						return CommandType.say;
					}
					if (type == MessageType.Yell)
					{
						return CommandType.yell;
					}
					if (type == MessageType.Tell)
					{
						return CommandType.tell;
					}
				}
				else
				{
					if (type == MessageType.Party)
					{
						return CommandType.group;
					}
					if (type == MessageType.Guild)
					{
						return CommandType.guild;
					}
					if (type == MessageType.Zone)
					{
						return CommandType.zone;
					}
				}
			}
			else if (type <= MessageType.Trade)
			{
				if (type == MessageType.World)
				{
					return CommandType.world;
				}
				if (type == MessageType.Officer)
				{
					return CommandType.officer;
				}
				if (type == MessageType.Trade)
				{
					return CommandType.trade;
				}
			}
			else
			{
				if (type == MessageType.Subscriber)
				{
					return CommandType.subscriber;
				}
				if (type == MessageType.Help)
				{
					return CommandType.help;
				}
				if (type == MessageType.Raid)
				{
					return CommandType.raid;
				}
			}
			throw new ArgumentException("MessageType " + type.ToString() + " does not have a corresponding CommandType!");
		}

		// Token: 0x06004D38 RID: 19768 RVA: 0x001BF4C8 File Offset: 0x001BD6C8
		public static bool GetColor(this MessageType type, out Color color, bool bypassCustomColors = false)
		{
			if (!bypassCustomColors)
			{
				if (MessageTypeExtensions.m_customDefinedColors == null)
				{
					MessageTypeExtensions.LoadCustomColorsFromFile(false);
				}
				if (MessageTypeExtensions.m_customDefinedColors != null && MessageTypeExtensions.m_customDefinedColors.TryGetValue(type, out color))
				{
					return true;
				}
			}
			if (MessageTypeExtensions.m_predefinedColors == null)
			{
				MessageTypeExtensions.m_predefinedColors = new Dictionary<MessageType, Color>(default(MessageTypeComparer));
				for (int i = 0; i < GlobalSettings.Values.Chat.MessageColorSettings.Length; i++)
				{
					MessageColorSetting messageColorSetting = GlobalSettings.Values.Chat.MessageColorSettings[i];
					MessageTypeExtensions.m_predefinedColors.Add(messageColorSetting.MsgType, messageColorSetting.Color);
				}
			}
			if (MessageTypeExtensions.m_predefinedColors.TryGetValue(type, out color))
			{
				return true;
			}
			if (type <= MessageType.Zone)
			{
				if (type <= MessageType.Yell)
				{
					if (type == MessageType.Notification)
					{
						color = Color.yellow;
						return true;
					}
					if (type == MessageType.Skills)
					{
						color = Colors.BrightNavyBlue;
						return true;
					}
					if (type == MessageType.Yell)
					{
						color = Color.red;
						return true;
					}
				}
				else if (type <= MessageType.Party)
				{
					if (type == MessageType.Tell)
					{
						color = Color.magenta;
						return true;
					}
					if (type == MessageType.Party)
					{
						color = Color.cyan;
						return true;
					}
				}
				else
				{
					if (type == MessageType.Guild)
					{
						color = Colors.LimeGreen;
						return true;
					}
					if (type == MessageType.Zone)
					{
						color = Color.green;
						return true;
					}
				}
			}
			else if (type <= MessageType.Officer)
			{
				if (type <= MessageType.Loot)
				{
					if (type == MessageType.Quest)
					{
						color = Colors.CornflowerBlue;
						return true;
					}
					if (type == MessageType.Loot)
					{
						color = Colors.BlueGray;
						return true;
					}
				}
				else
				{
					if (type == MessageType.Social)
					{
						color = Colors.BlueYonder;
						return true;
					}
					if (type == MessageType.Officer)
					{
						color = Colors.Teal;
						return true;
					}
				}
			}
			else if (type <= MessageType.Subscriber)
			{
				if (type == MessageType.Trade)
				{
					color = Colors.OrangeSoda;
					return true;
				}
				if (type == MessageType.Subscriber)
				{
					color = UIManager.SubscriberColor;
					return true;
				}
			}
			else
			{
				if (type == MessageType.Help)
				{
					color = UIManager.TrialColor;
					return true;
				}
				if (type == MessageType.Raid)
				{
					color = UIManager.RaidColor;
					return true;
				}
			}
			color = Color.white;
			return false;
		}

		// Token: 0x06004D39 RID: 19769 RVA: 0x001BF738 File Offset: 0x001BD938
		public static Color GetColor(this OverheadType type)
		{
			Color result;
			if (type != OverheadType.None && MessageTypeExtensions.m_customDefinedOverheadColors != null && MessageTypeExtensions.m_customDefinedOverheadColors.TryGetValue(type, out result))
			{
				return result;
			}
			if (!type.IsNegative())
			{
				return Color.green;
			}
			return Color.red;
		}

		// Token: 0x140000F2 RID: 242
		// (add) Token: 0x06004D3A RID: 19770 RVA: 0x001BF774 File Offset: 0x001BD974
		// (remove) Token: 0x06004D3B RID: 19771 RVA: 0x001BF7A8 File Offset: 0x001BD9A8
		public static event Action CustomColorsChanged;

		// Token: 0x06004D3C RID: 19772 RVA: 0x000742B9 File Offset: 0x000724B9
		public static void ReloadCustomColors()
		{
			MessageTypeExtensions.m_customDefinedColors = null;
			MessageTypeExtensions.m_customDefinedOverheadColors = null;
			ChatMessage.ResetMessageTypePrefixes();
			MessageTypeExtensions.LoadCustomColorsFromFile(true);
			ChatMessageQueue chatQueue = MessageManager.ChatQueue;
			if (chatQueue != null)
			{
				chatQueue.CustomColorsChanged();
			}
			Action customColorsChanged = MessageTypeExtensions.CustomColorsChanged;
			if (customColorsChanged == null)
			{
				return;
			}
			customColorsChanged();
		}

		// Token: 0x06004D3D RID: 19773 RVA: 0x001BF7DC File Offset: 0x001BD9DC
		private static void LoadCustomColorsFromFile(bool notifyChat)
		{
			MessageTypeExtensions.m_customDefinedColors = new Dictionary<MessageType, Color>(default(MessageTypeComparer));
			MessageTypeExtensions.m_customDefinedOverheadColors = new Dictionary<OverheadType, Color>(default(OverheadTypeComparer));
			string text = Path.Combine(Application.streamingAssetsPath, "chatColors.json");
			try
			{
				if (File.Exists(text))
				{
					string value = string.Empty;
					using (StreamReader streamReader = new StreamReader(text))
					{
						value = streamReader.ReadToEnd();
					}
					Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
					if (dictionary != null && dictionary.Count > 0)
					{
						foreach (KeyValuePair<string, string> keyValuePair in dictionary)
						{
							MessageType messageType;
							OverheadType key;
							Color value3;
							if (Enum.TryParse<MessageType>(keyValuePair.Key, true, out messageType) && messageType.IsChat())
							{
								Color value2;
								if (ColorUtility.TryParseHtmlString(keyValuePair.Value.StartsWith("#") ? keyValuePair.Value : ("#" + keyValuePair.Value), out value2))
								{
									MessageTypeExtensions.m_customDefinedColors.Add(messageType, value2);
								}
							}
							else if (Enum.TryParse<OverheadType>(keyValuePair.Key, true, out key) && ColorUtility.TryParseHtmlString(keyValuePair.Value.StartsWith("#") ? keyValuePair.Value : ("#" + keyValuePair.Value), out value3))
							{
								MessageTypeExtensions.m_customDefinedOverheadColors.Add(key, value3);
							}
						}
					}
					int num = MessageTypeExtensions.m_customDefinedColors.Count + MessageTypeExtensions.m_customDefinedOverheadColors.Count;
					if (num > 0)
					{
						if (notifyChat)
						{
							MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Loaded " + num.ToString() + " chat colors from chatColors.json.");
						}
					}
					else if (notifyChat)
					{
						MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "No custom colors loaded from chatColors.json.");
					}
				}
				else if (notifyChat)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Unable to find " + text + "!");
				}
			}
			catch
			{
				if (notifyChat)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Exception caught loading " + text + "!");
				}
			}
		}

		// Token: 0x06004D3E RID: 19774 RVA: 0x000742F1 File Offset: 0x000724F1
		public static bool ColorMessageContent(this MessageType messageType)
		{
			return messageType - MessageType.Notification <= 1 || messageType == MessageType.Social || messageType.IsChat();
		}

		// Token: 0x06004D3F RID: 19775 RVA: 0x001BFA44 File Offset: 0x001BDC44
		public static bool AddNoParse(this MessageType messageType)
		{
			if (messageType <= MessageType.Zone)
			{
				if (messageType <= MessageType.Tell)
				{
					if (messageType != MessageType.Say && messageType != MessageType.Yell && messageType != MessageType.Tell)
					{
						return false;
					}
				}
				else if (messageType != MessageType.Party && messageType != MessageType.Guild && messageType != MessageType.Zone)
				{
					return false;
				}
			}
			else if (messageType <= MessageType.Officer)
			{
				if (messageType != MessageType.Emote && messageType != MessageType.World && messageType != MessageType.Officer)
				{
					return false;
				}
			}
			else if (messageType <= MessageType.Subscriber)
			{
				if (messageType != MessageType.Trade && messageType != MessageType.Subscriber)
				{
					return false;
				}
			}
			else if (messageType != MessageType.Help && messageType != MessageType.Raid)
			{
				return false;
			}
			return true;
		}

		// Token: 0x06004D40 RID: 19776 RVA: 0x00074309 File Offset: 0x00072509
		public static string GetDisplayChannel(this MessageType messageType)
		{
			if (messageType <= MessageType.MyCombatIn)
			{
				if (messageType != MessageType.MyCombatOut && messageType != MessageType.MyCombatIn)
				{
					goto IL_30;
				}
			}
			else if (messageType != MessageType.OtherCombat && messageType != MessageType.WarlordSong)
			{
				goto IL_30;
			}
			return "";
			IL_30:
			return messageType.ToString();
		}

		// Token: 0x06004D41 RID: 19777 RVA: 0x001BFAD8 File Offset: 0x001BDCD8
		public static string GetFilterDisplayChannel(this MessageType messageType)
		{
			if (messageType <= MessageType.MyCombatIn)
			{
				if (messageType == MessageType.MyCombatOut)
				{
					return "My Combat OUT";
				}
				if (messageType == MessageType.MyCombatIn)
				{
					return "My Combat IN";
				}
			}
			else
			{
				if (messageType == MessageType.OtherCombat)
				{
					return "Other Combat";
				}
				if (messageType == MessageType.WarlordSong)
				{
					return "Warlord Songs";
				}
			}
			return messageType.GetDisplayChannel();
		}

		// Token: 0x17001111 RID: 4369
		// (get) Token: 0x06004D42 RID: 19778 RVA: 0x00074348 File Offset: 0x00072548
		public static MessageType[] MessageTypes
		{
			get
			{
				if (MessageTypeExtensions.m_messageTypes == null)
				{
					MessageTypeExtensions.m_messageTypes = (MessageType[])Enum.GetValues(typeof(MessageType));
				}
				return MessageTypeExtensions.m_messageTypes;
			}
		}

		// Token: 0x06004D43 RID: 19779 RVA: 0x001BFB30 File Offset: 0x001BDD30
		public static MessageType GetMessageTypeForShortcutText(string shortcutText)
		{
			MessageType result;
			if (MessageTypeExtensions.m_chatChannelShortcuts.TryGetValue(shortcutText, out result))
			{
				return result;
			}
			return MessageType.None;
		}

		// Token: 0x06004D44 RID: 19780 RVA: 0x001BFB50 File Offset: 0x001BDD50
		public static void RegisterChannelShortcutsAsCommands(Func<string, ValueTuple<string, string>> linkify)
		{
			using (Dictionary<string, MessageType>.Enumerator enumerator = MessageTypeExtensions.m_chatChannelShortcuts.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, MessageType> shortcut = enumerator.Current;
					if (shortcut.Key.Equals(shortcut.Value.ToString(), StringComparison.CurrentCultureIgnoreCase))
					{
						List<string> fromPool = StaticListPool<string>.GetFromPool();
						foreach (KeyValuePair<string, MessageType> keyValuePair in MessageTypeExtensions.m_chatChannelShortcuts)
						{
							if (keyValuePair.Value == shortcut.Value && keyValuePair.Key != shortcut.Key)
							{
								fromPool.Add(keyValuePair.Key);
							}
						}
						string[] aliases = fromPool.ToArray();
						StaticListPool<string>.ReturnToPool(fromPool);
						CommandRegistry.Register(shortcut.Key, delegate(string[] args)
						{
							string arg = string.Join(" ", args);
							ValueTuple<string, string> valueTuple = linkify(arg);
							if (valueTuple.Item1 == null)
							{
								return;
							}
							SolServerCommand solServerCommand = CommandClass.chat.NewCommand(shortcut.Value.GetCommandType());
							solServerCommand.Args.Add("Message", valueTuple.Item1);
							if (valueTuple.Item2 != null)
							{
								solServerCommand.Args.Add("instances", valueTuple.Item2);
							}
							solServerCommand.Send();
						}, string.Format("Sends a message to the {0} channel", shortcut.Value), null, aliases);
					}
				}
			}
		}

		// Token: 0x06004D45 RID: 19781 RVA: 0x0007436F File Offset: 0x0007256F
		public static bool CanSpeakInChannel(this MessageType type)
		{
			return MessageTypeExtensions.m_channelRequirements.ContainsKey(type) && MessageTypeExtensions.m_channelRequirements[type]();
		}

		// Token: 0x06004D46 RID: 19782 RVA: 0x00074390 File Offset: 0x00072590
		public static bool IsNegative(this OverheadType type)
		{
			return type != OverheadType.OverheadSelfPositive && type != OverheadType.OverheadTargetPositive;
		}

		// Token: 0x04004707 RID: 18183
		private static Dictionary<MessageType, Color> m_predefinedColors = null;

		// Token: 0x04004708 RID: 18184
		private static Dictionary<MessageType, Color> m_customDefinedColors = null;

		// Token: 0x04004709 RID: 18185
		private static Dictionary<OverheadType, Color> m_customDefinedOverheadColors = null;

		// Token: 0x0400470A RID: 18186
		private const string kCustomChatColorFileName = "chatColors.json";

		// Token: 0x0400470C RID: 18188
		private static MessageType[] m_messageTypes = null;

		// Token: 0x0400470D RID: 18189
		private static readonly Dictionary<string, MessageType> m_chatChannelShortcuts = new Dictionary<string, MessageType>
		{
			{
				"say",
				MessageType.Say
			},
			{
				"sa",
				MessageType.Say
			},
			{
				"s",
				MessageType.Say
			},
			{
				"local",
				MessageType.Say
			},
			{
				"loca",
				MessageType.Say
			},
			{
				"loc",
				MessageType.Say
			},
			{
				"lo",
				MessageType.Say
			},
			{
				"party",
				MessageType.Party
			},
			{
				"part",
				MessageType.Party
			},
			{
				"par",
				MessageType.Party
			},
			{
				"pa",
				MessageType.Party
			},
			{
				"p",
				MessageType.Party
			},
			{
				"group",
				MessageType.Party
			},
			{
				"grou",
				MessageType.Party
			},
			{
				"gro",
				MessageType.Party
			},
			{
				"gr",
				MessageType.Party
			},
			{
				"raid",
				MessageType.Raid
			},
			{
				"rai",
				MessageType.Raid
			},
			{
				"ra",
				MessageType.Raid
			},
			{
				"guild",
				MessageType.Guild
			},
			{
				"guil",
				MessageType.Guild
			},
			{
				"gui",
				MessageType.Guild
			},
			{
				"gu",
				MessageType.Guild
			},
			{
				"g",
				MessageType.Guild
			},
			{
				"officer",
				MessageType.Officer
			},
			{
				"office",
				MessageType.Officer
			},
			{
				"offic",
				MessageType.Officer
			},
			{
				"offi",
				MessageType.Officer
			},
			{
				"off",
				MessageType.Officer
			},
			{
				"of",
				MessageType.Officer
			},
			{
				"yell",
				MessageType.Yell
			},
			{
				"yel",
				MessageType.Yell
			},
			{
				"ye",
				MessageType.Yell
			},
			{
				"y",
				MessageType.Yell
			},
			{
				"zone",
				MessageType.Zone
			},
			{
				"zon",
				MessageType.Zone
			},
			{
				"zo",
				MessageType.Zone
			},
			{
				"z",
				MessageType.Zone
			},
			{
				"ooc",
				MessageType.Zone
			},
			{
				"oo",
				MessageType.Zone
			},
			{
				"o",
				MessageType.Zone
			},
			{
				"world",
				MessageType.World
			},
			{
				"worl",
				MessageType.World
			},
			{
				"wor",
				MessageType.World
			},
			{
				"wo",
				MessageType.World
			},
			{
				"w",
				MessageType.World
			},
			{
				"global",
				MessageType.World
			},
			{
				"globa",
				MessageType.World
			},
			{
				"glob",
				MessageType.World
			},
			{
				"glo",
				MessageType.World
			},
			{
				"gl",
				MessageType.World
			},
			{
				"trade",
				MessageType.Trade
			},
			{
				"trad",
				MessageType.Trade
			},
			{
				"tra",
				MessageType.Trade
			},
			{
				"tr",
				MessageType.Trade
			},
			{
				"subscriber",
				MessageType.Subscriber
			},
			{
				"subscribe",
				MessageType.Subscriber
			},
			{
				"subscrib",
				MessageType.Subscriber
			},
			{
				"subscri",
				MessageType.Subscriber
			},
			{
				"subscr",
				MessageType.Subscriber
			},
			{
				"subsc",
				MessageType.Subscriber
			},
			{
				"subs",
				MessageType.Subscriber
			},
			{
				"sub",
				MessageType.Subscriber
			},
			{
				"su",
				MessageType.Subscriber
			},
			{
				"help",
				MessageType.Help
			},
			{
				"hel",
				MessageType.Help
			},
			{
				"he",
				MessageType.Help
			},
			{
				"h",
				MessageType.Help
			}
		};

		// Token: 0x0400470E RID: 18190
		private static Dictionary<MessageType, Func<bool>> m_channelRequirements = new Dictionary<MessageType, Func<bool>>
		{
			{
				MessageType.Say,
				() => true
			},
			{
				MessageType.Yell,
				() => true
			},
			{
				MessageType.Tell,
				() => true
			},
			{
				MessageType.Party,
				delegate()
				{
					ClientGroupManager groupManager = ClientGameManager.GroupManager;
					return groupManager != null && groupManager.IsGrouped;
				}
			},
			{
				MessageType.Raid,
				delegate()
				{
					SocialManager socialManager = ClientGameManager.SocialManager;
					return socialManager != null && socialManager.IsInRaid;
				}
			},
			{
				MessageType.Guild,
				delegate()
				{
					SocialManager socialManager = ClientGameManager.SocialManager;
					return socialManager != null && socialManager.CanSendGuildChat;
				}
			},
			{
				MessageType.Officer,
				delegate()
				{
					SocialManager socialManager = ClientGameManager.SocialManager;
					return socialManager != null && socialManager.CanSendOfficerChat;
				}
			},
			{
				MessageType.Zone,
				() => true
			},
			{
				MessageType.Emote,
				() => true
			},
			{
				MessageType.World,
				() => !SessionData.IsTrial
			},
			{
				MessageType.Trade,
				() => !SessionData.IsTrial
			},
			{
				MessageType.Subscriber,
				() => LocalPlayer.GameEntity && (LocalPlayer.GameEntity.GM || LocalPlayer.GameEntity.Subscriber)
			},
			{
				MessageType.Help,
				() => true
			}
		};
	}
}
