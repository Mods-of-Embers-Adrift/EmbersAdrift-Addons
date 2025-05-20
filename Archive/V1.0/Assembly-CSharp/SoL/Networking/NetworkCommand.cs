using System;
using ENet;

namespace SoL.Networking
{
	// Token: 0x020003C4 RID: 964
	public class NetworkCommand
	{
		// Token: 0x060019F4 RID: 6644 RVA: 0x00107BCC File Offset: 0x00105DCC
		public void Reset()
		{
			this.Type = CommandType.None;
			this.ConnectionSettings = null;
			this.Source = default(Peer);
			this.Target = default(Peer);
			Peer[] targetGroup = this.TargetGroup;
			if (targetGroup != null)
			{
				targetGroup.ReturnToPool();
			}
			this.TargetGroup = null;
			this.Packet = default(Packet);
			this.Channel = NetworkChannel.Invalid;
		}

		// Token: 0x04002115 RID: 8469
		public CommandType Type;

		// Token: 0x04002116 RID: 8470
		public ConnectionSettings ConnectionSettings;

		// Token: 0x04002117 RID: 8471
		public Peer Source;

		// Token: 0x04002118 RID: 8472
		public Peer Target;

		// Token: 0x04002119 RID: 8473
		public Peer[] TargetGroup;

		// Token: 0x0400211A RID: 8474
		public Packet Packet;

		// Token: 0x0400211B RID: 8475
		public NetworkChannel Channel;
	}
}
