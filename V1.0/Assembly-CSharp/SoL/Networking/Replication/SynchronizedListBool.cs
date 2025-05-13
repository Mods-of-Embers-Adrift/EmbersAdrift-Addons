using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x0200048E RID: 1166
	public class SynchronizedListBool : SynchronizedList<bool>
	{
		// Token: 0x060020BA RID: 8378 RVA: 0x00057CAB File Offset: 0x00055EAB
		protected override BitBuffer WriteValue(BitBuffer buffer, bool value)
		{
			buffer.AddBool(value);
			return buffer;
		}

		// Token: 0x060020BB RID: 8379 RVA: 0x00057CB6 File Offset: 0x00055EB6
		protected override bool ReadValue(BitBuffer buffer)
		{
			return buffer.ReadBool();
		}
	}
}
