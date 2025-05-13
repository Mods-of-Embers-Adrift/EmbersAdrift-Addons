using System;
using NetStack.Serialization;
using SoL.Networking;
using SoL.Networking.Managers;
using SoL.Networking.Objects;

namespace SoL.Game.Quests
{
	// Token: 0x02000786 RID: 1926
	public struct ObjectiveIterationCache : INetworkSerializable
	{
		// Token: 0x060038D2 RID: 14546 RVA: 0x00170904 File Offset: 0x0016EB04
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.QuestId);
			int[] objectiveHashes = this.ObjectiveHashes;
			buffer.AddInt((objectiveHashes != null) ? objectiveHashes.Length : 0);
			if (this.ObjectiveHashes != null && this.ObjectiveHashes.Length != 0)
			{
				for (int i = 0; i < this.ObjectiveHashes.Length; i++)
				{
					buffer.AddInt(this.ObjectiveHashes[i]);
				}
			}
			buffer.AddUniqueId(this.RewardChoiceId);
			buffer.AddByte(this.IterationsRequested);
			buffer.AddUniqueId(this.WorldId);
			buffer.AddUInt(this.NpcEntity ? this.NpcEntity.NetworkId.Value : 0U);
			buffer.AddBool(this.StartQuestIfNotPresent);
			return buffer;
		}

		// Token: 0x060038D3 RID: 14547 RVA: 0x001709C8 File Offset: 0x0016EBC8
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.QuestId = buffer.ReadUniqueId();
			int num = buffer.ReadInt();
			if (num > 0)
			{
				this.ObjectiveHashes = new int[num];
				for (int i = 0; i < num; i++)
				{
					this.ObjectiveHashes[i] = buffer.ReadInt();
				}
			}
			this.RewardChoiceId = buffer.ReadUniqueId();
			this.IterationsRequested = buffer.ReadByte();
			this.WorldId = buffer.ReadUniqueId();
			this.NpcEntity = NetworkManager.EntityManager.GetNetEntityForId(buffer.ReadUInt());
			this.StartQuestIfNotPresent = buffer.ReadBool();
			return buffer;
		}

		// Token: 0x060038D4 RID: 14548 RVA: 0x000668A6 File Offset: 0x00064AA6
		public static int[] SharedSingleItemArray(int hash)
		{
			ObjectiveIterationCache.m_sharedSingleItemArray[0] = hash;
			return ObjectiveIterationCache.m_sharedSingleItemArray;
		}

		// Token: 0x040037B4 RID: 14260
		private static readonly int[] m_sharedSingleItemArray = new int[1];

		// Token: 0x040037B5 RID: 14261
		public UniqueId QuestId;

		// Token: 0x040037B6 RID: 14262
		public int[] ObjectiveHashes;

		// Token: 0x040037B7 RID: 14263
		public UniqueId RewardChoiceId;

		// Token: 0x040037B8 RID: 14264
		public bool StartQuestIfNotPresent;

		// Token: 0x040037B9 RID: 14265
		public byte IterationsRequested;

		// Token: 0x040037BA RID: 14266
		public UniqueId WorldId;

		// Token: 0x040037BB RID: 14267
		public NetworkEntity NpcEntity;
	}
}
