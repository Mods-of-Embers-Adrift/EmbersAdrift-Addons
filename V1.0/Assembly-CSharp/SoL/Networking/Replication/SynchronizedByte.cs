using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x02000492 RID: 1170
	[Serializable]
	public class SynchronizedByte : SynchronizedVariable<byte>
	{
		// Token: 0x060020DE RID: 8414 RVA: 0x00057E51 File Offset: 0x00056051
		public SynchronizedByte()
		{
		}

		// Token: 0x060020DF RID: 8415 RVA: 0x00057E59 File Offset: 0x00056059
		public SynchronizedByte(byte initial) : base(initial)
		{
		}

		// Token: 0x060020E0 RID: 8416 RVA: 0x00057E62 File Offset: 0x00056062
		protected override BitBuffer PackDataInternal(BitBuffer buffer)
		{
			buffer.AddByte(base.Value);
			return buffer;
		}

		// Token: 0x060020E1 RID: 8417 RVA: 0x00057B01 File Offset: 0x00055D01
		protected override byte ReadDataInternal(BitBuffer buffer)
		{
			return buffer.ReadByte();
		}
	}
}
