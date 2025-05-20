using System;
using ENet;
using NX.Buffers;

namespace SoL.Networking
{
	// Token: 0x020003D2 RID: 978
	public static class PeerArrayPool
	{
		// Token: 0x1700062F RID: 1583
		// (get) Token: 0x06001A53 RID: 6739 RVA: 0x000547DB File Offset: 0x000529DB
		private static BufferPool<Peer> Pool
		{
			get
			{
				if (PeerArrayPool.m_pool == null)
				{
					PeerArrayPool.m_pool = BufferPool<Peer>.Create(1024, 50);
				}
				return PeerArrayPool.m_pool;
			}
		}

		// Token: 0x06001A54 RID: 6740 RVA: 0x000547FA File Offset: 0x000529FA
		public static Peer[] GetArray(int size)
		{
			return PeerArrayPool.Pool.Rent(size);
		}

		// Token: 0x06001A55 RID: 6741 RVA: 0x001085B0 File Offset: 0x001067B0
		public static void ReturnToPool(this Peer[] data)
		{
			for (int i = 0; i < data.Length; i++)
			{
				data[i] = default(Peer);
			}
			PeerArrayPool.Pool.Return(data, false);
		}

		// Token: 0x04002141 RID: 8513
		private static BufferPool<Peer> m_pool;
	}
}
