using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x0200049A RID: 1178
	[Serializable]
	public class SynchronizedString : SynchronizedVariable<string>
	{
		// Token: 0x060020FE RID: 8446 RVA: 0x00057F49 File Offset: 0x00056149
		public SynchronizedString()
		{
		}

		// Token: 0x060020FF RID: 8447 RVA: 0x00057F51 File Offset: 0x00056151
		public SynchronizedString(string initial) : base(initial)
		{
		}

		// Token: 0x06002100 RID: 8448 RVA: 0x00057F5A File Offset: 0x0005615A
		protected override BitBuffer PackDataInternal(BitBuffer buffer)
		{
			if (base.Value == null)
			{
				base.Value = "";
			}
			buffer.AddString(base.Value);
			return buffer;
		}

		// Token: 0x06002101 RID: 8449 RVA: 0x00057C5D File Offset: 0x00055E5D
		protected override string ReadDataInternal(BitBuffer buffer)
		{
			return buffer.ReadString();
		}
	}
}
