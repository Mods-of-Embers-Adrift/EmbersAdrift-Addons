using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Animation
{
	// Token: 0x02000D87 RID: 3463
	public struct StanceData : INetworkSerializable
	{
		// Token: 0x0600684C RID: 26700 RVA: 0x00086042 File Offset: 0x00084242
		BitBuffer INetworkSerializable.PackData(BitBuffer buffer)
		{
			buffer.AddBool(this.BypassTransition);
			buffer.AddUniqueId(this.StanceId);
			return buffer;
		}

		// Token: 0x0600684D RID: 26701 RVA: 0x0008605F File Offset: 0x0008425F
		BitBuffer INetworkSerializable.ReadData(BitBuffer buffer)
		{
			this.BypassTransition = buffer.ReadBool();
			this.StanceId = buffer.ReadUniqueId();
			return buffer;
		}

		// Token: 0x0600684E RID: 26702 RVA: 0x0008607A File Offset: 0x0008427A
		public override string ToString()
		{
			return this.StanceId.ToString() + ", " + this.BypassTransition.ToString();
		}

		// Token: 0x04005A7F RID: 23167
		public bool BypassTransition;

		// Token: 0x04005A80 RID: 23168
		public UniqueId StanceId;
	}
}
