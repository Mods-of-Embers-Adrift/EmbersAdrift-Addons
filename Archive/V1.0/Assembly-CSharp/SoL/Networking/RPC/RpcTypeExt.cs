using System;

namespace SoL.Networking.RPC
{
	// Token: 0x0200047A RID: 1146
	public static class RpcTypeExt
	{
		// Token: 0x06001FF7 RID: 8183 RVA: 0x00057630 File Offset: 0x00055830
		public static CommandType GetCommandTypeForRpcType(this RpcType rpcType)
		{
			if (rpcType == RpcType.ServerBroadcast)
			{
				return CommandType.BroadcastAll;
			}
			return CommandType.Send;
		}

		// Token: 0x06001FF8 RID: 8184 RVA: 0x00057639 File Offset: 0x00055839
		public static NetworkChannel GetChannelForRpcType(this RpcType rpcType)
		{
			if (rpcType - RpcType.ServerToClient <= 1)
			{
				return NetworkChannel.Rpc_Server;
			}
			return NetworkChannel.Rpc_Client;
		}
	}
}
