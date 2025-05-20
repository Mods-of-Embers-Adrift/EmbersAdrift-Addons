using System;
using System.Collections.Generic;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x02000486 RID: 1158
	public class SynchronizedDictIntKey<TValue> : SynchronizedDictionary<Dictionary<int, TValue>, int, TValue> where TValue : INetworkSerializable, new()
	{
		// Token: 0x0600207C RID: 8316 RVA: 0x00057AAB File Offset: 0x00055CAB
		public SynchronizedDictIntKey()
		{
			this.m_objects = new Dictionary<int, TValue>();
		}

		// Token: 0x0600207D RID: 8317 RVA: 0x00057ABE File Offset: 0x00055CBE
		protected override BitBuffer WriteKey(BitBuffer buffer, int key)
		{
			buffer.AddInt(key);
			return buffer;
		}

		// Token: 0x0600207E RID: 8318 RVA: 0x00057AC9 File Offset: 0x00055CC9
		protected override BitBuffer WriteValue(BitBuffer buffer, TValue value)
		{
			value.PackData(buffer);
			return buffer;
		}

		// Token: 0x0600207F RID: 8319 RVA: 0x00057ADB File Offset: 0x00055CDB
		protected override int ReadKey(BitBuffer buffer)
		{
			return buffer.ReadInt();
		}

		// Token: 0x06002080 RID: 8320 RVA: 0x001222F4 File Offset: 0x001204F4
		protected override TValue ReadValue(BitBuffer buffer)
		{
			TValue result = Activator.CreateInstance<TValue>();
			result.ReadData(buffer);
			return result;
		}
	}
}
