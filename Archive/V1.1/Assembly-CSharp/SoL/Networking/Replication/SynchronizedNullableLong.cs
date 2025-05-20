using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x0200049D RID: 1181
	[Serializable]
	public class SynchronizedNullableLong : SynchronizedVariable<long?>
	{
		// Token: 0x0600210A RID: 8458 RVA: 0x00057FA6 File Offset: 0x000561A6
		public SynchronizedNullableLong()
		{
		}

		// Token: 0x0600210B RID: 8459 RVA: 0x00057FAE File Offset: 0x000561AE
		public SynchronizedNullableLong(long? initial) : base(initial)
		{
		}

		// Token: 0x0600210C RID: 8460 RVA: 0x00122850 File Offset: 0x00120A50
		protected override BitBuffer PackDataInternal(BitBuffer buffer)
		{
			buffer.AddBool(base.Value != null);
			if (base.Value != null)
			{
				buffer.AddLong(base.Value.Value);
			}
			return buffer;
		}

		// Token: 0x0600210D RID: 8461 RVA: 0x00122898 File Offset: 0x00120A98
		protected override long? ReadDataInternal(BitBuffer buffer)
		{
			if (buffer.ReadBool())
			{
				return new long?(buffer.ReadLong());
			}
			return null;
		}
	}
}
