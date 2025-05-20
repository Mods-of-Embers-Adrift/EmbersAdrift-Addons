using System;
using MongoDB.Bson.Serialization.Attributes;
using NetStack.Serialization;
using Newtonsoft.Json;
using SoL.Networking;
using SoL.Utilities;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A43 RID: 2627
	[Serializable]
	public class ItemAugment : INetworkSerializable, IPoolable
	{
		// Token: 0x17001226 RID: 4646
		// (get) Token: 0x0600515C RID: 20828 RVA: 0x001D041C File Offset: 0x001CE61C
		[BsonIgnore]
		[JsonIgnore]
		public IUtilityItem UtilityItem
		{
			get
			{
				IUtilityItem utilityItem;
				if (this.m_utilityItem == null && InternalGameDatabase.Archetypes.TryGetAsType<IUtilityItem>(this.ArchetypeId, out utilityItem))
				{
					this.m_utilityItem = utilityItem;
				}
				return this.m_utilityItem;
			}
		}

		// Token: 0x17001227 RID: 4647
		// (get) Token: 0x0600515D RID: 20829 RVA: 0x001D0454 File Offset: 0x001CE654
		[BsonIgnore]
		[JsonIgnore]
		public AugmentItem AugmentItemRef
		{
			get
			{
				AugmentItem augmentItem;
				if (this.m_augmentItem == null && InternalGameDatabase.Archetypes.TryGetAsType<AugmentItem>(this.ArchetypeId, out augmentItem))
				{
					this.m_augmentItem = augmentItem;
				}
				return this.m_augmentItem;
			}
		}

		// Token: 0x0600515E RID: 20830 RVA: 0x000764A7 File Offset: 0x000746A7
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.ArchetypeId);
			buffer.AddInt(this.Count);
			buffer.AddByte(this.StackCount);
			return buffer;
		}

		// Token: 0x0600515F RID: 20831 RVA: 0x000764D1 File Offset: 0x000746D1
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.ArchetypeId = buffer.ReadUniqueId();
			this.Count = buffer.ReadInt();
			this.StackCount = buffer.ReadByte();
			return buffer;
		}

		// Token: 0x06005160 RID: 20832 RVA: 0x000764F8 File Offset: 0x000746F8
		public BitBuffer PackDataBinary(BitBuffer buffer)
		{
			this.ArchetypeId.PackData_Binary(buffer);
			buffer.AddInt(this.Count);
			buffer.AddByte(this.StackCount);
			return buffer;
		}

		// Token: 0x06005161 RID: 20833 RVA: 0x00076522 File Offset: 0x00074722
		public BitBuffer ReadDataBinary(BitBuffer buffer)
		{
			this.ArchetypeId.ReadData_Binary(buffer, false);
			this.Count = buffer.ReadInt();
			this.StackCount = buffer.ReadByte();
			return buffer;
		}

		// Token: 0x06005162 RID: 20834 RVA: 0x0007654B File Offset: 0x0007474B
		void IPoolable.Reset()
		{
			this.ArchetypeId = UniqueId.Empty;
			this.Count = 0;
			this.StackCount = 1;
			this.m_utilityItem = null;
		}

		// Token: 0x17001228 RID: 4648
		// (get) Token: 0x06005163 RID: 20835 RVA: 0x0007656D File Offset: 0x0007476D
		// (set) Token: 0x06005164 RID: 20836 RVA: 0x00076575 File Offset: 0x00074775
		bool IPoolable.InPool
		{
			get
			{
				return this.m_inPool;
			}
			set
			{
				this.m_inPool = value;
			}
		}

		// Token: 0x040048A7 RID: 18599
		public UniqueId ArchetypeId;

		// Token: 0x040048A8 RID: 18600
		public int Count;

		// Token: 0x040048A9 RID: 18601
		public byte StackCount = 1;

		// Token: 0x040048AA RID: 18602
		[BsonIgnore]
		[JsonIgnore]
		private bool m_inPool;

		// Token: 0x040048AB RID: 18603
		[BsonIgnore]
		[JsonIgnore]
		private IUtilityItem m_utilityItem;

		// Token: 0x040048AC RID: 18604
		[BsonIgnore]
		[JsonIgnore]
		private AugmentItem m_augmentItem;
	}
}
