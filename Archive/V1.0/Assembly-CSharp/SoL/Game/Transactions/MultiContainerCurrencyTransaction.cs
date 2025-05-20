using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Transactions
{
	// Token: 0x0200063F RID: 1599
	public struct MultiContainerCurrencyTransaction : INetworkSerializable
	{
		// Token: 0x060031E8 RID: 12776 RVA: 0x0015E2E0 File Offset: 0x0015C4E0
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.Context);
			CurrencyAdjustment[] adjustments = this.Adjustments;
			buffer.AddInt((adjustments != null) ? adjustments.Length : 0);
			foreach (CurrencyAdjustment currencyAdjustment in this.Adjustments)
			{
				currencyAdjustment.PackData(buffer);
			}
			if (string.IsNullOrEmpty(this.Message))
			{
				buffer.AddBool(false);
			}
			else
			{
				buffer.AddBool(true);
				buffer.AddString(this.Message);
			}
			return buffer;
		}

		// Token: 0x060031E9 RID: 12777 RVA: 0x0015E364 File Offset: 0x0015C564
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Context = buffer.ReadEnum<CurrencyContext>();
			int num = buffer.ReadInt();
			if (num > 0)
			{
				this.Adjustments = new CurrencyAdjustment[num];
				for (int i = 0; i < num; i++)
				{
					this.Adjustments[i].ReadData(buffer);
				}
			}
			if (buffer.ReadBool())
			{
				this.Message = buffer.ReadString();
			}
			return buffer;
		}

		// Token: 0x0400308E RID: 12430
		public CurrencyAdjustment[] Adjustments;

		// Token: 0x0400308F RID: 12431
		public string Message;

		// Token: 0x04003090 RID: 12432
		public CurrencyContext Context;
	}
}
