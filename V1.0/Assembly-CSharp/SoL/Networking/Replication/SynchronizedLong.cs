using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x0200049C RID: 1180
	[Serializable]
	public class SynchronizedLong : SynchronizedVariable<long>
	{
		// Token: 0x06002106 RID: 8454 RVA: 0x00057F7D File Offset: 0x0005617D
		public SynchronizedLong()
		{
		}

		// Token: 0x06002107 RID: 8455 RVA: 0x00057F85 File Offset: 0x00056185
		public SynchronizedLong(long initial) : base(initial)
		{
		}

		// Token: 0x06002108 RID: 8456 RVA: 0x00057F8E File Offset: 0x0005618E
		protected override BitBuffer PackDataInternal(BitBuffer buffer)
		{
			buffer.AddLong(base.Value);
			return buffer;
		}

		// Token: 0x06002109 RID: 8457 RVA: 0x00057F9E File Offset: 0x0005619E
		protected override long ReadDataInternal(BitBuffer buffer)
		{
			return buffer.ReadLong();
		}
	}
}
