using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A38 RID: 2616
	[Serializable]
	public class EmberStoneInstanceData : INetworkSerializable
	{
		// Token: 0x060050F3 RID: 20723 RVA: 0x000761B2 File Offset: 0x000743B2
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.StoneId);
			buffer.AddInt(this.Count);
			buffer.AddInt(this.TravelCount);
			return buffer;
		}

		// Token: 0x060050F4 RID: 20724 RVA: 0x000761DC File Offset: 0x000743DC
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.StoneId = buffer.ReadUniqueId();
			this.Count = buffer.ReadInt();
			this.TravelCount = buffer.ReadInt();
			return buffer;
		}

		// Token: 0x04004875 RID: 18549
		public UniqueId StoneId;

		// Token: 0x04004876 RID: 18550
		public int Count;

		// Token: 0x04004877 RID: 18551
		public int TravelCount;
	}
}
