using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x0200048F RID: 1167
	public class SynchronizedListStruct<T> : SynchronizedList<T> where T : struct, INetworkSerializable
	{
		// Token: 0x060020BD RID: 8381 RVA: 0x00057CC6 File Offset: 0x00055EC6
		protected override BitBuffer WriteValue(BitBuffer buffer, T value)
		{
			return value.PackData(buffer);
		}

		// Token: 0x060020BE RID: 8382 RVA: 0x00122314 File Offset: 0x00120514
		protected override T ReadValue(BitBuffer buffer)
		{
			T result = Activator.CreateInstance<T>();
			result.ReadData(buffer);
			return result;
		}
	}
}
