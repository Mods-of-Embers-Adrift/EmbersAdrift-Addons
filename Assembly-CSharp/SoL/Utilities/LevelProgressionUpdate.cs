using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Utilities
{
	// Token: 0x020002B2 RID: 690
	public struct LevelProgressionUpdate : INetworkSerializable
	{
		// Token: 0x06001492 RID: 5266 RVA: 0x0005053C File Offset: 0x0004E73C
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.ArchetypeId);
			buffer.AddFloat(this.BaseLevel);
			buffer.AddNullableFloat(this.SpecializationLevel);
			buffer.AddBool(this.HasBonusXp);
			return buffer;
		}

		// Token: 0x06001493 RID: 5267 RVA: 0x00050573 File Offset: 0x0004E773
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.ArchetypeId = buffer.ReadUniqueId();
			this.BaseLevel = buffer.ReadFloat();
			this.SpecializationLevel = buffer.ReadNullableFloat();
			this.HasBonusXp = buffer.ReadBool();
			return buffer;
		}

		// Token: 0x04001CCB RID: 7371
		public UniqueId ArchetypeId;

		// Token: 0x04001CCC RID: 7372
		public float BaseLevel;

		// Token: 0x04001CCD RID: 7373
		public float? SpecializationLevel;

		// Token: 0x04001CCE RID: 7374
		public bool HasBonusXp;
	}
}
