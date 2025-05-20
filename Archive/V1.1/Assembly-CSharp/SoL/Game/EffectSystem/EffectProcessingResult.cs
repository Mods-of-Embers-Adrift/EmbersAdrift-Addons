using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C3C RID: 3132
	public struct EffectProcessingResult : INetworkSerializable
	{
		// Token: 0x060060B3 RID: 24755 RVA: 0x001FDE9C File Offset: 0x001FC09C
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUInt(this.SourceId);
			buffer.AddUInt(this.TargetId);
			buffer.AddFloat(this.DamageDone);
			buffer.AddFloat(this.DamageAbsorbed);
			buffer.AddFloat(this.Threat);
			buffer.AddEnum(this.Flags);
			buffer.AddUniqueId(this.ArchetypeId);
			return buffer;
		}

		// Token: 0x060060B4 RID: 24756 RVA: 0x001FDF08 File Offset: 0x001FC108
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.SourceId = buffer.ReadUInt();
			this.TargetId = buffer.ReadUInt();
			this.DamageDone = buffer.ReadFloat();
			this.DamageAbsorbed = buffer.ReadFloat();
			this.Threat = buffer.ReadFloat();
			this.Flags = buffer.ReadEnum<EffectApplicationFlags>();
			this.ArchetypeId = buffer.ReadUniqueId();
			return buffer;
		}

		// Token: 0x0400533B RID: 21307
		public uint SourceId;

		// Token: 0x0400533C RID: 21308
		public uint TargetId;

		// Token: 0x0400533D RID: 21309
		public float DamageDone;

		// Token: 0x0400533E RID: 21310
		public float DamageAbsorbed;

		// Token: 0x0400533F RID: 21311
		public float Threat;

		// Token: 0x04005340 RID: 21312
		public EffectApplicationFlags Flags;

		// Token: 0x04005341 RID: 21313
		public UniqueId ArchetypeId;
	}
}
