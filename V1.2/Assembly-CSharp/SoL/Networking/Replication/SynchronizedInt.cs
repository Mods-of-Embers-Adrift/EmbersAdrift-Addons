using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x02000494 RID: 1172
	[Serializable]
	public class SynchronizedInt : SynchronizedVariable<int>
	{
		// Token: 0x060020E6 RID: 8422 RVA: 0x00057E9B File Offset: 0x0005609B
		public SynchronizedInt()
		{
		}

		// Token: 0x060020E7 RID: 8423 RVA: 0x00057EA3 File Offset: 0x000560A3
		public SynchronizedInt(int initial) : base(initial)
		{
		}

		// Token: 0x060020E8 RID: 8424 RVA: 0x00057EAC File Offset: 0x000560AC
		protected override BitBuffer PackDataInternal(BitBuffer buffer)
		{
			buffer.AddInt(base.Value);
			return buffer;
		}

		// Token: 0x060020E9 RID: 8425 RVA: 0x00057ADB File Offset: 0x00055CDB
		protected override int ReadDataInternal(BitBuffer buffer)
		{
			return buffer.ReadInt();
		}
	}
}
