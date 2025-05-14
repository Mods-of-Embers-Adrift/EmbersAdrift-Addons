using System;
using NetStack.Serialization;
using SoL.Networking;
using SoL.Networking.Managers;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A67 RID: 2663
	public struct GroundTorchData : INetworkSerializable, IEquatable<GroundTorchData>
	{
		// Token: 0x06005282 RID: 21122 RVA: 0x00077122 File Offset: 0x00075322
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddString(this.SourceName);
			buffer.AddInt(this.Duration);
			buffer.AddDateTime(this.ExpirationTime);
			buffer.AddVector3(this.Position, NetworkManager.Range);
			return buffer;
		}

		// Token: 0x06005283 RID: 21123 RVA: 0x0007715E File Offset: 0x0007535E
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.SourceName = buffer.ReadString();
			this.Duration = buffer.ReadInt();
			this.ExpirationTime = buffer.ReadDateTime();
			this.Position = buffer.ReadVector3(NetworkManager.Range);
			return buffer;
		}

		// Token: 0x06005284 RID: 21124 RVA: 0x001D41C8 File Offset: 0x001D23C8
		public bool Equals(GroundTorchData other)
		{
			return this.SourceName == other.SourceName && this.Duration == other.Duration && this.ExpirationTime.Equals(other.ExpirationTime) && this.Position.Equals(other.Position);
		}

		// Token: 0x06005285 RID: 21125 RVA: 0x001D421C File Offset: 0x001D241C
		public override bool Equals(object obj)
		{
			if (obj is GroundTorchData)
			{
				GroundTorchData other = (GroundTorchData)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06005286 RID: 21126 RVA: 0x00077196 File Offset: 0x00075396
		public override int GetHashCode()
		{
			return HashCode.Combine<string, int, DateTime, Vector3>(this.SourceName, this.Duration, this.ExpirationTime, this.Position);
		}

		// Token: 0x040049C7 RID: 18887
		public string SourceName;

		// Token: 0x040049C8 RID: 18888
		public int Duration;

		// Token: 0x040049C9 RID: 18889
		public DateTime ExpirationTime;

		// Token: 0x040049CA RID: 18890
		public Vector3 Position;
	}
}
