using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Loot
{
	// Token: 0x02000B0B RID: 2827
	public struct LootRollItemResponse : INetworkSerializable
	{
		// Token: 0x06005731 RID: 22321 RVA: 0x0007A151 File Offset: 0x00078351
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.Id);
			buffer.AddEnum(this.Choice);
			return buffer;
		}

		// Token: 0x06005732 RID: 22322 RVA: 0x0007A16E File Offset: 0x0007836E
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Id = buffer.ReadUniqueId();
			this.Choice = buffer.ReadEnum<LootRollChoice>();
			return buffer;
		}

		// Token: 0x04004CF2 RID: 19698
		public UniqueId Id;

		// Token: 0x04004CF3 RID: 19699
		public LootRollChoice Choice;
	}
}
