using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AA9 RID: 2729
	public struct AugmentUpdateInfo : INetworkSerializable
	{
		// Token: 0x06005457 RID: 21591 RVA: 0x0007864F File Offset: 0x0007684F
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddBool(this.Expired);
			if (!this.Expired)
			{
				buffer.AddInt(this.Count);
				buffer.AddByte(this.StackCount);
			}
			return buffer;
		}

		// Token: 0x06005458 RID: 21592 RVA: 0x00078681 File Offset: 0x00076881
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Expired = buffer.ReadBool();
			if (!this.Expired)
			{
				this.Count = buffer.ReadInt();
				this.StackCount = buffer.ReadByte();
			}
			return buffer;
		}

		// Token: 0x04004B0E RID: 19214
		public bool Expired;

		// Token: 0x04004B0F RID: 19215
		public int Count;

		// Token: 0x04004B10 RID: 19216
		public byte StackCount;
	}
}
