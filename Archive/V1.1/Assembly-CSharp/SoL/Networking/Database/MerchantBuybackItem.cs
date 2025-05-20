using System;
using NetStack.Serialization;
using SoL.Game;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Player;
using SoL.Game.Transactions;
using SoL.Utilities;
using SoL.Utilities.Extensions;

namespace SoL.Networking.Database
{
	// Token: 0x02000439 RID: 1081
	[Serializable]
	public class MerchantBuybackItem : INetworkSerializable, IPoolable, IMerchantInventory
	{
		// Token: 0x06001ED6 RID: 7894 RVA: 0x00056D48 File Offset: 0x00054F48
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddDateTime(this.ExpirationTime);
			buffer.AddULong(this.Cost);
			this.Instance.PackData(buffer);
			return buffer;
		}

		// Token: 0x06001ED7 RID: 7895 RVA: 0x00056D72 File Offset: 0x00054F72
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.ExpirationTime = buffer.ReadDateTime();
			this.Cost = buffer.ReadULong();
			this.Instance = StaticPool<ArchetypeInstance>.GetFromPool();
			this.Instance.ReadData(buffer);
			return buffer;
		}

		// Token: 0x1700066F RID: 1647
		// (get) Token: 0x06001ED8 RID: 7896 RVA: 0x00056DA5 File Offset: 0x00054FA5
		// (set) Token: 0x06001ED9 RID: 7897 RVA: 0x00056DAD File Offset: 0x00054FAD
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

		// Token: 0x06001EDA RID: 7898 RVA: 0x00056DB6 File Offset: 0x00054FB6
		void IPoolable.Reset()
		{
			this.ExpirationTime = DateTime.MinValue;
			this.Cost = 0UL;
			this.Instance = null;
		}

		// Token: 0x17000670 RID: 1648
		// (get) Token: 0x06001EDB RID: 7899 RVA: 0x00056DD2 File Offset: 0x00054FD2
		BaseArchetype IMerchantInventory.Archetype
		{
			get
			{
				ArchetypeInstance instance = this.Instance;
				if (instance == null)
				{
					return null;
				}
				return instance.Archetype;
			}
		}

		// Token: 0x06001EDC RID: 7900 RVA: 0x00056DE5 File Offset: 0x00054FE5
		ulong IMerchantInventory.GetSellPrice(GameEntity entity)
		{
			return this.Cost;
		}

		// Token: 0x06001EDD RID: 7901 RVA: 0x00045BCD File Offset: 0x00043DCD
		ulong IMerchantInventory.GetEventCost(GameEntity entity)
		{
			return 0UL;
		}

		// Token: 0x06001EDE RID: 7902 RVA: 0x00056DED File Offset: 0x00054FED
		bool IMerchantInventory.EntityCanAcquire(GameEntity entity, out string errorMessage)
		{
			errorMessage = "NONE";
			return true;
		}

		// Token: 0x06001EDF RID: 7903 RVA: 0x0011D6D4 File Offset: 0x0011B8D4
		bool IMerchantInventory.AddToPlayer(GameEntity entity, ItemAddContext context, uint quantity, ItemFlags itemFlags, bool markAsSoulbound, out ArchetypeInstance instance)
		{
			instance = null;
			ItemArchetype archetype;
			return this.Instance != null && this.Instance.Archetype != null && this.Instance.Archetype.TryGetAsType(out archetype) && entity.CollectionController.TryAddItemToPlayer(archetype, ItemAddContext.Merchant, out instance, (int)quantity, -1, itemFlags, markAsSoulbound);
		}

		// Token: 0x04002443 RID: 9283
		public DateTime ExpirationTime;

		// Token: 0x04002444 RID: 9284
		public ulong Cost;

		// Token: 0x04002445 RID: 9285
		public ArchetypeInstance Instance;

		// Token: 0x04002446 RID: 9286
		private bool m_inPool;

		// Token: 0x04002447 RID: 9287
		private IMerchantInventory m_merchantInventoryImplementation;

		// Token: 0x04002448 RID: 9288
		private BaseArchetype m_archetype;
	}
}
