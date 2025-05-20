using System;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x02000882 RID: 2178
	[Serializable]
	public abstract class FilledWithEdge
	{
		// Token: 0x17000EAC RID: 3756
		// (get) Token: 0x06003F79 RID: 16249
		// (set) Token: 0x06003F7A RID: 16250
		public abstract float fillAmount { get; set; }

		// Token: 0x04003D48 RID: 15688
		[SerializeField]
		protected float m_fallbackWidth = 100f;

		// Token: 0x04003D49 RID: 15689
		[SerializeField]
		protected RectTransform m_parent;

		// Token: 0x04003D4A RID: 15690
		[SerializeField]
		protected Image m_edge;
	}
}
