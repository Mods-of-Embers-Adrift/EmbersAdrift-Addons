using System;
using System.Collections.Generic;

namespace SoL.Utilities
{
	// Token: 0x020002EB RID: 747
	public static class StaticDictionaryPool<TKey, TValue, TEqualityComparer> where TEqualityComparer : IEqualityComparer<TKey>, new()
	{
		// Token: 0x06001551 RID: 5457 RVA: 0x00050FB5 File Offset: 0x0004F1B5
		public static Dictionary<TKey, TValue> GetFromPool()
		{
			if (StaticDictionaryPool<TKey, TValue, TEqualityComparer>.m_pool.Count <= 0)
			{
				return new Dictionary<TKey, TValue>(64, Activator.CreateInstance<TEqualityComparer>());
			}
			return StaticDictionaryPool<TKey, TValue, TEqualityComparer>.m_pool.Pop();
		}

		// Token: 0x06001552 RID: 5458 RVA: 0x00050FE0 File Offset: 0x0004F1E0
		public static void ReturnToPool(Dictionary<TKey, TValue> item)
		{
			if (item != null)
			{
				item.Clear();
				if (StaticDictionaryPool<TKey, TValue, TEqualityComparer>.m_pool.Count < 128)
				{
					StaticDictionaryPool<TKey, TValue, TEqualityComparer>.m_pool.Push(item);
				}
			}
		}

		// Token: 0x04001D7B RID: 7547
		private const int kMaxPoolSize = 128;

		// Token: 0x04001D7C RID: 7548
		private static readonly Stack<Dictionary<TKey, TValue>> m_pool = new Stack<Dictionary<TKey, TValue>>(128);
	}
}
