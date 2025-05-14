using System;
using NetStack.Serialization;
using SoL.Game.NPCs;
using SoL.Game.Objects;
using SoL.Networking;
using SoL.Networking.Managers;
using SoL.Networking.Objects;

namespace SoL.Game.Quests
{
	// Token: 0x02000785 RID: 1925
	public class NpcLearningCache : INetworkSerializable
	{
		// Token: 0x060038CF RID: 14543 RVA: 0x00170874 File Offset: 0x0016EA74
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.NpcProfile.Id);
			buffer.AddInt(this.KnowledgeIndex);
			NetworkEntity npcEntity = this.NpcEntity;
			buffer.AddUInt((npcEntity != null) ? npcEntity.NetworkId.Value : 0U);
			buffer.AddUniqueId(this.WorldId);
			return buffer;
		}

		// Token: 0x060038D0 RID: 14544 RVA: 0x001708D0 File Offset: 0x0016EAD0
		public BitBuffer ReadData(BitBuffer buffer)
		{
			InternalGameDatabase.Archetypes.TryGetAsType<NpcProfile>(buffer.ReadUniqueId(), out this.NpcProfile);
			this.KnowledgeIndex = buffer.ReadInt();
			this.NpcEntity = NetworkManager.EntityManager.GetNetEntityForId(buffer.ReadUInt());
			this.WorldId = buffer.ReadUniqueId();
			return buffer;
		}

		// Token: 0x040037B0 RID: 14256
		public NpcProfile NpcProfile;

		// Token: 0x040037B1 RID: 14257
		public int KnowledgeIndex;

		// Token: 0x040037B2 RID: 14258
		public NetworkEntity NpcEntity;

		// Token: 0x040037B3 RID: 14259
		public UniqueId WorldId;
	}
}
