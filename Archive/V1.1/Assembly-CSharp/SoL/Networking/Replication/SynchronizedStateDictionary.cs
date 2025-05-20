using System;
using System.Collections.Generic;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x02000487 RID: 1159
	public class SynchronizedStateDictionary : SynchronizedDictionary<Dictionary<int, byte>, int, byte>
	{
		// Token: 0x06002081 RID: 8321 RVA: 0x00057AE3 File Offset: 0x00055CE3
		public SynchronizedStateDictionary()
		{
			this.m_objects = new Dictionary<int, byte>();
		}

		// Token: 0x06002082 RID: 8322 RVA: 0x00057ABE File Offset: 0x00055CBE
		protected override BitBuffer WriteKey(BitBuffer buffer, int key)
		{
			buffer.AddInt(key);
			return buffer;
		}

		// Token: 0x06002083 RID: 8323 RVA: 0x00057AF6 File Offset: 0x00055CF6
		protected override BitBuffer WriteValue(BitBuffer buffer, byte value)
		{
			buffer.AddByte(value);
			return buffer;
		}

		// Token: 0x06002084 RID: 8324 RVA: 0x00057ADB File Offset: 0x00055CDB
		protected override int ReadKey(BitBuffer buffer)
		{
			return buffer.ReadInt();
		}

		// Token: 0x06002085 RID: 8325 RVA: 0x00057B01 File Offset: 0x00055D01
		protected override byte ReadValue(BitBuffer buffer)
		{
			return buffer.ReadByte();
		}
	}
}
