using System;
using SoL.Game.Messages;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003EC RID: 1004
	public static class CommandTypeExtensions
	{
		// Token: 0x06001AB7 RID: 6839 RVA: 0x00109B78 File Offset: 0x00107D78
		public static MessageType GetMessageType(this CommandType cmdType)
		{
			if (cmdType <= CommandType.system)
			{
				switch (cmdType)
				{
				case CommandType.notification:
					return MessageType.Notification;
				case CommandType.renamed:
				case CommandType.setactivecharacters:
					break;
				case CommandType.say:
					return MessageType.Say;
				case CommandType.tell:
					return MessageType.Tell;
				case CommandType.yell:
					return MessageType.Yell;
				case CommandType.zone:
					return MessageType.Zone;
				case CommandType.group:
					return MessageType.Party;
				case CommandType.raid:
					return MessageType.Raid;
				case CommandType.motd:
					return MessageType.Motd;
				case CommandType.guild:
					return MessageType.Guild;
				case CommandType.officer:
					return MessageType.Officer;
				case CommandType.subscriber:
					return MessageType.Subscriber;
				case CommandType.help:
					return MessageType.Help;
				default:
					if (cmdType == CommandType.system)
					{
						return MessageType.System;
					}
					break;
				}
			}
			else
			{
				if (cmdType == CommandType.systemzone)
				{
					return MessageType.System;
				}
				switch (cmdType)
				{
				case CommandType.emote:
					return MessageType.Emote;
				case CommandType.world:
					return MessageType.World;
				case CommandType.trade:
					return MessageType.Trade;
				}
			}
			throw new ArgumentException(string.Format("Invalid CommandType!  Has no MessageType!  {0}", cmdType));
		}

		// Token: 0x06001AB8 RID: 6840 RVA: 0x00054B23 File Offset: 0x00052D23
		public static bool ShowAsOverheadText(this CommandType cmdType)
		{
			if (cmdType <= CommandType.help)
			{
				if (cmdType != CommandType.zone && cmdType - CommandType.guild > 3)
				{
					return true;
				}
			}
			else if (cmdType != CommandType.emote && cmdType - CommandType.world > 1)
			{
				return true;
			}
			return false;
		}
	}
}
