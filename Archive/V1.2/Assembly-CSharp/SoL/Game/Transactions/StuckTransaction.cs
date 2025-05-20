using System;
using NetStack.Serialization;
using SoL.Networking;
using SoL.Networking.Managers;
using UnityEngine;

namespace SoL.Game.Transactions
{
	// Token: 0x02000642 RID: 1602
	public struct StuckTransaction : INetworkSerializable
	{
		// Token: 0x060031EE RID: 12782 RVA: 0x0015E3E8 File Offset: 0x0015C5E8
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.Op);
			buffer.AddBool(this.Position != null);
			if (this.Position != null)
			{
				buffer.AddVector3(this.Position.Value, NetworkManager.Range);
			}
			return buffer;
		}

		// Token: 0x060031EF RID: 12783 RVA: 0x00062821 File Offset: 0x00060A21
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Op = buffer.ReadEnum<OpCodes>();
			if (buffer.ReadBool())
			{
				this.Position = new Vector3?(buffer.ReadVector3(NetworkManager.Range));
			}
			return buffer;
		}

		// Token: 0x04003097 RID: 12439
		public OpCodes Op;

		// Token: 0x04003098 RID: 12440
		public Vector3? Position;
	}
}
