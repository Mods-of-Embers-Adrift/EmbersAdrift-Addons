using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x0200048B RID: 1163
	public class SynchronizedListInt : SynchronizedList<int>
	{
		// Token: 0x060020B1 RID: 8369 RVA: 0x00057ABE File Offset: 0x00055CBE
		protected override BitBuffer WriteValue(BitBuffer buffer, int value)
		{
			buffer.AddInt(value);
			return buffer;
		}

		// Token: 0x060020B2 RID: 8370 RVA: 0x00057ADB File Offset: 0x00055CDB
		protected override int ReadValue(BitBuffer buffer)
		{
			return buffer.ReadInt();
		}
	}
}
