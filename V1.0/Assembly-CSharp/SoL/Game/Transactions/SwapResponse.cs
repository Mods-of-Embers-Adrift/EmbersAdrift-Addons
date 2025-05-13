using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Transactions
{
	// Token: 0x02000634 RID: 1588
	public struct SwapResponse : INetworkSerializable
	{
		// Token: 0x060031CF RID: 12751 RVA: 0x000625F2 File Offset: 0x000607F2
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.TransactionId);
			buffer.AddEnum(this.Op);
			return buffer;
		}

		// Token: 0x060031D0 RID: 12752 RVA: 0x0006260F File Offset: 0x0006080F
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.TransactionId = buffer.ReadUniqueId();
			this.Op = buffer.ReadEnum<OpCodes>();
			return buffer;
		}

		// Token: 0x04003069 RID: 12393
		public OpCodes Op;

		// Token: 0x0400306A RID: 12394
		public UniqueId TransactionId;
	}
}
