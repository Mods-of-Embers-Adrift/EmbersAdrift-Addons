using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Transactions
{
	// Token: 0x02000643 RID: 1603
	public struct PurchaseContainerExpansionTransaction : INetworkSerializable
	{
		// Token: 0x060031F0 RID: 12784 RVA: 0x0006284E File Offset: 0x00060A4E
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.Op);
			buffer.AddString(this.ContainerId);
			buffer.AddNullableUlong(this.CurrencyToRemoveFromContainer);
			buffer.AddNullableUlong(this.CurrencyToRemoveFromInventory);
			return buffer;
		}

		// Token: 0x060031F1 RID: 12785 RVA: 0x00062885 File Offset: 0x00060A85
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Op = buffer.ReadEnum<OpCodes>();
			this.ContainerId = buffer.ReadString();
			this.CurrencyToRemoveFromContainer = buffer.ReadNullableUlong();
			this.CurrencyToRemoveFromInventory = buffer.ReadNullableUlong();
			return buffer;
		}

		// Token: 0x04003099 RID: 12441
		public OpCodes Op;

		// Token: 0x0400309A RID: 12442
		public string ContainerId;

		// Token: 0x0400309B RID: 12443
		public ulong? CurrencyToRemoveFromContainer;

		// Token: 0x0400309C RID: 12444
		public ulong? CurrencyToRemoveFromInventory;
	}
}
