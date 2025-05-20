using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x0200049F RID: 1183
	public class SynchronizedStruct<T> : SynchronizedVariable<T> where T : struct, INetworkSerializable
	{
		// Token: 0x06002113 RID: 8467 RVA: 0x001228E4 File Offset: 0x00120AE4
		protected override BitBuffer PackDataInternal(BitBuffer buffer)
		{
			T value = base.Value;
			value.PackData(buffer);
			return buffer;
		}

		// Token: 0x06002114 RID: 8468 RVA: 0x00122314 File Offset: 0x00120514
		protected override T ReadDataInternal(BitBuffer buffer)
		{
			T result = Activator.CreateInstance<T>();
			result.ReadData(buffer);
			return result;
		}
	}
}
