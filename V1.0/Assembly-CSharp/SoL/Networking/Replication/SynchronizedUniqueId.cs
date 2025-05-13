using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x020004A0 RID: 1184
	public class SynchronizedUniqueId : SynchronizedVariable<UniqueId>
	{
		// Token: 0x06002116 RID: 8470 RVA: 0x0005802E File Offset: 0x0005622E
		protected override BitBuffer PackDataInternal(BitBuffer buffer)
		{
			buffer.AddUniqueId(base.Value);
			return buffer;
		}

		// Token: 0x06002117 RID: 8471 RVA: 0x0005803E File Offset: 0x0005623E
		protected override UniqueId ReadDataInternal(BitBuffer buffer)
		{
			return buffer.ReadUniqueId();
		}
	}
}
