using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Loot
{
	// Token: 0x02000B0D RID: 2829
	public struct LootRollMember : INetworkSerializable, IEquatable<LootRollMember>
	{
		// Token: 0x06005744 RID: 22340 RVA: 0x001E3428 File Offset: 0x001E1628
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddString(this.PlayerName);
			buffer.AddEnum(this.Choice);
			buffer.AddBool(this.Roll != null);
			if (this.Roll != null)
			{
				buffer.AddInt(this.Roll.Value);
			}
			return buffer;
		}

		// Token: 0x06005745 RID: 22341 RVA: 0x0007A23A File Offset: 0x0007843A
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.PlayerName = buffer.ReadString();
			this.Choice = buffer.ReadEnum<LootRollChoice>();
			if (buffer.ReadBool())
			{
				this.Roll = new int?(buffer.ReadInt());
			}
			return buffer;
		}

		// Token: 0x06005746 RID: 22342 RVA: 0x001E3484 File Offset: 0x001E1684
		public bool Equals(LootRollMember other)
		{
			if (string.Equals(this.PlayerName, other.PlayerName) && this.Choice == other.Choice)
			{
				int? roll = this.Roll;
				int? roll2 = other.Roll;
				return roll.GetValueOrDefault() == roll2.GetValueOrDefault() & roll != null == (roll2 != null);
			}
			return false;
		}

		// Token: 0x06005747 RID: 22343 RVA: 0x001E34E4 File Offset: 0x001E16E4
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is LootRollMember)
			{
				LootRollMember other = (LootRollMember)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06005748 RID: 22344 RVA: 0x0007A26E File Offset: 0x0007846E
		public override int GetHashCode()
		{
			return (((this.PlayerName != null) ? this.PlayerName.GetHashCode() : 0) * 397 ^ (int)this.Choice) * 397 ^ this.Roll.GetHashCode();
		}

		// Token: 0x06005749 RID: 22345 RVA: 0x0007A2AB File Offset: 0x000784AB
		public static bool operator ==(LootRollMember a, LootRollMember b)
		{
			return a.Equals(b);
		}

		// Token: 0x0600574A RID: 22346 RVA: 0x0007A2B5 File Offset: 0x000784B5
		public static bool operator !=(LootRollMember a, LootRollMember b)
		{
			return !(a == b);
		}

		// Token: 0x04004D07 RID: 19719
		public GameEntity Player;

		// Token: 0x04004D08 RID: 19720
		public string PlayerName;

		// Token: 0x04004D09 RID: 19721
		public LootRollChoice Choice;

		// Token: 0x04004D0A RID: 19722
		public int? Roll;

		// Token: 0x04004D0B RID: 19723
		public int Index;
	}
}
