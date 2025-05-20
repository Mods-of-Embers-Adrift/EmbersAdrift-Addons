using System;
using SoL.Game.Crafting;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A4A RID: 2634
	[Serializable]
	public class RangeOverride
	{
		// Token: 0x17001244 RID: 4676
		// (get) Token: 0x060051A1 RID: 20897 RVA: 0x000767D1 File Offset: 0x000749D1
		private string m_elementName
		{
			get
			{
				return this.Output.ToString();
			}
		}

		// Token: 0x040048E4 RID: 18660
		[SerializeField]
		public bool Enabled;

		// Token: 0x040048E5 RID: 18661
		[SerializeField]
		public MinMaxFloatRange Range;

		// Token: 0x040048E6 RID: 18662
		[HideInInspector]
		[SerializeField]
		public ItemAttributes.Names Input;

		// Token: 0x040048E7 RID: 18663
		[HideInInspector]
		[SerializeField]
		public ComponentEffectAssignerName Output;
	}
}
