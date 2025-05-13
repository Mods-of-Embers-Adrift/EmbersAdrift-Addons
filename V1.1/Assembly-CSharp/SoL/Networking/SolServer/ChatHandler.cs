using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SoL.Game.Audio;
using SoL.Game.Messages;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.Objects;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003DB RID: 987
	public static class ChatHandler
	{
		// Token: 0x06001A7B RID: 6779 RVA: 0x001088A4 File Offset: 0x00106AA4
		public static void Handle(SolServerCommand cmd)
		{
			string text = null;
			string text2 = null;
			string text3 = null;
			string value = null;
			string value2 = null;
			PresenceFlags presence = PresenceFlags.Invalid;
			AccessFlags access = AccessFlags.None;
			CommandType command = cmd.Command;
			if (command <= CommandType.who)
			{
				switch (command)
				{
				case CommandType.notification:
				case CommandType.motd:
					break;
				case CommandType.renamed:
				case CommandType.setactivecharacters:
					return;
				case CommandType.say:
				case CommandType.yell:
				case CommandType.zone:
				case CommandType.group:
				case CommandType.raid:
				case CommandType.guild:
				case CommandType.officer:
				case CommandType.subscriber:
				case CommandType.help:
					goto IL_C5;
				case CommandType.tell:
					if (SessionData.SelectedCharacter != null && cmd.TryGetArgValue("Sender", out text2) && cmd.TryGetArgValue("Receiver", out text3) && cmd.TryGetArgValue("Message", out text) && cmd.TryGetArgValue("presence", out value) && Enum.TryParse<PresenceFlags>(value, out presence))
					{
						string attachments;
						if (cmd.TryGetArgValue("instances", out attachments))
						{
							MessageManager.CacheAttachments(text, attachments);
						}
						if (cmd.TryGetArgValue("access", out value2))
						{
							Enum.TryParse<AccessFlags>(value2, out access);
						}
						bool flag = string.Equals(text3, SessionData.SelectedCharacter.Name, StringComparison.CurrentCultureIgnoreCase);
						bool flag2 = string.Equals(text2, SessionData.SelectedCharacter.Name, StringComparison.CurrentCultureIgnoreCase);
						if (flag)
						{
							ChatHandler.LastTellReceivedFrom = text2;
							text3 = null;
							TellAudio.PlayTellAudioClip();
						}
						else if (flag2)
						{
							text2 = null;
						}
						MessageManager.ChatQueue.AddToQueue(MessageType.Tell, text, text2, text3, presence, access);
						return;
					}
					return;
				default:
				{
					if (command != CommandType.who)
					{
						return;
					}
					string value3;
					string value4;
					bool isTruncated;
					if (cmd.TryGetArgValue("results", out value3) && cmd.TryGetArgValue("truncated", out value4) && bool.TryParse(value4, out isTruncated))
					{
						List<WhoResult> whoResults = JsonConvert.DeserializeObject<List<WhoResult>>(value3);
						WhoResultExtensions.PrintResults(isTruncated, whoResults);
						return;
					}
					return;
				}
				}
			}
			else if (command - CommandType.system > 1)
			{
				if (command != CommandType.emote && command - CommandType.world > 1)
				{
					return;
				}
				goto IL_C5;
			}
			if (cmd.TryGetArgValue("Message", out text))
			{
				string attachmentsStr;
				if (cmd.TryGetArgValue("instances", out attachmentsStr))
				{
					MessageManager.ExtractInstanceAttachments(attachmentsStr);
				}
				MessageManager.ChatQueue.AddToQueue(cmd.Command.GetMessageType(), text);
				return;
			}
			return;
			IL_C5:
			if (cmd.TryGetArgValue("Message", out text) && cmd.TryGetArgValue("Sender", out text2) && cmd.TryGetArgValue("presence", out value) && Enum.TryParse<PresenceFlags>(value, out presence))
			{
				string attachments2;
				if (cmd.TryGetArgValue("instances", out attachments2))
				{
					MessageManager.CacheAttachments(text, attachments2);
				}
				if (cmd.TryGetArgValue("access", out value2))
				{
					Enum.TryParse<AccessFlags>(value2, out access);
				}
				ChatMessage chatMessage = MessageManager.ChatQueue.AddToQueue(cmd.Command.GetMessageType(), text, text2, null, presence, access);
				NetworkEntity networkEntity;
				if (chatMessage != null && Options.GameOptions.ShowOverheadChat.Value && cmd.Command.ShowAsOverheadText() && ClientGameManager.GroupManager.TryGetEntityForName(text2, out networkEntity) && networkEntity.GameEntity && networkEntity.GameEntity.WorldSpaceOverheadController)
				{
					networkEntity.GameEntity.WorldSpaceOverheadController.InitializeChat(chatMessage);
					return;
				}
			}
		}

		// Token: 0x0400215F RID: 8543
		public static string LastTellReceivedFrom;

		// Token: 0x04002160 RID: 8544
		public const string kMessageKey = "Message";

		// Token: 0x04002161 RID: 8545
		public const string kSenderKey = "Sender";

		// Token: 0x04002162 RID: 8546
		public const string kReceiverKey = "Receiver";

		// Token: 0x04002163 RID: 8547
		public const string kPresenceKey = "presence";

		// Token: 0x04002164 RID: 8548
		public const string kAccessKey = "access";

		// Token: 0x04002165 RID: 8549
		public const string kInstancesKey = "instances";
	}
}
