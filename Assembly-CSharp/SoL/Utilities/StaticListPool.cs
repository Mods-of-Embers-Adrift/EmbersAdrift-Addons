using System;
using System.Collections.Generic;

namespace SoL.Utilities
{
	// Token: 0x020002E8 RID: 744
	public static class StaticListPool<T>
	{
		// Token: 0x06001548 RID: 5448 RVA: 0x00050ED1 File Offset: 0x0004F0D1
		public static List<T> GetFromPool()
		{
			if (StaticListPool<T>.m_pool.Count <= 0)
			{
				return new List<T>(64);
			}
			return StaticListPool<T>.m_pool.Pop();
		}

		// Token: 0x06001549 RID: 5449 RVA: 0x00050EF2 File Offset: 0x0004F0F2
		public static void ReturnToPool(List<T> item)
		{
			if (item != null)
			{
				item.Clear();
				if (StaticListPool<T>.m_pool.Count < 128)
				{
					StaticListPool<T>.m_pool.Push(item);
				}
			}
		}

		// Token: 0x04001D75 RID: 7541
		private const int kMaxPoolSize = 128;

		// Token: 0x04001D76 RID: 7542
		private static readonly Stack<List<T>> m_pool = new Stack<List<T>>(128);
	}
}
