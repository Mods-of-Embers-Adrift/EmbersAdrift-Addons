using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003F7 RID: 1015
	public class StateObject
	{
		// Token: 0x06001B06 RID: 6918 RVA: 0x00054F9C File Offset: 0x0005319C
		public StateObject(Socket socket)
		{
			this.WorkSocket = socket;
			this.Buffer = new byte[1024];
			this.RawMessage = new List<byte>();
		}

		// Token: 0x06001B07 RID: 6919 RVA: 0x0010A964 File Offset: 0x00108B64
		public void StoreBuffer(int bytesRead)
		{
			for (int i = 0; i < bytesRead; i++)
			{
				if (i == bytesRead - 1 && this.Buffer[i] == 4)
				{
					this.IsReady = true;
				}
				else
				{
					this.RawMessage.Add(this.Buffer[i]);
				}
			}
		}

		// Token: 0x06001B08 RID: 6920 RVA: 0x00054FC6 File Offset: 0x000531C6
		public void Reset()
		{
			this.RawMessage.Clear();
			this.IsReady = false;
		}

		// Token: 0x04002234 RID: 8756
		public const int kBufferSize = 1024;

		// Token: 0x04002235 RID: 8757
		public bool IsReady;

		// Token: 0x04002236 RID: 8758
		public readonly Socket WorkSocket;

		// Token: 0x04002237 RID: 8759
		public readonly byte[] Buffer;

		// Token: 0x04002238 RID: 8760
		public readonly List<byte> RawMessage;
	}
}
