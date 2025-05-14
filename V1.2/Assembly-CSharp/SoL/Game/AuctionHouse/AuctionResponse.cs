using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.AuctionHouse
{
	// Token: 0x02000D30 RID: 3376
	public struct AuctionResponse : INetworkSerializable
	{
		// Token: 0x06006580 RID: 25984 RVA: 0x0008453B File Offset: 0x0008273B
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.OpCode);
			buffer.AddBool(this.DestroyContents);
			buffer.AddNullableString(this.Message);
			return buffer;
		}

		// Token: 0x06006581 RID: 25985 RVA: 0x00084565 File Offset: 0x00082765
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.OpCode = buffer.ReadEnum<OpCodes>();
			this.DestroyContents = buffer.ReadBool();
			this.Message = buffer.ReadNullableString();
			return buffer;
		}

		// Token: 0x04005844 RID: 22596
		public OpCodes OpCode;

		// Token: 0x04005845 RID: 22597
		public bool DestroyContents;

		// Token: 0x04005846 RID: 22598
		public string Message;
	}
}
