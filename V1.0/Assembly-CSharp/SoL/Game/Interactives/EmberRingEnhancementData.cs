using System;
using NetStack.Serialization;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Networking;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B87 RID: 2951
	public struct EmberRingEnhancementData : INetworkSerializable, IEquatable<EmberRingEnhancementData>
	{
		// Token: 0x1700154B RID: 5451
		// (get) Token: 0x06005AFC RID: 23292 RVA: 0x0007D1A8 File Offset: 0x0007B3A8
		public ConsumableItemEmberRingEnhancment Item
		{
			get
			{
				if (this.m_item == null && !this.ItemId.IsEmpty)
				{
					InternalGameDatabase.Archetypes.TryGetAsType<ConsumableItemEmberRingEnhancment>(this.ItemId, out this.m_item);
				}
				return this.m_item;
			}
		}

		// Token: 0x06005AFD RID: 23293 RVA: 0x001EDC80 File Offset: 0x001EBE80
		public EmberRingEnhancementData(GameEntity source, ConsumableItemEmberRingEnhancment item)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			this.SourceName = (source.CharacterData ? source.CharacterData.Name.Value : "Unknown");
			this.ExpirationTime = DateTime.UtcNow.AddSeconds((double)item.Duration);
			this.ItemId = item.Id;
			this.m_item = item;
		}

		// Token: 0x06005AFE RID: 23294 RVA: 0x001EDD0C File Offset: 0x001EBF0C
		public BitBuffer PackData(BitBuffer buffer)
		{
			bool flag = this.m_item != null;
			buffer.AddBool(flag);
			if (flag)
			{
				buffer.AddString(this.SourceName);
				buffer.AddDateTime(this.ExpirationTime);
				buffer.AddUniqueId(this.ItemId);
			}
			return buffer;
		}

		// Token: 0x06005AFF RID: 23295 RVA: 0x0007D1E2 File Offset: 0x0007B3E2
		public BitBuffer ReadData(BitBuffer buffer)
		{
			if (buffer.ReadBool())
			{
				this.SourceName = buffer.ReadString();
				this.ExpirationTime = buffer.ReadDateTime();
				this.ItemId = buffer.ReadUniqueId();
			}
			return buffer;
		}

		// Token: 0x06005B00 RID: 23296 RVA: 0x0007D211 File Offset: 0x0007B411
		public bool Equals(EmberRingEnhancementData other)
		{
			return this.SourceName == other.SourceName && this.ExpirationTime.Equals(other.ExpirationTime) && this.ItemId.Equals(other.ItemId);
		}

		// Token: 0x06005B01 RID: 23297 RVA: 0x001EDD5C File Offset: 0x001EBF5C
		public override bool Equals(object obj)
		{
			if (obj is EmberRingEnhancementData)
			{
				EmberRingEnhancementData other = (EmberRingEnhancementData)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06005B02 RID: 23298 RVA: 0x0007D24C File Offset: 0x0007B44C
		public override int GetHashCode()
		{
			return HashCode.Combine<string, DateTime, UniqueId>(this.SourceName, this.ExpirationTime, this.ItemId);
		}

		// Token: 0x04004FA1 RID: 20385
		public string SourceName;

		// Token: 0x04004FA2 RID: 20386
		public DateTime ExpirationTime;

		// Token: 0x04004FA3 RID: 20387
		public UniqueId ItemId;

		// Token: 0x04004FA4 RID: 20388
		private ConsumableItemEmberRingEnhancment m_item;
	}
}
