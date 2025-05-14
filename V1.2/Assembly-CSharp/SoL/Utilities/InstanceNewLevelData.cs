using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Utilities
{
	// Token: 0x020002B3 RID: 691
	public struct InstanceNewLevelData : INetworkSerializable
	{
		// Token: 0x06001494 RID: 5268 RVA: 0x000FB04C File Offset: 0x000F924C
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddBool(this.MasteryId != null);
			if (this.MasteryId != null)
			{
				buffer.AddUniqueId(this.MasteryId.Value);
				buffer.AddFloat(this.NewMasteryLevel);
			}
			buffer.AddBool(this.AbilityId != null);
			if (this.AbilityId != null)
			{
				buffer.AddUniqueId(this.AbilityId.Value);
				buffer.AddFloat(this.NewAbilityLevel);
			}
			return buffer;
		}

		// Token: 0x06001495 RID: 5269 RVA: 0x000FB0D8 File Offset: 0x000F92D8
		public BitBuffer ReadData(BitBuffer buffer)
		{
			if (buffer.ReadBool())
			{
				this.MasteryId = new UniqueId?(buffer.ReadUniqueId());
				this.NewMasteryLevel = buffer.ReadFloat();
			}
			if (buffer.ReadBool())
			{
				this.AbilityId = new UniqueId?(buffer.ReadUniqueId());
				this.NewAbilityLevel = buffer.ReadFloat();
			}
			return buffer;
		}

		// Token: 0x04001CCF RID: 7375
		public UniqueId? MasteryId;

		// Token: 0x04001CD0 RID: 7376
		public float NewMasteryLevel;

		// Token: 0x04001CD1 RID: 7377
		public UniqueId? AbilityId;

		// Token: 0x04001CD2 RID: 7378
		public float NewAbilityLevel;
	}
}
