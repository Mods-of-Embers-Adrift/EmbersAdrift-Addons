using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x0200048A RID: 1162
	public class SynchronizedListString : SynchronizedList<string>
	{
		// Token: 0x060020AE RID: 8366 RVA: 0x00057C52 File Offset: 0x00055E52
		protected override BitBuffer WriteValue(BitBuffer buffer, string value)
		{
			buffer.AddString(value);
			return buffer;
		}

		// Token: 0x060020AF RID: 8367 RVA: 0x00057C5D File Offset: 0x00055E5D
		protected override string ReadValue(BitBuffer buffer)
		{
			return buffer.ReadString();
		}
	}
}
