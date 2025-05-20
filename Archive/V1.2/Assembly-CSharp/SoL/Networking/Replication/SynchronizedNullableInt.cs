using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x02000496 RID: 1174
	[Serializable]
	public class SynchronizedNullableInt : SynchronizedVariable<int?>
	{
		// Token: 0x060020EE RID: 8430 RVA: 0x00057EDD File Offset: 0x000560DD
		public SynchronizedNullableInt()
		{
		}

		// Token: 0x060020EF RID: 8431 RVA: 0x00057EE5 File Offset: 0x000560E5
		public SynchronizedNullableInt(int? initial) : base(initial)
		{
		}

		// Token: 0x060020F0 RID: 8432 RVA: 0x00057EEE File Offset: 0x000560EE
		protected override BitBuffer PackDataInternal(BitBuffer buffer)
		{
			buffer.AddNullableInt(base.Value);
			return buffer;
		}

		// Token: 0x060020F1 RID: 8433 RVA: 0x00057EFE File Offset: 0x000560FE
		protected override int? ReadDataInternal(BitBuffer buffer)
		{
			return buffer.ReadNullableInt();
		}
	}
}
