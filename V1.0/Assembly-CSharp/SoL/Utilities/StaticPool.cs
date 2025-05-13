using System;
using System.Collections.Generic;

namespace SoL.Utilities
{
	// Token: 0x020002E7 RID: 743
	public static class StaticPool<T> where T : IPoolable, new()
	{
		// Token: 0x06001545 RID: 5445 RVA: 0x000FC480 File Offset: 0x000FA680
		public static T GetFromPool()
		{
			if (StaticPool<T>.m_pool.Count <= 0)
			{
				return Activator.CreateInstance<T>();
			}
			T t = StaticPool<T>.m_pool.Pop();
			if (t != null)
			{
				t.InPool = false;
			}
			return t;
		}

		// Token: 0x06001546 RID: 5446 RVA: 0x000FC4C4 File Offset: 0x000FA6C4
		public static void ReturnToPool(T item)
		{
			if (item == null || item.InPool)
			{
				return;
			}
			item.Reset();
			if (StaticPool<T>.m_pool.Count < 128)
			{
				item.InPool = true;
				StaticPool<T>.m_pool.Push(item);
			}
		}

		// Token: 0x04001D73 RID: 7539
		private const int kMaxPoolSize = 128;

		// Token: 0x04001D74 RID: 7540
		private static readonly Stack<T> m_pool = new Stack<T>(128);
	}
}
