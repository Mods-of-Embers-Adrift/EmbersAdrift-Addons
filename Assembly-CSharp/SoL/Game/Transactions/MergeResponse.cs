using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Transactions
{
	// Token: 0x0200062E RID: 1582
	public struct MergeResponse : INetworkSerializable
	{
		// Token: 0x060031C0 RID: 12736 RVA: 0x000624E1 File Offset: 0x000606E1
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.Op);
			buffer.AddUniqueId(this.TransactionId);
			buffer.AddInt(this.NewTargetCount);
			return buffer;
		}

		// Token: 0x060031C1 RID: 12737 RVA: 0x0006250B File Offset: 0x0006070B
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Op = buffer.ReadEnum<OpCodes>();
			this.TransactionId = buffer.ReadUniqueId();
			this.NewTargetCount = buffer.ReadInt();
			return buffer;
		}

		// Token: 0x0400304E RID: 12366
		public OpCodes Op;

		// Token: 0x0400304F RID: 12367
		public UniqueId TransactionId;

		// Token: 0x04003050 RID: 12368
		public int NewTargetCount;
	}
}
