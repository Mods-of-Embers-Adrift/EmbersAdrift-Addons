using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Transactions
{
	// Token: 0x0200063E RID: 1598
	public struct CurrencyAdjustment : INetworkSerializable
	{
		// Token: 0x060031E6 RID: 12774 RVA: 0x0006272E File Offset: 0x0006092E
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddBool(this.Add);
			buffer.AddULong(this.Amount);
			buffer.AddString(this.TargetContainer);
			return buffer;
		}

		// Token: 0x060031E7 RID: 12775 RVA: 0x00062758 File Offset: 0x00060958
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Add = buffer.ReadBool();
			this.Amount = buffer.ReadULong();
			this.TargetContainer = buffer.ReadString();
			return buffer;
		}

		// Token: 0x0400308B RID: 12427
		public bool Add;

		// Token: 0x0400308C RID: 12428
		public ulong Amount;

		// Token: 0x0400308D RID: 12429
		public string TargetContainer;
	}
}
