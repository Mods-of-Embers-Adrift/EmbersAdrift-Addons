using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x02000497 RID: 1175
	[Serializable]
	public class SynchronizedNullableUInt : SynchronizedVariable<uint?>
	{
		// Token: 0x060020F2 RID: 8434 RVA: 0x00057F06 File Offset: 0x00056106
		public SynchronizedNullableUInt()
		{
		}

		// Token: 0x060020F3 RID: 8435 RVA: 0x00057F0E File Offset: 0x0005610E
		public SynchronizedNullableUInt(uint? initial) : base(initial)
		{
		}

		// Token: 0x060020F4 RID: 8436 RVA: 0x001226F4 File Offset: 0x001208F4
		protected override BitBuffer PackDataInternal(BitBuffer buffer)
		{
			buffer.AddBool(base.Value != null);
			if (base.Value != null)
			{
				buffer.AddUInt(base.Value.Value);
			}
			return buffer;
		}

		// Token: 0x060020F5 RID: 8437 RVA: 0x0012273C File Offset: 0x0012093C
		protected override uint? ReadDataInternal(BitBuffer buffer)
		{
			if (buffer.ReadBool())
			{
				return new uint?(buffer.ReadUInt());
			}
			return null;
		}
	}
}
