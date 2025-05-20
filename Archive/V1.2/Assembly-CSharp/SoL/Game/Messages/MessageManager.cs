using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NetStack.Serialization;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Messages
{
	// Token: 0x020009E2 RID: 2530
	public static class MessageManager
	{
		// Token: 0x06004D06 RID: 19718 RVA: 0x001BE9F4 File Offset: 0x001BCBF4
		public static bool TryGetLinkedInstance(string instanceId, out ArchetypeInstance instance)
		{
			string attachmentsStr;
			return MessageManager.LinkedInstances.TryGetValue(instanceId, out instance) || (MessageManager.AttachmentsCache.TryGetValue(instanceId, out attachmentsStr) && MessageManager.ExtractInstanceAttachments(attachmentsStr) > 0 && MessageManager.LinkedInstances.TryGetValue(instanceId, out instance));
		}

		// Token: 0x06004D07 RID: 19719 RVA: 0x001BEA38 File Offset: 0x001BCC38
		public static void AddLinkedInstance(ArchetypeInstance instance, bool createCopy = false)
		{
			if (instance == null || instance.InstanceId.IsEmpty)
			{
				return;
			}
			string value = instance.InstanceId.Value;
			ArchetypeInstance archetypeInstance;
			if (MessageManager.LinkedInstances.TryGetValue(value, out archetypeInstance))
			{
				archetypeInstance.CopyDataFrom(instance);
				MessageManager.LinkedInstanceIds.Remove(value);
				MessageManager.LinkedInstanceIds.Add(value);
			}
			else
			{
				ArchetypeInstance archetypeInstance2 = instance;
				if (createCopy)
				{
					archetypeInstance2 = StaticPool<ArchetypeInstance>.GetFromPool();
					archetypeInstance2.CopyDataFrom(instance);
				}
				MessageManager.LinkedInstances.Add(value, archetypeInstance2);
			}
			if (MessageManager.LinkedInstanceIds.Count > 120)
			{
				for (int i = 0; i < MessageManager.LinkedInstanceIds.Count; i++)
				{
					ArchetypeInstance item;
					if (!string.IsNullOrEmpty(MessageManager.LinkedInstanceIds[i]) && MessageManager.LinkedInstances.TryGetValue(MessageManager.LinkedInstanceIds[i], out item))
					{
						StaticPool<ArchetypeInstance>.ReturnToPool(item);
						MessageManager.LinkedInstances.Remove(MessageManager.LinkedInstanceIds[i]);
					}
					MessageManager.LinkedInstanceIds.RemoveAt(i);
					i--;
					if (i < 0 || MessageManager.LinkedInstanceIds.Count < 120)
					{
						break;
					}
				}
			}
		}

		// Token: 0x06004D08 RID: 19720 RVA: 0x001BEB3C File Offset: 0x001BCD3C
		public static void AddAttachmentInstanceId(string instanceId, string attachmentStr)
		{
			if (string.IsNullOrEmpty(instanceId) || string.IsNullOrEmpty(attachmentStr))
			{
				return;
			}
			if (MessageManager.AttachmentsCache.ContainsKey(instanceId))
			{
				MessageManager.AttachmentsCache[instanceId] = attachmentStr;
				MessageManager.AttachmentInstanceIds.Remove(instanceId);
				MessageManager.AttachmentInstanceIds.Add(instanceId);
			}
			else
			{
				MessageManager.AttachmentsCache.Add(instanceId, attachmentStr);
				MessageManager.AttachmentInstanceIds.Add(instanceId);
			}
			if (MessageManager.AttachmentInstanceIds.Count > 120)
			{
				for (int i = 0; i < MessageManager.AttachmentInstanceIds.Count; i++)
				{
					if (!string.IsNullOrEmpty(MessageManager.AttachmentInstanceIds[i]))
					{
						MessageManager.AttachmentsCache.Remove(MessageManager.AttachmentInstanceIds[i]);
					}
					MessageManager.AttachmentInstanceIds.RemoveAt(i);
					i--;
					if (i < 0 || MessageManager.AttachmentInstanceIds.Count < 120)
					{
						break;
					}
				}
			}
		}

		// Token: 0x1700110C RID: 4364
		// (get) Token: 0x06004D09 RID: 19721 RVA: 0x0007414D File Offset: 0x0007234D
		public static ChatMessageQueue ChatQueue
		{
			get
			{
				if (MessageManager.m_chatQueue == null)
				{
					MessageManager.m_chatQueue = new ChatMessageQueue();
				}
				return MessageManager.m_chatQueue;
			}
		}

		// Token: 0x1700110D RID: 4365
		// (get) Token: 0x06004D0A RID: 19722 RVA: 0x00074165 File Offset: 0x00072365
		public static ChatMessageQueue CombatQueue
		{
			get
			{
				if (MessageManager.m_combatQueue == null)
				{
					MessageManager.m_combatQueue = new ChatMessageQueue();
				}
				return MessageManager.m_combatQueue;
			}
		}

		// Token: 0x06004D0B RID: 19723 RVA: 0x001BEC10 File Offset: 0x001BCE10
		static MessageManager()
		{
			for (int i = 0; i < 288; i++)
			{
				StaticPool<ChatMessage>.ReturnToPool(StaticPool<ChatMessage>.GetFromPool());
			}
		}

		// Token: 0x06004D0C RID: 19724 RVA: 0x0007417D File Offset: 0x0007237D
		public static void ResetQueues()
		{
			ChatMessageQueue chatQueue = MessageManager.m_chatQueue;
			if (chatQueue != null)
			{
				chatQueue.ClearQueue();
			}
			ChatMessageQueue combatQueue = MessageManager.m_combatQueue;
			if (combatQueue == null)
			{
				return;
			}
			combatQueue.ClearQueue();
		}

		// Token: 0x06004D0D RID: 19725 RVA: 0x001BEC90 File Offset: 0x001BCE90
		public static string FindAndEncodeInstanceAttachments(string message)
		{
			List<ArchetypeInstance> fromPool = StaticListPool<ArchetypeInstance>.GetFromPool();
			int startIndex = 0;
			string result = null;
			int num;
			while ((num = message.IndexOf("instanceId:", startIndex)) != -1)
			{
				string text = message.Substring(num + "instanceId".Length + 1, 36);
				if (MessageManager.LinkedInstances.ContainsKey(text))
				{
					ArchetypeInstance item = MessageManager.LinkedInstances[text];
					if (!MessageManager.LinkedInstances[text].InstanceId.IsEmpty && !fromPool.Contains(item))
					{
						fromPool.Add(item);
					}
				}
				else
				{
					Debug.LogError("Found instance link with no matching instance. " + text);
				}
				startIndex = num + 1;
			}
			if (fromPool.Count > 0)
			{
				MessageManager.m_instancesBuffer.Clear();
				MessageManager.m_instancesBuffer.AddInt(fromPool.Count);
				foreach (ArchetypeInstance archetypeInstance in fromPool)
				{
					archetypeInstance.PackData_BinaryIDs(MessageManager.m_instancesBuffer);
				}
				if (MessageManager.m_instancesBuffer.Length > 4096)
				{
					return "toolong";
				}
				if (MessageManager.m_instancesBuffer.Length > MessageManager.m_tempBuffer.Length)
				{
					MessageManager.m_tempBuffer = new byte[MessageManager.m_instancesBuffer.Length];
				}
				int val = MessageManager.m_instancesBuffer.ToArray(MessageManager.m_tempBuffer);
				result = Convert.ToBase64String(MessageManager.m_tempBuffer, 0, Math.Min(val, MessageManager.m_tempBuffer.Length));
			}
			StaticListPool<ArchetypeInstance>.ReturnToPool(fromPool);
			return result;
		}

		// Token: 0x06004D0E RID: 19726 RVA: 0x001BEE10 File Offset: 0x001BD010
		public static int ExtractInstanceAttachments(string attachmentsStr)
		{
			MessageManager.m_instancesBuffer.Clear();
			byte[] array = Convert.FromBase64String(attachmentsStr);
			MessageManager.m_instancesBuffer.FromArray(array, array.Length);
			int num = MessageManager.m_instancesBuffer.ReadInt();
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					ArchetypeInstance instance = StaticPool<ArchetypeInstance>.GetFromPool();
					instance.ReadData_BinaryIDs(MessageManager.m_instancesBuffer);
					MessageManager.AddLinkedInstance(instance, false);
					MessageManager.AttachmentsCache.Remove(instance.InstanceId);
					MessageManager.AttachmentInstanceIds.RemoveAll((string x) => x == instance.InstanceId);
				}
			}
			return num;
		}

		// Token: 0x06004D0F RID: 19727 RVA: 0x001BEEB8 File Offset: 0x001BD0B8
		public static void CacheAttachments(string msg, string attachments)
		{
			MatchCollection matchCollection = ChatMessage.InstanceLinkPattern.Matches(msg);
			List<string> fromPool = StaticListPool<string>.GetFromPool();
			foreach (object obj in matchCollection)
			{
				foreach (object obj2 in ((Match)obj).Groups)
				{
					Group group = (Group)obj2;
					if (group.Length == 36)
					{
						fromPool.Add(group.Value);
					}
				}
			}
			foreach (string text in fromPool)
			{
				if (!MessageManager.LinkedInstances.ContainsKey(text))
				{
					MessageManager.AddAttachmentInstanceId(text, attachments);
				}
			}
			StaticListPool<string>.ReturnToPool(fromPool);
		}

		// Token: 0x040046C5 RID: 18117
		private const int kMaxExcessThreshold = 20;

		// Token: 0x040046C6 RID: 18118
		private const int kMaxInstances = 120;

		// Token: 0x040046C7 RID: 18119
		private const int kMaxAttachmentsLength = 4096;

		// Token: 0x040046C8 RID: 18120
		private static readonly Dictionary<string, ArchetypeInstance> LinkedInstances = new Dictionary<string, ArchetypeInstance>();

		// Token: 0x040046C9 RID: 18121
		private static readonly List<string> LinkedInstanceIds = new List<string>(120);

		// Token: 0x040046CA RID: 18122
		private static readonly Dictionary<string, string> AttachmentsCache = new Dictionary<string, string>();

		// Token: 0x040046CB RID: 18123
		private static readonly List<string> AttachmentInstanceIds = new List<string>(120);

		// Token: 0x040046CC RID: 18124
		private static ChatMessageQueue m_chatQueue = null;

		// Token: 0x040046CD RID: 18125
		private static ChatMessageQueue m_combatQueue = null;

		// Token: 0x040046CE RID: 18126
		private static BitBuffer m_instancesBuffer = new BitBuffer(375);

		// Token: 0x040046CF RID: 18127
		private static byte[] m_tempBuffer = new byte[1500];
	}
}
