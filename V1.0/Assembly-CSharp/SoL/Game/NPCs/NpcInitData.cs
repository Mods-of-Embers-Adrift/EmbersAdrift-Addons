using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.NPCs
{
	// Token: 0x02000803 RID: 2051
	public struct NpcInitData : INetworkSerializable, IEquatable<NpcInitData>
	{
		// Token: 0x06003B74 RID: 15220 RVA: 0x0017B5C0 File Offset: 0x001797C0
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.ProfileType);
			buffer.AddUniqueId(this.EnsembleId);
			buffer.AddUniqueId(this.ProfileId);
			buffer.AddUniqueId(this.OverrideDialogueId);
			buffer.AddBool(this.BypassLevelDeltaCombatAdjustments);
			return buffer;
		}

		// Token: 0x06003B75 RID: 15221 RVA: 0x0006837A File Offset: 0x0006657A
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.ProfileType = buffer.ReadEnum<VisualProfileType>();
			this.EnsembleId = buffer.ReadUniqueId();
			this.ProfileId = buffer.ReadUniqueId();
			this.OverrideDialogueId = buffer.ReadUniqueId();
			this.BypassLevelDeltaCombatAdjustments = buffer.ReadBool();
			return buffer;
		}

		// Token: 0x06003B76 RID: 15222 RVA: 0x0017B610 File Offset: 0x00179810
		public bool Equals(NpcInitData other)
		{
			return this.ProfileType.Equals(other.ProfileType) && this.EnsembleId.Equals(other.EnsembleId) && this.ProfileId.Equals(other.ProfileId) && this.OverrideDialogueId.Equals(other.OverrideDialogueId) && this.BypassLevelDeltaCombatAdjustments.Equals(other.BypassLevelDeltaCombatAdjustments);
		}

		// Token: 0x06003B77 RID: 15223 RVA: 0x0017B688 File Offset: 0x00179888
		public override bool Equals(object obj)
		{
			if (obj is NpcInitData)
			{
				NpcInitData other = (NpcInitData)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06003B78 RID: 15224 RVA: 0x000683B9 File Offset: 0x000665B9
		public override int GetHashCode()
		{
			return HashCode.Combine<VisualProfileType, UniqueId, UniqueId, UniqueId, bool>(this.ProfileType, this.EnsembleId, this.ProfileId, this.OverrideDialogueId, this.BypassLevelDeltaCombatAdjustments);
		}

		// Token: 0x040039D4 RID: 14804
		public VisualProfileType ProfileType;

		// Token: 0x040039D5 RID: 14805
		public UniqueId EnsembleId;

		// Token: 0x040039D6 RID: 14806
		public UniqueId ProfileId;

		// Token: 0x040039D7 RID: 14807
		public UniqueId OverrideDialogueId;

		// Token: 0x040039D8 RID: 14808
		public bool BypassLevelDeltaCombatAdjustments;
	}
}
