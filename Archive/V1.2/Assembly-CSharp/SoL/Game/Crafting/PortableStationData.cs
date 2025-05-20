using System;
using NetStack.Serialization;
using SoL.Networking;
using SoL.Networking.Managers;
using UnityEngine;

namespace SoL.Game.Crafting
{
	// Token: 0x02000CE0 RID: 3296
	public struct PortableStationData : INetworkSerializable, IEquatable<PortableStationData>
	{
		// Token: 0x060063E1 RID: 25569 RVA: 0x00083364 File Offset: 0x00081564
		BitBuffer INetworkSerializable.PackData(BitBuffer buffer)
		{
			buffer.AddString(this.SourceName);
			buffer.AddDateTime(this.PlacementTime);
			buffer.AddVector3(this.Position, NetworkManager.Range);
			return buffer;
		}

		// Token: 0x060063E2 RID: 25570 RVA: 0x00083393 File Offset: 0x00081593
		BitBuffer INetworkSerializable.ReadData(BitBuffer buffer)
		{
			this.SourceName = buffer.ReadString();
			this.PlacementTime = buffer.ReadDateTime();
			this.Position = buffer.ReadVector3(NetworkManager.Range);
			return buffer;
		}

		// Token: 0x060063E3 RID: 25571 RVA: 0x000833BF File Offset: 0x000815BF
		public bool Equals(PortableStationData other)
		{
			return this.SourceName == other.SourceName && this.PlacementTime.Equals(other.PlacementTime) && this.Position.Equals(other.Position);
		}

		// Token: 0x060063E4 RID: 25572 RVA: 0x00207D74 File Offset: 0x00205F74
		public override bool Equals(object obj)
		{
			if (obj is PortableStationData)
			{
				PortableStationData other = (PortableStationData)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x060063E5 RID: 25573 RVA: 0x000833FA File Offset: 0x000815FA
		public override int GetHashCode()
		{
			return HashCode.Combine<string, DateTime, Vector3>(this.SourceName, this.PlacementTime, this.Position);
		}

		// Token: 0x040056BC RID: 22204
		public string SourceName;

		// Token: 0x040056BD RID: 22205
		public DateTime PlacementTime;

		// Token: 0x040056BE RID: 22206
		public Vector3 Position;
	}
}
