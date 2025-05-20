using System;
using NetStack.Serialization;

namespace SoL.Networking
{
	// Token: 0x020003CE RID: 974
	public interface INetworkSerializable
	{
		// Token: 0x06001A40 RID: 6720
		BitBuffer PackData(BitBuffer buffer);

		// Token: 0x06001A41 RID: 6721
		BitBuffer ReadData(BitBuffer buffer);
	}
}
