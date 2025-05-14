using System;
using System.Collections.Generic;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003F0 RID: 1008
	public static class SolServerCommandDictionaryPool
	{
		// Token: 0x17000652 RID: 1618
		// (get) Token: 0x06001ACA RID: 6858 RVA: 0x00054C7C File Offset: 0x00052E7C
		private static List<Dictionary<string, object>> Pool
		{
			get
			{
				if (SolServerCommandDictionaryPool.m_pool == null)
				{
					SolServerCommandDictionaryPool.m_pool = new List<Dictionary<string, object>>();
				}
				return SolServerCommandDictionaryPool.m_pool;
			}
		}

		// Token: 0x06001ACB RID: 6859 RVA: 0x00054C94 File Offset: 0x00052E94
		public static Dictionary<string, object> GetDictionary()
		{
			if (SolServerCommandDictionaryPool.Pool.Count <= 0)
			{
				return new Dictionary<string, object>();
			}
			Dictionary<string, object> result = SolServerCommandDictionaryPool.Pool[0];
			SolServerCommandDictionaryPool.Pool.RemoveAt(0);
			return result;
		}

		// Token: 0x06001ACC RID: 6860 RVA: 0x00054CBF File Offset: 0x00052EBF
		public static void ReturnToPool(this Dictionary<string, object> pair)
		{
			pair.Clear();
			if (SolServerCommandDictionaryPool.Pool.Count < 16)
			{
				SolServerCommandDictionaryPool.Pool.Add(pair);
			}
		}

		// Token: 0x04002205 RID: 8709
		private const int kMaxPoolSize = 16;

		// Token: 0x04002206 RID: 8710
		private static List<Dictionary<string, object>> m_pool;
	}
}
