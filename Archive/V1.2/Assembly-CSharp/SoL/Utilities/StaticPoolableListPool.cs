using System;
using System.Collections.Generic;

namespace SoL.Utilities
{
	// Token: 0x020002E9 RID: 745
	public static class StaticPoolableListPool<T> where T : IPoolable, new()
	{
		// Token: 0x0600154B RID: 5451 RVA: 0x00050F2A File Offset: 0x0004F12A
		public static List<T> GetFromPool()
		{
			if (StaticPoolableListPool<T>.m_pool.Count <= 0)
			{
				return new List<T>(10);
			}
			return StaticPoolableListPool<T>.m_pool.Pop();
		}

		// Token: 0x0600154C RID: 5452 RVA: 0x000FC540 File Offset: 0x000FA740
		public static void ReturnToPool(List<T> item)
		{
			if (item != null)
			{
				foreach (T item2 in item)
				{
					StaticPool<T>.ReturnToPool(item2);
				}
				item.Clear();
				if (StaticPoolableListPool<T>.m_pool.Count < 128)
				{
					StaticPoolableListPool<T>.m_pool.Push(item);
				}
			}
		}

		// Token: 0x04001D77 RID: 7543
		private const int kMaxPoolSize = 128;

		// Token: 0x04001D78 RID: 7544
		private static readonly Stack<List<T>> m_pool = new Stack<List<T>>(128);
	}
}
