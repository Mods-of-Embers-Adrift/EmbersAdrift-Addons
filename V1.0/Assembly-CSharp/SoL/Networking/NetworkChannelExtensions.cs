using System;

namespace SoL.Networking
{
	// Token: 0x020003C1 RID: 961
	public static class NetworkChannelExtensions
	{
		// Token: 0x060019F0 RID: 6640 RVA: 0x0004BC2B File Offset: 0x00049E2B
		public static byte GetByte(this NetworkChannel channel)
		{
			return (byte)channel;
		}

		// Token: 0x060019F1 RID: 6641 RVA: 0x0004BC2B File Offset: 0x00049E2B
		public static NetworkChannel GetChannel(byte byteChannel)
		{
			return (NetworkChannel)byteChannel;
		}

		// Token: 0x060019F2 RID: 6642 RVA: 0x00107B78 File Offset: 0x00105D78
		public static int GetInternalChannelId(this NetworkChannel channel)
		{
			switch (channel)
			{
			case NetworkChannel.State_Client:
			case NetworkChannel.State_Server:
				return 0;
			case NetworkChannel.SyncVar_Client:
			case NetworkChannel.SyncVar_Server:
				return 1;
			case NetworkChannel.AnimatorState_Client:
			case NetworkChannel.AnimatorState_Server:
				return 3;
			case NetworkChannel.AnimatorSync_Client:
			case NetworkChannel.AnimatorSync_Server:
				return 2;
			default:
				throw new ArgumentException(string.Format("Not configured for {0}!", channel));
			}
		}
	}
}
