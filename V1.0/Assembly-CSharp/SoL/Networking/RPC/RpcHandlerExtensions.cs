using System;

namespace SoL.Networking.RPC
{
	// Token: 0x02000478 RID: 1144
	public static class RpcHandlerExtensions
	{
		// Token: 0x06001FF6 RID: 8182 RVA: 0x00051849 File Offset: 0x0004FA49
		public static bool TryGetAsType<T>(this RpcHandler handler, out T asType) where T : class
		{
			asType = (handler as T);
			return asType != null;
		}
	}
}
