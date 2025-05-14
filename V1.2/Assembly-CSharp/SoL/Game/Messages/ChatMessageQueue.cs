using System;
using System.Collections.Generic;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Utilities;

namespace SoL.Game.Messages
{
	// Token: 0x020009E4 RID: 2532
	public class ChatMessageQueue
	{
		// Token: 0x140000EE RID: 238
		// (add) Token: 0x06004D12 RID: 19730 RVA: 0x001BEFC4 File Offset: 0x001BD1C4
		// (remove) Token: 0x06004D13 RID: 19731 RVA: 0x001BEFFC File Offset: 0x001BD1FC
		public event Action QueueCleared;

		// Token: 0x140000EF RID: 239
		// (add) Token: 0x06004D14 RID: 19732 RVA: 0x001BF034 File Offset: 0x001BD234
		// (remove) Token: 0x06004D15 RID: 19733 RVA: 0x001BF06C File Offset: 0x001BD26C
		public event Action<ChatMessage> MessageAdded;

		// Token: 0x140000F0 RID: 240
		// (add) Token: 0x06004D16 RID: 19734 RVA: 0x001BF0A4 File Offset: 0x001BD2A4
		// (remove) Token: 0x06004D17 RID: 19735 RVA: 0x001BF0DC File Offset: 0x001BD2DC
		public event Action<ChatMessage> MessageRemoved;

		// Token: 0x140000F1 RID: 241
		// (add) Token: 0x06004D18 RID: 19736 RVA: 0x001BF114 File Offset: 0x001BD314
		// (remove) Token: 0x06004D19 RID: 19737 RVA: 0x001BF14C File Offset: 0x001BD34C
		public event Action<ChatMessage, bool> MessageAddedQueueTrimmed;

		// Token: 0x1700110E RID: 4366
		// (get) Token: 0x06004D1A RID: 19738 RVA: 0x000741B6 File Offset: 0x000723B6
		public int Count
		{
			get
			{
				return this.m_messages.Count;
			}
		}

		// Token: 0x06004D1B RID: 19739 RVA: 0x000741C3 File Offset: 0x000723C3
		public ChatMessage AddToQueue(MessageType type, string content)
		{
			return this.AddToQueue(type, content, null, null, PresenceFlags.Online, AccessFlags.Active);
		}

		// Token: 0x06004D1C RID: 19740 RVA: 0x001BF184 File Offset: 0x001BD384
		public ChatMessage AddToQueue(MessageType type, string content, string sender, string receiver, PresenceFlags presence, AccessFlags access = AccessFlags.None)
		{
			if ((type.IsChat() || type == MessageType.Emote) && !string.IsNullOrEmpty(sender) && ClientGameManager.SocialManager != null && ClientGameManager.SocialManager.IsBlocked(sender))
			{
				return null;
			}
			ChatMessage fromPool = StaticPool<ChatMessage>.GetFromPool();
			fromPool.Init(type, content, sender, receiver, presence, access);
			this.m_messages.Add(fromPool);
			bool arg = this.TrimQueue();
			Action<ChatMessage> messageAdded = this.MessageAdded;
			if (messageAdded != null)
			{
				messageAdded(fromPool);
			}
			Action<ChatMessage, bool> messageAddedQueueTrimmed = this.MessageAddedQueueTrimmed;
			if (messageAddedQueueTrimmed != null)
			{
				messageAddedQueueTrimmed(fromPool, arg);
			}
			return fromPool;
		}

		// Token: 0x06004D1D RID: 19741 RVA: 0x001BF214 File Offset: 0x001BD414
		private bool TrimQueue()
		{
			bool result = false;
			if (this.m_messages.Count > 288)
			{
				while (this.m_messages.Count > 256)
				{
					ChatMessage chatMessage = this.m_messages[0];
					Action<ChatMessage> messageRemoved = this.MessageRemoved;
					if (messageRemoved != null)
					{
						messageRemoved(chatMessage);
					}
					this.m_messages.RemoveAt(0);
					result = true;
					StaticPool<ChatMessage>.ReturnToPool(chatMessage);
				}
			}
			return result;
		}

		// Token: 0x06004D1E RID: 19742 RVA: 0x001BF280 File Offset: 0x001BD480
		public void ClearQueue()
		{
			for (int i = 0; i < this.m_messages.Count; i++)
			{
				StaticPool<ChatMessage>.ReturnToPool(this.m_messages[i]);
			}
			this.m_messages.Clear();
			Action queueCleared = this.QueueCleared;
			if (queueCleared == null)
			{
				return;
			}
			queueCleared();
		}

		// Token: 0x06004D1F RID: 19743 RVA: 0x001BF2D0 File Offset: 0x001BD4D0
		public void CustomColorsChanged()
		{
			for (int i = 0; i < this.m_messages.Count; i++)
			{
				ChatMessage chatMessage = this.m_messages[i];
				if (chatMessage != null)
				{
					chatMessage.CustomColorsChanged();
				}
			}
		}

		// Token: 0x06004D20 RID: 19744 RVA: 0x000741D1 File Offset: 0x000723D1
		public IEnumerable<ChatMessage> GetFilteredMessages(MessageType flags)
		{
			int num;
			for (int i = 0; i < this.m_messages.Count; i = num + 1)
			{
				if (flags.HasBitFlag(this.m_messages[i].Type))
				{
					yield return this.m_messages[i];
				}
				num = i;
			}
			yield break;
		}

		// Token: 0x06004D21 RID: 19745 RVA: 0x000741E8 File Offset: 0x000723E8
		public ChatMessage GetMessageAtIndex(int index)
		{
			if (index < 0 || index >= this.m_messages.Count)
			{
				throw new IndexOutOfRangeException("index");
			}
			return this.m_messages[index];
		}

		// Token: 0x040046D5 RID: 18133
		private const int kTargetMessageCount = 256;

		// Token: 0x040046D6 RID: 18134
		private const int kBuffer = 32;

		// Token: 0x040046D7 RID: 18135
		public const int kMaxMessages = 288;

		// Token: 0x040046D8 RID: 18136
		private readonly List<ChatMessage> m_messages = new List<ChatMessage>(288);
	}
}
