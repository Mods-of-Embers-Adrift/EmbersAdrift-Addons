using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x020004A3 RID: 1187
	public class SynchronizedDateTime : SynchronizedVariable<DateTime>
	{
		// Token: 0x06002120 RID: 8480 RVA: 0x0005807F File Offset: 0x0005627F
		public SynchronizedDateTime()
		{
		}

		// Token: 0x06002121 RID: 8481 RVA: 0x00058087 File Offset: 0x00056287
		public SynchronizedDateTime(DateTime initial) : base(initial)
		{
		}

		// Token: 0x06002122 RID: 8482 RVA: 0x00122970 File Offset: 0x00120B70
		protected override BitBuffer PackDataInternal(BitBuffer buffer)
		{
			buffer.AddLong(base.Value.Ticks);
			return buffer;
		}

		// Token: 0x06002123 RID: 8483 RVA: 0x00058090 File Offset: 0x00056290
		protected override DateTime ReadDataInternal(BitBuffer buffer)
		{
			return new DateTime(buffer.ReadLong());
		}
	}
}
