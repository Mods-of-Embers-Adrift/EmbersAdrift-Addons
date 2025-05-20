using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A75 RID: 2677
	public struct EquipableItemVisualData : INetworkSerializable, IEquatable<EquipableItemVisualData>
	{
		// Token: 0x060052D9 RID: 21209 RVA: 0x001D5920 File Offset: 0x001D3B20
		public EquipableItemVisualData(ArchetypeInstance instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			this.ArchetypeId = instance.ArchetypeId;
			ItemInstanceData itemData = instance.ItemData;
			this.VisualIndex = ((itemData != null) ? itemData.VisualIndex : null);
			ItemInstanceData itemData2 = instance.ItemData;
			this.ColorIndex = ((itemData2 != null) ? itemData2.ColorIndex : null);
		}

		// Token: 0x060052DA RID: 21210 RVA: 0x00077416 File Offset: 0x00075616
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.ArchetypeId);
			buffer.AddNullableByte(this.VisualIndex);
			buffer.AddNullableByte(this.ColorIndex);
			return buffer;
		}

		// Token: 0x060052DB RID: 21211 RVA: 0x00077440 File Offset: 0x00075640
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.ArchetypeId = buffer.ReadUniqueId();
			this.VisualIndex = buffer.ReadNullableByte();
			this.ColorIndex = buffer.ReadNullableByte();
			return buffer;
		}

		// Token: 0x060052DC RID: 21212 RVA: 0x001D5988 File Offset: 0x001D3B88
		public bool Equals(EquipableItemVisualData other)
		{
			if (this.ArchetypeId.Equals(other.ArchetypeId))
			{
				byte? b = this.VisualIndex;
				int? num = (b != null) ? new int?((int)b.GetValueOrDefault()) : null;
				b = other.VisualIndex;
				int? num2 = (b != null) ? new int?((int)b.GetValueOrDefault()) : null;
				if (num.GetValueOrDefault() == num2.GetValueOrDefault() & num != null == (num2 != null))
				{
					b = this.ColorIndex;
					num2 = ((b != null) ? new int?((int)b.GetValueOrDefault()) : null);
					b = other.ColorIndex;
					num = ((b != null) ? new int?((int)b.GetValueOrDefault()) : null);
					return num2.GetValueOrDefault() == num.GetValueOrDefault() & num2 != null == (num != null);
				}
			}
			return false;
		}

		// Token: 0x060052DD RID: 21213 RVA: 0x001D5A94 File Offset: 0x001D3C94
		public override bool Equals(object obj)
		{
			if (obj is EquipableItemVisualData)
			{
				EquipableItemVisualData other = (EquipableItemVisualData)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x060052DE RID: 21214 RVA: 0x001D5ABC File Offset: 0x001D3CBC
		public override int GetHashCode()
		{
			return (this.ArchetypeId.GetHashCode() * 397 ^ this.VisualIndex.GetHashCode()) * 397 ^ this.ColorIndex.GetHashCode();
		}

		// Token: 0x060052DF RID: 21215 RVA: 0x00077467 File Offset: 0x00075667
		public static bool operator ==(EquipableItemVisualData left, EquipableItemVisualData right)
		{
			return left.Equals(right);
		}

		// Token: 0x060052E0 RID: 21216 RVA: 0x00077471 File Offset: 0x00075671
		public static bool operator !=(EquipableItemVisualData left, EquipableItemVisualData right)
		{
			return !(left == right);
		}

		// Token: 0x060052E1 RID: 21217 RVA: 0x001D5B0C File Offset: 0x001D3D0C
		public override string ToString()
		{
			string text = (this.VisualIndex != null) ? this.VisualIndex.Value.ToString() : -1.ToString();
			string text2 = (this.ColorIndex != null) ? this.ColorIndex.Value.ToString() : -1.ToString();
			return string.Concat(new string[]
			{
				this.ArchetypeId.ToString(),
				", VI=",
				text,
				", CI=",
				text2
			});
		}

		// Token: 0x040049F9 RID: 18937
		public UniqueId ArchetypeId;

		// Token: 0x040049FA RID: 18938
		public byte? VisualIndex;

		// Token: 0x040049FB RID: 18939
		public byte? ColorIndex;
	}
}
