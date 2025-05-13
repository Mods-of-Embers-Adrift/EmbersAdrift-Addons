using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Utilities
{
	// Token: 0x020002B0 RID: 688
	public struct LevelProgressionData : INetworkSerializable
	{
		// Token: 0x0600148E RID: 5262 RVA: 0x00050504 File Offset: 0x0004E704
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.ArchetypeId);
			buffer.AddFloat(this.NewLevel);
			return buffer;
		}

		// Token: 0x0600148F RID: 5263 RVA: 0x00050521 File Offset: 0x0004E721
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.ArchetypeId = buffer.ReadUniqueId();
			this.NewLevel = buffer.ReadFloat();
			return buffer;
		}

		// Token: 0x04001CC8 RID: 7368
		public UniqueId ArchetypeId;

		// Token: 0x04001CC9 RID: 7369
		public float NewLevel;
	}
}
