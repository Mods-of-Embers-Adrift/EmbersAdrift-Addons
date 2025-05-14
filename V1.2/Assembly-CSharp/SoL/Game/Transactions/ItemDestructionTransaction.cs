using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Transactions
{
	// Token: 0x02000638 RID: 1592
	public struct ItemDestructionTransaction : INetworkSerializable
	{
		// Token: 0x060031DA RID: 12762 RVA: 0x000626C4 File Offset: 0x000608C4
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.Op);
			buffer.AddUniqueId(this.InstanceId);
			buffer.AddString(this.SourceContainer);
			buffer.AddEnum(this.Context);
			return buffer;
		}

		// Token: 0x060031DB RID: 12763 RVA: 0x000626FB File Offset: 0x000608FB
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Op = buffer.ReadEnum<OpCodes>();
			this.InstanceId = buffer.ReadUniqueId();
			this.SourceContainer = buffer.ReadString();
			this.Context = buffer.ReadEnum<ItemDestructionContext>();
			return buffer;
		}

		// Token: 0x04003075 RID: 12405
		public OpCodes Op;

		// Token: 0x04003076 RID: 12406
		public UniqueId InstanceId;

		// Token: 0x04003077 RID: 12407
		public string SourceContainer;

		// Token: 0x04003078 RID: 12408
		public ItemDestructionContext Context;
	}
}
