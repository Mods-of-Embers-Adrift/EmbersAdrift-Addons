using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Discovery
{
	// Token: 0x02000CA4 RID: 3236
	public struct DiscoveryNotification : INetworkSerializable
	{
		// Token: 0x0600621C RID: 25116 RVA: 0x000821BE File Offset: 0x000803BE
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.Id);
			buffer.AddEnum(this.Category);
			buffer.AddNullableString(this.Name);
			return buffer;
		}

		// Token: 0x0600621D RID: 25117 RVA: 0x000821E8 File Offset: 0x000803E8
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Id = buffer.ReadUniqueId();
			this.Category = buffer.ReadEnum<DiscoveryCategory>();
			this.Name = buffer.ReadNullableString();
			return buffer;
		}

		// Token: 0x040055BC RID: 21948
		public UniqueId Id;

		// Token: 0x040055BD RID: 21949
		public DiscoveryCategory Category;

		// Token: 0x040055BE RID: 21950
		public string Name;
	}
}
