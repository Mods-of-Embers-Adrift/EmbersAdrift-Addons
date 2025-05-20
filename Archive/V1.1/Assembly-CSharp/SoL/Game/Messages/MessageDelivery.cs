using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Messages
{
	// Token: 0x020009E0 RID: 2528
	public struct MessageDelivery : INetworkSerializable
	{
		// Token: 0x06004D00 RID: 19712 RVA: 0x000740FC File Offset: 0x000722FC
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.Type);
			buffer.AddString(this.Text);
			buffer.AddEnum(this.EventType);
			return buffer;
		}

		// Token: 0x06004D01 RID: 19713 RVA: 0x00074126 File Offset: 0x00072326
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Type = buffer.ReadEnum<MessageType>();
			this.Text = buffer.ReadString();
			this.EventType = buffer.ReadEnum<MessageEventType>();
			return buffer;
		}

		// Token: 0x040046C0 RID: 18112
		public string Text;

		// Token: 0x040046C1 RID: 18113
		public MessageType Type;

		// Token: 0x040046C2 RID: 18114
		public MessageEventType EventType;
	}
}
