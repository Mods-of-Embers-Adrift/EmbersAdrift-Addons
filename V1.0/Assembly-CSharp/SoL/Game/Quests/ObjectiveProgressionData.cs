using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Quests
{
	// Token: 0x02000787 RID: 1927
	[Serializable]
	public class ObjectiveProgressionData : INetworkSerializable
	{
		// Token: 0x060038D6 RID: 14550 RVA: 0x000668C2 File Offset: 0x00064AC2
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.ObjectiveId);
			buffer.AddInt(this.IterationsCompleted);
			return buffer;
		}

		// Token: 0x060038D7 RID: 14551 RVA: 0x000668DF File Offset: 0x00064ADF
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.ObjectiveId = buffer.ReadUniqueId();
			this.IterationsCompleted = buffer.ReadInt();
			return buffer;
		}

		// Token: 0x040037BC RID: 14268
		public UniqueId ObjectiveId;

		// Token: 0x040037BD RID: 14269
		public int IterationsCompleted;
	}
}
