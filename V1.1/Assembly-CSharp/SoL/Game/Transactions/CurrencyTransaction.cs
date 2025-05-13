using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Transactions
{
	// Token: 0x0200063D RID: 1597
	public struct CurrencyTransaction : INetworkSerializable
	{
		// Token: 0x060031E4 RID: 12772 RVA: 0x0015E21C File Offset: 0x0015C41C
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.Context);
			buffer.AddBool(this.Add);
			buffer.AddULong(this.Amount);
			buffer.AddString(this.TargetContainer);
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

		// Token: 0x060031E5 RID: 12773 RVA: 0x0015E28C File Offset: 0x0015C48C
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Context = buffer.ReadEnum<CurrencyContext>();
			this.Add = buffer.ReadBool();
			this.Amount = buffer.ReadULong();
			this.TargetContainer = buffer.ReadString();
			if (buffer.ReadBool())
			{
				this.Message = buffer.ReadString();
			}
			return buffer;
		}

		// Token: 0x04003086 RID: 12422
		public bool Add;

		// Token: 0x04003087 RID: 12423
		public ulong Amount;

		// Token: 0x04003088 RID: 12424
		public string TargetContainer;

		// Token: 0x04003089 RID: 12425
		public string Message;

		// Token: 0x0400308A RID: 12426
		public CurrencyContext Context;
	}
}
