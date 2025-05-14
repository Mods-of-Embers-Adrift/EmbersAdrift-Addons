using System;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AA6 RID: 2726
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Stackable")]
	public class StackableItem : ItemArchetype, IStackable
	{
		// Token: 0x1700135D RID: 4957
		// (get) Token: 0x06005447 RID: 21575 RVA: 0x00078596 File Offset: 0x00076796
		public int MaxStackCount
		{
			get
			{
				return this.m_maxStackCount;
			}
		}

		// Token: 0x1700135E RID: 4958
		// (get) Token: 0x06005448 RID: 21576 RVA: 0x0007859E File Offset: 0x0007679E
		public override bool CanPlaceInPouch
		{
			get
			{
				return this.m_canPlaceInPouch;
			}
		}

		// Token: 0x06005449 RID: 21577 RVA: 0x000785A6 File Offset: 0x000767A6
		public override bool IsAssignerHandled(ComponentEffectAssignerName assignerName)
		{
			return assignerName == ComponentEffectAssignerName.MaxStackCount || base.IsAssignerHandled(assignerName);
		}

		// Token: 0x0600544A RID: 21578 RVA: 0x000785B5 File Offset: 0x000767B5
		public override bool PopulateDynamicValue(ComponentEffectAssignerName assignerName, float value, ComponentEffectOutputType type, MinMaxFloatRange? rangeOverride)
		{
			if (assignerName == ComponentEffectAssignerName.MaxStackCount)
			{
				this.m_maxStackCount = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_maxStackCount);
				return true;
			}
			return base.PopulateDynamicValue(assignerName, value, type, rangeOverride);
		}

		// Token: 0x04004B05 RID: 19205
		[SerializeField]
		private bool m_canPlaceInPouch;

		// Token: 0x04004B06 RID: 19206
		[SerializeField]
		private int m_maxStackCount = 1;
	}
}
