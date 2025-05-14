using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Interactives
{
	// Token: 0x02000BB4 RID: 2996
	public struct LightableCampfireData : INetworkSerializable, IEquatable<LightableCampfireData>
	{
		// Token: 0x06005CE4 RID: 23780 RVA: 0x0007E5B3 File Offset: 0x0007C7B3
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddBool(this.IsLit);
			if (this.IsLit)
			{
				buffer.AddInt(this.MasteryLevel);
				buffer.AddDateTime(this.Timestamp);
				buffer.AddString(this.LighterName);
			}
			return buffer;
		}

		// Token: 0x06005CE5 RID: 23781 RVA: 0x001F2380 File Offset: 0x001F0580
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.IsLit = buffer.ReadBool();
			if (this.IsLit)
			{
				this.MasteryLevel = buffer.ReadInt();
				this.Timestamp = buffer.ReadDateTime();
				this.LighterName = buffer.ReadString();
			}
			else
			{
				this.MasteryLevel = 0;
				this.Timestamp = DateTime.MinValue;
				this.LighterName = "";
			}
			return buffer;
		}

		// Token: 0x06005CE6 RID: 23782 RVA: 0x001F23E8 File Offset: 0x001F05E8
		public bool Equals(LightableCampfireData other)
		{
			return this.IsLit == other.IsLit && this.MasteryLevel == other.MasteryLevel && this.Timestamp.Equals(other.Timestamp) && this.LighterName == other.LighterName;
		}

		// Token: 0x06005CE7 RID: 23783 RVA: 0x001F2438 File Offset: 0x001F0638
		public override bool Equals(object obj)
		{
			if (obj is LightableCampfireData)
			{
				LightableCampfireData other = (LightableCampfireData)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06005CE8 RID: 23784 RVA: 0x0007E5F2 File Offset: 0x0007C7F2
		public override int GetHashCode()
		{
			return HashCode.Combine<bool, int, DateTime, string>(this.IsLit, this.MasteryLevel, this.Timestamp, this.LighterName);
		}

		// Token: 0x04005054 RID: 20564
		public bool IsLit;

		// Token: 0x04005055 RID: 20565
		public int MasteryLevel;

		// Token: 0x04005056 RID: 20566
		public DateTime Timestamp;

		// Token: 0x04005057 RID: 20567
		public string LighterName;
	}
}
