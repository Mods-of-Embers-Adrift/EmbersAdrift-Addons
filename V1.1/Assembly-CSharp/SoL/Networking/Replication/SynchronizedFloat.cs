using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x02000498 RID: 1176
	[Serializable]
	public class SynchronizedFloat : SynchronizedVariable<float>
	{
		// Token: 0x060020F6 RID: 8438 RVA: 0x00057F17 File Offset: 0x00056117
		public SynchronizedFloat()
		{
		}

		// Token: 0x060020F7 RID: 8439 RVA: 0x00057F1F File Offset: 0x0005611F
		public SynchronizedFloat(float initial) : base(initial)
		{
		}

		// Token: 0x060020F8 RID: 8440 RVA: 0x00057F28 File Offset: 0x00056128
		protected override BitBuffer PackDataInternal(BitBuffer buffer)
		{
			buffer.AddFloat(base.Value);
			return buffer;
		}

		// Token: 0x060020F9 RID: 8441 RVA: 0x00057C9B File Offset: 0x00055E9B
		protected override float ReadDataInternal(BitBuffer buffer)
		{
			return buffer.ReadFloat();
		}
	}
}
