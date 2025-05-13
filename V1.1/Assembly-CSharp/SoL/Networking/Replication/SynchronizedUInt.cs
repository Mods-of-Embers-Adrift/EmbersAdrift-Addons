using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x02000495 RID: 1173
	[Serializable]
	public class SynchronizedUInt : SynchronizedVariable<uint>
	{
		// Token: 0x060020EA RID: 8426 RVA: 0x00057EBC File Offset: 0x000560BC
		public SynchronizedUInt()
		{
		}

		// Token: 0x060020EB RID: 8427 RVA: 0x00057EC4 File Offset: 0x000560C4
		public SynchronizedUInt(uint initial) : base(initial)
		{
		}

		// Token: 0x060020EC RID: 8428 RVA: 0x00057ECD File Offset: 0x000560CD
		protected override BitBuffer PackDataInternal(BitBuffer buffer)
		{
			buffer.AddUInt(base.Value);
			return buffer;
		}

		// Token: 0x060020ED RID: 8429 RVA: 0x00057C80 File Offset: 0x00055E80
		protected override uint ReadDataInternal(BitBuffer buffer)
		{
			return buffer.ReadUInt();
		}
	}
}
