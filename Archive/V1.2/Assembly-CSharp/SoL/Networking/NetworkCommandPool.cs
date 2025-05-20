using System;
using NetStack.Threading;

namespace SoL.Networking
{
	// Token: 0x020003C5 RID: 965
	public static class NetworkCommandPool
	{
		// Token: 0x17000628 RID: 1576
		// (get) Token: 0x060019F6 RID: 6646 RVA: 0x00054534 File Offset: 0x00052734
		private static ConcurrentPool<NetworkCommand> Pool
		{
			get
			{
				if (NetworkCommandPool.m_pool == null)
				{
					NetworkCommandPool.m_pool = new ConcurrentPool<NetworkCommand>(8, () => new NetworkCommand());
				}
				return NetworkCommandPool.m_pool;
			}
		}

		// Token: 0x060019F7 RID: 6647 RVA: 0x0005456C File Offset: 0x0005276C
		public static NetworkCommand GetNetworkCommand()
		{
			return NetworkCommandPool.Pool.Acquire();
		}

		// Token: 0x060019F8 RID: 6648 RVA: 0x00054578 File Offset: 0x00052778
		public static void ReturnToPool(this NetworkCommand command)
		{
			if (command != null)
			{
				command.Reset();
				NetworkCommandPool.Pool.Release(command);
			}
		}

		// Token: 0x0400211C RID: 8476
		private static ConcurrentPool<NetworkCommand> m_pool;
	}
}
