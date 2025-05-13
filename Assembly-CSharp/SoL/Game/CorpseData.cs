using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game
{
	// Token: 0x0200056A RID: 1386
	public struct CorpseData : INetworkSerializable, IEquatable<CorpseData>
	{
		// Token: 0x06002ACF RID: 10959 RVA: 0x0005D9CF File Offset: 0x0005BBCF
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddString(this.CharacterId);
			buffer.AddString(this.CharacterName);
			buffer.AddDateTime(this.TimeCreated);
			return buffer;
		}

		// Token: 0x06002AD0 RID: 10960 RVA: 0x0005D9F9 File Offset: 0x0005BBF9
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.CharacterId = buffer.ReadString();
			this.CharacterName = buffer.ReadString();
			this.TimeCreated = buffer.ReadDateTime();
			return buffer;
		}

		// Token: 0x06002AD1 RID: 10961 RVA: 0x0005DA20 File Offset: 0x0005BC20
		public bool Equals(CorpseData other)
		{
			return this.CharacterName == other.CharacterName && this.CharacterId == other.CharacterId && this.TimeCreated.Equals(other.TimeCreated);
		}

		// Token: 0x06002AD2 RID: 10962 RVA: 0x00144CA0 File Offset: 0x00142EA0
		public override bool Equals(object obj)
		{
			if (obj is CorpseData)
			{
				CorpseData other = (CorpseData)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06002AD3 RID: 10963 RVA: 0x0005DA5B File Offset: 0x0005BC5B
		public override int GetHashCode()
		{
			return HashCode.Combine<string, string, DateTime>(this.CharacterName, this.CharacterId, this.TimeCreated);
		}

		// Token: 0x04002B15 RID: 11029
		public string CharacterName;

		// Token: 0x04002B16 RID: 11030
		public string CharacterId;

		// Token: 0x04002B17 RID: 11031
		public DateTime TimeCreated;
	}
}
