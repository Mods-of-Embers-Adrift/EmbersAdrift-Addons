using System;
using NX.Buffers;

namespace SoL.Networking
{
	// Token: 0x020003BD RID: 957
	public static class ByteArrayPool
	{
		// Token: 0x17000627 RID: 1575
		// (get) Token: 0x060019E9 RID: 6633 RVA: 0x000544DB File Offset: 0x000526DB
		private static BufferPool<byte> Pool
		{
			get
			{
				if (ByteArrayPool.m_pool == null)
				{
					ByteArrayPool.m_pool = BufferPool<byte>.Create(1024, 50);
				}
				return ByteArrayPool.m_pool;
			}
		}

		// Token: 0x060019EA RID: 6634 RVA: 0x000544FA File Offset: 0x000526FA
		public static byte[] GetByteArray(int size)
		{
			return ByteArrayPool.Pool.Rent(size);
		}

		// Token: 0x060019EB RID: 6635 RVA: 0x00054507 File Offset: 0x00052707
		public static void ReturnToPool(this byte[] data)
		{
			ByteArrayPool.Pool.Return(data, false);
		}

		// Token: 0x040020EE RID: 8430
		private static BufferPool<byte> m_pool;
	}
}
