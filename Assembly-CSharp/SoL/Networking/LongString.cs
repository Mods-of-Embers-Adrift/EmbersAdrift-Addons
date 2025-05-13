using System;
using NetStack.Serialization;

namespace SoL.Networking
{
	// Token: 0x020003BE RID: 958
	public struct LongString : INetworkSerializable
	{
		// Token: 0x060019EC RID: 6636 RVA: 0x00054515 File Offset: 0x00052715
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddASCII(this.Value);
			return buffer;
		}

		// Token: 0x060019ED RID: 6637 RVA: 0x00054525 File Offset: 0x00052725
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Value = buffer.ReadASCII();
			return buffer;
		}

		// Token: 0x040020EF RID: 8431
		public string Value;
	}
}
