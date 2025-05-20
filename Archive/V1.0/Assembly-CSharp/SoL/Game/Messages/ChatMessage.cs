using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cysharp.Text;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Messages
{
	// Token: 0x020009DC RID: 2524
	public class ChatMessage : IPoolable, IEquatable<ChatMessage>, IEqualityComparer<ChatMessage>
	{
		// Token: 0x170010F4 RID: 4340
		// (get) Token: 0x06004CC4 RID: 19652 RVA: 0x00073EFB File Offset: 0x000720FB
		// (set) Token: 0x06004CC5 RID: 19653 RVA: 0x00073F03 File Offset: 0x00072103
		public MessageType Type { get; private set; }

		// Token: 0x170010F5 RID: 4341
		// (get) Token: 0x06004CC6 RID: 19654 RVA: 0x00073F0C File Offset: 0x0007210C
		// (set) Token: 0x06004CC7 RID: 19655 RVA: 0x00073F14 File Offset: 0x00072114
		public DateTime Timestamp { get; private set; }

		// Token: 0x170010F6 RID: 4342
		// (get) Token: 0x06004CC8 RID: 19656 RVA: 0x00073F1D File Offset: 0x0007211D
		// (set) Token: 0x06004CC9 RID: 19657 RVA: 0x00073F25 File Offset: 0x00072125
		public string Contents { get; private set; }

		// Token: 0x170010F7 RID: 4343
		// (get) Token: 0x06004CCA RID: 19658 RVA: 0x00073F2E File Offset: 0x0007212E
		// (set) Token: 0x06004CCB RID: 19659 RVA: 0x00073F36 File Offset: 0x00072136
		public string Sender { get; private set; }

		// Token: 0x170010F8 RID: 4344
		// (get) Token: 0x06004CCC RID: 19660 RVA: 0x00073F3F File Offset: 0x0007213F
		// (set) Token: 0x06004CCD RID: 19661 RVA: 0x00073F47 File Offset: 0x00072147
		public string Receiver { get; private set; }

		// Token: 0x170010F9 RID: 4345
		// (get) Token: 0x06004CCE RID: 19662 RVA: 0x00073F50 File Offset: 0x00072150
		// (set) Token: 0x06004CCF RID: 19663 RVA: 0x00073F58 File Offset: 0x00072158
		public PresenceFlags Presence { get; private set; }

		// Token: 0x170010FA RID: 4346
		// (get) Token: 0x06004CD0 RID: 19664 RVA: 0x00073F61 File Offset: 0x00072161
		// (set) Token: 0x06004CD1 RID: 19665 RVA: 0x00073F69 File Offset: 0x00072169
		public AccessFlags Access { get; private set; }

		// Token: 0x170010FB RID: 4347
		// (get) Token: 0x06004CD2 RID: 19666 RVA: 0x00073F72 File Offset: 0x00072172
		// (set) Token: 0x06004CD3 RID: 19667 RVA: 0x00073F7A File Offset: 0x0007217A
		public string FormattedTimestamp { get; private set; }

		// Token: 0x170010FC RID: 4348
		// (get) Token: 0x06004CD4 RID: 19668 RVA: 0x00073F83 File Offset: 0x00072183
		// (set) Token: 0x06004CD5 RID: 19669 RVA: 0x00073F8B File Offset: 0x0007218B
		public string SenderLink { get; private set; }

		// Token: 0x170010FD RID: 4349
		// (get) Token: 0x06004CD6 RID: 19670 RVA: 0x00073F94 File Offset: 0x00072194
		// (set) Token: 0x06004CD7 RID: 19671 RVA: 0x00073F9C File Offset: 0x0007219C
		public string ReceiverLink { get; private set; }

		// Token: 0x170010FE RID: 4350
		// (get) Token: 0x06004CD8 RID: 19672 RVA: 0x001BDA2C File Offset: 0x001BBC2C
		public string ContentsLinkified
		{
			get
			{
				string result;
				using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
				{
					MatchCollection matchCollection = ChatMessage.ArchetypeLinkPattern.Matches(this.Contents);
					MatchCollection matchCollection2 = ChatMessage.InstanceLinkPattern.Matches(this.Contents);
					List<Capture> fromPool = StaticListPool<Capture>.GetFromPool();
					int num = 0;
					int num2 = 0;
					while (fromPool.Count < matchCollection.Count + matchCollection2.Count)
					{
						if (num < matchCollection.Count && num2 >= matchCollection2.Count)
						{
							fromPool.Add(matchCollection[num].Groups[0]);
							num++;
						}
						else if (num >= matchCollection.Count && num2 < matchCollection2.Count)
						{
							fromPool.Add(matchCollection2[num2].Groups[0]);
							num2++;
						}
						else if (matchCollection[num].Groups[0].Index < matchCollection2[num2].Groups[0].Index)
						{
							fromPool.Add(matchCollection[num].Groups[0]);
							num++;
						}
						else
						{
							fromPool.Add(matchCollection2[num2].Groups[0]);
							num2++;
						}
					}
					int num3 = 0;
					foreach (Capture capture in fromPool)
					{
						if (capture.Index - num3 >= 0)
						{
							utf16ValueStringBuilder.Append(this.Contents.Substring(num3, capture.Index - num3));
							if (this.Type.AddNoParse())
							{
								utf16ValueStringBuilder.AppendFormat<string>("</noparse>{0}<noparse>", capture.Value);
							}
							else
							{
								utf16ValueStringBuilder.Append(capture.Value);
							}
							num3 = capture.Index + capture.Length;
						}
					}
					StaticListPool<Capture>.ReturnToPool(fromPool);
					utf16ValueStringBuilder.Append(this.Contents.Substring(num3));
					result = utf16ValueStringBuilder.ToString();
				}
				return result;
			}
		}

		// Token: 0x06004CD9 RID: 19673 RVA: 0x001BDC74 File Offset: 0x001BBE74
		public void Init(MessageType type, string content, string sender = null, string receiver = null, PresenceFlags presence = PresenceFlags.Online, AccessFlags access = AccessFlags.None)
		{
			this.Type = type;
			this.Timestamp = DateTime.Now;
			this.Contents = content;
			this.Sender = sender;
			this.Receiver = receiver;
			this.Presence = presence;
			this.Access = access;
			this.FormattedTimestamp = this.Timestamp.ToString("HH:mm");
			this.SenderLink = (string.IsNullOrEmpty(this.Sender) ? string.Empty : SoL.Utilities.Extensions.TextMeshProExtensions.CreatePlayerLink(this.Sender));
			this.ReceiverLink = (string.IsNullOrEmpty(this.Receiver) ? string.Empty : SoL.Utilities.Extensions.TextMeshProExtensions.CreatePlayerLink(this.Receiver));
			this.m_isPreformatted = this.Type.HasBitFlag(MessageType.PreFormatted);
			if (this.m_isPreformatted)
			{
				this.Type &= ~MessageType.PreFormatted;
			}
		}

		// Token: 0x06004CDA RID: 19674 RVA: 0x001BDD4C File Offset: 0x001BBF4C
		public string GetCachedFormattedMessage(bool showTimestamp)
		{
			if (showTimestamp && !this.m_cachedFormattedMessageWithTimestamp)
			{
				this.m_formattedMessageWithTimestamp = this.GetFormattedMessageBuildBackwards(true);
				this.m_cachedFormattedMessageWithTimestamp = true;
			}
			else if (!showTimestamp && !this.m_cachedFormattedMessage)
			{
				this.m_formattedMessage = this.GetFormattedMessageBuildBackwards(false);
				this.m_cachedFormattedMessage = true;
			}
			if (!showTimestamp)
			{
				return this.m_formattedMessage;
			}
			return this.m_formattedMessageWithTimestamp;
		}

		// Token: 0x06004CDB RID: 19675 RVA: 0x00073FA5 File Offset: 0x000721A5
		public void CustomColorsChanged()
		{
			this.m_cachedFormattedMessageWithTimestamp = false;
			this.m_cachedFormattedMessage = false;
		}

		// Token: 0x170010FF RID: 4351
		// (get) Token: 0x06004CDC RID: 19676 RVA: 0x00073FB5 File Offset: 0x000721B5
		// (set) Token: 0x06004CDD RID: 19677 RVA: 0x00073FBD File Offset: 0x000721BD
		public float PreferredHeight
		{
			get
			{
				return this.m_preferredHeight;
			}
			set
			{
				this.m_preferredHeight = value;
				this.m_lastCachedPreferredHeight = Time.time;
			}
		}

		// Token: 0x06004CDE RID: 19678 RVA: 0x00073FD1 File Offset: 0x000721D1
		public bool ShouldRecalculatePreferredHeight(float timeOfLastUpdate)
		{
			return timeOfLastUpdate < 0f || this.m_lastCachedPreferredHeight < 0f || timeOfLastUpdate > this.m_lastCachedPreferredHeight;
		}

		// Token: 0x17001100 RID: 4352
		// (get) Token: 0x06004CDF RID: 19679 RVA: 0x00073FF3 File Offset: 0x000721F3
		// (set) Token: 0x06004CE0 RID: 19680 RVA: 0x00073FFB File Offset: 0x000721FB
		bool IPoolable.InPool
		{
			get
			{
				return this.m_inPool;
			}
			set
			{
				this.m_inPool = value;
			}
		}

		// Token: 0x06004CE1 RID: 19681 RVA: 0x001BDDAC File Offset: 0x001BBFAC
		public void Reset()
		{
			this.Type = MessageType.None;
			this.Timestamp = DateTime.MinValue;
			this.Contents = null;
			this.Sender = null;
			this.Receiver = null;
			this.Presence = PresenceFlags.Invalid;
			this.Access = AccessFlags.None;
			this.FormattedTimestamp = null;
			this.SenderLink = null;
			this.ReceiverLink = null;
			this.m_cachedFormattedMessage = false;
			this.m_formattedMessage = null;
			this.m_cachedFormattedMessageWithTimestamp = false;
			this.m_formattedMessageWithTimestamp = null;
			this.m_isPreformatted = false;
			this.m_lastCachedPreferredHeight = -1f;
			this.m_preferredHeight = 0f;
		}

		// Token: 0x06004CE2 RID: 19682 RVA: 0x001BDE3C File Offset: 0x001BC03C
		public bool Equals(ChatMessage other)
		{
			return other != null && (this == other || (this.Type == other.Type && this.Timestamp.Equals(other.Timestamp) && this.Contents == other.Contents && this.Sender == other.Sender));
		}

		// Token: 0x06004CE3 RID: 19683 RVA: 0x00074004 File Offset: 0x00072204
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((ChatMessage)obj)));
		}

		// Token: 0x06004CE4 RID: 19684 RVA: 0x001BDEA0 File Offset: 0x001BC0A0
		public override int GetHashCode()
		{
			return (int)(((this.Type * (MessageType.Notification | MessageType.Motd | MessageType.Skills | MessageType.Party | MessageType.Guild) ^ (MessageType)this.Timestamp.GetHashCode()) * (MessageType.Notification | MessageType.Motd | MessageType.Skills | MessageType.Party | MessageType.Guild) ^ (MessageType)((this.Contents != null) ? this.Contents.GetHashCode() : 0)) * (MessageType.Notification | MessageType.Motd | MessageType.Skills | MessageType.Party | MessageType.Guild) ^ (MessageType)((this.Sender != null) ? this.Sender.GetHashCode() : 0));
		}

		// Token: 0x06004CE5 RID: 19685 RVA: 0x00074032 File Offset: 0x00072232
		public bool Equals(ChatMessage x, ChatMessage y)
		{
			return x != null && x.Equals(y);
		}

		// Token: 0x06004CE6 RID: 19686 RVA: 0x00050A5F File Offset: 0x0004EC5F
		public int GetHashCode(ChatMessage obj)
		{
			return obj.GetHashCode();
		}

		// Token: 0x06004CE7 RID: 19687 RVA: 0x00074040 File Offset: 0x00072240
		public static void ResetMessageTypePrefixes()
		{
			ChatMessage.m_messageTypeFormatPrefixes = null;
		}

		// Token: 0x17001101 RID: 4353
		// (get) Token: 0x06004CE8 RID: 19688 RVA: 0x001BDF04 File Offset: 0x001BC104
		private static Dictionary<MessageType, ChatMessage.MessageTypePrefixData> MessageTypeFormatPrefixes
		{
			get
			{
				if (ChatMessage.m_messageTypeFormatPrefixes == null)
				{
					ChatMessage.m_messageTypeFormatPrefixes = new Dictionary<MessageType, ChatMessage.MessageTypePrefixData>(default(MessageTypeComparer));
					foreach (object obj in Enum.GetValues(typeof(MessageType)))
					{
						MessageType messageType = (MessageType)obj;
						string text = messageType.GetDisplayChannel().ToUpper();
						if (!string.IsNullOrEmpty(text))
						{
							string text2 = text;
							string text3 = text[0].ToString();
							Color color;
							if (messageType.GetColor(out color, false))
							{
								text2 = text2.Color(color);
								text3 = text3.Color(color);
							}
							string firstLetterPrefixLink = SoL.Utilities.Extensions.TextMeshProExtensions.CreateTextTooltipLink(text, string.Concat(new string[]
							{
								"<mspace=",
								1.ToString(),
								"em>",
								text3,
								"</mspace>"
							}));
							ChatMessage.MessageTypePrefixData value = new ChatMessage.MessageTypePrefixData
							{
								FullPrefix = text2,
								FirstLetterPrefix = text3,
								FirstLetterPrefixLink = firstLetterPrefixLink
							};
							ChatMessage.m_messageTypeFormatPrefixes.Add(messageType, value);
						}
					}
				}
				return ChatMessage.m_messageTypeFormatPrefixes;
			}
		}

		// Token: 0x06004CE9 RID: 19689 RVA: 0x001BE050 File Offset: 0x001BC250
		private string GetFormattedMessageBuildBackwards(bool showTimestamp)
		{
			string result;
			try
			{
				using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
				{
					ChatMessage.MessageTypePrefixData messageTypePrefixData;
					if (ChatMessage.MessageTypeFormatPrefixes.TryGetValue(this.Type, out messageTypePrefixData))
					{
						if (showTimestamp)
						{
							utf16ValueStringBuilder.AppendFormat<string, string>("[{0} {1}] ", this.FormattedTimestamp, messageTypePrefixData.FirstLetterPrefixLink);
						}
						else
						{
							utf16ValueStringBuilder.AppendFormat<string>("[{0}] ", messageTypePrefixData.FirstLetterPrefixLink);
						}
					}
					else if (showTimestamp)
					{
						utf16ValueStringBuilder.AppendFormat<string>("[{0}] ", this.FormattedTimestamp);
					}
					if (this.Type.IsChat())
					{
						string presenceChatPrefix = ChatMessage.GetPresenceChatPrefix(this.Presence);
						if (!string.IsNullOrEmpty(this.Sender))
						{
							string chatPrefix = this.GetChatPrefix();
							if (this.Type == MessageType.Tell)
							{
								utf16ValueStringBuilder.AppendFormat<string, string, string>("<b>FROM</b> {0}<i>{1}</i>{2}: ", chatPrefix, this.SenderLink, presenceChatPrefix);
							}
							else
							{
								utf16ValueStringBuilder.AppendFormat<string, string>("{0}<i>{1}</i>: ", chatPrefix, this.SenderLink);
							}
						}
						else if (!string.IsNullOrEmpty(this.Receiver))
						{
							string chatPrefix2 = this.GetChatPrefix();
							utf16ValueStringBuilder.AppendFormat<string, string, string>("<b>TO</b> {0}<i>{1}</i>{2}: ", chatPrefix2, this.ReceiverLink, presenceChatPrefix);
						}
					}
					Color white = Color.white;
					bool flag = this.Type.ColorMessageContent() && this.Type.GetColor(out white, false);
					if (this.m_isPreformatted)
					{
						if (flag)
						{
							utf16ValueStringBuilder.AppendFormat<string>("<color={0}>", white.ToHex());
						}
						utf16ValueStringBuilder.Append(this.Contents);
						if (flag)
						{
							utf16ValueStringBuilder.Append("</color>");
						}
					}
					else
					{
						bool flag2 = this.Type.AddNoParse();
						bool flag3 = this.Type == MessageType.Emote;
						if (flag3)
						{
							utf16ValueStringBuilder.AppendFormat<string>("<i><b>{0}</b> ", this.SenderLink);
						}
						if (flag)
						{
							utf16ValueStringBuilder.AppendFormat<string>("<color={0}>", white.ToHex());
						}
						if (flag2)
						{
							utf16ValueStringBuilder.Append("<noparse>");
						}
						utf16ValueStringBuilder.Append(this.ContentsLinkified);
						if (flag2)
						{
							utf16ValueStringBuilder.Append("</noparse>");
						}
						if (flag)
						{
							utf16ValueStringBuilder.Append("</color>");
						}
						if (flag3)
						{
							utf16ValueStringBuilder.Append("</i>");
						}
					}
					result = utf16ValueStringBuilder.ToString();
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Failure in message formatting: {0}", arg));
				result = "[UNKNOWN ERROR]";
			}
			return result;
		}

		// Token: 0x06004CEA RID: 19690 RVA: 0x001BE2B8 File Offset: 0x001BC4B8
		private string GetChatPrefix()
		{
			if (this.Access != AccessFlags.None)
			{
				if (this.Access.HasBitFlag(AccessFlags.GM) && this.Presence.HasBitFlag(PresenceFlags.GM))
				{
					return ZString.Format<string, string>("{0}[<color={1}>GM</color>]</link> ", "<link=\"text:Developer\">", UIManager.EmberColor.ToHex());
				}
				if (this.Access.HasBitFlag(AccessFlags.Subscriber))
				{
					return ZString.Format<string, string, string>(" <link=\"{0}:Subscriber\"><color={1}><size=80%>{2}</size></color></link> ", "text", UIManager.SubscriberColor.ToHex(), UIManager.SubscriberChatIcon);
				}
			}
			return string.Empty;
		}

		// Token: 0x06004CEB RID: 19691 RVA: 0x001BE338 File Offset: 0x001BC538
		private static string GetPresenceChatPrefix(PresenceFlags value)
		{
			if (value <= PresenceFlags.Online || value == PresenceFlags.Anonymous || value == PresenceFlags.Invisible)
			{
				return string.Empty;
			}
			string result;
			if (ChatMessage.m_presenceChatPrefixes == null)
			{
				ChatMessage.m_presenceChatPrefixes = new Dictionary<PresenceFlags, string>();
			}
			else if (ChatMessage.m_presenceChatPrefixes.TryGetValue(value, out result))
			{
				return result;
			}
			string text = value.ToStringAbbreviation();
			if (string.IsNullOrEmpty(text))
			{
				ChatMessage.m_presenceChatPrefixes.Add(value, string.Empty);
				return string.Empty;
			}
			string text2 = ZString.Format<string>(" ({0})", text);
			ChatMessage.m_presenceChatPrefixes.Add(value, text2);
			return text2;
		}

		// Token: 0x04004693 RID: 18067
		private const string kTimestampFormat = "HH:mm";

		// Token: 0x04004694 RID: 18068
		public static Regex ArchetypeLinkPattern = new Regex("<link=\"archetypeId:([0-9a-f]{32})\">(?:<color=#[0-9A-F]{6}>)?\\[[\\w\\s'\\-<>#\\/=]+\\](?:<\\/color>)?<\\/link>", RegexOptions.Compiled);

		// Token: 0x04004695 RID: 18069
		public static Regex InstanceLinkPattern = new Regex("<link=\"instanceId:([0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12})\">(?:<color=#[0-9A-F]{6}>)?\\[[\\w\\s'\\-:()\"&!]+\\](?:<\\/color>)?<\\/link>", RegexOptions.Compiled);

		// Token: 0x040046A0 RID: 18080
		private bool m_inPool;

		// Token: 0x040046A1 RID: 18081
		private bool m_isPreformatted;

		// Token: 0x040046A2 RID: 18082
		private bool m_cachedFormattedMessage;

		// Token: 0x040046A3 RID: 18083
		private string m_formattedMessage;

		// Token: 0x040046A4 RID: 18084
		private bool m_cachedFormattedMessageWithTimestamp;

		// Token: 0x040046A5 RID: 18085
		private string m_formattedMessageWithTimestamp;

		// Token: 0x040046A6 RID: 18086
		private float m_preferredHeight;

		// Token: 0x040046A7 RID: 18087
		private float m_lastCachedPreferredHeight = -1f;

		// Token: 0x040046A8 RID: 18088
		private const int kMspaceEm = 1;

		// Token: 0x040046A9 RID: 18089
		private static Dictionary<MessageType, ChatMessage.MessageTypePrefixData> m_messageTypeFormatPrefixes = null;

		// Token: 0x040046AA RID: 18090
		private static Dictionary<PresenceFlags, string> m_presenceChatPrefixes = null;

		// Token: 0x020009DD RID: 2525
		private struct MessageTypePrefixData
		{
			// Token: 0x040046AB RID: 18091
			public string FullPrefix;

			// Token: 0x040046AC RID: 18092
			public string FirstLetterPrefix;

			// Token: 0x040046AD RID: 18093
			public string FirstLetterPrefixLink;
		}
	}
}
