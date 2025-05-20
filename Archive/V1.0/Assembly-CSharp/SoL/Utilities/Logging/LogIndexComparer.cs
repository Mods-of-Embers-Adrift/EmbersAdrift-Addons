using System;
using System.Collections.Generic;

namespace SoL.Utilities.Logging
{
	// Token: 0x0200031C RID: 796
	public struct LogIndexComparer : IEqualityComparer<LogIndex>
	{
		// Token: 0x06001619 RID: 5657 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(LogIndex x, LogIndex y)
		{
			return x == y;
		}

		// Token: 0x0600161A RID: 5658 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(LogIndex obj)
		{
			return (int)obj;
		}
	}
}
