using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Objects.Archetypes.Abilities
{
	// Token: 0x02000AFB RID: 2811
	public struct UtilityResult : INetworkSerializable
	{
		// Token: 0x060056F9 RID: 22265 RVA: 0x001E1948 File Offset: 0x001DFB48
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.Function);
			bool flag = this.Index0 != null;
			buffer.AddBool(flag);
			if (flag)
			{
				buffer.AddInt(this.Index0.Value);
			}
			flag = (this.Index1 != null);
			buffer.AddBool(flag);
			if (flag)
			{
				buffer.AddInt(this.Index1.Value);
			}
			flag = !string.IsNullOrEmpty(this.Text);
			buffer.AddBool(flag);
			if (flag)
			{
				buffer.AddString(this.Text);
			}
			return buffer;
		}

		// Token: 0x060056FA RID: 22266 RVA: 0x001E19DC File Offset: 0x001DFBDC
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Function = buffer.ReadEnum<UtilityFunction>();
			if (buffer.ReadBool())
			{
				this.Index0 = new int?(buffer.ReadInt());
			}
			if (buffer.ReadBool())
			{
				this.Index1 = new int?(buffer.ReadInt());
			}
			if (buffer.ReadBool())
			{
				this.Text = buffer.ReadString();
			}
			return buffer;
		}

		// Token: 0x04004CB0 RID: 19632
		public UtilityResultSendType SendType;

		// Token: 0x04004CB1 RID: 19633
		public UtilityFunction Function;

		// Token: 0x04004CB2 RID: 19634
		public int? Index0;

		// Token: 0x04004CB3 RID: 19635
		public int? Index1;

		// Token: 0x04004CB4 RID: 19636
		public string Text;
	}
}
