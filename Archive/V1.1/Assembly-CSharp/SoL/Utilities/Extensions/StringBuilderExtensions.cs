using System;
using System.Collections.Generic;
using System.Text;

namespace SoL.Utilities.Extensions
{
	// Token: 0x02000337 RID: 823
	public static class StringBuilderExtensions
	{
		// Token: 0x06001692 RID: 5778 RVA: 0x00051C36 File Offset: 0x0004FE36
		public static StringBuilder GetFromPool()
		{
			if (StringBuilderExtensions.m_pool.Count > 0)
			{
				return StringBuilderExtensions.m_pool.Pop();
			}
			return new StringBuilder();
		}

		// Token: 0x06001693 RID: 5779 RVA: 0x00051C55 File Offset: 0x0004FE55
		public static string ToString_ReturnToPool(this StringBuilder sb)
		{
			string result = (sb.Length > 0) ? sb.ToString() : string.Empty;
			sb.ReturnToPool();
			return result;
		}

		// Token: 0x06001694 RID: 5780 RVA: 0x00051C73 File Offset: 0x0004FE73
		public static void ReturnToPool(this StringBuilder sb)
		{
			sb.Clear();
			if (StringBuilderExtensions.m_pool.Count < 32)
			{
				StringBuilderExtensions.m_pool.Push(sb);
			}
		}

		// Token: 0x04001E5A RID: 7770
		private const int kMaxPoolSize = 32;

		// Token: 0x04001E5B RID: 7771
		private static readonly Stack<StringBuilder> m_pool = new Stack<StringBuilder>(32);
	}
}
