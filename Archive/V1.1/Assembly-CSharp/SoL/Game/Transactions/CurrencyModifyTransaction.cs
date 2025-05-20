using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Transactions
{
	// Token: 0x02000640 RID: 1600
	public struct CurrencyModifyTransaction : INetworkSerializable
	{
		// Token: 0x060031EA RID: 12778 RVA: 0x0006277F File Offset: 0x0006097F
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.Context);
			buffer.AddULong(this.Amount);
			buffer.AddString(this.ContainerId);
			return buffer;
		}

		// Token: 0x060031EB RID: 12779 RVA: 0x000627A9 File Offset: 0x000609A9
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Context = buffer.ReadEnum<CurrencyContext>();
			this.Amount = buffer.ReadULong();
			this.ContainerId = buffer.ReadString();
			return buffer;
		}

		// Token: 0x04003091 RID: 12433
		public ulong Amount;

		// Token: 0x04003092 RID: 12434
		public string ContainerId;

		// Token: 0x04003093 RID: 12435
		public CurrencyContext Context;
	}
}
