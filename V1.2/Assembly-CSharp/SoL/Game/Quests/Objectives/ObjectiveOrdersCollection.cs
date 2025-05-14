using System;
using System.Collections.Generic;
using SoL.Utilities;

namespace SoL.Game.Quests.Objectives
{
	// Token: 0x020007AC RID: 1964
	public class ObjectiveOrdersCollection
	{
		// Token: 0x060039D4 RID: 14804 RVA: 0x00067336 File Offset: 0x00065536
		public bool HasOrders<T>()
		{
			return this.m_orders.ContainsKey(typeof(T)) && this.m_orders[typeof(T)].Count > 0;
		}

		// Token: 0x060039D5 RID: 14805 RVA: 0x00174AD8 File Offset: 0x00172CD8
		public void Add<T>(UniqueId parentId, T objective) where T : OrderDrivenObjective<T>
		{
			if (!this.m_orders.ContainsKey(typeof(T)))
			{
				this.m_orders.Add(typeof(T), new List<ObjectiveOrder>
				{
					new ObjectiveOrder
					{
						ParentId = parentId,
						Objective = objective
					}
				});
				return;
			}
			this.m_orders[typeof(T)].Add(new ObjectiveOrder
			{
				ParentId = parentId,
				Objective = objective
			});
		}

		// Token: 0x060039D6 RID: 14806 RVA: 0x00174B74 File Offset: 0x00172D74
		public int Remove<T>(UniqueId parentId, T objective) where T : OrderDrivenObjective<T>
		{
			if (this.m_orders.ContainsKey(typeof(T)))
			{
				return this.m_orders[typeof(T)].RemoveAll((ObjectiveOrder x) => x.ParentId == parentId && x.Objective == objective);
			}
			return 0;
		}

		// Token: 0x060039D7 RID: 14807 RVA: 0x00174BD4 File Offset: 0x00172DD4
		public List<ValueTuple<UniqueId, T>> GetPooledOrderList<T>() where T : OrderDrivenObjective<T>
		{
			List<ValueTuple<UniqueId, T>> fromPool = StaticListPool<ValueTuple<UniqueId, T>>.GetFromPool();
			if (this.m_orders.ContainsKey(typeof(T)))
			{
				foreach (ObjectiveOrder objectiveOrder in this.m_orders[typeof(T)])
				{
					fromPool.Add(new ValueTuple<UniqueId, T>(objectiveOrder.ParentId, objectiveOrder.Objective as T));
				}
			}
			return fromPool;
		}

		// Token: 0x060039D8 RID: 14808 RVA: 0x0006736E File Offset: 0x0006556E
		public void ReturnPooledOrderList<T>(List<ValueTuple<UniqueId, T>> list) where T : OrderDrivenObjective<T>
		{
			StaticListPool<ValueTuple<UniqueId, T>>.ReturnToPool(list);
		}

		// Token: 0x04003875 RID: 14453
		private readonly Dictionary<Type, List<ObjectiveOrder>> m_orders = new Dictionary<Type, List<ObjectiveOrder>>();
	}
}
