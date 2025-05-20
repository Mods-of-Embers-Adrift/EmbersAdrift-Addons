using System;
using System.Text;
using NetStack.Serialization;
using SoL.Utilities.Extensions;

namespace SoL.Networking.Replication
{
	// Token: 0x0200049B RID: 1179
	[Serializable]
	public class SynchronizedASCII : SynchronizedVariable<string>
	{
		// Token: 0x06002102 RID: 8450 RVA: 0x00057F49 File Offset: 0x00056149
		public SynchronizedASCII()
		{
		}

		// Token: 0x06002103 RID: 8451 RVA: 0x00057F51 File Offset: 0x00056151
		public SynchronizedASCII(string initial) : base(initial)
		{
		}

		// Token: 0x06002104 RID: 8452 RVA: 0x001227DC File Offset: 0x001209DC
		protected override BitBuffer PackDataInternal(BitBuffer buffer)
		{
			if (base.Value == null)
			{
				base.Value = "";
			}
			int length = base.Value.Length;
			buffer.AddInt(length);
			for (int i = 0; i < length; i++)
			{
				buffer.AddByte((byte)base.Value[i]);
			}
			return buffer;
		}

		// Token: 0x06002105 RID: 8453 RVA: 0x00122834 File Offset: 0x00120A34
		protected override string ReadDataInternal(BitBuffer buffer)
		{
			StringBuilder fromPool = StringBuilderExtensions.GetFromPool();
			int num = buffer.ReadInt();
			for (int i = 0; i < num; i++)
			{
				fromPool.Insert(i, (char)buffer.ReadByte());
			}
			return fromPool.ToString_ReturnToPool();
		}
	}
}
