using System;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A61 RID: 2657
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Consumables/Stackable")]
	public class ConsumableItemStackable : ConsumableItem, IStackable
	{
		// Token: 0x170012AF RID: 4783
		// (get) Token: 0x0600525F RID: 21087 RVA: 0x0004479C File Offset: 0x0004299C
		protected override ReductionTaskType m_reductionTaskType
		{
			get
			{
				return ReductionTaskType.Count;
			}
		}

		// Token: 0x06005260 RID: 21088 RVA: 0x001D35DC File Offset: 0x001D17DC
		protected override bool ExecutionCheckInternal(ExecutionCache executionCache)
		{
			if (executionCache == null)
			{
				return false;
			}
			if (executionCache.Instance == null || executionCache.Instance.ItemData == null)
			{
				executionCache.Message = "Invalid item!";
				return false;
			}
			if (executionCache.Instance.ItemData.Count == null || executionCache.Instance.ItemData.Count.Value < 1)
			{
				executionCache.Message = "Invalid count!";
				return false;
			}
			return base.ExecutionCheckInternal(executionCache);
		}

		// Token: 0x170012B0 RID: 4784
		// (get) Token: 0x06005261 RID: 21089 RVA: 0x00076F9F File Offset: 0x0007519F
		int IStackable.MaxStackCount
		{
			get
			{
				return this.m_maxStackCount;
			}
		}

		// Token: 0x06005262 RID: 21090 RVA: 0x00076FA7 File Offset: 0x000751A7
		public override bool IsAssignerHandled(ComponentEffectAssignerName assignerName)
		{
			return assignerName == ComponentEffectAssignerName.MaxStackCount || base.IsAssignerHandled(assignerName);
		}

		// Token: 0x06005263 RID: 21091 RVA: 0x00076FB6 File Offset: 0x000751B6
		public override bool PopulateDynamicValue(ComponentEffectAssignerName assignerName, float value, ComponentEffectOutputType type, MinMaxFloatRange? rangeOverride)
		{
			if (assignerName == ComponentEffectAssignerName.MaxStackCount)
			{
				this.m_maxStackCount = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_maxStackCount);
				return true;
			}
			return base.PopulateDynamicValue(assignerName, value, type, rangeOverride);
		}

		// Token: 0x040049A9 RID: 18857
		[SerializeField]
		private int m_maxStackCount = 1;
	}
}
