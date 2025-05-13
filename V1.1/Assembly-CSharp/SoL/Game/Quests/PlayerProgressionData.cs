using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using NetStack.Serialization;
using Newtonsoft.Json;
using SoL.Networking;

namespace SoL.Game.Quests
{
	// Token: 0x02000788 RID: 1928
	[Serializable]
	public class PlayerProgressionData : INetworkSerializable
	{
		// Token: 0x060038D9 RID: 14553 RVA: 0x00170A5C File Offset: 0x0016EC5C
		public BitBuffer PackData(BitBuffer buffer)
		{
			Dictionary<UniqueId, QuestProgressionData> quests = this.Quests;
			buffer.AddInt((quests != null) ? quests.Count : 0);
			if (this.Quests != null)
			{
				foreach (KeyValuePair<UniqueId, QuestProgressionData> keyValuePair in this.Quests)
				{
					buffer.AddUniqueId(keyValuePair.Key);
					keyValuePair.Value.PackData(buffer);
				}
			}
			Dictionary<UniqueId, BBTaskProgressionData> bbtasks = this.BBTasks;
			buffer.AddInt((bbtasks != null) ? bbtasks.Count : 0);
			if (this.BBTasks != null)
			{
				foreach (KeyValuePair<UniqueId, BBTaskProgressionData> keyValuePair2 in this.BBTasks)
				{
					buffer.AddUniqueId(keyValuePair2.Key);
					keyValuePair2.Value.PackData(buffer);
				}
			}
			Dictionary<UniqueId, BitArray> npcKnowledge = this.NpcKnowledge;
			buffer.AddInt((npcKnowledge != null) ? npcKnowledge.Count : 0);
			if (this.NpcKnowledge != null)
			{
				foreach (KeyValuePair<UniqueId, BitArray> keyValuePair3 in this.NpcKnowledge)
				{
					buffer.AddUniqueId(keyValuePair3.Key);
					BitArray value = keyValuePair3.Value;
					buffer.AddInt((value != null) ? value.Count : 0);
					foreach (object obj in keyValuePair3.Value)
					{
						bool value2 = (bool)obj;
						buffer.AddBool(value2);
					}
				}
			}
			buffer.AddInt(this.BBTasks_AdventuringCompletionCount);
			buffer.AddInt(this.BBTasks_CraftingCompletionCount);
			buffer.AddInt(this.BBTasks_GatheringCompletionCount);
			return buffer;
		}

		// Token: 0x060038DA RID: 14554 RVA: 0x00170C64 File Offset: 0x0016EE64
		public BitBuffer ReadData(BitBuffer buffer)
		{
			int num = buffer.ReadInt();
			if (num > 0)
			{
				this.Quests = new Dictionary<UniqueId, QuestProgressionData>(num);
				for (int i = 0; i < num; i++)
				{
					UniqueId key = buffer.ReadUniqueId();
					QuestProgressionData questProgressionData = new QuestProgressionData();
					questProgressionData.ReadData(buffer);
					this.Quests.Add(key, questProgressionData);
				}
			}
			int num2 = buffer.ReadInt();
			if (num2 > 0)
			{
				this.BBTasks = new Dictionary<UniqueId, BBTaskProgressionData>(num2);
				for (int j = 0; j < num2; j++)
				{
					UniqueId key2 = buffer.ReadUniqueId();
					BBTaskProgressionData bbtaskProgressionData = new BBTaskProgressionData();
					bbtaskProgressionData.ReadData(buffer);
					this.BBTasks.Add(key2, bbtaskProgressionData);
				}
			}
			int num3 = buffer.ReadInt();
			if (num3 > 0)
			{
				this.NpcKnowledge = new Dictionary<UniqueId, BitArray>(num3);
				for (int k = 0; k < num3; k++)
				{
					UniqueId key3 = buffer.ReadUniqueId();
					int num4 = buffer.ReadInt();
					if (num4 > 0)
					{
						BitArray bitArray = new BitArray(num4);
						for (int l = 0; l < num4; l++)
						{
							bitArray[l] = buffer.ReadBool();
						}
						this.NpcKnowledge.Add(key3, bitArray);
					}
				}
			}
			this.BBTasks_AdventuringCompletionCount = buffer.ReadInt();
			this.BBTasks_CraftingCompletionCount = buffer.ReadInt();
			this.BBTasks_GatheringCompletionCount = buffer.ReadInt();
			return buffer;
		}

		// Token: 0x040037BE RID: 14270
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Dictionary<UniqueId, QuestProgressionData> Quests;

		// Token: 0x040037BF RID: 14271
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Dictionary<UniqueId, BBTaskProgressionData> BBTasks;

		// Token: 0x040037C0 RID: 14272
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Dictionary<UniqueId, BitArray> NpcKnowledge;

		// Token: 0x040037C1 RID: 14273
		public int BBTasks_AdventuringCompletionCount;

		// Token: 0x040037C2 RID: 14274
		public int BBTasks_CraftingCompletionCount;

		// Token: 0x040037C3 RID: 14275
		public int BBTasks_GatheringCompletionCount;
	}
}
