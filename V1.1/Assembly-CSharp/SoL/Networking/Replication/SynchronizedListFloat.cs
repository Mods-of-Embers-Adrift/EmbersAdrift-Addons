using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x0200048D RID: 1165
	public class SynchronizedListFloat : SynchronizedList<float>
	{
		// Token: 0x060020B7 RID: 8375 RVA: 0x00057C90 File Offset: 0x00055E90
		protected override BitBuffer WriteValue(BitBuffer buffer, float value)
		{
			buffer.AddFloat(value);
			return buffer;
		}

		// Token: 0x060020B8 RID: 8376 RVA: 0x00057C9B File Offset: 0x00055E9B
		protected override float ReadValue(BitBuffer buffer)
		{
			return buffer.ReadFloat();
		}
	}
}
