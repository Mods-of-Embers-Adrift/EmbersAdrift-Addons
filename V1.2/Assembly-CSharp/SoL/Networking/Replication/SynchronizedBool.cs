using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x02000491 RID: 1169
	[Serializable]
	public class SynchronizedBool : SynchronizedVariable<bool>
	{
		// Token: 0x060020DA RID: 8410 RVA: 0x00057E30 File Offset: 0x00056030
		public SynchronizedBool()
		{
		}

		// Token: 0x060020DB RID: 8411 RVA: 0x00057E38 File Offset: 0x00056038
		public SynchronizedBool(bool initial) : base(initial)
		{
		}

		// Token: 0x060020DC RID: 8412 RVA: 0x00057E41 File Offset: 0x00056041
		protected override BitBuffer PackDataInternal(BitBuffer buffer)
		{
			buffer.AddBool(base.Value);
			return buffer;
		}

		// Token: 0x060020DD RID: 8413 RVA: 0x00057CB6 File Offset: 0x00055EB6
		protected override bool ReadDataInternal(BitBuffer buffer)
		{
			return buffer.ReadBool();
		}
	}
}
