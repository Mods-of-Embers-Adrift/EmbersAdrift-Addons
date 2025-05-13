using System;
using System.Collections.Generic;

namespace TriangleNet.Logging
{
	// Token: 0x02000136 RID: 310
	public interface ILog<T> where T : ILogItem
	{
		// Token: 0x06000AA1 RID: 2721
		void Add(T item);

		// Token: 0x06000AA2 RID: 2722
		void Clear();

		// Token: 0x06000AA3 RID: 2723
		void Info(string message);

		// Token: 0x06000AA4 RID: 2724
		void Error(string message, string info);

		// Token: 0x06000AA5 RID: 2725
		void Warning(string message, string info);

		// Token: 0x17000332 RID: 818
		// (get) Token: 0x06000AA6 RID: 2726
		IList<T> Data { get; }

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x06000AA7 RID: 2727
		LogLevel Level { get; }
	}
}
