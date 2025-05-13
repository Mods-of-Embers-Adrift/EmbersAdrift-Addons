using System;
using ENet;
using NetStack.Serialization;

namespace SoL.Networking
{
	// Token: 0x020003CC RID: 972
	public static class EventExtensions
	{
		// Token: 0x06001A07 RID: 6663 RVA: 0x00107DFC File Offset: 0x00105FFC
		public static BitBuffer GetBufferFromPacket(this Packet packet, BitBuffer buffer = null)
		{
			byte[] byteArray = ByteArrayPool.GetByteArray(packet.Length);
			packet.CopyTo(byteArray);
			if (buffer == null)
			{
				buffer = BitBufferExtensions.GetFromPool();
			}
			else
			{
				buffer.Clear();
			}
			buffer.FromArray(byteArray, packet.Length);
			byteArray.ReturnToPool();
			return buffer;
		}
	}
}
