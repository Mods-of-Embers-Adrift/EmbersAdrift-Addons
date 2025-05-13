using System;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A60 RID: 2656
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Consumables/NON-Stackable")]
	public class ConsumableItemNonStackable : ConsumableItem
	{
		// Token: 0x170012AE RID: 4782
		// (get) Token: 0x0600525D RID: 21085 RVA: 0x00062532 File Offset: 0x00060732
		protected override ReductionTaskType m_reductionTaskType
		{
			get
			{
				return ReductionTaskType.Consume;
			}
		}
	}
}
