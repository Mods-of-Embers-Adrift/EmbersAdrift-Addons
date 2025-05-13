using System;
using System.Collections.Generic;
using SoL.Networking.Database;
using SoL.Utilities.Extensions;

namespace SoL.Game.Messages
{
	// Token: 0x020009DE RID: 2526
	public struct Message : IEquatable<Message>, IEqualityComparer<Message>
	{
		// Token: 0x17001102 RID: 4354
		// (get) Token: 0x06004CEE RID: 19694 RVA: 0x00074089 File Offset: 0x00072289
		// (set) Token: 0x06004CEF RID: 19695 RVA: 0x00074091 File Offset: 0x00072291
		public MessageType Type { readonly get; private set; }

		// Token: 0x17001103 RID: 4355
		// (get) Token: 0x06004CF0 RID: 19696 RVA: 0x0007409A File Offset: 0x0007229A
		// (set) Token: 0x06004CF1 RID: 19697 RVA: 0x000740A2 File Offset: 0x000722A2
		public DateTime Timestamp { readonly get; private set; }

		// Token: 0x17001104 RID: 4356
		// (get) Token: 0x06004CF2 RID: 19698 RVA: 0x000740AB File Offset: 0x000722AB
		public string Contents
		{
			get
			{
				return this.m_contents;
			}
		}

		// Token: 0x17001105 RID: 4357
		// (get) Token: 0x06004CF3 RID: 19699 RVA: 0x000740B3 File Offset: 0x000722B3
		public PresenceFlags Presence
		{
			get
			{
				return this.m_presence;
			}
		}

		// Token: 0x17001106 RID: 4358
		// (get) Token: 0x06004CF4 RID: 19700 RVA: 0x000740BB File Offset: 0x000722BB
		public string Sender
		{
			get
			{
				return this.m_sender;
			}
		}

		// Token: 0x17001107 RID: 4359
		// (get) Token: 0x06004CF5 RID: 19701 RVA: 0x000740C3 File Offset: 0x000722C3
		public string ReceiverLink
		{
			get
			{
				return this.m_receiverLink;
			}
		}

		// Token: 0x17001108 RID: 4360
		// (get) Token: 0x06004CF6 RID: 19702 RVA: 0x000740CB File Offset: 0x000722CB
		public string Receiver
		{
			get
			{
				return this.m_receiver;
			}
		}

		// Token: 0x17001109 RID: 4361
		// (get) Token: 0x06004CF7 RID: 19703 RVA: 0x000740D3 File Offset: 0x000722D3
		public string SenderLink
		{
			get
			{
				return this.m_senderLink;
			}
		}

		// Token: 0x1700110A RID: 4362
		// (get) Token: 0x06004CF8 RID: 19704 RVA: 0x000740DB File Offset: 0x000722DB
		public string FormattedTimestamp
		{
			get
			{
				return this.m_formattedTimestamp;
			}
		}

		// Token: 0x06004CF9 RID: 19705 RVA: 0x001BE3BC File Offset: 0x001BC5BC
		public Message(MessageType type, string content, string sender = null, string receiver = null, PresenceFlags presence = PresenceFlags.Online)
		{
			this.Type = type;
			this.Timestamp = DateTime.Now;
			this.m_contents = content;
			this.m_sender = sender;
			this.m_receiver = receiver;
			this.m_presence = presence;
			this.m_formattedTimestamp = this.Timestamp.ToString("HH:mm");
			this.m_cachedFormattedMessage = false;
			this.m_formattedMessage = string.Empty;
			this.m_cachedFormattedMessageWithTimestamp = false;
			this.m_formattedMessageWithTimestamp = string.Empty;
			this.m_senderLink = (string.IsNullOrEmpty(this.m_sender) ? string.Empty : TextMeshProExtensions.CreatePlayerLink(this.m_sender));
			this.m_receiverLink = (string.IsNullOrEmpty(this.m_receiver) ? string.Empty : TextMeshProExtensions.CreatePlayerLink(this.m_receiver));
		}

		// Token: 0x06004CFA RID: 19706 RVA: 0x001BE480 File Offset: 0x001BC680
		public string GetCachedFormattedMessage(bool showTimestamp)
		{
			if (showTimestamp && !this.m_cachedFormattedMessageWithTimestamp)
			{
				this.m_formattedMessageWithTimestamp = this.GetFormattedMessageBuildBackwards(showTimestamp);
				this.m_cachedFormattedMessageWithTimestamp = true;
			}
			else if (!showTimestamp && !this.m_cachedFormattedMessage)
			{
				this.m_formattedMessage = this.GetFormattedMessageBuildBackwards(showTimestamp);
				this.m_cachedFormattedMessage = true;
			}
			if (!showTimestamp)
			{
				return this.m_formattedMessage;
			}
			return this.m_formattedMessageWithTimestamp;
		}

		// Token: 0x06004CFB RID: 19707 RVA: 0x001BE4E8 File Offset: 0x001BC6E8
		public bool Equals(Message other)
		{
			return string.Equals(this.m_contents, other.m_contents) && string.Equals(this.m_sender, other.m_sender) && this.Type == other.Type && this.Timestamp.Equals(other.Timestamp);
		}

		// Token: 0x06004CFC RID: 19708 RVA: 0x001BE544 File Offset: 0x001BC744
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is Message)
			{
				Message other = (Message)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06004CFD RID: 19709 RVA: 0x001BE570 File Offset: 0x001BC770
		public override int GetHashCode()
		{
			return ((((this.m_contents != null) ? this.m_contents.GetHashCode() : 0) * 397 ^ ((this.m_sender != null) ? this.m_sender.GetHashCode() : 0)) * 397 ^ (int)this.Type) * 397 ^ this.Timestamp.GetHashCode();
		}

		// Token: 0x06004CFE RID: 19710 RVA: 0x000740E3 File Offset: 0x000722E3
		public bool Equals(Message x, Message y)
		{
			return x.Equals(y);
		}

		// Token: 0x06004CFF RID: 19711 RVA: 0x000740ED File Offset: 0x000722ED
		public int GetHashCode(Message obj)
		{
			return obj.GetHashCode();
		}

		// Token: 0x040046AE RID: 18094
		private const string kTimestampFormat = "HH:mm";

		// Token: 0x040046B1 RID: 18097
		private readonly string m_contents;

		// Token: 0x040046B2 RID: 18098
		private readonly PresenceFlags m_presence;

		// Token: 0x040046B3 RID: 18099
		private readonly string m_sender;

		// Token: 0x040046B4 RID: 18100
		private readonly string m_receiverLink;

		// Token: 0x040046B5 RID: 18101
		private readonly string m_receiver;

		// Token: 0x040046B6 RID: 18102
		private readonly string m_senderLink;

		// Token: 0x040046B7 RID: 18103
		private readonly string m_formattedTimestamp;

		// Token: 0x040046B8 RID: 18104
		private bool m_cachedFormattedMessage;

		// Token: 0x040046B9 RID: 18105
		private string m_formattedMessage;

		// Token: 0x040046BA RID: 18106
		private bool m_cachedFormattedMessageWithTimestamp;

		// Token: 0x040046BB RID: 18107
		private string m_formattedMessageWithTimestamp;
	}
}
