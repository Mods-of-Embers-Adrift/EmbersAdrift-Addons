using System;
using System.Collections;
using SoL.Game.Transactions;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000ABB RID: 2747
	public class LearnableArchetype : BaseArchetype, IMerchantInventory
	{
		// Token: 0x17001385 RID: 4997
		// (get) Token: 0x060054E4 RID: 21732 RVA: 0x000580DD File Offset: 0x000562DD
		public override ArchetypeCategory Category
		{
			get
			{
				return ArchetypeCategory.Learnable;
			}
		}

		// Token: 0x17001386 RID: 4998
		// (get) Token: 0x060054E5 RID: 21733 RVA: 0x00078C37 File Offset: 0x00076E37
		public bool Teachable
		{
			get
			{
				return this.m_teachable;
			}
		}

		// Token: 0x17001387 RID: 4999
		// (get) Token: 0x060054E6 RID: 21734 RVA: 0x0004F9FB File Offset: 0x0004DBFB
		private IEnumerable GetColorValues
		{
			get
			{
				return SolOdinUtilities.GetColorValues();
			}
		}

		// Token: 0x17001388 RID: 5000
		// (get) Token: 0x060054E7 RID: 21735 RVA: 0x00078C3F File Offset: 0x00076E3F
		public override Color IconTint
		{
			get
			{
				return this.m_iconTint;
			}
		}

		// Token: 0x17001389 RID: 5001
		// (get) Token: 0x060054E8 RID: 21736 RVA: 0x0004BC2B File Offset: 0x00049E2B
		public BaseArchetype Archetype
		{
			get
			{
				return this;
			}
		}

		// Token: 0x060054E9 RID: 21737 RVA: 0x00045BCD File Offset: 0x00043DCD
		public virtual ulong GetSellPrice(GameEntity entity)
		{
			return 0UL;
		}

		// Token: 0x060054EA RID: 21738 RVA: 0x00045BCD File Offset: 0x00043DCD
		ulong IMerchantInventory.GetEventCost(GameEntity entity)
		{
			return 0UL;
		}

		// Token: 0x060054EB RID: 21739 RVA: 0x00048A92 File Offset: 0x00046C92
		public virtual bool EntityCanAcquire(GameEntity entity, out string errorMessage)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060054EC RID: 21740 RVA: 0x00048A92 File Offset: 0x00046C92
		public virtual bool AddToPlayer(GameEntity entity, ItemAddContext context, uint quantity, ItemFlags itemFlags, bool markAsSoulbound, out ArchetypeInstance resultingInstance)
		{
			throw new NotImplementedException();
		}

		// Token: 0x04004B69 RID: 19305
		[SerializeField]
		private bool m_teachable;

		// Token: 0x04004B6A RID: 19306
		[SerializeField]
		private Color m_iconTint = Color.white;
	}
}
