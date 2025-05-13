using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x020004A1 RID: 1185
	public class SynchronizedNullableUniqueId : SynchronizedVariable<UniqueId?>
	{
		// Token: 0x06002119 RID: 8473 RVA: 0x0005804E File Offset: 0x0005624E
		protected override BitBuffer PackDataInternal(BitBuffer buffer)
		{
			buffer.AddNullableUniqueId(base.Value);
			return buffer;
		}

		// Token: 0x0600211A RID: 8474 RVA: 0x0005805E File Offset: 0x0005625E
		protected override UniqueId? ReadDataInternal(BitBuffer buffer)
		{
			return buffer.ReadNullableUniqueId();
		}
	}
}
