using System;
using SoL.Game.Messages;

namespace SoL.Game.UI.Chat
{
	// Token: 0x020009A3 RID: 2467
	public static class ChatFilterExtensions
	{
		// Token: 0x060049D5 RID: 18901 RVA: 0x001B0CE8 File Offset: 0x001AEEE8
		public static MessageType GetMessageType(this ChatFilter filter)
		{
			if (filter <= ChatFilter.Notification)
			{
				if (filter <= ChatFilter.Officer)
				{
					if (filter <= ChatFilter.Party)
					{
						switch (filter)
						{
						case ChatFilter.Say:
							return MessageType.Say;
						case ChatFilter.Yell:
							return MessageType.Yell;
						case ChatFilter.Say | ChatFilter.Yell:
							break;
						case ChatFilter.Tell:
							return MessageType.Tell;
						default:
							if (filter == ChatFilter.Party)
							{
								return MessageType.Party;
							}
							break;
						}
					}
					else
					{
						if (filter == ChatFilter.Guild)
						{
							return MessageType.Guild;
						}
						if (filter == ChatFilter.Officer)
						{
							return MessageType.Officer;
						}
					}
				}
				else if (filter <= ChatFilter.World)
				{
					if (filter == ChatFilter.Zone)
					{
						return MessageType.Zone;
					}
					if (filter == ChatFilter.World)
					{
						return MessageType.World;
					}
				}
				else
				{
					if (filter == ChatFilter.Trade)
					{
						return MessageType.Trade;
					}
					if (filter == ChatFilter.Emote)
					{
						return MessageType.Emote;
					}
					if (filter == ChatFilter.Notification)
					{
						return MessageType.Notification;
					}
				}
			}
			else if (filter <= ChatFilter.Quest)
			{
				if (filter <= ChatFilter.Motd)
				{
					if (filter == ChatFilter.Loot)
					{
						return MessageType.Loot;
					}
					if (filter == ChatFilter.Motd)
					{
						return MessageType.Motd;
					}
				}
				else
				{
					if (filter == ChatFilter.Skills)
					{
						return MessageType.Skills;
					}
					if (filter == ChatFilter.Quest)
					{
						return MessageType.Quest;
					}
				}
			}
			else if (filter <= ChatFilter.Social)
			{
				if (filter == ChatFilter.Discovery)
				{
					return MessageType.Discovery;
				}
				if (filter == ChatFilter.Social)
				{
					return MessageType.Social;
				}
			}
			else
			{
				if (filter == ChatFilter.Subscriber)
				{
					return MessageType.Subscriber;
				}
				if (filter == ChatFilter.Help)
				{
					return MessageType.Help;
				}
				if (filter == ChatFilter.Raid)
				{
					return MessageType.Raid;
				}
			}
			return MessageType.None;
		}

		// Token: 0x060049D6 RID: 18902 RVA: 0x001B0E64 File Offset: 0x001AF064
		public static MessageType GetMessageTypeFlags(this ChatFilter filter)
		{
			MessageType messageType = MessageType.System;
			if (filter.HasBitFlag(ChatFilter.Say))
			{
				messageType |= MessageType.Say;
			}
			if (filter.HasBitFlag(ChatFilter.Yell))
			{
				messageType |= MessageType.Yell;
			}
			if (filter.HasBitFlag(ChatFilter.Tell))
			{
				messageType |= MessageType.Tell;
			}
			if (filter.HasBitFlag(ChatFilter.Party))
			{
				messageType |= MessageType.Party;
			}
			if (filter.HasBitFlag(ChatFilter.Raid))
			{
				messageType |= MessageType.Raid;
			}
			if (filter.HasBitFlag(ChatFilter.Guild))
			{
				messageType |= MessageType.Guild;
			}
			if (filter.HasBitFlag(ChatFilter.Officer))
			{
				messageType |= MessageType.Officer;
			}
			if (filter.HasBitFlag(ChatFilter.Zone))
			{
				messageType |= MessageType.Zone;
			}
			if (filter.HasBitFlag(ChatFilter.World))
			{
				messageType |= MessageType.World;
			}
			if (filter.HasBitFlag(ChatFilter.Trade))
			{
				messageType |= MessageType.Trade;
			}
			if (filter.HasBitFlag(ChatFilter.Emote))
			{
				messageType |= MessageType.Emote;
			}
			if (filter.HasBitFlag(ChatFilter.Notification))
			{
				messageType |= MessageType.Notification;
			}
			if (filter.HasBitFlag(ChatFilter.Loot))
			{
				messageType |= MessageType.Loot;
			}
			if (filter.HasBitFlag(ChatFilter.Motd))
			{
				messageType |= MessageType.Motd;
			}
			if (filter.HasBitFlag(ChatFilter.Skills))
			{
				messageType |= MessageType.Skills;
			}
			if (filter.HasBitFlag(ChatFilter.Quest))
			{
				messageType |= MessageType.Quest;
			}
			if (filter.HasBitFlag(ChatFilter.Discovery))
			{
				messageType |= MessageType.Discovery;
			}
			if (filter.HasBitFlag(ChatFilter.Social))
			{
				messageType |= MessageType.Social;
			}
			if (filter.HasBitFlag(ChatFilter.Subscriber))
			{
				messageType |= MessageType.Subscriber;
			}
			if (filter.HasBitFlag(ChatFilter.Help))
			{
				messageType |= MessageType.Help;
			}
			return messageType;
		}

		// Token: 0x17001057 RID: 4183
		// (get) Token: 0x060049D7 RID: 18903 RVA: 0x000719C7 File Offset: 0x0006FBC7
		public static ChatFilter[] ChatFilters
		{
			get
			{
				if (ChatFilterExtensions.m_chatFilters == null)
				{
					ChatFilterExtensions.m_chatFilters = (ChatFilter[])Enum.GetValues(typeof(ChatFilter));
				}
				return ChatFilterExtensions.m_chatFilters;
			}
		}

		// Token: 0x060049D8 RID: 18904 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this ChatFilter a, ChatFilter b)
		{
			return (a & b) == b;
		}

		// Token: 0x040044E3 RID: 17635
		private static ChatFilter[] m_chatFilters;
	}
}
