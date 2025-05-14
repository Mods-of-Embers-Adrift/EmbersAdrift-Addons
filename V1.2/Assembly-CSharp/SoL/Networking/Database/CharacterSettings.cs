using System;
using MongoDB.Bson.Serialization.Attributes;
using NetStack.Serialization;

namespace SoL.Networking.Database
{
	// Token: 0x02000437 RID: 1079
	[BsonIgnoreExtraElements]
	[Serializable]
	public class CharacterSettings : INetworkSerializable
	{
		// Token: 0x06001ED2 RID: 7890 RVA: 0x0011D60C File Offset: 0x0011B80C
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddBool(this.HideEmberStone);
			buffer.AddBool(this.HideHelm);
			buffer.AddBool(this.SecondaryWeaponsActive);
			buffer.AddBool(this.BlockInspections);
			buffer.AddBool(this.PauseAdventuringExperience);
			buffer.AddUniqueId(this.TrackedMastery);
			buffer.AddUniqueId(this.PortraitId);
			buffer.AddEnum(this.Presence);
			return buffer;
		}

		// Token: 0x06001ED3 RID: 7891 RVA: 0x0011D684 File Offset: 0x0011B884
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.HideEmberStone = buffer.ReadBool();
			this.HideHelm = buffer.ReadBool();
			this.SecondaryWeaponsActive = buffer.ReadBool();
			this.BlockInspections = buffer.ReadBool();
			this.PauseAdventuringExperience = buffer.ReadBool();
			this.TrackedMastery = buffer.ReadUniqueId();
			this.PortraitId = buffer.ReadUniqueId();
			this.Presence = buffer.ReadEnum<Presence>();
			return buffer;
		}

		// Token: 0x04002439 RID: 9273
		public bool HideEmberStone;

		// Token: 0x0400243A RID: 9274
		public bool HideHelm;

		// Token: 0x0400243B RID: 9275
		public bool SecondaryWeaponsActive;

		// Token: 0x0400243C RID: 9276
		public bool BlockInspections;

		// Token: 0x0400243D RID: 9277
		public bool PauseAdventuringExperience;

		// Token: 0x0400243E RID: 9278
		public UniqueId TrackedMastery;

		// Token: 0x0400243F RID: 9279
		public UniqueId PortraitId;

		// Token: 0x04002440 RID: 9280
		public Presence Presence;
	}
}
