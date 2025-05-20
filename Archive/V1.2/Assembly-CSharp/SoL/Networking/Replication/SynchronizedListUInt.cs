using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x0200048C RID: 1164
	public class SynchronizedListUInt : SynchronizedList<uint>
	{
		// Token: 0x060020B4 RID: 8372 RVA: 0x00057C75 File Offset: 0x00055E75
		protected override BitBuffer WriteValue(BitBuffer buffer, uint value)
		{
			buffer.AddUInt(value);
			return buffer;
		}

		// Token: 0x060020B5 RID: 8373 RVA: 0x00057C80 File Offset: 0x00055E80
		protected override uint ReadValue(BitBuffer buffer)
		{
			return buffer.ReadUInt();
		}
	}
}
