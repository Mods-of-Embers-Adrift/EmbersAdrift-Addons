using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C66 RID: 3174
	public struct EffectKey : IEquatable<EffectKey>
	{
		// Token: 0x06006141 RID: 24897 RVA: 0x001FF5C4 File Offset: 0x001FD7C4
		public EffectKey(EffectRecord record)
		{
			if (record == null)
			{
				throw new ArgumentNullException("record");
			}
			this.ArchetypeId = record.ArchetypeId;
			this.ApplicantName = record.SourceData.Name;
			this.NetworkId = ((record.SourceData.Type == GameEntityType.Npc) ? record.SourceNetworkId : 0U);
		}

		// Token: 0x06006142 RID: 24898 RVA: 0x000818D2 File Offset: 0x0007FAD2
		public bool Equals(EffectKey other)
		{
			return this.ArchetypeId.Equals(other.ArchetypeId) && this.ApplicantName == other.ApplicantName && this.NetworkId == other.NetworkId;
		}

		// Token: 0x06006143 RID: 24899 RVA: 0x001FF620 File Offset: 0x001FD820
		public override bool Equals(object obj)
		{
			if (obj is EffectKey)
			{
				EffectKey other = (EffectKey)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06006144 RID: 24900 RVA: 0x0008190A File Offset: 0x0007FB0A
		public override int GetHashCode()
		{
			return HashCode.Combine<UniqueId, string, uint>(this.ArchetypeId, this.ApplicantName, this.NetworkId);
		}

		// Token: 0x04005473 RID: 21619
		public UniqueId ArchetypeId;

		// Token: 0x04005474 RID: 21620
		public string ApplicantName;

		// Token: 0x04005475 RID: 21621
		public uint NetworkId;
	}
}
