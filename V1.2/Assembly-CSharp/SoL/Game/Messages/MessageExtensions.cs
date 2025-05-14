using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Networking.Database;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Messages
{
	// Token: 0x020009E1 RID: 2529
	public static class MessageExtensions
	{
		// Token: 0x1700110B RID: 4363
		// (get) Token: 0x06004D02 RID: 19714 RVA: 0x001BE630 File Offset: 0x001BC830
		private static Dictionary<MessageType, string> MessageTypeFormatPrefixes
		{
			get
			{
				if (MessageExtensions.m_messageTypeFormatPrefixes == null)
				{
					MessageExtensions.m_messageTypeFormatPrefixes = new Dictionary<MessageType, string>(default(MessageTypeComparer));
					foreach (object obj in Enum.GetValues(typeof(MessageType)))
					{
						MessageType messageType = (MessageType)obj;
						string text = messageType.GetDisplayChannel().ToUpper();
						if (!string.IsNullOrEmpty(text))
						{
							Color color;
							if (messageType.GetColor(out color, false))
							{
								text = text.Color(color);
							}
							MessageExtensions.m_messageTypeFormatPrefixes.Add(messageType, text);
						}
					}
				}
				return MessageExtensions.m_messageTypeFormatPrefixes;
			}
		}

		// Token: 0x06004D03 RID: 19715 RVA: 0x001BE6EC File Offset: 0x001BC8EC
		internal static string GetFormattedMessage(this Message msg, bool showTimestamp)
		{
			string text = msg.Contents;
			Color color;
			if (msg.Type.ColorMessageContent() && msg.Type.GetColor(out color, false))
			{
				text = text.Color(color);
			}
			if (msg.Type.IsChat())
			{
				string presenceChatPrefix = MessageExtensions.GetPresenceChatPrefix(msg.Presence);
				if (!string.IsNullOrEmpty(msg.Sender))
				{
					text = ((msg.Type == MessageType.Tell) ? ZString.Format<string, string, string>("<b>FROM</b> <i>{0}</i>{1}: {2}", msg.SenderLink, presenceChatPrefix, text) : ZString.Format<string, string>("<i>{0}</i>: {1}", msg.SenderLink, text));
				}
				else if (!string.IsNullOrEmpty(msg.Receiver))
				{
					text = ZString.Format<string, string, string>("<b>TO</b> <i>{0}</i>{1}: {2}", msg.ReceiverLink, presenceChatPrefix, text);
				}
			}
			string text2;
			if (MessageExtensions.MessageTypeFormatPrefixes.TryGetValue(msg.Type, out text2))
			{
				text = (showTimestamp ? ZString.Format<string, string, string>("[{0} {1}] {2}", msg.FormattedTimestamp, text2, text) : ZString.Format<string, string>("[{0}] {1}", text2, text));
			}
			else if (showTimestamp)
			{
				text = ZString.Format<string, string>("[{0}] {1}", msg.FormattedTimestamp, text);
			}
			return text;
		}

		// Token: 0x06004D04 RID: 19716 RVA: 0x001BE7FC File Offset: 0x001BC9FC
		internal static string GetFormattedMessageBuildBackwards(this Message msg, bool showTimestamp)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				string text;
				if (MessageExtensions.MessageTypeFormatPrefixes.TryGetValue(msg.Type, out text))
				{
					if (showTimestamp)
					{
						utf16ValueStringBuilder.AppendFormat<string, string>("[{0} {1}] ", msg.FormattedTimestamp, text);
					}
					else
					{
						utf16ValueStringBuilder.AppendFormat<string>("[{0}] ", text);
					}
				}
				else if (showTimestamp)
				{
					utf16ValueStringBuilder.AppendFormat<string>("[{0}] ", msg.FormattedTimestamp);
				}
				if (msg.Type.IsChat())
				{
					string presenceChatPrefix = MessageExtensions.GetPresenceChatPrefix(msg.Presence);
					if (!string.IsNullOrEmpty(msg.Sender))
					{
						if (msg.Type == MessageType.Tell)
						{
							utf16ValueStringBuilder.AppendFormat<string, string>("<b>FROM</b> <i>{0}</i>{1}: ", msg.SenderLink, presenceChatPrefix);
						}
						else
						{
							utf16ValueStringBuilder.AppendFormat<string>("<i>{0}</i>: ", msg.SenderLink);
						}
					}
					else if (!string.IsNullOrEmpty(msg.Receiver))
					{
						utf16ValueStringBuilder.AppendFormat<string, string>("<b>TO</b> <i>{0}</i>{1}: ", msg.ReceiverLink, presenceChatPrefix);
					}
				}
				Color color;
				if (msg.Type.ColorMessageContent() && msg.Type.GetColor(out color, false))
				{
					utf16ValueStringBuilder.AppendFormat<string, string>("<color={0}>{1}</color>", color.ToHex(), msg.Contents);
				}
				else
				{
					utf16ValueStringBuilder.AppendFormat<string>("{0}", msg.Contents);
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06004D05 RID: 19717 RVA: 0x001BE970 File Offset: 0x001BCB70
		private static string GetPresenceChatPrefix(PresenceFlags value)
		{
			if (value <= PresenceFlags.Online || value == PresenceFlags.Anonymous || value == PresenceFlags.Invisible)
			{
				return string.Empty;
			}
			string result;
			if (MessageExtensions.m_presenceChatPrefixes == null)
			{
				MessageExtensions.m_presenceChatPrefixes = new Dictionary<PresenceFlags, string>();
			}
			else if (MessageExtensions.m_presenceChatPrefixes.TryGetValue(value, out result))
			{
				return result;
			}
			string text = value.ToStringAbbreviation();
			if (string.IsNullOrEmpty(text))
			{
				MessageExtensions.m_presenceChatPrefixes.Add(value, string.Empty);
				return string.Empty;
			}
			string text2 = ZString.Format<string>(" ({0})", text);
			MessageExtensions.m_presenceChatPrefixes.Add(value, text2);
			return text2;
		}

		// Token: 0x040046C3 RID: 18115
		private static Dictionary<MessageType, string> m_messageTypeFormatPrefixes;

		// Token: 0x040046C4 RID: 18116
		private static Dictionary<PresenceFlags, string> m_presenceChatPrefixes;
	}
}
