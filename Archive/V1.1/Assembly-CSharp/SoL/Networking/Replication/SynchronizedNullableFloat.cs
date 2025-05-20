using System;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x02000499 RID: 1177
	[Serializable]
	public class SynchronizedNullableFloat : SynchronizedVariable<float?>
	{
		// Token: 0x060020FA RID: 8442 RVA: 0x00057F38 File Offset: 0x00056138
		public SynchronizedNullableFloat()
		{
		}

		// Token: 0x060020FB RID: 8443 RVA: 0x00057F40 File Offset: 0x00056140
		public SynchronizedNullableFloat(float? initial) : base(initial)
		{
		}

		// Token: 0x060020FC RID: 8444 RVA: 0x00122748 File Offset: 0x00120948
		protected override BitBuffer PackDataInternal(BitBuffer buffer)
		{
			buffer.AddBool(base.Value != null);
			if (base.Value != null)
			{
				buffer.AddFloat(base.Value.Value);
			}
			return buffer;
		}

		// Token: 0x060020FD RID: 8445 RVA: 0x00122790 File Offset: 0x00120990
		protected override float? ReadDataInternal(BitBuffer buffer)
		{
			if (buffer.ReadBool())
			{
				return new float?(buffer.ReadFloat());
			}
			return null;
		}
	}
}
