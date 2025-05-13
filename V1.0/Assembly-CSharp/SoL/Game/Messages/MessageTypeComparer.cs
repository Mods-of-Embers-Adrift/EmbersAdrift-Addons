using System;
using System.Collections.Generic;

namespace SoL.Game.Messages
{
	// Token: 0x020009E8 RID: 2536
	public struct MessageTypeComparer : IEqualityComparer<MessageType>
	{
		// Token: 0x06004D2B RID: 19755 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(MessageType x, MessageType y)
		{
			return x == y;
		}

		// Token: 0x06004D2C RID: 19756 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(MessageType obj)
		{
			return (int)obj;
		}
	}
}
