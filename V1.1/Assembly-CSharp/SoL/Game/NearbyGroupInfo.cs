using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game
{
	// Token: 0x02000566 RID: 1382
	public struct NearbyGroupInfo : INetworkSerializable, IEquatable<NearbyGroupInfo>
	{
		// Token: 0x06002A3D RID: 10813 RVA: 0x0005D14C File Offset: 0x0005B34C
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddByte(this.GroupMembersNearby);
			buffer.AddByte(this.GroupedLevel);
			return buffer;
		}

		// Token: 0x06002A3E RID: 10814 RVA: 0x0005D169 File Offset: 0x0005B369
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.GroupMembersNearby = buffer.ReadByte();
			this.GroupedLevel = buffer.ReadByte();
			return buffer;
		}

		// Token: 0x06002A3F RID: 10815 RVA: 0x0005D184 File Offset: 0x0005B384
		public bool Equals(NearbyGroupInfo other)
		{
			return this.GroupMembersNearby == other.GroupMembersNearby && this.GroupedLevel == other.GroupedLevel;
		}

		// Token: 0x06002A40 RID: 10816 RVA: 0x00142C54 File Offset: 0x00140E54
		public override bool Equals(object obj)
		{
			if (obj is NearbyGroupInfo)
			{
				NearbyGroupInfo other = (NearbyGroupInfo)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06002A41 RID: 10817 RVA: 0x0005D1A4 File Offset: 0x0005B3A4
		public override int GetHashCode()
		{
			return HashCode.Combine<byte, byte>(this.GroupMembersNearby, this.GroupedLevel);
		}

		// Token: 0x06002A42 RID: 10818 RVA: 0x0005D1B7 File Offset: 0x0005B3B7
		public static bool operator ==(NearbyGroupInfo left, NearbyGroupInfo right)
		{
			return left.Equals(right);
		}

		// Token: 0x06002A43 RID: 10819 RVA: 0x0005D1C1 File Offset: 0x0005B3C1
		public static bool operator !=(NearbyGroupInfo left, NearbyGroupInfo right)
		{
			return !(left == right);
		}

		// Token: 0x04002AEC RID: 10988
		public byte GroupMembersNearby;

		// Token: 0x04002AED RID: 10989
		public byte GroupedLevel;
	}
}
