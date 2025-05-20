using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x020004A4 RID: 1188
	public class SynchronizedNullableDateTime : SynchronizedVariable<DateTime?>
	{
		// Token: 0x06002124 RID: 8484 RVA: 0x0005809D File Offset: 0x0005629D
		public SynchronizedNullableDateTime()
		{
		}

		// Token: 0x06002125 RID: 8485 RVA: 0x000580A5 File Offset: 0x000562A5
		public SynchronizedNullableDateTime(DateTime? initial) : base(initial)
		{
		}

		// Token: 0x06002126 RID: 8486 RVA: 0x00122994 File Offset: 0x00120B94
		protected override BitBuffer PackDataInternal(BitBuffer buffer)
		{
			bool flag = base.Value != null;
			buffer.AddBool(flag);
			if (flag)
			{
				buffer.AddLong(base.Value.Value.Ticks);
			}
			return buffer;
		}

		// Token: 0x06002127 RID: 8487 RVA: 0x001229DC File Offset: 0x00120BDC
		protected override DateTime? ReadDataInternal(BitBuffer buffer)
		{
			if (buffer.ReadBool())
			{
				return new DateTime?(new DateTime(buffer.ReadLong()));
			}
			return null;
		}
	}
}
