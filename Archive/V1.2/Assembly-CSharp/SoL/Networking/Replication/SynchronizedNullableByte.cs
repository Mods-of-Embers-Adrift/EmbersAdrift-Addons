using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x02000493 RID: 1171
	[Serializable]
	public class SynchronizedNullableByte : SynchronizedVariable<byte?>
	{
		// Token: 0x060020E2 RID: 8418 RVA: 0x00057E72 File Offset: 0x00056072
		public SynchronizedNullableByte()
		{
		}

		// Token: 0x060020E3 RID: 8419 RVA: 0x00057E7A File Offset: 0x0005607A
		public SynchronizedNullableByte(byte? initial) : base(initial)
		{
		}

		// Token: 0x060020E4 RID: 8420 RVA: 0x00057E83 File Offset: 0x00056083
		protected override BitBuffer PackDataInternal(BitBuffer buffer)
		{
			buffer.AddNullableByte(base.Value);
			return buffer;
		}

		// Token: 0x060020E5 RID: 8421 RVA: 0x00057E93 File Offset: 0x00056093
		protected override byte? ReadDataInternal(BitBuffer buffer)
		{
			return buffer.ReadNullableByte();
		}
	}
}
