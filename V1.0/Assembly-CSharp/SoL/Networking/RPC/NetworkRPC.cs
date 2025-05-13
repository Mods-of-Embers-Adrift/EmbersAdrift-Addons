using System;

namespace SoL.Networking.RPC
{
	// Token: 0x02000474 RID: 1140
	public class NetworkRPC : Attribute
	{
		// Token: 0x06001FE7 RID: 8167 RVA: 0x00057588 File Offset: 0x00055788
		public NetworkRPC(RpcType rpcType)
		{
			this.RpcType = rpcType;
		}

		// Token: 0x04002558 RID: 9560
		public readonly RpcType RpcType;
	}
}
