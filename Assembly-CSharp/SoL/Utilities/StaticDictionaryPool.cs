using System;
using System.Collections.Generic;

namespace SoL.Utilities
{
	// Token: 0x020002EA RID: 746
	public static class StaticDictionaryPool<TKey, TValue>
	{
		// Token: 0x0600154E RID: 5454 RVA: 0x00050F5C File Offset: 0x0004F15C
		public static Dictionary<TKey, TValue> GetFromPool()
		{
			if (StaticDictionaryPool<TKey, TValue>.m_pool.Count <= 0)
			{
				return new Dictionary<TKey, TValue>(64);
			}
			return StaticDictionaryPool<TKey, TValue>.m_pool.Pop();
		}

		// Token: 0x0600154F RID: 5455 RVA: 0x00050F7D File Offset: 0x0004F17D
		public static void ReturnToPool(Dictionary<TKey, TValue> item)
		{
			if (item != null)
			{
				item.Clear();
				if (StaticDictionaryPool<TKey, TValue>.m_pool.Count < 128)
				{
					StaticDictionaryPool<TKey, TValue>.m_pool.Push(item);
				}
			}
		}

		// Token: 0x04001D79 RID: 7545
		private const int kMaxPoolSize = 128;

		// Token: 0x04001D7A RID: 7546
		private static readonly Stack<Dictionary<TKey, TValue>> m_pool = new Stack<Dictionary<TKey, TValue>>(128);
	}
}
