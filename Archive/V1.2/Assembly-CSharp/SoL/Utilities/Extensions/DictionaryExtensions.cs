using System;
using System.Collections.Generic;

namespace SoL.Utilities.Extensions
{
	// Token: 0x0200032E RID: 814
	public static class DictionaryExtensions
	{
		// Token: 0x0600165C RID: 5724 RVA: 0x000519E0 File Offset: 0x0004FBE0
		public static void AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
		{
			if (dict.ContainsKey(key))
			{
				dict[key] = value;
				return;
			}
			dict.Add(key, value);
		}

		// Token: 0x0600165D RID: 5725 RVA: 0x00100444 File Offset: 0x000FE644
		public static int RemoveAll<TKey, TValue>(this Dictionary<TKey, TValue> dict, Predicate<TValue> predicate)
		{
			int num = 0;
			List<TKey> fromPool = StaticListPool<TKey>.GetFromPool();
			foreach (KeyValuePair<TKey, TValue> keyValuePair in dict)
			{
				if (predicate(keyValuePair.Value))
				{
					fromPool.Add(keyValuePair.Key);
				}
			}
			foreach (TKey key in fromPool)
			{
				if (dict.Remove(key))
				{
					num++;
				}
			}
			StaticListPool<TKey>.ReturnToPool(fromPool);
			return num;
		}
	}
}
